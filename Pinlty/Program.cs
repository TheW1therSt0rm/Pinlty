namespace Pinlty
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            using var window = new MainForm();
            window.Run();
            window.OnUpdateFrame();
        }
    }
}