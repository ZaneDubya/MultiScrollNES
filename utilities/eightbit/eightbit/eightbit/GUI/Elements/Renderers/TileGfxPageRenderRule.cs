using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Core.GUI.Framework;

namespace eightbit.GUI.Elements.Renderers
{
    class TileGfxPageRenderRule : RenderRule
    {
        private Rectangle _area;
        public override Rectangle Area
        {
            get { return _area; }
            set { _area = value; }
        }

        public int Height { get { return _area.Height; } }
        public int Width { get { return _area.Width; } }

        private TileGfxPage m_Parent;
        private Texture2D m_Texture;
        public Texture2D Texture
        {
            set { m_Texture = value; }
        }

        private int m_Attribute = 0;
        public int Attribute
        {
            set
            {
                if (value < 0 || value >= 4)
                    return;
                m_Attribute = value;
            }
        }

        public TileGfxPageRenderRule(TileGfxPage parent)
        {
            m_Parent = parent;
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
            RenderManager.SpriteBatch.GUIDrawSprite(m_Texture, Area, new Rectangle(0, 0, 128, 128), Palettized: true, Palette: m_Attribute);
            if (m_Parent.HasSelection)
            {
                Color border = new Color(255, 255, 255);
                int w = Area.Width / 16, h = Area.Height / 16;
                RenderManager.SpriteBatch.DrawRectangle(
                    new Vector3(Area.X + (m_Parent.SelectedTile % 16) * w - 1, Area.Y + (m_Parent.SelectedTile / 16) * h - 1, 0),
                    new Vector2(18, 18),
                    border);
            }
        }

        public override void DrawNoClipping() { }
    }
}
