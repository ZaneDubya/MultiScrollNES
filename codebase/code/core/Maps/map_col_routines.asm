; ==============================================================================
; Map_LoadCol
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
    pla                             ; if scrolling right,
    pha                             ; _ppu_addr_temp_hi = (_ppu_addr_temp_hi + $1f) & $1f
    beq +                           ; this is necessary to place the first replaced tile
    lda #$1f                        ; (tile 32) on the far left of the nametable.
    `addm _ppu_addr_temp+1
    and #$1f
    sta _ppu_addr_temp+1

*   pla
    cmp #$01                            ; a,x = x + (a == 1) ? 31 : 0
    txa                                 ;   |
    bcc +                               ;   |   (carry set if in.a >= 1)
    `add $1f                            ;   |
    and #$3f                            ;   |
    tax                                 ;   *
    
*   lda _ppu_addr_temp
    sta MapBuffer_C_PPUADDR
    lda _ppu_addr_temp+1
    sta MapBuffer_C_PPUADDR+1
    
    ; load attribute bits when (1) scrolling left and loading right tiles of,
    ; metatiles, (2) scrolling right and loading left tiles of metatiles.
    pla                                 ; a = 0 if scrolling left, 1 if right.
    
    _load_col:
    lda #$01                    ; debug - always load attributes
    jsr Map_WriteCol            ; wipes out $00-$07+$09, preserves $08.
    `SetMapDataFlag MapData_HasColData
    
    rts
}

; ==============================================================================
; Map_WriteCol      Sets up a column of tiles to be copied to PPU RAM.
; IN    Bank should be set to bank containing chunk data.
;       A is load attribute flag (1 == load)
;       X is X offset in subtiles. (0 - 63)
;       Y is Y offset in subtiles. (0 - 63)
; OUT   writes 30 tiles to MapData_ColBuffer
;       writes 15 2bit attributes to $0010 - $001e
; STK   Ph/Pl two bytes on stack
; NOTE  Takes about 25 scanlines to execute.
;       Wipes out $01-$0A.
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
    .alias  _attrIndex          $0A
    .alias  _attributebuffer    $10
    
    ; BEGIN shared code between this and map_writerow - 41 bytes
    sta _do_attributes
    lda #$00
    sta _attrIndex
    
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
    ; END shared code
    
    txa                             ; use right col tiles if (x & $01) == 1
    and #$01
    sta _right_x_col
    
    ; get the index of the first tile to write in the column. I can't quite
    ; remember why this works, but there is similar (and better commented) code
    ; in map_row_routines.asm
    tya                             ; a = y + (y_hi >> 1) << 2
    sta _temp                       
    lda CameraCurrentY2
    lsr
    asl
    asl
    `addm _temp
*   cmp #$1e                        ; if (a >= 30)    
    bcc +                           ;       a -= 30
    `sub $1e
    bne -
*   sta _writeindex                 ; writeindex = a
    sta _length                     ; length = a
    jsr _writeColPortion
    
    ; restore x and y, and update the attribute buffer if we are loading attributes.
    `RestoreXY
    lda _do_attributes
    beq +
    jsr Map_WriteAttributeCol
