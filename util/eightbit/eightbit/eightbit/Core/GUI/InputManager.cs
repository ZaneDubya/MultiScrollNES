using System;
using System.Collections.Generic;
using eightbit;
using Core.Input;
using Microsoft.Xna.Framework;

namespace Core.GUI.Framework
{
    /// <summary>
    /// Handles/triggers all of the mouse or keyboard events.
    /// </summary>
    internal class InputManager
    {
        internal InputManager(Game game, List<Widget> dom)
        {
            m_Widgets = dom;
        }

        public void Update()
        {
            List<InputEventMouse> mouse_events = Library.Input.GetMouseEvents();
            List<InputEventKeyboard> kb_events = Library.Input.GetKeyboardEvents();

            #region Mouse Events
            foreach (InputEventMouse e in mouse_events)
            {
                switch (e.EventType)
                {
                    case MouseEvent.Move:
                        if (PressedWidget != null)
                        {
                            PressedWidget.MouseMove(e);
                            if (PressedWidget.BlocksInput)
                                continue;
                        }
                        if (HoverWidget != null)
                            HoverWidget.MouseMove(e);
                        HoverWidget = FindHover();
                        break;
                    case MouseEvent.Down:
                        if (HoverWidget != null)
                        {
                            PressedWidget = HoverWidget;
                            KeyboardFocusWidget = HoverWidget;
                            PressedWidget.MouseDown(e);
                        }
                        break;
                    case MouseEvent.Up:
                        if (PressedWidget != null)
                        {
                            PressedWidget.MouseUp(e);
                            PressedWidget = null;
                        }
                        break;
                    case MouseEvent.Click:
                        if (HoverWidget != null)
                        {
                            PressedWidget = HoverWidget;
                            PressedWidget.MouseClick(e);
                        }
                        break;
                    case MouseEvent.DoubleClick:
                        if (PressedWidget != null)
                        {
                            PressedWidget.MouseClick(e);
                            PressedWidget.MouseDoubleClick(e);
                        }
                        break;
                    case MouseEvent.WheelScroll:
                        if (PressedWidget != null)
                            PressedWidget.MouseWheel(e);
                        else if (HoverWidget != null)
                            HoverWidget.MouseWheel(e);
                        break;
                }
            }
            #endregion

            #region Keyboard Events
            foreach (InputEventKeyboard e in kb_events)
            {
                switch (e.EventType)
                {
                    case KeyboardEvent.Press:
                        if (m_KeyboardEvents != null)
                            m_KeyboardEvents(e);
                        if (KeyboardFocusWidget != null)
                            KeyboardFocusWidget.CharEntered(e);
                        break;
                    case KeyboardEvent.Down:
                        if (KeyboardFocusWidget != null)
                            KeyboardFocusWidget.KeyDown(e);
                        break;
                    case KeyboardEvent.Up:
                        if (KeyboardFocusWidget != null)
                            KeyboardFocusWidget.KeyUp(e);
                        break;
                }
            }

            #endregion
        }

        /// <summary>
        /// Finds the element the mouse is currently being hovered over.
        /// </summary>
        private Widget FindHover()
        {
            m_HoverWidget = null;
            foreach (var child in m_Widgets)
            {
                DfsFindHover(child);
            }
            return m_HoverWidget;
        }

        private void DfsFindHover(Widget widget)
        {
            if (!widget.ScreenArea.Contains(Library.Input.MousePosition))
            {
                return;
            }

            if (widget.Parent != null && !widget.Parent.InputArea.Contains(Library.Input.MousePosition))
            {
                return;
            }

            if (!widget.Active || !widget.Visible)
            {
                return;
            }

            m_HoverWidget = widget;

            if (widget.Children != null)
                foreach (var child in widget.Children)
                    DfsFindHover(child);
        }

        Widget m_HoverWidget;

        private readonly List<Widget> m_Widgets;

        // When ever these Widgets are not not null they are in the state
        // specified by their names. Used to trigger events and by the widget
        // class to see what state the widget is in. 
        private Widget m_hoverWidget;
        internal Widget HoverWidget
        {
            get
            {
                return m_hoverWidget;
            }
            private set
            {
                if (value == m_hoverWidget) { return; }
                if (value != null) { value.EnterHover(); }
                if (m_hoverWidget != null) { m_hoverWidget.ExitHover(); }
                m_hoverWidget = value;
            }
        }

        private Widget m_pressedWidget;
        internal Widget PressedWidget
        {
            get
            {
                return m_pressedWidget;
            }
            private set
            {
                if (value == m_pressedWidget) { return; }
                if (value != null) { value.EnterPressed(); }
                if (m_pressedWidget != null) { m_pressedWidget.ExitPressed(); }
                m_pressedWidget = value;
            }
        }

        private Widget m_KeyboardFocusWidget;
        internal Widget KeyboardFocusWidget
        {
            get
            {
                return m_KeyboardFocusWidget;
            }
            private set
            {
                if (value == m_KeyboardFocusWidget)
                    return;
                if (m_KeyboardFocusWidget != null)
                    m_KeyboardFocusWidget.ExitHover();
                m_KeyboardFocusWidget = value;
                if (value != null)
                    value.EnterKeyboardFocus();
            }
        }

        private KeyboardEventHandler m_KeyboardEvents = null;
        public event KeyboardEventHandler KeyboardCharPress
        {
            add { m_KeyboardEvents += value; }
            remove { m_KeyboardEvents -= value; }
        }
    }
}