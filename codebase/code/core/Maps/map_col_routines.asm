; ===========================[ MapService_LoadCol ]=============================
; In:   Scroll values in Scroll_X, Scroll_X2, Scroll_Y, Scroll_Y2
;       x = first visible x tile, y = first visible y tile
;       a = 0 if load first col (scrolling left), 1 if load last col (right)
; Out:  writes 30 tiles to MapData_ColBuffer
; Note: wipes out $00 - $09
Map_LoadCol:
{
    .alias  _ppu_addr_temp      $00

    pha ; save a
    pha ; twice
    
    `Mapper_SwitchBank Bank_ChkData
    
    lda #$20
    sta _ppu_addr_temp
    txa
    and #$1f
    sta _ppu_addr_temp+1
    pla
    pha
    beq +
    
    lda #$1f
    `addm _ppu_addr_temp+1
    and #$1f
    sta _ppu_addr_temp+1
*
    pla
    cmp #$01                            ; a,x = x + (a == 1) ? 31 : 0
    txa                                 ;   |
    bcc +                               ;   |   (carry set if in.a >= 1)
    `add $1f                            ;   |
    and #$3f                            ;   |
    tax                                 ;   *
    tya                                 ; a = y (first row)
    
*   lda _ppu_addr_temp
    sta MapBuffer_C_PPUADDR
    lda _ppu_addr_temp+1
    sta MapBuffer_C_PPUADDR+1
    
    ; load the column
    lda #$00                            ; debug - do not load attributes.
    jsr Map_WriteCol            ; wipes out $00-$07+$09, preserves $08.
    `SetMapDataFlag MapData_HasColData
    
    ; load attribute bits
    pla                                 ; a = 0 if scrolling left, 1 if right.
    
    rts
}

; ==============================================================================
; Map_WriteCol      Sets up a column of tiles to be copied to PPU RAM.
; IN    Bank should be set to bank containing chunk data.
;       X is X offset in subtiles. (0 - 63)
;       Y is Y offset in subtiles. (0 - 63)
; OUT   writes 30 tiles to MapData_ColBuffer
; STK   Ph/Pl two bytes on stack
; NOTE  Takes about 25 scanlines to execute.
;       Wipes out $01-$09.
Map_WriteCol:
{
    .alias  _length             $01
    .alias  _writeindex         $02
    .alias  _chunk              $03
    .alias  _tile               $04
    .alias  _right_x_col        $05
    .alias  _ChunkPtr           $06
    .alias  _ChunkPtrHi         $07
    .alias  _do_attributes      $08    
    .alias  _temp               $09
    
    sta _do_attributes
    `SaveXY
    
    txa                             ; chunk = (x >> 4) | ((y & $30) >> 2)
    lsr
    lsr
    lsr
    lsr
    sta _chunk
    tya
    and #$30
    lsr
    lsr
    ora _chunk
    sta _chunk

    txa                             ; tile = ((x & $0e) >> 1) | ((y & $0e) << 2)
    and #$0e
    lsr
    sta _tile
    tya
    and #$0e
    asl
    asl
    ora _tile
    sta _tile
    
    txa                             ; use right col tiles if (x & $01) == 1
    and #$01
    sta _right_x_col
    
    tya                             ; THIS IS MAGIC.
    sta _temp                       ; a = y + (y_hi >> 1) << 2
    lda CameraCurrentY2
    lsr
    asl
    asl
    `addm _temp
    
*   cmp #$1e
    bcc +
    `sub $1e
    bne -
*   sta _writeindex
    sta _length
    jsr _writeColPortion
    `RestoreXY
    rts

    _writeColPortion:
    _getChunkPointer:
        ldx _chunk                      ; get pointer to the current chunk
        `SetPointer _ChunkPtr, ChunkData
        lda MapData_Chunks,x
        and #$c0
        `addm _ChunkPtr
        sta _ChunkPtr
        bcc +
        inc _ChunkPtrHi
    *   lda MapData_Chunks,x
        and #$3f
        `addm _ChunkPtrHi
        sta _ChunkPtrHi
    _getTile:
    *   ldy _tile
        lda (_ChunkPtr),y
        tay
    ; get sub tile
        lda _right_x_col
        bne _right0
        lda _writeindex
        and #$01
        bne _ll0
    _ul0:
        lda (Tileset_PtrUL),y
        jmp _copyTile
    _ll0:
        lda (Tileset_PtrLL),y
        jmp _copyTile
    _right0:
        lda _writeindex
        and #$01
        bne _lr0
    _ur0:
        lda (Tileset_PtrUR),y
        jmp _copyTile
    _lr0:
        lda (Tileset_PtrLR),y
        jmp _copyTile
    _copyTile:
        ldx _writeindex
        sta MapData_ColBuffer,x
        inc _writeindex
        
        lda _writeindex                     ; if (writeindex == length) return
        .if a == #$1e                       ; wrap on #$1e
        {
            lda #$00
            sta _writeindex
        }
        cmp _length
        bne +
        rts
        
    *   lda _writeindex         ; if ((writeindex & 1) == 1) copy second subtile
        and #$01
        bne _getTile            ; else
        
        lda _tile               ; tile += 8
        `add $08
        sta _tile
        lda #$c0                ; if ((tile % c0) == 0)
        and _tile
        beq _getTile
        lda _tile               ; tile -= 64
        `sub $40
        sta _tile
        lda _chunk              ; chunk += 4
        `add $04
        sta _chunk
        lda #$f0                ; if ((chunk % f0) == 0)
        and _chunk
        bne +
        jmp  _getChunkPointer
    *   lda _chunk
        `sub $10                ; chunk -= 16;
        sta _chunk
        jmp _getChunkPointer
}
