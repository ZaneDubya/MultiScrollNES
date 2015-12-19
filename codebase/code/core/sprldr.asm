; SprLdr - Sprite Loader functionality. Manages allocation of Sprite CHRRAM.

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
    ldy SprLdr_FirstSlot
    ; for (y == SprLdr_FirstSlot; y >= 0; y--)
    {
    _check_next_slot:
        lda SprLdr_SpriteIndexes,y
        cmp _spriteIndex
        bne _next
            tax
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
    ldy SprLdr_FirstSlot
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
    ldx _emptySlot
    lda #$80                        ; set 'Must Load' flag on first use index.
    sta SprLdr_UsageCounts,x
    lda _spriteIndex
    ldy _slotCount
    {
        _next:
        sta SprLdr_SpriteIndexes
        inc SprLdr_UsageCounts,x
        dey
        bpl _next
    }
    
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
; Takes between 1750-1800 Cycles.
; IN    None
; Out   Write tiles to CHRRAM.
;       A = 1 if tiles were loaded, 0 otherwise.
SprLdr_LoadNMI:
{
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
{
    rts
}