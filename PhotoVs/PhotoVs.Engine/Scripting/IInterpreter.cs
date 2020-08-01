namespace PhotoVs.Engine.Scripting
{
    public interface IInterpreter<T>
    {
        void AddFunction(string name, object func);
        object CallFunction(string name, params object[] args);
        bool CallIfDefined(out object result, string name, params object[] args);
        void AddType<TType>(string name);
        void RegisterGlobal(string name, object obj);
        void RunScript(string script);
        bool IsDefined(string name);
        void RunCoroutine(string source, T closure);
        void ClearScripts();
    }
}