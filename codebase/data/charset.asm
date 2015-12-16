; Charset, encoded by charcodr

; labels for the jumps
.word _Char20, _Char21, _Char22, _Char23, _Char24, _Char25, _Char26, _Char27, _Char28, _Char29, _Char2a, _Char2b, _Char2c, _Char2d, _Char2e, _Char2f
.word _Char30, _Char31, _Char32, _Char33, _Char34, _Char35, _Char36, _Char37, _Char38, _Char39, _Char3a, _Char3b, _Char3c, _Char3d, _Char3e, _Char3f
.word _Char40, _Char41, _Char42, _Char43, _Char44, _Char45, _Char46, _Char47, _Char48, _Char49, _Char4a, _Char4b, _Char4c, _Char4d, _Char4e, _Char4f
.word _Char50, _Char51, _Char52, _Char53, _Char54, _Char55, _Char56, _Char57, _Char58, _Char59, _Char5a, _Char5b, _Char5c, _Char5d, _Char5e, _Char5f
.word _Char60, _Char61, _Char62, _Char63, _Char64, _Char65, _Char66, _Char67, _Char68, _Char69, _Char6a, _Char6b, _Char6c, _Char6d, _Char6e, _Char6f
.word _Char70, _Char71, _Char72, _Char73, _Char74, _Char75, _Char76, _Char77, _Char78, _Char79, _Char7a, _Char7b, _Char7c, _Char7d, _Char7e, _Char7f

; 52 bytes, 68 cycles
_Char20:
	lda #$ff
	sta $2007
	sta $2007
	sta $2007
	sta $2007
	sta $2007
	sta $2007
	sta $2007
	sta $2007
	lda #$00
	sta $2007
	sta $2007
	sta $2007
	sta $2007
	sta $2007
	sta $2007
	sta $2007
	sta $2007
jmp _NextChar

; 64 bytes, 80 cycles
_Char21:
	lda #$ef
	sta $2007
	sta $2007
	sta $2007
	sta $2007
	sta $2007
	lda #$ff
	sta $2007
	lda #$ef
	sta $2007
	lda #$ff
	sta $2007
	lda #$18
	sta $2007
	sta $2007
	sta $2007
	sta $2007
	sta $2007
	lda #$00
	sta $2007
	lda #$18
	sta $2007
	lda #$00
	sta $2007
jmp _NextChar

; 56 bytes, 72 cycles
_Char22:
	lda #$d7
	sta $2007
	sta $2007
	sta $2007
	lda #$ff
	sta $2007
	sta $2007
	sta $2007
	sta $2007
	sta $2007
	lda #$3c
	sta $2007
	sta $2007
	sta $2007
	lda #$00
	sta $2007
	sta $2007
	sta $2007
	sta $2007
	sta $2007
jmp _NextChar

; 72 bytes, 88 cycles
_Char23:
	lda #$b7
	sta $2007
	lda #$03
	sta $2007
	lda #$b7
	sta $2007
	sta $2007
	lda #$03
	sta $2007
	lda #$b7
	sta $2007
	lda #$ff
	sta $2007
	sta $2007
	lda #$6c
	sta $2007
	lda #$fe
	sta $2007
	lda #$6c
	sta $2007
	sta $2007
	lda #$fe
	sta $2007
	lda #$6c
	sta $2007
	lda #$00
	sta $2007
	sta $2007
jmp _NextChar

; 80 bytes, 96 cycles
_Char24:
	lda #$ef
	sta $2007
	lda #$c3
	sta $2007
	lda #$df
	sta $2007
	lda #$e7
	sta $2007
	lda #$fb
	sta $2007
	lda #$c3
	sta $2007
	lda #$f7
	sta $2007
	lda #$ff
	sta $2007
	lda #$18
	sta $2007
	lda #$3e
	sta $2007
	lda #$60
	sta $2007
	lda #$3c
	sta $2007
	lda #$06
	sta $2007
	lda #$7c
	sta $2007
	lda #$18
	sta $2007
	lda #$00
	sta $2007
jmp _NextChar

; 80 bytes, 96 cycles
_Char25:
	lda #$bb
	sta $2007
	lda #$b7
	sta $2007
	lda #$ef
	sta $2007
	lda #$df
	sta $2007
	lda #$bf
	sta $2007
	lda #$7b
	sta $2007
	lda #$fb
	sta $2007
	lda #$ff
	sta $2007
	lda #$66
	sta $2007
	lda #$6c
	sta $2007
	lda #$18
	sta $2007
	lda #$30
	sta $2007
	lda #$60
	sta $2007
	lda #$c6
	sta $2007
	lda #$86
	sta $2007
	lda #$00
	sta $2007
jmp _NextChar

; 80 bytes, 96 cycles
_Char26:
	lda #$e3
	sta $2007
	lda #$cb
	sta $2007
	lda #$e7
	sta $2007
	lda #$d7
	sta $2007
	lda #$b1
	sta $2007
	lda #$b9
	sta $2007
	lda #$cd
	sta $2007
	lda #$ff
	sta $2007
	lda #$1c
	sta $2007
	lda #$36
	sta $2007
	lda #$1c
	sta $2007
	lda #$38
	sta $2007
	lda #$6f
	sta $2007
	lda #$66
	sta $2007
	lda #$3b
	sta $2007
	lda #$00
	sta $2007
jmp _NextChar

; 56 bytes, 72 cycles
_Char27:
	lda #$f7
	sta $2007
	sta $2007
	sta $2007
	lda #$ff
	sta $2007
	sta $2007
	sta $2007
	sta $2007
	sta $2007
	lda #$0c
	sta $2007
	sta $2007
	sta $2007
	lda #$00
	sta $2007
	sta $2007
	sta $2007
	sta $2007
	sta $2007
jmp _NextChar

; 72 bytes, 88 cycles
_Char28:
	lda #$f3
	sta $2007
	lda #$e7
	sta $2007
	lda #$ef
	sta $2007
	sta $2007
	sta $2007
	lda #$e7
	sta $2007
	lda #$f3
	sta $2007
	lda #$ff
	sta $2007
	lda #$0e
	sta $2007
	lda #$1c
	sta $2007
	lda #$18
	sta $2007
	sta $2007
	sta $2007
	lda #$1c
	sta $2007
	lda #$0e
	sta $2007
	lda #$00
	sta $2007
jmp _NextChar

; 70 bytes, 86 cycles
_Char29:
	lda #$8f
	sta $2007
	lda #$ef
	sta $2007
	sta $2007
	sta $2007
	sta $2007
	lda #$cf
	sta $2007
	lda #$9f
	sta $2007
	lda #$ff
	sta $2007
	lda #$70
	sta $2007
	lda #$38
	sta $2007
	lda #$18
	sta $2007
	sta $2007
	sta $2007
	lda #$38
	sta $2007
	lda #$70
	sta $2007
	lda #$00
	sta $2007
