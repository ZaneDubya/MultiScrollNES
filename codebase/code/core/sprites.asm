; ==============================================================================
; Sprite_Setup: sets up the sprite service.
Sprite_Setup:
{
    .alias  _PalPtr         $00

    ; Copies four palettes in SprPalette0 to the PPU's Palette data. 282 cycles.
    `SetPointer _PalPtr, Palettes       ; set the pointer to the palette data.
    `SetPPUAddress $3f10                ; set PPU Address to $3f11 (SPR pal 0)
    ldx #$00
*   lda PPU_DATA                        ; increment the PPU ADDR so we don't
    lda SprPalette0,x                   ; overwrite the first palette color.
    asl
    asl
    tay
    iny                                 ; skip the first pal color (transparent)
    lda (_PalPtr),y
    sta PPU_DATA
    iny
    lda (_PalPtr),y
    sta PPU_DATA
    iny
    lda (_PalPtr),y
    sta PPU_DATA
    inx
    cpx #$04
    bne -
    rts
}

; ==============================================================================
; Sprite_BeginFrame - called at the beginning of each frame.
Sprite_BeginFrame:
{
    ; reset oamcurrentindex and oamfull
    lda #$00
    sta OamCurrentIndex
    sta OamFull
    
    ; set the current superchunk for sprites
    lda CameraCurrentX2
    and #%00000001
    sta OamCurrentSprChk
    lda CameraCurrentY2
    and #%00000001
    asl
    ora OamCurrentSprChk
    sta OamCurrentSprChk
    rts
}

; ==============================================================================
; Sprite_EndFrame - called at the end of each frame.
Sprite_EndFrame:
{
    lda OamFull
    bne _return
    lda #$ff
    ldx OamCurrentIndex
*   sta $0200,x
    inx
    inx
    inx
    inx
    bne -
_return:
    rts
}


; ==============================================================================
; Sprite_DrawMetaSprite - draws a meta sprite based on the passed information.
;
; IN:	$00-$01 - ptr to metasprite data.
;       $04 - oam byte1
;       $05 - oam byte2
;       $07 - tiles_wh
;       $08 - sprite x
;       $09 - sprite y
;       $0A - sprite superchunk
;       $0B - 0 = draw in screen space, else draw offset by camera (world space)
; OUT:	Writes a sprite to OAM memory
; NOTE:	Wipes out $00-$0e
Sprite_DrawMetaSprite:
{
    .alias  _ptr_metasprite             $00 ; ... $01  
    .alias  _ix                         $02
    .alias  _iy                         $03
    .alias  _oam_byte1                  $04
    .alias  _oam_byte2                  $05
    .alias  _x_carry                    $06
    .alias  _tiles_wh                   $07
    .alias  _x                          $08
    .alias  _y                          $09
    .alias  _sprite_superchunk          $0a
    .alias  _flag_world_space           $0b
    .alias  _x2                         $0c
    .alias  _y2                         $0d
    ;.checkpc 0
    ; do not display sprite if oam is full.
    lda OamFull
    beq _oam_not_full
    rts
    _oam_not_full:

    ; determine the placement of the sprite on screen
    _check_x:
        lda _x
        `subm CameraCurrentX
        sta _x
        bcs _check_y
        ; sprite wrapped superchunk x
        lda #%00000001
        eor _sprite_superchunk
        sta _sprite_superchunk
    _check_y:
        lda _y
        `subm CameraCurrentY
        sta _y
        bcs _sprite_on_screen
        ; sprite wrapped superchunk y
        lda #%00000010
        eor _sprite_superchunk
        sta _sprite_superchunk
    
    ; draw the frames of the sprite.
    ;   x is used as the oam data pointer, should be set to current * 4.
    ;   y is used as the sprite data pointer, should be set to zero.
    ; zero out ix, iy, y
    _sprite_on_screen:
        lda _tiles_wh
        sta _ix
        sta _iy
        lda #$00
        sta _x_carry
        ldx OamCurrentIndex
        tay
        lda _x
        sta _x2
        lda _y
        sta _y2
        
    _drawTile:
    ; if the sprite is off screen, do not display
        lda _sprite_superchunk
        cmp OamCurrentSprChk
        bne _skipTile
    
    ; write the tile to the oam buffer
        ; oam byte 0 - y location
        lda _y2
        sta $0200,x
        
        ; oam byte 1 - tile
        lda (_ptr_metasprite),y
        `add _oam_byte1
        sta $0201,x
        iny
        
        ; oam byte 2 - palette and flip bits
        lda (_ptr_metasprite),y
        `add _oam_byte2
        sta $0202,x
        iny
        
        ; oam byte 3 - x location
        lda _x2
        sta $0203,x 
        inx                     ; oam_index += 4
        inx
        inx
        inx
        
        bne _nextXTile
    
    ; if oam is full, set OamFull = 1 and return
        stx OamCurrentIndex
        inx
        stx OamFull
        rts
    
    _skipTile:
        iny
        iny
    
    _nextXTile:
        lda _x2 
        `add $08
        sta _x2
        bcc _no_x_carry
        ; if we carried, increment superchunk x
        lda #%00000001
        sta _x_carry
        eor _sprite_superchunk      ; _sprite_superchunk ^= #$01
        sta _sprite_superchunk
        cmp OamCurrentSprChk
        beq _nextYTile
    
    _no_x_carry:
        dec _ix
        bne _drawTile
    
    _nextYTile:
        lda _x
        sta _x2
        lda _y2
        `add $08
        sta _y2
        bcc _no_y_carry
        lda #%00000010
        eor _sprite_superchunk
        sta _sprite_superchunk
        cmp OamCurrentSprChk
        beq _offScreen
    
    _no_y_carry:
        lda _x_carry            ; $00 if no x carry, $01 if x carry
        eor _sprite_superchunk
        sta _sprite_superchunk
        lda _tiles_wh
        sta _ix
        lda #$00
        sta _x_carry            ; reset x_carry even if no carry
        dec _iy
        bne _drawTile
    
    ; if we've drawn all the tiles, save OamCurrentIndex and return
    _offScreen:
        stx OamCurrentIndex
        rts
    
    _return:
        rts
}

