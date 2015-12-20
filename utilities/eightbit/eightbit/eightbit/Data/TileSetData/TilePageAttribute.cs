using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eightbit.Data.TileSetData
{
    public struct TilePageAttribute
    {
        public readonly byte Tile, Page, Attribute;
        public TilePageAttribute(byte t, byte p, byte a)
        {
            Tile = t;
            Page = p;
            Attribute = a;
        }

        public static TilePageAttribute Zero
        {
            get
            {
                return new TilePageAttribute(0, 0, 0);
            }
        }
    }
}
