; ==============================================================================
; Handles player movement, based on controller input.
World_PlayerMovement:
{
    .alias  _moveX          $10
    .alias  _moveY          $11
    .alias  _MoveBoth       $12     ; if != 0, moving both x and y
    .alias  _BothBlcked     $13     ; if != 0, both corners blocked
    .alias  _actorX         $14     
    .alias  _actorXHi       $15
    .alias  _actorY         $16     
    .alias  _actorYHi       $17
    .alias  _chunk          $18     ; used by CheckBlock subroutine
    .alias  _tile           $19     ; used by CheckBlock subroutine
    .alias  _temp           $1a
    .alias  _flgKeepFacing  $1b     ; if != 0, maintain player facing.
    
    .alias  Player_Speed    $01
    .alias  Negative        $80
    
    lda #$00                        ; movex = movey = moveboth = bothblocked = 0
    sta _moveX
    sta _moveY
    sta _MoveBoth
    
    ldx Player_ActorIndex           ; keepfacing = player.ismoving    
    lda Actor_Bitflags,x
    and #ActFlg_IsMoving
    sta _flgKeepFacing
    
_checkpad:
    lda Ctrl0_Now                   ; if (Ctrl0_Now & pad_urdl == 0)
    and #CTRL_UDLR                  ;   player.ismoving = false;
    bne _check_lr                   ;   return;
    lda Actor_Bitflags,x
    and #[$ff^ActFlg_IsMoving]
    sta Actor_Bitflags,x
    rts
    
_check_lr:
    and #CTRL_LEFTRIGHT             ; if (Ctrl0_Now & pad_lr != 0)
    beq _check_ud                   ; {
    cmp #CTRL_LEFTRIGHT             ;   if (Ctrl0_Now & pad_lr == pad_lr)
    bne +                           ;       keepfacing = false; 
    lda #$00                        ;       goto check_ud
    sta _flgKeepFacing
    beq _check_ud
*   lda #$01                        ;   moveboth = 1
    sta _MoveBoth
    lda Ctrl0_Now                   ;   if (ctrl == pad_left)
    and #CTRL_LEFT                  ;       movex = speed & negative;
    beq _moving_right               ;   else
    _moving_left:                   ;       movex = speed + 15;
        lda #[Player_Speed|Negative]
        sta _moveX
        bne _check_ud
    _moving_right:
        lda #[Player_Speed+15]
        sta _moveX
        
_check_ud:
    and #CTRL_UPDOWN                ; if (Ctrl0_Now & pad_ud == 0)
    bne _check_ud2                  ;   moveboth = 0
    sta _MoveBoth                   ;   jmp _check_facing
    beq _check_facing
_check_ud2:                         ; else
    cmp #CTRL_UPDOWN                ;   if (Ctrl0_Now & pad_ud == pad_ud)
    bne +                           ;       keepfacing = false; 
    lda #$00                        ;       moveboth = 0
    sta _flgKeepFacing              ;       goto _check_facing
    sta _MoveBoth
    beq _check_facing
*   lda #$01                        ;   moveboth &= 1
    and _MoveBoth
    sta _MoveBoth
    lda Ctrl0_Now                   ;   if (ctrl == pad_left)
    and #CTRL_UP                    ;       movey = speed & negative;
    beq _moving_down                ;   else
    _moving_up:                     ;       movey = speed + 15;    
        lda #[Player_Speed|Negative]
        sta _moveY
        bne _check_facing
    _moving_down:
        lda #[Player_Speed+15]
        sta _moveY
        
_check_facing:                      ; if (keepfacing)
    lda _flgKeepFacing              ;   goto movement
    bne _do_movement                ; if (speedx != 0)
    lda _moveX                      ;   if (speedx is negative)
    beq _check_facing_ud            ;       facing = left
    and #Negative                   ;   else        
    beq _face_right                 ;       facing = right    
    _face_left:
        lda #FACING_LEFT
        jmp _set_facing
    _face_right:
        lda #FACING_RIGHT
        jmp _set_facing
_check_facing_ud:                   ; if (speedy != 0)
    lda _moveY                      ;   if (speedy is negative)
    and #Negative                   ;       facing = up
    beq _face_down                  ;   else 
    _face_up:                       ;       facing = down   
        lda #FACING_UP
        jmp _set_facing
    _face_down:
        lda #FACING_DOWN
        ; fall through to _set_facing
_set_facing:
    clc
    asl
    asl
    sta _temp
    lda Actor_DrawData0,x
    and #$f3
    ora _temp
    sta Actor_DrawData0,x
    
_do_movement:
    lda _moveX                          ; if (movex == 0)
    beq _do_movement_y                  ;   goto movement_y
_do_movement_x:
    lda Actor_X,x                       ; get local copies of actor x and y
    sta _actorX
    lda Actor_SuperChunkX,x
    sta _actorXHi
    lda Actor_Y,x
    sta _actorY
    lda Actor_SuperChunkY,x
    sta _actorYHi
    lda _movex
    bpl _move_right                     ; if (movex < 0)
    _move_left:                         ;   // move left
        and #$7f
        sta _moveX
        lda _actorX
        sec
        sbc _moveX
        sta _actorX
        bcs +
        dec _actorXHi
        bpl +
        ; tried to move beyond the left edge of the map. set position = $0000
        lda #$00
        sta Actor_X,x
        sta Actor_SuperChunkX,x
        jmp _do_movement_y
    *   jmp _check_move_x
    _move_right:
        lda _actorX
        clc
        adc _moveX
        bcc +
        sta _actorX
        inc _actorXHi
        ; check that _actorX does not exceed camera_block
        ; fall through to _check_move_x
    _check_move_x:
        jsr CheckBlock
_do_movement_y:

    rts
}
; ==============================================================================
; determines if the passed location is blocked
; in: local copies of xy and xhi and yhi in $14-$17
; out: a == 1 if blocked, 0 if unblocked
CheckBlock:
{
    .alias  _actorX         $14     
    .alias  _actorXHi       $15
    .alias  _actorY         $16     
    .alias  _actorYHi       $17
    .alias  _chunk          $18     
    .alias  _tile           $19
    ; 1. get chunk and metatile from x/xhi and y/yhi
    ; xhi      x
    ; 76543210 76543210
    ; .......c cmmm....
}