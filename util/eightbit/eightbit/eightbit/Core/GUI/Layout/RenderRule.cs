using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace Core.GUI.Framework {

    public abstract class RenderRule {

        public bool Loaded { get; set; }

        private RenderManager m_RenderManager;
        internal protected RenderManager RenderManager
        {
            get { return m_RenderManager; }
            set { m_RenderManager = value; }
        }

        public abstract void SetSize(int w, int h);

        /// <summary>
        /// The area on screen taken up by the control.
        /// </summary>
        public abstract Rectangle Area { get; set; }
        /// <summary>
        /// The area on screen taken up by the control, minus any borders which do not accept input and clip over children.
        /// </summary>
        public virtual Rectangle SafeArea { get { return Area; } }

        protected string DefaultSkin { get { return RenderManager.DefaultSkin; } }
        protected string DefaultFontName { get { return RenderManager.DefaultFontName; } }

        private string _skin;
        internal protected string Skin
        {
            get
            {
                return _skin;
            }
            set
            {
                if (value == null) { return; }
                _skin = value;
                if (RenderManager != null) { Load(); }
            }
        }

        

        private string _fontName;
        internal protected string FontName
        {
            get
            {
                return _fontName;
            }
            set
            {
                if (value == null) { return; }
                _fontName = value;
                if (RenderManager != null) { Load(); }
            }
        }

        protected RenderRule()
        {
            Loaded = false;
        }

        protected T LoadRenderer<T>(string skin, string widget) where T : Renderer
        {
            return (T)RenderManager.Skins[skin].WidgetMap[widget];
        }

        protected abstract void LoadRenderers();
        public virtual void Load()
        {
            if (Skin == null) { Skin = DefaultSkin; }
            if (FontName == null) { FontName = DefaultFontName; }
            LoadRenderers();
            Loaded = true;
        }

        public abstract void Draw();

        public abstract void DrawNoClipping();
    }
}
