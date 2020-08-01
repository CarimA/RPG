using System;
using PhotoVs.Logic;

namespace PhotoVs.Platform.WindowsDX
{
    public static class Program
    {
        [STAThread]
        private static void Main()
        {
            using var game = new MainGame(new WindowsDXPlatform());
            game.Run();
        }
    }
}