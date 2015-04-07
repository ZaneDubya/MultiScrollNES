.require "map_creating.asm"
.require "map_loading.asm"

Map_GetFirstSubTilesInXY:
; in:	X and Y scroll values.
; out:	X and Y registers contain the first visible subtile (0 - 63).
;		A also contains Y
.scope
.alias	_x					$00

	lda Scroll_X
	lsr
	lsr
	lsr
	sta _x
	lda Scroll_X2
	and #$01
	asl
	asl
	asl
	asl
	asl
	ora _x
	tax
	
	lda Scroll_Y
	lsr
	lsr
	lsr
	sta _x
	lda Scroll_Y2
	and #$01
	asl
	asl
	asl
	asl
	asl
	ora _x
	tay
	rts
.scend

Map_GetPPUOffsetFromRow:
; in: Y row in A. Gets the PPU Address to the first tile in the specified row.
; adds this to MapBuffer_PPUADDR
.scope
.alias	_ppu_addr_temp		$00
.alias	_x					$02
	sta _x
	lda Scroll_Y2
	and #$fe
	asl
	`addm _x
	stx _x
	ldx #$1e
	jsr Divide8	; a = (a % $1e)
	tax
	and #$18	; ppu_hi += (y & $18) >> 3
	lsr
	lsr
	lsr
	`addm _ppu_addr_temp
	sta _ppu_addr_temp
	txa
	and #$07	; ppu_lo += (y & $07) << 5
	asl
	asl
	asl
	asl
	asl
	`addm _ppu_addr_temp+1
	sta _ppu_addr_temp+1
	ldx _x
	rts
.scend

; =====================[ MapService_GetSuperChunkPointer ]======================
; IN:	Superchunk loaded from X and Y.
; Out: 	Pointer to SuperChunk in $00,$01.
; Note:	Currently set up for 16x16 superchunk arrays. An additional asl after
;		loading Y will make this good for 32x32 data.
MapService_GetSuperChunkPointer:
.scope
.alias	_SuperChunkPtr		$02
; get the address of the superchunk pointer
	`SetPointer _SuperChunkPtr, $8000	; Pointer to begining of superchunk ptrs
	tya									; Add X and Y offsets to pointer
	asl									;	|	Y *= 16
	asl									;	|	|
	asl									;	|	|
	asl									;	|	|
	sta _SuperChunkPtr					;	|	+
	bcc +								; 	|	if carry, increment _Ptr hi
	inc _SuperChunkPtr+1				;	|	+
*	txa									;	|	X+Y *= 2 (2 = size of ptr)
	`addm _SuperChunkPtr				;	|	|
	asl									;	|	|
	sta _SuperChunkPtr					;	+	+
	bcc +								; if carry, increment _Ptr hi
	inc _SuperChunkPtr+1				;	+	+
; get the superchunk pointer
*	ldy #$00							; reset Y to 0
	lda (_SuperChunkPtr),y				; lo byte of superchunk address
	tax
	iny
	lda (_SuperChunkPtr),y				; hi byte of superchunk address
	stx _SuperChunkPtr
	sta _SuperChunkPtr+1
	rts
.scend

; ===================[ MapService_CheckLoadedSuperChunks ]======================
; If new superchunks should be loaded, based on the scroll values, load them.
; In: Scroll values.
; Wipes out a,x,y,$00,$01
MapService_CheckLoadedSuperChunks:
.scope
.alias _max_x		$00
.alias _max_y		$01
	ldx Scroll_X2
	cpx MapBuffer_SC_UL_X
	bne _load_chunks
	ldy Scroll_Y2
	cpy MapBuffer_SC_UL_Y
	bne _load_chunks
	rts
_load_chunks:
	ldx Scroll_X2
	stx MapBuffer_SC_UL_X
	inx
	inx
	stx _max_x
	ldy Scroll_Y2
	sty MapBuffer_SC_UL_Y
	iny
	iny
	sty _max_y
	ldy MapBuffer_SC_UL_Y
*	ldx MapBuffer_SC_UL_X
*	jsr MapService_LoadSuperChunk
	inx
	cpx _max_x
	bne -
	iny
	cpy _max_y
	bne --
	rts
.scend

; =======================[ MapService_LoadSuperChunk ]==========================
; Loads superchunk data into memory.
; IN: Superchunk to load from X and Y.
MapService_LoadSuperChunk:
.scope
.alias	_SuperChunkPtr		$02
.alias	_SC_Flags			$04
.alias	_ChunksOffset		$05
	`A53_SwitchBank Bank_MapData
