using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Core.GUI.Framework;
using SBX = Core.Graphics.SpriteBatchExtended;

namespace Core.GUI.Framework {

    public class RenderManager {

        private RasterizerState RasterizerState { get; set; }

        public GraphicsDevice GraphicsDevice { get; private set; }
        public SBX SpriteBatch { get; private set; }

        public string DefaultSkin { get; set; }
        public string DefaultFontName { get; set; }

        public Dictionary<string, Skin> Skins { get; private set; }
        public Dictionary<string, Font> SpriteFonts { get; private set; }

        private DepthStencilState ApplyStencil { get; set; }
        private DepthStencilState SampleStencil { get; set; }

        public Texture2D SelectionColor { get; set; }
        public Color HighlightingColor {
            get {
                var data = new Color[1];
                SelectionColor.GetData(data);
                return data[0];
            }
            set {
                SelectionColor.SetData(new[] { value });
            }
        }

        internal RenderManager(GraphicsDevice device, SBX batch) {

            Skins = new Dictionary<string, Skin>();
            SpriteFonts = new Dictionary<string, Font>();

            ApplyStencil = new DepthStencilState {
                StencilEnable = true,
                StencilFunction = CompareFunction.Always,
                StencilPass = StencilOperation.Replace,
                ReferenceStencil = 1,
                DepthBufferEnable = false,
            };

            SampleStencil = new DepthStencilState {
                StencilEnable = true,
                StencilFunction = CompareFunction.Equal,
                StencilPass = StencilOperation.Keep,
                ReferenceStencil = 1,
                DepthBufferEnable = false,
            };

            GraphicsDevice = device;
            RasterizerState = new RasterizerState { ScissorTestEnable = true };
            SpriteBatch = batch;
        }

        /*####################################################################*/
        /*                         State Management                           */
        /*####################################################################*/

        internal void Draw(List<Widget> widgets)
        {
            foreach (Widget widget in widgets)
                drawWidgetAndChildren(widget, true);

            SpriteBatch.GUIClipRect = GraphicsDevice.Viewport.Bounds;

            foreach (Widget widget in widgets)
                drawWidgetAndChildren(widget, false);
        }

        private void drawWidgetAndChildren(Widget widget, bool clip)
        {
            if (!widget.Visible)
                return;

            if (clip)
            {
                SpriteBatch.GUIClipRect = (widget.Parent == null) ? GraphicsDevice.Viewport.Bounds : widget.Parent.ClipArea;
                widget.Draw();
            }
            else
                widget.DrawNoClipping();

            if (widget.Children != null)
            {
                foreach (Widget w in widget.Children)
                    drawWidgetAndChildren(w, clip);
            }
        }

        /*####################################################################*/
        /*                          Skin Management                           */
        /*####################################################################*/

        internal void AddSkin(string name, Skin skin) {
            Skins.Add(name, skin);
        }

        internal void AddText(string name, Font textRenderer) {
            SpriteFonts.Add(name, textRenderer);
        }
    }
}
