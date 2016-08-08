/*  CutsceneMode displays full-screen text and conversations in the game. It
    also has a fairly simple bytecode scripting system that can set flags, read
    flags, branch, and display alternate text and conversation options.
    On end, it returns to FieldMode.

    CutsceneMode reads and changes the state of binary flags. There are 2048 of
    these, represented by eleven-bit indexes. An operation that addresses a
    flag will have the format sfff oooo ffff ffff, where the high byte (ffff ffff) is the
    lower 8 bits of the flag index, o is the index, fff is the upper 8 bits of
    the flag index, and s indicates whether the flag should be clear (0)
    or set (1).

    Operations:
        00  Fade out and return to FieldMode
        10  Pause for value * 10 frames (max 42 seconds).
        20  Show "[A] CONTINUE" icon. Wait for input.
        30  Play music with index value, FF = stop music
        40  Play sound effect with index value
        x1  Set/clear flag with index value (clear if bit 7 of op set)
            f5 ff
        x2  If flag with index value0 is (clear/set based on bit 7), jump.
            f5 ff jj jj
        03  2 byte jump always.
            03 jj jj
        04  Display full screen of text with index = t.
            04 tt tt
        14  Display background index = value0.
            14 vv
        24  Display portrait index = value0 on left.
            24 vv
        34  Display portrait index = value0 on right.
            34 vv
        44  Display portrait index = value0 centered.
            44 vv
        54  Display conversation: first byte is name or FF, followed by 2bytes text index.
            54 nn tt tt
        05  Display conversation tree with Goodbye option.
            1b  topic count
            2b  goodbye script location.
            Foreach topic:
                Condition:  .0 jj jj    Show Always, conversation script at jj jj.
                            f1 ff jj jj If value0 is (clear/set based on bit 7), show, script at jj jj.
*/
