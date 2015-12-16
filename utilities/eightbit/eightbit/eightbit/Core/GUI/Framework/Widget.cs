using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Core.Input;

namespace Core.GUI.Framework
{
    public abstract class Widget
    {
        private Gui m_Owner;
        protected Gui Owner
        {
            get { return m_Owner; }
            set { m_Owner = value; }
        }

        internal InputManager InputManager { get; set; }

        //Location
        public abstract Rectangle Area { get; protected set; }
        public abstract Rectangle ScreenArea { get; set; }
        internal abstract Rectangle InputArea { get; }
        public Point ScreenPosition
        {
            get
            {
                Point p = new Point(Area.X, Area.Y);
                if (Parent != null)
                {
                    Point pp = Parent.ScreenPosition;
                    p.X += pp.X;
                    p.Y += pp.Y;
                }
                return p;
            }
        }

        /// <summary>
        /// Area that children controls will be clipped to.
        /// </summary>
        internal Rectangle ClipArea { get; set; }

        private bool m_LayoutRequired = false;
        private bool m_InUpdate = false;

        private Widget m_Parent = null;
        private List<Widget> m_Children = null;

        public Widget Parent
        {
            get
            {
                return m_Parent;
            }
            internal set
            {
                m_Parent = value;
                m_LayoutRequired = true;
            }
        }
        public IEnumerable<Widget> Children
        {
            get
            {
                if (m_Children == null)
                    return null;
                else
                    return m_Children;
            }
            set
            {
                if (m_Children == null)
                    m_Children = new List<Widget>();
                m_Children.Clear();
                foreach (Widget w in value)
                    AddWidget(w);
            }
        }


        public void AddWidget(Widget child)
        {
            if (m_Children == null)
                m_Children = new List<Widget>();
            m_Children.Add(child);
            child.Parent = this;
            child.Initialize(Owner);
            m_LayoutRequired = true;
        }

        public void AddWidgets(IEnumerable<Widget> children)
        {
            foreach (Widget child in children)
                AddWidget(child);
        }

        public void RemoveWidget(Widget child)
        {
            if (m_Children != null)
            {
                m_Children.Remove(child);
                if (m_Children.Count == 0)
                    m_Children = null;
                m_LayoutRequired = true;
            }
        }

        public Widget LastWidget
        {
            get
            {
                if (m_Children == null)
                    return null;
                if (m_Children.Count == 0)
                    return null;
                return m_Children[m_Children.Count - 1];
            }
        }
        public void RemoveAllWidgets()
        {
            while (LastWidget != null)
                RemoveWidget(LastWidget);
        }

        /*####################################################################*/
        /*                                State                               */
        /*####################################################################*/

        private bool m_Visible;
        public bool Visible
        {
            get
            {
                if (!m_Visible)
                    return false;
                if (Parent != null)
                    return Parent.Visible;
                return true;
            }
            set
            {
                m_Visible = value;
            }
        }

        private bool m_Active = false;
        public bool Active
        {
            get
            {
                return m_Active;
            }
            set
            {
                m_Active = true;
            }
        }

        /// <summary>
        /// When true this widget blocks any other widget from 
        /// receiving input until the mouse leave this widget's
        /// input area.
        /// </summary>
        internal protected bool BlocksInput { get; set; }

        public bool IsPressed { get { return InputManager.PressedWidget == this; } }
        public bool IsHover { get { return InputManager.HoverWidget == this; } }

        #region Events

        /*####################################################################*/
        /*                                 Events                             */
        /*####################################################################*/

        //Used to update state because of external changes.
        protected virtual void Resize() { ScreenArea = Area; }

        //Interaction with internal layout system
        protected internal virtual void EnterHover() { }
        protected internal virtual void ExitHover() { }

        protected internal virtual void EnterPressed() { }
        protected internal virtual void ExitPressed() { }

        protected internal virtual void EnterKeyboardFocus() { }
        protected internal virtual void ExitKeyboardFocus() { }

        //Interaction with input system        
        protected internal virtual void KeyDown(InputEventKeyboard e) { }
        protected internal virtual void CharEntered(InputEventKeyboard e) { }
        protected internal virtual void KeyUp(InputEventKeyboard e) { }

        protected internal virtual void MouseClick(InputEventMouse e) { }
        protected internal virtual void MouseDoubleClick(InputEventMouse e) { }

        protected internal virtual void MouseDown(InputEventMouse e) { }
        protected internal virtual void MouseUp(InputEventMouse e) { }
        protected internal virtual void MouseMove(InputEventMouse e) { }
        protected internal virtual void MouseWheel(InputEventMouse e) { }

        #endregion

        #region Flow

        /*####################################################################*/
        /*                                Flow                                */
        /*####################################################################*/

        protected Widget()
        {
            Visible = true;
            Active = true;
            m_LayoutRequired = true;
            BlocksInput = false;
        }

        ~Widget()
        {

        }

        protected internal void Update()
        {
            m_InUpdate = true;
            if (m_LayoutRequired)
                Layout();
            OnUpdate();
            if (m_Children != null)
                foreach (Widget child in m_Children)
                    child.Update();
            m_InUpdate = false;
        }

        protected internal void Layout()
        {
            if (!m_InUpdate)
            {
                m_LayoutRequired = true;
            }
            else
            {
                ClipArea = Rectangle.Intersect(this.ScreenArea,
                    (Parent == null) ? Owner.RenderManager.GraphicsDevice.Viewport.Bounds : Parent.ClipArea);
                if (m_Children != null)
                    foreach (Widget child in m_Children)
                    {
                        child.ScreenArea = new Rectangle(
                            child.Area.X + InputArea.X,
                            child.Area.Y + InputArea.Y,
                            child.Area.Width,
                            child.Area.Height);
                        child.Layout();
                    }
                OnLayout();
                m_LayoutRequired = false;
            }
        }

        public void Initialize(Gui gui)
        {
            m_LayoutRequired = true;
            Owner = gui;
            InputManager = gui.InputManager;

            ClipArea = gui.RenderManager.GraphicsDevice.Viewport.Bounds;

            OnInitialize();
        }

        protected internal abstract void OnUpdate();
        protected internal abstract void OnInitialize();
        protected internal abstract void OnLayout();

        internal abstract void Draw();
        internal abstract void DrawNoClipping();

        #endregion
    }
}
