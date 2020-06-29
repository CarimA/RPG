using MoonSharp.Interpreter;
using PhotoVs.Utils.Logging;
using System;

namespace PhotoVs.Engine.Scripting
{
    public class MoonSharpInterpreter
    {
        public Script Script { get; }

        public MoonSharpInterpreter()
        {
            Script = new Script();
        }

        public void AddFunction(string name, object func)
        {
            Script.Globals[name] = func;
        }

        public object CallFunction(string name, params object[] args)
        {
            try
            {
                return Script.Call(name, args);
            }
            catch (Exception e)
            {
                HandleError(e, name);
                throw;
            }
        }

        public bool CallIfDefined(out object result, string name, params object[] args)
        {
            if (IsDefined(name))
            {
                result = CallFunction(name, args);
                return true;
            }

            result = null;
            return false;
        }

        public void AddType<T>(string name)
        {
            UserData.RegisterType<T>();
            RegisterGlobal(name, UserData.CreateStatic<T>());
        }

        public void RegisterGlobal(string name, object obj)
        {
            Script.Globals[name] = obj;
        }

        public void RunScript(string script)
        {
            try
            {
                Script.DoString(script);
            }
            catch (Exception e)
            {
                HandleError(e, "RunScript");
                throw;
            }
        }

        public bool IsDefined(string name)
        {
            return Script.Globals[name] != null;
        }

        public static void HandleError(Exception exception, string where)
        {
            switch (exception)
            {
                case MoonSharp.Interpreter.SyntaxErrorException syntaxError:
                    Logger.Write.Error($"{@where}: {syntaxError.DecoratedMessage}");
                    break;
                case MoonSharp.Interpreter.InterpreterException interpreter:
                    Logger.Write.Error($"{@where}: {interpreter.DecoratedMessage}");
                    break;
                default:
                    Logger.Write.Error($"{@where}: {exception.Message}");
                    break;
            }
        }
    }
}