jmp _NextChar

; 76 bytes, 92 cycles
_Char2a:
	lda #$ff
	sta $2007
	lda #$bb
	sta $2007
	lda #$d7
	sta $2007
	lda #$01
	sta $2007
	lda #$d7
	sta $2007
	lda #$bb
	sta $2007
	lda #$ff
	sta $2007
	sta $2007
	lda #$00
	sta $2007
	lda #$66
	sta $2007
	lda #$3c
	sta $2007
	lda #$ff
	sta $2007
	lda #$3c
	sta $2007
	lda #$66
	sta $2007
	lda #$00
	sta $2007
	sta $2007
jmp _NextChar

; 68 bytes, 84 cycles
_Char2b:
	lda #$ff
	sta $2007
	lda #$ef
	sta $2007
	sta $2007
	lda #$83
	sta $2007
	lda #$ef
	sta $2007
	sta $2007
	lda #$ff
	sta $2007
	sta $2007
	lda #$00
	sta $2007
	lda #$18
	sta $2007
	sta $2007
	lda #$7e
	sta $2007
	lda #$18
	sta $2007
	sta $2007
	lda #$00
	sta $2007
	sta $2007
jmp _NextChar

; 60 bytes, 76 cycles
_Char2c:
	lda #$ff
	sta $2007
	sta $2007
	sta $2007
	sta $2007
	sta $2007
	lda #$df
	sta $2007
	sta $2007
	lda #$9f
	sta $2007
	lda #$00
	sta $2007
	sta $2007
	sta $2007
	sta $2007
	sta $2007
	lda #$30
	sta $2007
	sta $2007
	lda #$60
	sta $2007
jmp _NextChar

; 60 bytes, 76 cycles
_Char2d:
	lda #$ff
	sta $2007
	sta $2007
	sta $2007
	lda #$83
	sta $2007
	lda #$ff
	sta $2007
	sta $2007
	sta $2007
	sta $2007
	lda #$00
	sta $2007
	sta $2007
	sta $2007
	lda #$7e
	sta $2007
	lda #$00
	sta $2007
	sta $2007
	sta $2007
	sta $2007
jmp _NextChar

; 60 bytes, 76 cycles
_Char2e:
	lda #$ff
	sta $2007
	sta $2007
	sta $2007
	sta $2007
	sta $2007
	lda #$bf
	sta $2007
	sta $2007
	lda #$ff
	sta $2007
	lda #$00
	sta $2007
	sta $2007
	sta $2007
	sta $2007
	sta $2007
	lda #$60
	sta $2007
	sta $2007
	lda #$00
	sta $2007
jmp _NextChar

; 78 bytes, 94 cycles
_Char2f:
	lda #$fd
	sta $2007
	lda #$fb
	sta $2007
	lda #$f7
	sta $2007
	lda #$ef
	sta $2007
	lda #$df
	sta $2007
	lda #$bf
	sta $2007
	lda #$ff
	sta $2007
	sta $2007
	lda #$02
	sta $2007
	lda #$06
	sta $2007
	lda #$0c
	sta $2007
	lda #$18
	sta $2007
	lda #$30
	sta $2007
	lda #$60
	sta $2007
	lda #$40
	sta $2007
	lda #$00
	sta $2007
jmp _NextChar

; 70 bytes, 86 cycles
_Char30:
	lda #$c7
	sta $2007
	lda #$bb
	sta $2007
	sta $2007
	sta $2007
	sta $2007
	sta $2007
	lda #$c7
	sta $2007
	lda #$ff
	sta $2007
	lda #$3c
	sta $2007
	lda #$66
	sta $2007
	lda #$6e
	sta $2007
	lda #$76
	sta $2007
	lda #$66
	sta $2007
	sta $2007
	lda #$3c
	sta $2007
	lda #$00
	sta $2007
jmp _NextChar

; 68 bytes, 84 cycles
_Char31:
	lda #$ef
	sta $2007
	lda #$cf
	sta $2007
	lda #$ef
	sta $2007
	sta $2007
	sta $2007
	sta $2007
	lda #$83
	sta $2007
	lda #$ff
	sta $2007
	lda #$18
	sta $2007
	lda #$38
	sta $2007
	lda #$18
	sta $2007
	sta $2007
	sta $2007
	sta $2007
	lda #$7e
	sta $2007
	lda #$00
	sta $2007
jmp _NextChar

; 80 bytes, 96 cycles
_Char32:
	lda #$c3
	sta $2007
	lda #$9b
	sta $2007
	lda #$fb
	sta $2007
	lda #$f7
	sta $2007
	lda #$ef
	sta $2007
	lda #$df
	sta $2007
	lda #$83
	sta $2007
	lda #$ff
	sta $2007
	lda #$3c
	sta $2007
	lda #$66
	sta $2007
	lda #$06
	sta $2007
	lda #$0c
	sta $2007
	lda #$18
	sta $2007
	lda #$30
	sta $2007
	lda #$7e
	sta $2007
	lda #$00
	sta $2007
jmp _NextChar

; 80 bytes, 96 cycles
_Char33:
	lda #$83
	sta $2007
	lda #$f7
	sta $2007
	lda #$ef
	sta $2007
	lda #$f3
	sta $2007
	lda #$fb
	sta $2007
	lda #$9b
	sta $2007
	lda #$c7
	sta $2007
	lda #$ff
	sta $2007
	lda #$7e
	sta $2007
	lda #$0c
	sta $2007
	lda #$18
	sta $2007
	lda #$0c
	sta $2007
	lda #$06
	sta $2007
	lda #$66
	sta $2007
	lda #$3c
	sta $2007
	lda #$00
	sta $2007
jmp _NextChar

; 76 bytes, 92 cycles
_Char34:
	lda #$f7
	sta $2007
	lda #$e7
	sta $2007
	lda #$d7
	sta $2007
	lda #$b7
	sta $2007
	lda #$83
	sta $2007
	lda #$f7
	sta $2007
	sta $2007
	lda #$ff
	sta $2007
	lda #$0c
	sta $2007
	lda #$1c
	sta $2007
	lda #$3c
	sta $2007
	lda #$6c
	sta $2007
	lda #$7e
	sta $2007
	lda #$0c
	sta $2007
	sta $2007
	lda #$00
	sta $2007
jmp _NextChar

; 76 bytes, 92 cycles
_Char35:
	lda #$83
	sta $2007
	lda #$bf
	sta $2007
	lda #$83
	sta $2007
	lda #$fb
	sta $2007
	sta $2007
	lda #$9b
	sta $2007
	lda #$c7
	sta $2007
	lda #$ff
	sta $2007
	lda #$7e
	sta $2007
	lda #$60
	sta $2007
	lda #$7c
	sta $2007
	lda #$06
	sta $2007
	sta $2007
	lda #$66
	sta $2007
	lda #$3c
	sta $2007
	lda #$00
	sta $2007
