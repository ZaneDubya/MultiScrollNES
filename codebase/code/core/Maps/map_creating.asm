; Map_Creating.asm
; Sets up rows and columns of tiles and attributes to be copied to PPU RAM
; during the next NMI.

; ==============================================================================
; MapService_CreateRow  Sets up a row of tiles to be copied to PPU RAM.
; IN    Bank should be set to bank containing chunk data.
;       a = 0 if scrolling up, 1 if scrolling down. REALLY, this should be a load attribute flag.
;       x = X offset in subtiles. (0 - 63)
;       y = Y offset in subtiles. (0 - 63)
; OUT   writes 32 tiles to $70
; STK   Ph/Pl two bytes on stack
; NOTE  Takes about 26.333 scanlines to execute.
MapService_CreateRow:
{
    .alias  _length             $01
    .alias  _writeindex         $02
    .alias  _chunk              $03
    .alias  _tile               $04
    .alias  _lower_y_row        $05
    .alias  _ChunkPtr           $06 ; ... $07
    ; $08 is unused - but is used in CreateCol.
    .alias  _scrollDown         $09
    
    sta _scrollDown
    `SaveXY
    
    ; draw the left portion of the row (on the right portion of the screen)
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
    
    tya                                 ; use lower tiles if (y & $01) == 1
    and #$01
    sta _lower_y_row
    
    txa
    
    _leftPortion:
        and #$1f
        sta _writeindex                 ; writeindex = x % 32
        lda #$20                        ; 
        sta _length                     ; length = $20
        jsr _writeRowPortion
    _rightPortion:
        lda #$00
        sta _writeindex
        pla
        pha                             ; a = x
        and #$1f
        sta _length
        jsr _writeRowPortion
        `RestoreXY
        rts

    _writeRowPortion:
    _getChunkPointer:
        ldx _chunk                      ; get pointer to the current chunk
        `SetPointer _ChunkPtr, ChunkData
        lda MapData_Chunks,x
        and #$c0
        `addm _ChunkPtr
        sta _ChunkPtr
        bcc +
        inc _ChunkPtr+1
    *   lda MapData_Chunks,x
        and #$3f
        `addm _ChunkPtr+1
        sta _ChunkPtr+1
    _getTile:
        lda _length                     ; if (writeindex == length) return
        cmp _writeindex
        bne +
        rts
    *   ldy _tile
        lda (_ChunkPtr),y
        tay
    ; get sub tile
        lda _lower_y_row
        bne _lower0
        lda _writeindex
        and #$01
        bne _ur0
    _ul0:
        lda (Tileset_PtrUL),y
        jmp _copyTile
    _ur0:
        lda (Tileset_PtrUR),y
        jmp _copyTile
    _lower0:
        lda _writeindex
        and #$01
        bne _lr0
    _ll0:
        lda (Tileset_PtrLL),y
        jmp _copyTile
    _lr0:
        lda (Tileset_PtrLR),y
        ; fall through to copyTile
    _copyTile:
        ldx _writeindex
        sta MapData_RowBuffer,x
        inc _writeindex
        lda _writeindex         ; if ((writeindex & 1) == 1) copy second subtile
        and #$01
        bne _getTile            ; else
        inc _tile               ; tile++
        lda #$07                ; if ((tile % 8) == 0)
        and _tile
        bne _getTile
        lda _tile               ; tile -= 8
        `sub $08
        sta _tile
        inc _chunk              ; chunk++
        lda #$03                ; if ((chunk % 4) == 0)
        and _chunk
        bne _getChunkPointer
        lda _chunk
        `sub $04                ; chunk -= 8;
        sta _chunk
        jmp _getChunkPointer
}

; ==============================================================================
; MapService_CreateCol  Sets up a column of tiles to be copied to PPU RAM.
; IN    Bank should be set to bank containing chunk data. REALLY, this should be a load attribute flag.
;       a = 0 if scrolling left, 1 if scrolling right
;       X is X offset in subtiles. (0 - 63)
;       Y is Y offset in subtiles. (0 - 63)
; OUT   writes 30 tiles to MapData_ColBuffer
; STK   Ph/Pl two bytes on stack
; NOTE  Takes about 25 scanlines to execute.
MapService_CreateCol:
{
    .alias  _length             $01
    .alias  _writeindex         $02
    .alias  _chunk              $03
    .alias  _tile               $04
    .alias  _right_x_col        $05
    .alias  _ChunkPtr           $06 ; ... $07
    .alias  _temp               $08
    .alias  _scrollRight        $09
    
    sta _scrollRight
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
        inc _ChunkPtr+1
    *   lda MapData_Chunks,x
        and #$3f
        `addm _ChunkPtr+1
        sta _ChunkPtr+1
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