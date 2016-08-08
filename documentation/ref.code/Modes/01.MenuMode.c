/*  MenuMode handles the Begin, Continue, Config, Load, Save, and Credits screens.
    MenuMode starts at the Begin or Continue screen, depending on the value in 
    ReturnGameMode , which MUST be set before MenuMode starts. If ReturnGameMode
    is $00, then Begin will show. If ReturnGameMode is anything else, then
    Continue will show.

    Modes by function:
    Begin:      Loaded after Reset mode (ReturnGameMode is $00). Player can Start a game,
                Load a game, access the Configuration screen, and play the Credits.
                Start -> NewGame (sets up world and show first chapter cutscene).
                Load -> RetoreMenu
                Save -> SaveMenu
                Config -> ConfigMenu
                Credits -> PlayCredits
                
    Continue:   The pause menu. Loaded from World during play. Player can Continue
                the current game, Start a new a game, Load a game, Save the current game,
                access the Configuration screen, and play the Credits.
    RetoreMenu: Displays the three savegame slots that can be restored from. If no game
                data exists in a slot, then the slot displays as empty, and cannot be
                restored from. A fourth option "Back" returns to the previous screen.
                If ReturnGameMode != $00, then prior to loading, a "Are you sure? Y/N"
                dialog displays.
    SaveMenu:   Displays the three savegame slots that can be saved to. Similar to 
                RestoreMenu. If no game data exists in a slot, then the slot displays
                as empty. Overwriting a game displays the "Are you sure? Y/N" prompt.
    Config:     Displays the Configuration menu. Not included in demo.
    Credits:    Sets up the game to display the credits.
    
 */

const char* GameModeState = (char*)0x0040;
const char* ReturnGameMode = (char*)0x0300;

// in main
asm("ldx #$00"); // set fsb = 0
asm("jsr MenuMode"); // call MenuMode

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
    // load tiles
    // load map
    // set up music
    // turn ppu on
    GameModeState = 1;
}
