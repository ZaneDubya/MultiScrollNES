using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace eightbit
{
    public class EBThread : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager m_Graphics;
        Core.Settings m_Settings;
        Core.InputState m_Input;
        Core.Graphics.SpriteBatchExtended m_SBX;
        GUI.GUIManager m_GUI;

        NES.VRAM m_VRAM;
        NES.NameTable m_NameTable;
        Galezie.Manager m_Galezie;

        public EBThread()
        {
            Content.RootDirectory = "Content";

            m_Graphics = new GraphicsDeviceManager(this);
            m_Graphics.IsFullScreen = false;
            m_Graphics.PreferredBackBufferWidth = 256;
            m_Graphics.PreferredBackBufferHeight = 240;

            m_Input = new Core.InputState();
            m_Input.Initialize(this.Window.Handle);

            m_SBX = new Core.Graphics.SpriteBatchExtended(this);
            this.Components.Add(m_SBX);

            this.IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            Core.Library.Content = new ResourceContentManager(Services, ResContent.ResourceManager);
            base.Initialize();
            m_Settings.Resolution = new Point(800, 600);
        }

        protected override void LoadContent()
        {
            m_Settings = new Core.Settings();

            Core.Library.Initialize(m_Settings, m_Graphics.GraphicsDevice, m_Input);

            State.Palette = new NES.Palette();

            m_VRAM = new NES.VRAM();
            m_VRAM.Randomize();

            m_NameTable = new NES.NameTable();
            m_NameTable.RandomizeTiles();
            m_NameTable.RandomizeAttributes();

            m_Galezie = new Galezie.Manager();

            m_GUI = new GUI.GUIManager(this);
        }

        protected override void UnloadContent()
        {
            m_SBX.Dispose();
        }

        protected override void Update(GameTime gameTime)
        {
            m_Input.Update(gameTime);

            if (m_Input.HandleKeyboardEvent(Core.Input.KeyboardEvent.Press, Core.Input.WinKeys.Escape, false, false, false))
                this.Exit();

            if (m_Settings.HasUpdates)
                handleUpdates();

            base.Update(gameTime);

            m_GUI.Update();
        }

        protected override void Draw(GameTime gameTime)
        {
            m_SBX.PaletteTexture = State.Palette.Texture;
            // m_SBX.DrawSprite(State.Palette.Texture, new Vector3(8, 24, 0), new Vector2(128, 4));
            m_GUI.Draw();
            base.Draw(gameTime);
            // m_SBX.DrawTriangleList(m_VRAM.Texture, new Vector3(8, 8, 0), m_NameTable.Vertexes);
        }

        private void handleUpdates()
        {
            Core.Settings.Setting s;
            while ((s = m_Settings.NextUpdate()) != Core.Settings.Setting.None)
            {
                switch (s)
                {
                    case Core.Settings.Setting.Resolution:
                        m_Graphics.PreferredBackBufferWidth = m_Settings.Resolution.X;
                        m_Graphics.PreferredBackBufferHeight = m_Settings.Resolution.Y;
                        m_Graphics.ApplyChanges();
                        if (m_GUI != null) { m_GUI.OnResize(); }
                        break;
                    default:
                        throw new Exception("Setting not handled.");
                }
            }
        }
    }

    static class Program
    {
        static void Main(string[] args)
        {
            using (EBThread game = new EBThread())
            {
                game.Run();
            }
        }
    }
}
