; SprLdr - Sprite Loader functionality. Manages allocation of Sprite CHRRAM.
SprLdr_Setup:
{
    lda #$80
    sta SprLdr_LoadThisSlot
    rts
}

; ==============================================================================
; SprLdr_AllocTiles - allocates a section of CHRRAM for an actor's sprite tiles.
; IN    x = the actor index that needs tiles loaded.
; OUT   1.  sets the upper 4 bits of the input actor's Actor_DrawData0 byte to
;           first allocated tile in CHRRAM.
;       2.  finds a free tile slot or a tile slot that already holds the
;           requested sprite index, and increments the use count for that slot.
;       3.  if the slot is newly allocated, then sets the $80 bit in the use
;           count.
; NOTE  Wipes out $00-$03, a, y. Preserves x.
SprLdr_AllocTiles:
{
    .alias  _actorIndex             $00
    .alias  _spriteIndex            $01
    .alias  _slotCount              $02
    .alias  _emptySlot              $03
    
    stx _actorIndex
    ldy Actor_Definition,x              ; y = actor definition index
    ldx ActorHeaderSprite,y             ; x = sprite definition index
    stx _spriteIndex
    ; check to see if this sprite is already loaded in memory.
    ldy #SprLdr_FirstSlot
    ; for (y == SprLdr_FirstSlot; y >= 0; y--)
    {
    _check_next_slot:
        lda SprLdr_SpriteIndexes,y
        cmp _spriteIndex
        bne _next
            ; this sprite index matches the one we're looking for... but it's
            ; only valid if it's in use. Check that.
            tax 
            lda SprLdr_UsageCounts,y
            beq _next
            inc SprLdr_UsageCounts,x
            ldx _actorIndex
            rts
        _next:
        dey
        bpl _check_next_slot
    }
    
    ; this sprite is not already loaded in memory. We must load it!
    ; get the slot count to hold this sprite - top 2 bits of SpriteHdrs_Tile.
    lda SpriteHdrs_Tile,x               ; format yxcc ccss, we want yx.
    rol
    rol
    rol
    and #$03
    sta _slotCount                      ; = number of chrram slots necessary.
    
    ; get the first unloaded set of slots with length equal to _slotCount.
    
    ldy #SprLdr_FirstSlot
    ; for (y == SprLdr_FirstSlot; y >= 0; y--)
    {
    _check_next_slot:
        lda SprLdr_UsageCounts,y            ; if this slot is UNUSED
        bne _next
        {                                   ; _emptySlot = y
            sty _emptySlot                  ; x = slot count
            ldx _slotCount                  ; while (x-- >= 0)
            {                               ; {
                _check_slot_count:          ;   y--
                dex                         ;   if (y < 0)    
                bmi _found_available_slot   ;       throw exception
                dey                         ;   a = SprLdr_UsageCounts,y
                bmi _did_not_fit            ;   if (a != 0) - if this slot is USED
                lda SprLdr_UsageCounts,y    ;   {
                beq _check_slot_count       ;       y = _emptySlot (restore y)
                ldy _emptySlot              ;       break
            }                               ;   }
        }                                   ; }    
        _next:
        dey
        bpl _check_next_slot
    }
    _did_not_fit:
    jsr ThrowException
    
    _found_available_slot:
    ; all sprite indexes are set to sprite index.
    ; 'first' (highest) use index is set to $81 (must load flag, hi bit)
    ; all othe use indexes set to $1.
    lda _emptySlot                  ; _emptySlot -= _slotCount
    `subm _slotCount
    sta _emptySlot
    ldx _emptySlot                  ; x = _emptySlot (first available slot)
    
    lda _spriteIndex
    ldy _slotCount
    {
        _next:
        sta SprLdr_SpriteIndexes,x
        inc SprLdr_UsageCounts,x
        inx
        dey
        bpl _next
    }
    
    ldx _emptySlot                  ; x = _emptySlot
    lda #$80                        ; set 'Must Load' flag on first use index.
    ora SprLdr_UsageCounts,x
    sta SprLdr_UsageCounts,x
    
    ldx _actorIndex
    rts
}
   
; ==============================================================================
; SprLdr_DeallocTiles - informs SprLdr that an actor that used this sprite def
; has been unloaded, and the CHRRAM used by the sprite def can be freed.
; IN    x = the actor index that is being unloaded.
; OUT   1. finds the tile slot that has the actor's sprite index loaded.
;       2. decrement's the slot's use count.
;       3. if (use count & $7f == 0), then clear the $80 bit, if it exists.
SprLdr_DeallocTiles:
{
    rts
}

; ==============================================================================
; SprLdr_LoadNMI - Called by NMI when there is enough time to load some tiles.
; If there are tiles left to be loaded, SprLdr will load 8 of them (128b).
; Will only be called if SprLdr_LoadOpReady != 0.
; Indexes to tiles are in SprLdr_SpriteLoads and SprLdr_PageLoads
; Address to copy these to is in SprLdr_NMIAddress.
; Takes 850-950 cycles. I think. No more than 1000.
; IN    None
; Out   Write tiles to CHRRAM.
; NOTE  Wipes out a, x, y, SprLdr_NMIAddress, and SprLdr_NMIAddressHi.
SprLdr_LoadNMI:
{
    `Mapper_SwitchBank Bank_TilGfxData
    lda SprLdr_NMIAddressHi
    sta PPU_ADDR
    lda SprLdr_NMIAddress
    sta PPU_ADDR
    ldx #$00
    {
    _nexttile:
        lda SprLdr_TileAddressesLo,x        ; 4
        sta SprLdr_NMIAddress               ; 3
        lda SprLdr_TileAddressesHi,x        ; 4
        sta SprLdr_NMIAddressHi             ; 3
        ldy #$00                            ; 2    
        ; Copy 16 bytes from ROM to PPU_DATA. Cycles: 11 * 16 = 176
        ; Unrolled this loop for speed at the expense of rom space.
        ; We're in NMI, so speed is far more important.
            lda (SprLdr_NMIAddress),y       ; 11. Iteration 1/16
            sta PPU_DATA                    ;
            iny                             ;
            lda (SprLdr_NMIAddress),y       ; 11. Iteration 2
            sta PPU_DATA                    ;
            iny                             ;
            lda (SprLdr_NMIAddress),y       ; 11. Iteration 3
            sta PPU_DATA                    ;
            iny                             ;
            lda (SprLdr_NMIAddress),y       ; 11. Iteration 4
            sta PPU_DATA                    ;
            iny                             ;
            lda (SprLdr_NMIAddress),y       ; 11. Iteration 5
            sta PPU_DATA                    ;
            iny                             ;
            lda (SprLdr_NMIAddress),y       ; 11. Iteration 6
            sta PPU_DATA                    ;
            iny                             ;
            lda (SprLdr_NMIAddress),y       ; 11. Iteration 7
            sta PPU_DATA                    ;
            iny                             ;
            lda (SprLdr_NMIAddress),y       ; 11. Iteration 8
            sta PPU_DATA                    ;
            iny                             ;
            lda (SprLdr_NMIAddress),y       ; 11. Iteration 9
            sta PPU_DATA                    ;
            iny                             ;
            lda (SprLdr_NMIAddress),y       ; 11. Iteration 10
            sta PPU_DATA                    ;
            iny                             ;
            lda (SprLdr_NMIAddress),y       ; 11. Iteration 11
            sta PPU_DATA                    ;
            iny                             ;
            lda (SprLdr_NMIAddress),y       ; 11. Iteration 12
            sta PPU_DATA                    ;
            iny                             ;
            lda (SprLdr_NMIAddress),y       ; 11. Iteration 13
            sta PPU_DATA                    ;
            iny                             ;
            lda (SprLdr_NMIAddress),y       ; 11. Iteration 14
            sta PPU_DATA                    ;
            iny                             ;
            lda (SprLdr_NMIAddress),y       ; 11. Iteration 15
            sta PPU_DATA                    ;
            iny                             ;
            lda (SprLdr_NMIAddress),y       ; 11. Iteration 16
            sta PPU_DATA                    ;
        inx                                 ; 2
        cpx #$04                            ; 2
        bne _nexttile                       ; 3/2 on last loop
    }
    lda #$00
    sta SprLdr_LoadOpReady
    lda #$04
    `addm SprLdr_LoadThisTile
    sta SprLdr_LoadThisTile
    rts
}

; ==============================================================================
; SprLdr_LoadAll - Calls SprLdr_Load until all CHRRAM Sprite tiles have been
; loaded. Should only be called when the PPU is disabled!
; Takes up to 58k cycles.
SprLdr_LoadAll:
{
    rts
}

; ==============================================================================
; SprLdr_Update - Called once a frame by main. Loops through all allocated slots
; and sets up a load operation for the first allocated and unloaded slot.
; IN    None
; OUT   Sets SprLdr vars in ZP and SprLdr RAM (see defines.asm)
; NOTE  Wipes out a,x,y,$00-$01
SprLdr_Update:
{
    .alias _ptr     $00
; first check if there is a current loading slot; if so, check if it loaded.
    ldx SprLdr_LoadThisSlot
    bmi _find_next_slot_to_load ; (a & $80) == $80, not loading a slot
    ; we have been loading a slot.
    lda SprLdr_LoadOpReady      ; if SprLdr_LoadOpReady == 1, load did not occur
    beq _check_continue_loading ; last NMI; so we don't need to set up a new
    rts                         ; load operation. if == 0, we do need to set up.
    
    _check_continue_loading:
    lda SprLdr_LoadThisTile
    cmp SplLdr_TilesToLoad
    bne _setup_next_load_op       ; slot is still being loaded...
    lda SprLdr_UsageCounts,x
    and #$7f
    sta SprLdr_UsageCounts,x
    
; find the next slot that needs to be loaded.
_find_next_slot_to_load:
    ldy #SprLdr_FirstSlot
    _nextSlot:
        lda SprLdr_UsageCounts,y
        bmi _load_slot_in_y
        dey
        bpl _nextSlot
    lda #$80                    ; no slots need to be loaded. Set LoadThisSlot
    sta SprLdr_LoadThisSlot     ; to $80, exit.
    rts
    
; set up SprLdr to load Sprite CHRRAM during NMI.
_load_slot_in_y:
    lda #$00
    sta SprLdr_LoadThisTile     ; SprLdr_LoadThisTile = $00
    sty SprLdr_LoadThisSlot     ; SprLdr_LoadThisSlot = slot
    
    ; get the slot count to hold this sprite - top 2 bits of SpriteHdrs_Tile + 1
    lda SprLdr_SpriteIndexes,y          ; a = sprite definition index to load
    tax
    lda SpriteHdrs_Tile,x; format yxcc ccss, we want yx + 1 multipled by 16.
    and #$c0
    lsr
    lsr 
    `add $10
    sta SplLdr_TilesToLoad      ; SplLdr_TilesToLoad = 16 * slots to load.
    
; create a load operation - copy 4 tile indexes to load to SprLdr_SpriteIndexes 
; and SprLdr_PageIndexes. There may be some garbage at the end of each slot -
; that's okay!
_setup_next_load_op:
    `Mapper_SwitchBank Bank_SprData
    ; set up the PPU Address to copy these tiles to during NMI
    lda SprLdr_LoadThisSlot     ; a = slot (range $0-$f)
    `add $10                    ; a = $10 | slot + tile
    sta SprLdr_NMIAddressHi
    lda SprLdr_LoadThisTile
    tax
    asl
    asl
    asl
    asl
    sta SprLdr_NMIAddress
    txa
    lsr
    lsr
    lsr
    lsr
    `addm SprLdr_NMIAddressHi
    sta SprLdr_NMIAddressHi
    
    ldx SprLdr_LoadThisSlot
    ldy SprLdr_SpriteIndexes,x
    lda SpriteHdrs_Data,y       ; count of metasprite frames, 0-31
    `add 1                      ; now 1-32
    ; multiply this value by the size of the metasprites (2,8,18,32).
    pha                         ; save a
    lda SpriteHdrs_Tile,y
    and #$03
    tax
    lda _metaspriteByteSizeTable,x
    tax                         ; x = size of metasprite in bytes.
    pla                         ; restore a. a = count of frames.
    jsr Multiply16              ; wipes out a and x. doesn't matter, we don't need them.
    lda SpriteHdrs_Address,y
    `addm MultiplySum
    sta [_ptr+0]
    lda SpriteHdrs_AddressHi,y
   `addm MultiplySumHi
    sta [_ptr+1]
    ; load and store tile and page indexes for 4 tiles from sprite's tile data.
    lda SprLdr_LoadThisTile
    asl
    tay
    ldx #$00
    {
    _next:
        lda (_ptr),y
        sta SprLdr_TileAddressesLo,x
        iny
        lda (_ptr),y
        sta SprLdr_TileAddressesHi,x
        iny
        inx
        cpx #$04
        bne _next
    }
    ; now multiply each of these 2-byte values by 16.
    {
    _next:
        dex
        lda SprLdr_TileAddressesHi,x
        asl
        asl
        asl
        asl
        clc
        adc #TileGfxAddressHi
        sta SprLdr_TileAddressesHi,x
        lda SprLdr_TileAddressesLo,x
        tay
        lsr
        lsr
        lsr
        lsr
        clc
        adc SprLdr_TileAddressesHi,x
        sta SprLdr_TileAddressesHi,x
        tya
        asl
        asl
        asl
        asl
        sta SprLdr_TileAddressesLo,x
        
        cpx #$00
        bne _next
    }
    ; indicate that a SprLdr load operation is ready to go, and return.
    lda #$01
    sta SprLdr_LoadOpReady
    rts
_metaspriteByteSizeTable:
    .byte 2,8,18,32
}