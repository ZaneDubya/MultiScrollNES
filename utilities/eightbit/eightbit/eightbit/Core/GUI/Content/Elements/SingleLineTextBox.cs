using Core.GUI.Framework;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace Core.GUI.Content
{
    public sealed class SingleLineTextBox : WidgetBase<PanelRenderRule>
    {
        public WidgetEvent OnValueChanged;

        private TextBox m_InnerTextBox;

        override public string Skin { set { m_InnerTextBox.RenderRule.Skin = value; } }
        override public string Font { set { m_InnerTextBox.RenderRule.FontName = value; } }

        public string Value
        {
            get
            {
                return m_InnerTextBox.RenderRule.Value;
            }
            set
            {
                m_InnerTextBox.RenderRule.Value = value;
            }
        }

        public int MaxCharacters {
            get { return m_InnerTextBox.RenderRule.MaxLength; }
            set { m_InnerTextBox.RenderRule.RecreateStringData(value); }
        }

        /*####################################################################*/
        /*                           Initialization                           */
        /*####################################################################*/

        private short m_MaxChars;

        public SingleLineTextBox(int x, int y, int width, short chars, WidgetEvent onValueChanged = null)
        {
            m_MaxChars = chars;
            Area = new Rectangle(x, y, width, 0);
            if (onValueChanged != null)
                OnValueChanged = onValueChanged;
        }

        protected override PanelRenderRule BuildRenderRule()
        {
            return new PanelRenderRule();
        }

        protected override void Attach()
        {
            if (m_InnerTextBox == null)
            {
                AddWidget(m_InnerTextBox = new TextBox(2, m_MaxChars));
                m_InnerTextBox.OnValueChanged += OnValueChanged;
            }
            var key = m_InnerTextBox.RenderRule.FontName ?? RenderRule.RenderManager.DefaultSkin;
            var height = RenderRule.RenderManager.SpriteFonts[key].SpriteFont.LineSpacing + (2 * RenderRule.BorderWidth);
            Area = new Rectangle(Area.X, Area.Y, Area.Width, height);
        }

        protected internal override void OnLayout()
        {
            m_InnerTextBox.ScreenArea = new Rectangle(InputArea.X, InputArea.Y, Area.Width, Area.Height);
            m_InnerTextBox.ClipArea = Rectangle.Intersect(m_InnerTextBox.ScreenArea, ClipArea);
        }

        protected internal override void OnUpdate()
        {

        }
    }
}