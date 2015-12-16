using Core.Input;
using Core.GUI.Framework;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace eightbit.GUI.Elements
{
    class PaletteNES : WidgetBase<Renderers.PaletteRenderRule>
    {
        public PaletteNES(int x, int y, int width, int height)
        {
            Area = new Rectangle(x, y, width, height);
            RenderRule.SetSize(width, height);
        }

        protected override Renderers.PaletteRenderRule BuildRenderRule()
        {
            return new Renderers.PaletteRenderRule(4);
        }

        protected override void Attach() { }
        protected internal override void OnUpdate() { }
        protected internal override void OnLayout() { }

        public int Selected
        {
            get { return RenderRule.Selected; }
        }

        public SortMode Sort
        {
            get { return RenderRule.Sort; }
            set { RenderRule.Sort = value; }
        }

        protected internal override void MouseDown(InputEventMouse e)
        {
            RenderRule.Select(e.Position);
            State.SelectedColor = RenderRule.Selected;
        }

        protected internal override void MouseMove(InputEventMouse e)
        {
            if (e.Button != MouseButton.None)
                RenderRule.Select(e.Position);
        }

        public enum SortMode
        {
            Default,
            ColorByRow
        }
    }
}
