/*  FieldMode is the main game mode, where the player spends most of their time.
    The player can walk around the world, interact with actors (scripted objects),
    enter towns, etc.
    States
    00  Walking around!
        DPad    Movement
        A       Do something
        Select  Examine something.
        Start   Display start menu
    01  Displaying the START/PAUSE menu at the bottom of the screen:
        Select character (0, 1, 2), Map, Spells, Rest, Menu
        Initially, Map is selected.
        Change selection with Left and Right.
        Affirm selection with A.
        Dismiss START menu with START or B.
    02  Displaying text at the bottom of the screen. 
        A/B - advance text page or goto next mode/return to world.
*/
