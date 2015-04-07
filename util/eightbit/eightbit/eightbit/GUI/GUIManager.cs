using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Core.GUI.Framework;

namespace eightbit.GUI
{
    public class GUIManager
    {
        public SpriteFont SpriteFont;
        public string SpriteDescription;
        public Texture2D SpriteMap;

        private Screens.Screen m_CurrentScreen;
        private Game _game;

        public GUIManager(Game game)
        {
            _game = game;

            SpriteMap = Core.Library.Content.Load<Texture2D>(@"ImageMap");
            // SpriteDescription = File.OpenText(@"Map").ReadToEnd();
            SpriteFont = Core.Library.Content.Load<SpriteFont>(@"Arial12");
            SpriteDescription = ResContent.Map;
            InitScreen(new Screens.FileScreen());
        }

        public void Update()
        {
            m_CurrentScreen.Update();
        }

        public void Draw()
        {
            m_CurrentScreen.Draw();
        }

        public void OnResize()
        {
            if (m_CurrentScreen != null)
                m_CurrentScreen.OnResize();
        }

        private void InitScreen(Screens.Screen screen)
        {
            if (m_CurrentScreen != null)
            {
                if (mnu_MenuBar != null)
                {
                    m_CurrentScreen.Gui.RemoveWidget(mnu_MenuBar);
                    mnu_MenuBar = null;
                }
                m_CurrentScreen.Dispose();
                m_CurrentScreen = null;
            }
            m_CurrentScreen = screen;
            m_CurrentScreen.Init(_game, this);
            bool enable_menu = (State.Data != null);
            m_CurrentScreen.Gui.AddWidget(
            mnu_MenuBar = new Elements.Menu(new List<Elements.MenuElement>()
                {
                    new Elements.MenuElement("File", action: MenuFile_Click),
                    new Elements.MenuDivider(),
                    new Elements.MenuElement("Palettes", action: MenuPalette_Click, enabled: enable_menu),
                    new Elements.MenuElement("TileGfx", action: MenuTileGfx_Click, enabled: enable_menu),
                    new Elements.MenuElement("TileSets", action: MenuTileset_Click, enabled: enable_menu),
                    new Elements.MenuElement("Chunks", action: MenuChunk_Click, enabled: enable_menu),
                    new Elements.MenuElement("Map", action: MenuMap_Click, enabled: enable_menu),
                    new Elements.MenuElement("SpriteDefs", action: MenuSprites_Click, enabled: enable_menu),
                    // new Elements.MenuElement("ActorDefs", action: MenuActors_Click, enabled: enable_menu)
                }));
            Update();
        }

        public void ResetMenu()
        {
            for (int i = 1; i < mnu_MenuBar.Children.Count; i++)
                mnu_MenuBar.Children[i].Enabled = (State.Data != null);
        }

        private void MenuFile_Click(Widget widget)
        {
            if (m_CurrentScreen.GetType() != typeof(Screens.FileScreen))
                InitScreen(new Screens.FileScreen());
        }

        private void MenuPalette_Click(Widget widget)
        {
            if (m_CurrentScreen.GetType() != typeof(Screens.PaletteScreen))
                InitScreen(new Screens.PaletteScreen());
        }

        private void MenuTileGfx_Click(Widget widget)
        {
            if (m_CurrentScreen.GetType() != typeof(Screens.TileGfxScreen))
                InitScreen(new Screens.TileGfxScreen());
        }

        private void MenuTileset_Click(Widget widget)
        {
            if (m_CurrentScreen.GetType() != typeof(Screens.TilesetScreen))
                InitScreen(new Screens.TilesetScreen());
        }

        private void MenuChunk_Click(Widget widget)
        {
            if (m_CurrentScreen.GetType() != typeof(Screens.ChunkScreen))
                InitScreen(new Screens.ChunkScreen());
        }

        private void MenuMap_Click(Widget widget)
        {
            if (m_CurrentScreen.GetType() != typeof(Screens.MapScreen))
                InitScreen(new Screens.MapScreen());
        }

        private void MenuSprites_Click(Widget widget)
        {
            if (m_CurrentScreen.GetType() != typeof(Screens.SpriteScreen))
                InitScreen(new Screens.SpriteScreen());
        }

        private void MenuActors_Click(Widget widget)
        {
            if (m_CurrentScreen.GetType() != typeof(Screens.ActorScreen))
                InitScreen(new Screens.ActorScreen());
        }

        Elements.Menu mnu_MenuBar;
    }
}
