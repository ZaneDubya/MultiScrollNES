using Core;
using eightbit.Data.TileSetData;
using System;
using System.Collections.Generic;

namespace eightbit.Data
{
    class TileSet
    {
        private const string c_SerializeIdentifier = "TilSet";
        public const int TilesPerSet = 255;

        private byte m_PaletteZero = 0;
        private byte[] m_Palettes;
        private int[][] m_Metatiles;
        private byte[] m_Bitfields;
        private byte[] m_Attributes;
        private TileCollection m_Tilegfx;

        public byte PaletteColorZero
        {
            get { return m_PaletteZero; }
            set { m_PaletteZero = value; }
        }

        public byte GetPalette(int index)
        {
            if (index < 0 || index >= m_Palettes.Length)
                return 0;
            return m_Palettes[index];
        }

        public void SetPalette(int index, byte value)
        {
            if (index < 0 || index >= m_Palettes.Length)
                return;
            m_Palettes[index] = value;
        }

        public void SetAttribute(int index, byte value)
        {
            if (index < 0 || index >= TilesPerSet)
                return;
            m_Attributes[index] = value;
        }

        /// <summary>
        /// Returns the tile, gfxpage, and attribute for the subtile in tile (index).
        /// </summary>
        /// <param name="index"></param>
        /// <param name="subtile"></param>
        /// <returns></returns>
        public TilePageAttribute GetSubTile(int index, int subtile)
        {
            if (index < 0 || index >= TilesPerSet)
                return TilePageAttribute.Zero;
            TilePage value = m_Tilegfx.GetTileAndPageFromIndex(m_Metatiles[index][subtile]);
            TilePageAttribute value2 = new TilePageAttribute(value.Tile, value.Page, m_Attributes[index]);
            return value2;
        }

        public void SetSubTileAndPage(int index, int subtile, byte tile, byte page)
        {
            if (index < 0 || index >= TilesPerSet)
                return;
            int gfxindex = m_Tilegfx.GetIndexFromTileAndPage(tile, page, create: true);
            m_Metatiles[index][subtile] = gfxindex;
        }

        public byte GetFlags(int index)
        {
            if (index < 0 || index >= TilesPerSet)
                return 0x00;
            return m_Bitfields[index];
        }

        public void ToggleFlag(int index, int flag_index)
        {
            if (index < 0 || index >= TilesPerSet)
                return;
            if (flag_index < 0 || flag_index >= 8)
                return;
            m_Bitfields[index] ^= (byte)(0x01 << flag_index);
        }

        public void SetFlags(int index, byte value)
        {
            if (index < 0 || index >= TilesPerSet)
                return;
            m_Bitfields[index] = value;
        }

        public TileSet()
        {
            m_PaletteZero = 0;
            m_Palettes = new byte[4] { 0, 0, 0, 0 };

            m_Tilegfx = new TileCollection();
            int tile = m_Tilegfx.GetIndexFromTileAndPage(0, 0, create: true);

            m_Metatiles = new int[Data.TileSet.TilesPerSet][];
            m_Bitfields = new byte[Data.TileSet.TilesPerSet];
            m_Attributes = new byte[Data.TileSet.TilesPerSet];
            for (int i = 0; i < TileSet.TilesPerSet; i++)
            {
                m_Metatiles[i] = new int[4] { tile, tile, tile, tile };
                m_Bitfields[i] = 0;
            }
        }

        public bool Unserialize(BinaryFileReader reader)
        {
            string id = reader.ReadString();
            if (id != c_SerializeIdentifier)
                return false;
            int version = reader.ReadInt();
            if (version >= 0)
            {
                // version 0
                // 16 byte header
                m_PaletteZero = reader.ReadByte();
                for (int i = 0; i < 4; i++)
                    m_Palettes[i] = reader.ReadByte();
                for (int i = 0; i < 3; i++)
                    reader.ReadByte();

                m_Tilegfx.Reset();
                m_Tilegfx.NextIndex = reader.ReadInt();
                int count = reader.ReadInt();
                for (int i = 0; i < count; i++)
                {
                    int index = reader.ReadInt();
                    byte tile = reader.ReadByte();
                    byte page = reader.ReadByte();
                    m_Tilegfx.AddIndexTilePage(index, tile, page);
                }

                for (int i = 0; i < TilesPerSet; i++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        m_Metatiles[i][j] = reader.ReadInt();
                    }
                    m_Bitfields[i] = reader.ReadByte();
                    m_Attributes[i] = reader.ReadByte();
                }
            }
            return true;
        }

        public void Serialize(BinaryFileWriter writer)
        {
            m_Tilegfx.Consolidate(m_Metatiles);

            writer.Write(c_SerializeIdentifier);
            writer.Write((int)0); // version

            writer.Write(m_PaletteZero);
            for (int i = 0; i < 4; i++)
                writer.Write(m_Palettes[i]);
            for (int i = 0; i < 3; i++)
                writer.Write((byte)0);

            writer.Write((int)m_Tilegfx.CurrentIndex);
            writer.Write((int)m_Tilegfx.Count);
            for (int i = 0; i < m_Tilegfx.Count; i++)
            {
                TilePage value = m_Tilegfx[i];
                writer.Write((int)i);
                writer.Write((byte)value.Tile);
                writer.Write((byte)value.Page);
            }

            for (int i = 0; i < TilesPerSet; i++)
            {
                for (int j = 0; j < 4; j++)
                    writer.Write((int)m_Metatiles[i][j]);
                writer.Write((byte)m_Bitfields[i]);
                writer.Write((byte)m_Attributes[i]);
            }
        }

