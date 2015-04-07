using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eightbit.Data
{
    class Maps
    {
        private const string c_SerializeIdentifier = "ThMxps";
        private List<Map> m_Maps;
        public List<Map> AllMaps
        {
            get { return m_Maps; }
        }

        public int Count
        {
            get { return m_Maps.Count; }
        }

        public Map this[int index]
        {
            get
            {
                if (index < 0 || index >= m_Maps.Count)
                    return null;
                return m_Maps[index];
            }
        }

        public Maps()
        {
            m_Maps = new List<Map>();
        }

        public void AddMap()
        {
            m_Maps.Add(new Map());
        }

        public void RemoveLastMap()
        {
            if (m_Maps.Count == 0)
                return;
            m_Maps.RemoveAt(m_Maps.Count - 1);
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
                    Map map = new Map();
                    map.Unserialize(reader);
                    m_Maps.Add(map);
                }
            }
            return true;
        }

        public void Serialize(Core.BinaryFileWriter writer)
        {
            writer.Write(c_SerializeIdentifier);
            writer.Write((int)0); // version
            writer.Write((int)m_Maps.Count);
            for (int i = 0; i < m_Maps.Count; i++)
            {
                m_Maps[i].Serialize(writer);
            }
        }

        public void Export(Core.BinaryFileWriter headers, Core.BinaryFileWriter pointers, Core.BinaryFileWriter data)
        {
            int bank_offset = 1; // offset to the first bank that contains superchunk data for this map
            int ptr_offset = 0x0200; // offset to the first available byte in the first superchunk available for data.
            int bank_first = 0; // should be zero when passed to Map.Export().

            for (int i = 0; i < m_Maps.Count; i++)
            {
                headers.Write((byte)i);
                headers.Write((byte)bank_offset);
                headers.Write((byte)m_Maps[i].Tileset);
                headers.Write((byte)0);

                m_Maps[i].Export(pointers, data, ref bank_first, ref ptr_offset);
            }
        }
    }
}
