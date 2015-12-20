using Core.GUI.Content;
using Core.GUI.Framework;
using Core.Input;
using eightbit.Data.TileSetData;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace eightbit.GUI.Screens
{
    class MapScreen : Screen
    {
        Elements.Window panelMaps, panelControls, panelCurrentMap;
        Elements.Menu mnuControls;
        Label lblMapCount, lblSelectedChunk;
        ComboBox cmbMaps, cmbTilesets, cmbMapSize;
        Elements.MiniMap mmpMini;
        Elements.ChunkElement chkSelectedChunk;
        Elements.ListBox<Data.Chunk> lstChunks;
        ScrollBars scrCurrentMap;

        public override void OnInit()
        {
            State.MapScreen_Scroll = new Point(0, 0);

            createPanels();
            createControlsPanel();
            refreshMapsPanel();

            int temp = State.SelectedMap;
            State.SelectedMap = -1;
            cmbMaps.SelectedIndex = (temp >= State.Data.Maps.Count) ? 0 : temp;

            EditMode = EditModeEnum.MiniMap;
        }

        // ================================================================================
        // Control management
        // ================================================================================

        /// <summary>
        /// Creates the panels and adds all the widgets for the 'Maps' panel.
        /// </summary>
        private void createPanels()
        {
            int leftcolumn = 192 + 8;
            int maps_height = 84;
            Gui.AddWidgets(new Widget[] {
                panelMaps = new Elements.Window(4, 4 + 24, leftcolumn, maps_height),
                panelControls = new Elements.Window(4, 4 + 28 + maps_height, leftcolumn, 592 - 28 - maps_height),
                panelCurrentMap = new Elements.Window(leftcolumn + 8, 4 + 24, 800 - leftcolumn - 12, 592 - 24),
            });

            panelMaps.AddWidgets(new Widget[] {
                lblMapCount = new Label(8, 14, string.Empty),
                new Button(leftcolumn - 82, 4, 32, "+", click_AddMap),
                new Button(leftcolumn - 46, 4, 32, "-", click_RemoveMap),
                cmbMaps = new ComboBox(4, 40, leftcolumn - 30, string.Empty)
                    { OnSelectionChanged = cmbMaps_SelectionChanged },
            });
        }

        /// <summary>
        /// Creates all the widgets for the 'Controls' panel.
        /// </summary>
        private void createControlsPanel()
        {
            int y_aftermenu = 32;

            panelControls.RemoveAllWidgets();
            panelControls.AddWidget(mnuControls = new Elements.Menu(new List<Elements.MenuElement>()
                {
                    new Elements.MenuElement("Map", action: mnuMap_Click),
                    new Elements.MenuElement("Chunks", action: mnuChunks_Click),
                    new Elements.MenuElement("Actors", action: mnuActors_Click),
                    new Elements.MenuElement("Eggs", action: mnuEggs_Click)
                }));
            panelControls.AddWidget(mmpMini = new Elements.MiniMap((panelControls.InputArea.Width - 128) / 2, y_aftermenu, 128, 128, mmpMini_Click));
            
            // Tilesets combo control
            panelControls.AddWidget(
                cmbTilesets = new ComboBox(4, mmpMini.Area.Height + 8 + y_aftermenu, panelControls.Area.Width - 30, string.Empty) { OnSelectionChanged = cmbTilesets_SelectionChanged });
            List<ComboBox.DropDownItem> tilesets = new List<ComboBox.DropDownItem>();
            for (int i = 0; i < State.Data.TileSets.Count; i++)
                tilesets.Add(new ComboBox.DropDownItem(string.Format("Tileset 0x{0:X2}", i)));
            cmbTilesets.SetDropDownItems(tilesets);

            // Map sizes combo control
            panelControls.AddWidget(
                cmbMapSize = new ComboBox(4, cmbTilesets.Area.Bottom + 8, panelControls.Area.Width - 30, string.Empty) { OnSelectionChanged = cmbMapSize_SelectionChanged });
            List<ComboBox.DropDownItem> map_sizes = new List<ComboBox.DropDownItem>();
            map_sizes.Add(new ComboBox.DropDownItem("Empty"));
            map_sizes.Add(new ComboBox.DropDownItem("16 x 16"));
            map_sizes.Add(new ComboBox.DropDownItem("32 x 32"));
            cmbMapSize.SetDropDownItems(map_sizes);

            panelControls.AddWidgets(new Widget[] {
                chkSelectedChunk = new Elements.ChunkElement((panelControls.InputArea.Width - 128) / 2, y_aftermenu + 18, 128, 128, null)
                { SelectionType = Elements.ChunkElement.SelectType.None },
                lblSelectedChunk = new Label(chkSelectedChunk.X, y_aftermenu - 2, "Selected chunk:"),
                lstChunks = new Elements.ListBox<Data.Chunk>(8, chkSelectedChunk.Area.Bottom + 8, 
                    panelControls.InputArea.Width - 16, panelControls.InputArea.Height - chkSelectedChunk.Area.Bottom - 16)
                    { OnSelectionChanged = lstChunks_OnSelectionChanged }
            });

            lstChunks.SetItems(State.Data.Chunks.ToArray());
            lstChunks.SelectedIndex = 0;
        }

        /// <summary>
        /// Refreshes the control panels, making a subset of these active based on the state of EditMode.
        /// </summary>
        private void refreshControlsPanelByEditMode()
        {
            if (IsMapOpen)
            {
                mmpMini.Visible = (EditMode == EditModeEnum.MiniMap);
                cmbTilesets.Visible = (EditMode == EditModeEnum.MiniMap);
                cmbMaps.Visible = (EditMode == EditModeEnum.MiniMap);
                cmbMapSize.Visible = (EditMode == EditModeEnum.MiniMap);

                lblSelectedChunk.Visible = (EditMode == EditModeEnum.Chunks);
                chkSelectedChunk.Visible = (EditMode == EditModeEnum.Chunks);
                lstChunks.Visible = (EditMode == EditModeEnum.Chunks);

                if (m_Chunks != null)
                {
                    Elements.ChunkElement.SelectType hoverstyle;
                    switch (EditMode)
                    {
                        case EditModeEnum.MiniMap:
                            hoverstyle = Elements.ChunkElement.SelectType.None;
                            break;
                        case EditModeEnum.Chunks:
                            hoverstyle = Elements.ChunkElement.SelectType.ByChunk;
                            break;
                        default:
                            hoverstyle = Elements.ChunkElement.SelectType.None;
                            break;
                    }

                    foreach (MapChunk chunk in m_Chunks)
                        chunk.Chunk.SelectionType = hoverstyle;
                }
            }
        }

        /// <summary>
        /// Refreshes the widgest in the 'Maps' panel and makes other panels inactive if there are no maps loaded.
        /// </summary>
        private void refreshMapsPanel()
        {
            int count = State.Data.Maps.Count;

            lblMapCount.Value = string.Format("{0} Maps", count);

            if (count == 0)
            {
                cmbMaps.Visible = false;
                panelControls.Visible = false;
                panelCurrentMap.Visible = false;
            }
            else
            {
                cmbMaps.Visible = true;
                panelControls.Visible = true;
                panelCurrentMap.Visible = true;
                List<ComboBox.DropDownItem> maps = new List<ComboBox.DropDownItem>();
                for (int i = 0; i < count; i++)
                    maps.Add(new ComboBox.DropDownItem(string.Format("Map 0x{0:X2}", i)));
                cmbMaps.SetDropDownItems(maps);
                if (State.SelectedMap < count)
                    cmbMaps.SelectedIndex = State.SelectedMap;
                else
                    cmbMaps.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// Sets up the 'Current map' Panel.
        /// </summary>
        private void refreshCurrentMapPanel()
        {
            if (scrCurrentMap == null)
            {
                panelCurrentMap.AddWidget(scrCurrentMap = new ScrollBars());
                State.MapScreen_WindowArea = new Point(panelCurrentMap.InputArea.Width, panelCurrentMap.InputArea.Width);
            }

            Data.Map map = State.Data.Maps[State.SelectedMap];
            Point mapArea = new Point(map.WidthInSuperChunks * 256, map.HeightInSuperChunks * 256);
            scrCurrentMap.OverrideScrollArea(new Rectangle(0, 0, mapArea.X, mapArea.Y));
            State.MapScreen_MapArea = mapArea;

            if (State.MapScreen_Scroll.X >= mapArea.X - panelCurrentMap.InputArea.Width)
                State.MapScreen_Scroll.X = mapArea.X - panelCurrentMap.InputArea.Width;
            if (State.MapScreen_Scroll.Y >= mapArea.Y - panelCurrentMap.InputArea.Height)
                State.MapScreen_Scroll.Y = mapArea.Y - panelCurrentMap.InputArea.Height;

            initializeMapAndLoadAllChunks(1);
            mmpMini.RefreshEntireMap(State.Data.Maps[State.SelectedMap]);
        }

        // ================================================================================
        // Data handling
        // ================================================================================

        private enum EditModeEnum
        {
            MiniMap,
            Chunks,
            Actors,
            Eggs
        }

        private EditModeEnum m_EditMode = EditModeEnum.MiniMap;
        /// <summary>
        /// What the map is currently editing. Setting this value refreshes the visible controls in the Controls panel.
        /// </summary>
        private EditModeEnum EditMode
        {
            get { return m_EditMode; }
            set
            {
                m_EditMode = value;
                refreshControlsPanelByEditMode();
            }
        }

        private bool IsMapOpen
        {
            get { return State.Data.Maps[State.SelectedMap] != null; }
        }

        private MapChunk[] m_Chunks;
        private int m_ChunkW, m_ChunkH, m_ChunkScale;

        /// <summary>
        /// Creates the Chunks controls in the current map panel, centered at a camera point, with a variable scale.
        /// </summary>
        /// <param name="camera">The point around which the map will be initially centered.</param>
        /// <param name="scale">The integer scale at which the map will be drawn, from 1x to 4x.</param>
        private void initializeMapAndLoadAllChunks(int scale)
        {
            if (scale < 0) scale = 1;
            if (scale > 4) scale = 4;
            m_ChunkScale = scale;

            // determine how many chunks will be visible on screen.
            Rectangle render_area = scrCurrentMap.InputArea;
            const int chunksize = 8 * 16;
            m_ChunkW = render_area.Width / (chunksize * m_ChunkScale) + 2;
            m_ChunkH = render_area.Height / (chunksize * m_ChunkScale) + 2;

            // create the m_Chunks array and add the chunks elements to the current map panel.
            scrCurrentMap.RemoveAllWidgets();
            m_Chunks = new MapChunk[m_ChunkW * m_ChunkH];
            for (int i = 0; i < m_ChunkW * m_ChunkH; i++)
            {
                m_Chunks[i] = new MapChunk(
                    new Elements.ChunkElement(0, 0, chunksize * m_ChunkScale, chunksize * m_ChunkScale, null)
                    { DrawBorders = false, OnClick = mapChunks_Click, ChunkIndexXY = new Point(-1, -1) });
                scrCurrentMap.AddWidget(m_Chunks[i].Chunk);
            }

            // scroll the map, automatically loading chunks with necessary graphic information.
            scrollAllChunks();
        }

        /// <summary>
        /// Loads data into a chunk in the 'Current Map' panel.
        /// </summary>
        /// <param name="p">The x and y indexes of the chunk to load.</param>
        /// <param name="index">The index of the chunk in the Current Map panel.</param>
        private void loadChunk(Point p, int ctl_index)
        {
            Data.Map map = State.Data.Maps[State.SelectedMap];
            MapChunk ctlChunk = m_Chunks[ctl_index];

            int chunk_index = p.X + p.Y * State.Data.Maps[State.SelectedMap].WidthInChunks;
            int chunk_type = map.GetSuperChunk(p.X / 2, p.Y / 2).Chunks[p.X % 2 + (p.Y % 2) * 2];

                ctlChunk.Chunk.ChunkIndexXY = p;
            
            if (ctlChunk.Chunk.ChunkType != chunk_type)
            {

                if (p.X < 0 || p.X >= map.WidthInChunks || p.Y < 0 || p.Y >= map.HeightInChunks)
                {
                    return;
                }
                else
                {
                    ctlChunk.Chunk.ChunkType = chunk_type;
                    Data.Chunk chunk = State.Data.Chunks[chunk_type];
                    for (int i = 0; i < 64; i++)
                    {
                        int tile = chunk[i];
                        for (int j = 0; j < 4; j++)
                        {
                            TilePageAttribute tile_page_attrib = State.Data.TileSets[State.SelectedTileset].GetSubTile(tile, j);
                            ctlChunk.Chunk.SetTile(i, j, tile_page_attrib.Tile, State.GfxPage(tile_page_attrib.Page).Texture);
                            ctlChunk.Chunk.SetAttribute(i, tile_page_attrib.Attribute);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Move all chunks based on a passed camera point.
        /// </summary>
        private void scrollAllChunks()
        {
            doDelegateWithAllChunks(scrollChunk);
        }

        /// <summary>
        /// Move a single chunk based on the camera point.
        /// </summary>
        /// <param name="p">The x and y indexes of the chunk to move</param>
        /// <param name="index">The index of the chunk in m_Chunks</param>
        private void scrollChunk(Point p, int index)
        {
            MapChunk ctlChunk = m_Chunks[index];
            if (p.X < 0 || p.X >= State.Data.Maps[State.SelectedMap].WidthInChunks ||
                p.Y < 0 || p.Y >= State.Data.Maps[State.SelectedMap].HeightInChunks)
            {
                ctlChunk.Chunk.Visible = false;
            }
            else
            {
                ctlChunk.Chunk.Visible = true;
                Point centerChunk = FirstVisibleChunk;
                
                const int chunksize = 8 * 16;
                ctlChunk.Chunk.X = centerChunk.X * chunksize - State.MapScreen_Scroll.X + (p.X - centerChunk.X) * chunksize;
                ctlChunk.Chunk.Y = centerChunk.Y * chunksize - State.MapScreen_Scroll.Y + (p.Y - centerChunk.Y) * chunksize;

                if (ctlChunk.Chunk.ChunkIndexXY != p)
                    loadChunk(p, index);
            }
        }

        /// <summary>
        /// Do a delegate action on all the chunks.
        /// </summary>
        private void doDelegateWithAllChunks(AllChunksAction action)
        {
            Point firstChunk = FirstVisibleChunk;
            for (int y = 0; y < m_ChunkH; y++)
            {
                for (int x = 0; x < m_ChunkW; x++)
                {
                    int ix = x + (firstChunk.X / m_ChunkW) * m_ChunkW;
                    if (x < (firstChunk.X % m_ChunkW))
                        ix += m_ChunkW;
                    int iy = y + (firstChunk.Y / m_ChunkH) * m_ChunkH;
                    if (y < (firstChunk.Y % m_ChunkH))
                        iy += m_ChunkH;
                    action(new Point(ix, iy), x + y * m_ChunkW);
                }
            }
        }

        private Point FirstVisibleChunk
        {
            get
            {
                const int chunksize = 8 * 16;
                Rectangle render_area = scrCurrentMap.InputArea;
                return new Point((State.MapScreen_Scroll.X) / chunksize, (State.MapScreen_Scroll.Y) / chunksize);
            }
        }

        /// <summary>
        /// Select a chunk with the index specified. This changes the State's SelectedChunk and loads the specified chunk into the Selected Chunk widget.
        /// </summary>
        private void selectChunk(int index)
        {
            if (index >= State.Data.Chunks.Count)
                index = State.Data.Chunks.Count - 1;

            State.SelectedChunk = index;
            lstChunks.SetSelectionIndexWithoutOnSelectionChanged(index);

            if (index < 0)
                return;
            if (State.Data.Palettes.Count == 0 || State.Data.TileGfx.PageCount == 0 || State.Data.TileSets.Count == 0)
                return;

            refreshSelectedChunk();
        }

        /// <summary>
        /// Loads data into the 'Selected Chunk' widget in the 'Controls' panel.
        /// </summary>
        private void refreshSelectedChunk()
        {
            Data.Chunk chunk = State.Data.Chunks[State.SelectedChunk];
            if (chunk == null)
                return;

            Data.TileSet tileset = State.Data.TileSets[State.SelectedTileset];
            if (tileset == null)
                return;

            chkSelectedChunk.SetChunk(chunk, tileset);
        }

        // ================================================================================
        // Input
        // ================================================================================

        private void click_AddMap(Widget widget)
        {
            State.Data.Maps.AddMap();
            refreshMapsPanel();
            cmbMaps.SelectedIndex = State.Data.Maps.Count - 1;
            refreshMapsPanel();
        }

        private void click_RemoveMap(Widget widget)
        {
            State.Data.Maps.RemoveLastMap();
            refreshMapsPanel();
        }

        private void cmbMaps_SelectionChanged(Widget widget)
        {
            int selected = ((ComboBox)widget).SelectedIndex;
            if (State.SelectedMap != selected)
            {
                State.SelectedMap = selected;
                EditMode = EditModeEnum.MiniMap;
                cmbMapSize.SelectedIndex = (int)(State.Data.Maps[State.SelectedMap].Size);
                if (State.Data.Maps[State.SelectedMap].Tileset >= State.Data.TileSets.Count)
                    cmbTilesets.SelectedIndex = 0;
                else
                    cmbTilesets.SelectedIndex = State.Data.Maps[State.SelectedMap].Tileset;
            }
        }

        private void cmbTilesets_SelectionChanged(Widget widget)
        {
            int index = ((ComboBox)widget).SelectedIndex;
            if (index != State.SelectedTileset || index != State.Data.Maps[State.SelectedMap].Tileset)
            {
                State.SelectedTileset = State.Data.Maps[State.SelectedMap].Tileset = index;
                refreshCurrentMapPanel();
            }

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
            }
        }

        private void cmbMapSize_SelectionChanged(Widget widget)
        {
            State.Data.Maps[State.SelectedMap].Size = (Data.MapSize)(cmbMapSize.SelectedIndex);
            refreshCurrentMapPanel();
            refreshControlsPanelByEditMode();
        }

        private void lstChunks_OnSelectionChanged(Widget widget)
        {
            int index = ((Elements.ListBox<Data.Chunk>)widget).SelectedIndex;
            selectChunk(index);
        }

        private void mapChunks_Click(Widget widget, InputEventMouse e)
        {
            Data.Map map = State.Data.Maps[State.SelectedMap];
            Elements.ChunkElement chkelem = ((Elements.ChunkElement)widget);
            int index = chkelem.ChunkIndex;
            int x = chkelem.ChunkIndexXY.X;
            int y = chkelem.ChunkIndexXY.Y;

            switch (EditMode)
            {
                case EditModeEnum.MiniMap:
                    break;
                case EditModeEnum.Chunks:
                    map.GetSuperChunk(x / 2, y / 2).Chunks[x % 2 + (y % 2) * 2] = State.SelectedChunk;
                    loadChunk(new Point(x, y), x % m_ChunkW + (y % m_ChunkH) * m_ChunkW);
                    break;
                case EditModeEnum.Actors:
                case EditModeEnum.Eggs:
                    break;
            }
        }

        private void mmpMini_Click(Widget widget)
        {
            Point clicked = ((Elements.MiniMap)widget).ClickedPoint;
            clicked.X = clicked.X * (State.MapScreen_MapArea.X / mmpMini.Area.Width) - (State.MapScreen_WindowArea.X / 2);
            clicked.Y = clicked.Y * (State.MapScreen_MapArea.Y / mmpMini.Area.Height) - (State.MapScreen_WindowArea.Y / 2);
            scrCurrentMap.ScrollPosition = clicked;
        }

        private void mnuMap_Click(Widget widget)
        {
            EditMode = EditModeEnum.MiniMap;
        }

        private void mnuChunks_Click(Widget widget)
        {
            EditMode = EditModeEnum.Chunks;
        }

        private void mnuActors_Click(Widget widget)
        {
            EditMode = EditModeEnum.Actors;
        }

        private void mnuEggs_Click(Widget widget)
        {
            EditMode = EditModeEnum.Eggs;
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
            if (scrCurrentMap != null)
            {
                if (scrCurrentMap.ScrollPosition != State.MapScreen_Scroll)
                {
                    State.MapScreen_Scroll = scrCurrentMap.ScrollPosition;
                    scrollAllChunks();
                    scrCurrentMap.Layout();
                }
            }
            Gui.Update();
        }

        public override void Draw()
        {
            Gui.Draw();
        }

        // ================================================================================
        // Data support
        // ================================================================================

        private delegate void AllChunksAction(Point p, int index);

        private class MapChunk
        {
            public Elements.ChunkElement Chunk;

            public MapChunk(Elements.ChunkElement chunk)
            {
                Chunk = chunk;
            }

            public override string ToString()
            {
                return string.Format("<{0},{1}>", Chunk.ChunkIndexXY.X, Chunk.ChunkIndexXY.Y);
            }
        }
    }
}
