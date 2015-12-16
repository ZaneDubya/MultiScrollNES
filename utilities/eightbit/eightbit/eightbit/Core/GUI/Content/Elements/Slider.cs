using Microsoft.Xna.Framework;
using Core.Input;
using Core.GUI.Framework;

namespace Core.GUI.Content {

    public sealed class Slider : WidgetBase<SliderRenderRule> {

        /*####################################################################*/
        /*                               Variables                            */
        /*####################################################################*/

        private int _travelStart = -1;

        private Pin Pin { get; set; }

        /// <summary>
        /// The Percentage of the value the slider is at. 100% is all the way to the right.
        /// 0% is all the way to the left. Accepts a float value between 0 and 1 inclusive.
        /// </summary>
        public float Value {
            get {
                return RenderRule.Percentage;
            } set {
                RenderRule.Percentage = value;
            }
        }

        /// <summary>
        /// Event fired when the slider is dragged.
        /// </summary>
        public WidgetEvent ValueChanged { get; set; }

        /*####################################################################*/
        /*                           Initialization                           */
        /*####################################################################*/

        protected override SliderRenderRule BuildRenderRule() {
            return new SliderRenderRule();
        }

        public Slider(int x, int y, int width, WidgetEvent onValueChanged = null) {

            Pin = new Pin();
            Area = new Rectangle(x, y, width, 0);
            ValueChanged = onValueChanged;
        } 

        protected override void Attach() {

            Area = new Rectangle(Area.X, Area.Y, Area.Width, RenderRule.IconSize.Y);
        }

        protected internal override void OnLayout() { }

        protected internal override void OnUpdate() {

            if (_travelStart == -1) { return; }

            RenderRule.Travel = _travelStart - (int)Pin.Shift.X;
            ValueChanged(this);
        }

        /*####################################################################*/
        /*                                Events                              */
        /*####################################################################*/

        protected internal override void MouseDown(InputEventMouse e) {

            if (!RenderRule.GripArea.Contains(e.Position)) { return; }

            _travelStart = RenderRule.Travel;
            Pin.Push();
        }

        protected internal override void MouseUp(InputEventMouse e) {

            _travelStart = -1;
            Pin.Pull();
        }
    }
}