jmp _NextChar

; 72 bytes, 88 cycles
_Char36:
	lda #$c7
	sta $2007
	lda #$bf
	sta $2007
	sta $2007
	lda #$83
	sta $2007
	lda #$bb
	sta $2007
	sta $2007
	lda #$c7
	sta $2007
	lda #$ff
	sta $2007
	lda #$3c
	sta $2007
	lda #$60
	sta $2007
	sta $2007
	lda #$7c
	sta $2007
	lda #$66
	sta $2007
	sta $2007
	lda #$3c
	sta $2007
	lda #$00
	sta $2007
jmp _NextChar

; 72 bytes, 88 cycles
_Char37:
	lda #$83
	sta $2007
	lda #$fb
	sta $2007
	lda #$f7
	sta $2007
	lda #$ef
	sta $2007
	lda #$df
	sta $2007
	sta $2007
	sta $2007
	lda #$ff
	sta $2007
	lda #$7e
	sta $2007
	lda #$06
	sta $2007
	lda #$0c
	sta $2007
	lda #$18
	sta $2007
	lda #$30
	sta $2007
	sta $2007
	sta $2007
	lda #$00
	sta $2007
jmp _NextChar

; 72 bytes, 88 cycles
_Char38:
	lda #$c7
	sta $2007
	lda #$bb
	sta $2007
	sta $2007
	lda #$c3
	sta $2007
	lda #$bb
	sta $2007
	sta $2007
	lda #$c7
	sta $2007
	lda #$ff
	sta $2007
	lda #$3c
	sta $2007
	lda #$66
	sta $2007
	sta $2007
	lda #$3c
	sta $2007
	lda #$66
	sta $2007
	sta $2007
	lda #$3c
	sta $2007
	lda #$00
	sta $2007
jmp _NextChar

; 76 bytes, 92 cycles
_Char39:
	lda #$c7
	sta $2007
	lda #$bb
	sta $2007
	sta $2007
	lda #$c3
	sta $2007
	lda #$fb
	sta $2007
	lda #$f7
	sta $2007
	lda #$cf
	sta $2007
	lda #$ff
	sta $2007
	lda #$3c
	sta $2007
	lda #$66
	sta $2007
	sta $2007
	lda #$3e
	sta $2007
	lda #$06
	sta $2007
	lda #$0c
	sta $2007
	lda #$38
	sta $2007
	lda #$00
	sta $2007
jmp _NextChar

; 68 bytes, 84 cycles
_Char3a:
	lda #$ff
	sta $2007
	lda #$ef
	sta $2007
	sta $2007
	lda #$ff
	sta $2007
	lda #$ef
	sta $2007
	sta $2007
	lda #$ff
	sta $2007
	sta $2007
	lda #$00
	sta $2007
	lda #$18
	sta $2007
	sta $2007
	lda #$00
	sta $2007
	lda #$18
	sta $2007
	sta $2007
	lda #$00
	sta $2007
	sta $2007
jmp _NextChar

; 72 bytes, 88 cycles
_Char3b:
	lda #$ff
	sta $2007
	lda #$ef
	sta $2007
	sta $2007
	lda #$ff
	sta $2007
	lda #$ef
	sta $2007
	sta $2007
	lda #$df
	sta $2007
	lda #$ff
	sta $2007
	lda #$00
	sta $2007
	lda #$18
	sta $2007
	sta $2007
	lda #$00
	sta $2007
	lda #$18
	sta $2007
	sta $2007
	lda #$30
	sta $2007
	lda #$00
	sta $2007
jmp _NextChar

; 80 bytes, 96 cycles
_Char3c:
	lda #$f7
	sta $2007
	lda #$ef
	sta $2007
	lda #$df
	sta $2007
	lda #$bf
	sta $2007
	lda #$df
	sta $2007
	lda #$ef
	sta $2007
	lda #$f7
	sta $2007
	lda #$ff
	sta $2007
	lda #$0c
	sta $2007
	lda #$18
	sta $2007
	lda #$30
	sta $2007
	lda #$60
	sta $2007
	lda #$30
	sta $2007
	lda #$18
	sta $2007
	lda #$0c
	sta $2007
	lda #$00
	sta $2007
jmp _NextChar

; 68 bytes, 84 cycles
_Char3d:
	lda #$ff
	sta $2007
	sta $2007
	lda #$83
	sta $2007
	lda #$ff
	sta $2007
	sta $2007
	lda #$83
	sta $2007
	lda #$ff
	sta $2007
	sta $2007
	lda #$00
	sta $2007
	sta $2007
	lda #$7e
	sta $2007
	lda #$00
	sta $2007
	sta $2007
	lda #$7e
	sta $2007
	lda #$00
	sta $2007
	sta $2007
jmp _NextChar

; 78 bytes, 94 cycles
_Char3e:
	lda #$cf
	sta $2007
	lda #$f7
	sta $2007
	lda #$fb
	sta $2007
	sta $2007
	lda #$f7
	sta $2007
	lda #$ef
	sta $2007
	lda #$df
	sta $2007
	lda #$ff
	sta $2007
	lda #$30
	sta $2007
	lda #$18
	sta $2007
	lda #$0c
	sta $2007
	lda #$06
	sta $2007
	lda #$0c
	sta $2007
	lda #$18
	sta $2007
	lda #$30
	sta $2007
	lda #$00
	sta $2007
jmp _NextChar

; 80 bytes, 96 cycles
_Char3f:
	lda #$c3
	sta $2007
	lda #$bb
	sta $2007
	lda #$fb
	sta $2007
	lda #$f7
	sta $2007
	lda #$ef
	sta $2007
	lda #$ff
	sta $2007
	lda #$ef
	sta $2007
	lda #$ff
	sta $2007
	lda #$3c
	sta $2007
	lda #$66
	sta $2007
	lda #$06
	sta $2007
	lda #$0c
	sta $2007
	lda #$18
	sta $2007
	lda #$00
	sta $2007
	lda #$18
	sta $2007
	lda #$00
	sta $2007
jmp _NextChar

; 80 bytes, 96 cycles
_Char40:
	lda #$c3
	sta $2007
	lda #$bb
	sta $2007
	lda #$b3
	sta $2007
	lda #$b7
	sta $2007
	lda #$b3
	sta $2007
	lda #$bf
	sta $2007
	lda #$c3
	sta $2007
	lda #$ff
	sta $2007
	lda #$3c
	sta $2007
	lda #$66
	sta $2007
	lda #$6e
	sta $2007
	lda #$6a
	sta $2007
	lda #$6e
	sta $2007
	lda #$60
	sta $2007
	lda #$3e
	sta $2007
	lda #$00
	sta $2007
jmp _NextChar

