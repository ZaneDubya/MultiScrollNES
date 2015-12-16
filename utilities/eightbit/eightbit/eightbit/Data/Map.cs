using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eightbit.Data
{
    class Map
    {
        private const string c_SerializeIdentifier = "XneMxp";
        private List<SuperChunk> m_SC;
        public List<SuperChunk> SuperChunks
        {
            get { return m_SC; }
        }

        /// <summary>
        /// Returns the number of superchunks in this map.
        /// </summary>
        public int Count
        {
            get { return m_SC.Count; }
        }

        public SuperChunk this[int index]
        {
            get
            {
                if (index < 0 || index >= m_SC.Count)
                    return null;
                return m_SC[index];
            }
        }

        private MapSize m__Size = MapSize.Empty;
        public MapSize Size
        {
            get { return m__Size; }
            set
            {
                if (value == m__Size)
                    return;

                int old_width = WidthInSuperChunks;
                int old_height = HeightInSuperChunks;
                m__Size = value;

                List<SuperChunk> sc = new List<SuperChunk>();
                for (int y = 0; y < HeightInSuperChunks; y++)
                    for (int x = 0; x < WidthInSuperChunks; x++)
                    {
                        if ((m_SC != null) && (x < old_width) && (y < old_height))
                            sc.Add(m_SC[x + y * old_width]);
                        else
                            sc.Add(new SuperChunk());
                    }
                m_SC = sc;
            }
        }

        public int WidthInSuperChunks
        {
            get
            {
                switch (Size)
                {
                    case MapSize.W16H16:
                        return 16;
                    case MapSize.W32H32:
                        return 32;
                    default:
                        return 0;
                }
            }
        }

        public int HeightInSuperChunks
        {
            get
            {
                switch (Size)
                {
                    case MapSize.W16H16:
                        return 16;
                    case MapSize.W32H32:
                        return 32;
                    default:
                        return 0;
                }
            }
        }

        public int WidthInChunks { get { return WidthInSuperChunks * 2; } }
        public int HeightInChunks { get { return HeightInSuperChunks * 2; } }

        private int m_Tileset = 0;
        public int Tileset
        {
            get { return m_Tileset; }
            set { m_Tileset = value; }
        }

        public SuperChunk GetSuperChunk(int x, int y)
        {
            if (x < 0 || x >= WidthInSuperChunks)
                return null;
            if (y < 0 || y >= HeightInSuperChunks)
                return null;
            if (m_SC == null)
                return null;
            return this[y * WidthInSuperChunks + x];
        }

        public Map()
        {
            Size = MapSize.W32H32;
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
                m_Tileset = reader.ReadInt();
                if (version == 0)
                {
                    reader.ReadInt();
                    reader.ReadInt();
                    Size = MapSize.W32H32;
                }
                else if (version >= 1)
                    Size = (MapSize)reader.ReadInt();

                if (m_SC == null)
                    m_SC = new List<SuperChunk>();
                m_SC.Clear();
                for (int i = 0; i < WidthInSuperChunks * HeightInSuperChunks; i++)
                {
                    SuperChunk superchunk = new SuperChunk();
                    superchunk.Unserialize(reader);
                    m_SC.Add(superchunk);
                }
            }
            return true;
        }

        public void Serialize(Core.BinaryFileWriter writer)
        {
            writer.Write(c_SerializeIdentifier);
            writer.Write((int)1); // version
            writer.Write((int)m_Tileset);
            writer.Write((int)Size);
            for (int i = 0; i < WidthInSuperChunks * HeightInSuperChunks; i++)
            {
                m_SC[i].Serialize(writer);
            }
        }

        public void Export(Core.BinaryFileWriter pointers, Core.BinaryFileWriter data, ref int bank_offset, ref int ptr_offset)
        {
            int bank_size = 8192; // note that constants on line 'int pointer =' also reflect this value.
            int ptr_begin = 0x8000;
            List<byte[]> superchunk_data = new List<byte[]>();
            for (int i = 0; i < Count; i++)
                superchunk_data.Add(m_SC[i].Export());

            for (int i = 0; i < Count; i++)
            {
                if (ptr_offset + superchunk_data[i].Length >= bank_size)
                {
                    int pad = (bank_size - ptr_offset);
                    for (int j = 0; j < pad; j++)
                        data.Write((byte)0xAA);
                    ptr_offset = 0;
                    bank_offset += 1;
                }
                int pointer = ptr_begin + (ptr_offset & 0x1FFF) + ((bank_offset & 0x0007) << 13);
                pointers.Write((ushort)pointer);
                for (int j = 0; j < superchunk_data[i].Length; j++)
                    data.Write((byte)superchunk_data[i][j]);
                ptr_offset += superchunk_data[i].Length;
            }
        }
    }

    public enum MapSize : int
    {
        Empty = 0,
        W16H16 = 1,
        W32H32 = 2
    }
}
