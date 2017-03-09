using System;
using System.Collections.Generic;
using System.Text;

namespace SqlSimpleDictionary
{
    struct NameValueResult
    {
        public readonly bool IsValid;
        public readonly string Name;
        public readonly string Value;
        private NameValueResult(bool isValid, string name, string value)
        {
            IsValid = isValid;
            Name = name;
            Value = value;
        }
        public static NameValueResult Parse(string str)
        {
            var a = str.Split('=');
            return a.Length == 2 ? new NameValueResult(true, a[0], a[1]) : new NameValueResult(false, null, null);


        }
    }
}
