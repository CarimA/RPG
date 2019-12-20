using Microsoft.Xna.Framework;
using PhotoVs.Models.FSM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoVs.Logic.Scenes
{
    public class TextInputScene : IUpdateableScene, IDrawableScene
    {
        private SceneMachine _scene;

        public bool IsBlocking { get; set; } = false;

        private char[][] _keyboard =
        {
            "ABCDEFGHIJKLM".ToCharArray(),
            "NOPQRSTUVWXYZ".ToCharArray(),
            "abcdefghijklm".ToCharArray(),
            "nopqrstuvwxyz".ToCharArray(),
            "0123456789-. ".ToCharArray(),
        };

        public TextInputScene(SceneMachine scene)
        {
            _scene = scene;
        }

        public void Enter(params object[] args)
        {
            var question = args[0].ToString();
            var limit = args.Length > 1 
                ? int.Parse(args[1].ToString()) 
                : 15;
            var defaultText = args.Length > 2
                ? args[2].ToString()
                : string.Empty;
        }

        public void Exit()
        {

        }

        public void Resume()
        {

        }

        public void Suspend()
        {

        }

        public void Update(GameTime gameTime)
        {

        }

        public void Draw(GameTime gameTime)
        {

        }
    }
}