; 72 bytes, 88 cycles
_Char41:
	lda #$e7
	sta $2007
	lda #$db
	sta $2007
	lda #$bb
	sta $2007
	sta $2007
	lda #$a3
	sta $2007
	lda #$bb
	sta $2007
	sta $2007
	lda #$ff
	sta $2007
	lda #$18
	sta $2007
	lda #$3c
	sta $2007
	lda #$66
	sta $2007
	sta $2007
	lda #$7e
	sta $2007
	lda #$66
	sta $2007
	sta $2007
	lda #$00
	sta $2007
jmp _NextChar

; 72 bytes, 88 cycles
_Char42:
	lda #$83
	sta $2007
	lda #$bb
	sta $2007
	sta $2007
	lda #$87
	sta $2007
	lda #$bb
	sta $2007
	sta $2007
	lda #$87
	sta $2007
	lda #$ff
	sta $2007
	lda #$7c
	sta $2007
	lda #$66
	sta $2007
	sta $2007
	lda #$7c
	sta $2007
	lda #$66
	sta $2007
	sta $2007
	lda #$7c
	sta $2007
	lda #$00
	sta $2007
jmp _NextChar

; 72 bytes, 88 cycles
_Char43:
	lda #$c3
	sta $2007
	lda #$bb
	sta $2007
	lda #$bf
	sta $2007
	sta $2007
	sta $2007
	lda #$bb
	sta $2007
	lda #$c7
	sta $2007
	lda #$ff
	sta $2007
	lda #$3c
	sta $2007
	lda #$66
	sta $2007
	lda #$60
	sta $2007
	sta $2007
	sta $2007
	lda #$66
	sta $2007
	lda #$3c
	sta $2007
	lda #$00
	sta $2007
jmp _NextChar

; 72 bytes, 88 cycles
_Char44:
	lda #$87
	sta $2007
	lda #$b3
	sta $2007
	lda #$bb
	sta $2007
	sta $2007
	sta $2007
	lda #$b7
	sta $2007
	lda #$8f
	sta $2007
	lda #$ff
	sta $2007
	lda #$78
	sta $2007
	lda #$6c
	sta $2007
	lda #$66
	sta $2007
	sta $2007
	sta $2007
	lda #$6c
	sta $2007
	lda #$78
	sta $2007
	lda #$00
	sta $2007
jmp _NextChar

; 72 bytes, 88 cycles
_Char45:
	lda #$83
	sta $2007
	lda #$bf
	sta $2007
	sta $2007
	lda #$87
	sta $2007
	lda #$bf
	sta $2007
	sta $2007
	lda #$83
	sta $2007
	lda #$ff
	sta $2007
	lda #$7e
	sta $2007
	lda #$60
	sta $2007
	sta $2007
	lda #$7c
	sta $2007
	lda #$60
	sta $2007
	sta $2007
	lda #$7e
	sta $2007
	lda #$00
	sta $2007
jmp _NextChar

; 68 bytes, 84 cycles
_Char46:
	lda #$83
	sta $2007
	lda #$bf
	sta $2007
	sta $2007
	lda #$87
	sta $2007
	lda #$bf
	sta $2007
	sta $2007
	sta $2007
	lda #$ff
	sta $2007
	lda #$7e
	sta $2007
	lda #$60
	sta $2007
	sta $2007
	lda #$7c
	sta $2007
	lda #$60
	sta $2007
	sta $2007
	sta $2007
	lda #$00
	sta $2007
jmp _NextChar

; 72 bytes, 88 cycles
_Char47:
	lda #$c1
	sta $2007
	lda #$bf
	sta $2007
	sta $2007
	lda #$b3
	sta $2007
	lda #$bb
	sta $2007
	sta $2007
	lda #$c3
	sta $2007
	lda #$ff
	sta $2007
	lda #$3e
	sta $2007
	lda #$60
	sta $2007
	sta $2007
	lda #$6e
	sta $2007
	lda #$66
	sta $2007
	sta $2007
	lda #$3e
	sta $2007
	lda #$00
	sta $2007
jmp _NextChar

; 64 bytes, 80 cycles
_Char48:
	lda #$bb
	sta $2007
	sta $2007
	sta $2007
	lda #$83
	sta $2007
	lda #$bb
	sta $2007
	sta $2007
	sta $2007
	lda #$ff
	sta $2007
	lda #$66
	sta $2007
	sta $2007
	sta $2007
	lda #$7e
	sta $2007
	lda #$66
	sta $2007
	sta $2007
	sta $2007
	lda #$00
	sta $2007
jmp _NextChar

; 64 bytes, 80 cycles
_Char49:
	lda #$c7
	sta $2007
	lda #$ef
	sta $2007
	sta $2007
	sta $2007
	sta $2007
	sta $2007
	lda #$c7
	sta $2007
	lda #$ff
	sta $2007
	lda #$3c
	sta $2007
	lda #$18
	sta $2007
	sta $2007
	sta $2007
	sta $2007
	sta $2007
	lda #$3c
	sta $2007
	lda #$00
	sta $2007
jmp _NextChar

; 64 bytes, 80 cycles
_Char4a:
	lda #$fb
	sta $2007
	sta $2007
	sta $2007
	sta $2007
	sta $2007
	lda #$bb
	sta $2007
	lda #$c3
	sta $2007
	lda #$ff
	sta $2007
	lda #$06
	sta $2007
	sta $2007
	sta $2007
	sta $2007
	sta $2007
	lda #$66
	sta $2007
	lda #$3c
	sta $2007
	lda #$00
	sta $2007
jmp _NextChar

; 78 bytes, 94 cycles
_Char4b:
	lda #$b9
	sta $2007
	lda #$b7
	sta $2007
	lda #$af
	sta $2007
	sta $2007
	lda #$b7
	sta $2007
	lda #$bb
	sta $2007
	lda #$bd
	sta $2007
	lda #$ff
	sta $2007
	lda #$66
	sta $2007
	lda #$6c
	sta $2007
	lda #$78
	sta $2007
	lda #$70
	sta $2007
	lda #$78
	sta $2007
	lda #$6c
	sta $2007
	lda #$66
	sta $2007
	lda #$00
	sta $2007
jmp _NextChar

; 60 bytes, 76 cycles
_Char4c:
	lda #$bf
	sta $2007
	sta $2007
	sta $2007
	sta $2007
	sta $2007
	sta $2007
	lda #$83
	sta $2007
	lda #$ff
	sta $2007
	lda #$60
	sta $2007
	sta $2007
	sta $2007
	sta $2007
	sta $2007
	sta $2007
	lda #$7e
	sta $2007
	lda #$00
	sta $2007
jmp _NextChar

; 70 bytes, 86 cycles
_Char4d:
	lda #$3b
	sta $2007
	lda #$53
	sta $2007
	lda #$6b
	sta $2007
	lda #$7b
	sta $2007
	sta $2007
	sta $2007
	sta $2007
	lda #$ff
	sta $2007
	lda #$c6
	sta $2007
	lda #$ee
	sta $2007
	lda #$fe
	sta $2007
	lda #$d6
	sta $2007
	lda #$c6
	sta $2007
	sta $2007
	sta $2007
	lda #$00
	sta $2007
