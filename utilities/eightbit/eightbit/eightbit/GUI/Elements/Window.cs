using System;
using Core.GUI.Content;
using Core.GUI.Framework;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace eightbit.GUI.Elements
{
    class Window : WidgetBase<PanelRenderRule>
    {
        public Window(int x, int y, int width, int height, WindowButtons buttons = WindowButtons.None)
        {
            Area = new Rectangle(x, y, width, height);

            if (buttons.HasFlag(WindowButtons.Close))
            {
                _btn_Close = new WindowButton(0, 0);
            }
        }

        WindowButton _btn_Close;

        protected override PanelRenderRule BuildRenderRule()
        {
            return new PanelRenderRule();
        }
        protected override void Attach() { }
        protected internal override void OnUpdate() { }

        protected internal override void OnLayout()
        {
            if (_btn_Close != null)
                _btn_Close.X = Area.Width - _btn_Close.Area.Width - RenderRule.BorderWidth * 2;
            ClipArea = RenderRule.SafeArea;
        }
    }

    [Flags]
    enum WindowButtons : int
    {
        None = 0,
        Close = 1,
    }
}
