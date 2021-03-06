﻿using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace RazorEngineCore
{
    public class AnonymousTypeWrapper : DynamicObject
    {
        private readonly object model;

        public AnonymousTypeWrapper(object model)
        {
            this.model = model;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            var propertyInfo = model.GetType().GetProperty(binder.Name);

            if (propertyInfo == null)
            {
                result = null;
                return false;
            }

            result = propertyInfo.GetValue(model, null);

            if (result == null)
            {
                return true;
            }

            var type = result.GetType();

            if (result.IsAnonymous())
            {
                result = new AnonymousTypeWrapper(result);
            }

            if (type.IsArray)
            {
                result = ((IEnumerable<object>)result).Select(e => new AnonymousTypeWrapper(e)).ToList();
            }

            return true;
        }
    }
}