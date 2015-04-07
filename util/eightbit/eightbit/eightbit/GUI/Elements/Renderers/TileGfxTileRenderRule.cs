using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Core.GUI.Framework;

namespace eightbit.GUI.Elements.Renderers
{
    class TileGfxTileRenderRule : RenderRule
    {
        private Rectangle _area;
        public override Rectangle Area
        {
            get { return _area; }
            set { _area = value; }
        }

        public int Height { get { return _area.Height; } }
        public int Width { get { return _area.Width; } }

        private TileGfxTile m_Parent;
        private int m_TileIndex;
        public int TileIndex
        {
            set { m_TileIndex = value; }
        }

        private Texture2D m_Texture;
        public Texture2D PageTexture
        {
            set { m_Texture = value; }
        }

        public TileGfxTileRenderRule(TileGfxTile parent, int tile_index)
        {
            m_Parent = parent;
            m_TileIndex = tile_index;
        }

        public override void SetSize(int w, int h)
        {
            _area.Width = w;
            _area.Height = h;
        }

        protected override void LoadRenderers()
        {

        }

        public override void Draw()
        {
            int x = (m_TileIndex % 16) * 8;
            int y = (m_TileIndex / 16) * 8;
            RenderManager.SpriteBatch.GUIDrawSprite(m_Texture, Area, new Rectangle(x, y, 8, 8), Palettized: true);

            if (m_Parent.IsHover)
            {
                RenderManager.SpriteBatch.DrawRectangle(new Vector3(Area.X - 1, Area.Y - 1, 0), new Vector2(Area.Width + 2, Area.Height + 2), Color.Gray);
            }
        }

        public override void DrawNoClipping() { }
    }
}
