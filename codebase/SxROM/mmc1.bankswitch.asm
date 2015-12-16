; Mapper specific code for MMC1, iNES Mapper 001. By Zane Wagner, 2013.
; Unlike almost all other mappers, the MMC1 is configured through a serial port.
; CPU $8000-$FFFF is connected to a common shift register. Writing a value with
; bit 7 set ($80 through $FF) to any address in $8000-$FFFF clears the shift
; register to its initial state. To change a register's value, the CPU writes
; five times with bit 7 clear and a bit of the desired value in bit 0. On the
; first four writes, the MMC1 shifts bit 0 into a shift register. On the fifth
; write, the MMC1 copies bit 0 and the shift register contents into an internal
; register selected by bits 14 and 13 of the address, and then it clears the
; shift register. Only on the fifth write does the address matter, and even
; then, only bits 14 and 13 of the address matter because the mapper registers
; are incompletely decoded like the PPU registers. After the fifth write, the
; shift register is cleared automatically, so a write to the shift register with
; bit 7 on to reset it is not needed. [http://wiki.nesdev.com/w/index.php/MMC1]

.alias  MMC1_CONTROL        $8000
.alias  MMC1_CHR0           $A000
.alias  MMC1_CHR1           $C000
.alias  MMC1_PRGBANK        $E000

.alias  MMC1_RESET          $80

.alias  MMC1_MIRR_1_2000    $00
.alias  MMC1_MIRR_1_2400    $01  
.alias	MMC1_MIRR_V			$02
.alias	MMC1_MIRR_H			$03

.alias  MMC1_PRG_32K        $04
.alias  MMC1_PRG_FIX8000    $08 ; fix first bank at 8000, switches bank at C000
.alias  MMC1_PRG_FIXC000    $0C ; fix last bank at C000, switches bank at 8000
.alias  MMC1_CHR_8KBBank    $00 ; 1 8kb CHR Bank.
.alias  MMC1_CHR_4KBBanks   $10 ; 2 4kb CHR Banks, independetly switchable.

.alias  MMC1_PRG_Bank       $0F  ; 4 address lines to select bank 0-15.
.alias  MMC1_PRGRAM_Enable  $00  ; Enables 8kb PRGRAM at $6000-$7FFF.
.alias  MMC1_PRGRAM_Disable $10  ; Disables 8kb PRGRAM at $6000-$7FFF.

; sets up the initial mapper state.
.macro Mapper_Setup
    ; reset mapper
    lda #MMC1_RESET
    sta MMC1_CONTROL
    ; fix bank at $C000, single screen 
    lda #[MMC1_PRG_FIXC000|MMC1_MIRR_1_2000]
    sta MMC1_CONTROL
    lsr
    sta MMC1_CONTROL
    lsr
    sta MMC1_CONTROL
    lsr
    sta MMC1_CONTROL
    lsr
    sta MMC1_CONTROL
.macend

; show single screen $2000
.macro Mapper_Show2000
    ; fix bank at $C000, single screen 
    lda #[MMC1_PRG_FIXC000|MMC1_MIRR_1_2000]
    sta MMC1_CONTROL
    lsr
    sta MMC1_CONTROL
    lsr
    sta MMC1_CONTROL
    lsr
    sta MMC1_CONTROL
    lsr
    sta MMC1_CONTROL
.macend

; show single screen $2400
.macro Mapper_Show2400
    ; fix bank at $C000, single screen 
    lda #[MMC1_PRG_FIXC000|MMC1_MIRR_1_2400]
    sta MMC1_CONTROL
    lsr
    sta MMC1_CONTROL
    lsr
    sta MMC1_CONTROL
    lsr
    sta MMC1_CONTROL
    lsr
    sta MMC1_CONTROL
.macend

; switch bank at $8000-$bfff to value in macro statement.
; `Mapper_SwitchBank 2 to switch to bank 2.
.macro Mapper_SwitchBank
    lda #_1
    sta MMC1_PRGBANK
    lsr
    sta MMC1_PRGBANK
    lsr
    sta MMC1_PRGBANK
    lsr
    sta MMC1_PRGBANK
    lsr
    sta MMC1_PRGBANK
.macend

; switch bank at $8000-$bfff to value in memory address.
; `Mapper_SwitchBank 2 to switch bank to value in memory $2.
.macro Mapper_SwitchBankMem
    lda _1
    sta MMC1_PRGBANK
    lsr
    sta MMC1_PRGBANK
    lsr
    sta MMC1_PRGBANK
    lsr
    sta MMC1_PRGBANK
    lsr
    sta MMC1_PRGBANK
.macend