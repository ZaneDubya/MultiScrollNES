; Core Engine loaded at $C000
.org $C000
TilesetData:
	.incbin "data/gfx/tilesets.dat", 0, 1536	; omit the tile indexes (312b)
												; as we are auto-loading the
												; first 256 tiles.
DpcmData:
	.incbin "data/audio/music_dpcm.dmc"
SfxData:
	.include "data/audio/sounds.asm"
SpriteHeaders:
	.incbin "data/gfx/spr_hdrs.dat", 1
MapHeaders:
	.incbin "data/gfx/map_hdrs.dat"
Palettes:
	.incbin "data/gfx/palettes.dat"

.include "code/core.asm"

; Bank specific routines and defines.
.include "mmc1.bankswitch.asm"

.advance  $fffa,$FF
.word   NMI
.word   Reset
.word	Reset