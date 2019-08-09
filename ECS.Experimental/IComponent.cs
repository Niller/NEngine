using System;

namespace ECS.Experimental
{
    public interface IComponent
    {
        void SetContext(Context context);
        void SetEntityId(int entityId);

    }
}