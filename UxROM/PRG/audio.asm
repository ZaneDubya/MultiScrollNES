; Audio bank
.alias origin $8000
.org origin

.require "../code/includes.asm" ; music data relies on defines

MusicDataSet0:							; $8000
.include "../data/audio/music0.asm"
.advance  origin+$2000,$02
MusicDataSet1:							; $a000
.include "../data/audio/music1.asm"
.advance  origin+$4000,$02