; ==============================================================================
; Debug shading macros - calling these shades the output of the PPU.
; ==============================================================================
.macro DebugShadePPU_Normal
    .alias MaskBG_Normal		%00011000
    `SetByte PPU_MASK, MaskBG_Normal
.macend

.macro DebugShadePPU_Shaded
    .alias MaskBG_Shaded		%11111000
    `SetByte PPU_MASK, MaskBG_Shaded
.macend

.macro DebugShadePPU_Red
    .alias MaskBG_Red			%00111000
    `SetByte PPU_MASK, MaskBG_Red
.macend

.macro DebugShadePPU_Blue
    .alias MaskBG_Blue			%10011000
    `SetByte PPU_MASK, MaskBG_Blue
.macend

; ==============================================================================
; Memory helper macros
; ==============================================================================
.macro SetPointer
; loads the address in _2 to the pointer in _1. Must be in zp for indirect,y.
	lda #<_2
	sta _1
	lda #>_2
	sta [_1+1]
.macend

.macro	SetByte
	lda #_2
    sta _1
.macend

.macro	SetWord
	lda #<_2
    sta _1
	lda #>_2
    sta [_1+1]
.macend

.macro	CopyByte
	lda _2
    sta _1
.macend

.macro SetPPUAddress
	lda PPU_STATUS	; clear the address latch.
	lda #>_1
	sta PPU_ADDR
	lda #<_1
	sta PPU_ADDR
.macend

.macro	ClearVRAM
	.invoke SetPPUAddress $0000
	ldx #$00
*	ldy #$40
*	sta PPU_DATA
	iny
	bne -
	inx
	bne --
.macend

.macro ClearWRAM
	lda #$00
	ldx #$00
*	sta $0000,x
    sta $0100,x
    sta $0200,x
    sta $0300,x
    sta $0400,x
    sta $0500,x
    sta $0600,x
    sta $0700,x
    inx
    bne -
.macend

.macro ClearCartRAM		;Clear cartridge RAM at $6000-$7FFF.
*	ldy #$7F			;High byte of start address.
	sty $01				;
	ldy #$00			;Low byte of start address.
	sty $00				;$0000 points to $7F00
	tya	   				;A = 0
*	sta ($00),y			;
	iny					;Clears 256 bytes of memory before decrementing to next-->
	bne -				;256 bytes.
	dec $01				;
	ldx $01				;Is address < $6000?-->
	cpx #$60     		;If not, do another page.
	bcs -      			; 
.macend

.macro DelayCycles
	ldy #_1							; delay for 2+<_1>*(2+5+5*(49-1)+4)== 2+256*var cycles
*	ldx #50							; 256 cycles per loop
*	dex								;  |
	bne -							;  |
	dey								;  |
	bne --							;  +
.macend

.macro SavePAXY
	php				;Save processor status, A, X and Y on stack.
	pha				;Save A.
	txa				;
	pha				;Save X.
	tya				;
	pha				;Save Y.
.macend

.macro RestorePAXY
	pla				;Restore Y.
	tay				;
	pla				;Restore X.
	tax				;
	pla				;restore A.
	plp				;Restore processor status flags.
.macend

.macro SaveXY
	tya
	pha
	txa
	pha
.macend

.macro RestoreXY
	pla
	tax
	pla
	tay
.macend

.macro SetDEBUG
	lda #_1
	sta DEBUG
.macend

.macro SetGameFlag
	lda #_1
	ora GameFlags
	sta GameFlags
.macend

.macro ClearGameFlag
	lda #[$FF^_1]
	and GameFlags
	sta GameFlags
.macend

.macro CheckGameFlag
	lda #_1
	and GameFlags
.macend

.macro SetMapDataFlag
	lda #_1
	ora MapBuffer_Flags
	sta MapBuffer_Flags
.macend

.macro ClearMapDataFlag
	lda #[$FF^_1]
	and MapBuffer_Flags
	sta MapBuffer_Flags
.macend

.macro CheckMapDataFlag
	lda #_1
	and MapBuffer_Flags
.macend

.macro eor
	lda _1
	eor #_2
	sta _1
.macend

.macro add
	clc
	adc #_1
.macend

.macro addm
	clc
	adc _1
.macend

.macro sub
	sec	
	sbc #_1
.macend

.macro subm
	sec	
	sbc _1
.macend

.macro PlaySfx
	lda #_1
	ldx #_2
	jsr FamiToneSfxPlay
.macend

.macro PlaySample
	lda #_1
	jsr FamiToneSamplePlay
.macend

.macro FT_PlaySong
	lda #_1
	jsr FamiToneMusicPlay
.macend

.macro	FT_Init
	ldx #<_1	
	ldy #>_1
	lda TVSystem
    .if a == #$00                       ; NTSC
    {
        lda #$80                        ; Init to NTSC
        jsr FamiToneInit				; init FamiTone
    }
    .else                               ; PAL? Or DENDY
    {
        lda #$00                        ; Init to PAL
        jsr FamiToneInit				; init FamiTone
    }
	
	ldx #<SfxData
	ldy #>SfxData
	jsr FamiToneSfxInit					; set sound effects data location
.macend