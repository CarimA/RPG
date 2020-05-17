using PhotoVs.Logic;
using System;

namespace PhotoVs.Platform.WindowsDX
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using var game = new MainGame(new WindowsDXPlatform());
            game.Run();
        }
    }
}
