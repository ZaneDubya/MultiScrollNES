using Core.GUI.Framework;
using Core.Input;
using Microsoft.Xna.Framework;

namespace eightbit.GUI.Screens
{

    public abstract class Screen {

        public Color Color { get; set; }
        public Gui Gui { get; set; }

        public void Init(Game game, GUIManager manager)
        {
            _manager = manager;

            Color = Color.White;

            Gui = new Gui(game,
                new Skin(manager.SpriteMap, manager.SpriteDescription),
                new Font(manager.SpriteFont, Color.White));
            Gui.KeyboardCharPress += OnKeyboardPress;
            OnInit();
        }

        ~Screen()
        {
            Gui.KeyboardCharPress -= OnKeyboardPress;
        }

        public void Dispose()
        {
            Gui.RemoveAllWidgets();
        }

        GUIManager _manager;
        protected GUIManager Manager
        {
            get { return _manager; }
        }

        public abstract void OnInit();
        public abstract void OnResize();
        public abstract void Update();
        public abstract void Draw();

        internal virtual void OnKeyboardPress(InputEventKeyboard args) { }
    }
}
