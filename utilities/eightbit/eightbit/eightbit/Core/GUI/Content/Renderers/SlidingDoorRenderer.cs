using Microsoft.Xna.Framework.Graphics;
using Core.GUI.Framework;

namespace Core.GUI.Content {

// ReSharper disable ClassNeverInstantiated.Global
    public abstract class SlidingDoorRenderer : Renderer {
// ReSharper restore ClassNeverInstantiated.Global

        public abstract int Across { get; }
        public abstract int Length { get; }
        public abstract int Edge { get; }
        public abstract int Buffer { get; }

        protected SlidingDoorRenderer(Texture2D imageMap) : base(imageMap) { }
    }
}
