using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RuminateGui.Content;
using RuminateGui.Framework;

namespace eightbit.Screens
{
    /// <summary>
    /// This screen is for demonstrating the available widget and their capabilities. 
    /// </summary>
    public class WidgetDemonstration : Screen {

        Slider _slider;
        Label _sliderLabel;
        SingleLineTextBox _singleLineTextBox;

        public override void Init(Game game, GUIData data)
        {

            Color = new Color(244, 246, 250);

            var beaker = game.Content.Load<Texture2D>("beaker");

            var skin = new Skin(data.GreyImageMap, data.GreyMap);
            var text = new Text(data.GreySpriteFont, Color.Black);

            Gui = new Gui(game, skin, text) {
                Widgets = new Widget[] {                                           
                    //By default the Button is as wide as the width of the label plus the edge of the button graphic
                    new Button(10, 10 + (40 * 0), "Button"),
                    new Button(10, 10 + (40 * 1), "Wide Button"),
                    new Button(10, 10 + (40 * 2), "T"),
                    new Button(10, 10 + (40 * 3), 120, "Width 120"),
                    //Button will resized to fit if the specified width is smaller than the width of the text
                    new Button(10, 10 + (40 * 4), 20, "Width 20"), 
                    //The optional padding argument causes the button to be as wide as the label + (padding * 2)
                    new Button(10, 10 + (40 * 5), "5 Padding", 5),
                    new Button(10, 10 + (40 * 6), "10 Padding", 10),                    

                    //The ToggleButton behaves similarly to the Button but toggles between being 
                    //pressed or released each time its clicked.
                    new ToggleButton(150, 10 + (40 * 0), "Button"), 
                    new ToggleButton(150, 10 + (40 * 1), "Wide Button"),
                    new ToggleButton(150, 10 + (40 * 2), "T"),
                    new ToggleButton(150, 10 + (40 * 3), 120, "Width 120"),
                    new ToggleButton(150, 10 + (40 * 4), 20, "Width 20"), 
                    new ToggleButton(150, 10 + (40 * 5), "5 Padding", 5),
                    new ToggleButton(150, 10 + (40 * 6), "10 Padding", 10),
                    
                    //Standard old checkbox
                    new CheckBox(300, 10, "Check Box"),

                    //Standard old radio button
                    //Only one radio buttons in the same group can have a value of true
                    new RadioButton(300, 40, "GRP", "Group GRP"),
                    new RadioButton(300, 70, "GRP", "Group GRP"),
                    new RadioButton(300, 100, "GRP", "Group GRP"),

                    //Slider allowing anologue selection. Value is the percent selected between 0 and 1 inclusive.                    
                    _slider = new Slider(300, 130, 200, delegate(Widget slider) {
                        _sliderLabel.Value = "Value = " + ((Slider)slider).Value;
                    }),
                    _sliderLabel = new Label(300, 160, "Value = 0"),

                    _singleLineTextBox = new SingleLineTextBox(300, 180, 100, 10),
                    new Button(410, 177, 100, "Change", delegate {
                        var result = 0.0f;
                        if (float.TryParse(_singleLineTextBox.Value, out result)) {
                            _slider.Value = result;
                        }
                    }),

                    //Combo box allowing that expands to allow the user to select one of the items in the DropDownItem List.
                    new ComboBox(300, 210, "Pick a Color", 2, CardinalDirection.North, new List<ComboBox.DropDownItem> { //The padding argument behaves the same as for the Button
                        new ComboBox.DropDownItem("Violet", null, delegate { Color = Color.Violet; }),
                        new ComboBox.DropDownItem("Tomato", null, delegate { Color = Color.Tomato; }),
                        new ComboBox.DropDownItem("YellowGreen", null, delegate { Color = Color.YellowGreen; }),
                        new ComboBox.DropDownItem("LightSkyBlue", null, delegate { Color = Color.LightSkyBlue; })
                    }),
                    new ComboBox(300, 250, 131, "Holder Text", CardinalDirection.South, new List<ComboBox.DropDownItem> {
                        new ComboBox.DropDownItem("Test 1"),
                        new ComboBox.DropDownItem("Test 2"),
                        new ComboBox.DropDownItem("Test 3"),
                        new ComboBox.DropDownItem("Test 4"),
                        new ComboBox.DropDownItem("Test 5")
                    }),                                                              

                    //For labels with icons the label is centered in the height of the icon
                    new Label(450, 10, "Research"),
                    new Label(450, 40, beaker, "Research"),
                    new Label(450, 70, beaker, "Research", 4), //Use the optional field for padding                                         

                    //ScrollBars no longer have borders so nest them in panels if you need them.
                    //Panels have a min size of twice the renderers border width.
                    new Panel(10, 300, 220, 220) {
                        Children = new Widget[] {
                            new ScrollBars { 
                                Children = new Widget[] {
                                    new CheckBox(10, 10, "Button"),
                                    new CheckBox(210, 10, "Button"),
                                    new CheckBox(10, 210, "Button"),
                                    new CheckBox(210, 210, "Button")  
                                }
                            }
                        }
                    },

                    //TextBoxs no longer have borders so nest them in panels if you need them.
                    new Panel(240, 300, 400, 400) {
                        Children = new Widget[] {
                            new TextBox(2, 800) { Value = "This is a textbox!" }
                        }
                    },

                    new SingleLineTextBox(10, 525, 120, 10), //Basic Test
                    new SingleLineTextBox(10, 550, 120, 10) { Value = "0123456789" }, //Test with default value 
                    new SingleLineTextBox(10, 575, 120, 10) { Value = "0123456789" } //Test with specified font
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
