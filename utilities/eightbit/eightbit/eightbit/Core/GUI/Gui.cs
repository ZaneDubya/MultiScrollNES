using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Core.GUI.Framework;

namespace Core.GUI.Framework {

    public class Gui {
       
        // Internal System Managers
        internal InputManager InputManager { get; private set; }
        internal RenderManager RenderManager { get; private set; }

        /*####################################################################*/
        /*                           Initialization                           */
        /*####################################################################*/

        public Gui(Game game, Skin defaultSkin, Font defaultText,
            IEnumerable<Tuple<string, Skin>> skins = null,
            IEnumerable<Tuple<string, Font>> textRenderers = null)
        {
            InitWidgets();

            NewState = OldState = new MouseState();

            InputManager = new InputManager(game, m_Widgets);
            Core.Graphics.SpriteBatchExtended SBX = (Core.Graphics.SpriteBatchExtended)game.Components[0];
            RenderManager = new RenderManager(game.GraphicsDevice, SBX);

            SetDefaultSettings(game, defaultSkin, defaultText);

            if (skins != null)
            {
                foreach (var skin in skins)
                {
                    AddSkin(skin.Item1, skin.Item2);
                }
            }

            if (textRenderers != null)
            {
                foreach (var textRenderer in textRenderers)
                {
                    AddText(textRenderer.Item1, textRenderer.Item2);
                }
            }
        }

        /*####################################################################*/
        /*                           Widget Management                        */
        /*####################################################################*/

        private List<Widget> m_Widgets;

        private void InitWidgets()
        {
            m_Widgets = new List<Widget>();
        }

        public Widget[] Widgets
        {
            get
            {
                return m_Widgets.ToArray();
            }
            set
            {
                m_Widgets.Clear();
                AddWidgets(value);
            }
        }

        public void AddWidget(Widget widget)
        {
            m_Widgets.Add(widget);
            widget.Initialize(this);
        }

        public void AddWidgets(IEnumerable<Widget> widgets)
        {
            foreach (Widget w in widgets)
                AddWidget(w);
        }

        public void RemoveWidget(Widget widget)
        {
            m_Widgets.Remove(widget);
        }

        public void RemoveAllWidgets()
        {
            m_Widgets.Clear();
        }

        /*####################################################################*/
        /*                             Game Loop                              */
        /*####################################################################*/


        public void Resize()
        {
            foreach (Widget w in m_Widgets)
            {
                w.ClipArea = RenderManager.GraphicsDevice.Viewport.Bounds;
                w.Layout();
            }
        }

        internal MouseState NewState, OldState;

        public void Update()
        {
            InputManager.Update();

            NewState = Mouse.GetState();

            foreach (Widget widget in m_Widgets)
            {
                if (!widget.Active)
                    continue;
                widget.Update();
            }

            OldState = NewState;
        }

        public void Draw()
        {
            RenderManager.Draw(m_Widgets);
        }

        /*####################################################################*/
        /*                              Settings                              */
        /*####################################################################*/

        #region Settings

        private void SetDefaultSettings(Game game, Skin defaultSkin, Font defaultText)
        {

            DefaultScrollSpeed = 3;
            DefaultWheelSpeed = 6;

            SelectionColor = new Texture2D(game.GraphicsDevice, 1, 1);
            HighlightingColor = Color.LightSkyBlue * 0.3f;

            AddSkin("Default", defaultSkin);
            DefaultSkin = "Default";

            AddText("Default", defaultText);
            DefaultText = "Default";
        }

        public Rectangle ScreenBounds { get { return RenderManager.GraphicsDevice.Viewport.Bounds; } }

        public int DefaultScrollSpeed { get; set; }
        public int DefaultWheelSpeed { get; set; }

        public Texture2D SelectionColor
        {
            get { return RenderManager.SelectionColor; }
            set { RenderManager.SelectionColor = value; }
        }

        public Color HighlightingColor
        {
            get { return RenderManager.HighlightingColor; }
            set { RenderManager.HighlightingColor = value; }
        }

        public string DefaultSkin
        {
            get { return RenderManager.DefaultSkin; }
            set { RenderManager.DefaultSkin = value; }
        }

        public string DefaultText
        {
            get { return RenderManager.DefaultFontName; }
            set { RenderManager.DefaultFontName = value; }
        }

        public SpriteFont DefaultFont
        {
            get { return RenderManager.SpriteFonts[DefaultText].SpriteFont; }
        }

        public void AddSkin(string name, Skin skin)
        {
            RenderManager.AddSkin(name, skin);
        }

        public void AddText(string name, Font renderer)
        {
            RenderManager.AddText(name, renderer);
        }

        #endregion

        /*####################################################################*/
        /*                        Event Based Input                           */
        /*####################################################################*/

        internal event Core.Input.KeyboardEventHandler KeyboardCharPress
        {
            add { InputManager.KeyboardCharPress += value; }
            remove { InputManager.KeyboardCharPress -= value; }
        }

        /*
        #region Input

        public bool HasMouse { get { return InputManager.HoverWidget != null; } }

        public event KeyEventHandler KeyDown
        {
            add { InputSystem.KeyDown += value; }
            remove { InputSystem.KeyDown -= value; }
        }

        public event KeyEventHandler KeyUp
        {
            add { InputSystem.KeyUp += value; }
            remove { InputSystem.KeyUp -= value; }
        }

        public event MouseEventHandler MouseDoubleClick
        {
            add { InputSystem.MouseDoubleClick += value; }
            remove { InputSystem.MouseDoubleClick -= value; }
        }

        public event MouseEventHandler MouseDown
        {
            add { InputSystem.MouseDown += value; }
            remove { InputSystem.MouseDown -= value; }
        }

        public event MouseEventHandler MouseUp
        {
            add { InputSystem.MouseUp += value; }
            remove { InputSystem.MouseUp -= value; }
        }

        public event MouseEventHandler MouseWheel
        {
            add { InputSystem.MouseWheel += value; }
            remove { InputSystem.MouseWheel -= value; }
        }

        #endregion
        */
    }
}
