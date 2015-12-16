using System.Collections.Generic;
using Core.GUI.Content;
using Core.GUI.Framework;
using Core.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace eightbit.GUI.Screens
{
    class TilesetScreen : Screen
    {
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
            Gui.AddWidgets(new Widget[] {
                panel_TileGfx = new Elements.Window(4, 32 + 124, 274, 592 - 28 - 124),
                panel_Palettes = new Elements.Window(282, 28, 792 - 278, 84),
                panel_Metatiles = new Elements.Window(282, 32 + 84, 792 - 278, 592 - 28 - 84),
                panel_Control = new Elements.Window(4, 28, 274, 124),
            });

            panel_TileGfx.Children = new Widget[] {
                scrTileGfxPages = new ScrollBars() };

            panel_Metatiles.Children = new Widget[] {
                scrMetaTiles = new ScrollBars() };

            panel_Control.AddWidgets(new Widget[] {
                lbl_TileSetCount = new Label(8, 14, string.Empty),
                new Button(192, 4, 32, "+", click_AddSet),
                new Button(228, 4, 32, "-", click_RemoveSet),
                lbl_TileSetsCaption = new Label(8, 48, "TileSets:"),
                cmb_Tilesets = new ComboBox(82, 40, 164, string.Empty)
                    { OnSelectionChanged = tileset_Change },
                new Label(8, 87, "Edit:"),
                btn_DrawTiles = new Core.GUI.Content.ToggleButton(8 + 32, 78, 68, "Tiles")
                    { OnToggle = btnEditTiles_OnToggle },
                btn_DrawAttributes = new Core.GUI.Content.ToggleButton(14 + 68 + 32, 78, 68, "Attribs")
                    { OnToggle = btnEditAttributes_OnToggle },
                btn_DrawFlags = new Core.GUI.Content.ToggleButton(20 + 68 * 2 + 32, 78, 68, "Flags")
                    { OnToggle = btnEditBitflags_OnToggle },
            });

            createPalettePanel();
            createTileGfxPanel();
            createMetaTilePanel();

            refreshEditMode();
            refreshControlPanel();
        }

        private void refreshEditMode()
        {
            switch (m_EditMode)
            {
                case EditMode.EditTiles:
                    btn_DrawTiles.IsToggled = true;
                    btn_DrawAttributes.IsToggled = false;
                    btn_DrawFlags.IsToggled = false;
                    break;
                case EditMode.EditAttributes:
                    btn_DrawTiles.IsToggled = false;
                    btn_DrawAttributes.IsToggled = true;
                    btn_DrawFlags.IsToggled = false;
                    break;
                case EditMode.EditBitflags:
                    btn_DrawTiles.IsToggled = false;
                    btn_DrawAttributes.IsToggled = false;
                    btn_DrawFlags.IsToggled = true;
                    break;
            }

            for (int i = 0; i < m_MetaTiles.Count; i++)
            {
                m_MetaTiles[i].DrawFlags = (m_EditMode == EditMode.EditBitflags);
            }
        }

        private void refreshControlPanel()
        {
            int count = State.Data.TileSets.Count;
            lbl_TileSetCount.Value = string.Format("0x{0:X2} ({0}) TileSets", count);
            if (count == 0 || State.Data.Palettes.Count == 0 || State.Data.TileGfx.PageCount == 0)
            {
                lbl_TileSetsCaption.Visible = false;
                cmb_Tilesets.Visible = false;
                panel_Metatiles.Visible = false;
                panel_Palettes.Visible = false;
                panel_TileGfx.Visible = false;
                btn_DrawAttributes.Visible = false;
                btn_DrawTiles.Visible = false;
            }
            else
            {
                lbl_TileSetsCaption.Visible = true;
                cmb_Tilesets.Visible = true;
                List<ComboBox.DropDownItem> tilesets = new List<ComboBox.DropDownItem>();
                for (int i = 0; i < count; i++)
                    tilesets.Add(new ComboBox.DropDownItem(string.Format("Tileset 0x{0:X2}", i)));
                cmb_Tilesets.SetDropDownItems(tilesets);
                if (State.SelectedTileset < count)
                    cmb_Tilesets.SelectedIndex = State.SelectedTileset;
                else
                    cmb_Tilesets.SelectedIndex = State.SelectedTileset = 0;

                panel_Metatiles.Visible = true;

                panel_Palettes.Visible = true;
                palette_OnClick(m_Palettes[0]);

                panel_TileGfx.Visible = true;
                btn_DrawAttributes.Visible = true;
                btn_DrawTiles.Visible = true;
            }
        }

        private void createPalettePanel()
        {
            if (State.Data.Palettes.Count > 0)
            {
                if (m_PaletteCombos == null)
                {
                    List<ComboBox.DropDownItem> pal4 = new List<ComboBox.DropDownItem>();
                    List<ComboBox.DropDownItem> pal3 = new List<ComboBox.DropDownItem>();
                    for (int i = 0; i < State.Data.Palettes.Count; i++)
                    {
                        Texture2D icon4, icon3;
                        byte[] pal;
                        uint[] colors, pixels;

                        pal = State.Data.Palettes[i];
                        colors = new uint[4] { 
                            NES.Palette.ColorFromIndex(pal[0]), 
                            NES.Palette.ColorFromIndex(pal[1]), 
                            NES.Palette.ColorFromIndex(pal[2]),
                            NES.Palette.ColorFromIndex(pal[3])};
                        pixels = new uint[8 * 4 * 16];
                        for (int j = 0; j < pixels.Length; j++)
                            pixels[j] = colors[(j % 32) / 8];
                        icon4 = Core.Library.CreateTexture(32, 16);
                        icon4.SetData<uint>(pixels);

                        pal = State.Data.Palettes[i];
                        colors = new uint[3] { 
                            NES.Palette.ColorFromIndex(pal[1]), 
                            NES.Palette.ColorFromIndex(pal[2]),
                            NES.Palette.ColorFromIndex(pal[3])};
                        pixels = new uint[11 * 3 * 16];
                        for (int j = 0; j < pixels.Length; j++)
                            pixels[j] = colors[(j % 33) / 11];
                        icon3 = Core.Library.CreateTexture(33, 16);
                        icon3.SetData<uint>(pixels);

                        pal4.Add(new ComboBox.DropDownItem(string.Format("0x{0:X2} ({0})", i), icon4));
                        pal3.Add(new ComboBox.DropDownItem(string.Format("0x{0:X2} ({0})", i), icon3));
                    }

                    m_PaletteCombos = new List<ComboBox>();
                    m_Palettes = new List<Elements.PaletteFourColor>();

                    for (int i = 0; i < 4; i++)
                    {
                        ComboBox combo = new ComboBox(
                            8 + 124 * i, 4, 105, string.Empty) {
                                OnSelectionChanged = palette_OnChange };
                        combo.SetDropDownItems((i == 0) ? pal4 : pal3);
                        combo.SelectedIndex = 0;
                        m_PaletteCombos.Add(combo);

                        Elements.PaletteFourColor box = new Elements.PaletteFourColor(
                            8 + 124 * i, 41, 116, 30, 0, 0, 0, palette_OnClick);
                        box.ColorIndex = (i * 4) + ((i * 4 + 1) << 8) + ((i * 4 + 2) << 16) + ((i * 4 + 3) << 24);

                        m_Palettes.Add(box);
                        panel_Palettes.AddWidget(box);
                    }
                    panel_Palettes.AddWidgets(m_PaletteCombos);
                    panel_Palettes.AddWidgets(m_Palettes);
                }
            }
        }

        private void createTileGfxPanel()
        {
            if (m_GfxPages == null)
            {
                scrTileGfxPages.RemoveAllWidgets();
                m_GfxPages = new List<Elements.TileGfxPage>();
                m_GfxPages.Clear();
                if (State.Data.TileGfx.PageCount > 0)
                {
                    for (int i = 0; i < State.Data.TileGfx.PageCount; i++)
                    {
                        Elements.TileGfxPage gfxpage = new Elements.TileGfxPage(0, 0, 256, 256, i, pages_OnClick);
                        gfxpage.Y += (256 + 18) * i + 18;
                        gfxpage.RenderRule.Texture = State.GfxPage(i).Texture;
                        scrTileGfxPages.AddWidget(new Label(4, (256 + 18) * i + 2, string.Format("Tiles 0x{0:X4}-0x{1:X4}", i * 256, i * 256 + 255)));
                        scrTileGfxPages.AddWidget(gfxpage);
                        m_GfxPages.Add(gfxpage);
                    }
                    if (State.SelectedPage >= m_GfxPages.Count)
                        pages_OnClick(0, 0);
                    else
                        pages_OnClick(State.SelectedPage, State.SelectedTile);
                }
            }
        }

        private void createMetaTilePanel()
        {
            if (m_MetaTiles == null)
            {
                m_MetaTiles = new List<Elements.MetaTile>();
                int tilesperrow = 8, tilesize = 48, tilemargin = 8;
                int x = 16, y = 4;
                bool loadtiles = (State.Data.TileSets[State.SelectedTileset] != null && State.Data.Palettes.Count != 0 && State.Data.TileGfx.PageCount != 0);
                for (int i = 0; i < Data.TileSet.TilesPerSet; i++)
                {
                    Elements.MetaTile metatile = new Elements.MetaTile(i,
                        x + (tilesize + tilemargin) * (i % tilesperrow), y + 18 + (tilesize + tilemargin) * (i / tilesperrow), tilesize, tilesize, metatile_OnClick);
                    if (loadtiles)
                    {
                        for (int j = 0; j < 4; j++)
                        {
                            Data.TilePageAttribute tile_page_attrib = State.Data.TileSets[State.SelectedTileset].GetSubTile(i, j);
                            metatile.SetTile(j, tile_page_attrib.Tile, State.GfxPage(tile_page_attrib.Page).Texture);
                            metatile.Attribute = tile_page_attrib.Attribute;
                        }
                    }
                    m_MetaTiles.Add(metatile);
                }
                scrMetaTiles.AddWidget(
                    new Label(x, y, "MetaTiles:"));
                scrMetaTiles.AddWidgets(m_MetaTiles);
            }
        }

        // panels
        Elements.Window panel_Control, panel_Metatiles, panel_Palettes, panel_TileGfx;
        // tileset controls
        Label lbl_TileSetCount, lbl_TileSetsCaption;
        ToggleButton btn_DrawTiles, btn_DrawAttributes, btn_DrawFlags;
        ComboBox cmb_Tilesets;
        // gfx pages
        ScrollBars scrTileGfxPages;
        List<Elements.TileGfxPage> m_GfxPages;
        // palette selectors
        List<ComboBox> m_PaletteCombos;
        List<Elements.PaletteFourColor> m_Palettes;
        // metatiles
        ScrollBars scrMetaTiles;
        List<Elements.MetaTile> m_MetaTiles;

        private void click_AddSet(Widget widget)
        {
            State.Data.TileSets.AddTileset();
            refreshControlPanel();
            cmb_Tilesets.SelectedIndex = State.SelectedTileset;
        }

        private void click_RemoveSet(Widget widget)
        {
            State.Data.TileSets.RemoveLastTileset();
            refreshControlPanel();
            if (State.SelectedTileset >= State.Data.TileSets.Count)
                cmb_Tilesets.SelectedIndex = State.Data.TileSets.Count - 1;
        }

        private void tileset_Change(Widget widget)
        {
            State.SelectedTileset = ((ComboBox)widget).SelectedIndex;
            Data.TileSet tileset = State.Data.TileSets[State.SelectedTileset];
            for (int i = 0; i < 4; i++)
            {
                if (tileset.GetPalette(i) < 0 || tileset.GetPalette(i) >= State.Data.Palettes.Count)
                    tileset.SetPalette(i, 0);
                m_PaletteCombos[i].SelectedIndex = tileset.GetPalette(i);
            }

            bool loadtiles = (State.Data.TileSets[State.SelectedTileset] != null);
            if (loadtiles)
            {
                for (int i = 0; i < Data.TileSet.TilesPerSet; i++)
                {
                    m_MetaTiles[i].Flags = State.Data.TileSets[State.SelectedTileset].GetFlags(i);
                    for (int j = 0; j < 4; j++)
                    {
                        Data.TilePageAttribute tile_page_attrib = State.Data.TileSets[State.SelectedTileset].GetSubTile(i, j);
                        m_MetaTiles[i].SetTile(j, tile_page_attrib.Tile, State.GfxPage(tile_page_attrib.Page).Texture);
                        m_MetaTiles[i].Attribute = tile_page_attrib.Attribute;
                    }
                }
            }
        }

        private void palette_OnChange(Widget widget)
        {
            int pal_index = indexInList<ComboBox>((ComboBox)widget, m_PaletteCombos);
            if (pal_index == -1)
                return;
            ComboBox combo = (ComboBox)widget;
            if (combo.SelectedIndex < 0 || combo.SelectedIndex >= State.Data.Palettes.Count)
                combo.SelectedIndex = 0;

            State.Data.TileSets[State.SelectedTileset].SetPalette(pal_index, (byte)combo.SelectedIndex);
            for (int i = 0; i < 4; i++)
            {
                byte[] palette = new byte[4];
                palette[0] = State.Data.Palettes[m_PaletteCombos[0].SelectedIndex][0];
                for (int j = 1; j < 4; j++)
                    palette[j] = State.Data.Palettes[m_PaletteCombos[i].SelectedIndex][j];
                State.Palette.LoadPalette(i, palette);
            }
        }

        private void palette_OnClick(Widget widget)
        {
            int index = indexInList<Elements.PaletteFourColor>((Elements.PaletteFourColor)widget, m_Palettes);
            if (index == -1)
                return;
            State.SelectedPalette = index;
            for (int i = 0; i < 4; i++)
                m_Palettes[i].BorderColor = (i == State.SelectedPalette) ? Color.White : Color.Black;
            for (int i = 0; i < m_GfxPages.Count; i++)
                m_GfxPages[i].Attribute = index;
        }

        private void btnEditTiles_OnToggle(Widget widget)
        {
            if ((m_EditMode != EditMode.EditTiles && ((ToggleButton)widget).IsToggled == true) ||
                (m_EditMode == EditMode.EditTiles && ((ToggleButton)widget).IsToggled == false))
            {
                m_EditMode = EditMode.EditTiles;
                refreshEditMode();
            }
        }

        private void btnEditAttributes_OnToggle(Widget widget)
        {
            if ((m_EditMode != EditMode.EditAttributes && ((ToggleButton)widget).IsToggled == true) ||
                (m_EditMode == EditMode.EditAttributes && ((ToggleButton)widget).IsToggled == false))
            {
                m_EditMode = EditMode.EditAttributes;
                refreshEditMode();
            }
        }

        private void btnEditBitflags_OnToggle(Widget widget)
        {
            if ((m_EditMode != EditMode.EditBitflags && ((ToggleButton)widget).IsToggled == true) ||
                (m_EditMode == EditMode.EditBitflags && ((ToggleButton)widget).IsToggled == false))
            {
                m_EditMode = EditMode.EditBitflags;
                refreshEditMode();
            }
        }

        private void pages_OnClick(int page_index, int tile_index)
        {
            State.SelectedTile = tile_index;
            State.SelectedPage = page_index;
            foreach (Elements.TileGfxPage page in m_GfxPages)
            {
                if (page.PageIndex == page_index)
                {
                    page.HasSelection = true;
                    page.SelectedTile = tile_index;
                }
                else
                    page.HasSelection = false;
            }
        }

        private void metatile_OnClick(Widget widget, InputEventMouse e)
        {
            Elements.MetaTile metatile = (Elements.MetaTile)widget;

            switch (m_EditMode)
            {
                case EditMode.EditTiles:
                    metatile.Attribute = (byte)State.SelectedPalette;
                    State.Data.TileSets[State.SelectedTileset].SetAttribute(metatile.Index, (byte)State.SelectedPalette);
                    break;
                case EditMode.EditAttributes:
                    metatile.SetTile(metatile.ClickedTile, (byte)State.SelectedTile, State.GfxPage(State.SelectedPage).Texture);
                    State.Data.TileSets[State.SelectedTileset].SetSubTileAndPage(metatile.Index, metatile.ClickedTile, (byte)State.SelectedTile, (byte)State.SelectedPage);
                    break;
                case EditMode.EditBitflags:
                    State.Data.TileSets[State.SelectedTileset].ToggleFlag(metatile.Index, metatile.ClickedFlag);
                    metatile.Flags = State.Data.TileSets[State.SelectedTileset].GetFlags(metatile.Index);
                    break;
            }
        }

        internal override void OnKeyboardPress(InputEventKeyboard args)
        {

        }

        // Helpers

        private int indexInList<T>(T widget, List<T> list) where T : Widget
        {
            for (int i = 0; i < list.Count; i++)
                if (list[i] == widget)
                    return i;
            return -1;
        }

        private EditMode m_EditMode = EditMode.EditTiles;
        enum EditMode
        {
            EditTiles,
            EditAttributes,
            EditBitflags
        }
    }
}
