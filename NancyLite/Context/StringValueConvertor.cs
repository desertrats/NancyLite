using Newtonsoft.Json;
using System;

namespace NancyLite
{
    public static class StringValueConvertor
    {
        public static TValue ChangeTypeConvertor<TValue>(string strValue, TValue defaultValue = default)
        {
            try
            {
                return (TValue)Convert.ChangeType(strValue, typeof(TValue));
            }
            catch (Exception)
            {
                return defaultValue;
            }
        }

        public static TValue JsonDeserializeConvertor<TValue>(string strValue, TValue defaultValue = default)
        {
            try
            {
                return JsonConvert.DeserializeObject<TValue>(strValue);
            }
            catch (Exception)
            {
                return defaultValue;
            }
        }

    }
}
