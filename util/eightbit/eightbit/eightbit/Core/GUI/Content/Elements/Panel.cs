using Core.GUI.Framework;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace Core.GUI.Content
{
    public sealed class Panel : WidgetBase<PanelRenderRule> {

        /*####################################################################*/
        /*                           Initialization                           */
        /*####################################################################*/

        public Panel(int x, int y, int width, int height)
        {
            Area = new Rectangle(x, y, width, height);
        }        

        protected override PanelRenderRule BuildRenderRule() {
            return new PanelRenderRule();
        }

        protected override void Attach() { }
        protected internal override void OnUpdate() { }
        protected internal override void OnLayout() { } 
    }
}