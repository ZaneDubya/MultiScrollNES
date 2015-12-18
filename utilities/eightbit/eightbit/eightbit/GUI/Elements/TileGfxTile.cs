using Core.GUI.Framework;
using Core.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace eightbit.GUI.Elements
{
    class TileGfxTile : WidgetBase<Renderers.TileGfxTileRenderRule>
    {
        public TileGfxTile(int x, int y, int pixelscale, int tile_x, int tile_y, 
            Screens.TileGfxScreen.TileClickHandler onClick, Screens.TileGfxScreen.TileClickHandler onMove)
        {
            m_PixelScale = pixelscale;
            Area = new Rectangle(x, y, 8 * m_PixelScale, 8 * m_PixelScale);
            RenderRule.SetSize(8 * m_PixelScale, 8 * m_PixelScale);
            m_TileX = tile_x;
            m_TileY = tile_y;
            m_OnClick += onClick;
            m_OnMove += onMove;
        }

        private int m_HoverTile = 0;
        public int HoverTile { get { return m_HoverTile; } }

        private int m_PixelScale;
        internal Screens.TileGfxScreen.TileClickHandler m_OnClick, m_OnMove;
        private int m_TileX, m_TileY, m_TileIndex = 0;
        public int TileX
        {
            get { return m_TileX; }
        }
        public int TileY
        {
            get { return m_TileY; }
        }
        public int TileIndex
        {
            set { m_TileIndex = RenderRule.TileIndex = value; }
            get { return m_TileIndex; }
        }
        public Texture2D PageTexture
        {
            set { RenderRule.PageTexture = value; }
        }

        protected override Renderers.TileGfxTileRenderRule BuildRenderRule()
        {
            return new Renderers.TileGfxTileRenderRule(this, m_TileIndex);
        }

        protected override void Attach() { }
        protected internal override void OnUpdate() { }
        protected internal override void OnLayout() { }

        protected internal override void MouseDown(InputEventMouse e)
        {
            int pixel = ((e.Position.X - RenderRule.Area.X) / m_PixelScale) + ((e.Position.Y - RenderRule.Area.Y) / m_PixelScale) * 8;
            int x = ((e.Position.X - RenderRule.Area.X) / m_PixelScale);
            int y = ((e.Position.Y - RenderRule.Area.Y) / m_PixelScale);
            if (x < 0 || x >= 8 || y < 0 || y >= 8)
                return;
            m_OnClick(this, e, x + y * 8);
        }

        protected internal override void MouseMove(InputEventMouse e)
        {
            int x = ((e.Position.X - RenderRule.Area.X) / m_PixelScale);
            int y = ((e.Position.Y - RenderRule.Area.Y) / m_PixelScale);
            if (x < 0 || x >= 8 || y < 0 || y >= 8)
                m_HoverTile = -1;
            else
                m_HoverTile = x + y * 8;
            if (m_HoverTile != -1)
                m_OnMove(this, e, m_HoverTile);
        }
    }
}
