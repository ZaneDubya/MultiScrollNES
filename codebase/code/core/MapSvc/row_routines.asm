; ==============================================================================
; MapSvc_LoadRow
; In:   Scroll values in Scroll_X, Scroll_X2, Scroll_Y, Scroll_Y2
;       x = first visible x tile, y = first visible y tile
;       a = 0 if load the first row (scrolling up), 1 if load the last row (down)
; Out:  writes 32 tiles to MapData_RowBuffer
; Note: wipes out $00 - $08
MapSvc_LoadRow:
{
    .alias  _ppu_addr_temp      $00

    pha ; save a
    pha ; twice
    
    `Mapper_SwitchBank Bank_ChkData
    
    lda #$20
    sta _ppu_addr_temp
    lda #$00
    sta _ppu_addr_temp+1
    
    pla
    cmp #$01                            ; a,y = y + (a == 1) ? 1d : 0
    tya                                 ;   |
    bcc +                               ;   |   (carry set if in.a >= 1)
    `add $1d                            ;   |
    tay                                 ;   *
    
*   jsr MapSvc_GetPPUOffsetFromRow         ; wipes $00-$02
    lda _ppu_addr_temp
    sta MapBuffer_R_PPUADDR
    lda _ppu_addr_temp+1
    sta MapBuffer_R_PPUADDR+1
    
    ; load attribute bits when (1) scrolling up and loading lower tile of 
    ; metatile; (2) scrolling down and loading upper tile of metatile.
    pla                                 ; a = 0 if scrolling up, 1 if down.
    bne _scroll_down
    lda CameraCurrentY
    and #$08
    beq _load_attributes
    lda #$00
    beq _load_row
    
    _scroll_down:
    lda CameraCurrentY
    and #$08
    beq _load_row
    
    _load_attributes:
    lda #$01
    
    _load_row:
    jsr MapSvc_WriteRow            ; wipes out $01-$07, preserves x y
    `SetMapDataFlag MapData_HasRowData
    
    rts
}

; ==============================================================================
; MapSvc_WriteRow      Sets up a row of tiles to be copied to PPU RAM.
; IN    Bank should be set to bank containing chunk data.
;       a = load attributes if == 1.
;       x = X offset in subtiles. (0 - 63)
;       y = Y offset in subtiles. (0 - 63)
; OUT   writes 32 byte tile indexes to $0070 - $008f
;       writes 16 2bit attributes to $0010 - $001f
; STK   Ph/Pl two bytes on stack
; NOTE  Takes about 26.333 scanlines to execute.
;       Wipes out $01-$09
MapSvc_WriteRow:
{
    .alias  _length             $01
    .alias  _writeindex         $02
    .alias  _chunk              $03
    .alias  _tile               $04
    .alias  _lower_y_row        $05
    .alias  _ChunkPtr           $06
    .alias  _ChunkPtrHi         $07
    .alias  _do_attributes      $08
    .alias  _attrIndex          $09
    .alias  _attributebuffer    $10
    
    sta _do_attributes
    lda #$00
    sta _attrIndex
    
    `SaveXY
    
    jsr MapSvc_GetChunkAndTile
    
    tya                                 ; use lower tiles if (y & $01) == 1
    and #$01
    sta _lower_y_row
    
    ; write the left side of the next chunk to the 'right side' of ppu memory
    txa
    and #$1f
    sta _writeindex                 ; writeindex = x % 32
    lda #$20                        ; 
    sta _length                     ; length = $20
    jsr _writeRowPortion
    ; write the right side of the current chunk to the 'left side' of ppu memory
    lda #$00
    sta _writeindex
    pla
    pha                             ; a = x
    and #$1f
    sta _length
    jsr _writeRowPortion
    
    ; restore x and y, and update the attribute buffer if we are loading attributes.
    `RestoreXY
    lda _do_attributes
    beq +
    jsr MapSvc_WriteAttributeRow
*   rts

    _writeRowPortion:
    _getChunkPointer:
        ldx _chunk                      ; get pointer to the current chunk
        jsr MapSrv_GetChunkPtr
    _getSubTile:
        lda _length                     ; if (writeindex == length) then return
        cmp _writeindex                 ;   |
        bne +                           ;   |
        rts                             ;   +
    *   ldy _tile                       ; y = index of metatile in chunk (0-63)
        lda (_ChunkPtr),y               ; 
        tay                             ; y = a = index of metatile in tileset
    ; read subtile based on which corner of the metatile we are writing.
        lda _lower_y_row
        bne _lower
        lda _writeindex
        and #$01
        bne _ur
    _ul:
        lda (Tileset_PtrUL),y
        jmp _copyTile
    _ur:
        lda _do_attributes              ; if (_do_attributes != 0)
        beq +                           ; {
        ldx _attrIndex                  ;   x = _attrIndex 
        lda (Tileset_PtrAttribs),y      ;   a = Tileset_PtrAttribs[y]
        sta _attributebuffer,x          ;   _attributebuffer[x] = a
        inx
        stx _attrIndex
*       lda (Tileset_PtrUR),y
        jmp _copyTile
    _lower:
        lda _writeindex
        and #$01
        bne _lr
    _ll:
        lda (Tileset_PtrLL),y
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
        ; fall through to copyTile
    _copyTile:
        ldx _writeindex
        sta MapData_RowBuffer,x ; MapData_RowBuffer[x] == subtile
        ; increment the write index to the next byte of MapData_RowBuffer.
        ; if the incremented write index & 1 == 1, then we just wrote the
        ; first subtile of a tile, and now we need to write the second subtile.
        inc _writeindex         ; if ((writeindex++ & 1) == 1) 
        lda _writeindex         ;   copy a second subtile from the same tile
        and #$01                ;   |
        bne _getSubTile         ;   +
*       inc _tile               ; advance to the next tile.
        lda #$07                ; if ((tile % 8) == 0)
        and _tile               ; {
        bne _getSubTile            ;
        lda _tile               ;   tile -= 8
        `sub $08                ;   |    
        sta _tile               ;   +
        inc _chunk              ;   chunk++
        lda #$03                ;   if ((chunk % 4) == 0)
        and _chunk              ;   {
        beq +                   ;
        jmp _getChunkPointer    ;    
*       lda _chunk              ;
        `sub $04                ;       chunk -= 8;
        sta _chunk              ;   }
        jmp _getChunkPointer    ; }
}

; ==============================================================================
; IN:   x = X offset in subtiles. (0 - 63)
;       y = Y offset in subtiles. (0 - 63)
;       16 2-bit attribute values in $0010-$001f
; OUT:  Writes passed attributes into appropriate bytes of MapData_Attributes
MapSvc_WriteAttributeRow:
{
    .alias  _byte           $01 ; address of attribute byte (0-63)
    .alias  _shift          $02 ; shift of current attribute bits (0-3)
    .alias  _y_attr         $03 ; the y-attribute row (0-15)
    .alias  _temp           $04 ; temp storage for y register
    .alias  _shifted        $05 ; temp storage for attribute bytes
    
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
    
    ; shift value to sub attribute bits: (y & 2) * 2 + (x & 2): 0, 2, 4, or 6.
    ; I normalize this to 0, 1, 2, or 3.
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
    
    ; address of attribute byte = ((x & $1f) / 4) + (((y & $1f) / 4) * 8): 0-63
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
    
    ; MapBuffer_RA_Index is the low byte of PPU_ADDR that attr will be written to.
    and #$f8 ; mask out lower three bits - write an entire row at a time.
    clc
    adc #$c0
    sta MapBuffer_RA_Index
    
    ldy #$ff                ; for (y = 0; y < 16; y++)
_fory:                      ; {
    iny                     ;
    cpy #$10                ;
    beq _end_fory           ;    
    lda $10,y                   ; a = attr_row[y]
    sty _temp
    jsr _shift_bits             ; shift a by _shift * 2 bits, y = 0-3
    sta _shifted                ; _shifted = a
    lda MapData_Attributes,x    ; a = attr[x]
    and _save_bits,y            ; a &= save_bits[_shift] (y is set in _shift_bits)
    ldy _temp
    ora _shifted                ; a |= shifted.
    sta MapData_Attributes,x    ; attr[x] = a
    lda _shift                  ; _shift += 1
    eor #$01                    ;
    sta _shift                  ;
    and #$01                    ;
    bne _fory                   ; if (_shift bit 0 is 0, we just advanced a tile) {
    txa                         ;     if (x & 7 == 7)
    and #$07                    ;
    cmp #$07                    ;
    bne _inc_x                  ;    
    txa                         ;         x &= ^7
    and #$f8                    ;
    tax                         ;
    jmp _fory                   ;    
    _inc_x:                     ;       else
    inx                         ;           x += 1;
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