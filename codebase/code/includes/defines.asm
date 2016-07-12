; This is the Main Memory Map. It is organized into three sections:
;       * Memory Aliases
;       * Enumerations & Defines
;       * Registers
; ================================ Memory Aliases =============================

; $000x - $003x - Scratch space  ----------------------------------------------
; 64b total. Any routine can overwrite these.


; $004x - Camera variables ----------------------------------------------------
; AVAILABLE: NONE 
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


; $005x - $00Ax - Map data ----------------------------------------------------
; AVAILABLE: $005B
.alias  MapBuffer           $0050
.alias  MapBuffer_SC_UL_X   MapBuffer+$00
.alias  MapBuffer_SC_UL_Y   MapBuffer+$01
.alias  MapBuffer_Last_X    MapBuffer+$02
.alias  MapBuffer_Last_Y    MapBuffer+$03
.alias  MapBuffer_C_PPUADDR MapBuffer+$04
.alias  MapBuffer_R_PPUADDR MapBuffer+$06
.alias  MapBuffer_Flags     MapBuffer+$08   ; see MapData enum
.alias  MapBuffer_RA_Index  MapBuffer+$09   ; Attribute Row Index.
.alias  MapBuffer_CA_Index  MapBuffer+$0A   ; Attribute Column Index.
.alias  SprPalette0         MapBuffer+$0C   ; Sprite Pal 0
.alias  SprPalette1         MapBuffer+$0D   ; Sprite Pal 1
.alias  SprPalette2         MapBuffer+$0E   ; Sprite Pal 2
.alias  SprPalette3         MapBuffer+$0F   ; Sprite Pal 3
.alias  MapData_RowBuffer   MapBuffer+$10   ; $006x Buffers for VBLANK PPU data.
.alias  MapData_ColBuffer   MapBuffer+$30   ; $008x Buffers for VBLANK PPU data.
.alias  MapData_Chunks      MapBuffer+$50   ; $00Ax 4 screens worth of superchunk
                                            ;       indexes.


; $00Bx - Sprite Loader Library. AVAILABLE: $00B6-$00BF (10 - maybe!) ---------
.alias  SprLdr_NMIAddress       $00B0 ; address to load data to in NMI.
.alias  SprLdr_NMIAddressHi     $00B1 ; 
.alias  SprLdr_LoadThisSlot     $00B2 ; current slot being loaded
.alias  SprLdr_LoadThisTile     $00B3 ; next tile to load (0,8,16,24,32,48,etc.)
.alias  SplLdr_TilesToLoad      $00B4 ; max tile to load (16,32,48,or 64)
.alias  SprLdr_LoadOpReady      $00B5 ; 1 if ready to load tiles, 0 if not.


; $00Cx - The currently loaded tileset ----------------------------------------
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


; $00Dx -- Actor sort array ---------------------------------------------------
; AVAILABLE: NONE
.alias  Actor_SortArray     $00D0   ; List of Actors, sorted based on y value
                                    ; $80 = no Actor in this slot.
                                    ; highest y value should be in highest slot


; $00E2x - core variables -----------------------------------------------------
; AVAILABILE: $EC-$EF (4)
.alias  GameMode            $00E0
.alias  GameFlags           $00E1
.alias  BankIn8000          $00E2
.alias  BankIn8000_Saved    $00E3
.alias  FrameCount          $00E4   ; Increments each frame, rolls over at 256.
.alias  Ctrl0_Now           $00E5   ; control pad - current status
.alias  Ctrl0_Last          $00E6   ; control pad - status last frame
.alias  Ctrl0_New           $00E7   ; control pad - new button pushes
.alias  TVSystem            $00E8   ; 0=ntsc, 1=pal, 2=dendy, 3=unknown
.alias  Screen_X            $00E9   ; NMI's local copy of CameraCurrentX
.alias  Screen_Y            $00EA   ; Y offset for screen. Rolls over at 239.
.alias  Player_ActorIndex   $00EB   ; $80 if no player actor, otherwise $00-$0F


; $00Fx - values for library routines -----------------------------------------
; AVAILABLE: $F7-$FC (6)
.alias  Random_Seed         $00F0
.alias  TimerDelay          $00F1   ; Count down from 9 to 0. Dec each frame.
.alias  Timer1              $00F2   ; Decremented every frame after set.
.alias  Timer2              $00F3   ; Decremented every 10 frames after set.
.alias  Multiplier          $00F4
.alias  Numerator           $00F4
.alias  Multiplicand        $00F5
.alias  Denominator         $00F5
.alias  MultiplySum         $00F5
.alias  MultiplySumHi       $00F6
.alias  ErrorAddr           $00F7
.alias  ErrorAddrHi         $00F8
.alias  Mod15Temp           $00F4
.alias  OamCurrentSprChk    $00FD
.alias  OamCurrentIndex     $00FE
.alias  OamFull             $00FF


; $01xx - Famitone and Stack --------------------------------------------------
; AVAILABLE: $01BA - $01BE (63)
.alias  FT_BASE_ADR             $0100   ; $BA bytes.   
.alias  STACK_OVERFLOW          $01BF
; $01C0 - $01FF - Stack (64b)


; $02xx - OAM Buffer ----------------------------------------------------------
.alias  OAM_BUFFER              $0200


; $03xx  - Actor data (16 total) ----------------------------------------------
; for description of this data, see ref.data\Engines\Actor Data.txt
.alias  Actor_Bitflags          $0300   
.alias  Actor_Definition        $0310
.alias  Actor_DrawData0         $0320
.alias  Actor_DrawData1         $0330
.alias  Actor_X                 $0340
.alias  Actor_Y                 $0350
.alias  Actor_SuperChunkX       $0360
.alias  Actor_SuperChunkY       $0370
.alias  Actor_ScriptPtr         $0380
.alias  Actor_ScriptPtrHi       $0390
.alias  Actor_Var0              $03A0
.alias  Actor_Var1              $03B0
.alias  Actor_Var2              $03C0
.alias  Actor_Var3              $03D0
.alias  Actor_Var4              $03E0
.alias  Actor_Var5              $03F0


; $04xx - Sprite Loader Library -----------------------------------------------
.alias  SprLdr_SpriteIndexes    $0400
.alias  SprLdr_UsageCounts      $0410 
.alias  SprLdr_TileAddressesLo  $0420
.alias  SprLdr_TileAddressesHi  $0428
; $043x - available
.alias  MapData_Attributes      $0440   ; $40 bytes - attribute table 0
; $0480 - $04ff is AVAILABLE.


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
.require "defines_sprldr.asm"