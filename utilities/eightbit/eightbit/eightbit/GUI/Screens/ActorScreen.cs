using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Core.GUI.Content;
using Core.GUI.Framework;
using eightbit.GUI.Elements;
using Microsoft.Xna.Framework.Graphics;
using Core.Input;

namespace eightbit.GUI.Screens
{
    class ActorScreen : Screen
    {
        // ================================================================================
        // Overrides
        // ================================================================================

        public override void OnResize()
        {
            Gui.Resize();
        }

        public override void Update()
        {
            Gui.Update();
        }

        public override void Draw()
        {
            Gui.Draw();
        }

        // ================================================================================
        // Controls
        // ================================================================================

        Window panel_Control, panel_List, panel_CurrentSprite, panel_TileGfx;
        // main control
        Label lblSpriteCount;
        ComboBox cmbPalettes;
        // list
        ListBox<Data.Sprite> lstSprites;

        // ================================================================================
        // Initialization
        // ================================================================================

        public override void OnInit()
        {
            int leftcolumn = 120;

            // Add panels: Control, List, CurrentSprite, and TileGfx
            Gui.AddWidgets(new Widget[] {
                panel_Control = new Window(4, 4 + 24, leftcolumn, 84),
                panel_List = new Window(4, 4 + panel_Control.Area.Bottom, leftcolumn, 592 - 28 - panel_Control.Area.Height),
                panel_CurrentSprite = new Window(leftcolumn + 8, 4 + 24, 800 - leftcolumn - 12, 592 - 24 - 4 - 276),
                panel_TileGfx = new Window(leftcolumn + 8, 4 + panel_CurrentSprite.Area.Bottom, panel_CurrentSprite.Area.Width, 276)
            });

            panel_Control.AddWidgets(new Widget[] {
                lblSpriteCount = new Label(8, 14, string.Empty),
                new Button(leftcolumn - 82, 4, 32, "+", click_AddSprite),
                new Button(leftcolumn - 46, 4, 32, "-", click_RemoveSprite)
            });
        }

        // ================================================================================
        // Control management
        // ================================================================================

        private void refreshControls()
        {
            int count = State.Data.Sprites.Count;
            lblSpriteCount.Value = string.Format("{0}", count);

            if (count == 0 || State.Data.Palettes.Count == 0 || State.Data.TileGfx.PageCount < 2)
            {
                if (cmbPalettes != null)
                    cmbPalettes.Visible = false;
                panel_List.Visible = false;
                panel_CurrentSprite.Visible = false;
                panel_TileGfx.Visible = false;
            }
            else
            {
                // load all palettes into combo viewer...
                // CreatePaletteCombo();
                return;
                cmbPalettes.Visible = true;
                if (State.SelectedTileset < State.Data.TileSets.Count)
                    cmbPalettes.SelectedIndex = State.SelectedTileset;
                else
                    cmbPalettes.SelectedIndex = State.SelectedTileset = 0;

                panel_List.Visible = true;
                lstSprites.SetItems(State.Data.Sprites.ToArray());

                panel_CurrentSprite.Visible = true;
                panel_TileGfx.Visible = true;
            }
        }

        // ================================================================================
        // Input
        // ================================================================================

        private void click_AddSprite(Widget widget)
        {
            State.Data.Sprites.AddSprite();
            refreshControls();
            lstSprites.SelectedIndex = State.Data.Sprites.Count - 1;
        }

        private void click_RemoveSprite(Widget widget)
        {
            State.Data.Sprites.RemoveLastSprite();
            refreshControls();
            if (State.SelectedSprite >= State.Data.Sprites.Count)
            {
                State.SelectedSprite = lstSprites.SelectedIndex = State.Data.Sprites.Count - 1;
            }
        }
    }
}
