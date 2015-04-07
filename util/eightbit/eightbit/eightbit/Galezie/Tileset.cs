using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eightbit.Galezie
{
    class Tileset
    {
        private byte[] _palette = new byte[16];
        private byte[] _gfxSets = new byte[4];
        private byte[] _tilesetReuse = new byte[4];
        private byte[] _reserved = new byte[8];
        private byte[] _meta_bitfields = new byte[80];
        private byte[] _meta_attributes = new byte[80];
        private byte[] _meta_gfxUL = new byte[80];
        private byte[] _meta_gfxUR = new byte[80];
        private byte[] _meta_gfxLL = new byte[80];
        private byte[] _meta_gfxLR = new byte[80];

        public void Serialize(Core.GenericWriter writer)
        {
            for (int i = 0; i < 16; i++)
                writer.Write(_palette[i]);
            for (int i = 0; i < 4; i++)
                writer.Write(_gfxSets[i]);
            for (int i = 0; i < 4; i++)
                writer.Write(_tilesetReuse[i]);
            for (int i = 0; i < 8; i++)
                writer.Write(_reserved[i]);

            for (int i = 0; i < 80; i++)
                writer.Write(_meta_bitfields[i]);
            for (int i = 0; i < 80; i++)
                writer.Write(_meta_attributes[i]);
            for (int i = 0; i < 80; i++)
                writer.Write(_meta_gfxUL[i]);
            for (int i = 0; i < 80; i++)
                writer.Write(_meta_gfxUR[i]);
            for (int i = 0; i < 80; i++)
                writer.Write(_meta_gfxLL[i]);
            for (int i = 0; i < 80; i++)
                writer.Write(_meta_gfxLR[i]);
        }

        public void Unserialize(Core.GenericReader reader)
        {
            for (int i = 0; i < 16; i++)
                _palette[i] = reader.ReadByte();
            for (int i = 0; i < 4; i++)
                _gfxSets[i] = reader.ReadByte();
            for (int i = 0; i < 4; i++)
                _tilesetReuse[i] = reader.ReadByte();
            for (int i = 0; i < 8; i++)
                 _reserved[i] = reader.ReadByte();

            for (int i = 0; i < 80; i++)
                 _meta_bitfields[i] = reader.ReadByte();
            for (int i = 0; i < 80; i++)
                 _meta_attributes[i] = reader.ReadByte();
            for (int i = 0; i < 80; i++)
                 _meta_gfxUL[i] = reader.ReadByte();
            for (int i = 0; i < 80; i++)
                 _meta_gfxUR[i] = reader.ReadByte();
            for (int i = 0; i < 80; i++)
                 _meta_gfxLL[i] = reader.ReadByte();
            for (int i = 0; i < 80; i++)
                 _meta_gfxLR[i] = reader.ReadByte();
        }
    }
}
