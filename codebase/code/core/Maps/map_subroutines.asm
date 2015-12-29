.require "map_writing.asm"
.require "map_loading.asm"

; ==============================================================================
; IN:   x = X offset in subtiles. (0 - 63)
;       y = Y offset in subtiles. (0 - 63)
;       16 2-bit attribute values in $0010-$001f
; OUT:  Writes passed attributes into appropriate bytes of MapData_Attributes
MapService_UpdateAttrBuffer:
{
    .alias  _byte           $01
    .alias  _shift          $02
    .alias  _y_attr         $03
    .alias  _y              $04
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
    
    ; shift value to sub attribute bits: (y & 2) * 2 + (x & 2): 0, 2, 4, or 6.
    ; I normalize this to 0, 1, 2, or 3.
    asl
    and #$02
    sta _shift
    txa
    and #$02
    asl
    ora _shift
    sta _shift ; shift = 0, 1, 2, or 3.
    
    ; address of attribute byte = ((x & $1f) / 4) + (((y & $1f) / 4) * 8): 0-63
    txa
    and #$1f
    lsr
    lsr
    sta _byte ; byte = x & $1f / 4
    lda _y_attr
    and #$fe
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
    sty _y
    jsr _shift_bits             ; shift a by _shift * 2 bits, wipes out y.
    sta _shifted                ; _shifted = a
    lda MapData_Attributes,x    ; a = attr[x]
    and _save_bits,y            ; a &= save_bits[_shift] (y is set in _shift_bits)
    ldy _y
    ora _shifted                ; a |= shifted.
    sta MapData_Attributes,x    ; attr[x] = a
    lda _shift                  ; _shift += 2
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


; ==============================================================================
; Map_GetFirstSubTilesInXY
; IN:   X and Y scroll values.
; OUT:  X and Y registers contain the first visible subtile (0 - 63).
;       A also contains Y
Map_GetFirstSubTilesInXY:
{
    .alias  _x                  $0f

    lda CameraCurrentX
    lsr
    lsr
    lsr
    sta _x
    lda CameraCurrentX2
    and #$01
    asl
    asl
    asl
    asl
    asl
    ora _x
    tax

    lda CameraCurrentY
    lsr
    lsr
    lsr
    sta _x
    lda CameraCurrentY2
    and #$01
    asl
    asl
    asl
    asl
    asl
    ora _x
    tay
    rts
}

; ==============================================================================
; Map_GetPPUOffsetFromRow
; IN:   a = y row.
; OUT:  Gets the PPU Address to the first tile in the specified row.
; adds this to MapBuffer_PPUADDR
Map_GetPPUOffsetFromRow:
{
    .alias  _ppu_addr_temp      $00
    .alias  _x                  $02
    
    sta _x
    lda CameraCurrentY2
    and #$fe
    asl
    `addm _x
    stx _x
    ldx #$1e
    jsr Divide8 ; a = (a % $1e)
    tax
    and #$18    ; ppu_hi += (y & $18) >> 3
    lsr
    lsr
    lsr
    `addm _ppu_addr_temp
    sta _ppu_addr_temp
    txa
    and #$07    ; ppu_lo += (y & $07) << 5
    asl
    asl
    asl
    asl
    asl
    `addm _ppu_addr_temp+1
    sta _ppu_addr_temp+1
    ldx _x
    rts
}

; ==============================================================================
; MapService_GetSuperChunkPointer
; IN:   Superchunk loaded from X and Y.
; Out:  Pointer to SuperChunk in $00,$01.
; NOTE: Currently set up for 16x16 superchunk arrays. An additional asl after
;       loading Y will make this good for 32x32 data.
MapService_GetSuperChunkPointer:
{
    .alias  _SuperChunkPtr      $02
    ; get the address of the superchunk pointer
        `SetPointer _SuperChunkPtr, $8000   ; Pointer to begining of superchunk ptrs
        tya                                 ; Add X and Y offsets to pointer
        asl                                 ;   |   Y *= 16
        asl                                 ;   |   |
        asl                                 ;   |   |
        asl                                 ;   |   |
        sta _SuperChunkPtr                  ;   |   +
        bcc +                               ;   |   if carry, increment _Ptr hi
        inc _SuperChunkPtr+1                ;   |   +
    *   txa                                 ;   |   X+Y *= 2 (2 = size of ptr)
        `addm _SuperChunkPtr                ;   |   |
        asl                                 ;   |   |
        sta _SuperChunkPtr                  ;   +   +
        bcc +                               ; if carry, increment _Ptr hi
        inc _SuperChunkPtr+1                ;   +   +
    ; get the superchunk pointer
    *   ldy #$00                            ; reset Y to 0
        lda (_SuperChunkPtr),y              ; lo byte of superchunk address
        tax
        iny
        lda (_SuperChunkPtr),y              ; hi byte of superchunk address
        stx _SuperChunkPtr
        sta _SuperChunkPtr+1
        rts
}

; ==============================================================================
; MapService_CheckLoadedSuperChunks
; If new superchunks should be loaded, based on the scroll values, load them.
; In: Scroll values.
; Wipes out a,x,y,$00,$01
MapService_CheckLoadedSuperChunks:
{
    .alias _max_x       $00
    .alias _max_y       $01
        ldx CameraCurrentX2
        cpx MapBuffer_SC_UL_X
        bne _load_chunks
        ldy CameraCurrentY2
        cpy MapBuffer_SC_UL_Y
        bne _load_chunks
        rts
    _load_chunks:
        ldx CameraCurrentX2
        stx MapBuffer_SC_UL_X
        inx
        inx
        stx _max_x
        ldy CameraCurrentY2
        sty MapBuffer_SC_UL_Y
        iny
        iny
        sty _max_y
        ldy MapBuffer_SC_UL_Y
    *   ldx MapBuffer_SC_UL_X
    *   jsr MapService_LoadSuperChunk
        inx
        cpx _max_x
        bne -
        iny
        cpy _max_y
        bne --
        rts
}

; ==============================================================================
; MapService_LoadSuperChunk - Loads superchunk data into memory.
; IN: Superchunk to load from X and Y.
MapService_LoadSuperChunk:
{
    .alias  _SuperChunkPtr      $02
    .alias  _SC_Flags           $04
    .alias  _ChunksOffset       $05
        `Mapper_SwitchBank Bank_MapData
    ; set the offset into memory where we will be saving the chunk indexes
        txa
        pha
        and #$01
        .if a == #$01
        {
            lda #$02
            sta _ChunksOffset
        }
        .else
        {
            ; a = 0
            sta _ChunksOffset
        }
        tya
        pha
        and #$01
        .if a == #$01
        {
            lda _ChunksOffset
            `add $08
            sta _ChunksOffset
        }
        jsr MapService_GetSuperChunkPointer
    ; check the superchunk flags for data. Load data if it exists.
        ldy #$00                            ; reset Y to 0
        lda (_SuperChunkPtr),y              ; load the superchunk flags
        sta _SC_Flags                       ; save the flags
        and #$01                            ; if bit 1 = 0, no data in this SC.
        bne _hasChunks                      ;
        ldx _ChunksOffset                   ;
        sta MapData_Chunks,x                    ; a = 0
        inx
        sta MapData_Chunks,x
        inx
        inx
        inx
        sta MapData_Chunks,x
        inx
        sta MapData_Chunks,x
        jmp _return
    _hasChunks:
    ; load the chunk indexes into Chunks_0
        ldx _ChunksOffset
    *   iny
        lda (_SuperChunkPtr),y
        sta MapData_Chunks,x
        inx
        .if y == #$2
        {
            inx
            inx
        }
        cpy #$04
        bne -
    _return:
        pla
        tay
        pla
        tax
        rts
}

; ==============================================================================
; bounds a point at x=$00-$01,y=$02-$03 to the CameraBound variables.
MapService_BoundTarget:
{
    .alias  x_l     CameraTargetX
    .alias  x_h     CameraTargetX2
    .alias  y_l     CameraTargetY
    .alias  y_h     CameraTargetY2

    _cmp_x_left:
        lda x_h             ; compare hi bytes first
        bpl +               ; check if x is negative and bound is positive.
        ldx CameraBoundL2
        bpl _x_lt_left
    *   cmp CameraBoundL2   ;
        bcc _x_lt_left      ; if x2 < left2 then x < left_bound
        bne _cmp_y_top      ; if x2 != left2 then x > left_bound. next.
        lda x_l             ; compare lo bytes
        cmp CameraBoundL    ;
        bcs _cmp_y_top      ; if x >= left, check next. Else, x < left_bound
    _x_lt_left: 
        lda CameraBoundL2
        sta x_h
        lda CameraBoundL
        sta x_l
    _cmp_y_top:
        lda y_h             ; compare hi bytes first
        bpl +               ; check if y is negative and bound is positive.
        ldx CameraBoundT2
        bpl _y_lt_top
    *   cmp CameraBoundT2   ;
        bcc _y_lt_top       ; if y2 < top2 then y < top
        bne _cmp_x_right    ; if y2 != top2 then y > top. next.
        lda y_l             ; compare lo bytes
        cmp CameraBoundT    ;
        bcs _cmp_x_right    ; if y >= top, check next. Else, x < top
    _y_lt_top:  
        lda CameraBoundT2
        sta y_h
        lda CameraBoundT
        sta y_l
    _cmp_x_right:
        lda x_h             ; compare hi bytes first
        cmp CameraBoundR2   ;
        bcc _cmp_y_btm      ; if x2 < right2 then x < right. next.
        bne _x_gt_right     ; if x2 != right2 then x > right.
        lda x_l             ; compare lo bytes
        cmp CameraBoundR    ;
        bcc _cmp_y_btm      ; if x < right then x < right. next. else x >= right.
    _x_gt_right:
        lda CameraBoundR2
        sta x_h
        lda CameraBoundR
        sta x_l
    _cmp_y_btm:
        lda y_h             ; compare hi bytes first
        cmp CameraBoundB2   ;
        bcc _return         ; if y2 < btm2 then y < btm. return.
        bne _y_gt_btm       ; if y2 != btm2 then y > btm.
        lda y_l             ; compare lo bytes
        cmp CameraBoundB    ;
        bcc _return         ; if y < btm then y < btm. return. else y >= btm.
    _y_gt_btm:
        lda CameraBoundB2
        sta y_h
        lda CameraBoundB
        sta y_l
    _return:
        rts
}