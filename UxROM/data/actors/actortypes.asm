; Actortypes.asm: all the actor types and scripts.
.scope

    ; Debug Player object - index 000
    .byte $00, $00, %11010000, $00
    .word _DebugPlayerInteract, _DebugPlayerScript

_DebugPlayerInteract:

_DebugPlayerScript:

.scend