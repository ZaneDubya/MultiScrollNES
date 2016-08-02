/*  CharacterMode - player can view information about their characters from this mode.
    Screen:
        Split into top and bottom. Top is items and stats, btm is portraits.
            Between top and btm, vblank is turned off to update palettes.
            Items/Stats: top 256x176 (32x22) of screen.
                TLR 16px blank.
                Name, Lvlxx CLASS, HP/MP CUR/MAX, 4 stats, 4 substats.
                WEAPON, ARMOR, ITEM text done with sprites, with icons below.
                4x3 grid of 24x24 item icons.
                Buttons for SPELLS/SKILLS (dep. on class)
                Cursor is Arrow when scrolling, can be pick up, x (For no drop)
            Mode selection menu: bottom 256x64 (32x8)
                TBLR 8px are blank, LR padded 8px.
                32px icons - each character, shop/body/chest, keys/gold
            STRETCH Item grid is a stretch feature.
        Palettes
            Top:
                bg: 4 shared bw all item icons & text
                sp: 4 for selection and buff icons and text.
            Btm: new pal during ppu off.

    Modes:
        00  Initialize - Load graphics and data. Show menu below.
            Inventory not yet displaying.
            -> SetupCharacter, char index == 0
        01  SetupCharacter - Turns off inventory displaying if on.
            Sets up text and inventory icons for the currently 
            selected character. When done -> SelectCharacter
        02  SelectCharacter - Player is selecting which character to show.
            L/R - changes the current char index and -> SetupCharacter
            A -> SelectInventory
            B -> Exit to previous Mode (world screen?)
        03  SelectInventory
            Selecting items. Starts at icon index = 0.
            A   'picks up' icon - cursor chgs to 'picked  up', selected
                sprites around icon. A then drops the item into a grid slot.
                Can also drop icons into equipment slots, but only if allowed.
                If not allowed, cursor is x.
                If over spells/skills -> ShowSpells.
                Can also drop to another character or to shop/trash
                (trash bag shows when not in shop and not in body)
            B   If no icon selected, drops out to previous mode
                or screen.
                If item selected, stops item selected.
            SEL -> ItemDetails.
        04  ItemDetails - Shows description of currently selected thing. 
            Fade out all colors - item is a sprite.
            Move item sprite to UL. Replace sprite with bg.
            Write text and stats.
            Fade in text.
            B - back
        05  ShowSpells - list of all spells/skills that can be used.
        06  MerchantHaggleMode - not in demo. STRETCH feature.
*/
