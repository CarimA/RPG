using System;
using PhotoVs.Logic;
using PhotoVs.Platform.Desktop;

namespace PhotoVs.Core.Windows
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using var game = new MainGame(new WindowsPlatform());
            game.Run();
        }
    }
}
