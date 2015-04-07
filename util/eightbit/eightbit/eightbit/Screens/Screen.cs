using Microsoft.Xna.Framework;
using RuminateGui.Framework;

namespace eightbit.Screens
{

    public abstract class Screen {

        public Color Color { get; set; }
        public Gui Gui { get; set; }

        public abstract void Init(Game game, GUIData data);
        public abstract void OnResize();
        public abstract void Update();
        public abstract void Draw();
    }
}
