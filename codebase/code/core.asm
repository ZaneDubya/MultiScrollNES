; core.asm - contains code located in the fixed PRGROM bank at $C000.
; For final release, include spacing bytes to reduce page crossing branches.

; ==============================================================================
; included code files.
.require "includes.asm"
.include "core/interrupts.asm"
.include "core/famitone2.asm"
.include "core/library.asm"
.include "core/controller.asm"
.include "core/sprites.asm"
.include "core/sprldr.asm"
.include "core/actors.asm"
.include "core/MapSvc.asm"
.include "core/startup.asm"
.include "core/exception.asm"
.include "ModeWorld/ModeWorld.asm"
.include "ModeText/ModeText.asm"
.include "debug.asm"

; ==============================================================================
; RunOneFrame   Called from NMI once per frame if FlagMainInProcess is clear.
;               Runs all code that counts as a single update.
;               Sets up data to load during next NMI.
; RET           Falls through to the endless loop at the end of Startup.
RunOneFrame:
    `SetGameFlag FlagMainInProcess
    
    jsr FamiToneUpdate              ; music engine - should go at end of nmi.
    jsr UpdateTimers                ; update Timer1 and Timer2
    jsr Input_Get                   ; poll gamepad
    jsr UpdateGameMode              ; Main routine for updating game.
    
; Write sprites to OAM buffer.
    `DebugShadePPU_Blue
    jsr Sprite_BeginFrame
    jsr Actors_DrawActors
    jsr Sprite_EndFrame
    
; Load new map data
    `DebugShadePPU_Green
    jsr MapSvc_LoadNewData

; Clear the flag main_in_progress and return
    `DebugShadePPU_Normal   
    `ClearGameFlag FlagMainInProcess
    rts

; ==============================================================================
; UpdateGameMode    This is where the real code of each frame is executed.
;                   ChooseRoutine is called, the return pc address is popped off
;                   and used as the base address of a code pointer table,
;                   GameMode is used as the index of the table we want to call,
;                   and the routine at that index is executed.
UpdateGameMode:
    lda GameMode
    jsr ChooseRoutine       ; Use GameMode as index into routine table below.
    
.word Reset                 ; Setting GameMode = 0 will reset the game
.word ModeWorld             ; World engine, for the overworld
.word TextEngine            ; Text Engine

; ==============================================================================
; ChooseRoutine     This is an indirect jump routine. A is used as an index into
;                   a code pointer table, and the routine at that position is
;                   executed. The pointer table itself  directly follows the JSR
;                   to ChooseRoutine, meaning that its address can be popped
;                   from the stack.
; IN    Pointer in stack, A as index.
; OUT   JSR to address at pointer+(A*2)
; CLBR  Clobbers $00-$03.
ChooseRoutine:
{
.alias _TempX               $0000
.alias _TempY               $0001
.alias _CodePtr             $0002   ;Points to address to jump to when choosing
;      CodePtr+1            $0003   ;a routine from a list of routine addresses.

    asl                             ; * 2, as each ptr is 2 bytes (16-bit).
    sty _TempY                      ; Temp storage.
    stx _TempX                      ; Temp storage.
    tay                             ;
    iny                             ;
    pla                             ; Low byte of ptr table address.
    sta _CodePtr                    ;
    pla                             ; High byte of ptr table address.
    sta _CodePtr+1                  ;
    lda (_CodePtr),y                ; Low byte of code ptr.
    tax                             ;
    iny                             ;
    lda (_CodePtr),y                ; High byte of code ptr.
    sta _CodePtr+1                  ;
    stx _CodePtr                    ;
    ldx _TempX                      ; Restore X.
    ldy _TempY                      ; Restore Y.
    jmp (_CodePtr)                  ;
}