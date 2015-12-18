using eightbit.Data.SpriteData;
using System;
using System.Collections.Generic;

namespace eightbit.Data
{
    class Sprites
    {
        private const string c_SerializeIdentifier = "Sprites";
        private List<Sprite> m_Sprites = new List<Sprite>();

        public Sprites()
        {

        }

        public int Count
        {
            get { return m_Sprites.Count; }
        }

        public Sprite this[int index]
        {
            get
            {
                if (index < 0 || index >= m_Sprites.Count)
                    return null;
                return m_Sprites[index];
            }
        }

        public void AddSprite()
        {
            Sprite s = new Sprite();
            m_Sprites.Add(s);
        }

        public void RemoveLastSprite()
        {
            if (m_Sprites.Count > 0)
                m_Sprites.RemoveAt(m_Sprites.Count - 1);
        }

        public bool Unserialize(Core.BinaryFileReader reader)
        {
            string id = reader.ReadString();
            if (id != c_SerializeIdentifier)
                return false;
            int version = reader.ReadInt();

            // version 0
            int count = reader.ReadInt();
            for (int i = 0; i < count; i++)
            {
                Sprite sprite = new Sprite();
                sprite.Unserialize(reader, version);
                m_Sprites.Add(sprite);
            }

            return true;
        }

        public void Serialize(Core.BinaryFileWriter writer)
        {
            int version = 1;

            writer.Write(c_SerializeIdentifier);
            writer.Write((int)version); // version
            writer.Write(Count);
            for (int i = 0; i < Count; i++)
            {
                m_Sprites[i].Serialize(writer, version);
            }
        }

        public void Export(Core.BinaryFileWriter header_writer, Core.BinaryFileWriter tiledata_writer)
        {
            int maxTileDataOffset = 1024 * 16;
            int currentTiledataOffset = 1024; // which would be 4 * 256 - all possible actor headers!

            for (int i = 0; i < Count; i++)
            {
                Tuple<byte, byte>[] tile_transform_table = m_Sprites[i].TileTransformTable;

                ushort tiledataOffset = CreateTileDataOffset(ref currentTiledataOffset, m_Sprites[i], tile_transform_table, maxTileDataOffset);

                // sprite header data: 4b per sprite. REFERENCE ref.data\Engines\Sprite Data.txt
                byte dataByte = (byte)m_Sprites[i].DataByte;
                byte tileByte = (byte)createTileDataByte(tile_transform_table);

                header_writer.Write(tiledataOffset);
                header_writer.Write(dataByte); // wTss xfff
                header_writer.Write(tileByte); // yxMM cccc

                // metasprite/chr tile index data.
                m_Sprites[i].ExportMetaSprites(tiledata_writer, tile_transform_table);
                for (int j = 0; j < tile_transform_table.Length; j++)
                {
                    tiledata_writer.Write((byte)tile_transform_table[j].Item1); // tile
                    tiledata_writer.Write((byte)tile_transform_table[j].Item2); // page
                    
                }
            }
        }

        private ushort CreateTileDataOffset(ref int offset, Sprite sprite, Tuple<byte, byte>[] sprite_transform_table, int max_offset)
        {
            if (offset >= max_offset)
                throw new Exception(string.Format("Bad Sprite MetaTile offset size. Too large for specified max_offset size of {0}.", max_offset));

            ushort data = (ushort)offset;
            offset += sprite.MetaDataSize + sprite_transform_table.Length * 2;

            return data;
        }

        private byte createTileDataByte(Tuple<byte, byte>[] tile_transform_table)
        {
            byte tile_count = (byte)(tile_transform_table.Length + 1);
            byte data = (byte)(tile_count & 0xf);
            data |= (byte)((tile_count & 0x30) << 2);
            return data;
        }

        public Sprite[] ToArray()
        {
            return m_Sprites.ToArray();
        }
    }
}
