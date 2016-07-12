.require "../includes.asm"
.include "Movement.asm"

; ==============================================================================
;   WorldModule: handles controls, actors, update, etc, for world.
;   Note: need not maintain any memory.
World_Update:
{
    jsr World_CheckScroll               ; debug - scroll on dpad
    jsr World_CheckController
    
    jsr MapSvc_UpdateCamera         ; Move the Camera
    jsr SprLdr_Update                   ; update Sprite Loader data.
    
    rts
}
    
World_CheckScroll:
    lda Ctrl0_Now
    bne +
    rts

*   tax                                     ; LEFT: X = X + 1
    and #CTRL_LEFT
    beq +
    lda CameraTargetX
    `add $04
    sta CameraTargetX
    bcc +
    inc CameraTargetX2
    
*   txa                                     ; RIGHT: X = X - 1
    and #CTRL_RIGHT
    beq +
    lda CameraTargetX
    `sub $04
    sta CameraTargetX
    bcs +
    dec CameraTargetX2
*   txa                                     ; UP: Y = Y + 1
    and #CTRL_UP
    beq +
    lda CameraTargetY
    `add $04
    sta CameraTargetY
    bcc +
    inc CameraTargetY2
    
*   txa                                     ; UP: Y = Y - 1
    and #CTRL_DOWN
    beq +
    lda CameraTargetY
    `sub $04
    sta CameraTargetY
    bcs +
    dec CameraTargetY2
    
*   rts

World_CheckController:
    lda Ctrl0_New
    bne +
    rts
    
*   and #CTRL_LEFT               ; LEFT: play effect 0 on channel 0
    beq +
    `PlaySfx 0, FT_SFX_CH0
    
*   lda Ctrl0_New
    and #CTRL_RIGHT
    beq +
    `PlaySample 12
    
*   lda Ctrl0_New               ; UP: play effect 1 on channel 1
    and #CTRL_UP
    beq +
    `PlaySfx 1, FT_SFX_CH1
    
*   lda Ctrl0_New               ; DOWN: play effect 3 on channel 3
    and #CTRL_DOWN
    beq +
    `PlaySfx 3, FT_SFX_CH3
    
*   lda Ctrl0_New               ; B: play first one
    and #CTRL_B
    beq +
    `FT_Init MusicDataSet0
    `FT_PlaySong $00
    
*   lda Ctrl0_New               ; A: show script, play second song
    and #CTRL_A
    beq +
    ; lda #118
    ; ldy #0
    ; jsr TextEngine_DisplayMsg
    `FT_Init MusicDataSet1
    `FT_PlaySong $00
    
*   lda Ctrl0_New               ; SELECT: stop music
    and #CTRL_SELECT
    beq +
    jsr FamiToneMusicStop                       
    
*   lda Ctrl0_New               ; START: play sfx
    and #CTRL_START
    beq +
    `PlaySfx 2, FT_SFX_CH2
    
*    rts
