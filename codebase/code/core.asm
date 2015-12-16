.require "includes.asm"
.include "core/interrupts.asm"
.include "core/famitone2.asm"

.include "core/library.asm"
.include "core/controller.asm"
.include "core/sprites.asm"
.include "core/actors.asm"
.include "core/maps.asm"
.include "core/startup.asm"

.include "ModeWorld/ModeWorld.asm"
.include "ModeText/ModeText.asm"

.include "debug.asm"

;--------------------------------[ RunOneFrame ]--------------------------------
; The main engine routine. Called from NMI
RunOneFrame:
    `DebugShadePPU_Shaded
	`SetGameFlag FlagMainInProcess
    
	jsr FamiToneUpdate
	jsr UpdateTimers						; update Timer1 and Timer2
	jsr Input_Get							; poll gamepad
	jsr UpdateGameMode						; Main routine for updating game.
	
; Write sprites to OAM buffer.
	jsr Sprite_BeginFrame
	jsr Actors_DrawActors
	jsr Sprite_EndFrame
	
; Load new map data
	jsr MapService_LoadNewData

; Clear the flag main_in_progress and return
	`DebugShadePPU_Normal	
	`ClearGameFlag FlagMainInProcess
	rts

;------------------------------[ UpdateGameMode ]-------------------------------
; This is where the real code of each frame is executed. ChooseRoutine is
; called, the return pc address is popped off and used as the base address of
; a code pointer table, GameMode is the index of the table we want to call,
; and the routine at that index is executed.

UpdateGameMode:
	lda GameMode
	jsr ChooseRoutine		; Use GameMode as index into routine table below.
	
.word Reset					; Setting GameMode = 0 will reset the game
.word ModeWorld 			; World engine, for the overworld
.word TextEngine			; Text Engine

;-------------------------------[ ChooseRoutine ]-------------------------------
; This is an indirect jump routine. A is used as an index into a code pointer 
; table, and the routine at that position is executed. The pointer table itself 
; directly follows the JSR to ChooseRoutine, meaning that its address can be
; popped from the stack.
;   In:     Pointer in stack, A as index.
;   Out:    JSR to address at pointer+(A*2)
;   Notes:  Needs 4 bytes in zp for temp variables.
ChooseRoutine:
.scope
.alias _TempX				$0000
.alias _TempY				$0001
.alias _CodePtr				$0002	;Points to address to jump to when choosing
;      CodePtr+1			$0003	;a routine from a list of routine addresses.
	asl								;* 2, each ptr is 2 bytes (16-bit).
	sty _TempY						;Temp storage.
	stx _TempX						;Temp storage.
	tay								;
	iny								;
	pla								;Low byte of ptr table address.
	sta _CodePtr					;
	pla								;High byte of ptr table address.
	sta _CodePtr+1					;
	lda (_CodePtr),y				;Low byte of code ptr.
	tax								;
	iny								;
	lda (_CodePtr),y				;High byte of code ptr.
	sta _CodePtr+1					;
	stx _CodePtr					;
	ldx _TempX						;Restore X.
	ldy _TempY						;Restore Y.
	jmp (_CodePtr)					;
.scend