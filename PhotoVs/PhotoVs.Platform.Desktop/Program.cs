using System;
using PhotoVs.Logic;

namespace PhotoVs.Platform.Desktop
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using var game = new MainGame(new DesktopPlatform());
            game.Run();
        }
    }
}
