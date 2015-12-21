; ============================= [ MapService.asm ] =============================
.require "../includes.asm"
.require "Maps/map_subroutines.asm"
.scope

; ==============================================================================
; MapService_LoadTileset    Loads a tileset from ROM. Currently it just loads
;                           the default tileset (at 8200 in bank 0) into VRAM.
; IN    Nothing at the moment. Ideally this would be a byte index value - index
;       of the tileset to load.
; OUT   Sets up tileset addresses in ZP.
MapService_LoadTileset:
{
    .alias  _TilesetPtr     $00
    .alias  _GfxPtr         $02
    .alias  _SavePtrPtr     $04
    .alias  _TempX          $06 ; $07 free
    .alias  _MapPalettes    $08 ; - $0b
    
    ; create addresses to the various kinds of data in the tileset.
    ; Save these address to ZP at Tileset_ZP.
    `SetPointer _TilesetPtr, TilesetData    ; base address of the tileset.
    `SetPointer _GfxPtr, Palettes           ; base address of palettes.
    `SetPointer _SavePtrPtr, Tileset_ZP     ; base address of tileset ZP memory.
    ldx #6                                  ; six ptrs for six kinds of data.
_next_ptr:
    ldy #$00                                ; y is offset to Tileset_ZP.
    lda _TilesetPtr                         ; save lo byte of this address.
    sta (_SavePtrPtr),y                     ;   +
    lda _TilesetPtr+1                       ; save hi byte of this address
    iny                                     ;   |
    sta (_SavePtrPtr),y                     ;   +
    lda #2                                  ; increment ptr to ZP by 2.
    `addm _SavePtrPtr                       ;   |
    sta _SavePtrPtr                         ;   +
    .if x < #05                             ; 4 palette indexes interleaved with
    {                                       ; last 4 255 bytes of tileset data.
        ldy #$ff                            ; get the palette index byte
        lda (_TilesetPtr),y                 ;   +
        sta [_MapPalettes-1],x              ; save it to _mappalettes (x is off
    }                                       ; by one).
    inc [_TilesetPtr+1]                     ; increment hi byte of pointer.
    dex                                     ; next 256 bytes of data
    bne _next_ptr
    
    ; load the four palettes
    `SetPPUAddress $3f00            ; set PPU Address to $3f00 (BG pal 0)
    ldx #3
_next_pal:
    lda _MapPalettes,x
    asl
    asl
    tay
    lda (_GfxPtr),y             ; load the palette at this index into PALRAM
    sta PPU_DATA
    iny
    lda (_GfxPtr),y
    sta PPU_DATA
    iny
    lda (_GfxPtr),y
    sta PPU_DATA
    iny
    lda (_GfxPtr),y
    sta PPU_DATA
    dex
    bpl _next_pal
    rts
}

; ========================[ MapService_WriteScreen ]============================
; IN:   Scroll values in Scroll_X, Scroll_X2, Scroll_Y, Scroll_Y2
; OUT:  Writes an entile screen into VRAM.
; NOTE: Wipes out $00-$09 in zp
;       Writes to VRAM - SCREEN MUST BE OFF!
; STK:  Uses 2 bytes of stack space.
MapService_WriteScreen:
.scope
.alias  _ppu_addr_temp      $00
.alias  _max_y              $00
.alias  _x                  $01

    ; check if we need to load new superchunks.
    jsr MapService_CheckLoadedSuperChunks
    
    `Mapper_SwitchBank Bank_ChkData
    
    ; get the x and y offsets for tiles (0 - 63), place them in x and y
    jsr Map_GetFirstSubTilesInXY
    stx MapBuffer_Last_X
    sty MapBuffer_Last_Y
    
    ; set ppu_addr to first y tile (in a)
    lda #$20
    sta _ppu_addr_temp
    lda #$00
    sta _ppu_addr_temp+1
    
    tya
    jsr Map_GetPPUOffsetFromRow
    lda _ppu_addr_temp
    sta MapBuffer_R_PPUADDR
    lda _ppu_addr_temp+1
    sta MapBuffer_R_PPUADDR+1
    
    tya
    `add $1e
    sta _max_y
    
_nextRow:
    lda #$01                                ; set 'load attributes' flag.
    jsr MapService_CreateRow                ; wipes out $00-$06
    
    lda MapBuffer_R_PPUADDR
    sta PPU_ADDR
    lda MapBuffer_R_PPUADDR+1
    sta PPU_ADDR
    
    stx _x
    ldx #$0
*   lda MapData_RowBuffer,x                 ; copy subtiles to nametable.
    sta PPU_DATA
    inx
    cpx #$20
    bne -
    ldx _x
    
    lda MapBuffer_R_PPUADDR+1                   ; increment ppu address by $20
    `add $20
    sta MapBuffer_R_PPUADDR+1
    bcc +
    inc MapBuffer_R_PPUADDR                 ; if ppu_lo carried, incrememt ppu_hi
    
; wrap ppu address at $23c0
*   lda #$23                            ; if ppu_hi == $23, and ppu_lo >= $c0
    cmp MapBuffer_R_PPUADDR                 ; ppu_hi = $20, and ppu_lo -= $c0
    bne +
    lda MapBuffer_R_PPUADDR+1
    cmp #$c0
    bcc +
    lda MapBuffer_R_PPUADDR+1
    `sub $c0
    sta MapBuffer_R_PPUADDR+1
    lda #$20
    sta MapBuffer_R_PPUADDR
    
*   iny
    cpy _max_y
    bne _nextRow

    rts
.scend

; =========================[ MapService_LoadNewData ]===========================
; In:   Scroll values. Determines if new data must be loaded (rows or cols).
; Out:  writes rows (32 tiles) and cols (30 tiles), attributes, MapBuffer_Flags.
;       Updates MapBuffer_Last_X and _Y.
MapService_LoadNewData:
.scope
.alias  _x      $0e
.alias  _y      $0f

    ; check if we need to load new superchunks.
    jsr MapService_CheckLoadedSuperChunks
    
    jsr Map_GetFirstSubTilesInXY
    stx _x          ; save x and y
    sty _y          ;   *
    
    sec                     ; a = y - last_y
    sbc MapBuffer_Last_Y    ; 
    beq _checkColumn
    ; load a new row
    ; scroll routines make it impossible to move more than one tile at a time.
    ; if a == 1, then we're loading the bottom row, else load the top row.
    cmp #$01
    beq +
    cmp #$C1
    beq +
    lda #$00
    
*   jsr MapService_LoadRow ; wipes out $00-$07
    ldx _x
    ldy _y
_checkColumn:
    txa
    sec
    sbc MapBuffer_Last_X
    beq _return
    
    ; load a new column
    ; scroll routines make it impossible to move more than one tile at a time
    ; if a == 1, load the right column, else load the left column.
    cmp #$01
    beq +
    cmp #$C1
    beq +
    lda #$00
*   jsr MapService_LoadCol ; wipes out $00-$08
    ldx _x
    ldy _y
_return:
    stx MapBuffer_Last_X
    sty MapBuffer_Last_Y
    rts
.scend

; ========================[ MapService_CopyRowColData ]=========================
; Copies row and column tiles to the PPU during VBLANK.
; Cycles:   27 if no data to copy.
;           26+479=505 if copying a column.
;           26+449=475 if copying a row.
;           25+479+449=953 if copying both column and row.
MapService_CopyRowColData:
{
    _checkRow:                                          ; Cycles
        `CheckMapDataFlag MapData_HasRowData            ; 5    
        beq _checkColumn                                ; 3 if none, 2 otherwise
        
        lda MapBuffer_R_PPUADDR                         ; 16        (323 total)   
        sta PPU_ADDR
        lda MapBuffer_R_PPUADDR+1
        sta PPU_ADDR
        `SetByte PPU_CTRL, $88                          ; 6
        ldx #0                                          ; 2    
    *   lda MapData_RowBuffer,x                         ; 4         (479 total)
        sta PPU_DATA                                    ; 4             |    
        inx                                             ; 2             |
        cpx #$20                                        ; 2             |
        bne -                                           ; 3 (2)         +
    
    _checkColumn:
        `CheckMapDataFlag MapData_HasColData            ; 5
        beq _return                                     ; 3 if none, 2 otherwise
        
        lda MapBuffer_C_PPUADDR                         ; 16    
        sta PPU_ADDR
        lda MapBuffer_C_PPUADDR+1
        sta PPU_ADDR
        `SetByte PPU_CTRL, $8C                          ; 6         (449 total)
        ldx #0                                          ; 2
    *   lda MapData_ColBuffer,x                         ; 4
        sta PPU_DATA                                    ; 4
        inx                                             ; 2
        cpx #$1e                                        ; 2    
        bne -                                           ; 3 (2)
    
    _return:
        lda #$00                                        ; 5
        sta MapBuffer_Flags
        rts                                             ; 6
}

; ========================[ MapService_UpdateScroll ]===========================
; 1. Bounds target.
; 2. Gets delta value based on difference between target & current.
; 3. Applies delta to current, scroll, and screen variables.
; NOTE: Delta greater than -8/+8 will break the tile gfx scrolling.
; NOTE: Delta greater than -128/+127 will break the algorithm altogether
; Wipes out a,x,y
MapService_UpdateCamera:
.scope
.alias  delta           $00
.alias  delta_hi        $01
.alias  is_neg          delta_hi
    jsr MapService_BoundTarget
    sec
    lda CameraTargetX
    sbc CameraCurrentX
    jsr _get_log_delta_from_a
    
    cpy #$00
    beq _delta_y
    
    lda delta
    clc
    adc CameraCurrentX
    sta CameraCurrentX
    lda delta_hi
    adc CameraCurrentX2
    sta CameraCurrentX2
_delta_y:
    sec
    lda CameraTargetY
    sbc CameraCurrentY
    jsr _get_log_delta_from_a
    
    cpy #$00
    beq _out
    
    lda delta
    clc
    adc CameraCurrentY
    sta CameraCurrentY
    lda delta_hi
    adc CameraCurrentY2
    sta CameraCurrentY2
    
    lda Screen_Y
    `addm delta
    .if a >= #240
    {
        ldx delta_hi
        .if x == #$ff
        {
            `add 240
        }
        .else
        {
            `sub 240
        }
    }
    sta Screen_Y
_out:
    rts
_get_log_delta_from_a:
; wipes out a,x, and y. returns log-ish delta.
    sta delta
    tax
    and #$80                ; test for negative number
    sta is_neg              ; needed to zero out is_neg value
    beq +
    txa
    and #$7f
    sta delta
    lda #$80
    sec
    sbc delta
    tax
*   txa
    ldy #$04
    and #$c0
    bne _return
    txa
    ldy #$03
    and #$30
    bne _return
    txa
    ldy #$02
    and #$0c
    bne _return
    txa
    ldy #$01
    and #$03
    bne _return
    ldy #$00
_return:
    sty delta
    lda is_neg
    beq +
    lda #$00
    sec
    sbc delta
    sta delta
    lda #$ff
    sta delta_hi
*   rts
.scend

.scend