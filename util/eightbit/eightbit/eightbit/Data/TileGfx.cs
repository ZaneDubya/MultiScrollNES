using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eightbit.Data
{
    class TileGfx
    {
        private const string c_SerializeIdentifier = "TilGfx";
        private List<byte[]> m_TileGfxPages;

        public int TileCount
        {
            get { return m_TileGfxPages.Count * 256; }
        }

        public int PageCount
        {
            get { return m_TileGfxPages.Count; }
        }

        public void SetPixel(int page, int index, int pixel, byte color)
        {
            if (page < 0 || page >= m_TileGfxPages.Count)
                return;
            if (index < 0 || index >= 256)
                return;
            byte[] pixels = GetTile(page, index);
            int byte_index = pixel >> 2;
            int bit_index = (pixel & 0x03) * 2;

            byte b0 = (byte)(0x03 << bit_index);
            byte b1 = (byte)~b0;

            pixels[byte_index] = (byte)(((pixels[byte_index] & b1) | (color << bit_index)));
            SetTile(page, index, pixels);
        }

        public byte GetPixel(int page, int index, int pixel)
        {
            if (page < 0 || page >= m_TileGfxPages.Count)
                return 0;
            if (index < 0 || index >= 256)
                return 0;
            byte[] pixels = GetTile(page, index);
            int byte_index = pixel >> 2;
            int bit_index = (pixel & 0x03) * 2;

            byte b0 = (byte)(0x03 << bit_index);
            byte p = (byte)((pixels[byte_index] & b0) >> bit_index);
            return p;
        }

        public byte[] GetTile(int page, int index)
        {
            if (page < 0 || page >= m_TileGfxPages.Count)
                return null;
            if (index < 0 || index >= 256)
                return null;
            byte[] data = new byte[16];
            int index_in_page = (index % 256) * 16;
            Array.Copy(m_TileGfxPages[page], index_in_page, data, 0, 16);
            return data;
        }

        public void SetTile(int page, int index, byte[] pixels)
        {
            if (page < 0 || page >= m_TileGfxPages.Count)
                return;
            if (index < 0 || index >= 256)
                return;
            if (pixels.Length != 16)
                return;
            for (int i = 0; i < 16; i++)
                m_TileGfxPages[page][index * 16 + i] = pixels[i];
        }

        public byte[] GetPage(int index)
        {
            if (index < 0 || index >= m_TileGfxPages.Count)
                return null;
            return m_TileGfxPages[index];
        }

        public TileGfx()
        {
            m_TileGfxPages = new List<byte[]>();
        }

        public void AddTilePage()
        {
            m_TileGfxPages.Add(new byte[4096]);
        }

        public void RemoveTilePage()
        {
            if (PageCount > 0)
                m_TileGfxPages.RemoveAt(m_TileGfxPages.Count - 1);
        }

        public bool Unserialize(Core.BinaryFileReader reader)
        {
            string id = reader.ReadString();
            if (id != c_SerializeIdentifier)
                return false;
            int version = reader.ReadInt();
            if (version >= 0)
            {
                // version 0
                int count = reader.ReadInt();
                for (int i = 0; i < count; i++)
                {
                    byte[] data = new byte[4096];
                    for (int j = 0; j < 4096; j++)
                        data[j] = reader.ReadByte();
                    m_TileGfxPages.Add(data);
                }
            }
            return true;
        }

        public void Serialize(Core.BinaryFileWriter writer)
        {
            writer.Write(c_SerializeIdentifier);
            writer.Write((int)0); // version
            writer.Write(PageCount);
            for (int i = 0; i < PageCount; i++)
            {
                byte[] data = m_TileGfxPages[i];
                for (int j = 0; j < 4096; j++)
                    writer.Write(data[j]);
            }
        }

        public void Export(Core.BinaryFileWriter writer)
        {
            for (int i = 0; i < PageCount; i++)
            {
                byte[] data = m_TileGfxPages[i];

                for (int j = 0; j < 256; j++)
                {
                    byte[] tile = new byte[16];
                    for (int k = 0; k < 16; k++)
                        tile[k] = data[j * 16 + k];

                    for (int k = 0; k < 8; k++)
                    {
                        byte export = (byte)(
                            ((tile[(k * 2)] & 0x01) != 0 ? 0x80 : 0) |
                            ((tile[(k * 2)] & 0x04) != 0 ? 0x40 : 0) |
                            ((tile[(k * 2)] & 0x10) != 0 ? 0x20 : 0) |
                            ((tile[(k * 2)] & 0x40) != 0 ? 0x10 : 0) |
                            ((tile[(k * 2) + 1] & 0x01) != 0 ? 0x08 : 0) |
                            ((tile[(k * 2) + 1] & 0x04) != 0 ? 0x04 : 0) |
                            ((tile[(k * 2) + 1] & 0x10) != 0 ? 0x02 : 0) |
                            ((tile[(k * 2) + 1] & 0x40) != 0 ? 0x01 : 0)
                            );
                        writer.Write(export);
                    }

                    for (int k = 0; k < 8; k++)
                    {
                        byte export = (byte)(
                            ((tile[(k * 2)] & 0x02) != 0 ? 0x80 : 0) |
                            ((tile[(k * 2)] & 0x08) != 0 ? 0x40 : 0) |
                            ((tile[(k * 2)] & 0x20) != 0 ? 0x20 : 0) |
                            ((tile[(k * 2)] & 0x80) != 0 ? 0x10 : 0) |
                            ((tile[(k * 2) + 1] & 0x02) != 0 ? 0x08 : 0) |
                            ((tile[(k * 2) + 1] & 0x08) != 0 ? 0x04 : 0) |
                            ((tile[(k * 2) + 1] & 0x20) != 0 ? 0x02 : 0) |
                            ((tile[(k * 2) + 1] & 0x80) != 0 ? 0x01 : 0)
                            );
                        writer.Write(export);
                    }
                }
            }
        }
    }
}
