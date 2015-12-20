using Microsoft.Xna.Framework;

namespace eightbit.NES
{
    class NameTable
    {
        private float _scale = 8.0f;
        public float Scale
        {
            get { return _scale; }
            set
            {
                _scale = value * 8.0f;
                Vector4 t = getTile(0);
                int i = 0;
                for (int y = 0; y < _h; y++)
                    for (int x = 0; x < _w; x++)
                    {
                        m_V[i++] = new Core.Graphics.VertexPositionTextureHueExtra(new Vector3(x * _scale, y * _scale, 0), new Vector2(t.X, t.Y), Color.White, new Vector2());
                        m_V[i++] = new Core.Graphics.VertexPositionTextureHueExtra(new Vector3(x * _scale + _scale, y * _scale, 0), new Vector2(t.W, t.Y), Color.White, new Vector2());
                        m_V[i++] = new Core.Graphics.VertexPositionTextureHueExtra(new Vector3(x * _scale, y * _scale + _scale, 0), new Vector2(t.X, t.Z), Color.White, new Vector2());
                        m_V[i++] = new Core.Graphics.VertexPositionTextureHueExtra(new Vector3(x * _scale + _scale, y * _scale + _scale, 0), new Vector2(t.W, t.Z), Color.White, new Vector2());
                    }
            }
        }

        public NameTable()
        {
            m_Tiles = new int[_w * _h];
            m_Attributes = new int[_w * _h / 4];
            m_V = new Core.Graphics.VertexPositionTextureHueExtra[_w * _h * 4];
            Scale = 2;
        }

        public void RandomizeTiles()
        {
            for (int i = 0; i < _w * _h; i++)
                this[i] = Core.Library.Random(0, 255);
        }

        public void RandomizeAttributes()
        {
            for (int i = 0; i < _w * _h / 4; i++)
                this.SetAttribute(i, Core.Library.Random(0, 3));
        }

        public Core.Graphics.VertexPositionTextureHueExtra[] Vertexes
        {
            get { return m_V; }
        }

        public int this[int index]
        {
            get { return m_Tiles[index % m_Tiles.Length]; }
            set
            {
                index %= m_Tiles.Length;
                m_Tiles[index] = value;
                Vector4 t = getTile(value);
                m_V[index * 4 + 0].TextureCoordinate = new Vector2(t.X, t.Y);
                m_V[index * 4 + 1].TextureCoordinate = new Vector2(t.W, t.Y);
                m_V[index * 4 + 2].TextureCoordinate = new Vector2(t.X, t.Z);
                m_V[index * 4 + 3].TextureCoordinate = new Vector2(t.W, t.Z);
            }
        }

        public int GetAttribute(int index)
        {
            return m_Attributes[index % m_Attributes.Length];
        }

        public void SetAttribute(int index, int value)
        {
            index %= m_Attributes.Length;
            m_Attributes[index] = value;
            int x = (index % (_w / 2)) * 2;
            int y = (index / (_w / 2)) * 2;
            for (int i = x; i < x + 2; i++)
                for (int j = y; j < y + 2; j++)
                {
                    int tIndex = ((j * _w) + i);
                    m_V[tIndex * 4 + 0].Extra.X = value;
                    m_V[tIndex * 4 + 1].Extra.X = value;
                    m_V[tIndex * 4 + 2].Extra.X = value;
                    m_V[tIndex * 4 + 3].Extra.X = value;
                }
        }

        private Vector4 getTile(int index)
        {
            index %= 256;
            float x = (index % 16) / 32f;
            float y = ((index - (index % 16)) / 16f) / 16f;
            return new Vector4(x, y, y + nexty, x + nextx);
        }

        private const int _w = 32, _h = 30;
        private int[] m_Tiles, m_Attributes;
        private Core.Graphics.VertexPositionTextureHueExtra[] m_V;

        const float nextx = (1f / 32f);
        const float nexty = (1f / 16f);
        const bool AttributeIs4 = true;
    }
}
