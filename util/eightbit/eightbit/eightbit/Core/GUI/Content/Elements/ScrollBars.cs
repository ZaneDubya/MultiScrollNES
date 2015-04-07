using System.Linq;
using Microsoft.Xna.Framework;
using Core.Input;
using Core.GUI.Framework;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace Core.GUI.Content {

    public class ScrollBars : WidgetBase<ScrollBarsRenderRule> {

        double _yF, _xF;

        public int ScrollSpeed { get; set; }
        public int WheelSpeed { get; set; }

        private enum DragState { None, VBar, HBar, Up, Down, Left, Right }
        private DragState State { get; set; }
        private Pin Pin { get; set; }

        private Rectangle? m_ScrollAreaOverride;
        public void OverrideScrollArea(Rectangle area)
        {
            m_ScrollAreaOverride = area;
            RenderRule.BuildBars(m_ScrollAreaOverride.Value);
        }
        public Point ScrollPosition
        {
            get
            {
                return new Point((int)RenderRule.Horizontal.ChildOffset, (int)RenderRule.Vertical.ChildOffset);
            }
            set
            {
                if (IsScrollOverride)
                    OnLayout();
                RenderRule.Horizontal.ChildOffset = value.X;
                RenderRule.Vertical.ChildOffset = value.Y;
            }
        }
        private bool IsScrollOverride
        {
            get { return m_ScrollAreaOverride.HasValue; }
        }

        /*####################################################################*/
        /*                           Initialization                           */
        /*####################################################################*/

        protected override ScrollBarsRenderRule BuildRenderRule()
        {
            return new ScrollBarsRenderRule();
        }

        public ScrollBars()
        {
            Pin = new Pin();
            State = DragState.None;
            BlocksInput = true;
        }

        protected override void Attach()
        {
            if (Parent != null)
            {
                var a = Parent.InputArea;
                Area = new Rectangle(0, 0, a.Width, a.Height);
            }
            else
            {
                Area = Owner.ScreenBounds;
            }

            ScrollSpeed = Owner.DefaultScrollSpeed;
            WheelSpeed = Owner.DefaultWheelSpeed;
        }

        protected internal override void OnLayout()
        {
            if (IsScrollOverride)
            {
                RenderRule.BuildBars(m_ScrollAreaOverride.Value);
            }
            else
            {
                // Pixel size of panel to contain child widgets
                Rectangle outerArea;
                Point scroll;

                BuildContainerRenderArea(out outerArea, out scroll); // scroll isn't used. Scroll should always be positive.
                RenderRule.BuildBars(outerArea);

                if (Children != null)
                {
                    foreach (Widget widget in Children)
                    {
                        widget.ScreenArea = new Rectangle(
                            widget.Area.X + InputArea.X - (int)(RenderRule.Horizontal.ChildOffset),
                            widget.Area.Y + InputArea.Y - (int)(RenderRule.Vertical.ChildOffset),
                            widget.Area.Width,
                            widget.Area.Height);
                        widget.ClipArea = RenderRule.SafeArea;
                    }
                }
            }

            ClipArea = RenderRule.SafeArea;
        }

        /*####################################################################*/
        /*                                Logic                               */
        /*####################################################################*/

        protected internal override void OnUpdate()
        {
            if (!IsHover && !IsPressed)
            {
                return;
            }

            if (Owner.NewState.ScrollWheelValue != Owner.OldState.ScrollWheelValue)
            {
                RenderRule.Vertical.BarOffset -= ((Owner.NewState.ScrollWheelValue - Owner.OldState.ScrollWheelValue) / 120f) * WheelSpeed;
                RenderRule.Vertical.BarOffset = Core.Library.Clamp<double>(RenderRule.Vertical.BarOffset, 0, RenderRule.Vertical.MaxBarOffset);
            }
            else
            {
                switch (State)
                {
                    case DragState.None:
                        return;
                    case DragState.VBar:
                        RenderRule.Vertical.BarOffset = _yF - Pin.Shift.Y;
                        break;
                    case DragState.HBar:
                        RenderRule.Horizontal.BarOffset = _xF - Pin.Shift.X;
                        break;
                    case DragState.Up:
                        RenderRule.Vertical.BarOffset += ScrollSpeed;
                        break;
                    case DragState.Down:
                        RenderRule.Vertical.BarOffset -= ScrollSpeed;
                        break;
                    case DragState.Right:
                        RenderRule.Horizontal.BarOffset += ScrollSpeed;
                        break;
                    case DragState.Left:
                        RenderRule.Horizontal.BarOffset -= ScrollSpeed;
                        break;
                }
            }

            // Call Layout if we have not overriden the scroll bar area.
            if (!IsScrollOverride)
                Layout();
        }

        /*####################################################################*/
        /*                               Helpers                              */
        /*####################################################################*/

        private void BuildContainerRenderArea(out Rectangle area, out Point scroll)
        {
            if (Children == null || !Children.Any())
            {
                area = Rectangle.Empty;
                scroll = Point.Zero;
                return;
            }

            int left = int.MaxValue;
            int right = int.MinValue;
            int top = int.MaxValue;
            int bottom = int.MinValue;

            foreach (Widget child in Children)
            {
                int childLeft = child.Area.X;
                int childRight = child.Area.X + child.Area.Width;
                int childTop = child.Area.Y;
                int childBottom = child.Area.Y + child.Area.Height;

                left = (left < childLeft) ? left : childLeft;
                right = (right > childRight) ? right : childRight;
                top = (top < childTop) ? top : childTop;
                bottom = (bottom > childBottom) ? bottom : childBottom;
            }

            if (left > 0)
                left = 0;
            if (top > 0)
                top = 0;

            area = new Rectangle(left, top, right - left, bottom - top);
            scroll = new Point(left, top);
        }

        /*####################################################################*/
        /*                                 Events                             */
        /*####################################################################*/

        protected internal override void MouseDoubleClick(InputEventMouse e)
        {
            MouseDown(e);
        }

        protected internal override void MouseDown(InputEventMouse e)
        {
            Point mouse = e.Position;
            BarArea area = getAreaFromMouse(mouse);

            switch (area)
            {
                case BarArea.None:
                    if (Children == null || Children.Count<Widget>() == 0)
                        Parent.MouseDown(e);
                    break;
                case BarArea.V_Increase:
                    State = DragState.Up;
                    break;
                case BarArea.V_Decrease:
                    State = DragState.Down;
                    break;
                case BarArea.V_Bar:
                    _yF = RenderRule.Vertical.BarOffset;
                    Pin.Push();
                    State = DragState.VBar;
                    break;
                case BarArea.H_Increase:
                    State = DragState.Right;
                    break;
                case BarArea.H_Decrease:
                    State = DragState.Left;
                    break;
                case BarArea.H_Bar:
                    _xF = RenderRule.Horizontal.BarOffset;
                    Pin.Push();
                    State = DragState.HBar;
                    break;
            }
        }

        protected internal override void MouseClick(InputEventMouse e)
        {
            if (getAreaFromMouse(e.Position) == BarArea.None && (Children == null || Children.Count<Widget>() == 0))
                Parent.MouseClick(e);
        }

        BarArea getAreaFromMouse(Point mouse)
        {
            if (RenderRule.BarDirection == Direction.Vertical || RenderRule.BarDirection == Direction.Both)
            {
                if (RenderRule.Vertical.IncreaseArea.Contains(mouse))
                {
                    return BarArea.V_Increase;
                }
                else if (RenderRule.Vertical.DecreaseArea.Contains(mouse))
                {
                    return BarArea.V_Decrease;
                }
                else if (RenderRule.Vertical.BarArea.Contains(mouse))
                {
                    return BarArea.V_Bar;
                }
            }

            if (RenderRule.BarDirection == Direction.Horizontal || RenderRule.BarDirection == Direction.Both)
            {
                if (RenderRule.Horizontal.IncreaseArea.Contains(mouse))
                {
                    return BarArea.H_Increase;
                }
                else if (RenderRule.Horizontal.DecreaseArea.Contains(mouse))
                {
                    return BarArea.H_Decrease;
                }
                else if (RenderRule.Horizontal.BarArea.Contains(mouse))
                {
                    return BarArea.H_Bar;
                }
            }

            return BarArea.None;
        }

        protected internal override void MouseUp(InputEventMouse e)
        {
            Pin.Pull();
            State = DragState.None;
        }

        enum BarArea
        {
            None,
            H_Bar,
            H_Increase,
            H_Decrease,
            V_Bar,
            V_Increase,
            V_Decrease
        }
    }
}