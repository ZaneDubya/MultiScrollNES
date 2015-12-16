using System;
using Core.GUI.Content;
using Core.GUI.Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace eightbit.GUI.Elements.Renderers
{
    class MetaTileRenderRule : RenderRule
    {
        private IconRenderer _checkbox, _checkbox_checked;

        private Rectangle _area;
        public override Rectangle Area
        {
            get { return _area; }
            set { _area = value; }
        }

        public int Height { get { return _area.Height; } }
        public int Width { get { return _area.Width; } }

        private MetaTile m_Parent;

        public MetaTileRenderRule(MetaTile parent)
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
            _checkbox = LoadRenderer<IconRenderer>(Skin, "checkbox_icon");
            _checkbox_checked = LoadRenderer<IconRenderer>(Skin, "checkbox_icon_checked");
        }

        public override void Draw()
        {
            for (int i = 0; i < m_Parent.TilesWidth * m_Parent.TilesHeight; i++)
            {
                Tuple<byte, Texture2D, bool, bool> tuple = m_Parent.GetTile(i);
               
                bool h = tuple.Item3, v = tuple.Item4;
                SpriteEffects effect = (h ? SpriteEffects.FlipHorizontally : 0) | (v ? SpriteEffects.FlipVertically : 0);

                if (tuple == null || tuple.Item2 == null)
                    break;
                int x_tile = tuple.Item1 % 16 * 8;
                int y_tile = (tuple.Item1 / 16) * 8;
                RenderManager.SpriteBatch.GUIDrawSprite(tuple.Item2,
                    new Rectangle(
                        Area.X + (i % m_Parent.TilesWidth) * (Area.Width / m_Parent.TilesWidth),
                        Area.Y + (i / m_Parent.TilesWidth) * (Area.Height / m_Parent.TilesHeight),
                        Area.Width / m_Parent.TilesWidth,
                        Area.Height / m_Parent.TilesHeight),
                        new Rectangle(x_tile, y_tile, 8, 8), effects: effect, Palettized: true, Palette: m_Parent.Attribute);
            }

            if (m_Parent.DrawFlags)
            {
                for (int i = 0; i < 8; i++)
                {
                    if ((m_Parent.Flags & (0x01 << i)) != 0)
                    {
                        _checkbox_checked.Render(RenderManager.SpriteBatch, new Rectangle(
                            Area.X + (i % 3) * 16, Area.Y + (i / 3) * 16, 16, 16));
                    }
                    else
                    {
                        _checkbox.Render(RenderManager.SpriteBatch, new Rectangle(
                            Area.X + (i % 3) * 16, Area.Y + (i / 3) * 16, 16, 16));
                    }
                }
            }

            Color border = Color.Black;
            RenderManager.SpriteBatch.DrawRectangle(
                new Vector3(Area.X - 1, Area.Y - 1, 0),
                new Vector2(Area.Width + 2, Area.Height + 2),
                border);
        }

        public override void DrawNoClipping() { }
    }
}
