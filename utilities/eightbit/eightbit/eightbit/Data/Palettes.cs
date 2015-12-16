using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eightbit.Data
{
    class Palettes
    {
        private const string c_SerializeIdentifier = "PalDat";
        private List<byte[]> m_Palettes;

        public int Count
        {
            get { return m_Palettes.Count; }
        }

        public byte[] this[int index]
        {
            get
            {
                if (index < 0 || index >= m_Palettes.Count)
                    return null;
                return m_Palettes[index];
            }
        }

        public Palettes()
        {
            m_Palettes = new List<byte[]>();
        }

        public void AddPalette(byte a, byte b, byte c, byte d)
        {
            m_Palettes.Add(new byte[4] { a, b, c, d });
        }

        public void RemovePalette()
        {
            if (m_Palettes.Count > 0)
                m_Palettes.RemoveAt(m_Palettes.Count - 1);
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
                    byte[] data = new byte[4];
                    data[0] = reader.ReadByte();
                    data[1] = reader.ReadByte();
                    data[2] = reader.ReadByte();
                    data[3] = reader.ReadByte();
                    m_Palettes.Add(data);
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
                byte[] data = this[i];
                writer.Write(data[0]);
                writer.Write(data[1]);
                writer.Write(data[2]);
                writer.Write(data[3]);
            }
        }

        public void Export(Core.BinaryFileWriter writer)
        {
            for (int i = 0; i < Count; i++)
            {
                byte[] data = this[i];
                writer.Write(data[0]);
                writer.Write(data[1]);
                writer.Write(data[2]);
                writer.Write(data[3]);
            }
        }
    }
}
