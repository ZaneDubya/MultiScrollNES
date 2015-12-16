using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eightbit.Data
{
    class Egg
    {
        private byte m_Location = 0; // 1b location(yyyyxxxx)
        private List<Condition> m_Conditions = new List<Condition>();
        private List<Effect> m_Effects = new List<Effect>();

        public int ByteLength
        {
            get
            {
                int length = 4;
                foreach (Condition c in m_Conditions)
                    length += c.ByteLength;
                foreach (Effect e in m_Effects)
                    length += e.ByteLength;
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

        public int ConditionsCount
        {
            get { return m_Conditions.Count; }
        }

        public int EffectsCount
        {
            get { return m_Conditions.Count; }
        }

        public Condition GetCondition(int index)
        {
            if (index < 0)
                return null;
            if (index >= m_Conditions.Count)
                return null;
            return m_Conditions[index];
        }

        public Effect GetEffect(int index)
        {
            if (index < 0)
                return null;
            if (index >= m_Effects.Count)
                return null;
            return m_Effects[index];
        }

        public Condition AddCondition()
        {
            Condition condition = new Condition();
            m_Conditions.Add(condition);
            return condition;
        }

        public Effect AddEffect()
        {
            Effect effect = new Effect();
            m_Effects.Add(effect);
            return effect;
        }

        public void RemoveCondition(int index)
        {
            if (index < 0)
                return;
            if (index >= m_Conditions.Count)
                return;
            m_Conditions.RemoveAt(index);
        }

        public void RemoveCondition(Condition condition)
        {
            m_Conditions.Remove(condition);
        }

        public void RemoveEffect(int index)
        {
            if (index < 0)
                return;
            if (index >= m_Effects.Count)
                return;
            m_Effects.RemoveAt(index);
        }

        public void RemoveEffect(Effect effect)
        {
            m_Effects.Remove(effect);
        }

        public Egg()
        {

        }

        public void Unserialize(Core.BinaryFileReader reader)
        {
            m_Location = reader.ReadByte();
            reader.ReadByte(); // length

            m_Conditions.Clear();
            int conditioncount = reader.ReadByte();
            for (int i = 0; i < conditioncount; i++)
            {
                Condition condition = new Condition();
                condition.Unserialize(reader);
                m_Conditions.Add(condition);
            }

            m_Effects.Clear();
            int effectcount = reader.ReadByte();
            for (int i = 0; i < effectcount; i++)
            {
                Effect effect = new Effect();
                effect.Unserialize(reader);
                m_Effects.Add(effect);
            }
        }

        public void Serialize(Core.BinaryFileWriter writer)
        {
            writer.Write((byte)ByteLength);
            writer.Write((byte)m_Location);

            writer.Write((byte)m_Conditions.Count);
            for (int i = 0; i < m_Conditions.Count; i++)
                m_Conditions[i].Serialize(writer);

            writer.Write((byte)m_Effects.Count);
            for (int i = 0; i < m_Effects.Count; i++)
                m_Effects[i].Serialize(writer);
        }
    }

    class Condition
    {
        public byte ByteLength = 1;

        public void Unserialize(Core.BinaryFileReader reader)
        {
            reader.ReadByte();
        }

        public void Serialize(Core.BinaryFileWriter writer)
        {
            writer.Write((byte)0);
        }
    }

    class Effect
    {
        public byte ByteLength = 1;

        public void Unserialize(Core.BinaryFileReader reader)
        {
            reader.ReadByte();
        }

        public void Serialize(Core.BinaryFileWriter writer)
        {
            writer.Write((byte)0);
        }
    }
}