jmp _NextChar

; 72 bytes, 88 cycles
_Char4e:
	lda #$9b
	sta $2007
	lda #$ab
	sta $2007
	lda #$a3
	sta $2007
	lda #$b3
	sta $2007
	lda #$bb
	sta $2007
	sta $2007
	sta $2007
	lda #$ff
	sta $2007
	lda #$66
	sta $2007
	lda #$76
	sta $2007
	lda #$7e
	sta $2007
	sta $2007
	lda #$6e
	sta $2007
	lda #$66
	sta $2007
	sta $2007
	lda #$00
	sta $2007
jmp _NextChar

; 64 bytes, 80 cycles
_Char4f:
	lda #$c3
	sta $2007
	lda #$bb
	sta $2007
	sta $2007
	sta $2007
	sta $2007
	sta $2007
	lda #$c3
	sta $2007
	lda #$ff
	sta $2007
	lda #$3c
	sta $2007
	lda #$66
	sta $2007
	sta $2007
	sta $2007
	sta $2007
	sta $2007
	lda #$3c
	sta $2007
	lda #$00
	sta $2007
jmp _NextChar

; 68 bytes, 84 cycles
_Char50:
	lda #$83
	sta $2007
	lda #$bb
	sta $2007
	sta $2007
	lda #$83
	sta $2007
	lda #$bf
	sta $2007
	sta $2007
	sta $2007
	lda #$ff
	sta $2007
	lda #$7c
	sta $2007
	lda #$66
	sta $2007
	sta $2007
	lda #$7c
	sta $2007
	lda #$60
	sta $2007
	sta $2007
	sta $2007
	lda #$00
	sta $2007
jmp _NextChar

; 72 bytes, 88 cycles
_Char51:
	lda #$c3
	sta $2007
	lda #$bb
	sta $2007
	sta $2007
	sta $2007
	lda #$ab
	sta $2007
	lda #$b3
	sta $2007
	lda #$cb
	sta $2007
	lda #$ff
	sta $2007
	lda #$3c
	sta $2007
	lda #$66
	sta $2007
	sta $2007
	sta $2007
	lda #$76
	sta $2007
	lda #$6c
	sta $2007
	lda #$36
	sta $2007
	lda #$00
	sta $2007
jmp _NextChar

; 74 bytes, 90 cycles
_Char52:
	lda #$83
	sta $2007
	lda #$bb
	sta $2007
	sta $2007
	lda #$83
	sta $2007
	lda #$bb
	sta $2007
	lda #$b9
	sta $2007
	lda #$bd
	sta $2007
	lda #$ff
	sta $2007
	lda #$7c
	sta $2007
	lda #$66
	sta $2007
	sta $2007
	lda #$7c
	sta $2007
	lda #$6c
	sta $2007
	lda #$66
	sta $2007
	sta $2007
	lda #$00
	sta $2007
jmp _NextChar

; 80 bytes, 96 cycles
_Char53:
	lda #$c7
	sta $2007
	lda #$bb
	sta $2007
	lda #$9f
	sta $2007
	lda #$c7
	sta $2007
	lda #$fb
	sta $2007
	lda #$bb
	sta $2007
	lda #$c7
	sta $2007
	lda #$ff
	sta $2007
	lda #$3c
	sta $2007
	lda #$66
	sta $2007
	lda #$60
	sta $2007
	lda #$3c
	sta $2007
	lda #$06
	sta $2007
	lda #$66
	sta $2007
	lda #$3c
	sta $2007
	lda #$00
	sta $2007
jmp _NextChar

; 60 bytes, 76 cycles
_Char54:
	lda #$83
	sta $2007
	lda #$ef
	sta $2007
	sta $2007
	sta $2007
	sta $2007
	sta $2007
	sta $2007
	lda #$ff
	sta $2007
	lda #$7e
	sta $2007
	lda #$18
	sta $2007
	sta $2007
	sta $2007
	sta $2007
	sta $2007
	sta $2007
	lda #$00
	sta $2007
jmp _NextChar

; 60 bytes, 76 cycles
_Char55:
	lda #$bb
	sta $2007
	sta $2007
	sta $2007
	sta $2007
	sta $2007
	sta $2007
	lda #$c3
	sta $2007
	lda #$ff
	sta $2007
	lda #$66
	sta $2007
	sta $2007
	sta $2007
	sta $2007
	sta $2007
	sta $2007
	lda #$3e
	sta $2007
	lda #$00
	sta $2007
jmp _NextChar

; 68 bytes, 84 cycles
_Char56:
	lda #$bb
	sta $2007
	sta $2007
	sta $2007
	lda #$9b
	sta $2007
	lda #$db
	sta $2007
	lda #$c7
	sta $2007
	lda #$ef
	sta $2007
	lda #$ff
	sta $2007
	lda #$66
	sta $2007
	sta $2007
	sta $2007
	sta $2007
	lda #$3c
	sta $2007
	sta $2007
	lda #$18
	sta $2007
	lda #$00
	sta $2007
jmp _NextChar

; 72 bytes, 88 cycles
_Char57:
	lda #$7b
	sta $2007
	sta $2007
	sta $2007
	lda #$6b
	sta $2007
	lda #$53
	sta $2007
	lda #$7b
	sta $2007
	lda #$39
	sta $2007
	lda #$ff
	sta $2007
	lda #$c6
	sta $2007
	sta $2007
	sta $2007
	lda #$d6
	sta $2007
	lda #$fe
	sta $2007
	lda #$ee
	sta $2007
	lda #$c6
	sta $2007
	lda #$00
	sta $2007
jmp _NextChar

; 72 bytes, 88 cycles
_Char58:
	lda #$bb
	sta $2007
	sta $2007
	lda #$c3
	sta $2007
	lda #$e7
	sta $2007
	lda #$cb
	sta $2007
	lda #$bb
	sta $2007
	sta $2007
	lda #$ff
	sta $2007
	lda #$66
	sta $2007
	sta $2007
	lda #$3c
	sta $2007
	lda #$18
	sta $2007
	lda #$3c
	sta $2007
	lda #$66
	sta $2007
	sta $2007
	lda #$00
	sta $2007
jmp _NextChar

; 66 bytes, 82 cycles
_Char59:
	lda #$bb
	sta $2007
	sta $2007
	lda #$9b
	sta $2007
	lda #$c7
	sta $2007
	lda #$ef
	sta $2007
	sta $2007
	sta $2007
	lda #$ff
	sta $2007
	lda #$66
	sta $2007
	sta $2007
	sta $2007
	lda #$3c
	sta $2007
	lda #$18
	sta $2007
	sta $2007
	sta $2007
	lda #$00
	sta $2007
