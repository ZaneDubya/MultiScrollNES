using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eightbit.Data
{
    class Chunks
    {
        private const string c_SerializeIdentifier = "Chunks";
        private List<Chunk> m_Chunks;

        public int Count
        {
            get { return m_Chunks.Count; }
        }

        public Chunk this[int index]
        {
            get
            {
                if (index < 0 || index >= m_Chunks.Count)
                    return null;
                return m_Chunks[index];
            }
        }

        public Chunk[] ToArray()
        {
            if (m_Chunks == null || m_Chunks.Count == 0)
                return null;
            return m_Chunks.ToArray();
        }

        public Chunks()
        {
            m_Chunks = new List<Chunk>();
        }

        public void AddChunk()
        {
            m_Chunks.Add(new Chunk());
        }

        public void RemoveLastChunk()
        {
            if (m_Chunks.Count > 0)
                m_Chunks.RemoveAt(m_Chunks.Count - 1);
        }

        public void RemoveChunk(int index)
        {
            m_Chunks.RemoveAt(index);
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
                    string name = reader.ReadString();
                    int tileset = reader.ReadInt();
                    byte[] data = new byte[64];
                    for (int j = 0; j < 64; j++)
                        data[j] = reader.ReadByte();
                    m_Chunks.Add(new Chunk(data, tileset));
                    m_Chunks[m_Chunks.Count - 1].Name = name;
                }
            }
            return true;
        }

        public void Serialize(Core.BinaryFileWriter writer)
        {
            writer.Write(c_SerializeIdentifier);
            writer.Write((int)0); // version
            writer.Write(Count);
            for (int i = 0; i < Count; i++)
            {
                writer.Write((string)m_Chunks[i].Name);
                writer.Write((int)m_Chunks[i].Tileset);
                for (int j = 0; j < 64; j++)
                {
                    byte b = (byte)m_Chunks[i][j];
                    writer.Write((byte)b);
                }
            }
        }

        public void Export(Core.BinaryFileWriter writer)
        {
            for (int i = 0; i < Count; i++)
            {
                for (int j = 0; j < 64; j++)
                {
                    byte b = (byte)m_Chunks[i][j];
                    writer.Write((byte)b);
                }
            }
        }
    }

    public class Chunk
    {
        private byte[] m_Tiles;
        private int m_Tileset;

        private string m_Name;
        public string Name
        {
            get { return m_Name; }
            set { m_Name = value; }
        }

        public int Tileset
        {
            get { return m_Tileset; }
            set { m_Tileset = value; }
        }

        public int this[int index]
        {
            get
            {
                if (index < 0 || index >= 64)
                    return 0;
                return m_Tiles[index];
            }
            set
            {
                if (index < 0 || index >= 64)
                    return;
                if (value < 0 || value >= Data.TileSet.TilesPerSet)
                    return;
                m_Tiles[index] = (byte)value;

            }
        }

        public Chunk()
        {
            m_Tiles = new byte[64];
            m_Tileset = 0;
        }

        public Chunk(byte[] tiles, int tileset)
        {
            m_Tiles = tiles;
            m_Tileset = tileset;
        }

        public override string ToString()
        {
            if (m_Name == null || m_Name == string.Empty)
                return "Unnamed Chunk";
            return m_Name;
        }
    }
}
