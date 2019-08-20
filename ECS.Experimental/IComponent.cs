using System;

namespace ECS.Experimental
{
    public interface IComponent
    {
        void SetContext(Context context);
        void SetEntityId(int entityId);
        void SetType(Type type);
        void AddEntityByIndex();
        void RemoveEntityByIndex();

    }
}