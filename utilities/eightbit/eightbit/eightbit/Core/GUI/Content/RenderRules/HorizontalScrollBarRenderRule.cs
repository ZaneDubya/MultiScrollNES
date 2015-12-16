using Microsoft.Xna.Framework;

namespace Core.GUI.Content {

    public class HorizontalScrollBarRenderRule : ScrollBarRenderRule {

        private double _barOffset;
        public override double BarOffset {
            get
            {
                return _barOffset;
            } set
            {
                _barOffset = Core.Library.Clamp<double>(value, 0, MaxBarOffset);
                barArea.X = HolderArea.Left + (int)_barOffset;
            }
        }

        private Rectangle _area;
        public override Rectangle Area {
            get
            {
                return _area;
            }
            set
            {
                _area = value;
                decreaseArea.X = _area.Left; //Left button
                decreaseArea.Y = _area.Bottom - Holder.Across;

                increaseArea.X = _area.Right - Holder.Edge; //Right button
                increaseArea.Y = _area.Bottom - Holder.Across;

                holderArea.Height = increaseArea.Height = decreaseArea.Height = Holder.Across; //Height of everything except the bar
                increaseArea.Width = decreaseArea.Width = Holder.Edge; //Width of the buttons

                holderArea.X = _area.Left; //Holder
                holderArea.Y = decreaseArea.Top;
                holderArea.Width = _area.Width;

                barArea.X = HolderArea.Left + Holder.Buffer + (int)BarOffset; //Bar
                barArea.Y = HolderArea.Top;
                barArea.Height = Bar.Across;

                if (Both) {
                    increaseArea.X -= Holder.Across;
                    holderArea.Width -= Holder.Across;
                }
            }
        }

        public override void SetSize(int w, int h)
        {
            _area.Width = w; 
            _area.Height = h;
        }

        protected override void LoadRenderers()
        {
            Bar = LoadRenderer<SlidingDoorRenderer>(Skin, "scrollbar_bar");
            Holder = LoadRenderer<SlidingDoorRenderer>(Skin, "scrollbar_holder");
        }

        public override void CalculateRatio(Rectangle childArea)
        {
            int holderSize = (holderArea.Width - (Holder.Buffer * 2));

            Ratio = (double)holderSize / (double)childArea.Width;

            int barSize = (int)(holderSize * Ratio);
            MaxBarOffset = (int)(holderSize - barSize);

            if (Both)
            {
                Ratio = (double)MaxBarOffset / (childArea.Width - (Area.Width - Holder.Edge));
            }
            else
            {
                Ratio = (double)MaxBarOffset / (childArea.Width - Area.Width);
            }

            barArea.Width = barSize;
            Area = Area;
        }
    }
}