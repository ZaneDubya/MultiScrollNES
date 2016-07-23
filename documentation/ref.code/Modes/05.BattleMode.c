/*  BattleMode - displays the battle grid with a display of statistics and menu options below.
    Battle Grid - top 256x176 (32x22) of the screen.
        Enemies and allies are placed on a grid: a 12 wide by 8 high grid of 16x16 squares.
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
        00  Loading...
        01  Get next turn
        02  End turn
        03  Begin enemy turn (show menu)
        04  Enemy turn - do AI
        05  Begin player turn (calculate options and show menu)
        06  Waiting for player input - menu
        07  Calculate player movement range
        08  Waiting for player input - select move target
        09  Show spell menu
        0A  Show skill menu
        0B  Waiting for player input - select skill/spell
        0C  Waiting for player input - select skill/spell target
        0D  Show item menu
        0E  Waiting for player input - select item
        0F  Waiting for player input - select item target
        10  Do item effect on self
        11  Do item effect on target
        12  Do actor move
        13  Do actor attack
        14  Do actor cast spell
        15  Do actor use skill
        16  Do actor use item
        17  Do actor die
        18  Do actor get hit
        19  Do actor buff
        1A  Do projectile
        1B  Do player flee
*/
