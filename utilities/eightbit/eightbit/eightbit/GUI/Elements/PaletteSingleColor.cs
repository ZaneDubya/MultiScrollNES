using Core.Input;
using Microsoft.Xna.Framework;
using Core.GUI.Framework;

namespace eightbit.GUI.Elements
{
    class PaletteSingleColor : WidgetBase<Renderers.PaletteWellRenderRule>
    {
        public PaletteSingleColor(int x, int y, int width, int height, int color_index, int palette_index, int palette_subindex, WidgetEvent onClick)
        {
            Area = new Rectangle(x, y, width, height);
            RenderRule.SetSize(width, height);
            ColorIndex = color_index;
            m_PaletteIndex = palette_index;
            m_PaletteSubIndex = palette_subindex;
            m_OnClick += onClick;
        }

        protected override Renderers.PaletteWellRenderRule BuildRenderRule()
        {
            return new Renderers.PaletteWellRenderRule(this);
        }

        protected override void Attach() { }
        protected internal override void OnUpdate() { }
        protected internal override void OnLayout() { }

        internal WidgetEvent m_OnClick;
        private int m_PaletteIndex, m_PaletteSubIndex;
        public int PaletteIndex
        {
            get { return m_PaletteIndex; }
        }
        public int PaletteSubIndex
        {
            get { return m_PaletteSubIndex; }
        }

        private int m_ColorIndex = 0;
        public int ColorIndex
        {
            get { return m_ColorIndex; }
            set { m_ColorIndex = value; }
        }

        public Color BorderColor
        {
            get { return RenderRule.BorderColor; }
            set { RenderRule.BorderColor = value; }
        }

        protected internal override void MouseDown(InputEventMouse e)
        {
            if (m_OnClick != null)
                m_OnClick(this);
        }
    }
}
