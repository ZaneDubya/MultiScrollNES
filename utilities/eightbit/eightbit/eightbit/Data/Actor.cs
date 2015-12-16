using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eightbit.Data
{
    class Actor
    {
        private byte m_Location = 0; // 1b location(yyyyxxxx)
        private int m_SaveIndex = 0; // 2b actor index (0 = on/exists, 1 = off/missing)
        private byte m_TypeLo = 0, m_TypeHi = 0;
        // 2b actor type + flags
        // 76543210 76543210
        // cduiTTTT tttttttt
        private List<Actor> m_Contents = new List<Actor>();

        public int ByteLength
        {
            get
            {
                int length = 5;
                if (IsContainer)
                {
                    length += 2;
                    foreach (Actor actor in m_Contents)
                        length += actor.ByteLength;
                }
                return length;
            }
        }

        public byte X
        {
            get { return (byte)(m_Location & 0x0F); }
            set
            {
                value = (byte)Core.Library.Clamp(value, 0, 0x0f);
                m_Location = (byte)((m_Location & 0xF0) | value);
            }
        }

        public byte Y
        {
            get { return (byte)((m_Location & 0xF0) >> 4); }
            set
            {
                value = (byte)Core.Library.Clamp(value, 0, 0x0f);
                m_Location = (byte)((m_Location & 0x0F) | (value << 4));
            }
        }

        public int SaveIndex
        {
            get { return m_SaveIndex; }
            set
            {
                value = Core.Library.Clamp(value, 0, State.Data.MaxSaveIndex);
                m_SaveIndex = value;
            }
        }

        public byte ActorTypeLo
        {
            get { return m_TypeLo; }
            set { m_TypeLo = value; }
        }

        public byte ActorTypeHi
        {
            get { return (byte)(m_TypeHi & 0x0F); }
            set
            {
                value = (byte)Core.Library.Clamp(value, 0, 0x0f);
                m_TypeHi = (byte)((m_Location & 0xF0) | value);
            }
        }

        public bool IsItem
        {
            get
            {
                return ((m_TypeHi & 0x10) == 0) ? false : true;
            }
            set
            {
                if (value == true)
                {
                    m_TypeHi = (byte)(m_TypeHi | (byte)0x10);
                }
                else
                {
                    m_TypeHi = (byte)(m_TypeHi & ~(byte)0x10);
                }
            }
        }

        public bool IsContainer
        {
            get
            {
                return ((m_TypeHi & 0x80) == 0) ? false : true;
            }
            set
            {
                if (value == true)
                {
                    m_TypeHi = (byte)(m_TypeHi | (byte)0x80);
                }
                else
                {
                    m_TypeHi = (byte)(m_TypeHi & ~(byte)0x80);
                }
            }
        }

        public bool IsUnit
        {
            get
            {
                return ((m_TypeHi & 0x20) == 0) ? false : true;
            }
            set
            {
                if (value == true)
                {
                    m_TypeHi = (byte)(m_TypeHi | (byte)0x20);
                }
                else
                {
                    m_TypeHi = (byte)(m_TypeHi & ~(byte)0x20);
                }
            }
        }

        public bool IsGameObject
        {
            get
            {
                return ((m_TypeHi & 0x40) == 0) ? false : true;
            }
            set
            {
                if (value == true)
                {
                    m_TypeHi = (byte)(m_TypeHi | (byte)0x40);
                }
                else
                {
                    m_TypeHi = (byte)(m_TypeHi & ~(byte)0x40);
                }
            }
        }

        public int ContentsCount
        {
            get
            {
                if (!IsContainer)
                    return 0;
                else
                {
                    return m_Contents.Count;
                }
            }
        }

        public Actor GetContentsAtIndex(int index)
        {
            if (index < 0)
                return null;
            if (index >= m_Contents.Count)
                return null;
            return m_Contents[index];
        }

        public Actor()
        {

        }

        public void Unserialize(Core.BinaryFileReader reader)
        {
            m_Location = reader.ReadByte();
            m_SaveIndex = reader.ReadUShort();
            m_TypeLo = reader.ReadByte();
            m_TypeHi = reader.ReadByte();
            if (IsContainer)
            {
                reader.ReadByte();
                int count = reader.ReadByte();
                for (int i = 0; i < count; i++)
                {
                    Actor actor = new Actor();
                    actor.Unserialize(reader);
                    m_Contents.Add(actor);
                }
            }
        }

        public void Serialize(Core.BinaryFileWriter writer)
        {
            writer.Write((byte)m_Location);
            writer.Write((ushort)m_SaveIndex);
            writer.Write((byte)m_TypeLo);
            writer.Write((byte)m_TypeHi);
            if (IsContainer)
            {
                writer.Write((byte)ByteLength - 5);
                writer.Write((byte)m_Contents.Count);
                for (int i = 0; i < m_Contents.Count; i++)
                    m_Contents[i].Serialize(writer);
            }
        }
    }
}
