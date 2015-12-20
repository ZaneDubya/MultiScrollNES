using Core.GUI.Framework;
using Core.Input;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace eightbit.GUI.Elements
{
    public sealed class WindowButton : WidgetBase<Renderers.ClickableIconRenderRule>
    {
        public WidgetEvent OnCheck { get; set; }
        public WidgetEvent OnUnCheck { get; set; }

        public WindowButton(int x, int y)
        {
            Area = new Rectangle(x, y, 0, 0);
        }

        protected override Renderers.ClickableIconRenderRule BuildRenderRule()
        {
            return new Renderers.ClickableIconRenderRule(basename: "close_button");
        }

        protected override void Attach()
        {
            int width = RenderRule.Width;
            int height = RenderRule.Height;
            Area = new Rectangle(Area.X, Area.Y, width, height);
        }

        protected internal override void OnUpdate() { }
        protected internal override void OnLayout() { }

        protected internal override void MouseClick(InputEventMouse e)
        {
            // 
        }

        protected internal override void EnterPressed()
        {
            RenderRule.Mode = Renderers.ClickableIconRenderRule.RenderMode.Pressed;
        }

        protected internal override void ExitPressed()
        {
            if (IsHover)
                RenderRule.Mode = (IsHover) ? Renderers.ClickableIconRenderRule.RenderMode.Hover : 
                    Renderers.ClickableIconRenderRule.RenderMode.Default;
        }

        protected internal override void EnterHover()
        {
            RenderRule.Mode = Renderers.ClickableIconRenderRule.RenderMode.Hover;
        }

        protected internal override void ExitHover()
        {
            RenderRule.Mode = Renderers.ClickableIconRenderRule.RenderMode.Default;
        }
    }
}
