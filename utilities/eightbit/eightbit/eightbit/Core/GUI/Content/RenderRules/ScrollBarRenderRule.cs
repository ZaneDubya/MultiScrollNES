using Microsoft.Xna.Framework;
using Core.GUI.Framework;

namespace Core.GUI.Content {

    public abstract class ScrollBarRenderRule : RenderRule {

        protected SlidingDoorRenderer Bar, Holder;

// ReSharper disable InconsistentNaming
        protected Rectangle barArea;
        public Rectangle BarArea { get { return barArea; } }

        protected Rectangle holderArea;
        public Rectangle HolderArea { get { return holderArea; } }

        protected Rectangle increaseArea;
        public Rectangle IncreaseArea { get { return increaseArea; } }

        protected Rectangle decreaseArea;
        public Rectangle DecreaseArea { get { return decreaseArea; } }
// ReSharper restore InconsistentNaming

        public double ChildOffset
        {
            get
            {
                return double.IsNaN(BarOffset / Ratio) ? 0 : BarOffset / Ratio;
            }
            set
            {
                if (value < 0)
                    value = 0;
                if (value > MaxBarOffset / Ratio)
                    value = MaxBarOffset / Ratio;
                BarOffset = value * Ratio;
            }
        }
        public abstract double BarOffset { get; set; }
        public double MaxBarOffset { get; protected set; }
        public double Ratio { get; protected set; }

        public bool Both { get; set; }

        public abstract void CalculateRatio(Rectangle renderArea);

        public void SetRenderManager(RenderManager manager) {
            RenderManager = manager;
        }

        public override void Draw() {
            
            Holder.Render(RenderManager.SpriteBatch, HolderArea);
            Bar.Render(RenderManager.SpriteBatch, BarArea);
        }

        public override void DrawNoClipping() { }
    }
}