/*  BattleMode - displays the battle grid with a display of statistics and menu options below.
    Battle Grid - top 256x176 (32x22) of the screen.
        Enemies and allies are placed on a grid: a 12 wide by 8 high (96 total) grid of 16x16 squares.
        Each actor is 1x1, 1x2, or 2x2 in size; humanoids are 1x2.
        There is additionally 32p on each side and 32p on the top of the screen that are not part of the grid.
        Colors are as follows:
            bg: 2 palettes for heroes, 2 for enemies, but bottom tile must share a common palette.
            sp: 2 palettes for h/e common palette, 2 pal for selection/special effects.
    Menu - bottom 256x64 (32x8) of the screen.
        Top, bottom, left, right 8 pixels are blank. Furthermore, left and right sides are padded 8px.
        When an enemy turn is in progress, the menu is blank.
        When a player turn is in progress, the menu is as follows:
            Portrait (32x32) (select this to view stats)
            Name and Stats: Health, Energy, Strength, Armor (96px wide)
            Actions are 16px wide each, 6, max 96x.
                ActionMove (and minor attack) (always)
                ActionAttack (if close to an enemy)
                ActionSpell (if magic user)
                ActionSkill (if has skills)
                ActionItem (always)
                AttemptEscape (if on edge of map)
    Modes:
        00  Initialize - Load data and chr gfx / nametable, place enemies, get initiative order.
            -> GetNextTurn
        01  GetNextTurn - choose next actor based on initiative order
            -> CheckActive
        02  CheckActive - determine if currently selected actor gets a turn.
            - If actor is active -> BeginPlayerTurn or BeginEnemyTurn
            - If actor is not active -> GetNextTurn
        03  EndDefeat - shows defeat message.
            -> Menu mode
        04  EndVictory - show victory message. Update field data.
            -> World mode
        10  BeginEnemyTurn - show enemy menu. Highlight enemy.
            -> DoEnemyTurn
        11  DoEnemyTurn - do AI. Pause for long enough so that player can follow AI's actions.
            Depending on AI, DoActor****.
        20  BeginPlayerTurn - calculate what actions are available. Show menu. Highlight player.
            - Possible actions are Move, Attack, Spell, Skill, Item, Escape.
            - Actions not available are greyed out.
            -> WaitInputMenu
        21  WaitInputMenu - player can select an option from menu.
            - Highlight options with dpad left and right.
            - B shows help message for highlighted action.
            - A selects option.
        22  BeginPlayerMove - determine where player can move to.
            - calculate where player can move based on player speed. Results: 12 byte bitfield.
            - calculate if enemies are in range of movement. Results: 12 byte bitfield.
            -> DoPlayerSelectMove
        23  DoPlayerSelectMove - player selects a target from the grid.
            - Possible moves are in light green. Possible attacks are in red. Impossible moves gray/yellow.
            - On select valid move -> DoActorMove
            - On select valid move -> DoActorMoveAndAttack
            - B -> BeginPlayerTurn
        24  BeginPlayerSelectAttack
        25  DoPlayerSelectAttack
        26  BeginPlayerSelectSpell
        27  DoPlayerSelectSpell
        28  BeginPlayerSelectSkill
        29  DoPlayerSelectSkill
        2A  BeginPlayerSelectItem
        2B  DoPlayerSelectItem
        2C  DoPlayerSelectSpellSkillItemTarget
        2D  BeginPlayerSelectFlee
        2E  DoPlayerShowFleeResult
        30  Do item effect on self
        31  Do item effect on target
        32  Do actor move
        33  Do actor attack
        34  Do actor cast spell
        35  Do actor use skill
        36  Do actor use item
        37  Do actor die
        38  Do actor get hit
        39  Do actor buff
        3A  Do projectile
*/
