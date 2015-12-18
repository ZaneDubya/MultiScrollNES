using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace eightbit
{
    class State
    {
        public static Data.Manager Data;
        public static NES.Palette Palette;

        public static int SelectedColor = 0;
        public static int SelectedTile = 0;
        public static int SelectedPage = 0;
        public static int SelectedTileset = 0;
        public static int SelectedPalette = 0;
        public static int SelectedMetatile = 0;
        public static int SelectedSprite = 0;

        public static int SelectedMap = 0;
        public static Point MapScreen_Scroll = Point.Zero;
        public static Point MapScreen_MapArea = Point.Zero;
        public static Point MapScreen_WindowArea = Point.Zero;

        // clipboard
        private static Clipboard m_Clipboard;
        public static Clipboard Clipboard
        {
            get
            {
                if (m_Clipboard == null)
                    m_Clipboard = new Clipboard();
                return m_Clipboard;
            }
        }

        private static int m_SChunk;
        public static int SelectedChunk
        {
            get { return m_SChunk; }
            set
            {
                m_SChunk = value;
            }
        }

        public static void SetPixel(int page, int tile, int pixel, byte color)
        {
            State.Data.TileGfx.SetPixel(page, tile, pixel, color);
            GfxPage(page).WriteTile(tile, State.Data.TileGfx.GetTile(State.SelectedPage, State.SelectedTile));
        }

        private static Dictionary<int, Galezie.Gfxbank> m_Gfxbanks;

        public static Galezie.Gfxbank GfxPage(int index)
        {
            if (index < 0 || index >= Data.TileGfx.PageCount)
                return null;
            if (m_Gfxbanks == null)
                m_Gfxbanks = new Dictionary<int, Galezie.Gfxbank>();
            if (!m_Gfxbanks.ContainsKey(index))
            {
                Galezie.Gfxbank bank = new Galezie.Gfxbank(index);
                bank.WritePage(Data.TileGfx.GetPage(index));
                m_Gfxbanks.Add(index, bank);
            }
            return m_Gfxbanks[index];
        }

        public static void InvalidateGfxPages()
        {
            if (m_Gfxbanks != null)
                m_Gfxbanks.Clear();
        }

        public static bool LoadData()
        {
            if (!System.IO.File.Exists(DataPath + "default.dat"))
            {
                if (System.IO.File.Exists(DefaultPath + "default.dat"))
                    System.IO.File.Copy(DefaultPath + "default.dat", DataPath + "default.dat");
            }

            if (State.Data.Unserialize(DataPath + "default.dat"))
            {
                InvalidateGfxPages();
                return true;
            }
            return false;
        }

        public static void SaveData()
        {
            string path = DataPath;
            State.Data.Serialize(path + "default.dat");
        }

        public static void ExportData()
        {
            string path = ExportPath;
            State.Data.Export(path + "\\");
        }

        public static string DataPath
        {
            get
            {
                System.IO.DirectoryInfo dir;
                string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

                path += "\\My Games";
                dir = System.IO.Directory.CreateDirectory(path);

                path += "\\eightbit";
                dir = System.IO.Directory.CreateDirectory(path);

                return path + "\\";
            }
        }

        public static string DefaultPath
        {
            get
            {
                string path = "..\\resources\\eightbit\\";
                return path;
            }
        }

        public static string ExportPath
        {
            get
            {
                System.IO.DirectoryInfo dir;
                string path = DataPath;

                path += "\\Export";
                dir = System.IO.Directory.CreateDirectory(path);

                return path + "\\";
            }
        }
    }
}
