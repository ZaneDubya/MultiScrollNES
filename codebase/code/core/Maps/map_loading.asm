; ===========================[ MapService_LoadRow ]=============================
; In:   Scroll values in Scroll_X, Scroll_X2, Scroll_Y, Scroll_Y2
;       x = first visible x tile, y = first visible y tile
;       a = 0 if load the first row (scrolling up), 1 if load the last row (down)
; Out:  writes 32 tiles to MapData_RowBuffer
; Note: wipes out $00 - $09
MapService_LoadRow:
{
    .alias  _ppu_addr_temp      $00
    .alias  _ppu_addr_lo        $08    

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
    
*   jsr Map_GetPPUOffsetFromRow         ; $00-$02
    lda _ppu_addr_temp
    sta MapBuffer_R_PPUADDR
    sta _ppu_addr_lo
    lda _ppu_addr_temp+1
    sta MapBuffer_R_PPUADDR+1

    ; load the row
    jsr MapService_CreateRow            ; wipes out $00-$07, preserves $08.
    `SetMapDataFlag MapData_HasRowData
    
    ; load attribute bits when (1) scrolling up and (ppu address & $0020) == 0,
    ; (2) scrolling down and (ppu address & $0020) == $20.
    pla                                 ; a = 0 if scrolling up, 1 if down.
    ;bne _scroll_down
    
    ; scrolling up
    ;lda _ppu_addr_temp      ; if (_ppu_addr & $20 == 0)
    ;and #$20                ;   write attribute
    ;bne _return             ; else return.
    ; write attribute!
    ;rts
    
    ; scrolling down
    ;_scroll_down:
    ;lda _ppu_addr_temp      ; if (_ppu_addr & $20 == 0)
    ;and #$20                ;   write attribute
    ;beq _return             ; else return.
    ; write attribute!
    rts
    
_return:
    rts
}

; ===========================[ MapService_LoadCol ]=============================
; In:   Scroll values in Scroll_X, Scroll_X2, Scroll_Y, Scroll_Y2
;       x = first visible x tile, y = first visible y tile
;       a = 0 if load first col (scrolling left), 1 if load last col (right)
; Out:  writes 30 tiles to MapData_ColBuffer
; Note: wipes out $00 - $08
MapService_LoadCol:
{
    .alias  _ppu_addr_temp      $00
    .alias  _ppu_addr_lo        $08    

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
    sta _ppu_addr_lo
    lda _ppu_addr_temp+1
    sta MapBuffer_C_PPUADDR+1
    
    ; load the column
    jsr MapService_CreateCol            ; wipes out $00-$07+$09, preserves $08.
    `SetMapDataFlag MapData_HasColData
    
    ; load attribute bits
    pla                                 ; a = 0 if scrolling left, 1 if right.
    
    rts
}