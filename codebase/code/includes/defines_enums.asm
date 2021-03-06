; ================================ Enumerations ================================
; GameFlags Bitflag Enumeration
.alias  FlagDoNMI           $01
.alias  FlagMainInProcess   $02
.alias  FlagNoVBLastFrame   $04
.alias  FlagDialogueOpen    $08

; GameMode Enumeration
.alias GameModeReset        $00
.alias GameModeWorld        $01
.alias GameModeText         $02

; MapData Enumerations
.alias  MapData_HasColData  $01
.alias  MapData_HasColAttr  $02
.alias  MapData_HasRowData  $10
.alias  MapData_HasRowAttr  $20

; Actor bitflag enumerations.
; REFERENCE ref.data\Engines\Actor Data.txt 2.A
.alias  ActFlg_IsActive         $01
.alias  ActFlg_UNUSED           $02
.alias  ActFlg_IsAction         $04
.alias  ActFlg_IsMoving         $08
.alias  ActFlg_IsDynamic        $10
.alias  ActFlg_IsInteractable   $20
.alias  ActFlg_IsBlocking       $40
.alias  ActFlg_IsVisible        $80

; TextEngine Actions
.alias TEA_DisplayMsg       $01     ; Displays a message box, waits for input.
.alias TEA_LoadIntoMem      $02     ; Decompress a string into memory.

;FamiTone settings
.alias  FT_TEMP             $00     ;3 bytes in zeropage used as a scratchpad
.alias  FT_SFX_STREAMS      4       ;# sound effects played at once, 4 or less
;internal defines
.alias  FT_DPCM_DATA        $C000
.alias  FT_DPCM_PTR         [FT_DPCM_DATA & $3fff]/64

; PPU ops - usually found in VBLANK_OpData
.alias  PPUOp_Break         $00
.alias  PPUOp_UnRLEData     $01
.alias  PPUOp_SwitchBank    $02
.alias  PPUOp_SetAddress    $11
.alias  PPUOp_CopyDataBLen  $12
.alias  PPUOp_ChrCopy       $13
.alias  PPUOp_SubRoutine    $14

; defines for controller reading
.alias  CTRL_A              $0001
.alias  CTRL_B              $0002
.alias  CTRL_SELECT         $0004
.alias  CTRL_START          $0008
.alias  CTRL_UP             $0010
.alias  CTRL_DOWN           $0020
.alias  CTRL_LEFT           $0040
.alias  CTRL_RIGHT          $0080
.alias  CTRL_UDLR           $00F0
.alias  CTRL_UPDOWN         $0030
.alias  CTRL_LEFTRIGHT      $00C0

; defines for facings
.alias  FACING_UP           $00
.alias  FACING_RIGHT        $01
.alias  FACING_DOWN         $02
.alias  FACING_LEFT         $03