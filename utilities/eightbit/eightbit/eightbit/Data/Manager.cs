using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using eightbit.Data.TileSetData;

namespace eightbit.Data
{
    class Manager
    {
        private const string c_SerializeIdentifier = "State";

        private Palettes m_Palettes;
        private TileGfx m_TileGfx;
        private TileSets m_TileSets;
        private Chunks m_Chunks;
        private Maps m_Maps;
        private Sprites m_Sprites;

        private int m_MaxSaveIndex = 8191;

        public int MaxSaveIndex
        {
            get { return m_MaxSaveIndex; }
        }

        public Manager()
        {
            m_Palettes = new Palettes();
            m_TileGfx = new TileGfx();
            m_TileSets = new TileSets();
            m_Chunks = new Chunks();
            m_Maps = new Maps();
            m_Sprites = new Sprites();
        }

        public Manager(string path)
            :this()
        {
            if (!Unserialize(path))
                throw new Exception("Error loading state.");
        }

        public bool Unserialize(string path)
        {
            if (File.Exists(path))
            {
                using (FileStream bin = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    Core.BinaryFileReader reader = new Core.BinaryFileReader(new BinaryReader(bin));
                    string id = reader.ReadString();
                    if (id != c_SerializeIdentifier)
                        return false;
                    int version = reader.ReadInt();

                    if (version > 0)
                    {
                        return false;
                    }
                    else
                    {
                        if (!m_Palettes.Unserialize(reader))
                            return false;
                        if (!m_TileGfx.Unserialize(reader))
                            return false;
                        if (!m_TileSets.Unserialize(reader))
                            return false;
                        if (!m_Chunks.Unserialize(reader))
                            return false;
                        if (!m_Maps.Unserialize(reader))
                            return false;
                        if (!m_Sprites.Unserialize(reader))
                            return false;
                        fixData();
                        return true;
                    }
                }
            }
            else
            {
                return false;
            }
        }

        public void Serialize(string path)
        {
            Core.BinaryFileWriter writer = new Core.BinaryFileWriter(path, false);
            writer.Write(c_SerializeIdentifier);
            writer.Write((int)0); // version
            m_Palettes.Serialize(writer);
            m_TileGfx.Serialize(writer);
            m_TileSets.Serialize(writer);
            m_Chunks.Serialize(writer);
            m_Maps.Serialize(writer);
            m_Sprites.Serialize(writer);
            writer.Close();
        }

        public void Export(string directory)
        {
            Core.BinaryFileWriter writer;

            writer = new Core.BinaryFileWriter(directory + "palettes.dat", false);
            m_Palettes.Export(writer);
            writer.Close();

            writer = new Core.BinaryFileWriter(directory + "tilegfx.dat", false);
            m_TileGfx.Export(writer);
            writer.Close();

            writer = new Core.BinaryFileWriter(directory + "tilesets.dat", false);
            m_TileSets.Export(writer);
            writer.Close();

            writer = new Core.BinaryFileWriter(directory + "chunks.dat", false);
            m_Chunks.Export(writer);
            writer.Close();

            Core.BinaryFileWriter headers = new Core.BinaryFileWriter(directory + "map_hdrs.dat", false);
            Core.BinaryFileWriter pointers = new Core.BinaryFileWriter(directory + "map_ptrs.dat", false);
            Core.BinaryFileWriter data = new Core.BinaryFileWriter(directory + "map_data.dat", false);
            m_Maps.Export(headers, pointers, data);
            headers.Close();
            pointers.Close();
            data.Close();

            headers = new Core.BinaryFileWriter(directory + "spr_hdrs.dat", false);
            data = new Core.BinaryFileWriter(directory + "spr_data.dat", false);
            m_Sprites.Export(headers, data);
            headers.Close();
            data.Close();

            System.Windows.Forms.MessageBox.Show("Export to " + directory);
        }

        public Palettes Palettes
        {
            get { return m_Palettes; }
        }

        public TileSets TileSets
        {
            get { return m_TileSets; }
        }

        public TileGfx TileGfx
        {
            get { return m_TileGfx; }
        }

        public Chunks Chunks
        {
            get { return m_Chunks; }
        }

        public Maps Maps
        {
            get { return m_Maps; }
        }

        public Sprites Sprites
        {
            get { return m_Sprites; }
        }

        // Control

        public void ReplaceChunk(int chunk_index, int replace_with)
        {
            if (replace_with == chunk_index)
                return;

            if (replace_with > chunk_index)
                replace_with -= 1;

            int[] replaces = new int[Chunks.Count];
            for (int i = 0; i < Chunks.Count; i++)
            {
                if (i < chunk_index)
                    replaces[i] = i;
                else if (i == chunk_index)
                    replaces[i] = replace_with;
                else if (i > chunk_index)
                    replaces[i] = i - 1;
            }

            foreach (Map m in Maps.AllMaps)
            {
                foreach (SuperChunk s in m.SuperChunks)
                {
                    s.Chunk0 = replaces[s.Chunk0];
                    s.Chunk1 = replaces[s.Chunk1];
                    s.Chunk2 = replaces[s.Chunk2];
                    s.Chunk3 = replaces[s.Chunk3];
                }
            }

            Chunks.RemoveChunk(chunk_index);
        }

        private void fixData()
        {
            for (int ts = 0; ts < m_TileSets.Count; ts++)
            {
                for (int i = 0; i < TileSet.TilesPerSet; i++)
                    for (int j = 0; j < 4; j++)
                    {
                        TilePageAttribute tpa = m_TileSets[ts].GetSubTile(i, j);
                        if (tpa.Page >= m_TileGfx.PageCount)
                            m_TileSets[ts].SetSubTileAndPage(i, j, 0, 0);
                    }
            }
        }
    }
}
