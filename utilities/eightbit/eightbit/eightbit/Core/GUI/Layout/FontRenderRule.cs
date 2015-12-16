using Microsoft.Xna.Framework.Graphics;

namespace Core.GUI.Framework {

    public abstract class FontRenderRule : RenderRule {

        protected Font TextRenderer { get; set; }
        public SpriteFont Font { get { return TextRenderer.SpriteFont; } }

        public override void Load() {
            if (Skin == null) { Skin = DefaultSkin; }
            if (FontName == null) { FontName = DefaultFontName; }
            TextRenderer = RenderManager.SpriteFonts[FontName];
            LoadRenderers();
            Loaded = true;
        } 
    }
}
