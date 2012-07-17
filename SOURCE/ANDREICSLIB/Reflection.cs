using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace ANDREICSLIB
{
    public static class Reflection
    {
        public static string GetMemberName<T>(Expression<Func<T>> memberExpression)
        {
            var expressionBody = (MemberExpression)memberExpression.Body;
            return expressionBody.Member.Name;
        }

        public static List<Tuple<string,object> > GetFieldNameAndValue(object o)
        {
            var ty = o.GetType();
            var fields = ty.GetFields();

            return fields.Select(v => new Tuple<string, object>(v.Name, v.GetValue(o))).ToList();
        }
        
        public static bool SerialiseObject(object obj,string filename)
        {
            if (File.Exists(filename) == false)
                return false;

            String r = "";
            var ol = GetFieldNameAndValue(obj);

            foreach(var o in ol)
            {
                r += o.Item1 + "\t" + o.Item2 + "\r\n";
            }

            FileUpdates.SaveToFile(filename,r);
            return true;
        }

        public static object DeserialiseFile(Type objectType,String filename)
        {
            if (File.Exists(filename)==false)
                return null;

            var s = FileUpdates.LoadFile(filename);

            if (string.IsNullOrEmpty(s))
                return null;

            var s2 = StringUpdates.SplitString(s, "\r\n");

            var instance = Activator.CreateInstance(objectType);

            foreach(var s3 in s2)
            {
                var s4=StringUpdates.SplitString(s3, "\t");
                if (s4.Length != 2)
                    return null;

                var fieldname = s4[0];
                var fieldval = s4[1];

                var ff = objectType.GetFields();
                var field = objectType.GetField(fieldname);
                if (field == null)
                    return null;

                field.SetValue(instance, fieldval);
            }
            return instance;
        }

    }
}
