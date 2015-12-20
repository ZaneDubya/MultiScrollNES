; Gfx bank
.alias origin $8000
.org origin

; ADDR    SIZE    DESCRIPTION
; 8000    2000    Chunks
.incbin "data/gfx/chunks.dat"

; A000    1000    TileGfx Page 0 (256 tiles)
.advance origin+$2000,$00
.incbin "data/gfx/tilegfx.dat", $0000, $1000

; B000    1000    TileGfx Page  1 (256 tiles)
.advance origin+$3000,$00
.incbin "data/gfx/tilegfx.dat", $1000, $1000

.advance  origin+$4000,$01