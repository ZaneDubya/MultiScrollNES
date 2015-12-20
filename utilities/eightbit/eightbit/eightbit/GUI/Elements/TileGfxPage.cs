using Core.GUI.Framework;
using Core.Input;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace eightbit.GUI.Elements
{
    class TileGfxPage : WidgetBase<Renderers.TileGfxPageRenderRule>
    {
        public TileGfxPage(int x, int y, int width, int height, int page_index, Screens.TileGfxScreen.PageClickHandler onTileGfxPageHandler)
        {
            Area = new Rectangle(x, y, width, height);
            m_PageIndex = page_index;
            RenderRule.SetSize(width, height);
            m_OnClickHandler += onTileGfxPageHandler;
        }

        public int Attribute
        {
            set { RenderRule.Attribute = value; }
        }

        protected override Renderers.TileGfxPageRenderRule BuildRenderRule()
        {
            return new Renderers.TileGfxPageRenderRule(this);
        }

        protected override void Attach() { }
        protected internal override void OnUpdate() { }
        protected internal override void OnLayout() { }

        Screens.TileGfxScreen.PageClickHandler m_OnClickHandler;
        private int m_PageIndex = 0, m_SelectedTile = 0;
        public int PageIndex
        {
            get { return m_PageIndex; }
        }
        public int SelectedTile
        {
            get { return m_SelectedTile; }
            set { m_SelectedTile = value; }
        }
        private bool m_HasSelection = false;
        public bool HasSelection
        {
            get { return m_HasSelection; }
            set { m_HasSelection = value; }
        }

        protected internal override void MouseDown(InputEventMouse e)
        {
            int selected = ((e.Position.X - RenderRule.Area.X) / 16) + ((e.Position.Y - RenderRule.Area.Y) / 16) * 16;
            if (m_OnClickHandler != null)
                m_OnClickHandler(m_PageIndex, selected);
        }
    }
}
