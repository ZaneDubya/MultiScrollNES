; ================================ Enumerations ================================
; GameFlags Bitflag Enumeration
.alias	FlagDoNMI			$01
.alias	FlagMainInProcess	$02
.alias	FlagNoVBLastFrame	$04
.alias	FlagDialogueOpen	$08

; GameMode Enumeration
.alias GameModeReset        $00
.alias GameModeWorld        $01
.alias GameModeText    		$02

; MapData Enumerations
.alias	MapData_HasColData	$01
.alias	MapData_HasColAttr	$02
.alias	MapData_HasRowData	$10
.alias	MapData_HasRowAttr	$20

; Actor bitflag enumerations
.alias  ActFlg_IsInteractable   $01 ; 'a' interaction
.alias  ActFlg_IsBlocking       $02 ; blocks movement
.alias  ActFlg_IsRunning        $04 ; is instantiated
.alias  ActFlg_IsPlayer         $20 ; is player actor
.alias  ActFlg_IsDynamic        $40
.alias  ActFlg_IsVisible        $80

; TextEngine Actions
.alias TEA_DisplayMsg		$01		; Displays a message box, waits for input.
.alias TEA_LoadIntoMem		$02		; Decompress a string into memory.

;FamiTone settings
.alias	FT_TEMP			  	$00		;3 bytes in zeropage used as a scratchpad
.alias	FT_SFX_STREAMS		4		;# sound effects played at once, 4 or less
;internal defines
.alias	FT_DPCM_DATA		$C600
.alias	FT_DPCM_PTR		  	[FT_DPCM_DATA & $3fff]/64

; PPU ops - usually found in VBLANK_OpData
.alias	PPUOp_Break			$00
.alias	PPUOp_UnRLEData		$01
.alias	PPUOp_SwitchBank	$02
.alias	PPUOp_SetAddress	$11
.alias	PPUOp_CopyDataBLen	$12
.alias	PPUOp_ChrCopy		$13
.alias	PPUOp_SubRoutine	$14

; defines for controller reading
.alias PAD_A				$0001
.alias PAD_B				$0002
.alias PAD_SELECT			$0004
.alias PAD_START			$0008
.alias PAD_UP				$0010
.alias PAD_DOWN				$0020
.alias PAD_LEFT				$0040
.alias PAD_RIGHT			$0080