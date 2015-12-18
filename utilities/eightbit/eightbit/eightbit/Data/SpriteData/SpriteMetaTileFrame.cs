using System;

namespace eightbit.Data.SpriteData
{
    public class SpriteMetaTileFrame
    {
        public SpriteMetaTileFrame()
        {
            m_Tile = m_Attributes = m_TilePage = 0;
        }

        public SpriteMetaTileFrame(Core.BinaryFileReader reader, int version)
        {
            m_Tile = reader.ReadByte();
            m_Attributes = reader.ReadByte();

            if (version >= 2)
            {
                m_TilePage = reader.ReadByte();
            }
            else
            {
                m_TilePage = 1; // debug.
            }
        }

        private byte m_Tile = 0x00;
        private byte m_Attributes = 0x00;
        private byte m_TilePage = 0x00;

        public byte Tile
        {
            get { return m_Tile; }
            set { m_Tile = value; }
        }

        public byte TilePage
        {
            get { return m_TilePage; }
            set { m_TilePage = value; }
        }

        public bool FlipH
        {
            get
            {
                return (m_Attributes & 0x40) != 0 ? true : false;
            }
            set
            {
                m_Attributes = (byte)((m_Attributes & 0xBF) + (value ? 0x40 : 0x00));
            }
        }

        public bool FlipV
        {
            get
            {
                return (m_Attributes & 0x80) != 0 ? true : false;
            }
            set
            {
                m_Attributes = (byte)((m_Attributes & 0x7F) + (value ? 0x80 : 0x00));
            }
        }

        public void Flip()
        {
            if (FlipH)
            {
                FlipH = false;
                FlipV = !FlipV;
            }
            else
            {
                FlipH = true;
            }
        }

        public void Serialize(Core.BinaryFileWriter writer, int version)
        {
            writer.Write(m_Tile);
            writer.Write(m_Attributes);

            if (version >= 2)
                writer.Write(m_TilePage);
        }

        public void Export(Core.BinaryFileWriter writer, Tuple<byte, byte>[] tile_transform_table)
        {
            for (int i = 0; i < tile_transform_table.Length; i++)
            {
                if (tile_transform_table[i].Item1 == Tile && tile_transform_table[i].Item2 == TilePage)
                {
                    writer.Write(i);
                    writer.Write(m_Attributes);
                    return;
                }
            }
            throw new Exception("SpriteMetaTileFrame:Export() - did not find valid entry in tile_transform_table");
        }
    }
}