        public void Export(BinaryFileWriter writer)
        {
            m_Tilegfx.Consolidate(m_Metatiles);

            for (int i = 0; i < TileSet.TilesPerSet; i++)
                writer.Write((byte)(m_Bitfields[i]));
            writer.Write((byte)208);

            for (int i = 0; i < TileSet.TilesPerSet; i++)
                writer.Write((byte)(m_Attributes[i]));
            writer.Write((byte)m_PaletteZero);

            for (int i = 0; i < TileSet.TilesPerSet; i++)
            {
                TilePage value = m_Tilegfx.GetTileAndPageFromIndex(m_Metatiles[i][0]);
                writer.Write((byte)value.Tile);
            }
            writer.Write((byte)m_Palettes[0]);

            for (int i = 0; i < TileSet.TilesPerSet; i++)
            {
                TilePage value = m_Tilegfx.GetTileAndPageFromIndex(m_Metatiles[i][1]);
                writer.Write((byte)value.Tile);
            }
            writer.Write((byte)m_Palettes[1]);

            for (int i = 0; i < TileSet.TilesPerSet; i++)
            {
                TilePage value = m_Tilegfx.GetTileAndPageFromIndex(m_Metatiles[i][2]);
                writer.Write((byte)value.Tile);
            }
            writer.Write((byte)m_Palettes[2]);

            for (int i = 0; i < TileSet.TilesPerSet; i++)
            {
                TilePage value = m_Tilegfx.GetTileAndPageFromIndex(m_Metatiles[i][3]);
                writer.Write((byte)value.Tile);
            }
            writer.Write((byte)m_Palettes[3]);

            byte[] lo_tiles = new byte[208];
            byte[] hi_tiles = new byte[104];

            for (int i = 0; i < 208; i++)
            {
                if (i < m_Tilegfx.Count)
                {
                    TilePage tp = m_Tilegfx.GetTileAndPageFromIndex(i);
                    lo_tiles[i] = tp.Tile;
                    if (i % 2 == 0)
                        hi_tiles[i / 2] = (byte)(tp.Page % 16);
                    else
                        hi_tiles[i / 2] = (byte)(hi_tiles[i / 2] | (byte)((tp.Page % 16) << 4));
                }
            }

            for (int i = 0; i < 208; i++)
                writer.Write((byte)lo_tiles[i]);
            for (int i = 0; i < 104; i++)
                writer.Write((byte)hi_tiles[i]);
        }

        private class TileCollection
        {
            public void Consolidate(int[][] metatiles)
            {
                // get the list of tile indexes that are actually used in this tileset.
                List<int> old_indexes = new List<int>();
                for (int i = 0; i < TileSet.TilesPerSet; i++)
                    for (int j = 0; j < 4; j++)
                    {
                        bool in_old_indexes = false;
                        for (int k = 0; k < old_indexes.Count; k++)
                            if (old_indexes[k] == metatiles[i][j])
                            {
                                in_old_indexes = true;
                                break;
                            }
                        if (!in_old_indexes)
                            old_indexes.Add(metatiles[i][j]);
                    }

                List<Tuple<int, TilePage>> tiles = new List<Tuple<int, TilePage>>();

                for (int i = 0; i < old_indexes.Count; i++)
                    tiles.Add(new Tuple<int, TilePage>(i, GetTileAndPageFromIndex(old_indexes[i])));
                m_Tiles = tiles;

                for (int i = 0; i < TileSet.TilesPerSet; i++)
                    for (int j = 0; j < 4; j++)
                        for (int k = 0; k < old_indexes.Count; k++)
                            if (old_indexes[k] == metatiles[i][j])
                            {
                                metatiles[i][j] = k;
                                break;
                            }
            }

            private List<Tuple<int, TilePage>> m_Tiles;

            private int m_NextIndex = 0;
            public int NextIndex
            {
                set
                {
                    m_NextIndex = value;
                }
                get
                {
                    return m_NextIndex++;
                }
            }

            public int CurrentIndex
            {
                get { return m_NextIndex; }
            }

            public TileCollection()
            {
                m_Tiles = new List<Tuple<int, TilePage>>();
            }

            public TilePage GetTileAndPageFromIndex(int index)
            {
                for (int i = 0; i < Count; i++)
                    if (m_Tiles[i].Item1 == index)
                        return m_Tiles[i].Item2;
                return TilePage.Zero;
            }

            public int GetIndexFromTileAndPage(byte tile, byte page, bool create = false)
            {
                for (int i = 0; i < m_Tiles.Count; i++)
                    if (m_Tiles[i].Item2.Tile == tile && m_Tiles[i].Item2.Page == page)
                        return m_Tiles[i].Item1;
                if (create)
                {
                    int new_index = NextIndex;
                    this.AddIndexTilePage(new_index, tile, page);
                    return new_index;
                }
                return -1;
            }

            public int Count
            {
                get
                {
                    return m_Tiles.Count;
                }
            }

            public void Reset()
            {
                m_Tiles.Clear();
                m_NextIndex = 0;
            }

            /// <summary>
            /// Returns the tile and page indexes for the tile with this index.
            /// </summary>
            /// <returns></returns>
            public TilePage this[int index]
            {
                get
                {
                    return GetTileAndPageFromIndex(index);
                }
            }

            public void AddIndexTilePage(int index, byte tile, byte page)
            {
                m_Tiles.Add(new Tuple<int, TilePage>(index, new TilePage(tile, page)));
            }
        }
    }
}
