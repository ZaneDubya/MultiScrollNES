using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace eightbit.NES
{
    class Palette
    {
        uint[] m_Palette;
        Texture2D m_Texture;
        public Texture2D Texture
        {
            get { return m_Texture; }
        }

        public Palette()
        {
            if (m_Palette == null)
                m_Palette = new uint[0x40];
            if (m_Texture != null)
            {
                m_Texture.Dispose();
                m_Texture = null;
            }
            Reset();
        }

        public void Reset()
        {
            for (uint i = 0; i < 0x40; i++)
            {
                m_Palette[i] = 0xFF000000 |
                    (nes_palette[i * 3 + 2] << 16) |
                    (nes_palette[i * 3 + 1] << 8) |
                    (nes_palette[i * 3]);
            }
            m_WriteTexture();
        }

        public void LoadPalette(int index, byte[] colors)
        {
            if (index < 0 || index >= 4)
                return;
            if (colors == null || colors.Length != 4)
                return;
            index *= 4;
            for (int i = 0; i < 4; i++)
            {
                m_Palette[i + index] = 0xFF000000 |
                    (nes_palette[colors[i] * 3 + 2] << 16) |
                    (nes_palette[colors[i] * 3 + 1] << 8) |
                    (nes_palette[colors[i] * 3]);
            }
            m_WriteTexture();
        }

        private void m_WriteTexture()
        {
            unsafe
            {
                uint[] pixels = new uint[64];
                fixed (uint* pPixels = pixels)
                {
                    fixed (uint* pData = m_Palette)
                    {
                        uint* pPixelsRef = pPixels;
                        uint* pDataRef = pData;

                        for (int i = 0; i < 64; i++)
                            *pPixelsRef++ = *pDataRef++;
                    }
                }

                m_Texture = Core.Library.CreateTexture(64, 1);
                m_Texture.SetData<uint>(m_Palette);
            }
        }

        public static uint ColorFromIndex(int index)
        {
            if (index < 0 || index >= 64)
                return 0xFFFF00FF;
            return (uint)(nes_palette[index * 3] + (nes_palette[index * 3 + 1] << 8) + (nes_palette[index * 3 + 2] << 16)) | 0xFF000000;
        }

        static uint[] nes_palette = new uint[64 * 3]
        {
           124,124,124,
            0,0,252,
            0,0,188,
            68,40,188,
            148,0,132,
            168,0,32,
            168,16,0,
            136,20,0,
            80,48,0,
            0,120,0,
            0,104,0,
            0,88,0,
            0,64,88,
            0,0,0,
            0,0,0,
            0,0,0,
            188,188,188,
            0,120,248,
            0,88,248,
            104,68,252,
            216,0,204,
            228,0,88,
            248,56,0,
            228,92,16,
            172,124,0,
            0,184,0,
            0,168,0,
            0,168,68,
            0,136,136,
            0,0,0,
            0,0,0,
            0,0,0,
            248,248,248,
            60,188,252,
            104,136,252,
            152,120,248,
            248,120,248,
            248,88,152,
            248,120,88,
            252,160,68,
            248,184,0,
            184,248,24,
            88,216,84,
            88,248,152,
            0,232,216,
            120,120,120,
            0,0,0,
            0,0,0,
            252,252,252,
            164,228,252,
            184,184,248,
            216,184,248,
            248,184,248,
            248,164,192,
            240,208,176,
            252,224,168,
            248,216,120,
            216,248,120,
            184,248,184,
            184,248,216,
            0,252,252,
            248,216,248,
            0,0,0,
            0,0,0
        };
    }
}
