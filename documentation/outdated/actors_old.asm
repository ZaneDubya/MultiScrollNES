; =============================== [ Actors.asm ] ==============================
; This code controls the statics and tiles that make the world alive.
; The SuperChunks have data in $300, $400, $500, and $600.
; Memory map:
;	$00 - PtrSC_Bank, PSCLo, PSCHi
;	$10 - Static sorting array
;	$20 - Flags, X, Y, IDLo, IDHi, PtrData_Bank, PDLo, PDHi (interleaved data)
;	$A0 - unused

; -------------------------------- [ Defines ] --------------------------------
.alias	StaticCount					$10

.alias	SuperChunkPtr_Bank			$00
.alias	SuperChunkPtr_Lo			$01
.alias	SuperChunkPtr_Hi			$02
; 13 unused
.alias	StaticSortArray				$10
.alias	StaticsFlag					StaticSortArray+[StaticCount*1]
.alias	StaticsX					StaticSortArray+[StaticCount*2]
.alias	StaticsY					StaticSortArray+[StaticCount*3]
.alias	StaticID_Lo					StaticSortArray+[StaticCount*4]
.alias	StaticID_Hi					StaticSortArray+[StaticCount*5]
.alias	StaticPtr_Bank				StaticSortArray+[StaticCount*6]
.alias	StaticPtr_Lo				StaticSortArray+[StaticCount*7]
.alias	StaticPtr_Hi				StaticSortArray+[StaticCount*8]

.alias	StaticFlag_InUse			$01
.alias	StaticFlag_Interact			$02

; ---------------------------- [ ResetSortTable ] -----------------------------
; In: A is the superchunk index (0-3) to reset.
ResetSortTable:
.scope
.alias	_PointerSort	$00
	
	and #$03						; clip A to $00-$03
	`add $03						; A += 3
	sta [_PointerSort+1]			; _PointerSort is pointer to sort array.
	lda #StaticSortArray
	sta _PointerSort
	
	ldy #StaticCount
	dey
	_while:
		tya
		sta (_PointerSort),y
		dey
		bpl _while
	rts
.scend

RandomizeSortTable:
.scope
.alias	_PointerYVals	$00
	
	and #$03						; clip A to $00-$03
	`add $03						; A += 3
	
	sta [_PointerYVals+1]			; _PointerYVals is pointer to sort array.
	lda #StaticsY
	sta _PointerYVals
	
	ldy #StaticCount
	dey
	_while:
		jsr GetRandom
		sta (_PointerYVals),y
		dey
		bpl _while
	rts
.scend

; -------------------------------- [ Sorting ] --------------------------------
; In: 	A is the superchunk index (0-3) to sort.
; Notes: Uses $00-$05 in zp.
SortActors:
.scope
.alias	_PointerSort	$00
.alias	_PointerYVals	$02
.alias	_key			$04
.alias	_y				$05
;.alias	_x				$06

	and #$03				; clip A to $00-$03
	`add $03				; a = a + 3
	
	sta [_PointerSort+1]	; _PointerSort = ptr to sort array.
	sta [_PointerYVals+1]	; _PointerYVals = to y-values.
	lda #StaticSortArray
	sta _PointerSort
	lda #StaticsY
	sta _PointerYVals

	; insertion sort
	ldx #01
	_iSort:
		txa							; y = x
		;stx _x
		tay							;
		lda (_PointerSort),y		; a = actor[x]
		sty _y						;
		tay							;
		pha							; push a (index pointed to by x)
		lda (_PointerYVals),y		; a = actor[x].y
		sta _key					; key = actor[x].y
		ldy _y
		dey							; y = x - 1
		_jSort:						; while (key < actor[y].y && y >= 0) {
			; possible to use .byte $B3, _PointerSort	; LAX 
			lda (_PointerSort),y	; ; a = actor[y].y 
			sty _y					; ; _y = y
			tay						;
			lda (_PointerYVals),y	;
			cmp _key				; 
			ldy _y					; ;	y = _y
			bcc _iNext				;
			lda	(_PointerSort),y	; 
			iny						;	actor[y+1] = actor[y]
			sta (_PointerSort),y	;
			dey						;
			dey						; 	y = y - 1
			bpl _jSort				; }
		_iNext:
		iny							; actor[y+1] = actor[x]
		pla							; pull a (index pointed to by x)
		sta (_PointerSort),y		;
		;ldx _x
		inx
		cpx #StaticCount
		bne _iSort
	rts
.scend