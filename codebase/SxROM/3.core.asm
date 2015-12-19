; Core Engine loaded at $C000
.alias origin $C000
.org origin

; ADDR    SIZE    DESCRIPTION
; C000    1000    DPCM reserved space (4kb)
DpcmData:
	.incbin "data/audio/music_dpcm.dmc"
    .advance origin+$1000,$00

; D000     800    SFX reserved space (2kb)    
SfxData:
	.include "data/audio/sounds.asm"
    .advance origin+$1800,$00
    
; D800     600    Tileset 00 - this will go in a 'tileset' bank.
TilesetData:
	.incbin "data/gfx/tilesets.dat", 0, 1536	; omit tile indexes (312b), we
    .advance origin+$1E00,$00                   ; auto load first 256 tiles.    
    
; DE00      80    Sprite Headers (4b ea, 32 total).
SpriteHeaders:
	.include "data/gfx/spr_hdrs.asm"
    .advance origin+$1E80,$00
    
; DE80      80    Actor Headers (8b ea, 16 total).
ActorHeaders:
    .include "data/actors/actor_hdrs.asm"
    .advance origin+$1F00,$00
    
; DF00      20    Map Headers (4b ea, 8 total).
MapHeaders:
	.incbin "data/gfx/map_hdrs.dat"
    .advance origin+$1F20,$00
    
; DF20      E0    Palettes (224b, 3b ea, 74 total + 2b)
Palettes:
	.incbin "data/gfx/palettes.dat"
    .advance origin+$2000,$00
    
; E000    2000    Core Engine Code
.include "code/core.asm"  ; Core engine code!
.include "bankswitch.asm" ; Mapper specific routines and defines.

; Interrupt vector table
.advance    $fffa,$FF
.word       NMI
.word       Reset
.word	    Reset