using System;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace Pinlty
{
    static class Program
    {
        static void Main()
        {
            var nws = new NativeWindowSettings()
            {
                ClientSize = new Vector2i(1280, 720),
                Title = "Pinlty",
                DepthBits = 24,
            };

            var window = new Window(GameWindowSettings.Default, nws);
            var engine = new Engine(window, 60.0f);
            window._engine = engine;
            engine.Run();
        }
    }
}