jmp _NextChar

; 80 bytes, 96 cycles
_Char5a:
	lda #$83
	sta $2007
	lda #$fb
	sta $2007
	lda #$f7
	sta $2007
	lda #$ef
	sta $2007
	lda #$df
	sta $2007
	lda #$bf
	sta $2007
	lda #$83
	sta $2007
	lda #$ff
	sta $2007
	lda #$7e
	sta $2007
	lda #$06
	sta $2007
	lda #$0c
	sta $2007
	lda #$18
	sta $2007
	lda #$30
	sta $2007
	lda #$60
	sta $2007
	lda #$7e
	sta $2007
	lda #$00
	sta $2007
jmp _NextChar

; 64 bytes, 80 cycles
_Char5b:
	lda #$ff
	sta $2007
	lda #$80
	sta $2007
	sta $2007
	lda #$87
	sta $2007
	lda #$8f
	sta $2007
	lda #$9f
	sta $2007
	sta $2007
	sta $2007
	lda #$ff
	sta $2007
	sta $2007
	sta $2007
	sta $2007
	sta $2007
	lda #$fc
	sta $2007
	lda #$f8
	sta $2007
	sta $2007
jmp _NextChar

; 56 bytes, 72 cycles
_Char5c:
	lda #$ff
	sta $2007
	lda #$00
	sta $2007
	sta $2007
	lda #$ff
	sta $2007
	sta $2007
	sta $2007
	sta $2007
	sta $2007
	sta $2007
	sta $2007
	sta $2007
	sta $2007
	sta $2007
	lda #$00
	sta $2007
	sta $2007
	sta $2007
jmp _NextChar

; 64 bytes, 80 cycles
_Char5d:
	lda #$ff
	sta $2007
	lda #$01
	sta $2007
	sta $2007
	lda #$e1
	sta $2007
	lda #$f1
	sta $2007
	lda #$f9
	sta $2007
	sta $2007
	sta $2007
	lda #$ff
	sta $2007
	sta $2007
	sta $2007
	sta $2007
	sta $2007
	lda #$3f
	sta $2007
	lda #$1f
	sta $2007
	sta $2007
jmp _NextChar

; 52 bytes, 68 cycles
_Char5e:
	lda #$9f
	sta $2007
	sta $2007
	sta $2007
	sta $2007
	sta $2007
	sta $2007
	sta $2007
	sta $2007
	lda #$f8
	sta $2007
	sta $2007
	sta $2007
	sta $2007
	sta $2007
	sta $2007
	sta $2007
	sta $2007
jmp _NextChar

; 52 bytes, 68 cycles
_Char5f:
	lda #$f9
	sta $2007
	sta $2007
	sta $2007
	sta $2007
	sta $2007
	sta $2007
	sta $2007
	sta $2007
	lda #$1f
	sta $2007
	sta $2007
	sta $2007
	sta $2007
	sta $2007
	sta $2007
	sta $2007
	sta $2007
jmp _NextChar

; 64 bytes, 80 cycles
_Char60:
	lda #$3f
	sta $2007
	lda #$df
	sta $2007
	lda #$ef
	sta $2007
	lda #$ff
	sta $2007
	sta $2007
	sta $2007
	sta $2007
	sta $2007
	lda #$c0
	sta $2007
	lda #$60
	sta $2007
	lda #$30
	sta $2007
	lda #$00
	sta $2007
	sta $2007
	sta $2007
	sta $2007
	sta $2007
jmp _NextChar

; 76 bytes, 92 cycles
_Char61:
	lda #$ff
	sta $2007
	sta $2007
	lda #$c3
	sta $2007
	lda #$fb
	sta $2007
	lda #$c3
	sta $2007
	lda #$bb
	sta $2007
	lda #$c3
	sta $2007
	lda #$ff
	sta $2007
	lda #$00
	sta $2007
	sta $2007
	lda #$3c
	sta $2007
	lda #$06
	sta $2007
	lda #$3e
	sta $2007
	lda #$66
	sta $2007
	lda #$3e
	sta $2007
	lda #$00
	sta $2007
jmp _NextChar

; 68 bytes, 84 cycles
_Char62:
	lda #$bf
	sta $2007
	sta $2007
	lda #$83
	sta $2007
	lda #$bb
	sta $2007
	sta $2007
	sta $2007
	lda #$83
	sta $2007
	lda #$ff
	sta $2007
	lda #$60
	sta $2007
	sta $2007
	lda #$7c
	sta $2007
	lda #$66
	sta $2007
	sta $2007
	sta $2007
	lda #$7c
	sta $2007
	lda #$00
	sta $2007
jmp _NextChar

; 68 bytes, 84 cycles
_Char63:
	lda #$ff
	sta $2007
	sta $2007
	lda #$c7
	sta $2007
	lda #$bf
	sta $2007
	sta $2007
	sta $2007
	lda #$c7
	sta $2007
	lda #$ff
	sta $2007
	lda #$00
	sta $2007
	sta $2007
	lda #$3c
	sta $2007
	lda #$60
	sta $2007
	sta $2007
	sta $2007
	lda #$3c
	sta $2007
	lda #$00
	sta $2007
jmp _NextChar

; 68 bytes, 84 cycles
_Char64:
	lda #$fb
	sta $2007
	sta $2007
	lda #$c3
	sta $2007
	lda #$bb
	sta $2007
	sta $2007
	sta $2007
	lda #$c3
	sta $2007
	lda #$ff
	sta $2007
	lda #$06
	sta $2007
	sta $2007
	lda #$3e
	sta $2007
	lda #$66
	sta $2007
	sta $2007
	sta $2007
	lda #$3e
	sta $2007
	lda #$00
	sta $2007
jmp _NextChar

; 76 bytes, 92 cycles
_Char65:
	lda #$ff
	sta $2007
	sta $2007
	lda #$c3
	sta $2007
	lda #$bb
	sta $2007
	lda #$83
	sta $2007
	lda #$bf
	sta $2007
	lda #$c3
	sta $2007
	lda #$ff
	sta $2007
	lda #$00
	sta $2007
	sta $2007
	lda #$3c
	sta $2007
	lda #$66
	sta $2007
	lda #$7e
	sta $2007
	lda #$60
	sta $2007
	lda #$3c
	sta $2007
	lda #$00
	sta $2007
jmp _NextChar

; 68 bytes, 84 cycles
_Char66:
	lda #$e7
	sta $2007
	lda #$df
	sta $2007
	lda #$87
	sta $2007
	lda #$df
	sta $2007
	sta $2007
	sta $2007
	sta $2007
	lda #$ff
	sta $2007
	lda #$1c
	sta $2007
	lda #$30
	sta $2007
	lda #$7c
	sta $2007
	lda #$30
	sta $2007
	sta $2007
	sta $2007
	sta $2007
	lda #$00
	sta $2007
jmp _NextChar

