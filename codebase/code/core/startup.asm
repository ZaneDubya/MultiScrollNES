; ==============================================================================
; Reset         This is the 'reset' vector, called on reset interrupt.
Reset:
    cld                             ; processor to binary mode
    sei                             ; disable IRQ
    sta $e000                       ; acknowledge/disable the IRQ (MMC3)
    
    `Mapper_Setup                   ; set up the Mapper (A53, MMC1, MMC3, etc.)
    ldx #$00                        ; Clear PPU control registers
    stx PPU_CTRL                    ;   |
    stx PPU_MASK                    ;   |
    stx GameFlags                   ; clear initialized flag (and all others)
                                    ; fall through to Startup

; ==============================================================================
; Startup       Sets up the initial gamestate, which will be updated next NMI.
;               Then enters an endless loop. When we enter this code, NMI and
;               Screen, interrupts, and pending interrupts (mmc3) are disabled.
    stx APU_DMC_FREQ                    ; APU_DMC_FREQ = $00
    dex                                 ; x = $ff
    txs                                 ; S points to end of stack page
    `SetByte APU_FRAMECNT, $40          ; something to do with APU interupts
    
    ; Clear RAM
    `ClearWRAM
    `ClearCartRAM
    `ClearVRAM
    
    ; Detect NTSC/PAL
    `SetByte PPU_CTRL, $80              ; Turn on PPU rendering to detect NTSC
    jsr GetTVSystem                     ; 
    sta TVSystem                        ; 
    `SetByte PPU_CTRL, $00              ; Rendering off while we complete loading

    ; Initialize FamiTone (music engine)
    `FT_Init MusicDataSet0              ; initialize using the first song data,
                                        ; as it contains the DPCM sound effect
    ; Init Input
    jsr Input_Init                      ; init gamepad polling code
    
    ; Init Tileset
    jsr MapSvc_LoadTileset
    
    ; Load CHRRAM
    jsr Debug_LoadCHRRAM
    
    ; Set up sprite loader
    jsr SprLdr_Setup
    
    jsr Debug_LoadMap
    jsr Debug_InitSprites
    jsr Debug_ClearAllActors
    ;jsr Debug_SpriteDisplay
    jsr Debug_CreatePlayerActor
    
    ; Set GameMode, turn on NMI.
    `SetByte GameMode, GameModeWorld
    `SetByte FrameCount, 0
    `SetGameFlag FlagDoNMI
    `SetByte PPU_CTRL, $88
    
    `FT_PlaySong $00
    
    ; Fall through to infinite loop
    
; ==============================================================================
; EndlessLoop       The game is here when not in NMI or RunOneFrame.
*   jmp -                           ; infinite loop