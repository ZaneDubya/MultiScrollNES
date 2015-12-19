; Actortypes.asm: all the actor types and scripts.
; for a description of this file, see ref.data\Engines\Actors.txt S 1.A.
; this file contains 16 interleaved values.

ActorHeaderData:                                    ; sprite data bitfield
    .byte $07, $00, $00, $00
    .byte $00, $00, $00, $00
    .byte $00, $00, $00, $00
    .byte $00, $00, $00, $00
ActorHeaderSprite:                                  ; sprite def index
    .byte $00, $00, $00, $00
    .byte $00, $00, $00, $00
    .byte $00, $00, $00, $00
    .byte $00, $00, $00, $00
ActorHeaderAnimatePtr:                              ; animation script
    .byte <ActDrw_Std, $00, $00, $00
    .byte $00, $00, $00, $00
    .byte $00, $00, $00, $00
    .byte $00, $00, $00, $00
ActorHeaderAnimatePtrHi:
    .byte >ActDrw_Std, $00, $00, $00
    .byte $00, $00, $00, $00
    .byte $00, $00, $00, $00
    .byte $00, $00, $00, $00
ActorHeaderInteractPtr:                             ; interaction script
    .byte $00, $00, $00, $00
    .byte $00, $00, $00, $00
    .byte $00, $00, $00, $00
    .byte $00, $00, $00, $00
ActorHeaderInteractPtrHi:
    .byte $00, $00, $00, $00
    .byte $00, $00, $00, $00
    .byte $00, $00, $00, $00
    .byte $00, $00, $00, $00
ActorHeaderScriptPtr:                               ; dynamic script entry point
    .byte $00, $00, $00, $00
    .byte $00, $00, $00, $00
    .byte $00, $00, $00, $00
    .byte $00, $00, $00, $00
ActorHeaderScriptPtrHi:
    .byte $00, $00, $00, $00
    .byte $00, $00, $00, $00
    .byte $00, $00, $00, $00
    .byte $00, $00, $00, $00
    