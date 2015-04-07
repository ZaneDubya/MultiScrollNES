using Microsoft.Xna.Framework;
using Core.GUI.Content;
using Core.GUI.Framework;

namespace eightbit.GUI.Elements.Renderers
{
    class MenuBarRenderRule : RenderRule
    {
        private Font _textRenderer;
        private SlidingDoorRenderer _default, _hover, _pressed;

        private Rectangle _area;
        public override Rectangle Area
        {
            get { return _area; }
            set { _area = value; }
        }

        public int Height { get { return _default.Across; } }
        public int Edge { get { return _default.Edge; } }

        private Menu m_Parent;

        public MenuBarRenderRule(string skin = null, Menu parent = null)
        {
            Skin = skin;
            m_Parent = parent;
        }

        public override void SetSize(int w, int h)
        {
            _area.Width = w;
            _area.Height = h;
        }

        protected override void LoadRenderers()
        {
            _default = LoadRenderer<SlidingDoorRenderer>(Skin, "menubar");
            _hover = LoadRenderer<SlidingDoorRenderer>(Skin, "menubar_hover");
            _pressed = LoadRenderer<SlidingDoorRenderer>(Skin, "menubar_pushed");

            _textRenderer = RenderManager.SpriteFonts[FontName];
        }

        public override void Draw()
        {
            _default.Render(RenderManager.SpriteBatch, Area);

            if (m_Parent != null)
            {
                Rectangle area = Area;
                area.X += _default.Edge;
                area.Width -= _default.Edge;
                foreach (MenuElement e in m_Parent.Children)
                {
                    bool elementDown = false;
                    Vector2 v = _textRenderer.SpriteFont.MeasureString(e.Label);

                    if (m_Parent.IsMouseOver)
                    {
                        Rectangle button = new Rectangle(area.X - _hover.Buffer, area.Y, (int)v.X + _hover.Buffer * 2, 24);
                        if (e.Enabled && button.Contains(m_Parent.MouseLocation))
                            if (m_Parent.IsMouseDown)
                            {
                                _pressed.Render(RenderManager.SpriteBatch, button);
                                elementDown = true;
                            }
                            else
                                _hover.Render(RenderManager.SpriteBatch, button);
                    }
                    if (!elementDown) { area.Y--; }
                    _textRenderer.Render(RenderManager.SpriteBatch, e.Label, area, v: TextVertical.CenterAligned);
                    if (!elementDown) { area.Y++; }
                    area.X += (_hover.Buffer * 2 + (int)v.X);
                    area.Width -= (_hover.Buffer * 2 + (int)v.X);
                }
            }
        }

        public MenuElement ElementFromMouseCoordinates()
        {
            if (m_Parent != null && m_Parent.IsMouseOver)
            {
                Rectangle area = Area;
                area.X = area.X + _default.Edge;
                area.Width -= _default.Edge;
                foreach (MenuElement e in m_Parent.Children)
                {
                    Vector2 v = _textRenderer.SpriteFont.MeasureString(e.Label);
                    Rectangle button = new Rectangle(area.X - _hover.Buffer, area.Y, (int)v.X + _hover.Buffer * 2, 24);
                    if (e.Enabled && button.Contains(m_Parent.MouseLocation))
                        return e;
                    area.X += (_hover.Buffer * 2 + (int)v.X);
                    area.Width -= (_hover.Buffer * 2 + (int)v.X);
                }
            }
            return null;
        }

        public override void DrawNoClipping() { }
    }
}
