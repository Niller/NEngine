using System;
using System.Collections.Generic;
using ECS.Experimental;

namespace ECS
{
    public abstract class ReactiveSystem<TContext, TComponent> : IReactiveSystem
        where TContext : Context
        where TComponent : struct
    {
        public Type ContextType => typeof(TContext);

        public Type SubscribeType => typeof(TComponent);

        public abstract void Execute(List<int> entities);
    }
}