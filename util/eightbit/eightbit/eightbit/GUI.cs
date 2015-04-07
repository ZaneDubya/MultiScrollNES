using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using RuminateGui.Framework;

namespace eightbit
{
    public class GUIData
    {
        public SpriteFont GreySpriteFont;
        public string GreyMap;
        public Texture2D GreyImageMap;

        private Screens.Screen _currentScreen;
        private Screens.Screen[] _currentScreens;

        private int _index = 0;
        private Game _game;

        public GUIData(Game game)
        {
            _game = game;

            GreyImageMap = game.Content.Load<Texture2D>(@"ImageMap");
            GreyMap = File.OpenText(@"Content/Map.txt").ReadToEnd();
            GreySpriteFont = game.Content.Load<SpriteFont>(@"Texture");

            _currentScreens = new Screens.Screen[] {                
                new Screens.WidgetDemonstration(),
                new Screens.InputTest(),          
                new Screens.LayoutTest(),
                new Screens.ButtonTest()
            };

            _index = 0;
            _currentScreen = _currentScreens[_index];
            _currentScreen.Init(_game, this);
        }

        public void Update()
        {
            _currentScreen.Update();
        }

        public void Draw()
        {
            _currentScreen.Draw();
        }

        public void OnResize()
        {
            if (_currentScreen != null)
                _currentScreen.OnResize();
        }
    }
}
