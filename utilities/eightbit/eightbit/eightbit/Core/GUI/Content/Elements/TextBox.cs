using System;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Core.GUI.Framework;
using Timer = System.Timers.Timer;
using Core.Input;

namespace Core.GUI.Content {

    /// <summary>
    /// Plain text box.
    /// </summary>
    public class TextBox : WidgetBase<TextBoxRenderRule>, IDisposable {

        /*####################################################################*/
        /*                               Variables                            */
        /*####################################################################*/

        #region Variables

        /* ## Local Variables ## */

        private readonly Timer _cursorTimer;
        private void ResetTimer() {

            _isPressed = false;
            RenderRule.CursorVisible = true;
            _cursorTimer.Interval = 500;
        }

        private bool _isPressed;

        /* ## Exposed Variables ## */

        public WidgetEvent OnValueChanged;

        public string Value {
            get { return RenderRule.Value; } 
            set { RenderRule.Value = value; }
        }

        public int MaxCharacters {
            get { return RenderRule.MaxLength; }
            set { RenderRule.RecreateStringData(value); }
        }

        public int Padding { get; set; }

        #endregion

        /*####################################################################*/
        /*                           Initialization                           */
        /*####################################################################*/

        #region Node Initialization

        protected override TextBoxRenderRule BuildRenderRule()
        {
            return new TextBoxRenderRule(this);
        }

        /// <summary>
        /// Creates a new textbox that automatically fills the parent widget.
        /// </summary>
        /// <param name="buffer">Padding between the textbox and the parent widget.</param>
        /// <param name="maxChars"></param>
        public TextBox(int buffer, short maxChars)
        {
            RenderRule.RecreateStringData(maxChars);

            Padding = buffer;            

            _cursorTimer = new Timer(500);
            _cursorTimer.Elapsed += delegate {
                RenderRule.CursorVisible = !RenderRule.CursorVisible;
            };
        }

        public void Dispose() {
            _cursorTimer.Dispose();
        }

        protected override void Attach()
        {

        }

        protected internal override void OnLayout()
        {
            if (Area != Parent.InputArea)
            {
                var a = Parent.InputArea;
                Area = new Rectangle(0, 0, a.Width, a.Height);
            }
            RenderRule.BakeText();
        }

        #endregion

        /*####################################################################*/
        /*                                Events                              */
        /*####################################################################*/

        #region Events

        protected internal override void EnterKeyboardFocus()
        {
            ResetTimer();
            _cursorTimer.Start();
        }

        protected internal override void ExitKeyboardFocus()
        {
            RenderRule.CursorVisible = false;
            _cursorTimer.Stop();
        }

        protected internal override void CharEntered(InputEventKeyboard e)
        {
            lock (RenderRule)
            {
                switch (e.KeyChar)
                {
                    case '\b': {
                        //Backspace <= remove selected or remove before cursor
                        if (RenderRule.HasSelected) {
                            RenderRule.RemoveSelected();
                        } else {
                            RenderRule.BackSpace();
                        }
                        break;
                    } case '\n': case '\r': {
                        //New Line <= Insert character after cursor                        
                        if (RenderRule.HasSelected) { RenderRule.RemoveSelected(); }
                        RenderRule.Insert(e.KeyChar);
                        break;                    
                    } case '\t': { 
                        //Tabs currently not supported
                        return;
                    } case (char)3: {
                        //Copy                        
                        Clipboard.SetDataObject(RenderRule.GetSelected(), true);
                        break;                    
                    } case (char)22: {
                        //Paste
                        if (RenderRule.HasSelected) { RenderRule.RemoveSelected(); }

                        var dataObject = Clipboard.GetDataObject();
                        if (dataObject != null) {
                            var text = dataObject.GetData(DataFormats.Text).ToString();
                            RenderRule.Insert(text);
                        }
                        break;
                    } case (char)24: {
                        //Cut
                        if (RenderRule.HasSelected) {
                            Clipboard.SetDataObject(RenderRule.GetSelected(), true);
                            RenderRule.RemoveSelected();
                        }
                        break;
                    } default: { 
                        //Add the character
                        if (RenderRule.HasSelected) {
                            RenderRule.RemoveSelected();
                        }
                        RenderRule.Insert(e.KeyChar);
                        RenderRule.SelectedChar = null;
                        break;
                    }
                }
                RenderRule.BakeText();
                if (OnValueChanged != null)
                    OnValueChanged(this);
            }

            ResetTimer();
        }

        protected internal override void MouseDoubleClick(InputEventMouse e) {
            MouseDown(e);
        }

        protected internal override void MouseDown(InputEventMouse e) {

            _isPressed = true;

            lock (RenderRule) {
                RenderRule.SetSelection(
                    new Point(
                        e.Position.X - RenderRule.Area.X,
                        e.Position.Y - RenderRule.Area.Y));
                RenderRule.SetTextCursor(
                    new Point(
                        e.Position.X - RenderRule.Area.X,
                        e.Position.Y - RenderRule.Area.Y));
            }
        }

        protected internal override void MouseMove(InputEventMouse e) {

            if (!_isPressed) { return; }

            lock (RenderRule) {
                RenderRule.SetTextCursor(
                    new Point(
                        e.Position.X - RenderRule.Area.X,
                        e.Position.Y - RenderRule.Area.Y));
            }
        }

        protected internal override void MouseUp(InputEventMouse e) {
            
            _isPressed = false;
            lock (RenderRule) {
                RenderRule.SetTextCursor(
                    new Point(
                        e.Position.X - RenderRule.Area.X,
                        e.Position.Y - RenderRule.Area.Y));
            }
        }

        protected internal override void KeyDown(InputEventKeyboard e) {

            lock (RenderRule)
            {                
                if (RenderRule.Length != 0)
                {
                    switch (e.KeyCode)
                    {
                        case WinKeys.Left:
                            RenderRule.TextCursor--;
                            RenderRule.SelectedChar = null;
                            break;
                        case WinKeys.Right:
                            RenderRule.TextCursor++;
                            RenderRule.SelectedChar = null;
                            break;
                        case WinKeys.Up:
                            RenderRule.CursorUp();
                            RenderRule.SelectedChar = null;
                            break;
                        case WinKeys.Down:
                            RenderRule.CursorDown();
                            RenderRule.SelectedChar = null;
                            break;
                        case WinKeys.Home:
                            RenderRule.CursorHome();
                            RenderRule.SelectedChar = null;
                            break;
                        case WinKeys.End:
                            RenderRule.CursorEnd();
                            RenderRule.SelectedChar = null;
                            break;
                        case WinKeys.PageUp:                       
                            RenderRule.TextCursor = 0;
                            RenderRule.SelectedChar = null;
                            break;
                        case WinKeys.PageDown:                        
                            RenderRule.TextCursor = RenderRule.Length;
                            RenderRule.SelectedChar = null;
                            break;
                        case WinKeys.Delete:                           
                            RenderRule.Delete();
                            RenderRule.BakeText();
                            RenderRule.SelectedChar = null;
                            break;
                    }
                }                
            }

            ResetTimer();
        }

        #endregion

        protected internal override void OnUpdate() { }
    }
}