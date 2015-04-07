using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SBX = Core.Graphics.SpriteBatchExtended;

namespace Core.GUI.Framework
{
    public class Font
    {
        public SpriteFont SpriteFont { get; set; }

        public Color Color { get; set; }

        public Font(SpriteFont font, Color color)
        {
            SpriteFont = font;
            Color = color;
        }

        public void Render(SBX spriteBatch, string value, Rectangle renderArea, TextHorizontal h = TextHorizontal.LeftAligned, TextVertical v = TextVertical.TopAligned, Color? color = null)
        {
            if (value == null)
                return;

            if (color == null)
                color = Color;

            Vector2 location = new Vector2(renderArea.X, renderArea.Y);
            Vector2 size = SpriteFont.MeasureString(value);

            switch (h)
            {
                case TextHorizontal.CenterAligned:
                    location.X += (renderArea.Width - size.X) / 1.9f;
                    break;
                case TextHorizontal.RightAligned:
                    location.X += renderArea.Width - size.X;
                    break;
            }

            switch (v)
            {
                case TextVertical.CenterAligned:
                    location.Y += (renderArea.Height - size.Y) / 1.9f;
                    break;
                case TextVertical.BottomAligned:
                    location.Y += renderArea.Height - size.Y;
                    break;
            }

            spriteBatch.GUIDrawString(SpriteFont, value, location, color);
        }
    }
}