; 72 bytes, 88 cycles
_Char67:
	lda #$ff
	sta $2007
	sta $2007
	lda #$c3
	sta $2007
	lda #$bb
	sta $2007
	sta $2007
	lda #$c3
	sta $2007
	lda #$fb
	sta $2007
	lda #$83
	sta $2007
	lda #$00
	sta $2007
	sta $2007
	lda #$3e
	sta $2007
	lda #$66
	sta $2007
	sta $2007
	lda #$3e
	sta $2007
	lda #$06
	sta $2007
	lda #$7c
	sta $2007
jmp _NextChar

; 64 bytes, 80 cycles
_Char68:
	lda #$bf
	sta $2007
	sta $2007
	lda #$83
	sta $2007
	lda #$bb
	sta $2007
	sta $2007
	sta $2007
	sta $2007
	lda #$ff
	sta $2007
	lda #$60
	sta $2007
	sta $2007
	lda #$7c
	sta $2007
	lda #$66
	sta $2007
	sta $2007
	sta $2007
	sta $2007
	lda #$00
	sta $2007
jmp _NextChar

; 72 bytes, 88 cycles
_Char69:
	lda #$ef
	sta $2007
	lda #$ff
	sta $2007
	lda #$cf
	sta $2007
	lda #$ef
	sta $2007
	sta $2007
	sta $2007
	lda #$c7
	sta $2007
	lda #$ff
	sta $2007
	lda #$18
	sta $2007
	lda #$00
	sta $2007
	lda #$38
	sta $2007
	lda #$18
	sta $2007
	sta $2007
	sta $2007
	lda #$3c
	sta $2007
	lda #$00
	sta $2007
jmp _NextChar

; 64 bytes, 80 cycles
_Char6a:
	lda #$ef
	sta $2007
	lda #$ff
	sta $2007
	lda #$ef
	sta $2007
	sta $2007
	sta $2007
	sta $2007
	sta $2007
	lda #$8f
	sta $2007
	lda #$18
	sta $2007
	lda #$00
	sta $2007
	lda #$18
	sta $2007
	sta $2007
	sta $2007
	sta $2007
	sta $2007
	lda #$70
	sta $2007
jmp _NextChar

; 76 bytes, 92 cycles
_Char6b:
	lda #$bf
	sta $2007
	sta $2007
	lda #$b9
	sta $2007
	lda #$b3
	sta $2007
	lda #$87
	sta $2007
	lda #$bb
	sta $2007
	lda #$bd
	sta $2007
	lda #$ff
	sta $2007
	lda #$60
	sta $2007
	sta $2007
	lda #$66
	sta $2007
	lda #$6c
	sta $2007
	lda #$78
	sta $2007
	lda #$6c
	sta $2007
	lda #$66
	sta $2007
	lda #$00
	sta $2007
jmp _NextChar

; 64 bytes, 80 cycles
_Char6c:
	lda #$cf
	sta $2007
	lda #$ef
	sta $2007
	sta $2007
	sta $2007
	sta $2007
	sta $2007
	lda #$c7
	sta $2007
	lda #$ff
	sta $2007
	lda #$38
	sta $2007
	lda #$18
	sta $2007
	sta $2007
	sta $2007
	sta $2007
	sta $2007
	lda #$3c
	sta $2007
	lda #$00
	sta $2007
jmp _NextChar

; 70 bytes, 86 cycles
_Char6d:
	lda #$ff
	sta $2007
	sta $2007
	lda #$13
	sta $2007
	lda #$63
	sta $2007
	lda #$7b
	sta $2007
	sta $2007
	sta $2007
	lda #$ff
	sta $2007
	lda #$00
	sta $2007
	sta $2007
	lda #$ec
	sta $2007
	lda #$fe
	sta $2007
	lda #$d6
	sta $2007
	lda #$c6
	sta $2007
	sta $2007
	lda #$00
	sta $2007
jmp _NextChar

; 64 bytes, 80 cycles
_Char6e:
	lda #$ff
	sta $2007
	sta $2007
	lda #$83
	sta $2007
	lda #$bb
	sta $2007
	sta $2007
	sta $2007
	sta $2007
	lda #$ff
	sta $2007
	lda #$00
	sta $2007
	sta $2007
	lda #$7c
	sta $2007
	lda #$66
	sta $2007
	sta $2007
	sta $2007
	sta $2007
	lda #$00
	sta $2007
jmp _NextChar

; 68 bytes, 84 cycles
_Char6f:
	lda #$ff
	sta $2007
	sta $2007
	lda #$c3
	sta $2007
	lda #$bb
	sta $2007
	sta $2007
	sta $2007
	lda #$c3
	sta $2007
	lda #$ff
	sta $2007
	lda #$00
	sta $2007
	sta $2007
	lda #$3c
	sta $2007
	lda #$66
	sta $2007
	sta $2007
	sta $2007
	lda #$3c
	sta $2007
	lda #$00
	sta $2007
jmp _NextChar

; 68 bytes, 84 cycles
_Char70:
	lda #$ff
	sta $2007
	sta $2007
	lda #$83
	sta $2007
	lda #$bb
	sta $2007
	sta $2007
	sta $2007
	lda #$83
	sta $2007
	lda #$bf
	sta $2007
	lda #$00
	sta $2007
	sta $2007
	lda #$7c
	sta $2007
	lda #$66
	sta $2007
	sta $2007
	sta $2007
	lda #$7c
	sta $2007
	lda #$60
	sta $2007
jmp _NextChar

; 68 bytes, 84 cycles
_Char71:
	lda #$ff
	sta $2007
	sta $2007
	lda #$c3
	sta $2007
	lda #$bb
	sta $2007
	sta $2007
	sta $2007
	lda #$c3
	sta $2007
	lda #$fb
	sta $2007
	lda #$00
	sta $2007
	sta $2007
	lda #$3e
	sta $2007
	lda #$66
	sta $2007
	sta $2007
	sta $2007
	lda #$3e
	sta $2007
	lda #$06
	sta $2007
jmp _NextChar

; 68 bytes, 84 cycles
_Char72:
	lda #$ff
	sta $2007
	sta $2007
	lda #$83
	sta $2007
	lda #$bb
	sta $2007
	lda #$bf
	sta $2007
	sta $2007
	sta $2007
	lda #$ff
	sta $2007
	lda #$00
	sta $2007
	sta $2007
	lda #$7c
	sta $2007
	lda #$66
	sta $2007
	lda #$60
	sta $2007
	sta $2007
	sta $2007
	lda #$00
	sta $2007
jmp _NextChar

; 76 bytes, 92 cycles
_Char73:
	lda #$ff
	sta $2007
	sta $2007
	lda #$c3
	sta $2007
	lda #$9f
	sta $2007
	lda #$e3
	sta $2007
	lda #$fb
	sta $2007
	lda #$87
	sta $2007
	lda #$ff
	sta $2007
	lda #$00
	sta $2007
	sta $2007
	lda #$3e
	sta $2007
	lda #$60
	sta $2007
	lda #$3c
	sta $2007
	lda #$06
	sta $2007
	lda #$7c
	sta $2007
	lda #$00
	sta $2007
