.alias  A53_REGISTER_SELECT $5000
.alias  A53_REGISTER_VALUE  $8000

.alias	A53_CHR_BANK		$00
.alias	A53_PRG_BANK		$01
.alias	A53_MODE			$80
.alias	A53_OUTER_BANK		$81

.alias	A53_PRG_32K			$00
.alias	A53_PRG_FIXED_80	$08
.alias	A53_PRG_FIXED_C0	$0C
.alias	A53_MIRR_1			$00
.alias	A53_MIRR_V			$02
.alias	A53_MIRR_H			$03
.alias	A53_GAME_32K		$00
.alias	A53_GAME_64K		$10

; sets up the initial mapper state.
.macro Mapper_Setup
    ; bank 03 in $C000 - $FFFF
	lda #A53_OUTER_BANK
	sta A53_REGISTER_SELECT
	lda #$03
	sta A53_REGISTER_VALUE
    ; A53 mode: 64kb rom, outer bank is fixed at c000, single screen mirroring.
	lda #A53_MODE
	sta A53_REGISTER_SELECT
	lda #[A53_GAME_64K|A53_PRG_FIXED_C0|A53_MIRR_1]
	sta A53_REGISTER_VALUE
.macend

; show single screen $2000
.macro Mapper_Show2000
	lda #A53_PRG_BANK
	sta A53_REGISTER_SELECT
	lda BankIn8000
	and #$0F
	sta BankIn8000
	sta A53_REGISTER_VALUE
.macend

; show single screen $2400
.macro Mapper_Show2400
	lda #A53_PRG_BANK
	sta A53_REGISTER_SELECT
	lda BankIn8000
	ora #$10
	sta BankIn8000
	sta A53_REGISTER_VALUE
.macend

; switch bank at $8000-$bfff to value in macro statement.
; `Mapper_SwitchBank 2 to switch to bank 2.
.macro Mapper_SwitchBank
	lda #A53_PRG_BANK
	sta A53_REGISTER_SELECT
	lda BankIn8000
	and #$10
	sta BankIn8000
	lda #_1
	and #$0f
	ora BankIn8000
	sta BankIn8000
	sta A53_REGISTER_VALUE
.macend

; switch bank at $8000-$bfff to value in memory address.
; `Mapper_SwitchBank 2 to switch bank to value in memory $2.
.macro Mapper_SwitchBankMem
	lda #A53_PRG_BANK
	sta A53_REGISTER_SELECT
	lda BankIn8000
	and #$10
	sta BankIn8000
	lda _1
	and #$0f
	ora BankIn8000
	sta BankIn8000
	sta A53_REGISTER_VALUE
.macend