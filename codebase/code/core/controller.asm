.scope

.alias _temp                $0000
.alias _temp1               $0001
.alias _temp2               $0002
.alias _temp3               $0003

Input_Init:
    lda #0
    sta Ctrl0_Now
    sta Ctrl0_Last
    sta Ctrl0_New
    rts

Input_Get:
    jsr _read_joy
    sta Ctrl0_Now
    eor Ctrl0_Last
    and Ctrl0_Now
    sta Ctrl0_New
    lda Ctrl0_Now
    sta Ctrl0_Last
    rts

; Reads controller into A and _temp.
; Unreliable if DMC is playing.
; Preserved: X, Y
; Time: 153 clocks
_read_joy_fast:
    ; Strobe controller
    lda #1          ; 2
    sta $4016       ; 4
    lda #0          ; 2
    sta $4016       ; 4
    
    ; Read 8 bits
    lda #$80        ; 2
    sta <_temp       ; 3
*       lda $4016   ; *4
    
    ; Merge bits 0 and 1 into carry. Normal
    ; controllers use bit 0, and Famicom
    ; external controllers use bit 1.
    and #$03        ; *2
    cmp #$01        ; *2
    
    ror <_temp       ; *5
    bcc -           ; *3
                    ; -1
    lda <_temp       ; 3
    rts             ; 6


; Reads controller into A.
; Reliable even if DMC is playing.
; Preserved: X, Y
; Time: ~660 clocks
_read_joy:
    jsr _read_joy_fast
    sta <_temp3
    jsr _read_joy_fast
    pha
    jsr _read_joy_fast
    sta <_temp2
    jsr _read_joy_fast
    
    ; All combinations of one controller
    ; change and one DMC DMA corruption
    ; leave at least two matching readings,
    ; and never just the first and last
    ; matching. No more than one DMC DMA
    ; corruption can occur.
    
    ; X--X can't occur
    pla
    cmp <_temp3
    beq +           ; XX--
    cmp <_temp
    beq +           ; -X-X
    
    lda <_temp2     ; X-X-
                    ; -XX-
                    ; --XX
*   cmp #0
    rts

.scend