using System.Collections.Generic;
using Core.Input;
using Microsoft.Xna.Framework;
using Core.GUI.Framework;

namespace eightbit.GUI.Elements
{
    class Menu : WidgetBase<Renderers.MenuBarRenderRule>
    {
        private List<MenuElement> m_Elements;
        public int Height = 24;
        public new List<MenuElement> Children
        {
            get { return m_Elements; }
        }

        public Menu(List<MenuElement> children = null)
        {
            if (children == null)
                m_Elements = new List<MenuElement>();
            else
                m_Elements = children;
        }

        protected override Renderers.MenuBarRenderRule BuildRenderRule()
        {
            return new Renderers.MenuBarRenderRule(parent: this);
        }
        protected override void Attach() { }

        protected internal override void OnUpdate()
        {

        }

        protected internal override void OnLayout()
        {
            Area = new Rectangle(0, 0, (Parent == null) ? Owner.ScreenBounds.Width : Parent.InputArea.Width, Height);
            RenderRule.SetSize(Area.Width, Area.Height);
        }

        private bool m_MouseHover = false, m_MouseDown = false;
        private Point m_MouseLocation;

        public bool IsMouseOver
        {
            get { return m_MouseHover; }
        }
        public bool IsMouseDown
        {
            get { return m_MouseDown; }
        }
        public Point MouseLocation
        {
            get { return m_MouseLocation; }
        }

        protected internal override void EnterHover()
        {
            m_MouseHover = true;
        }

        protected internal override void MouseDown(InputEventMouse e)
        {
            m_MouseDown = true;
        }

        protected internal override void MouseUp(InputEventMouse e)
        {
            m_MouseDown = false;
        }

        protected internal override void ExitHover()
        {
            m_MouseHover = false;
            m_MouseDown = false;
        }

        protected internal override void MouseClick(InputEventMouse e)
        {
            MenuElement element = RenderRule.ElementFromMouseCoordinates();
            if (element != null && element.Action != null)
                element.Action(this);
        }

        protected internal override void MouseMove(InputEventMouse e)
        {
            m_MouseLocation = e.Position;
            m_MouseLocation.X -= Area.X;
            m_MouseLocation.Y -= Area.Y;
        }
    }

    class MenuElement
    {
        public string Label { get; set; }
        public bool Enabled { get; set; }

        public MenuElement(string label, bool enabled = true, WidgetEvent action = null, List<MenuElement> children = null)
        {
            Label = label;
            Enabled = enabled;
            if (action != null)
                Action = action;
            if (children != null)
                m_Children = children;
        }

        public bool HasChildren
        {
            get
            {
                return (m_Children == null) ? false : true;
            }
        }

        public bool HasAction
        {
            get
            {
                return (m_Action == null) ? false : true;
            }
        }

        public WidgetEvent Action
        {
            get
            {
                if (HasAction)
                    return m_Action;
                return null;
            }
            set
            {
                if (HasChildren)
                    clearChildren();
                m_Action = value;
            }
        }

        public List<MenuElement> Children
        {
            get
            {
                if (HasChildren)
                    return m_Children;
                return null;
            }
        }

        public void AddChild(MenuElement element)
        {
            if (HasAction)
            {
                m_Action = null;
                m_Children = new List<MenuElement>();
            }
            m_Children.Add(element);
        }

        private List<MenuElement> m_Children = null;
        private WidgetEvent m_Action = null;

        private void clearChildren()
        {
            for (int i = 0; i < m_Children.Count; i++)
                m_Children[i] = null;
            m_Children.Clear();
            m_Children = null;
        }
    }

    class MenuDivider : MenuElement
    {
        public MenuDivider()
            : base("|", false)
        {

        }
    }
}
