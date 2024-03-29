﻿using System;
using System.Collections.Generic;

namespace PhotoVs.Engine.Core
{
    public interface IKernel
    {
        IEnumerable<object> Bindings { get; }

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