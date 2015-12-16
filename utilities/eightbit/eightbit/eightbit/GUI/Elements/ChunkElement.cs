using System;
using Core.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Core.GUI.Framework;

namespace eightbit.GUI.Elements
{
    class ChunkElement : WidgetBase<Renderers.ChunkElementRenderRule>
    {
        public int TilesPerRow = 8;
        public int TilesTotal = 64;

        public ChunkElement(int x, int y, int width, int height, WidgetMouseEvent onClick)
        {
            Area = new Rectangle(x, y, width, height);
            RenderRule.SetSize(width, height);
            OnClick += onClick;

            m_Attributes = new byte[TilesTotal];
            m_Tiles = new byte[TilesTotal][];
            m_Textures = new Texture2D[TilesTotal][];
            for (int i = 0; i < TilesTotal; i++)
            {
                m_Tiles[i] = new byte[4];
                m_Textures[i] = new Texture2D[4];
            }
        }

        protected override Renderers.ChunkElementRenderRule BuildRenderRule()
        {
            return new Renderers.ChunkElementRenderRule(this);
        }

        protected override void Attach() { }
        protected internal override void OnUpdate() { }
        protected internal override void OnLayout() { }

        public WidgetMouseEvent OnClick;
        private byte[] m_Attributes;
        private byte[][] m_Tiles;
        private Texture2D[][] m_Textures;

        public bool DrawBorders
        {
            get { return RenderRule.DrawBorders; }
            set { RenderRule.DrawBorders = value; }
        }

        /// <summary>
        /// Sets an entire chunk at once.
        /// </summary>
        /// <param name="chunk"></param>
        /// <param name="tileset"></param>
        public void SetChunk(Data.Chunk chunk, Data.TileSet tileset)
        {
            for (int i = 0; i < 64; i++)
            {
                int tile = chunk[i];
                for (int j = 0; j < 4; j++)
                {
                    Data.TilePageAttribute tile_page_attrib = State.Data.TileSets[State.SelectedTileset].GetSubTile(tile, j);
                    SetTile(i, j, tile_page_attrib.Tile, State.GfxPage(tile_page_attrib.Page).Texture);
                    SetAttribute(i, tile_page_attrib.Attribute);
                }
            }
        }

        /// <summary>
        /// Gets a subtile index and texture.
        /// </summary>
        /// <param name="metatile">The index of the metatile (0 - 63)</param>
        /// <param name="subtile">The index of the subtile (0 - 3)</param>
        /// <returns>Tuple, Item1: subtile index, Item2: texture containing this subtile</returns>
        public Tuple<byte, Texture2D> GetTileIndexAndTexture(int metatile, int subtile)
        {
            if (metatile < 0 || metatile >= TilesTotal)
                return null;
            if (subtile < 0 || subtile >= 4)
                return null;
            return new Tuple<byte, Texture2D>(m_Tiles[metatile][subtile], m_Textures[metatile][subtile]);
        }

        /// <summary>
        /// Sets a tile graphic
        /// </summary>
        /// <param name="metatile">The metatile index (0 - 63)</param>
        /// <param name="subtile">The subtile index within the specified metatile (0 - 3)</param>
        /// <param name="value">The index of the tile graphic (0 - 255)</param>
        /// <param name="texture">A texture of the page containing the tile graphic</param>
        public void SetTile(int metatile, int subtile, byte value, Texture2D texture)
        {
            if (metatile < 0 || metatile >= TilesTotal)
                return;
            if (subtile < 0 || subtile >= 4)
                return;
            m_Tiles[metatile][subtile] = value;
            m_Textures[metatile][subtile] = texture;
        }

        /// <summary>
        /// Gets the palette attribute from the specified metatile.
        /// </summary>
        public byte GetAttribute(int metatile)
        {
            if (metatile < 0 || metatile >= TilesTotal)
                return 0;
            return m_Attributes[metatile];
        }

        /// <summary>
        /// Sets the palette attribute to a metatile.
        /// </summary>
        public void SetAttribute(int metatile, byte value)
        {
            if (metatile < 0 || metatile >= TilesTotal)
                return;
            m_Attributes[metatile] = value;
        }

        private int m_ClickedTile = 0, m_HoverTile = 0;
        private int m_ChunkIndex = -1;
        private int m_ChunkType = -1;

        /// <summary>
        /// The tile or metatile that was last clicked on.
        /// </summary>
        public int ClickedTile
        {
            set { m_ClickedTile = value; }
            get { return m_ClickedTile; }
        }

        /// <summary>
        /// The tile or metatile that the mouse is hovering over.
        /// </summary>
        public int HoverTile
        {
            set { m_HoverTile = value; }
            get { return m_HoverTile; }
        }

        /// <summary>
        /// The index of this chunk - set by owner and then referenced to determine which chunk was clicked on.
        /// </summary>
        public int ChunkIndex
        {
            set { m_ChunkIndex = value; m_IsChunkIndexXY = false; }
            get { return m_ChunkIndex; }
        }

        public Point ChunkIndexXY
        {
            set { m_ChunkIndex = value.X + (value.Y * 1024); m_IsChunkIndexXY = true; }
            get { return new Point(m_ChunkIndex % 1024, m_ChunkIndex / 1024); }
        }

        private bool m_IsChunkIndexXY = false;

        public bool IsChunkIndexXY
        {
            get { return m_IsChunkIndexXY; }
        }

        /// <summary>
        /// The type of this chunk.
        /// </summary>
        public int ChunkType
        {
            set { m_ChunkType = value; }
            get { return m_ChunkType; }
        }

        // ================================================================================
        // Input
        // ================================================================================

        private SelectType m_SelectType = SelectType.ByMetatile;

        public SelectType SelectionType
        {
            get { return m_SelectType; }
            set { m_SelectType = value; }
        }

        protected internal override void ExitHover()
        {
            m_HoverTile = -1;
        }

        protected internal override void MouseMove(InputEventMouse e)
        {
            if (m_SelectType != SelectType.None)
            {
                m_HoverTile = getTileIndexFromPoint(e.Position);
            }

            if (e.Button == MouseButton.Left && getTileIndexFromPoint(e.Position) != m_ClickedTile)
                MouseDown(e);
        }

        protected internal override void MouseDown(InputEventMouse e)
        {
            m_ClickedTile = getTileIndexFromPoint(e.Position);

            if (OnClick != null)
                OnClick(this, e);
        }

        private int getTileIndexFromPoint(Point p)
        {
            if (m_SelectType == SelectType.None)
                return -1;
            else if (m_SelectType == SelectType.ByChunk)
                return 0;
            else
            {
                bool subtile = (m_SelectType == SelectType.ByTile);
                int iTilesPerRow = TilesPerRow * (subtile ? 2 : 1);
                int iTilesTotal = iTilesPerRow * iTilesPerRow;
                int x = (p.X - InputArea.X) / (Area.Width / iTilesPerRow);
                int y = (p.Y - InputArea.Y) / (Area.Height / (iTilesTotal / iTilesPerRow));
                if (x < 0 || x >= 8 || y < 0 || y >= 8)
                    return -1;
                int tile = x + y * iTilesPerRow;
                return tile;
            }
        }

        public enum SelectType
        {
            None,
            ByTile,
            ByMetatile,
            ByChunk
        }
    }
}
