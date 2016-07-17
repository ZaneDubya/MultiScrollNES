#require "defines.asm"
byte MenuState;

void MenuMode() {
    switch (GameModeState) {
        case 0: // initialize
            inline Initialize();
            break;
        default:
            break;
    }
}

void Initialize() {
    GameModeState = 1;
    // load tiles
    // load map
    // set up music
    // turn ppu on
}
