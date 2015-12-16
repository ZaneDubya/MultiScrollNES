using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Core.GUI.Framework;

namespace eightbit.GUI.Elements.Renderers
{
    class TilesetAIORenderRule : RenderRule
    {
        private Rectangle _area;
        public override Rectangle Area
        {
            get { return _area; }
            set { _area = value; }
        }

        public int Height { get { return _area.Height; } }
        public int Width { get { return _area.Width; } }

        private TilesetAllInOne m_Parent;

        public TilesetAIORenderRule(TilesetAllInOne parent)
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

        }

        public override void Draw()
        {
            int metatilesperrow = m_Parent.TilesPerRow;
            int tilewidth = (Area.Width / (metatilesperrow * 2));
            int tileheight = (Area.Height / ((256 / metatilesperrow) * 2));
            for (int tile = 0; tile < Data.TileSet.TilesPerSet; tile++)
            {
                for (int i = 0; i < 4; i++)
                {
                    Tuple<byte, Texture2D> tuple = m_Parent.GetTile(tile, i);
                    if (tuple == null || tuple.Item2 == null)
                        break;
                    int u = tuple.Item1 % 16 * 8;
                    int v = (tuple.Item1 / 16) * 8;
                    RenderManager.SpriteBatch.GUIDrawSprite(tuple.Item2,
                        new Rectangle(Area.X + ((tile % metatilesperrow) * 2 + (i % 2)) * tilewidth, Area.Y + ((tile / metatilesperrow) * 2 + (i / 2)) * tileheight, tilewidth, tileheight),
                        new Rectangle(u, v, 8, 8), Palettized: true, Palette: m_Parent.GetAttribute(tile));
                }
            }

            Color border = Color.Black;
            RenderManager.SpriteBatch.DrawRectangle(
                new Vector3(Area.X - 1, Area.Y - 1, 0),
                new Vector2(Area.Width + 2, Area.Height + 2),
                border);

            border = new Color(255, 255, 255);
            int w = 16, h = 16;
            RenderManager.SpriteBatch.DrawRectangle(
                new Vector3(Area.X + (m_Parent.SelectedTile % metatilesperrow) * w - 1, Area.Y + (m_Parent.SelectedTile / metatilesperrow) * h - 1, 0),
                new Vector2(18, 18),
                border);
        }

        public override void DrawNoClipping() { }
    }
}
