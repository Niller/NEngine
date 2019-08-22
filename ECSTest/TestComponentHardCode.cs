using System;
using ECS.Experimental;

namespace ECSTest
{
    //[Component(typeof(MainContext))]
    public struct TestComponentHardCode : IComponent
    {
        private Context _context;
        private int _sourceEntityId;
        private Type _type;
        private Type _indexType;

        public Type Type => typeof(TestComponent2);

        private int _x;
        public int X
        {
            get => _x;
            set
            {
                _x = value;
                _context.MarkComponentDirty(_sourceEntityId, typeof(TestComponentHardCode));
            }
        }

        public void SetContext(Context context)
        {
            _context = context;
        }

        public void SetEntityId(int entityId)
        {
            _sourceEntityId = entityId;
        }

        public void SetType(Type type)
        {
            _type = type;
        }

        public void AddEntityByIndex()
        {
            var hashCode = X.GetHashCode();
            _context.AddEntityWithIndex(Type, hashCode, _sourceEntityId);
        }

        public void RemoveEntityByIndex()
        {
            _context.RemoveEntityWithIndex(_type, X.GetHashCode(), _sourceEntityId);
        }
    }
}