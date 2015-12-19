; ==============================================================================
.require "../includes/defines.asm"

ThrowException:
{
    pla 
    sta ErrorAddr
    pla
    sta ErrorAddrHi
    _hang:
    lda #$bb
    bne _hang
}
