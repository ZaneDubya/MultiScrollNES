using System.Collections.Generic;
using Core.GUI.Content;
using Core.GUI.Framework;
using Core.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace eightbit.GUI.Screens
{
    class TileGfxScreen : Screen
    {
        public delegate void PageClickHandler(int page_index, int tile_index);
        public delegate void TileClickHandler(Elements.TileGfxTile tile, InputEventMouse e, int pixel);

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
                panel_Editor = new Elements.Window(274 + 8, 4 + 24, 514, 592 - 24, Elements.WindowButtons.None),
                panel_TileControl = new Elements.Window(4, 4 + 24, 274, 112),
                panel_Tiles = new Elements.Window(4, 4 + 28 + 112, 274, 592 - 28 - 112)
            });

            panel_TileControl.AddWidgets(new Widget[] {
                lbl_TileCount = new Label(8, 14, string.Empty),
                btn_AddTiles = new Button(192, 4, 32, "+", click_AddTiles),
                btn_SubTiles = new Button(228, 4, 32, "-", click_SubTiles),
                new Label(8, 48, "Palette:"),
                combo_Palettes = new ComboBox(82, 40, 164, string.Empty)
                    { OnSelectionChanged = OnPaletteChange }
            });

            for (int i = 0; i < 4; i++)
            {
                Elements.PaletteSingleColor well = new Elements.PaletteSingleColor(82 + 45 * i, 76, 41, 24, i, 0, 0, wells_OnClick);
                panel_TileControl.AddWidget(well);
                m_Wells.Add(well);
            }

            panel_Tiles.AddWidget(
                scr_TileGfxPages = new ScrollBars());

            refreshControlPanel();
            refreshTilePalePanel();
            refreshTiles();
            refreshPaletteCombo();
            wells_OnClick(m_Wells[0]);
        }

        Label lbl_TileCount;
        Button btn_AddTiles, btn_SubTiles;
        Elements.Window panel_Tiles, panel_TileControl, panel_Editor;
        ScrollBars scr_TileGfxPages;
        ComboBox combo_Palettes;
        List<Elements.TileGfxPage> m_Pages = new List<Elements.TileGfxPage>();
        List<Elements.TileGfxTile> m_Tiles = new List<Elements.TileGfxTile>();
        List<Elements.PaletteSingleColor> m_Wells = new List<Elements.PaletteSingleColor>();

        private void refreshTilePalePanel()
        {
            scr_TileGfxPages.RemoveAllWidgets();
            m_Pages.Clear();
            if (State.Data.TileGfx.PageCount > 0)
            {
                for (int i = 0; i < State.Data.TileGfx.PageCount; i++)
                {
                    Elements.TileGfxPage gfxpage = new Elements.TileGfxPage(0, 0, 256, 256, i, pages_OnClick);
                    gfxpage.Y += (256 + 18) * i + 18;
                    gfxpage.RenderRule.Texture = State.GfxPage(i).Texture;
                    scr_TileGfxPages.AddWidget(new Label(4, (256 + 18) * i + 2, string.Format("Tiles 0x{0:X4}-0x{1:X4}", i * 256, i * 256 + 255)));
                    scr_TileGfxPages.AddWidget(gfxpage);
                    m_Pages.Add(gfxpage);
                }
                if (State.SelectedPage >= m_Pages.Count)
                    pages_OnClick(0, 0);
                else
                    pages_OnClick(State.SelectedPage, State.SelectedTile);
            }
        }

        private void refreshTiles()
        {
            if (State.Data.TileGfx.PageCount > 0)
            {
                if (m_Tiles.Count == 0)
                {
                    panel_Editor.RemoveAllWidgets();
                    m_Tiles.Clear();
                    int pixelscale = 16;
                    float tilescale = 8.5f;
                    int rows = (int)System.Math.Ceiling((double)(panel_Editor.Area.Width / pixelscale - 8) / 18);
                    int columns = (int)System.Math.Ceiling((double)(panel_Editor.Area.Height / pixelscale - 8) / 18);
                    int x = (panel_Editor.Area.Width / 2) - (pixelscale * 4) - rows * (int)(pixelscale * tilescale);
                    int y = (panel_Editor.Area.Height / 2) - (pixelscale * 4) - columns * (int)(pixelscale * tilescale);
                    for (int iY = -columns; iY <= columns; iY++)
                    {
                        for (int iX = -rows; iX <= rows; iX++)
                        {
                            Elements.TileGfxTile tile = new Elements.TileGfxTile(
                                x + ((iX + columns) * (int)(tilescale * pixelscale)),
                                y + ((iY + rows) * (int)(tilescale * pixelscale)),
                                pixelscale, iX, iY, tiles_OnClick, tiles_OnMouseMove);
                            panel_Editor.AddWidget(tile);
                            m_Tiles.Add(tile);
                        }
                    }
                }

                foreach (Elements.TileGfxTile tile in m_Tiles)
                {
                    int x = tile.TileX + (State.SelectedTile % 16);
                    int y = tile.TileY + (State.SelectedTile / 16);
                    if (x < 0 || x >= 16 || y < 0 || y >= 16)
                        tile.Visible = false;
                    else
                    {
                        tile.Visible = true;
                        tile.TileIndex = x + y * 16;
                        tile.PageTexture = State.GfxPage(State.SelectedPage).Texture;
                    }
                }
            }
            else if (State.Data.TileGfx.PageCount == 0 && m_Tiles.Count > 0)
            {
                panel_Editor.RemoveAllWidgets();
                m_Tiles.Clear();
            }
        }

        private void refreshControlPanel()
        {
            int count = State.Data.TileGfx.TileCount;
            lbl_TileCount.Value = string.Format("0x{0:X4} ({0}) Tiles", count);
            if (count == 0 || State.Data.Palettes.Count == 0)
            {
                panel_Editor.Visible = false;
                panel_Tiles.Visible = false;
            }
            else
            {
                panel_Editor.Visible = true;
                panel_Tiles.Visible = true;
            }
        }

        private void refreshPaletteCombo()
        {
            if (State.Data.Palettes.Count > 0)
            {
                List<ComboBox.DropDownItem> palettes = new List<ComboBox.DropDownItem>();
                for (int i = 0; i < State.Data.Palettes.Count; i++)
                {
                    byte[] pal = State.Data.Palettes[i];
                    uint[] colors = new uint[4] { 
                    NES.Palette.ColorFromIndex(pal[0]), 
                    NES.Palette.ColorFromIndex(pal[1]), 
                    NES.Palette.ColorFromIndex(pal[2]),
                    NES.Palette.ColorFromIndex(pal[3])};
                    uint[] pixels = new uint[16 * 4 * 16];
                    for (int j = 0; j < 16 * 4 * 16; j++)
                        pixels[j] = colors[(j % 64) / 16];
                    Texture2D icon = Core.Library.CreateTexture(64, 16);
                    icon.SetData<uint>(pixels);

                    palettes.Add(new ComboBox.DropDownItem(string.Format("0x{0:X2} ({0})", i), icon));
                }
                combo_Palettes.SetDropDownItems(palettes);
                combo_Palettes.SelectedIndex = 0;
            }
        }

        private void OnPaletteChange(Widget widget)
        {
            State.Palette.LoadPalette(0, State.Data.Palettes[((Core.GUI.Content.ComboBox)widget).SelectedIndex]);
        }

        private void click_AddTiles(Widget widget)
        {
            State.Data.TileGfx.AddTilePage();
            refreshControlPanel();
            refreshTilePalePanel();
            refreshTiles();
        }

        private void click_SubTiles(Widget widget)
        {
            State.Data.TileGfx.RemoveTilePage();
            refreshControlPanel();
            refreshTilePalePanel();
            refreshTiles();
        }

        private void pages_OnClick(int page_index, int tile_index)
        {
            State.SelectedTile = tile_index;
            State.SelectedPage = page_index;
            foreach (Elements.TileGfxPage page in m_Pages)
            {
                if (page.PageIndex == page_index)
                {
                    page.HasSelection = true;
                    page.SelectedTile = tile_index;
                }
                else
                    page.HasSelection = false;
            }
            refreshTiles();
        }

        private void tiles_OnClick(Elements.TileGfxTile tile, InputEventMouse e, int pixel)
        {
            if (e.Button == MouseButton.Left)
            {
                if (tile.TileX != 0 || tile.TileY != 0)
                {
                    pages_OnClick(State.SelectedPage, tile.TileIndex);
                }
                else
                {
                    State.SetPixel(State.SelectedPage, State.SelectedTile, pixel, (byte)State.SelectedColor);
                }
            }
            else if (e.Button == MouseButton.Right)
            {
                byte color = State.Data.TileGfx.GetPixel(State.SelectedPage, State.SelectedTile + tile.TileX + tile.TileY * 16, pixel);
                wells_OnClick(m_Wells[color]);
            }
        }

        private void tiles_OnMouseMove(Elements.TileGfxTile tile, InputEventMouse e, int pixel)
        {
            if (tile.TileX == 0 && tile.TileY == 0)
                tiles_OnClick(tile, e, pixel);
        }

        private void wells_OnClick(Widget widget)
        {
            Elements.PaletteSingleColor well = (Elements.PaletteSingleColor)widget;
            foreach (Elements.PaletteSingleColor w in m_Wells)
                w.BorderColor = (w == well) ? Color.White : Color.Black;
            State.SelectedColor = well.ColorIndex;
        }

        internal override void OnKeyboardPress(InputEventKeyboard args)
        {
            // Ctrl-X: Cut
            if (args.KeyCode == WinKeys.X && args.Control)
            {
                byte[] data = new byte[64];
                for (int i = 0; i < 64; i++)
                {
                    data[i] = State.Data.TileGfx.GetPixel(State.SelectedPage, State.SelectedTile, i);
                    State.SetPixel(State.SelectedPage, State.SelectedTile, i, 0);
                }
                State.Clipboard.Cut(ClipboardData.Tile, data);
            }
            // Ctrl-C: Copy
            if (args.KeyCode == WinKeys.C && args.Control)
            {
                byte[] data = new byte[64];
                for (int i = 0; i < 64; i++)
                {
                    data[i] = State.Data.TileGfx.GetPixel(State.SelectedPage, State.SelectedTile, i);
                }
                State.Clipboard.Copy(ClipboardData.Tile, data);
            }
            // Ctrl-V: Paste
            if (args.KeyCode == WinKeys.V && args.Control)
            {
                if (!State.Clipboard.HasData || State.Clipboard.DataType != ClipboardData.Tile)
                    return;
                byte[] data = State.Clipboard.Paste();
                for (int i = 0; i < 64; i++)
                {
                    State.SetPixel(State.SelectedPage, State.SelectedTile, i, data[i]);
                }
            }
            // Ctrl-H: Horizontal Mirror
            if (args.KeyCode == WinKeys.H && args.Control)
            {
                byte[] data = new byte[64];
                for (int i = 0; i < 64; i++)
                    data[i] = State.Data.TileGfx.GetPixel(State.SelectedPage, State.SelectedTile, i);
                for (int i = 0; i < 64; i++)
                    State.SetPixel(State.SelectedPage, State.SelectedTile, (7 - i % 8) + (i / 8) * 8, data[i]);
            }
            // Ctrl-F: Vertical Mirror
            if (args.KeyCode == WinKeys.F && args.Control)
            {
                byte[] data = new byte[64];
                for (int i = 0; i < 64; i++)
                    data[i] = State.Data.TileGfx.GetPixel(State.SelectedPage, State.SelectedTile, i);
                for (int i = 0; i < 64; i++)
                    State.SetPixel(State.SelectedPage, State.SelectedTile, (i % 8) + (7 - (i / 8)) * 8, data[i]);
            }
        }
    }
}
