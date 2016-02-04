using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;

namespace ANDREICSLIB.Extracters
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="System.Web.Script.Serialization.JavaScriptConverter" />
    public sealed class DynamicJsonConverter : JavaScriptConverter
    {
        /// <summary>
        /// When overridden in a derived class, gets a collection of the supported types.
        /// </summary>
        public override IEnumerable<Type> SupportedTypes
        {
            get { return new ReadOnlyCollection<Type>(new List<Type>(new[] {typeof (object)})); }
        }

        /// <summary>
        /// Deserializes the specified dictionary.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="type">The type.</param>
        /// <param name="serializer">The serializer.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">dictionary</exception>
        public override object Deserialize(IDictionary<string, object> dictionary, Type type,
            JavaScriptSerializer serializer)
        {
            if (dictionary == null)
                throw new ArgumentNullException("dictionary");

            return type == typeof (object) ? new DynamicJsonObject(dictionary) : null;
        }

        /// <summary>
        /// Serializes the specified object.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="serializer">The serializer.</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            throw new NotImplementedException();
        }

        #region Nested type: DynamicJsonObject

        /// <summary>
        /// 
        /// </summary>
        /// <seealso cref="System.Dynamic.DynamicObject" />
        public class DynamicJsonObject : DynamicObject
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="DynamicJsonObject" /> class.
            /// </summary>
            /// <param name="dictionary">The dictionary.</param>
            /// <exception cref="ArgumentNullException">dictionary</exception>
            public DynamicJsonObject(IDictionary<string, object> dictionary)
            {
                if (dictionary == null)
                    throw new ArgumentNullException("dictionary");
                _dictionary = dictionary;
            }

            /// <summary>
            /// Gets the _dictionary.
            /// </summary>
            /// <value>
            /// The _dictionary.
            /// </value>
            public IDictionary<string, object> _dictionary { get; }

            /// <summary>
            /// Performs an implicit conversion />.
            /// </summary>
            /// <param name="d">The d.</param>
            /// <returns>
            /// The result of the conversion.
            /// </returns>
            public static implicit operator Dictionary<string, object>(DynamicJsonObject d)
            {
                return (Dictionary<string, object>) d._dictionary;
            }

            /// <summary>
            /// Returns a <see cref="System.String" /> that represents this instance.
            /// </summary>
            /// <returns>
            /// A <see cref="System.String" /> that represents this instance.
            /// </returns>
            public override string ToString()
            {
                var sb = new StringBuilder("{");
                ToString(sb);
                return sb.ToString();
            }

            /// <summary>
            /// To the string.
            /// </summary>
            /// <param name="sb">The sb.</param>
            private void ToString(StringBuilder sb)
            {
                var firstInDictionary = true;
                foreach (var pair in _dictionary)
                {
                    if (!firstInDictionary)
                        sb.Append(",");
                    firstInDictionary = false;
                    var value = pair.Value;
                    var name = pair.Key;
                    if (value is string)
                    {
                        sb.AppendFormat("{0}:\"{1}\"", name, value);
                    }
                    else if (value is IDictionary<string, object>)
                    {
                        new DynamicJsonObject((IDictionary<string, object>) value).ToString(sb);
                    }
                    else if (value is ArrayList)
                    {
                        sb.Append(name + ":[");
                        var firstInArray = true;
                        foreach (var arrayValue in (ArrayList) value)
                        {
                            if (!firstInArray)
                                sb.Append(",");
                            firstInArray = false;
                            if (arrayValue is IDictionary<string, object>)
                                new DynamicJsonObject((IDictionary<string, object>) arrayValue).ToString(sb);
                            else if (arrayValue is string)
                                sb.AppendFormat("\"{0}\"", arrayValue);
                            else
                                sb.AppendFormat("{0}", arrayValue);
                        }
                        sb.Append("]");
                    }
                    else
                    {
                        sb.AppendFormat("{0}:{1}", name, value);
                    }
                }
                sb.Append("}");
            }

            /// <summary>
            /// Tries the get member.
            /// </summary>
            /// <param name="binder">The binder.</param>
            /// <param name="result">The result.</param>
            /// <returns></returns>
            public override bool TryGetMember(GetMemberBinder binder, out object result)
            {
                if (!_dictionary.TryGetValue(binder.Name, out result))
                {
                    // return null to avoid exception.  caller can check for null this way...
                    result = null;
                    return true;
                }

                result = WrapResultObject(result);
                return true;
            }

            /// <summary>
            /// Tries the index of the get.
            /// </summary>
            /// <param name="binder">The binder.</param>
            /// <param name="indexes">The indexes.</param>
            /// <param name="result">The result.</param>
            /// <returns></returns>
            public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
            {
                if (indexes.Length == 1 && indexes[0] != null)
                {
                    if (!_dictionary.TryGetValue(indexes[0].ToString(), out result))
                    {
                        // return null to avoid exception.  caller can check for null this way...
                        result = null;
                        return true;
                    }

                    result = WrapResultObject(result);
                    return true;
                }

                return base.TryGetIndex(binder, indexes, out result);
            }

            /// <summary>
            /// Wraps the result object.
            /// </summary>
            /// <param name="result">The result.</param>
            /// <returns></returns>
            private static object WrapResultObject(object result)
            {
                var dictionary = result as IDictionary<string, object>;
                if (dictionary != null)
                    return new DynamicJsonObject(dictionary);

                var arrayList = result as ArrayList;
                if (arrayList != null && arrayList.Count > 0)
                {
                    return arrayList[0] is IDictionary<string, object>
                        ? new List<object>(
                            arrayList.Cast<IDictionary<string, object>>().Select(x => new DynamicJsonObject(x)))
                        : new List<object>(arrayList.Cast<object>());
                }

                return result;
            }
        }

        #endregion
    }
}