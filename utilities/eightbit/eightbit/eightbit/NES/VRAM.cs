using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace eightbit.NES
{
    class VRAM
    {
        private Texture2D m_Texture;

        private const int _x = 256, _y = 256;
        private const int _tileswidth = 16;
        private const int _tilesperbank = 256;

        public Texture2D Texture
        {
            get { return m_Texture; }
        }

        public VRAM()
        {
            if (m_Texture != null)
            {
                m_Texture.Dispose();
                m_Texture = null;
            }
            createTexture();
        }

        public void Randomize()
        {
            for (int i = 0; i < 512; i++)
                WriteTile(i, Core.Library.RandomTile());
        }

        private void createTexture()
        {
            uint[] pixels = new uint[_x * _y];
            m_Texture = Core.Library.CreateTexture(_x, _y);
            m_Texture.SetData<uint>(pixels);
        }

        public void WriteTile(int index, uint[] tiledata)
        {
            int bank = (int)(index / _tilesperbank);
            int x = (index % _tileswidth) + (bank * _tileswidth);
            int y = (int)(index / _tileswidth) - (bank * _tileswidth);
            Rectangle dest = new Rectangle(x * 8, y * 8, 8, 8);
            m_Texture.SetData<uint>(0, dest, tiledata, 0, 64);
        }
    }
}
