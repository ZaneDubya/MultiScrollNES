using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eightbit.Data.TileSetData
{
    public struct TilePage
    {
        public readonly byte Tile, Page;
        public TilePage(byte t, byte p)
        {
            Tile = t;
            Page = p;
        }

        public static TilePage Zero
        {
            get
            {
                return new TilePage(0, 0);
            }
        }
    }
}
