using Core.GUI.Content;
using Core.GUI.Framework;
using Core.Input;
using eightbit.Data;
using eightbit.Data.SpriteData;
using eightbit.GUI.Elements;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace eightbit.GUI.Screens
{
    class SpriteScreen : Screen
    {
        public delegate void PaletteChangeEventHandler(int palette, int subindex, byte color);

        // ================================================================================
        // Overrides
        // ================================================================================

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

        // ================================================================================
        // Initialization
        // ================================================================================

        public override void OnInit()
        {
            int leftcolumn = 120;

            // Add panels: Control, List, CurrentSprite, and TileGfx
            Gui.AddWidgets(new Widget[] {
                panel_Control = new Window(4, 4 + 24, leftcolumn, 84),
                panel_List = new Window(4, 4 + panel_Control.Area.Bottom, leftcolumn, 592 - 28 - panel_Control.Area.Height),
                panel_CurrentSprite = new Window(leftcolumn + 8, 4 + 24, 800 - leftcolumn - 12, 592 - 24 - 4 - 276),
                panel_TileGfx = new Window(leftcolumn + 8, 4 + panel_CurrentSprite.Area.Bottom, panel_CurrentSprite.Area.Width, 276)
            });

            panel_Control.AddWidgets(new Widget[] {
                lblSpriteCount = new Label(8, 14, string.Empty),
                new Button(leftcolumn - 82, 4, 32, "+", click_AddSprite),
                new Button(leftcolumn - 46, 4, 32, "-", click_RemoveSprite)
            });

            panel_List.AddWidgets(new Widget[] {
                lstSprites = new ListBox<Sprite>(0, 0, panel_List.InputArea.Width, panel_List.InputArea.Height)
                { OnSelectionChanged = lstSprites_OnSelectionChanged }
            });

            panel_CurrentSprite.AddWidgets(new Widget[] {
                new Label(8,8, "Name:"),
                txtSpriteName = new SingleLineTextBox(52, 4, 136, 16) {
                    OnValueChanged = txtSpriteName_OnValueChanged },
                lblSpriteDataByte = new Label(8, 30, string.Empty),
                lblSpriteMetaDataSize = new Label(8, 46, string.Empty)
            });

            int y_offset = 32;
            panel_CurrentSprite.AddWidgets(new Widget[] {
                new Label(8, y_offset + 12 + 24 + 4, "Sprite size:"),
                cmbSpriteSize = new ComboBox(8 + 88, y_offset + 8 + 24, 78, string.Empty, dropDownItems: new List<ComboBox.DropDownItem>() {
                    new ComboBox.DropDownItem("8x8"), new ComboBox.DropDownItem("16x16"), 
                    new ComboBox.DropDownItem("24x24"), new ComboBox.DropDownItem("32x32") }) {
                    OnSelectionChanged = cmbSpriteSize_OnSelectionChanged },
                new Label(8, y_offset + 20 + 48 + 4, "Extra frames:"),
                cmbExtraFrames = new ComboBox(8 + 88, y_offset + 18 + 48, 78, string.Empty, dropDownItems: new List<ComboBox.DropDownItem>() {
                    new ComboBox.DropDownItem("0"), new ComboBox.DropDownItem("1"), new ComboBox.DropDownItem("2"), new ComboBox.DropDownItem("3"),
                    new ComboBox.DropDownItem("4"), new ComboBox.DropDownItem("8"), new ComboBox.DropDownItem("16"), new ComboBox.DropDownItem("16*") }) {
                    OnSelectionChanged = cmbExtraFrames_OnSelectionChanged },
                chkHasWalkSprites = new CheckBox(8, y_offset + 28 + 72, "Has Walk Sprites") {
                    OnToggle = chkHasWalkSprites_OnToggle },
                chkHasExtendedSprites = new CheckBox(8, y_offset + 32 + 96, "Has Extended Sprites") {
                    OnToggle = chkHasExtendedSprites_OnToggle },
            });

            panel_TileGfx.AddWidgets(new Widget[] {
                scrTileGfxPages = new ScrollBars()
            });

            createTileGfxPanel();
            lstMetaSpriteLabels = new List<Label>();
            lstMetaSprites = new List<MetaTile>();

            refreshControls();

            if (State.SelectedSprite >= 0 && State.SelectedSprite < State.Data.Sprites.Count)
                lstSprites.SelectedIndex = State.SelectedSprite;
            else
                lstSprites.SelectedIndex = State.SelectedSprite = 0;
        }

        // ================================================================================
        // Control management
        // ================================================================================

        Window panel_Control, panel_List, panel_CurrentSprite, panel_TileGfx;
        // main control
        Label lblSpriteCount;
        ComboBox cmbPalettes;
        // list
        ListBox<Sprite> lstSprites;
        // gfx pages
        ScrollBars scrTileGfxPages;
        List<TileGfxPage> m_GfxPages;
        // sprite display
        Label lblSpriteDataByte, lblSpriteMetaDataSize;
        CheckBox chkHasWalkSprites, chkHasExtendedSprites;
        ComboBox cmbSpriteSize, cmbExtraFrames;
        SingleLineTextBox txtSpriteName;
        List<Label> lstMetaSpriteLabels;
        List<MetaTile> lstMetaSprites;

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
                        Elements.TileGfxPage gfxpage = new Elements.TileGfxPage(0, 1, 256, 256, i, pages_OnClick);
                        gfxpage.X += (256 + 1) * i + 1;
                        gfxpage.RenderRule.Texture = State.GfxPage(i).Texture;
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

        private void createPaletteCombo()
        {
            if (State.Data.Palettes.Count > 0)
            {
                if (cmbPalettes == null)
                {
                    List<ComboBox.DropDownItem> pal4 = new List<ComboBox.DropDownItem>();
                    for (int i = 0; i < State.Data.Palettes.Count; i++)
                    {
                        Texture2D icon4;
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

                        pal4.Add(new ComboBox.DropDownItem(string.Format("0x{0:X2}", i), icon4));
                    }

                    panel_Control.AddWidget(
                    cmbPalettes = new ComboBox(4, 40, 90, string.Empty)
                        { OnSelectionChanged = cmbPalettes_SelectionChanged });
                    cmbPalettes.SetDropDownItems(pal4);
                    cmbPalettes.SelectedIndex = 0;
                }
            }
        }

        private void refreshControls()
        {
            int count = State.Data.Sprites.Count;
            lblSpriteCount.Value = string.Format("{0}", count);

            if (count == 0 || State.Data.Palettes.Count == 0 || State.Data.TileGfx.PageCount < 2)
            {
                if (cmbPalettes != null)
                    cmbPalettes.Visible = false;
                panel_List.Visible = false;
                panel_CurrentSprite.Visible = false;
                panel_TileGfx.Visible = false;
            }
            else
            {
                // load all palettes into combo viewer...
                createPaletteCombo();
                cmbPalettes.Visible = true;
                if (State.SelectedTileset < State.Data.TileSets.Count)
                    cmbPalettes.SelectedIndex = State.SelectedTileset;
                else
                    cmbPalettes.SelectedIndex = State.SelectedTileset = 0;

                panel_List.Visible = true;
                lstSprites.SetItems(State.Data.Sprites.ToArray());

                panel_CurrentSprite.Visible = true;
                panel_TileGfx.Visible = true;
            }
        }

        private bool m_SpriteDataDisplayRefreshInProgress = false;
        private void refreshSpriteDataDisplay()
        {
            if (!m_SpriteDataDisplayRefreshInProgress)
            {
                // this is required because updating any of the controls will cause all controls to update.
                m_SpriteDataDisplayRefreshInProgress = true;
                Sprite sprite = State.Data.Sprites[State.SelectedSprite];
                if (sprite == null)
                {
                    txtSpriteName.Value = string.Empty;
                    chkHasWalkSprites.IsToggled = false;
                    chkHasExtendedSprites.IsToggled = false;
                    cmbSpriteSize.SelectedIndex = -1;
                    cmbExtraFrames.SelectedIndex = -1;
                    lblSpriteDataByte.Value = string.Empty;
                    lblSpriteMetaDataSize.Value = string.Empty;
                }
                else
                {
                    txtSpriteName.Value = sprite.Name;
                    chkHasWalkSprites.IsToggled = sprite.HasStandardSprites;
                    chkHasExtendedSprites.IsToggled = sprite.HasExtendedSprites;
                    cmbSpriteSize.SelectedIndex = (int)sprite.SpriteSize;
                    cmbExtraFrames.SelectedIndex = (int)sprite.ExtraFrames;
                    lblSpriteDataByte.Value = string.Format("Header Data: 0x{0:X2}{1:X2}", sprite.DataByte, sprite.TileByte);
                    lblSpriteMetaDataSize.Value = string.Format("Data Size: {0}b", sprite.MetaDataSize + sprite.TileTransformTable.Length * 2);
                    int tileGfxSize = sprite.TileTransformTable.Length * 16;
                }
                refreshMetatileDisplay();
                m_SpriteDataDisplayRefreshInProgress = false;
            }
        }

        private bool m_InvalidateAllMetatiles = false;
        private void refreshMetatileDisplay()
        {
            Sprite sprite = State.Data.Sprites[State.SelectedSprite];
            if (sprite == null)
            {
                foreach (Label l in lstMetaSpriteLabels)
                    l.Visible = false;
                foreach (MetaTile m in lstMetaSprites)
                    m.Visible = false;
            }
            else
            {
                if (lstMetaSprites.Count != sprite.FrameCount || m_InvalidateAllMetatiles)
                {
                    m_InvalidateAllMetatiles = false;
                    foreach (Label l in lstMetaSpriteLabels)
                        l.Parent.RemoveWidget(l);
                    foreach (MetaTile m in lstMetaSprites)
                        m.Parent.RemoveWidget(m);
                    lstMetaSpriteLabels.Clear();
                    lstMetaSprites.Clear();

                    int x = 192;
                    int y = 8;

                    int tiles_w = cmbSpriteSize.SelectedIndex + 1;
                    int tiles_h = tiles_w;

                    if (sprite.HasStandardSprites)
                    {
                        Label lbl = new Label(x, y, "Standard Sprites:");
                        panel_CurrentSprite.AddWidget(lbl);
                        lstMetaSpriteLabels.Add(lbl);
                        for (int i = 0; i < 8; i++)
                        {
                            MetaTile m = new MetaTile(i, x + i * 38, y + 18, 32, 32, Metatile_OnClick, tiles_w, tiles_h);
                            panel_CurrentSprite.AddWidget(m);
                            lstMetaSprites.Add(m);
                            SpriteMetaTileFrame[] tiles = sprite.GetMetaTileFrame(Sprite.FrameTypeEnum.Standard, i);
                            for (int j = 0; j < tiles.Length; j++)
                            {
                                m.SetTile(j, tiles[j].Tile, State.GfxPage(tiles[j].TilePage).Texture);
                                m.SetTileFlip(j, tiles[j].FlipH, tiles[j].FlipV);
                            }
                        }
                        y += 56;
                    }
                    if (sprite.HasExtendedSprites)
                    {
                        Label lbl = new Label(x, y, "Extended Sprites:");
                        panel_CurrentSprite.AddWidget(lbl);
                        lstMetaSpriteLabels.Add(lbl);
                        for (int i = 0; i < 4; i++)
                        {
                            MetaTile m = new MetaTile(0x100 + i, x + i * 38, y + 18, 32, 32, Metatile_OnClick, tiles_w, tiles_h);
                            panel_CurrentSprite.AddWidget(m);
                            lstMetaSprites.Add(m);
                            SpriteMetaTileFrame[] tiles = sprite.GetMetaTileFrame(Sprite.FrameTypeEnum.Extended, i);
                            for (int j = 0; j < tiles.Length; j++)
                            {
                                m.SetTile(j, tiles[j].Tile, State.GfxPage(tiles[j].TilePage).Texture);
                                m.SetTileFlip(j, tiles[j].FlipH, tiles[j].FlipV);
                            }
                        }
                        y += 56;
                    }
                    if (sprite.ExtraFramesInt != 0)
                    {
                        Label lbl = new Label(x, y, "Extra Frames:");
                        panel_CurrentSprite.AddWidget(lbl);
                        lstMetaSpriteLabels.Add(lbl);
                        for (int i = 0; i < sprite.ExtraFramesInt; i++)
                        {
                            MetaTile m = new MetaTile(0x200 + i, x + (i % 12) * 38, y + 18 + (i / 12) * 38, 32, 32, Metatile_OnClick, tiles_w, tiles_h);
                            panel_CurrentSprite.AddWidget(m);
                            lstMetaSprites.Add(m);
                            SpriteMetaTileFrame[] tiles = sprite.GetMetaTileFrame(Sprite.FrameTypeEnum.Extra, i);
                            for (int j = 0; j < tiles.Length; j++)
                            {
                                m.SetTile(j, tiles[j].Tile, State.GfxPage(tiles[j].TilePage).Texture);
                                m.SetTileFlip(j, tiles[j].FlipH, tiles[j].FlipV);
                            }
                        }
                    }
                }
                foreach (Label l in lstMetaSpriteLabels)
                    l.Visible = true;
                foreach (MetaTile m in lstMetaSprites)
                    m.Visible = true;
            }
        }

        // ================================================================================
        // Input
        // ================================================================================

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

        private void click_AddSprite(Widget widget)
        {
            State.Data.Sprites.AddSprite();
            refreshControls();
            lstSprites.SelectedIndex = State.Data.Sprites.Count - 1;
        }

        private void click_RemoveSprite(Widget widget)
        {
            State.Data.Sprites.RemoveLastSprite();
            refreshControls();
            if (State.SelectedSprite >= State.Data.Sprites.Count)
            {
                State.SelectedSprite = lstSprites.SelectedIndex = State.Data.Sprites.Count - 1;
            }
        }

        private void cmbPalettes_SelectionChanged(Widget widget)
        {
            State.Palette.LoadPalette(0, State.Data.Palettes[((Core.GUI.Content.ComboBox)widget).SelectedIndex]);
        }

        private void lstSprites_OnSelectionChanged(Widget widget)
        {
            State.SelectedSprite = lstSprites.SelectedIndex;
            refreshSpriteDataDisplay();
        }

        private void txtSpriteName_OnValueChanged(Widget widget)
        {
            Sprite sprite = State.Data.Sprites[State.SelectedSprite];
            if (sprite != null)
            {
                sprite.Name = ((TextBox)widget).Value;
            }
        }

        private void cmbSpriteSize_OnSelectionChanged(Widget widget)
        {
            Sprite sprite = State.Data.Sprites[State.SelectedSprite];
            if (sprite != null)
            {
                sprite.SpriteSize = (Sprite.SpriteSizeEnum)((ComboBox)widget).SelectedIndex;
                m_InvalidateAllMetatiles = true;
                refreshSpriteDataDisplay();
            }
        }

        private void cmbExtraFrames_OnSelectionChanged(Widget widget)
        {
            Sprite sprite = State.Data.Sprites[State.SelectedSprite];
            if (sprite != null)
            {
                sprite.ExtraFrames = (Sprite.ExtraFramesEnum)((ComboBox)widget).SelectedIndex;
                refreshSpriteDataDisplay();
            }
        }

        private void chkHasWalkSprites_OnToggle(Widget widget)
        {
            Sprite sprite = State.Data.Sprites[State.SelectedSprite];
            if (sprite != null)
            {
                sprite.HasStandardSprites = ((CheckBox)widget).IsToggled;
                refreshSpriteDataDisplay();
            }
        }

        private void chkHasExtendedSprites_OnToggle(Widget widget)
        {
            Sprite sprite = State.Data.Sprites[State.SelectedSprite];
            if (sprite != null)
            {
                sprite.HasExtendedSprites = ((CheckBox)widget).IsToggled;
                refreshSpriteDataDisplay();
            }
        }

        private void Metatile_OnClick(Widget widget, InputEventMouse e)
        {
            Sprite sprite = State.Data.Sprites[State.SelectedSprite];
            if (sprite != null)
            {
                MetaTile metatile = (MetaTile)widget;
                Sprite.FrameTypeEnum frametype;
                if (metatile.Index >= 0x200)
                    frametype = Sprite.FrameTypeEnum.Extra;
                else if (metatile.Index >= 0x100)
                    frametype = Sprite.FrameTypeEnum.Extended;
                else
                    frametype = Sprite.FrameTypeEnum.Standard;
                SpriteMetaTileFrame tile = sprite.GetMetaTileFrame(frametype, metatile.Index & 0xFF)[metatile.ClickedTile];
                if (e.Button == MouseButton.Left)
                {
                    tile.Tile = (byte)State.SelectedTile;
                    tile.TilePage = (byte)(State.SelectedPage);
                    metatile.SetTile(metatile.ClickedTile, tile.Tile, State.GfxPage(tile.TilePage).Texture);
                }
                else if (e.Button == MouseButton.Right)
                {
                    tile.Flip();
                    metatile.SetTileFlip(metatile.ClickedTile, tile.FlipH, tile.FlipV);
                }
                refreshSpriteDataDisplay();
            }
        }
    }
}
