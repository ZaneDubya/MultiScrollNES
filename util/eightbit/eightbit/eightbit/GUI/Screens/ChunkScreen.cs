using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Core.GUI.Content;
using Core.GUI.Framework;
using Core.Input;

namespace eightbit.GUI.Screens
{
    class ChunkScreen : Screen
    {
        // ================================================================================
        // Interface
        // ================================================================================

        public override void OnInit()
        {
            int leftcolumn = 120;

            Gui.AddWidgets(new Widget[] {
                panel_Control = new Elements.Window(4, 4 + 24, leftcolumn, 84),
                panel_ChunksList = new Elements.Window(4, 4 + 28 + 84, leftcolumn, 592 - 28 - 84),
                panel_CurrentChunk = new Elements.Window(leftcolumn + 8, 4 + 24, 800 - leftcolumn - 12, 592 - 24),
            });

            panel_Control.AddWidgets(new Widget[] {
                lblTileSetCount = new Label(8, 14, string.Empty),
                new Button(leftcolumn - 82, 4, 32, "+", click_AddChunk),
                new Button(leftcolumn - 46, 4, 32, "-", click_RemoveChunk),
                cmbTilesets = new ComboBox(4, 40, leftcolumn - 30, string.Empty)
                    { OnSelectionChanged = cmbTilesets_SelectionChanged },
            });

            panel_ChunksList.AddWidgets(new Widget[] {
                lstChunks = new Elements.ListBox<Data.Chunk>(0, 0, panel_ChunksList.InputArea.Width, panel_ChunksList.InputArea.Height)
                { OnSelectionChanged = lstChunks_OnSelectionChanged }
            });

            panel_CurrentChunk.AddWidgets(new Widget[] {
                new Label(14, 12, "Chunk name:"),
                txtChunkName = new SingleLineTextBox(100, 8, 200, 20, txtChunkName_ValueChanged),
            });

            createCurrentChunkPanel();
            refreshControls();
            loadChunk(State.SelectedChunk);
        }

        private Elements.Window panel_Control, panel_ChunksList, panel_CurrentChunk;
        private Label lblTileSetCount;
        private ComboBox cmbTilesets;
        private Elements.ListBox<Data.Chunk> lstChunks;
        private SingleLineTextBox txtChunkName;
        private Elements.TilesetAllInOne aioTileset;
        private Elements.ChunkElement ctlChunk;

        private void refreshControls()
        {
            int count = State.Data.Chunks.Count;
            lblTileSetCount.Value = string.Format("{0}", count);

            if (count == 0 || State.Data.Palettes.Count == 0 || State.Data.TileGfx.PageCount == 0 || State.Data.TileSets.Count == 0)
            {
                cmbTilesets.Visible = false;
                panel_ChunksList.Visible = false;
                panel_CurrentChunk.Visible = false;
            }
            else
            {
                cmbTilesets.Visible = true;
                List<ComboBox.DropDownItem> tilesets = new List<ComboBox.DropDownItem>();
                for (int i = 0; i < State.Data.TileSets.Count; i++)
                    tilesets.Add(new ComboBox.DropDownItem(string.Format("Tileset 0x{0:X2}", i)));
                cmbTilesets.SetDropDownItems(tilesets);
                if (State.SelectedTileset < State.Data.TileSets.Count)
                    cmbTilesets.SelectedIndex = State.SelectedTileset;
                else
                    cmbTilesets.SelectedIndex = State.SelectedTileset = 0;

                panel_ChunksList.Visible = true;
                lstChunks.SetItems(State.Data.Chunks.ToArray());

                panel_CurrentChunk.Visible = true;
            }
        }

        private void createCurrentChunkPanel()
        {
            if (aioTileset == null)
            {
                int y = 41;
                panel_CurrentChunk.AddWidget(aioTileset = new Elements.TilesetAllInOne(6, y, 128, 512, null));
                aioTileset.OnClick += aioTileset_OnClick;

                int tilesize = 32;
                int x = panel_CurrentChunk.InputArea.Width - tilesize * 16 - 6; 
                panel_CurrentChunk.AddWidget(ctlChunk = new Elements.ChunkElement(x, y, tilesize * 16, tilesize * 16, null));
                ctlChunk.OnClick += ctlChunk_OnClick;
                ctlChunk.SelectionType = Elements.ChunkElement.SelectType.ByMetatile;
            }
        }

        private void loadChunkElement()
        {
            Data.Chunk chunk = State.Data.Chunks[State.SelectedChunk];
            if (chunk == null)
                return;

            Data.TileSet tileset = State.Data.TileSets[State.SelectedTileset];
            if (tileset == null)
                return;

            ctlChunk.SetChunk(chunk, tileset);
        }

