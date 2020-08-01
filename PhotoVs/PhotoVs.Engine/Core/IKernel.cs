using System;

namespace PhotoVs.Engine.Core
{
    public interface IKernel
    {
        Action<object> OnBind { get; set; }
        Action OnConstruct { get; set; }

        Kernel Bind<T>(T instance) where T : class;
        Kernel Bind(Type type, object instance);

        Kernel Bind<TInterface, TObject>()
            where TInterface : class
            where TObject : TInterface;

        void Construct();
    }
}