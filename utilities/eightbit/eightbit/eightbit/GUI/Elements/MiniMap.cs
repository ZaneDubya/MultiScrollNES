using Core.GUI.Framework;
using Core.Input;
using Microsoft.Xna.Framework;
using System;

namespace eightbit.GUI.Elements
{
    class MiniMap : WidgetBase<Renderers.MiniMapRenderRule>
    {
        public MiniMap(int x, int y, int width, int height, WidgetEvent onClick)
        {
            Area = new Rectangle(x, y, width, height);
            RenderRule.SetSize(width, height);
            OnClick += onClick;
        }

        protected override Renderers.MiniMapRenderRule BuildRenderRule()
        {
            return new Renderers.MiniMapRenderRule(this);
        }

        public void RefreshEntireMap(Data.Map map)
        {
            if (m_RenderingMap)
            {
                m_Cancel = true;
                while (m_RenderingMap)
                    System.Threading.Thread.Sleep(1);
            }
            m_Cancel = false;
            m_Map = map;
            Core.ParallelTasks.Parallel.StartBackground(new Action(refreshEntireMap));
        }

        private Data.Map m_Map;
        private bool m_Cancel = false;
        private bool m_RenderingMap = false;
        public bool RenderInProcess { get { return m_RenderingMap; } }

        private void refreshEntireMap()
        {
            try
            {
                m_RenderingMap = true;
                int w = m_Map.WidthInSuperChunks;
                int h = m_Map.HeightInSuperChunks;
                RenderRule.TextureDimension = w * 2 * 2;
                for (int i = 0; i < w * h; i++)
                {
                    for (int j = 0; j < 4; j++)
                    {

                        RenderRule.WriteChunk((i % w) * 2 + (j % 2), (i / w) * 2 + (j / 2),
                            State.Data.Chunks[m_Map.GetSuperChunk(i % w, (i / w)).Chunks[j]],
                            State.Data.TileSets[State.SelectedTileset],
                            State.Data.TileGfx);
                        if (m_Cancel)
                        {
                            m_RenderingMap = false;
                            return;
                        }
                    }
                }
            }
            catch
            {
                m_RenderingMap = false;
                return;
            }
            m_RenderingMap = false;
        }

        protected override void Attach() { }
        protected internal override void OnUpdate() { }
        protected internal override void OnLayout() { }

        public WidgetEvent OnClick;

        private Point m_ClickedPoint = Point.Zero;
        public Point ClickedPoint
        {
            protected set { m_ClickedPoint = value; }
            get { return m_ClickedPoint; }
        }

        protected internal override void MouseDown(InputEventMouse e)
        {
            if (e.Button == MouseButton.Left)
            {
                int x = (e.Position.X - InputArea.X);
                int y = (e.Position.Y - InputArea.Y);
                ClickedPoint = new Point(x, y);

                if (OnClick != null)
                    OnClick(this);
            }
        }

        protected internal override void MouseMove(InputEventMouse e)
        {
            if (e.Button == MouseButton.Left)
            {
                MouseDown(e);
            }
        }
    }
}
