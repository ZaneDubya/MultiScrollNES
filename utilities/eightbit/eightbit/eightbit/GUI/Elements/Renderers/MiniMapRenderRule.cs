using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Core.GUI.Framework;
using System.Collections.Generic;
using eightbit.Data.TileSetData;

namespace eightbit.GUI.Elements.Renderers
{
    class MiniMapRenderRule : RenderRule
    {
        private Rectangle _area;
        public override Rectangle Area
        {
            get { return _area; }
            set { _area = value; }
        }

        public int Height { get { return _area.Height; } }
        public int Width { get { return _area.Width; } }

        private int m_TextureDimension;
        public int TextureDimension
        {
            get { return m_TextureDimension; }
            set
            {
                if (m_TextureDimension != value)
                {
                    m_TextureDimension = value;
                    createTexture();
                }
            }
        }

        private const string c_RenderInProgress = "Rendering Minimap...";
        private Font m_textRenderer;
        private MiniMap m_Parent;
        private Texture2D m_Texture;
        public Texture2D Texture
        {
            get { return m_Texture; }
        }

        public MiniMapRenderRule(MiniMap parent)
        {
            m_Parent = parent;
        }

        public override void SetSize(int w, int h)
        {
            _area.Width = w;
            _area.Height = h;
        }

        protected override void LoadRenderers()
        {
            m_textRenderer = RenderManager.SpriteFonts[FontName];
        }

        public override void Draw()
        {
            if (m_Parent.RenderInProcess)
                m_textRenderer.Render(RenderManager.SpriteBatch, c_RenderInProgress, Area,
                    TextHorizontal.CenterAligned, TextVertical.CenterAligned);
            else
                RenderManager.SpriteBatch.GUIDrawSprite(m_Texture, Area, Palettized: true);

            Color border = Color.Black;
            RenderManager.SpriteBatch.DrawRectangle(
                new Vector3(Area.X - 1, Area.Y - 1, 0),
                new Vector2(Area.Width + 2, Area.Height + 2),
                border);

            if (State.MapScreen_WindowArea != Point.Zero)
            {
                float x = Area.Width * ((float)State.MapScreen_Scroll.X / State.MapScreen_MapArea.X);
                float y = Area.Height * ((float)State.MapScreen_Scroll.Y / State.MapScreen_MapArea.Y);
                float w = Area.Width * ((float)State.MapScreen_WindowArea.X / State.MapScreen_MapArea.X);
                float h = Area.Height * ((float)State.MapScreen_WindowArea.Y / State.MapScreen_MapArea.Y);
                border = Color.White;
                RenderManager.SpriteBatch.DrawRectangle(
                    new Vector3((int)(Area.X + x), (int)(Area.Y + y), 0),
                    new Vector2(w, h),
                    border);
            }
        }

        public override void DrawNoClipping() { }

        private Dictionary<int, int[]> m_TilesToBytes;

        public void WriteChunk(int x, int y, Data.Chunk chunk, Data.TileSet tileset, Data.TileGfx gfx)
        {
            if (m_TilesToBytes == null)
                m_TilesToBytes = new Dictionary<int, int[]>();

            for (int i = 0; i < 4; i++)
            {
                int[] chunksums = new int[16];
                for (int iy = 0; iy < 4; iy++)
                {
                    for (int ix = 0; ix < 4; ix++)
                    {
                        int metatile = chunk[(iy + (i / 2) * 2) * 8 + (ix + (i % 2) * 2)];
                        if (!m_TilesToBytes.ContainsKey(metatile))
                        {
                            int[] tilesums = new int[16];
                            for (int j = 0; j < 4; j++)
                            {
                                TilePageAttribute value = tileset.GetSubTile(metatile, j);
                                byte[] tile = gfx.GetTile(value.Page, value.Tile);
                                for (int k = 0; k < 16; k++)
                                {
                                    int t = (tile[k] & 0x03);
                                    tilesums[t + (t == 0 ? 0 : value.Attribute * 4)]++;
                                    t = ((tile[k] >> 2) & 0x03);
                                    tilesums[t + (t == 0 ? 0 : value.Attribute * 4)]++;
                                    t = ((tile[k] >> 4) & 0x03);
                                    tilesums[t + (t == 0 ? 0 : value.Attribute * 4)]++;
                                    t = ((tile[k] >> 6) & 0x03);
                                    tilesums[t + (t == 0 ? 0 : value.Attribute * 4)]++;
                                }
                            }
                            m_TilesToBytes.Add(metatile, tilesums);
                        }
                        for (int j = 0; j < 16; j++)
                            chunksums[j] += m_TilesToBytes[metatile][j];
                    }
                    uint[] mostCommonPixel = new uint[1] { (uint)GreatestIndexInArray(chunksums) };
                    m_Texture.SetData<uint>(0, new Rectangle(x * 2 + (i % 2), y * 2 + (i / 2), 1, 1), mostCommonPixel, 0, 1);
                }
            }
        }

        private int GreatestIndexInArray(int[] array)
        {
            int max = array[0];
            int index = 0;
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] >= max)
                {
                    max = array[i];
                    index = i;
                }
            }
            return index;
        }

        private void createTexture()
        {
            uint[] pixels = new uint[TextureDimension * TextureDimension];
            m_Texture = Core.Library.CreateTexture(TextureDimension, TextureDimension);
            m_Texture.SetData<uint>(pixels);
        }
    }
}
