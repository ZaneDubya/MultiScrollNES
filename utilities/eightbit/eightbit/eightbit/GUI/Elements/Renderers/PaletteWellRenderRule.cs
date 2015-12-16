using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Core.GUI.Framework;

namespace eightbit.GUI.Elements.Renderers
{
    class PaletteWellRenderRule : RenderRule
    {
        private Rectangle _area;
        public override Rectangle Area
        {
            get { return _area; }
            set { _area = value; }
        }

        public int Height { get { return _area.Height; } }
        public int Width { get { return _area.Width; } }

        private Texture2D m_Texture;
        private int m_LastIndex;
        private bool m_IsFourColor;
        private PaletteSingleColor m_ParentSingle;
        private PaletteFourColor m_ParentFour;

        public PaletteWellRenderRule(PaletteSingleColor parent)
        {
            m_IsFourColor = false;
            m_LastIndex = int.MaxValue;
            m_ParentSingle = parent;
        }

        public PaletteWellRenderRule(PaletteFourColor parent)
        {
            m_IsFourColor = true;
            m_LastIndex = int.MaxValue;
            m_ParentFour = parent;
        }

        public override void SetSize(int w, int h)
        {
            _area.Width = w;
            _area.Height = h;
        }

        protected override void LoadRenderers()
        {

        }

        private Color m_BorderColor = new Color(23, 24, 26);
        public Color BorderColor
        {
            get { return m_BorderColor; }
            set { m_BorderColor = value; }
        }

        public override void Draw()
        {
            if ((!m_IsFourColor && m_LastIndex != m_ParentSingle.ColorIndex) || (m_IsFourColor && m_LastIndex != m_ParentFour.ColorIndex))
                createTexture();
            RenderManager.SpriteBatch.GUIDrawSprite(m_Texture, Area, Palettized: true);
            Color border = new Color(23, 24, 26);
            RenderManager.SpriteBatch.DrawRectangle(
                new Vector3(Area.X, Area.Y, 0),
                new Vector2(Area.Width, Area.Height),
                m_BorderColor);
        }

        public override void DrawNoClipping() { }

        private void createTexture()
        {
            m_LastIndex = (m_IsFourColor) ? m_ParentFour.ColorIndex : m_ParentSingle.ColorIndex;
            if (m_IsFourColor)
            {
                uint[] pixels = new uint[4];
                byte[] data = System.BitConverter.GetBytes(m_LastIndex);
                for (int i = 0; i < 4; i++)
                    pixels[i] = (uint)data[i];
                m_Texture = Core.Library.CreateTexture(4, 1);
                m_Texture.SetData<uint>(pixels);
            }
            else
            {
                uint[] pixels = new uint[1] { (uint)m_LastIndex } ;
                m_Texture = Core.Library.CreateTexture(1, 1);
                m_Texture.SetData<uint>(pixels);
            }
        }
    }
}
