using PhotoVs.Logic;
using System;

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
