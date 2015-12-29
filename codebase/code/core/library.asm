;--------------------------------[ GetTVSystem ]--------------------------------
; NES TV system detection code, (c) 2011 Damian Yerrick. Copying and
; distribution of this file, with or without modification, are permitted in
; any medium without royalty provided the copyright notice and this notice 
; are preserved in any source code copies. This file is offered as-is, without
; any warranty.
GetTVSystem:
    ldx #0
    ldy #0
    lda FrameCount
nmiwait1:
    cmp FrameCount
    beq nmiwait1
    lda FrameCount
nmiwait2:
    inx             ; Each iteration takes 11 cycles.
    bne +           ; NTSC NES: 29780 cycles or 2707 = $A93 iterations
    iny             ; PAL NES:  33247 cycles or 3022 = $BCE iterations
*   cmp FrameCount  ; Dendy:    35464 cycles or 3224 = $C98 iterations
    beq nmiwait2    ; so we can divide by $100 (rounding down), subtract ten,
    tya             ; and end up with 0=ntsc, 1=pal, 2=dendy, 3=unknown
    sec
    sbc #10
    cmp #3
    bcc notAbove3
    lda #3
notAbove3:
    rts

;-------------------------------[ Get Random ]----------------------------------
; Sets A to a random number between 0-255.
; Time: 19-22 cycles
GetRandom:
.scope
    lda Random_Seed
    beq _do
    asl
    beq _no         ;if the input was $80, skip the EOR
    bcc _no
_do:eor #$1d
_no:sta Random_Seed
    rts
.scend

;-------------------------------[ Update Timers ]-------------------------------
; Updates the two timers.
; TimerDelay is decremented from 9 to 0, decreasing by one each frame.
; Timer1 is decremented every frame. Timer2 is decremented every 10 frames.
UpdateTimers:
.scope
    ldx #$00            ; Decrement only Timer1
    dec TimerDelay      ;
    bpl _DecTimer       ;
    lda #$09            ; TimerDelay hits #$00 every 10th frame.
    sta TimerDelay      ; Reset TimerDelay after it hits #$00.
    ldx #$01            ; Decrement Timer2 and Timer1.
_DecTimer:
    lda Timer1,x        ;
    beq +               ;Don't decrease if timer is already zero.
    dec Timer1,x        ;
*   dex                 ;Timer1 and Timer2 decremented every frame.
    bpl _DecTimer       ;
    rts                 ;
.scend

;---------------------[ Simple divide and multiply routines ]-------------------
                        ; Divide by shifting A right.
Adiv32: lsr             ; Divide by 32.
Adiv16: lsr             ; Divide by 16.
Adiv8:  lsr             ; Divide by 8.
Adiv4:  lsr             ; Divide by 4.
Adiv2:  lsr             ; Divide by 2.
    rts                 ;
                        ; Multiply by shifting A left.
Amul32: asl             ; Multiply by 32.
Amul16: asl             ; Multiply by 16.
Amul8:  asl             ; Multiply by 8.
Amul4:  asl             ; Multiply by 4.
Amul2:  asl             ; Multiply by 2.
    rts                 ;
    
;-------------------------------[ 8bit Multiply ]-------------------------------
; General 8bit * 8bit = 8bit multiply by White Flame (aka David Holz)
; Multiplies multiplier by multiplicand and returns result in A
; In:   A (multiplicand) and X (multiplier, should be small for speed)
;       Signedness should not matter
; Out:  A is the product, X is not touched.
Multiply8:
.scope
    sta Multiplicand
    stx Multiplier
    lda #$00
    beq _enterLoop
_doAdd:
    clc
    adc Multiplier
_loop:
    asl Multiplier
_enterLoop:
    lsr Multiplicand
    bcs _doAdd
    bne _loop
_end:
    rts
.scend

;-------------------------------[ 16bit Multiply ]------------------------------
; 8bit * 8bit = 16bit multiply by White Flame (aka David Holz)
; In:   Multiplies A and X. X should be smaller.
; Out:  Low byte in X and MultiplySum, HiByte in MultiplySumHi.
;       Y is preserved.
Multiply16:
.scope
    sta Multiplier
    stx Multiplicand
    tya
    pha
    lda #$00
    tay
    sty MultiplySumHi  
    beq _enterLoop
_doAdd:
    clc
    adc Multiplier
    tax
    tya
    adc MultiplySumHi
    tay
    txa
_loop:
    asl Multiplier
    rol MultiplySumHi
_enterLoop:  ; accumulating multiply entry point (enter with .A=lo, .Y=hi)
    lsr Multiplicand
    bcs _doAdd
    bne _loop
    stx MultiplySum
    sty MultiplySumHi
    pla
    tay
    rts
.scend

;--------------------------------[ 8bit Divide ]--------------------------------
; 8bit/8bit division by White Flame (aka David Holz)
; In:   A (numerator) and X (denominator)   
; Out:  A (remainder) and X (quotient)
Divide8:
    sta Numerator
    stx Denominator
    lda #$00
    ldx #$07
    clc
*   rol Numerator
    rol
    cmp Denominator
    bcc +
    sbc Denominator
*   dex
    bpl --
    rol Numerator
    ldx Numerator
    rts
    
;-----------------------------[ 8bit Modulus by 15 ]----------------------------
; 8bit modulus by 15. Based on an algorithm by Douglas W. Jones
; http://homepage.cs.uiowa.edu/~jones/bcd/mod.shtml
; IN    A = value
; OUT   A = remainder
Mod15:
{
    ; a = (a >>  4) + (a & 0xF);    /* sum base 2**4 digits */
    pha
    clc
    lsr
    lsr
    lsr
    lsr
    sta Mod15Temp
    pla
    and #$0f
    clc
    adc Mod15Temp
    ; if (a < 15) return a;
    cmp #$10
    bcs _gt15
    rts
_gt15:
    ; if (a < (2 * 15)) return a - 15;
    cmp #$1f
    bcs _gt30
    sec
    sbc #$0f
    rts
_gt30:
    ; return a - (2 * 15);
    sec
    sbc #$1e
    rts
}