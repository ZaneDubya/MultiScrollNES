using Microsoft.Xna.Framework;
using Core.GUI.Content;
using Core.GUI.Framework;

namespace eightbit.GUI.Screens
{
    class FileScreen : Screen
    {
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

        public override void OnInit()
        {
            Gui.AddWidgets(new Widget[]
            {
                new Button(8,32,0,"New data", click_NewData),
                new Button(8,32+38*1,0,"Load data", click_LoadData),
                new Button(8,32+38*2,0,"Save data", click_SaveData),
                new Button(8,32+38*3,0,"Export data", click_ExportData),
            });
        }

        private void click_NewData(Widget widget)
        {
            State.Data = new Data.Manager();
            Manager.ResetMenu();
        }

        private void click_LoadData(Widget widget)
        {
            State.Data = new Data.Manager();
            if (State.LoadData())
            {
                Manager.ResetMenu();
            }
        }

        private void click_SaveData(Widget widget)
        {
            if (State.Data == null)
            {
                return;
            }
            State.SaveData();
        }

        private void click_ExportData(Widget widget)
        {
            if (State.Data == null)
            {
                return;
            }
            State.ExportData();
        }
    }
}
