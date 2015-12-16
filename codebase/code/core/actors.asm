; ============================== [ Actors.asm ] ================================
; This code controls the actors that make the world alive.
.scope

; -------------------------- [ Actors_DrawActors ] -----------------------------
; This routine will draw the actors that are currently visible on the screen.
; In:   None (reads data from active actor array and actor sort array).
; Out:  None
; Note: Takes about ~40% of a frame's cpu budget to draw 16 16x16 sprites.
;       *** Possible optimization: don't save the _sprite values, simply
;           pass 'x' register to Sprite_DrawSprite. Might save cycles, might
;           lose them.
Actors_DrawActors:
.scope
    .alias  _objdata_ptr                $00 ; ... $01 (temp)
    .alias  _sprite_index               $06 ; (input for Sprite_DrawSprite)
    .alias  _sprite_frame               $07 ; (input for Sprite_DrawSprite)
    .alias  _sprite_x                   $08 ; (input for Sprite_DrawSprite)
    .alias  _sprite_y                   $09 ; (input for Sprite_DrawSprite)
    .alias  _sprite_superchunk          $0a ; (input for Sprite_DrawSprite)
    .alias  _i                          $0c

        ldx #$0f
        stx _i
    _draw_Actor:
        ; make sure there's an Actor in this position of the sort array
        ; if the position is empty, the index will be $80 (-128)
        lda Actor_SortArray,x
        bmi _next_Actor
        ; draw Actor with index = _i
        lda Actor_ObjPtr,x          ; get ptr to Object data for this Actor
        sta [_objdata_ptr+0]        ;   |
        lda Actor_ObjPtrHi,x        ;   |
        sta [_objdata_ptr+1]        ;   +
        ldy #$00                    ; get the SpriteIndex from the object data.
        lda (_objdata_ptr),y        ;   |
        sta _sprite_index           ;   +
        lda Actor_Frame,x           ; get the current frame for this Actor
        sta _sprite_frame           ;   +
        lda Actor_SuperChunk,x      
        sta _sprite_superchunk
        lda Actor_Y,x
        sta _sprite_x
        lda Actor_X,x
        sta _sprite_y
        jsr Sprite_DrawSprite ; wipes out $00-$0b, no return values
    _next_Actor:
        ldx _i
        dex
        bmi _return
        stx _i
        bpl _draw_Actor
    _return:
        rts
.scend

; ----------------------------- [ Actors_Update ] ------------------------------
; In:   None (reads data from active actor array).
; Out:  None
Actors_Update:
.scope
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
.scend                                  ; }

; --------------------------- [ Actors_ClearActor ] ----------------------------
; In:   x = index of actor to clear
; Out:  Clears this actor's bitflags (importantly, clears bit 0).
;       Sets all instances of this actor's index in the sort array to $80.
; Note: wipes out a, y.
Actors_ClearActor:
.scope
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
.scend

.scend