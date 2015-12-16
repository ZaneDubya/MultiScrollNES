using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Core.GUI.Framework;

namespace eightbit.GUI.Elements
{
    public class ListBoxRenderRule<T> : RenderRule where T : class
    {
        private ListBox<T> m_Parent;

        private Rectangle _area;
        public override Rectangle Area
        {
            set { _area = value; }
            get { return _area; }
        }

        public override void SetSize(int w, int h)
        {
            
        }

        private int _borderWidth = 1;
        public override Rectangle SafeArea
        {
            get
            {
                return new Rectangle(
                    Area.X + _borderWidth,
                    Area.Y + _borderWidth,
                    Area.Width - (_borderWidth * 2),
                    Area.Height - (_borderWidth * 2));
            }
        }

        private Font _textRenderer;
        public SpriteFont Font { get { return _textRenderer.SpriteFont; } }

        public ListBoxRenderRule(ListBox<T> parent)
        {
            m_Parent = parent;
        }

        protected override void LoadRenderers()
        {
            _borderWidth = 1;
            _textRenderer = RenderManager.SpriteFonts[FontName];
        }

        public int LineSpace
        {
            get { return Font.LineSpacing + 2; }
        }

        public override void Draw()
        {
            RenderManager.SpriteBatch.GUIClipRect_Push(Area);

            Color background = Color.LightGray;
            Color foreground = background * 1.1f;
            Color selected = Color.SkyBlue;

            int linecount = (Area.Height / LineSpace) + 1;
            int y = Area.Y + _borderWidth;
            int width = Area.Width - ((linecount <= m_Parent.Items.Count) ? 11 : 0);

            RenderManager.SpriteBatch.DrawRectangleFilled(
                new Vector3(Area.X, Area.Y, 0),
                new Vector2(width, Area.Height), background);

            int first_item = (m_Parent.ScrollPosition / LineSpace);
            int item_offset = (m_Parent.ScrollPosition % LineSpace);

            for (int i = 0; i < linecount; i++)
            {
                if (m_Parent.Items.Count > (i + first_item))
                {
                    if (i == (m_Parent.SelectedIndex - first_item))
                    {
                        RenderManager.SpriteBatch.DrawRectangleFilled(
                            new Vector3(Area.X, y + i * LineSpace - item_offset, 0),
                            new Vector2(width, LineSpace), selected);
                    }
                    _textRenderer.Render(RenderManager.SpriteBatch, m_Parent.Items[i + first_item].ToString(),
                        new Rectangle(Area.X + 4, y + i * LineSpace + _borderWidth - item_offset, Area.Width - 8, LineSpace),
                        v: TextVertical.CenterAligned, color: Color.Black);
                }
            }

            RenderManager.SpriteBatch.DrawRectangle(
                new Vector3(Area.X, Area.Y, 0), 
                new Vector2(Area.Width, Area.Height), Color.Black);

            RenderManager.SpriteBatch.GUIClipRect_Pop();
        }

        public override void DrawNoClipping() { }

        public int ItemAtPosition(Point position)
        {
            int first_item = (m_Parent.ScrollPosition / LineSpace);
            int item_offset = (m_Parent.ScrollPosition % LineSpace);

            int y = position.Y - SafeArea.Y + item_offset;
            int index = (y / LineSpace) + first_item;
            return index;
        }
    }
}