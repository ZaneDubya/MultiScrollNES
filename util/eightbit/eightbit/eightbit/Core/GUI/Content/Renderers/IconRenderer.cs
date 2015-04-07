using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Core.GUI.Framework;
using Color = Microsoft.Xna.Framework.Color;
using Rectangle = Microsoft.Xna.Framework.Rectangle;
using SBX = Core.Graphics.SpriteBatchExtended;

namespace Core.GUI.Content {

    public class IconRenderer: Renderer {

        private readonly Rectangle _source;

        public Point Size { get { return new Point(_source.Width, _source.Height); } }        
        public int Width { get { return _source.Width; } }
        public int Height { get { return _source.Height; } }

        public IconRenderer(Texture2D imageMap, Rectangle source) : base(imageMap) {

            _source = source;
        }

        public override Rectangle BuildChildArea(Point size) {

            return Rectangle.Empty;
        }

        public override void Render(SBX batch, Rectangle destination) {

            batch.GUIDrawSprite(ImageMap, destination, _source, Color.White);
        }
    }
}