*   rts

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
        bne _right
        lda _writeindex
        and #$01
        bne _ll
    _ul:
        lda (Tileset_PtrUL),y
        jmp _copyTile
    _ll:
        lda _do_attributes              ; if (_do_attributes != 0)
        beq +                           ; {
        ldx _attrIndex                  ;   x = _attrIndex 
        lda (Tileset_PtrAttribs),y      ;   a = Tileset_PtrAttribs[y]
        sta _attributebuffer,x          ;   _attributebuffer[x] = a
        inx
        stx _attrIndex
*       lda (Tileset_PtrLL),y
        jmp _copyTile
    _right:
        lda _writeindex
        and #$01
        bne _lr
    _ur:
        lda (Tileset_PtrUR),y
        jmp _copyTile
    _lr:
        lda _do_attributes              ; if (_do_attributes != 0)
        beq +                           ; {
        ldx _attrIndex                  ;   x = _attrIndex 
        lda (Tileset_PtrAttribs),y      ;   a = Tileset_PtrAttribs[y]
        sta _attributebuffer,x          ;   _attributebuffer[x] = a
        inx
        stx _attrIndex
*       lda (Tileset_PtrLR),y
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

; ==============================================================================
; IN:   x = X offset in subtiles. (0 - 63)
;       y = Y offset in subtiles. (0 - 63)
;       16 2-bit attribute values in $0010-$001f
; OUT:  Writes passed attributes into appropriate bytes of MapData_Attributes
Map_WriteAttributeCol:
{
    .alias  _byte           $01
    .alias  _shift          $02
    .alias  _y_attr         $03
    .alias  _temp           $04
    .alias  _shifted        $05
    
    `SaveXY
    
    ; each attribute table can hold only 15 metatile rows. because each
    ; superchunk is 16 metatile rows, we need to offset the attribute row we
    ; are writing to by (rows % 15).
    lda CameraCurrentY      ; _y_attr = (ccy >> 4 + ccyh) % 15
    clc
    lsr
    lsr
    lsr
    lsr
    clc
    adc CameraCurrentY2
    jsr Mod15
    sta _y_attr
    
    ; shift value to 1st sub attribute bits: (y & 2) * 2 + (x & 2): 0,2,4, or 6
    ; I normalize this to 0, 1, 2, or 3
    clc
    asl
    and #$02
    sta _shift
    txa
    and #$02
    clc
    lsr
    ora _shift
    sta _shift ; shift = 0, 1, 2, or 3.
    
    ; addr of 1st attribute byte = ((x & $1f) / 4) + (((y & $1f) / 4) * 8): 0-63
    txa
    and #$1f
    clc
    lsr
    lsr
    sta _byte ; byte = x & $1f / 4
    lda _y_attr
    and #$0e
    clc
    asl
    asl
    ora _byte
    tax ; x = index of the first attribute byte
    
    ; MapBuffer_CA_Index is the lo byte of PPU_ADDR for this column of attrs.
    ; we write an entire column at a time (8 bytes with same low three bits).
    and #$07 ; only save lower three bits.
    clc
    adc #$c0
    sta MapBuffer_CA_Index
    ldy #$ff                ; for (y = 0; y < 15; y++)
_fory:                      ; {
    iny                     ;
    cpy #$0f                ;
    beq _end_fory           ;    
    lda $10,y                   ; a = attr_row[y]
    sty _temp                   ; save y
    jsr _shift_bits             ; shift a by _shift * 2 bits, y = 0-3
    sta _shifted                ; _shifted = shifted attribute bits
    lda MapData_Attributes,x    ; a = attr[x]
    and _save_bits,y            ; a &= save_bits[_shift] (y is set in _shift_bits)
    ldy _temp                   ; restore y
    ora _shifted                ; a = attribute byte with new attribute
    sta MapData_Attributes,x    ; attr[x] = a
    lda _shift                  ; _shift += 2
    eor #$02                    ;
    sta _shift                  ;
    and #$02                    ;
    bne _fory                   ; if (_shift bit 1 is 0, we just advanced a tile) {
    txa                         ;     if (x & $38 == $38) (x has range of 0-$3f)
    and #$38                    ;
    cmp #$38                    ;
    bne _inc_x                  ;    
    txa                         ;         x &= ^7
    and #$07                    ;
    tax                         ;
    jmp _fory                   ;    
    _inc_x:                     ;       else
    txa                         ;           x += 8;
    clc                         ;
    adc #$08                    ;
    tax                         ;
    jmp _fory                   ; }
_end_fory:                 ; }
    `RestoreXY
    rts

_shift_bits:
    clc
    ldy _shift
    beq _shift_0
    cpy #$2
    beq _shift_4
    bcc _shift_2
_shift_6:
    asl
    asl
_shift_4:
    asl
    asl
_shift_2:
    asl
    asl
_shift_0:
    rts
    
_save_bits:
    .byte   $FC, $F3, $CF, $3F
}