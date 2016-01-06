; ============================== [ Actors.asm ] ================================
; This code controls the actors that make the world alive.

; -------------------------- [ Actors_DrawActors ] -----------------------------
; This routine will draw the actors that are currently visible on the screen.
; In:   None (reads data from active actor array and actor sort array).
; Out:  None
; Note: Takes about ~40% of a frame's cpu budget to draw 16 16x16 sprites.
Actors_DrawActors:
{
    .alias  _draw_addr                  $00
    .alias  _spritedata_index           $03 ; index of sprite data in ROM
    .alias  _sprite_x                   $08
    .alias  _sprite_y                   $09
    .alias  _sprite_superchunk          $0a
    .alias  _i                          $0f ; iterator - must not be overwritten by called routines.
    
    ; switch to bank containing metasprite data.
    `Mapper_SwitchBank Bank_SprData
    
    ; The game has already sorted the 16 actors in the sort array. We check
    ; every index of the sort array, and draw if one is present at that index.
    ; Empty slots have bit 7 set (negative).
    ldx #$0f                        ; for (x = $f; x >= 0; x--) {
    stx _i
    
    _draw_Actor:

        lda Actor_SortArray,x           ; if Actor_SortArray[x] has bit 7 set
        bmi _next_Actor                 ;   continue;
        
        tax                             ; x = index of current actor in memory
        ; get the x and y and superchunk values
        lda Actor_X,x
        sta _sprite_x
        lda Actor_Y,x
        sta _sprite_y
        lda Actor_SuperChunkX,x
        sta _sprite_superchunk
        
        lda Actor_Definition,x          ; a = actor definition index
        tax 
        lda ActorHeaderSprite,x         
        sta _spritedata_index           ; index of sprite data
        lda ActorHeaderAnimatePtr,x     ; _draw_routine_addr = address of draw routine.
        sta [_draw_addr+0]
        lda ActorHeaderAnimatePtrHi,x
        sta [_draw_addr+1]
        
        jsr _JSR_draw_addr_indirect     ; JSR (_draw_addr)
    
    _next_Actor:
        ldx _i
        dex
        bmi _return
        stx _i
        bpl _draw_Actor             ; }
    
    _return:
        rts
        
    _JSR_draw_addr_indirect:
        jmp (_draw_addr)
}

; ------------------------------------------------------------------------------
; ActDrw_Std - An example actor drawing routine.
; In:   x   = index of actor data in ROM
;       $03 = index of sprite data.
;       $08 = actor x
;       $09 = actor y
;       $0a = actor sc
; Ret:  None.
; Note: Must preserve $0f
ActDrw_Std:
{
    .alias  _ptr_metasprite             $00
    .alias  _ptr_metasprite_hi          $01
    .alias  _spritedata_index           $03 ; index of sprite data in ROM
    .alias  _oam_byte1                  $04
    .alias  _oam_byte2                  $05
    .alias  _frame                      $06
    .alias  _tiles_wh                   $07
    .alias  _sprite_x                   $08
    .alias  _sprite_y                   $09
    .alias  _sprite_superchunk          $0a
    .alias  _flag_world_space           $0b
    
    lda Actor_DrawData0,x                   ; cccc ffpp, see Actor Data.txt
    tay
    and #$f0
    sta _oam_byte1
    tya
    and #$03
    sta _oam_byte2
    lda Actor_DrawData1,x                   ; m..i iiii, see Actor Data.txt
    bmi _Draw_MovementSprite

_Draw_IndexedSprite:
    ; draw a sprite with index in lower 5 bytes of a.
    and #$1f
    sta _frame
    jmp _draw_frame
    
_Draw_MovementSprite:
    ; draw sprite based on facing, moving, and (if moving) determine whether to draw 
    ; frame 0 or frame 1 of the moving animation; otherwise draw frame 0 (standing)
    tya
    and #$0c                                ; a = .... ff.. (facing bits)
    lsr
    sta _frame
    lda Actor_Bitflags,x
    and ActFlg_IsMoving
    beq _draw_frame
    lda FrameCount
    and #$10 ; change sprite every 16 frames
    beq _draw_frame
    inc _frame
    ldx _spritedata_index
    
_draw_frame:
    ; x is _sprite_index, _frame is frame to draw. from these, get metasprite ptr
    ; first get the metasprite size (and use this value to set _tiles_wh)
    lda SpriteHdrs_Tile,x               ; a = SpriteHdrs_Tile[x]
    and #$03                            ; _tiles_wh = a + 1
    sta _tiles_wh                       ; |
    inc _tiles_wh                       ; +
    cmp #$01
    beq _metasprite_16x16
    bmi _metasprite_8x8
    cmp #$02
    beq _metasprite_24x24
_metasprite_32x32: ; max 31 frames, shift left by 32b (5x), max of 1000+, need to carry into second byte.
    lda _frame
    lsr
    lsr
    lsr
    sta _ptr_metasprite_hi
    lda _frame
    ror
    ror
    ror
    ror
    and #$e0
    jmp _add_meta_ptr
_metasprite_24x24: ; 3x3 metasprites, each is 18b, add 16b + 2b per frame to offset.
    lda _frame
    lsr
    lsr
    lsr
    lsr
    sta _ptr_metasprite_hi
    lda _frame
    asl
    asl
    asl
    asl
    sta _ptr_metasprite
    lda _frame
    asl
    `addm _ptr_metasprite
    bcc +
    inc _ptr_metasprite_hi
*   jmp _add_meta_ptr
_metasprite_16x16:
    lda #$00
    sta _ptr_metasprite_hi
    lda _frame ; max 31 frames, shift left by 8b (3x) = max of 248, fits in one byte!
    asl
    asl
    asl
    jmp _add_meta_ptr
_metasprite_8x8: ; max 31 frames, shift left by 2b (1x) = max of 62, fits in one byte!
    lda #$00
    sta _ptr_metasprite_hi
    lda _frame
    asl
_add_meta_ptr:
    sta _ptr_metasprite
    lda SpriteHdrs_AddressHi,x
    `addm _ptr_metasprite_hi
    sta _ptr_metasprite_hi
    lda SpriteHdrs_Address,x
    `addm _ptr_metasprite
    sta _ptr_metasprite
    bcc +
    inc _ptr_metasprite_hi
*   
    lda #$01
    sta _flag_world_space
    jmp Sprite_DrawMetaSprite
}

; ----------------------------- [ Actors_Update ] ------------------------------
; In:   None (reads data from active actor array).
; Out:  None
Actors_Update:
{
    .alias  _i                          $0c
    
    ldx #$0f                            ; x = $0f                
    stx _i                              ; i = x
    _update_Actor:
    
    _next_Actor:
        ldx _i                          ; x = i--
        dex                             ;   
        bmi _return                     ; if (x >= 0) {
        stx _i                          ; i = x
        bpl _update_Actor               ; goto _update_Actor
    _return:                            ; } else {
        rts                             ; return
}                                       ; }

; --------------------------- [ Actors_ClearActor ] ----------------------------
; In:   x = index of actor to clear
; Out:  Clears this actor's bitflags (importantly, clears bit 0).
;       Sets all instances of this actor's index in the sort array to $80.
; Note: wipes out a, y.
Actors_ClearActor:
{
    lda #$00
    sta Actor_Bitflags,x
    txa
    ldy #$0f
    
    _check_sort_array:
        cmp Actor_SortArray,y
        bne _next
        lda #$80
        sta Actor_SortArray,y
        txa
        _next:
        dey
        bpl _check_sort_array
    
    _return:
        rts
}