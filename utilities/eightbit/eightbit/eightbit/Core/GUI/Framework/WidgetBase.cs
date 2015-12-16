using Microsoft.Xna.Framework;
using Core.Input;

namespace Core.GUI.Framework
{
    public abstract class WidgetBase<T> : Widget where T : RenderRule
    {
        protected internal T RenderRule { get; set; }

        public override Rectangle Area { get; protected set; }

        public override Rectangle ScreenArea
        {
            get { return RenderRule.Area; }
            set { RenderRule.Area = value; }
        }

        internal override Rectangle InputArea
        {
            get
            {
                return RenderRule.SafeArea;
            }
        }

        public int X
        {
            get { return Area.X; }
            set { Area = new Rectangle(value, Area.Y, Area.Width, Area.Height); }
        }

        public int Y
        {
            get { return Area.Y; }
            set { Area = new Rectangle(Area.X, value, Area.Width, Area.Height); }
        }

        

        public virtual string Skin { set { RenderRule.Skin = value; } }
        public virtual string Font { set { RenderRule.FontName = value; } }

        protected abstract T BuildRenderRule();
        protected WidgetBase()
        {
            RenderRule = BuildRenderRule();
        }

        protected abstract void Attach();

        protected internal override void OnInitialize()
        {
            RenderRule.RenderManager = Owner.RenderManager;
            RenderRule.Load();

            Attach();
            Resize();
        }

        internal override void Draw() { RenderRule.Draw(); }

        internal override void DrawNoClipping() { RenderRule.DrawNoClipping(); }
    }
}
