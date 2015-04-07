Debug_LoadCHRRAM:
.scope
    .alias	_GfxPtr			$00
    .alias	_SpriteGfxRom	$A000		; Location of tilegfx in tilegfx bank.
    .alias	_TileGfxRom		$B000		; Location of tilegfx in tilegfx bank.
    
	; copy all 512 tiles in the bg graphic set to chrram.
	`A53_SwitchBank Bank_TilGfxData
	`SetPointer _GfxPtr, _TileGfxRom	        ; set the pointer to the tile gfx data.
	`SetPPUAddress $0000				        ; set PPU Address to CHR 0 ($0000)
	jsr _CopyGfxBank
	`SetPointer _GfxPtr, _SpriteGfxRom	        ; set the pointer to the tile gfx data.
	`SetPPUAddress $1000				        ; set PPU Address to CHR 0 ($0000)
	jsr _CopyGfxBank
    rts
	
_CopyGfxBank:
	ldx #$00 							    ; copy 256 tiles
_CopyTile:
	ldy #$00
_CopyByte:
	lda (_GfxPtr),Y 					; load a tile byte
	sta PPU_DATA
	iny
	cpy #$10
	bne _CopyByte 						;copy 16 bytes (1 tile).
	
	inx
	beq _Return
	
	lda _GfxPtr
	`add $10
	sta _GfxPtr
	bcc _CopyTile
	inc [_GfxPtr+1]
	jmp _CopyTile
_Return:
	rts
.scend

Debug_LoadMap:
    ; Load Debug Map
	`SetByte Scroll_X, 0
	`SetByte Scroll_X2, 0
	`SetByte Scroll_Y, 0
	`SetByte Scroll_Y2, 0
	`SetByte Screen_Y, 0
	`SetByte CameraCurrentY, 0
	`SetByte CameraTargetY,$70
	`SetByte CameraTargetY2,$FF
	`SetByte CameraBoundR, $ff
	`SetByte CameraBoundR2, $0f
	`SetByte CameraBoundB, $ff
	`SetByte CameraBoundB2, $0f
	`SetByte MapBuffer_SC_UL_X, $ff	    ; MapService will load all superchunks
	jsr MapService_WriteScreen		    ; load a screen of tiless into vram
    rts

Debug_InitSprites:
    ; Init Sprites
	`SetByte SprPalette0, $04
	`SetByte SprPalette1, $02
	`SetByte SprPalette2, $03
	`SetByte SprPalette3, $00
	jsr Sprite_Setup
    rts

Debug_SpriteDisplay:
	ldy #$0f
    lda #$80
*   sta Actor_SortArray,y
    dey
    bpl -
    rts
    ; Debug sprite display.
	ldx #$0f
	ldy #$0f
    lda #$50
_next_sprite:
	sta Actor_X,x
	sta Actor_Y,x
	`add $08
    stx Actor_SortArray,y
    ;pha
    ;txa
    ;sta Actor_Frame,y
    ;pla
    dey
	dex
	bpl _next_sprite
    ; Random sprite off to the side
	;`SetByte Actor_SuperChunk, $02
	;`SetByte Actor_Y, $fe
    ; turn off random sprite
    lda #$80
    ldy #$06
    sta Actor_SortArray,y
    rts

Debug_ClearAllActors:
.scope
    .alias  _i              $00
    ; first, set the sort array to 0 (not really necessary, but for sanity, 
    ; just in case there's some bizarre data in there).
    ldx #$0f
    lda #$00
    _loop1:
        sta Actor_SortArray,x
        dex
        bpl _loop1
    ; second, clear the actor bitflag arrays and sort arrays.
    ldx #$0f
    stx _i
    _loop2:
        jsr Actors_ClearActor
        ldx _i
        dex
        stx _i
        bpl _loop2
    rts
.scend
    
Debug_CreatePlayerActor:
    ldx #$00
    stx Player_ActorIndex
    stx Actor_SortArray
    lda [ActFlg_IsDynamic|ActFlg_IsPlayer]
    sta Actor_Bitflags,x
    lda #$80
    sta Actor_X,x
    lda #$60
    sta Actor_Y,x
    lda #$00
    sta Actor_SuperChunk,x
    
    rts