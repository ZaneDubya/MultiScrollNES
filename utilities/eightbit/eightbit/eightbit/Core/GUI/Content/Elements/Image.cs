using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Core.GUI.Framework;

namespace Core.GUI.Content {

    public sealed class Image : WidgetBase<LabelRenderRule> {

        /*####################################################################*/
        /*                               Variables                            */
        /*####################################################################*/

        public Texture2D Icon {
            get { return RenderRule.Image; }
            set { RenderRule.Image = value; }
        }

        /*####################################################################*/
        /*                           Initialization                           */
        /*####################################################################*/

        protected override LabelRenderRule BuildRenderRule() {
            return new LabelRenderRule();
        }

        public Image(int x, int y, Texture2D texture) {

            Icon = texture;
            Area = new Rectangle(x, y, texture.Width, texture.Height);
        }        

        public Image(int x, int y, int width, int height, Texture2D texture) {

            Icon = texture;
            Area = new Rectangle(x, y, width, height);
        }

        protected override void Attach() { }
        protected internal override void OnUpdate() { }
        protected internal override void OnLayout() { }    
    }
}