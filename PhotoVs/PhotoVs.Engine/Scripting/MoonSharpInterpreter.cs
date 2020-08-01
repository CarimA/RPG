using System;
using MoonSharp.Interpreter;
using PhotoVs.Engine.Events.Coroutines;
using PhotoVs.Utils.Logging;
using Coroutine = PhotoVs.Engine.Events.Coroutines.Coroutine;

namespace PhotoVs.Engine.Scripting
{
    public class MoonSharpInterpreter : IInterpreter<Closure>
    {
        private readonly ICoroutineRunner _coroutineRunner;
        private Script _script;

        public MoonSharpInterpreter(ICoroutineRunner coroutineRunner)
        {
            _coroutineRunner = coroutineRunner;
            ClearScripts();
        }

        public void ClearScripts()
        {
            _script = new Script();
            AddFunction("print", (Action<string, object[]>) Logger.Write.Trace);
        }

        public void AddFunction(string name, object func)
        {
            _script.Globals[name] = func;
        }

        public object CallFunction(string name, params object[] args)
        {
            try
            {
                return _script.Call(name, args);
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
            _script.Globals[name] = obj;
        }

        public void RunScript(string script)
        {
            try
            {
                _script.DoString(script);
            }
            catch (Exception e)
            {
                HandleError(e, "RunScript");
                throw;
            }
        }

        public bool IsDefined(string name)
        {
            return _script.Globals[name] != null;
        }

        public void RunCoroutine(string source, Closure closure)
        {
            var coroutine = _script.CreateCoroutine(closure);
            var iterator = coroutine.Coroutine.AsUnityCoroutine();
            _coroutineRunner.Start(new Coroutine(source, iterator));
        }

        public static void HandleError(Exception exception, string where)
        {
            switch (exception)
            {
                case SyntaxErrorException syntaxError:
                    Logger.Write.Error($"{where}: {syntaxError.DecoratedMessage}");
                    break;
                case InterpreterException interpreter:
                    Logger.Write.Error($"{where}: {interpreter.DecoratedMessage}");
                    break;
                default:
                    Logger.Write.Error($"{where}: {exception.Message}");
                    break;
            }
        }
    }
}