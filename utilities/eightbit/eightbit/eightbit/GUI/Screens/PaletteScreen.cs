using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Core.GUI.Content;
using Core.GUI.Framework;

namespace eightbit.GUI.Screens
{
    class PaletteScreen : Screen
    {
        public delegate void PaletteChangeEventHandler(int palette, int subindex, byte color);

        public override void OnResize()
        {
            Gui.Resize();
        }

        public override void Update()
        {
            Gui.Update();
        }

        public override void Draw()
        {
            Gui.Draw();
        }

        public override void OnInit()
        {
            State.Palette.Reset();

            Gui.AddWidgets(new Widget[] {
                win_Palettes = new Elements.Window(4, 4 + 24, 596, 592 - 24, Elements.WindowButtons.None),
                win_NESPalette = new Elements.Window(604, 4 + 24, 192, 592 - 24, Elements.WindowButtons.None)
            });

            win_Palettes.AddWidget(Controls = new ScrollBars());
            Controls.AddWidgets(new Widget[]
                {
                    lbl_Title = new Label(16, 24, string.Empty),
                    btn_AddPalette = new Button(584 - 60 * 2, 16, 48, "+", click_AddPalette),
                    btn_SubPalette = new Button(584 - 60 * 1, 16, 48, "-", click_RemovePalette),
                });

            win_NESPalette.AddWidgets(new Widget[]
            {
                _palette = new Elements.PaletteNES(0, 48, 184, 584 - 48 - 24),
                new Label(8, 16, "Sort:"),
                new ComboBox(44, 8, 120, string.Empty, CardinalDirection.South, new List<ComboBox.DropDownItem> {
                    new ComboBox.DropDownItem("NES Standard"),
                    new ComboBox.DropDownItem("By Hue"),
                }) { SelectedIndex = 1, OnSelectionChanged = sort_change }
            });

            for (int i = 0; i < State.Data.Palettes.Count; i++)
                addPaletteControls(i, State.Data.Palettes[i][0], State.Data.Palettes[i][1], State.Data.Palettes[i][2], State.Data.Palettes[i][3]);
            refreshLabelCount();
        }

        Label lbl_Title;
        Button btn_AddPalette, btn_SubPalette;
        Elements.Window win_Palettes, win_NESPalette;
        ScrollBars Controls;
        Elements.PaletteNES _palette;

        private void addPaletteControls(int index, byte a, byte b, byte c, byte d)
        {
            int y = index * 46;
            Controls.AddWidgets(new Widget[] {
                new Label(16, 56 + 8 + y, string.Format("Palette 0x{0:X2}", index)),
                new Elements.PaletteSingleColor(112 + 64 * 0, 57 + y, 54, 30, a, index, 0, wells_OnClick),
                new Elements.PaletteSingleColor(112 + 64 * 1, 57 + y, 54, 30, b, index, 1, wells_OnClick),
                new Elements.PaletteSingleColor(112 + 64 * 2, 57 + y, 54, 30, c, index, 2, wells_OnClick),
                new Elements.PaletteSingleColor(112 + 64 * 3, 57 + y, 54, 30, d, index, 3, wells_OnClick),
                new Button(584 - 60 * 3 - 32, 56 + y, 78, "In use by..."),
                new Button(584 - 60 * 2, 56 + y, 48, "Copy"),
                new Button(584 - 60 * 1, 56 + y, 48, "Paste")
            });
        }

        private void removePaletteControls()
        {
            if (State.Data.Palettes.Count == 0)
                return;
            for (int i = 0; i < 8; i++)
                Controls.RemoveWidget(Controls.LastWidget);
        }

        private void refreshLabelCount()
        {
            lbl_Title.Value = string.Format("0x{0:X2} ({0}) Palettes", State.Data.Palettes.Count);
        }

        private void sort_change(Widget widget)
        {
            int value = ((ComboBox)widget).SelectedIndex;
            _palette.Sort = (Elements.PaletteNES.SortMode)value;
        }

        private void click_AddPalette(Widget widget)
        {
            State.Data.Palettes.AddPalette(0, 0, 0, 0);
            addPaletteControls(State.Data.Palettes.Count - 1, 0, 0, 0, 0);
            refreshLabelCount();
        }

        private void click_RemovePalette(Widget widget)
        {
            removePaletteControls();
            State.Data.Palettes.RemovePalette();
            refreshLabelCount();
        }

        private void wells_OnClick(Widget widget)
        {
            Elements.PaletteSingleColor well = (Elements.PaletteSingleColor)widget;
            int value = State.SelectedColor;
            if (value >= 0 && value < 64)
            {
                well.ColorIndex = value;
                State.Data.Palettes[well.PaletteIndex][well.PaletteSubIndex] = (byte)value;
            }
            
        }
    }
}
