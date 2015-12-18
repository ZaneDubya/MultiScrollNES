using Core.GUI.Content;
using Core.GUI.Framework;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace eightbit.GUI.Elements
{
    class ContextMenu : WidgetBase<PanelRenderRule>
    {
        public ContextMenu(int x, int y, int width, int height)
        {
            Area = new Rectangle(x, y, width, height);
        }

        protected override PanelRenderRule BuildRenderRule()
        {
            return new PanelRenderRule();
        }
        protected override void Attach() { }
        protected internal override void OnUpdate() { }

        protected internal override void OnLayout()
        {
            ClipArea = RenderRule.SafeArea;
        }
    }
}
