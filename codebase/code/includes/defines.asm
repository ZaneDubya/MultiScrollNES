; This is the Galezie Memory Map. It is organized into three sections:
;       * Memory Aliases
;       * Enumerations & Defines
;       * Registers
; ================================ Memory Aliases ==============================
; $000x - Scratch space. Any routine can overwrite these.

; $001x - FREE. AVAILABLE: ALL (16)

; $002x - core variables. AVILABILE: $2C-$2F (4).
.alias  GameMode            $0020
.alias  GameFlags           $0021
.alias  BankIn8000          $0022
.alias  BankIn8000_Saved    $0023
.alias  FrameCount          $0024   ; Increments each frame, rolls over at 256.
.alias  Ctrl0_Now           $0025   ; control pad - current status
.alias  Ctrl0_Last          $0026   ; control pad - status last frame
.alias  Ctrl0_New           $0027   ; control pad - new button pushes
.alias  TVSystem            $0028   ; 0=ntsc, 1=pal, 2=dendy, 3=unknown
.alias  Screen_X            $0029   ; NMI's local copy of CameraCurrentX
.alias  Screen_Y            $002A   ; Y offset for screen. Rolls over at 239.
.alias  Player_ActorIndex   $002B   ; $80 if no player actor, otherwise $00-$0F

; $003x - values for library routines. AVAILABLE: $37-$3C (6)
.alias  Random_Seed         $0030
.alias  TimerDelay          $0031   ; Count down from 9 to 0. Dec each frame.
.alias  Timer1              $0032   ; Decremented every frame after set.
.alias  Timer2              $0033   ; Decremented every 10 frames after set.
.alias  Multiplier          $0034
.alias  Numerator           $0034
.alias  Multiplicand        $0035
.alias  Denominator         $0035
.alias  MultiplySum         $0035
.alias  MultiplySumHi       $0036

.alias  OamCurrentSprChk    $003D
.alias  OamCurrentIndex     $003E
.alias  OamFull             $003F

; $004x - Camera variables. AVAILABLE: NONE
.alias  CameraCurrentX      $0040
.alias  CameraCurrentX2     $0041
.alias  CameraCurrentY      $0042
.alias  CameraCurrentY2     $0043
.alias  CameraTargetX       $0044
.alias  CameraTargetX2      $0045
.alias  CameraTargetY       $0046
.alias  CameraTargetY2      $0047
.alias  CameraBoundL        $0048
.alias  CameraBoundL2       $0049
.alias  CameraBoundT        $004A
.alias  CameraBoundT2       $004B
.alias  CameraBoundR        $004C
.alias  CameraBoundR2       $004D
.alias  CameraBoundB        $004E
.alias  CameraBoundB2       $004F

; $005x - $00Bx - Map data.
.alias  MapBuffer           $0050
.alias  MapBuffer_SC_UL_X   MapBuffer+$00
.alias  MapBuffer_SC_UL_Y   MapBuffer+$01
.alias  MapBuffer_Last_X    MapBuffer+$02
.alias  MapBuffer_Last_Y    MapBuffer+$03
.alias  MapBuffer_C_PPUADDR MapBuffer+$04
.alias  MapBuffer_R_PPUADDR MapBuffer+$06
.alias  MapBuffer_Flags     MapBuffer+$08   ; see MapData enum
;AVAILABLE: $0059 - $005B (3 bytes)
.alias  SprPalette0         MapBuffer+$0C   ; Sprite Pal 0
.alias  SprPalette1         MapBuffer+$0D   ; Sprite Pal 1
.alias  SprPalette2         MapBuffer+$0E   ; Sprite Pal 2
.alias  SprPalette3         MapBuffer+$0F   ; Sprite Pal 3
.alias  MapData_RowBuffer   MapBuffer+$10   ; $006x Buffers for VBLANK PPU data.
.alias  MapData_ColBuffer   MapBuffer+$30   ; $008x Buffers for VBLANK PPU data.
.alias  MapData_RowAttrBuf  MapBuffer+$50   ; $00Ax Buffers for VBLANK PPU data.
.alias  MapData_ColAttrBuf  MapBuffer+$58   ; $00Ax Buffers for VBLANK PPU data.
.alias  MapData_Chunks      MapBuffer+$60   ; $00Bx Indexes of on-screen superchunks.

