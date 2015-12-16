using System;
using Core.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Core.GUI.Framework;

namespace eightbit.GUI.Elements
{
    class MetaTile : WidgetBase<Renderers.MetaTileRenderRule>
    {
        public MetaTile(int index, int x, int y, int width, int height, WidgetMouseEvent onClick, int tiles_width = 2, int tiles_height = 2)
        {
            m_TilesWidth = tiles_width;
            m_TilesHeight = tiles_height;

            int tilecount = tiles_width * tiles_height;

            m_Tiles = new byte[tilecount];
            m_Textures = new Texture2D[tilecount];
            m_FlipH = new bool[tilecount];
            m_FlipV = new bool[tilecount];

            m_TileIndex = index;
            Area = new Rectangle(x, y, width, height);
            RenderRule.SetSize(width, height);
            m_OnClick += onClick;
        }

        protected override Renderers.MetaTileRenderRule BuildRenderRule()
        {
            return new Renderers.MetaTileRenderRule(this);
        }

        protected override void Attach() { }
        protected internal override void OnUpdate() { }
        protected internal override void OnLayout() { }

        internal WidgetMouseEvent m_OnClick;

        private int m_TileIndex = 0;
        private byte m_Attribute = 0;

        private int m_TilesWidth, m_TilesHeight;
        public int TilesWidth { get { return m_TilesWidth; } }
        public int TilesHeight { get { return m_TilesHeight; } }

        private byte[] m_Tiles;
        private Texture2D[] m_Textures;
        private bool[] m_FlipH, m_FlipV;

        private bool m_DrawFlags = false;
        public bool DrawFlags
        {
            get { return m_DrawFlags; }
            set { m_DrawFlags = value; }
        }

        private byte m_Flags;
        public byte Flags
        {
            get { return m_Flags; }
            set { m_Flags = value; }
        }

        public Tuple<byte, Texture2D, bool, bool> GetTile(int index)
        {
            if (index < 0 || index >= m_TilesWidth * m_TilesHeight)
                return null;
            return new Tuple<byte, Texture2D, bool, bool>(m_Tiles[index], m_Textures[index], m_FlipH[index], m_FlipV[index]);
        }

        public void SetTile(int index, byte value, Texture2D texture)
        {
            if (index < 0 || index >= m_TilesWidth * m_TilesHeight)
                return;
            m_Tiles[index] = value;
            m_Textures[index] = texture;
        }

        public void SetTileFlip(int index, bool horizontal, bool vertical)
        {
            m_FlipH[index] = horizontal;
            m_FlipV[index] = vertical;
        }

        public byte Attribute
        {
            get { return m_Attribute; }
            set { m_Attribute = value; }
        }

        public int Index
        {
            get { return m_TileIndex; }
            set { m_TileIndex = value; }
        }

        private int m_ClickedTile = 0;
        public int ClickedTile
        {
            get { return m_ClickedTile; }
        }

        private int m_ClickedFlag = 0;
        public int ClickedFlag
        {
            get { return m_ClickedFlag; }
        }

        protected internal override void MouseDown(InputEventMouse e)
        {
            int x = (e.Position.X - InputArea.X) / (Area.Width / m_TilesWidth);
            int y = (e.Position.Y - InputArea.Y) / (Area.Height / m_TilesHeight);
            m_ClickedTile = x + y * m_TilesWidth;
            x = (e.Position.X - InputArea.X) / (Area.Width / 3);
            y = (e.Position.Y - InputArea.Y) / (Area.Height / 3);
            m_ClickedFlag = x + y * 3;

            if (m_OnClick != null)
                m_OnClick(this, e);
        }
    }
}