; set the offset into memory where we will be saving the chunk indexes
	txa
	pha
	and #$01
	.if a == #$01
	{
		lda #$02
		sta _ChunksOffset
	}
	.else
	{
		; a = 0
		sta _ChunksOffset
	}
	tya
	pha
	and #$01
	.if a == #$01
	{
		lda _ChunksOffset
		`add $08
		sta _ChunksOffset
	}
	jsr MapService_GetSuperChunkPointer
; check the superchunk flags for data. Load data if it exists.
	ldy #$00							; reset Y to 0
	lda (_SuperChunkPtr),y				; load the superchunk flags
	sta _SC_Flags						; save the flags
	and #$01							; if bit 1 = 0, no data in this SC.
	bne _hasChunks						;
	ldx _ChunksOffset					;
	sta MapData_Chunks,x					; a = 0
	inx
	sta MapData_Chunks,x
	inx
	inx
	inx
	sta MapData_Chunks,x
	inx
	sta MapData_Chunks,x
	jmp _return
_hasChunks:
; load the chunk indexes into Chunks_0
	ldx _ChunksOffset
*	iny
	lda (_SuperChunkPtr),y
	sta MapData_Chunks,x
	inx
	.if y == #$2
	{
		inx
		inx
	}
	cpy #$04
	bne -
_return:
	pla
	tay
	pla
	tax
	rts
.scend

; =========================[ MapService_BoundTarget ]===========================
; bounds a point at x=$00-$01,y=$02-$03 to the CameraBound variables.
MapService_BoundTarget:
.scope
.alias	x_l		CameraTargetX
.alias	x_h		CameraTargetX2
.alias	y_l		CameraTargetY
.alias	y_h		CameraTargetY2

_cmp_x_left:
	lda x_h				; compare hi bytes first
	bpl +				; check if x is negative and bound is positive.
	ldx CameraBoundL2
	bpl _x_lt_left
*	cmp CameraBoundL2	;
	bcc _x_lt_left		; if x2 < left2 then x < left_bound
	bne _cmp_y_top		; if x2 != left2 then x > left_bound. next.
	lda x_l				; compare lo bytes
	cmp CameraBoundL	;
	bcs _cmp_y_top		; if x >= left, check next. Else, x < left_bound
_x_lt_left:	
	lda CameraBoundL2
	sta x_h
	lda CameraBoundL
	sta x_l
_cmp_y_top:
	lda y_h				; compare hi bytes first
	bpl +				; check if y is negative and bound is positive.
	ldx CameraBoundT2
	bpl _y_lt_top
*	cmp CameraBoundT2	;
	bcc _y_lt_top		; if y2 < top2 then y < top
	bne _cmp_x_right	; if y2 != top2 then y > top. next.
	lda y_l				; compare lo bytes
	cmp CameraBoundT	;
	bcs _cmp_x_right	; if y >= top, check next. Else, x < top
_y_lt_top:	
	lda CameraBoundT2
	sta y_h
	lda CameraBoundT
	sta y_l
_cmp_x_right:
	lda x_h				; compare hi bytes first
	cmp CameraBoundR2	;
	bcc _cmp_y_btm		; if x2 < right2 then x < right. next.
	bne _x_gt_right		; if x2 != right2 then x > right.
	lda x_l				; compare lo bytes
	cmp CameraBoundR	;
	bcc _cmp_y_btm		; if x < right then x < right. next. else x >= right.
_x_gt_right:
	lda CameraBoundR2
	sta x_h
	lda CameraBoundR
	sta x_l
_cmp_y_btm:
	lda y_h				; compare hi bytes first
	cmp CameraBoundB2	;
	bcc _return			; if y2 < btm2 then y < btm. return.
	bne _y_gt_btm		; if y2 != btm2 then y > btm.
	lda y_l				; compare lo bytes
	cmp CameraBoundB	;
	bcc _return			; if y < btm then y < btm. return. else y >= btm.
_y_gt_btm:
	lda CameraBoundB2
	sta y_h
	lda CameraBoundB
	sta y_l
_return:
	rts
.scend