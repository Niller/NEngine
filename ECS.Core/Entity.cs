﻿using System;

namespace ECS
{
    public struct Entity
    {
        public int Id
        {
            get;
        }

        private readonly BaseContext _currentContext;

        internal Entity(BaseContext currentContext, int id)
        {
            Id = id;
            _currentContext = currentContext;
        }

        public T GetComponent<T>() where T : struct
        {
            throw new Exception("You cannot use directly GetComponent method. It must be replaced by code injection!");
        }
        
    }
}