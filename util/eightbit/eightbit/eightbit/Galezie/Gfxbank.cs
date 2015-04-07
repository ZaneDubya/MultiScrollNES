using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace eightbit.Galezie
{
    class Gfxbank
    {
        private int m_GfxBankIndex;
        public int Index
        {
            get { return m_GfxBankIndex; }
        }

        private Texture2D m_Texture;
        public Texture2D Texture
        {
            get { return m_Texture; }
        }

        private byte[] _data = new byte[8192];
        private byte[] _subindex_masks = new byte[4] { 0xFC, 0xF3, 0xCF, 0x3F };

        public Gfxbank(int gfxbank_index)
        {
            m_GfxBankIndex = gfxbank_index;
            createTexture();
        }

        private void createTexture()
        {
            uint[] pixels = new uint[128 * 128];
            m_Texture = Core.Library.CreateTexture(128, 128);
            m_Texture.SetData<uint>(pixels);
        }

        public byte[] GetTile(int index)
        {
            byte[] tile = new byte[16];
            if (index < 0 || index > 511)
                return tile;
            for (int i = 0; i < 16; i++)
                tile[i] = _data[i + index * 16];
            return tile;
        }

        public void SetTilePixel(int tile, int x, int y, int pixel)
        {
            if (tile < 0 || tile > 511)
                return;
            if (x < 0 || x > 7)
                return;
            if (y < 0 || y > 7)
                return;
            if (pixel < 0 || pixel > 3)
                return;

            int index = tile * 16 + y * 2 + (x >> 2);
            int subindex = (x % 4);
            int data = (_data[index] & _subindex_masks[subindex]) + ((pixel & 0x03) << (subindex * 2));
            _data[index] = (byte)data;
        }

        public void Test()
        {
            for (int i = 0; i < 512; i++)
                for (int j = 0; j < 64; j++)
                    for (int k = 0; k < 4; k++)
                    {
                        SetTilePixel(i, j % 8, j >> 3, k);
                        int l = _data[i * 16 + (j >> 2)];
                        if ((l >> ((j % 4) * 2) & 0x03) != k)
                            throw new Exception();
                    }
        }

        public void WriteTile(int index, byte[] tile)
        {
            uint[] data = new uint[64];
            for (int i = 0; i < 16; i++)
            {
                data[i * 4 + 0] = (uint)(tile[i] & 0x03);
                data[i * 4 + 1] = (uint)((tile[i] >> 2) & 0x03);
                data[i * 4 + 2] = (uint)((tile[i] >> 4) & 0x03);
                data[i * 4 + 3] = (uint)((tile[i] >> 6) & 0x03);
            }
            m_Texture.SetData<uint>(0, new Rectangle((index % 16) * 8, (index / 16) * 8, 8, 8), data, 0, 64);
        }

        public void WritePage(byte[] page)
        {
            uint[] data = new uint[128 * 128];
            for (int i = 0; i < 256; i++)
            {
                int pixel_start = (i % 16) * 8 + ((i / 16) * 128 * 8);
                for (int j = 0; j < 16; j++)
                {
                    int pixel = pixel_start + (j % 2) * 4 + (j / 2) * 128;
                    data[pixel + 0] = (uint)(page[i * 16 + j] & 0x03);
                    data[pixel + 1] = (uint)((page[i * 16 + j] >> 2) & 0x03);
                    data[pixel + 2] = (uint)((page[i * 16 + j] >> 4) & 0x03);
                    data[pixel + 3] = (uint)((page[i * 16 + j] >> 6) & 0x03);
                }
            }
            m_Texture.SetData<uint>(data);
        }
    }
}
