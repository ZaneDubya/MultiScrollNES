; Map bank
.alias origin $8000
.org origin
.incbin "../data/gfx/map_ptrs.dat"      ; 512 bytes
.incbin "../data/gfx/map_data.dat"      ; up to 13.5kb
.advance    origin+$3800,$00            ; at the 14kb border.
.incbin "../data/gfx/spr_data.dat"
.advance    origin+$3C00,$00            ; at the 15kb border.
.include "../data/actors/actortypes.asm"
.advance origin+$4000,$00