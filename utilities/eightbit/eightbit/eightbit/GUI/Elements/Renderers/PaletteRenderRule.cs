using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Core.GUI.Framework;

namespace eightbit.GUI.Elements.Renderers
{
    class PaletteRenderRule : RenderRule
    {
        private Rectangle _area;
        public override Rectangle Area
        {
            get { return _area; }
            set { _area = value; }
        }

        public int Height { get { return _area.Height; } }
        public int Width { get { return _area.Width; } }

        private int _tilesAcross = 0;

        private int m_Selected = 0;
        public void Select(Point p)
        {
            int x = p.X, y = p.Y;
            x = (int)((x - Area.X) / ((float)Area.Width / _tilesAcross));
            y = (int)((y - Area.Y) / ((float)Area.Height / (64 / _tilesAcross)));
            m_Selected = y * _tilesAcross + x;
        }
        public int Selected
        {
            get
            {
                switch (Sort)
                {
                    case PaletteNES.SortMode.Default:
                        return m_Selected;
                    case PaletteNES.SortMode.ColorByRow:
                        return ((m_Selected & 0x03) << 4) + ((m_Selected & 0x3C) >> 2);
                    default:
                        return m_Selected;
                }
            }
        }

        public PaletteRenderRule(int tilesAcross)
        {
            _tilesAcross = tilesAcross;
        }

        private Core.Graphics.VertexPositionTextureHueExtra[] m_V;
        private Texture2D m_Texture;

        public override void SetSize(int w, int h)
        {
            _area.Width = w;
            _area.Height = h;
            setupTiles();
        }

        protected override void LoadRenderers()
        {
            createTexture();
        }

        public override void Draw()
        {
            RenderManager.SpriteBatch.DrawTriangleList(m_Texture, new Vector3(Area.X, Area.Y, 0), m_V);
            drawSelection(m_Selected);
        }

        private void drawSelection(int select)
        {
            float x = (float)Area.Width / (float)_tilesAcross;
            float y = (float)Area.Height / (float)(64 / _tilesAcross);
            RenderManager.SpriteBatch.DrawRectangle(new Vector3(Area.X + (x * (select % _tilesAcross)) - 1, Area.Y + (y * (int)(select / _tilesAcross)) - 1, 0),
                new Vector2(x + 2, y + 2),
                Color.White);
            RenderManager.SpriteBatch.DrawRectangle(new Vector3(Area.X + (x * (select % _tilesAcross)), Area.Y + (y * (int)(select / _tilesAcross)), 0),
                new Vector2(x, y),
                Color.Black);
        }

        public override void DrawNoClipping() { }

        private void setupTiles()
        {
            m_V = new Core.Graphics.VertexPositionTextureHueExtra[64 * 4];
            float x = (float)Area.Width / (float)_tilesAcross;
            float y = (float)Area.Height / (float)(64 / _tilesAcross);
            int h = (64 / _tilesAcross);
            int i = 0, j = 0;
            Vector4 t = new Vector4(0, 0, 0, 1);
            for (int iy = 0; iy < h; iy++)
                for (int ix = 0; ix < _tilesAcross; ix++)
                {
                    t.X = (j / 64.0f);
                    t.W = ((float)(j++ + 1) / 64.0f);
                    m_V[i++] = new Core.Graphics.VertexPositionTextureHueExtra(new Vector3(ix * x, iy * y, 0), new Vector2(t.X, t.Y), Color.White, new Vector2());
                    m_V[i++] = new Core.Graphics.VertexPositionTextureHueExtra(new Vector3(ix * x + x, iy * y, 0), new Vector2(t.W, t.Y), Color.White, new Vector2());
                    m_V[i++] = new Core.Graphics.VertexPositionTextureHueExtra(new Vector3(ix * x, iy * y + y, 0), new Vector2(t.X, t.Z), Color.White, new Vector2());
                    m_V[i++] = new Core.Graphics.VertexPositionTextureHueExtra(new Vector3(ix * x + x, iy * y + y, 0), new Vector2(t.W, t.Z), Color.White, new Vector2());
                }
        }

        private void createTexture()
        {
            uint[] pixels = new uint[64];
            m_Texture = Core.Library.CreateTexture(64, 1);
            switch (Sort)
            {
                case PaletteNES.SortMode.Default:
                    for (uint i = 0; i < 64; i++)
                        pixels[i] = i;
                break;
                case PaletteNES.SortMode.ColorByRow:
                    for (uint i = 0; i < 64; i++)
                        pixels[i] = ((i & 0x03) << 4) + ((i & 0x3C) >> 2);
                break;
            }
            m_Texture.SetData<uint>(pixels);
        }

        private int transposeSelection(PaletteNES.SortMode oldSort, PaletteNES.SortMode newSort)
        {
            int sel = 0;
            switch (oldSort)
            {
                case PaletteNES.SortMode.Default:
                    sel = m_Selected;
                    break;
                case PaletteNES.SortMode.ColorByRow:
                    sel = ((m_Selected & 0x03) << 4) + ((m_Selected & 0x3C) >> 2);
                    break;
                default:
                    sel = m_Selected;
                    break;
            }
            switch (newSort)
            {
                case PaletteNES.SortMode.Default:
                    return sel;
                case PaletteNES.SortMode.ColorByRow:
                    return ((sel & 0x0F) << 2) + ((sel & 0x30) >> 4);
                default:
                    return sel;
            }
        }

        private PaletteNES.SortMode _sort = PaletteNES.SortMode.ColorByRow;
        public PaletteNES.SortMode Sort
        {
            get { return _sort; }
            set
            {
                m_Selected = transposeSelection(_sort, value);
                _sort = value;
                createTexture();
            }
        }
    }
}
