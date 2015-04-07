.require "defines.asm"
.require "macros.asm"
 
.scope
; =============================================================================
;	ChrCopyr - copies 20 tiles from PRG ROM to CHR RAM in ~2800 cycles
;	In:		X is number of chr tiles to copy
;			Y is offset into TEXT_Chars to start copying from
;			TEXT_Chars is an array of chars to copy.
;			PPU_Addr should already be written.
;	Out:	Writes X chr tiles to PPU_Addr
;	Notes:	A is wiped out, value in X too. Y is saved.
;			Uses 3 bytes in zp for temp variables: TempY, CodePtr, CodePtr+1, however, it saves the current contents of these.
;			If X == FF, call copybordertiles routine.
ChrCopyr:
.alias	_TempY				$0000
.alias	_CodePtr			$0001			;Points to address to jump to when choosing
.alias	_CodePtrHi			$0002			;a routine from a list of routine addresses.
	lda _TempY
	pha
	lda _CodePtr
	pha
	lda _CodePtrHi
	pha
_ChrCopyrInternal:
	txa
	.if a == #$ff				;	If a == $ff, copy 8 text border tiles to CHR RAM. ~1135 cycles
	{							;	Setup: store 3B,3C,3D,3E,3F,5B,5C,5D in TEXT_Characters
		clc
		ldx #$00
		.while x < #$08
		{
			lda #$3B
			_borderTileLoop:
			sta TEXT_Characters,x
			.if a == #$3F
			{
				lda #$5A
			}
			clc 				; carry flag is set when a >= #$3F, must be cleared
			adc #$01
			inx
			cpx #$08
			bne _borderTileLoop
		}	; at the end of this loop, x == 8
		.invoke SetByte PPU_Addr, $0f
		.invoke SetByte PPU_Addr, $80
		ldy #0
	}
*	lda TEXT_Characters,y					; get index of the chr to copy.
	asl										;* 2, each ptr is 2 bytes (16-bit).
	sty _TempY								; temp storage.
	tay
	lda _FirstChar,y						; low byte of code ptr
	sta _CodePtr
	iny
	lda _FirstChar,y
	sta _CodePtrHi
	ldy _TempY
	jmp (_CodePtr)							; jump to the char copying routine
_NextChar:
	iny
	dex
	bne -
	pla
	sta _CodePtrHi
	pla
	sta _CodePtr
	pla
	sta _TempY
	rts
	
; Charset, encoded by charcodr
; Each tile is max 83 bytes in ROM and loads in 141 or less cycles.
.advance origin+$0400,$7F
_FirstChar:
.include "../data/charset.asm"
.scend