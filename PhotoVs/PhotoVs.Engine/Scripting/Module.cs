using Microsoft.Xna.Framework;

namespace PhotoVs.Engine.Scripting
{
    public class Module
    {
        protected ScriptHost ScriptHost;

        public Module()
        {

        }

        public void AttachScriptHost(ScriptHost scriptHost)
        {
            ScriptHost = scriptHost;
        }

        public virtual void DefineApi(MoonSharpInterpreter interpreter)
        {

        }

        public virtual void Update(GameTime gameTime)
        {

        }
    }
}
