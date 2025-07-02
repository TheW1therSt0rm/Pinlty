namespace Pinlty
{
    public static class Input
    {
        public static OpenTK.Windowing.GraphicsLibraryFramework.KeyboardState? Keyboard;
        public static OpenTK.Windowing.GraphicsLibraryFramework.MouseState? Mouse;

        public static void Update(OpenTK.Windowing.GraphicsLibraryFramework.KeyboardState? kb, OpenTK.Windowing.GraphicsLibraryFramework.MouseState? ms)
        {
            Keyboard = kb;
            Mouse = ms;
        }
    }
}