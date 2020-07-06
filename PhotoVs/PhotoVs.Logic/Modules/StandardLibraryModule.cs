using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PhotoVs.Engine.Scripting;

namespace PhotoVs.Logic.Modules
{
    public class StandardLibraryModule : Module
    {
        public const string StdLib = @"
-- event conditions

function FlagCondition(f, s)
  return function ()
    return _FlagCondition(f, (s ~= false))
  end
end

function VarCondition(v, e, o)
  return function ()
    return _VarCondition(v, e, o)
  end
end

-- general helpers for coroutine-centric functions (the _ variants are implemented in-engine)

function Move(g, t, s)
  while _Move(g, t, s) do
    coroutine.yield()
  end
end
                
function Say(n, d)
  while _Say(n, d) do
    coroutine.yield()
  end
end

function Wait(t)
  while t > 0 do
    coroutine.yield()
    t = t - GetDeltaTime()
  end
end

function LockWhile(f, ...)
  Lock()
  f(arg)
  Unlock()
end
";

        public StandardLibraryModule()
        {
            
        }

        public override void DefineApi(MoonSharpInterpreter interpreter)
        {
            if (interpreter == null) throw new ArgumentNullException(nameof(interpreter));
            interpreter.RunScript(StdLib);

            base.DefineApi(interpreter);
        }
    }
}
