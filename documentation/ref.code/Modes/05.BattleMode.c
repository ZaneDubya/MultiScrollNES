/*  BattleMode - displays the battle grid with a display of statistics and menu options below.
    Battle Grid - top 256x176 (32x22) of the screen.
        Enemies and allies are placed on a grid: a 12 wide by 8 high grid of 16x16 squares.
        Each actor is 1x1, 1x2, or 2x2 in size; humanoids are 1x2.
        There is additionally 32p on each side and 32p on the top of the screen that are not part of the grid.
        Colors are as follows:
            bg: 2 palettes for heroes, 2 for enemies, but bottom tile must share a common palette.
            sp: 2 palettes for h/e common palette, 2 pal for selection/special effects.
    Menu - bottom 256x64 (32x8) of the screen.
        Top, bottom, left, right 8 pixels are blank.
*/
