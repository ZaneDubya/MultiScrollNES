using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eightbit.Data
{
    class SuperChunk
    {
        private const string c_SerializeIdentifier = "SChunk";

        private int[] m_Chunks = new int[4];
        private byte m_Tileset = 0;
        private List<Actor> m_Actors = new List<Actor>();
        private List<Egg> m_Eggs = new List<Egg>();

        public int[] Chunks
        {
            get { return m_Chunks; }
        }

        public int Chunk0
        {
            get { return m_Chunks[0]; }
            set
            {
                if (value < 0)
                    value = 0;
                if (value > 1023)
                    value = 1023;
                m_Chunks[0] = value;
            }
        }

        public int Chunk1
        {
            get { return m_Chunks[1]; }
            set
            {
                if (value < 0)
                    value = 0;
                if (value > 1023)
                    value = 1023;
                m_Chunks[1] = value;
            }
        }

        public int Chunk2
        {
            get { return m_Chunks[2]; }
            set
            {
                if (value < 0)
                    value = 0;
                if (value > 1023)
                    value = 1023;
                m_Chunks[2] = value;
            }
        }

        public int Chunk3
        {
            get { return m_Chunks[3]; }
            set
            {
                if (value < 0)
                    value = 0;
                if (value > 1023)
                    value = 1023;
                m_Chunks[3] = value;
            }
        }

        public int ActorCount
        {
            get { return m_Actors.Count; }
        }

        public int EggCount
        {
            get { return m_Eggs.Count; }
        }

        public Actor Actor(int index)
        {
            if (index < 0 || index >= m_Actors.Count)
                return null;
            return m_Actors[index];
        }

        public Egg Egg(int index)
        {
            if (index < 0 || index >= m_Eggs.Count)
                return null;
            return m_Eggs[index];
        }

        public void RemoveActor(Actor actor)
        {
            int index = -1;

            for (int i = 0; i < m_Actors.Count; i++)
                if (m_Actors[i] == actor)
                {
                    index = i;
                    break;
                }

            if (index != -1)
                m_Actors.RemoveAt(index);
        }

        public void RemoveActor(int index)
        {
            if (index < 0 || index >= m_Actors.Count)
                return;
            m_Actors.RemoveAt(index);
        }

        public void RemoveEgg(Egg egg)
        {
            int index = -1;

            for (int i = 0; i < m_Eggs.Count; i++)
                if (m_Eggs[i] == egg)
                {
                    index = i;
                    break;
                }

            if (index != -1)
                m_Actors.RemoveAt(index);
        }

        public void RemoveEgg(int index)
        {
            if (index < 0 || index >= m_Eggs.Count)
                return;
            m_Eggs.RemoveAt(index);
        }

        public Actor CreateActor()
        {
            Actor actor = new Actor();
            m_Actors.Add(actor);
            return actor;
        }

        public Egg CreateEgg()
        {
            Egg egg = new Egg();
            m_Eggs.Add(egg);
            return egg;
        }

        public SuperChunk()
        {

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
                for (int i = 0; i < 4; i++)
                    m_Chunks[i] = reader.ReadInt();
                m_Tileset = reader.ReadByte();

                int count;

                count = reader.ReadInt();
                for (int i = 0; i < count; i++)
                {
                    Actor actor = new Actor();
                    actor.Unserialize(reader);
                    m_Actors.Add(actor);
                }

                byte length = reader.ReadByte();

                count = reader.ReadInt();
                for (int i = 0; i < count; i++)
                {
                    Egg egg = new Egg();
                    egg.Unserialize(reader);
                    m_Eggs.Add(egg);
                }
            }
            return true;
        }

        public void Serialize(Core.BinaryFileWriter writer)
        {
            int length = 0;

            writer.Write(c_SerializeIdentifier);
            writer.Write((int)0); // version
            for (int i = 0; i < 4; i++)
                writer.Write((int)m_Chunks[i]);
            writer.Write((byte)m_Tileset);

            writer.Write((int)m_Actors.Count);
            length = 0;
            for (int i = 0; i < m_Actors.Count; i++)
                length += m_Actors[i].ByteLength;
            writer.Write((byte)length);
            for (int i = 0; i < m_Actors.Count; i++)
                m_Actors[i].Serialize(writer);

            writer.Write((int)m_Eggs.Count);
            for (int i = 0; i < m_Eggs.Count; i++)
                m_Eggs[i].Serialize(writer);
        }

        public byte[] Export()
        {
            List<byte> data = new List<byte>();

            bool has_chunks = (Chunk0 != 0) | (Chunk1 != 0) | (Chunk2 != 0) | (Chunk3 != 0);
            if (!has_chunks)
            {
                data.Add((byte)0); // setting bit 0 to 0 means this chunk is not in use, and there is no data.
            }
            else
            {
                // has_chunks is always true at this point.
                bool alternate_tileset = false;
                bool has_actors = (m_Actors.Count > 0);
                bool has_eggs = (m_Eggs.Count > 0);
                byte flags = (byte)((has_chunks ? 1 : 0) | (alternate_tileset ? 2 : 0) | (has_actors ? 4 : 0) | (has_eggs ? 8 : 0));
                data.Add(flags);

                byte chunks_hi = (byte)(((Chunk0 & 0x300) >> 8) | ((Chunk1 & 0x300) >> 6) | ((Chunk2 & 0x300) >> 4) | ((Chunk3 & 0x300) >> 2));
                // we transform the data (hi two bits in 0x03, low six bits in 0xfc) to make it faster for the nes game to load it.
                data.Add((byte)(((Chunk0 & 0x0003) << 6) + ((Chunk0 & 0x00FC) >> 2)));
                data.Add((byte)(((Chunk1 & 0x0003) << 6) + ((Chunk1 & 0x00FC) >> 2)));
                data.Add((byte)(((Chunk2 & 0x0003) << 6) + ((Chunk2 & 0x00FC) >> 2)));
                data.Add((byte)(((Chunk3 & 0x0003) << 6) + ((Chunk3 & 0x00FC) >> 2)));
                // data.Add((byte)(chunks_hi));         don't add chunks_hi - we won't have more than 256 chunks in this game.

                if (alternate_tileset)
                {
                    // not yet implemented.
                }

                if (has_actors)
                {
                    // not yet implemented.
                }

                if (has_eggs)
                {
                    // not yet implemented.
                }
            }

            byte[] export = data.ToArray();
            return export;
        }

        public override string ToString()
        {
            return string.Format("<{0},{1},{2},{3}>", Chunk0, Chunk1, Chunk2, Chunk3);
        }
    }
}
