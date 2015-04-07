using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eightbit.Data
{
    class TileSets
    {
        private const string c_SerializeIdentifier = "TilSts";
        private List<TileSet> m_TileSets;

        public int Count
        {
            get { return m_TileSets.Count; }
        }

        public TileSet this[int index]
        {
            get
            {
                if (index < 0 || index >= m_TileSets.Count)
                    return null;
                return m_TileSets[index];
            }
        }

        public TileSets()
        {
            m_TileSets = new List<TileSet>();
        }

        public TileSet AddTileset()
        {
            TileSet t = new TileSet();
            m_TileSets.Add(t);
            return t;
        }

        public void RemoveLastTileset()
        {
            if (m_TileSets.Count > 0)
                m_TileSets.RemoveAt(m_TileSets.Count - 1);
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
                    TileSet t = new TileSet();
                    t.Unserialize(reader);
                    m_TileSets.Add(t);
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
                m_TileSets[i].Serialize(writer);
            }
        }

        public void Export(Core.BinaryFileWriter writer)
        {
            for (int i = 0; i < Count; i++)
            {
                m_TileSets[i].Export(writer);
            }
        }
    }
}
