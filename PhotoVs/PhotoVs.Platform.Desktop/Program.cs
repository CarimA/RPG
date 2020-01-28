using System;
using PhotoVs.Logic;

namespace PhotoVs.Core.Desktop
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using var game = new MainGame();
            game.Run();
        }
    }
}
