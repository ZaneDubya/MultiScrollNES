using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using RuminateGui.Content;
using RuminateGui.Framework;

namespace eightbit.Screens
{

    /// <summary>
    /// This screen is an example of how to take advantage of the even driven input 
    /// that is a feature of the RuminateFramework.
    /// </summary>
    public class InputTest : Screen {

        private Label _hasMouse, _charEntered, _keyDown, _keyUp, _doubleClick, _mouseDown, _mouseUp, _mouseWheel;

        public override void Init(Game game, GUIData data)
        {

            Color = Color.White;

            var skin = new Skin(data.GreyImageMap, data.GreyMap);
            var text = new Text(data.GreySpriteFont, Color.Black);

            //Simply subscribe to the events demonstrated below
            Gui = new Gui(game, skin, text);
            Gui.CharacterPress += CharacterPress;
            Gui.KeyDown += KeyDown;
            Gui.KeyUp += KeyUp;
            Gui.MouseDoubleClick += MouseDoubleClick;
            Gui.MouseDown += MouseDown;
            Gui.MouseUp += MouseUp;
            Gui.MouseWheel += MouseWheel;

            Gui.AddWidget(_hasMouse = new Label(100, 10 + 30 * 0, "HasMouse"));

            Gui.AddWidget(_charEntered = new Label(100, 10 + 30 * 1, "CharEntered"));
            Gui.AddWidget(_keyDown = new Label(100, 10 + 30 * 2, "KeyDown"));
            Gui.AddWidget(_keyUp = new Label(100, 10 + 30 * 3, "KeyUp"));

            Gui.AddWidget(_doubleClick = new Label(100, 10 + 30 * 4, "MouseDoubleClick"));
            Gui.AddWidget(_mouseDown = new Label(100, 10 + 30 * 5, "MouseDown"));
            Gui.AddWidget(_mouseUp = new Label(100, 10 + 30 * 6, "MouseUp"));

            Gui.AddWidget(_mouseWheel = new Label(100, 10 + 30 * 7, "MouseWheel"));
        }

        private void CharacterPress(object sender, CharacterEventArgs args) {
            //Prevents an error from being thrown if the font does not support the entered character
            if (Gui.DefaultFont.Characters.Contains(args.Character)) {
                _charEntered.Value = "Last char entered = " + new string(new[] { args.Character });
            }
        }

        private void KeyDown(object sender, KeyEventArgs args) {
            _keyDown.Value = "Key down = " + args.KeyCode;
        }

        private void KeyUp(object sender, KeyEventArgs args) {
            _keyUp.Value = "Key up = " + args.KeyCode;
        }

        private void MouseDoubleClick(object sender, MouseEventArgs args) {
            _doubleClick.Value = "DoubleClick @ " + args.Location.X + "," + args.Location.Y;
        }

        private void MouseDown(object sender, MouseEventArgs args) {
            _mouseDown.Value = "Down @ " + args.Location.X + "," + args.Location.Y;
        }

        private void MouseUp(object sender, MouseEventArgs args) {
            _mouseUp.Value = "Up @ " + args.Location.X + "," + args.Location.Y;
        }

        private void MouseWheel(object sender, MouseEventArgs args) {
            _mouseWheel.Value = "Wheel Delta = " + args.Delta;
        }

        public override void OnResize() {
            Gui.Resize();
        }

        public override void Update() {
            Gui.Update();

            //Has mouse only returns true if the mouse is above a GIU widget.
            _hasMouse.Value = "HasMouse = " + Gui.HasMouse;
        }

        public override void Draw() {
            Gui.Draw();
        }
    }
}
