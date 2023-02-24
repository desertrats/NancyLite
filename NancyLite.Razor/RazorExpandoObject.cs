using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;

namespace NancyLite.Razor
{
    public class RazorExpandoObject : DynamicObject, IDictionary<string, object>
    {
        public RazorExpandoObject()
        {

        }
        private RazorExpandoObject(IDictionary<string, object> src)
        {
            _dictionary = new ConcurrentDictionary<string, object>(src);
        }
        public RazorExpandoObject Duplicate()
        {
            return new RazorExpandoObject(_dictionary);
        }

        private readonly ConcurrentDictionary<string, object> _dictionary = new ConcurrentDictionary<string, object>();

        public object this[string key]
        {
            get
            {
                if (_dictionary.ContainsKey(key)) return _dictionary[key];
                return default;
            }

            set
            {
                if (_dictionary.ContainsKey(key))
                {

                    _dictionary[key] = value;
                }
                else
                {
                    _dictionary.AddOrUpdate(key, value, (k, oldContent) => value);
                }
            }
        }

        public ICollection<string> Keys => _dictionary.Keys;

        public ICollection<object> Values => _dictionary.Values;

        public int Count => _dictionary.Count;

        public bool IsReadOnly => false;

        public void Add(string key, object value)
        {
            _dictionary.AddOrUpdate(key, value, (k, oldContent) => value);
        }

        public void Add(KeyValuePair<string, object> item)
        {
            _dictionary.AddOrUpdate(item.Key, item.Value, (key, oldContent) => item.Value);
        }

        public void Clear()
        {
            _dictionary.Clear();
        }

        public bool Contains(KeyValuePair<string, object> item)
        {
            return _dictionary.ContainsKey(item.Key);
        }

        public bool ContainsKey(string key)
        {
            return _dictionary.ContainsKey(key);
        }

        public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        {
            _dictionary.ToArray().CopyTo(array, arrayIndex);
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }

        public bool Remove(string key)
        {
            return _dictionary.Remove(key, out _);
        }

        public bool Remove(KeyValuePair<string, object> item)
        {
            return _dictionary.Remove(item.Key, out _);
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            var name = binder.Name;
            result = _dictionary.ContainsKey(name) ? _dictionary[name] : default;
            return true;
        }

        public bool TryGetValue(string key, [MaybeNullWhen(false)] out object value)
        {
            if (_dictionary.ContainsKey(key))
            {
                value = _dictionary[key];
                return true;
            }
            value = default;
            return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            var name = binder.Name;
            _dictionary.AddOrUpdate(name, value, (key, oldContent) => value);
            return true;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }
    }
}
