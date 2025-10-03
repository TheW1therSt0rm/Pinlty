using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;

namespace Pinlty
{
    public class Window(GameWindowSettings gws, NativeWindowSettings nws) : GameWindow(gws, nws)
    {
        public Vector2 _size = nws.ClientSize.ToVector2();
        public Shader _shader;
        public Engine _engine;

        protected override void OnLoad()
        {
            base.OnLoad();

            GL.Enable(EnableCap.DepthTest);
            GL.ClearColor(0.07f, 0.13f, 0.17f, 1.0f);

            // Create shader AFTER context is ready
            _shader = _engine._shader;
        }

        protected override void OnUnload()
        {
            base.OnUnload();
            _shader?.Clean();
            // Dispose VAOs, VBOs, Textures, etc.
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, e.Width, e.Height);
            GL.ClearColor(0.07f, 0.13f, 0.17f, 1.0f);
            _size = new(e.Width, e.Height);
        }
    }
}