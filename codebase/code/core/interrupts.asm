; Handles NMI for a single screen mapper (Examples: Action 53, MMC1)

.scope
;--------------------------[ Non-Maskable Interrupt ]--------------------------- CYCLES
; The NMI is called 60 times a second by the VBlank signal from the PPU.
; It is called at scanline 240, which is off the visible area.
; We have 2358 cycles to update PPU Data. When the engine is running, either:
; 1. Copy map data, set up scroll registers, OAMDMA; OR
; 2. Copy CHRRAM (<=128b/8 tiles), set up scroll regs, OAMDMA. 
NMI:
    inc FrameCount                                                              ; 5
    
    pha                                                                         ; 3
    `CheckGameFlag FlagDoNMI				; IF Engine is running              ; 5    
    bne +									; THEN do NMI                       ; 3
    pla										; ELSE
    rti										; Return from Interrupt
*
    `CheckGameFlag FlagMainInProcess		; IF Main is not in process         ; 5
    beq +									; THEN do NMI                       ; 3
    pla										; ELSE
    rti										; Return from Interupt
*    
    lda MapBuffer_Flags						; Check for map data to be copied   ; 6 if no data to copy.
    beq +                                                                       ; 958 if copying both R & C data.
    jsr MapService_CopyRowColData           ; max 953 cycles.                    
*	
    ; dec x scroll by 8 (hides tile change artifact on right side of screen)    ; (57 Total)
    lda CameraCurrentX                                                          ; 3     |
    `sub $08                                                                    ; 4     |
    sta Screen_X                                                                ; 3     |
    ; Update the H/V scrolling by writing directly to the PPU ports.            ;       |
    ; Read 'The skinny on NES scrolling' if you want to understand what this    ;       |
    ; is doing.                                                                 ;       |
    lda #00					; 					; lda #$00, rol, rol            ; 2     |
    sta PPU_ADDR			; ---- NN--			; N is nametable select         ; 4     |
    lda Screen_Y			;					;                               ; 3     |
    sta PPU_SCROLL			; VV-- -yyy			; V is coarse Y scroll          ; 4     |
    asl						;					; y is fine y scroll            ; 2     |
    asl						;					; 	|                           ; 2     |
    and #%11100000			; VVV- ----			; 	|                           ; 2     |
    ldx Screen_X								; 	|                           ; 3     |
    sta Screen_X								; 	|                           ; 3     |
    txa											; 	|                           ; 2     |
    lsr											; 	|                           ; 2     |
    lsr											; 	|                           ; 2     |
    lsr											; 	|                           ; 2     |
    ora Screen_X								;	|                           ; 3     |
    stx Screen_X								;	|                           ; 3     |
    stx PPU_SCROLL								;	|                           ; 4     |
    sta PPU_ADDR								;	+                           ; 4     +
    
    ; Transfer $0200-$20FF to PPU Sprite RAM.
    lda #$02                                                                    ; 515/516
    sta PPU_SPR_DMA			                                                    ;       +    
    
    jsr RunOneFrame
    
    pla
    rti
    
.scend