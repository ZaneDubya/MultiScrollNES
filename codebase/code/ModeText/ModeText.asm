.require "../includes.asm"
{

; =============================================================================
;   TextEngine_DisplayMsg: Setup routine for Text Engine. Called from anywhere.
;   In:     A is the lo byte of the index of the statement to load
;           Y is the hi byte of the index of the statement to load (bank)
;   Out:    Sets up TextEngine
;   Notes:  Currently Y is the bank, but this should be changed so that the
;           Text Engine can determine which bank the requested statement is in.
TextEngine_DisplayMsg:
    sta TEXT_StatementLo
    sty TEXT_StatementHi
    .invoke SetByte GameMode, GameModeText
    .invoke SetByte TEXT_Action, TEA_DisplayMsg
    .invoke SetByte TEXT_Stage, 0
    rts

; =============================================================================
;   TextEngine: The Main routine for Text Engine. Called from GoMainRoutine.
;   In:     TEXT_Stage
;   Out:    
;   Notes:  TextEngine sets up for display in 4 frames.
;           0: translate string, get list of chars
;           1: create nametable and attribute table
;           3: write text nametable/attributetable to vram
;           4: wait for user to press a
;           5: restore control to world
TextEngine:
    lda TEXT_Stage
    jsr ChooseRoutine                       ; Use TEXT_Stage as index into routine table below.

.word TextEngine0, TextEngine1, TextEngine3, TextEngine4, TextEngine5

TextEngine0:
    jsr _TextEngine_LoadStringToMemory      ; translate string and get list of chars used
    ;.invoke AddPPUOperation PPUOp_SetAddress, $0d, $00
    ldy #00                                 ; Y = 0
    lda TEXT_NumChars                       ; A = NumChars
    cmp #20                                 ; IF NumChars >= 20
    bcs +                                   ; THEN load 20
    tax                                     ; ELSE load NumChars
    bcs ++                                  ;   +
*   ldx #20                                 ; Load 20
*   ;.invoke AddPPUOperationXY PPUOp_ChrCopy
    inc TEXT_Stage
    rts

TextEngine1:
    ; jsr _TextEngine_CreateTables !!!
    lda TEXT_NumChars                       ; A = NumChars
    cmp #20                                 ; IF NumChars <= 20
    bcs +                                   ; THEN do not load any more chars
    rts
*   sec                                     ; X = (numchars - 20) ; load this many chars
    lda TEXT_NumChars                       ;   |
    sbc #20                                 ;   |
    tax                                     ;   +
    ldy #20                                 ; Y = 20 ; start at this tile
    inc TEXT_Stage
    rts
    
TextEngine3:
    ; jsr _TextEngine_PrepareVblankTableWrite !!!!
    .invoke SetGameFlag FlagDialogueOpen
    inc TEXT_Stage
    rts

TextEngine4:
    lda Ctrl0_New
    bne +
    rts
*   and #CTRL_A
    bne +
    rts
*   inc TEXT_Stage
    rts

TextEngine5:
    .invoke ClearGameFlag FlagDialogueOpen
    .invoke SetByte GameMode, GameModeWorld
*   rts

; =============================================================================
;   TextEngine_LoadString: Loads a string
;   In:     Index of statement to load is in TEXT_StatementLo & TEXT_StatementHi
;   Out:    Writes string and list of chars used by string
;   Notes:  A,X,Y are wiped out
;           10 temp bytes in zp
;           For very long AND complicated strings, this will take two frames,
;               and Main won't be called next frame.
_TextEngine_LoadStringToMemory:
.scope
; temp variables in zp
.alias  _stringLength       $00
.alias  _numChars           $01
.alias  _Recursion          $02
.alias  _TempA              $03
.alias  _StringPtr          $04
.alias  _BPE_Left           $06
.alias  _BPE_Right          $08
; these constants are true for strings with characters from 0x00 - 0x60
.alias  _HighR              $ff
.alias  _LowR               $61
; Load the string and decompress it into RAM
    ldx #%01000111                          ; load bank in TEXT_StatementHi into $a000
    stx $8000                               ;   |
    ldy TEXT_StatementHi                    ;   |
    sty $8001                               ;   +
    lda TEXT_StatementLo                    ; Load Index into A
    asl                                     ; Multiply Index A by 2; ptr is 2 bytes
    tax                                     ;   +
    bcc +                                   ; IF carry, then we need to add $100 to address
    lda $a100,x                             ; Get the address of the string ...
    ldy $a101,x                             ;   |
    bcs ++
*   lda $a000,x                             ; Get the address of the string and save it.
    ldy $a001,x                             ;   |
*   sta _StringPtr                          ;   |
    sty _StringPtr+1                        ;   +
    lda #<[$c000-[_HighR-_LowR+1]*2]        ; Get pointer to BPE_Left
    sta _BPE_Left                           ;   |
    lda #>[$c000-[_HighR-_LowR+1]*2]        ;   |
    sta _BPE_Left+1                         ;   +
    lda #<[$c000-[_HighR-_LowR+1]]          ; Get pointer to BPE_Right
    sta _BPE_Right                          ;   |
    lda #>[$c000-[_HighR-_LowR+1]]          ;   |
    sta _BPE_Right+1                        ;   +
    ldx #$01                                ; X is index of current decoded byte
    ldy #$00                                ; Y is index of current encoded byte
    sty _Recursion                          ; Recursion = 0
    lda (_StringPtr),y                      ; Save length of encoded data
    sta _numChars                           ;   +
_getCode:
    cpy _numChars                           ; IF Y == Num Encoded Bytes
    beq _translateString                    ; THEN all data is decoded, now translate
    iny                                     ; y++
    lda (_StringPtr),y                      ; A = an encoded byte
_decode:
    cmp #_LowR                              ; IF A < LowR
    bcc _writeCode                          ; THEN goto WriteCode ELSE decode byte
    sbc #_LowR                              ; A = A - _LowR. Carry is already set.
    sta _TempA                              ; Save Y
    tya                                     ;   |
    pha                                     ;   |
    lda _TempA                              ;   +
    pha                                     ; Save A
    tay                                     ; Y = A
    inc _Recursion                          ; Recursion++
    lda (_BPE_Left),y                       ; A = BPE_LEFT[Y]
    jsr _decode                             ; Decode this byte
    pla                                     ; Restore A
    tay                                     ; Y = A
    lda (_BPE_Right),y                      ; A = BPE_RIGHT[Y]
    jsr _decode                             ; Decode this byte
    pla                                     ; Restore Y
    tay                                     ;   +
    dec _Recursion                          ; Recursion--
    beq _getCode                            ; IF (Recursion == 0) THEN get next code
    rts                                     ; ELSE return to recursion thread
_writeCode:
    sta TEXT_String,x                       ; TEXT_String[x] = a
    inx                                     ; x++
    lda _Recursion                          ; IF (Recursion == 0)
    beq _getCode                            ; THEN get next code
    rts                                     ; ELSE return to recursion thread
; Get a list of all characters used in the string and translate the string to the list.
_translateString:
    dex                                     ; X-- because X is 1 higher than it should be.
    stx TEXT_String                         ; x is number of decoded bytes. Store this
    stx _stringLength                       ; in the string and a copy in zp for speed.
    ldy #$01                                ; We make sure that the first char is always empty
    sty _numChars                           ; TEXT_NumChars = 1
    ldy #$00                                ; y = index of current char of string
    sty TEXT_Characters
_advanceNextChar:
    iny                                     ; y++
    lda TEXT_String,y                       ; get the next char of the string
    cmp #$60                                ; IF A = newline constant
    beq _advanceNextChar                    ; THEN get next char
    ldx _numChars                           ; x = used to loop through translated array
_loopTranslatedArray:
    dex                                     ; x--
    bmi +                                   ; IF no more chars left to parse THEN add new char
    cmp TEXT_Characters,x                   ; ELSE IF a == chars[x]
    bne _loopTranslatedArray                ; THEN goto loopTranslatedArray
    beq _writeChar                          ; ELSE writeChar
*   ldx _numChars                           ; set x == numchars (next available char index)
    sta TEXT_Characters,x                   ; set this index to a new char/trans pair
    inc _numChars                           ; numchars++ and fall through to writeChar
_writeChar:
    txa
    sta TEXT_String,y                       ; translated[y] = x
    cpy _stringLength                       ; is this the final byte?
    beq _return                             ; if so, then return
    bne _advanceNextChar                    ; goto getnextchar (branch always)
_return:
    lda _numChars
    sta TEXT_NumChars
    rts
.scend

}