jmp _NextChar

; 72 bytes, 88 cycles
_Char74:
	lda #$ff
	sta $2007
	lda #$ef
	sta $2007
	lda #$83
	sta $2007
	lda #$ef
	sta $2007
	sta $2007
	sta $2007
	lda #$f3
	sta $2007
	lda #$ff
	sta $2007
	lda #$00
	sta $2007
	lda #$18
	sta $2007
	lda #$7e
	sta $2007
	lda #$18
	sta $2007
	sta $2007
	sta $2007
	lda #$0e
	sta $2007
	lda #$00
	sta $2007
jmp _NextChar

; 64 bytes, 80 cycles
_Char75:
	lda #$ff
	sta $2007
	sta $2007
	lda #$bb
	sta $2007
	sta $2007
	sta $2007
	sta $2007
	lda #$c3
	sta $2007
	lda #$ff
	sta $2007
	lda #$00
	sta $2007
	sta $2007
	lda #$66
	sta $2007
	sta $2007
	sta $2007
	sta $2007
	lda #$3e
	sta $2007
	lda #$00
	sta $2007
jmp _NextChar

; 70 bytes, 86 cycles
_Char76:
	lda #$ff
	sta $2007
	sta $2007
	lda #$bb
	sta $2007
	sta $2007
	lda #$9b
	sta $2007
	lda #$c7
	sta $2007
	lda #$ef
	sta $2007
	lda #$ff
	sta $2007
	lda #$00
	sta $2007
	sta $2007
	lda #$66
	sta $2007
	sta $2007
	sta $2007
	lda #$3c
	sta $2007
	lda #$18
	sta $2007
	lda #$00
	sta $2007
jmp _NextChar

; 72 bytes, 88 cycles
_Char77:
	lda #$ff
	sta $2007
	sta $2007
	lda #$7b
	sta $2007
	sta $2007
	lda #$29
	sta $2007
	lda #$83
	sta $2007
	lda #$b7
	sta $2007
	lda #$ff
	sta $2007
	lda #$00
	sta $2007
	sta $2007
	lda #$c6
	sta $2007
	sta $2007
	lda #$d6
	sta $2007
	lda #$7c
	sta $2007
	lda #$6c
	sta $2007
	lda #$00
	sta $2007
jmp _NextChar

; 76 bytes, 92 cycles
_Char78:
	lda #$ff
	sta $2007
	sta $2007
	lda #$9b
	sta $2007
	lda #$e3
	sta $2007
	lda #$e7
	sta $2007
	lda #$cb
	sta $2007
	lda #$9b
	sta $2007
	lda #$ff
	sta $2007
	lda #$00
	sta $2007
	sta $2007
	lda #$66
	sta $2007
	lda #$3c
	sta $2007
	lda #$18
	sta $2007
	lda #$3c
	sta $2007
	lda #$66
	sta $2007
	lda #$00
	sta $2007
jmp _NextChar

; 68 bytes, 84 cycles
_Char79:
	lda #$ff
	sta $2007
	sta $2007
	lda #$bb
	sta $2007
	sta $2007
	sta $2007
	lda #$c3
	sta $2007
	lda #$fb
	sta $2007
	lda #$83
	sta $2007
	lda #$00
	sta $2007
	sta $2007
	lda #$66
	sta $2007
	sta $2007
	sta $2007
	lda #$3e
	sta $2007
	lda #$06
	sta $2007
	lda #$7c
	sta $2007
jmp _NextChar

; 76 bytes, 92 cycles
_Char7a:
	lda #$ff
	sta $2007
	sta $2007
	lda #$83
	sta $2007
	lda #$f7
	sta $2007
	lda #$ef
	sta $2007
	lda #$df
	sta $2007
	lda #$83
	sta $2007
	lda #$ff
	sta $2007
	lda #$00
	sta $2007
	sta $2007
	lda #$7e
	sta $2007
	lda #$0c
	sta $2007
	lda #$18
	sta $2007
	lda #$30
	sta $2007
	lda #$7e
	sta $2007
	lda #$00
	sta $2007
jmp _NextChar

; 64 bytes, 80 cycles
_Char7b:
	lda #$9f
	sta $2007
	sta $2007
	lda #$8f
	sta $2007
	lda #$87
	sta $2007
	lda #$80
	sta $2007
	sta $2007
	sta $2007
	lda #$ff
	sta $2007
	lda #$f8
	sta $2007
	sta $2007
	lda #$fc
	sta $2007
	lda #$ff
	sta $2007
	sta $2007
	sta $2007
	sta $2007
	sta $2007
jmp _NextChar

; 58 bytes, 74 cycles
_Char7c:
	lda #$ff
	sta $2007
	sta $2007
	sta $2007
	sta $2007
	lda #$00
	sta $2007
	sta $2007
	sta $2007
	lda #$ff
	sta $2007
	lda #$00
	sta $2007
	sta $2007
	sta $2007
	lda #$ff
	sta $2007
	sta $2007
	sta $2007
	sta $2007
	sta $2007
jmp _NextChar

; 64 bytes, 80 cycles
_Char7d:
	lda #$f9
	sta $2007
	sta $2007
	lda #$f1
	sta $2007
	lda #$e1
	sta $2007
	lda #$01
	sta $2007
	sta $2007
	sta $2007
	lda #$ff
	sta $2007
	lda #$1f
	sta $2007
	sta $2007
	lda #$3f
	sta $2007
	lda #$ff
	sta $2007
	sta $2007
	sta $2007
	sta $2007
	sta $2007
jmp _NextChar

; 72 bytes, 88 cycles
_Char7e:
	lda #$ff
	sta $2007
	sta $2007
	lda #$9f
	sta $2007
	lda #$2f
	sta $2007
	lda #$73
	sta $2007
	lda #$f7
	sta $2007
	lda #$ff
	sta $2007
	sta $2007
	lda #$00
	sta $2007
	sta $2007
	lda #$60
	sta $2007
	lda #$f2
	sta $2007
	lda #$9e
	sta $2007
	lda #$0c
	sta $2007
	lda #$00
	sta $2007
	sta $2007
jmp _NextChar

; 60 bytes, 76 cycles
_Char7f:
	lda #$ff
	sta $2007
	sta $2007
	sta $2007
	sta $2007
	sta $2007
	lda #$6d
	sta $2007
	sta $2007
	lda #$ff
	sta $2007
	lda #$00
	sta $2007
	sta $2007
	sta $2007
	sta $2007
	sta $2007
	lda #$db
	sta $2007
	sta $2007
	lda #$00
	sta $2007
jmp _NextChar

