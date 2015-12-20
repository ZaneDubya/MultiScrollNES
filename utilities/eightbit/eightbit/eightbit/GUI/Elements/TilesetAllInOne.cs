using Core.GUI.Framework;
using Core.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace eightbit.GUI.Elements
{
    class TilesetAllInOne : WidgetBase<Renderers.TilesetAIORenderRule>
    {
        public int TilesPerRow = 8;

        public TilesetAllInOne(int x, int y, int width, int height, WidgetEvent onClick)
        {
            Area = new Rectangle(x, y, width, height);
            RenderRule.SetSize(width, height);
            OnClick += onClick;

            m_Attributes = new byte[Data.TileSet.TilesPerSet];
            m_Tiles = new byte[Data.TileSet.TilesPerSet][];
            m_Textures = new Texture2D[Data.TileSet.TilesPerSet][];
            for (int i = 0; i < Data.TileSet.TilesPerSet; i++)
            {
                m_Tiles[i] = new byte[4];
                m_Textures[i] = new Texture2D[4];
            }
        }

        protected override Renderers.TilesetAIORenderRule BuildRenderRule()
        {
            return new Renderers.TilesetAIORenderRule(this);
        }

        protected override void Attach() { }
        protected internal override void OnUpdate() { }
        protected internal override void OnLayout() { }

        public WidgetEvent OnClick;
        private byte[] m_Attributes;
        private byte[][] m_Tiles;
        private Texture2D[][] m_Textures;

        public Tuple<byte, Texture2D> GetTile(int metatile, int subtile)
        {
            if (metatile < 0 || metatile >= Data.TileSet.TilesPerSet)
                return null;
            if (subtile < 0 || subtile >= 4)
                return null;
            return new Tuple<byte, Texture2D>(m_Tiles[metatile][subtile], m_Textures[metatile][subtile]);
        }

        public void SetTile(int metatile, int subtile, byte value, Texture2D texture)
        {
            if (metatile < 0 || metatile >= Data.TileSet.TilesPerSet)
                return;
            if (subtile < 0 || subtile >= 4)
                return;
            m_Tiles[metatile][subtile] = value;
            m_Textures[metatile][subtile] = texture;
        }

        public byte GetAttribute(int metatile)
        {
            if (metatile < 0 || metatile >= Data.TileSet.TilesPerSet)
                return 0;
            return m_Attributes[metatile];
        }

        public void SetAttribute(int metatile, byte value)
        {
            if (metatile < 0 || metatile >= Data.TileSet.TilesPerSet)
                return;
            m_Attributes[metatile] = value;
        }

        private int m_ClickedTile = 0;
        public int SelectedTile
        {
            set { m_ClickedTile = value; }
            get { return m_ClickedTile; }
        }

        protected internal override void MouseDown(InputEventMouse e)
        {
            int x = (e.Position.X - InputArea.X) / (Area.Width / TilesPerRow);
            int y = (e.Position.Y - InputArea.Y) / (Area.Height / (256 / TilesPerRow));
            int tile = x + y * TilesPerRow;
            if (tile == 255)
                return;

            if (tile != m_ClickedTile)
            {
                m_ClickedTile = tile;
                if (OnClick != null)
                    OnClick(this);
            }
        }
    }
}
