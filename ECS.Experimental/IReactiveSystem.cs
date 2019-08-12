using System;
using System.Collections.Generic;
using ECS.Experimental;

namespace ECS
{
    internal interface IReactiveSystem : ISystem 
    {
        Type ContextType
        {
            get;
        }

        Type SubscribeType
        {
            get;
        }

        void Execute(List<int> entities);
    }
}