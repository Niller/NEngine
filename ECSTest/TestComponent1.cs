﻿using System;
using ECS.Experimental;

namespace ECSTest
{
    //[Component(typeof(MainContext))]
    public struct TestComponent1 : IComponent
    {
        private Context _context;
        private int _sourceEntityId;
        private Type _type;

        public Type Type => typeof(TestComponent2);

        private int _x;
        public int X
        {
            get => _x;
            set
            {
                _x = value;
                _context.MarkComponentDirty(_sourceEntityId, typeof(TestComponent1));
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
    }
}