using System;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace Pinlty
{
    internal static class Program
    {
        [STAThread]
        [Obsolete]
        static void Main(string[] args)
        {
            // Default settings for game engine dev / editor use
            int width = 1600;
            int height = 900;
            bool fullscreen = false;
            bool vsync = true;

            // Create native and window settings
            var nativeSettings = new NativeWindowSettings
            {
                Title = "Pinlty Game Engine",
                Size = new Vector2i(width, height),
                WindowState = fullscreen ? WindowState.Fullscreen : WindowState.Normal,
                WindowBorder = fullscreen ? WindowBorder.Hidden : WindowBorder.Resizable,
                StartFocused = true,
                StartVisible = true
            };

            var gameSettings = new GameWindowSettings
            {
                RenderFrequency = 0.0,     // Let OpenTK choose the best rate
                UpdateFrequency = 0.0,
                IsMultiThreaded = false    // Disable for easier debugging
            };

            // Launch the engine
            using var engine = new GameWindow(gameSettings, nativeSettings)
            {
                VSync = vsync ? VSyncMode.On : VSyncMode.Off
            };

            engine.Run();
        }
    }
}
