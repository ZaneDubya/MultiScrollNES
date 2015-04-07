using Core.GUI.Content;
using Core.GUI.Framework;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace eightbit.GUI.Elements.Renderers
{
    public class ClickableIconRenderRule : RenderRule
    {
        private Font _textRenderer;
        private IconRenderer _default, _hover, _pressed;

        private Rectangle _area;
        public override Rectangle Area
        {
            get { return _area; }
            set { _area = value; }
        }

        public enum RenderMode { Default, Hover, Pressed }
        public RenderMode Mode { get; set; }

        public int Height { get { return _default.Height; } }
        public int Width { get { return _default.Width; } }

        private string _basename;

        public ClickableIconRenderRule(string skin = null, string basename = "")
        {
            Skin = skin;
            _basename = (basename == "") ? "close_button" : basename;
        }

        public override void SetSize(int w, int h)
        {
            _area.Width = w;
            _area.Height = h;
        }

        protected override void LoadRenderers()
        {
            _default = LoadRenderer<IconRenderer>(Skin, _basename);
            _hover = LoadRenderer<IconRenderer>(Skin, _basename + "_hover");
            _pressed = LoadRenderer<IconRenderer>(Skin, _basename + "_pressed");

            _textRenderer = RenderManager.SpriteFonts[FontName];
        }

        public override void Draw()
        {
            switch (Mode)
            {
                case RenderMode.Default:
                    {
                        _default.Render(RenderManager.SpriteBatch, Area);
                        break;
                    }
                case RenderMode.Hover:
                    {
                        _hover.Render(RenderManager.SpriteBatch, Area);
                        break;
                    }
                case RenderMode.Pressed:
                    {
                        _pressed.Render(RenderManager.SpriteBatch, Area);
                        break;
                    }
            }
        }

        public override void DrawNoClipping() { }
    }
}
