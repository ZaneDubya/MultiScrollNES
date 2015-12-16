using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eightbit.Galezie
{
    class Manager
    {
        public Manager()
        {
            Gfxset gfx = new Gfxset();
            gfx.Test();
            Gfxbank gfxb = new Gfxbank(0);
            gfxb.Test();
        }
    }
}
