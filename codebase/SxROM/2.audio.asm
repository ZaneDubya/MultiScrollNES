; Audio bank
.require "code/includes.asm" ; music data relies on defines
.alias origin $8000
.org origin

; ADDR    SIZE    DESCRIPTION
; 8000    2000    Famitone Song 0
MusicDataSet0:
.include "data/audio/music0.asm"
.advance  origin+$2000,$02

; A000    2000    Famitone Song 1
MusicDataSet1:
.include "data/audio/music1.asm"
.advance  origin+$4000,$02