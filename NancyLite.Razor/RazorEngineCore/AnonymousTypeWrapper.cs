using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace RazorEngineCore
{
    /// <summary>
    /// 匿名类型的wrapper
    /// </summary>
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

            //var type = result.GetType();

            if (result.IsAnonymous())
            {
                result = new AnonymousTypeWrapper(result);
            }

            if (result is IDictionary dictionary)
            {
                var keys = new List<object>();

                foreach (var key in dictionary.Keys)
                {
                    keys.Add(key);
                }

                foreach (var key in keys)
                {
                    if (dictionary[key].IsAnonymous())
                    {
                        dictionary[key] = new AnonymousTypeWrapper(dictionary[key]);
                    }
                }
            }
            else if (result is IEnumerable enumerate && !(result is string))
            {
                result = enumerate.Cast<object>()
                        .Select(e =>
                        {
                            if (e.IsAnonymous())
                            {
                                return new AnonymousTypeWrapper(e);
                            }

                            return e;
                        })
                        .ToList();
            }


            return true;
        }
    }
}