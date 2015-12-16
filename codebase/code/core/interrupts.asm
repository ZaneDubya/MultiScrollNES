; Handles NMI for a single screen mapper (Examples: Action 53, MMC1)

.scope
;--------------------------[ Non-Maskable Interrupt ]---------------------------
; The NMI is called 60 times a second by the VBlank signal from the PPU.
; It is called at scanline 240, which is off the visible area.
NMI:
	inc FRAME_CNT
	
	pha
	`CheckGameFlag FlagDoNMI				; IF Engine is running
	bne +									; THEN do NMI
	pla										; ELSE
	rti										; Return from Intterupt
*
	`CheckGameFlag FlagMainInProcess		; IF Main is not in process
	beq +									; THEN do NMI
	pla										; ELSE
	rti										; Return from Interupt
*    
    lda MapBuffer_Flags						; Check for map data to be copied
	beq +
	jsr MapService_CopyRowColData
*	
	; Update the H/V scrolling by writing directly to the PPU ports.
    ; Read 'The skinny on NES scrolling' if you want to understand what this
    ; is doing.
	lda Scroll_X
	`sub $08
	sta Scroll_X
	
	lda #00					; 					; lda #$00, rol, rol
	sta PPU_ADDR			; ---- NN--			; N is nametable select
	lda Screen_Y			;					;
	sta PPU_SCROLL			; VV-- -yyy			; V is coarse Y scroll
	asl						;					; y is fine y scroll
	asl						;					; 	|
	and #%11100000			; VVV- ----			; 	|
	ldx Scroll_X								; 	|
	sta Scroll_X								; 	|
	txa											; 	|
	lsr											; 	|
	lsr											; 	|
	lsr											; 	|
	ora Scroll_X								;	|
	stx Scroll_X								;	|
	stx PPU_SCROLL								;	|
	sta PPU_ADDR								;	+
	
	lda Scroll_X
	`add $08
	sta Scroll_X
	
    ; Transfer $0200-$20FF to PPU Sprite RAM.
	lda #$02
	sta PPU_SPR_DMA			
	
	jsr RunOneFrame
	
	pla
	rti
	
.scend