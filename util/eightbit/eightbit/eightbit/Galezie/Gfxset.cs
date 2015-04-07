using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eightbit.Galezie
{
    class Gfxset
    {
        private byte[] _indexesLo = new byte[48];
        private byte[] _indexesHi = new byte[24];

        public int this[int index]
        {
            get
            {
                if (index > 47 || index < 0)
                    return -1;

                int gfx = _indexesLo[index];
                if (index % 2 == 0)
                    gfx += (_indexesHi[index >> 1] & 0x0F) << 8;
                else
                    gfx += (_indexesHi[index >> 1] & 0xF0) << 4;
                return gfx;
            }
            set
            {
                if (index > 47 || index < 0)
                    return;
                if (value > 4095 || value < 0)
                    return;

                _indexesLo[index] = (byte)(value & 0x00FF);

                int ghi = _indexesHi[index >> 1];
                if (index % 2 == 0)
                {
                    ghi = ghi & 0xF0;
                    ghi += (byte)(value >> 8);
                }
                else
                {
                    ghi = ghi & 0x0F;
                    ghi += (byte)((value & 0x0F00) >> 4);
                }
                _indexesHi[index >> 1] = (byte)ghi;
            }
        }

        public void Test()
        {
            for (int i = 0; i < 48; i++)
                for (int j = 0; j < 4096; j++)
                {
                    this[i] = j;
                    if (this[i] != j)
                        throw new Exception();
                }
        }

        public void Serialize(Core.GenericWriter writer)
        {
            for (int i = 0; i < 48; i++)
                writer.Write(_indexesLo[i]);
            for (int i = 0; i < 24; i++)
                writer.Write(_indexesHi[i]);
        }

        public void Unserialize(Core.GenericReader reader)
        {
            for (int i = 0; i < 48; i++)
                _indexesLo[i] = reader.ReadByte();
            for (int i = 0; i < 24; i++)
                _indexesHi[i] = reader.ReadByte();
        }
    }
}
