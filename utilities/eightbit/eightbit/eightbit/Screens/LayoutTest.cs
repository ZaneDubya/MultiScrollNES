using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RuminateGui.Content;
using RuminateGui.Framework;

namespace eightbit.Screens
{

    /// <summary>
    /// This screen is for debugging purposes to make sure that I haven't
    /// broken the the layout engine again.
    /// </summary>
    public class LayoutTest : Screen {

        Texture2D _beaker;

        public override void Init(Game game, GUIData data) {

            Color = Color.White;

            _beaker = game.Content.Load<Texture2D>("beaker");

            var skin = new Skin(data.GreyImageMap, data.GreyMap);
            var text = new Text(data.GreySpriteFont, Color.LightGray);

            Gui = new Gui(game, skin, text) {
                Widgets = new Widget[] {
                    new ScrollBars {
                        Children = new Widget[] {
                            new Panel(10, 10, 1000, 1000) {
                                Children = new Widget[] {
                                    new ScrollBars {
                                        Children = new Widget[] {
                                            new Button(10, 10, "Test 1"),
                                            new Button(10, 50, "Test 2"),
                                            new Button(10, 90, "Test 3"),
                                            new Button(10, 130, "Test 4"),
                                            new Button(10, 170, "Test 5"),
                                            new Button(10, 210, "Test 6"),
                                            new Button(10, 250, "Test 7"),
                                            new Button(10, 290, "Test 8"),
                                            new Button(10, 330, "Test 9"),
                                            new Button(10, 370, "Test 10"),
                                            new Button(10, 410, "Test 11"),
                                            new Button(10, 450, "Test 12"),
                                            new Button(10, 490, "Test 13"),
                                            new Button(10, 530, "Test 14"),
                                            new Button(10, 570, "Test 15"),
                                            new Button(10, 610, "Test 16"),
                                            new Panel(100, 10, 200, 200) {
                                                Children = new Widget[] {
                                                    new Button(10, 10, "Test 1"),
                                                    new Button(10, 50, "Test 2"),
                                                    new Button(10, 90, "Test 3")      
                                                }
                                            },
                                            new Panel(100, 230, 400, 400) {
                                                Children = new Widget[] {
                                                    new ScrollBars {
                                                        Children = new Widget[] {
                                                            new Button(10, 10, "Test 1"),
                                                            new Button(10, 50, "Test 2"),
                                                            new Button(10, 90, "Test 3"),
                                                            new Button(10, 130, "Test 4"),
                                                            new Button(10, 170, "Test 5"),
                                                            new Button(10, 210, "Test 6"),
                                                            new Button(10, 250, "Test 7"),
                                                            new Button(10, 290, "Test 8"),
                                                            new Button(10, 330, "Test 9"),
                                                            new Button(10, 370, "Test 10"),
                                                            new Button(10, 410, "Test 11"),
                                                            new Button(10, 450, "Test 12"),
                                                            new Button(10, 490, "Test 13"),
                                                            new Button(10, 530, "Test 14"),
                                                            new Button(10, 570, "Test 15"),
                                                            new Button(10, 610, "Test 16"),
                                                            new Panel(100, 10, 600, 600) {
                                                                Children = new Widget[] { 
                                                                    new ScrollBars {
                                                                        Children = new Widget[] {
                                                                            new Button(10, 10, "Button"),
                                                                            new ToggleButton(10, 50, "Toggle Button"),
                                                                            new Panel(10, 90, 120, 120),
                                                                            new CheckBox(10, 215, "Check Box"),
                                                                            new RadioButton(10, 255, "GRP", "Radio 1"),
                                                                            new RadioButton(10, 285, "GRP", "Radio 2"),
                                                                            new RadioButton(10, 315, "GRP", "Radio 3"),
                                                                            new Label(10, 340, "Research"),
                                                                            new Label(10, 365, _beaker, "Research"),
                                                                            new Panel(140, 90, 220, 220) {
                                                                                Children = new Widget[] {
                                                                                    new TextBox(2, 600)
                                                                                }
                                                                            },
                                                                            new Panel(370, 70, 220, 220) {
                                                                                Children = new Widget[] {
                                                                                    new ScrollBars {
                                                                                        Children = new Widget[] {
                                                                                            new CheckBox(10, 10, "Button"),
                                                                                            new CheckBox(210, 10, "Button"),
                                                                                            new CheckBox(10, 210, "Button"),
                                                                                            new CheckBox(210, 210, "Button"),
                                                                                            new Panel(10, 230, 300, 300) {
                                                                                                Children = new Widget[] {
                                                                                                    new TextBox(2, 300)
                                                                                                }
                                                                                            }
                                                                                        }
                                                                                    }
                                                                                }
                                                                            }
                                                                        }
                                                                    }           
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            };            
        }

        public override void OnResize() {
            Gui.Resize();
        }

        public override void Update() {
            Gui.Update();
        }

        public override void Draw() {
            Gui.Draw();
        }
    }
}
