; defines for bank indexes
.alias  Bank_MapData        $00
.alias  Bank_SprData        $00
.alias  Bank_ChkData        $01         ; chunk data
.alias  Bank_TilGfxData     $01
.alias  Bank_SfxData        $02
.alias  Bank_MusicData      $02
.alias  Bank_DPCMData       $03         ; fixed - never used.

.alias  MusicDataHi         $80
.alias  MusicDataLo         $00

.alias  ChunkData           $8000

.alias  MusicDataSet0       $8000       ; these come from PRG/audio.asm
.alias  MusicDataSet1       $a000       ; these come from PRG/audio.asm

.alias  TileGfxAddressHi    $a0