; $00Cx - The currently loaded tileset
.alias  Tileset_ZP          $00C0
.alias  Tileset_PtrBits     Tileset_ZP+$00
.alias  Tileset_PtrAttribs  Tileset_ZP+$02
.alias  Tileset_PtrUL       Tileset_ZP+$04
.alias  Tileset_PtrUR       Tileset_ZP+$06
.alias  Tileset_PtrLL       Tileset_ZP+$08
.alias  Tileset_PtrLR       Tileset_ZP+$0A
.alias  Tileset_Pal0        Tileset_ZP+$0C
.alias  Tileset_Pal1        Tileset_ZP+$0D
.alias  Tileset_Pal2        Tileset_ZP+$0E
.alias  Tileset_Pal3        Tileset_ZP+$0F

; $00Dx -- Actor sort array. AVAILABLE: NONE.
.alias  Actor_SortArray     $00D0   ; List of Actors, sorted based on y value
                                    ; $80 = no Actor in this slot.
                                    ; highest y value should be in highest slot

; $00Ex - AVAILABLE.
                                    
; $00Fx - Flags and save data. AVAILABLE: $F4 - $FF (12)
.alias  ProgressFlag0           $00F0 ; $F0-$F2 = 24 bits of progression
.alias  TemporaryFlag           $00F3 ; unsaved, reset on location change

; $01xx - VBLANK Buffer and Stack. AVAILABLE: $0180 - $01BE (63)
.alias  CHRRAM_BUFFER           $0100    
.alias  STACK_OVERFLOW          $01BF
; $01C0 - $01FF - Stack (64b)

; $02xx - OAM Buffer. AVAILABLE: NONE.
.alias  OAM_BUFFER              $0200

; $03xx  - Actor data (16 total). AVAILABLE: NONE.
; for description of this data, see ref.data\Engines\Actors.txt
.alias  Actor_Bitflags          $0300   
.alias  Actor_X                 $0310
.alias  Actor_Y                 $0320
.alias  Actor_SuperChunk        $0330
.alias  Actor_DrawData0         $0340
.alias  Actor_DrawData1         $0350
.alias  Actor_UNUSED            $0360
.alias  Actor_Definition        $0370
.alias  Actor_ScriptPtr         $0380
.alias  Actor_ScriptPtrHi       $0390
.alias  Actor_Var0              $03A0
.alias  Actor_Var1              $03B0
.alias  Actor_Var2              $03C0
.alias  Actor_Var3              $03D0
.alias  Actor_Var4              $03E0
.alias  Actor_Var5              $03F0

; $04xx - Famitone and Sprite data.
.alias  FT_BASE_ADR             $0400   ; $BA bytes. 
.alias  SpriteLoaderDefs        $04E0    
.alias  SpriteLoaderUses        $04F0    

; $05xx - AVAILABLE.

; $06xx - AVAILABLE.

; $0700-$07FF is available for use by the current engine. Since any active
; engine can overwrite this data, an engine can only store data here as long
; as it is active.
; ---------------------------------- TextEngine --------------------------------
.alias  TEXT_String         $0700   ; Buffer to decompress and translate string.
.alias  TEXT_Characters     $0780   ; Chars to be copied. Max 47 (space for 48,
                                    ; but using 48 screws up MMC3 IRQ counting).
.alias  TEXT_Action         $07B0   ; What we want text engine to do. See enum.
.alias  TEXT_Stage          $07B1   ; At what stage is current action on?
.alias  TEXT_NumChars       $07B2   ; number of chars in TEXT_Characters
.alias  TEXT_StatementLo    $07B3   ; Index of statement.
.alias  TEXT_StatementHi    $07B4   ; Bank of statement.

.require "defines_enums.asm"
.require "defines_banks.asm"
.require "defines_registers.asm"