        private void loadChunkTile(int index, int tile)
        {
            Data.Chunk chunk = State.Data.Chunks[State.SelectedChunk];
            if (chunk == null)
                return;

            Data.TileSet tileset = State.Data.TileSets[State.SelectedTileset];
            if (tileset == null)
                return;

            if (index < 0 || index >= 64)
                return;

            chunk[index] = State.SelectedMetatile;

            for (int j = 0; j < 4; j++)
            {
                Data.TilePageAttribute tile_page_attrib = State.Data.TileSets[State.SelectedTileset].GetSubTile(tile, j);
                ctlChunk.SetTile(index, j, tile_page_attrib.Tile, State.GfxPage(tile_page_attrib.Page).Texture);
                ctlChunk.SetAttribute(index, tile_page_attrib.Attribute);
            }
        }

        // ================================================================================
        // Input
        // ================================================================================

        private void click_AddChunk(Widget widget)
        {
            State.Data.Chunks.AddChunk();
            refreshControls();
            lstChunks.SelectedIndex = State.Data.Chunks.Count - 1;
        }

        private void click_RemoveChunk(Widget widget)
        {
            State.Data.Chunks.RemoveLastChunk();
            refreshControls();
            if (State.SelectedChunk >= State.Data.Chunks.Count)
                lstChunks.SelectedIndex = State.Data.Chunks.Count - 1;
        }

        private void cmbTilesets_SelectionChanged(Widget widget)
        {
            State.SelectedTileset = ((ComboBox)widget).SelectedIndex;

            Data.Chunk chunk = State.Data.Chunks[State.SelectedChunk];
            if (chunk != null)
                chunk.Tileset = State.SelectedTileset;

            Data.TileSet tileset = State.Data.TileSets[State.SelectedTileset];

            if (tileset != null)
            {
                for (int i = 0; i < 4; i++)
                {
                    if (tileset.GetPalette(i) < 0 || tileset.GetPalette(i) >= State.Data.Palettes.Count)
                        tileset.SetPalette(i, 0);
                    byte[] palette = new byte[4];
                    palette[0] = State.Data.Palettes[tileset.GetPalette(0)][0];
                    for (int j = 1; j < 4; j++)
                        palette[j] = State.Data.Palettes[tileset.GetPalette(i)][j];
                    State.Palette.LoadPalette(i, palette);
                }

                for (int i = 0; i < Data.TileSet.TilesPerSet; i++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        Data.TilePageAttribute tile_page_attrib = State.Data.TileSets[State.SelectedTileset].GetSubTile(i, j);
                        aioTileset.SetTile(i, j, tile_page_attrib.Tile, State.GfxPage(tile_page_attrib.Page).Texture);
                        aioTileset.SetAttribute(i, tile_page_attrib.Attribute);
                    }
                }

                loadChunkElement();
            }
        }

        private void lstChunks_OnSelectionChanged(Widget widget)
        {
            State.SelectedChunk = ((Elements.ListBox<Data.Chunk>)widget).SelectedIndex;
            loadChunk(State.SelectedChunk);
        }

        private void txtChunkName_ValueChanged(Widget widget)
        {
            Data.Chunk chunk = State.Data.Chunks[State.SelectedChunk];
            if (chunk != null)
                chunk.Name = ((SingleLineTextBox)txtChunkName).Value;
        }

        private void aioTileset_OnClick(Widget widget)
        {
            State.SelectedMetatile = ((Elements.TilesetAllInOne)widget).SelectedTile;
        }

        private void ctlChunk_OnClick(Widget widget, InputEventMouse e)
        {
            if (e.Button == MouseButton.Left)
            {
                int tile = ((Elements.ChunkElement)widget).ClickedTile;
                loadChunkTile(tile, State.SelectedMetatile);
            }
            else if (e.Button == MouseButton.Right)
            {
                int index = ((Elements.ChunkElement)widget).ClickedTile;
                int tile = State.Data.Chunks[State.SelectedChunk][index];
                State.SelectedMetatile = aioTileset.SelectedTile = tile;
            }
        }

        // ================================================================================
        // Data manipulation
        // ================================================================================

        private void loadChunk(int index)
        {
            if (index >= State.Data.Chunks.Count)
                index = State.Data.Chunks.Count - 1;
            lstChunks.SetSelectionIndexWithoutOnSelectionChanged(index);
            if (index < 0)
                return;
            if (State.Data.Palettes.Count == 0 || State.Data.TileGfx.PageCount == 0 || State.Data.TileSets.Count == 0)
                return;

            txtChunkName.Value = State.Data.Chunks[index].Name;
            cmbTilesets.SelectedIndex = State.Data.Chunks[index].Tileset;
            loadChunkElement();
        }

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
    }
}
