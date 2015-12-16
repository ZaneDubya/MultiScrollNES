using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Core.GUI.Framework;

namespace eightbit.GUI.Elements.Renderers
{
    class ChunkElementRenderRule : RenderRule
    {
        private Rectangle _area;
        public override Rectangle Area
        {
            get { return _area; }
            set { _area = value; }
        }

        public int Height { get { return _area.Height; } }
        public int Width { get { return _area.Width; } }

        private bool m_DrawBorders = true;
        public bool DrawBorders
        {
            get { return m_DrawBorders; }
            set { m_DrawBorders = value; }
        }

        private Font _textRenderer;
        private ChunkElement m_Parent;

        public ChunkElementRenderRule(ChunkElement parent)
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
            _textRenderer = RenderManager.SpriteFonts[FontName];
        }

        public override void Draw()
        {
            int metatilesperrow = m_Parent.TilesPerRow;
            int tilewidth = (Area.Width / (metatilesperrow * 2));
            int tileheight = (Area.Height / ((64 / metatilesperrow) * 2));
            for (int tile = 0; tile < Data.TileSet.TilesPerSet; tile++)
            {
                for (int i = 0; i < 4; i++)
                {
                    Tuple<byte, Texture2D> tuple = m_Parent.GetTileIndexAndTexture(tile, i);
                    if (tuple == null || tuple.Item2 == null)
                        break;
                    int u = tuple.Item1 % 16 * 8;
                    int v = (tuple.Item1 / 16) * 8;
                    RenderManager.SpriteBatch.GUIDrawSprite(tuple.Item2,
                        new Rectangle(Area.X + ((tile % metatilesperrow) * 2 + (i % 2)) * tilewidth, Area.Y + ((tile / metatilesperrow) * 2 + (i / 2)) * tileheight, tilewidth, tileheight),
                        new Rectangle(u, v, 8, 8), Palettized: true, Palette: m_Parent.GetAttribute(tile));
                }
            }

            if (m_Parent.IsHover && m_Parent.SelectionType != ChunkElement.SelectType.None)
            {
                Color hover = Color.White;
                Vector3 position = Vector3.Zero;
                Vector2 area = Vector2.Zero;

                int tilesperrow = m_Parent.TilesPerRow * (m_Parent.SelectionType == ChunkElement.SelectType.ByTile ? 2 : 1);
                switch (m_Parent.SelectionType)
                {
                    case ChunkElement.SelectType.ByTile:
                    case ChunkElement.SelectType.ByMetatile:
                        area = new Vector2(Area.Width / tilesperrow, Area.Height / tilesperrow);
                        position = new Vector3((m_Parent.HoverTile % tilesperrow) * area.X + Area.X, (m_Parent.HoverTile / tilesperrow) * area.Y + Area.Y, 0);
                        break;
                    case ChunkElement.SelectType.ByChunk:
                        area = new Vector2(Area.Width, Area.Height);
                        position = new Vector3(Area.X, Area.Y, 0);
                        break;
                }
                RenderManager.SpriteBatch.DrawRectangle(position, area, hover);
            }

            if (m_Parent.ChunkIndex != -1)
            {
                _textRenderer.Render(RenderManager.SpriteBatch, m_Parent.IsChunkIndexXY ? string.Format("{0},{1}", m_Parent.ChunkIndexXY.X, m_Parent.ChunkIndexXY.Y) : m_Parent.ChunkIndex.ToString(), SafeArea);
            }
        }

        public override void DrawNoClipping()
        {
            if (m_DrawBorders)
            {
                Color border = Color.Black;
                RenderManager.SpriteBatch.DrawRectangle(
                    new Vector3(Area.X - 1, Area.Y - 1, 0),
                    new Vector2(Area.Width + 2, Area.Height + 2),
                    border);
            }
        }
    }
}
