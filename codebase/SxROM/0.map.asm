; Map bank
.alias origin $8000
.org origin

; ADDR    SIZE    DESCRIPTION
; 8000     200    Map pointers: 16 x 16 x 2b superchunk pointers.
.incbin "data/gfx/map_ptrs.dat"         ; 512b ptrs (16 x 16 x 2b ptrs)

; 8200    1E00    Map chunk data: actors and egg scripts.
.incbin "data/gfx/map_data.dat"
.advance    origin+$2000,$00 

; A000    1000    Sprite data
.incbin "data/gfx/spr_data.dat"
.advance    origin+$3000,$00 

; B000    1000    Actor scripts
.advance origin+$4000,$00