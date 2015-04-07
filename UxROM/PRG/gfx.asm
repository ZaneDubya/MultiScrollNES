; Gfx bank
.alias origin $8000
.org origin

.incbin "../data/gfx/chunks.dat"

.advance origin+$2000,$00
.incbin "../data/gfx/tilegfx.dat", $1000, $1000
.advance origin+$3000,$00
.incbin "../data/gfx/tilegfx.dat", $0000, $1000

.advance  origin+$4000,$01