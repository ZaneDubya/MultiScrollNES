; switches an 8kb PRG ROM into a bank
; MMC3 has two banks available: at $A000 and at $C000
.macro MMC3_BankA_Switch
	;lda #%01000111
	;sta $8000
	;lda #_1
	;sta $8001
.macend

.macro MMC3_BankA_Switch_Mem
	;lda #%01000111
	;sta $8000
	;lda _1
	;sta $8001
.macend

.macro MMC3_BankC_Switch
	;lda #%01000110
	;sta $8000
	;lda #_1
	;sta $8001
.macend

.macro MMC3_BankC_Switch_Mem
	;lda #%01000110
	;sta $8000
	;lda _1
	;sta $8001
.macend

.macro MMC3_BankA_Switch_X
	;lda #%01000111
	;sta $8000
	;stx $8001
.macend

.macro A53_Setup
	lda #A53_OUTER_BANK
	sta $5000
	lda #$03
	sta $8000
	lda #A53_MODE
	sta $5000
	lda #[A53_GAME_64K|A53_PRG_FIXED_C0|A53_MIRR_1]
	sta $8000
.macend

.macro A53_Show2000
	lda #A53_PRG_BANK
	sta $5000
	lda BankIn8000
	and #$0F
	sta BankIn8000
	sta $8000
.macend

.macro A53_Show2400
	lda #A53_PRG_BANK
	sta $5000
	lda BankIn8000
	ora #$10
	sta BankIn8000
	sta $8000
.macend

.macro A53_SwitchBank
	lda #A53_PRG_BANK
	sta $5000
	lda BankIn8000
	and #$10
	sta BankIn8000
	lda #_1
	and #$0f
	ora BankIn8000
	sta BankIn8000
	sta $8000
.macend

.macro A53_SwitchBankMem
	lda #A53_PRG_BANK
	sta $5000
	lda BankIn8000
	and #$10
	sta BankIn8000
	lda _1
	and #$0f
	ora BankIn8000
	sta BankIn8000
	sta $8000
.macend

.macro A53_PushBank							; not currently used
	lda BankIn8000
	and #$0f
	sta BankIn8000_Saved
.macend

.macro A53_PopBank							; not currently used
	lda #A53_PRG_BANK
	sta $5000
	lda BankIn8000
	and #$10
	sta BankIn8000
	lda BankIn8000_Saved
	and #$0f
	ora BankIn8000
	sta BankIn8000
	sta $8000
.macend

.macro SetPPUAddress
	lda PPU_STATUS	; clear the address latch.
	lda #>_1
	sta PPU_ADDR
	lda #<_1
	sta PPU_ADDR
.macend

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