using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ANDREICSLIB.Helpers
{
    /// <summary>
    /// 
    /// </summary>
    public class NoPrivateItemsResolver : Newtonsoft.Json.Serialization.DefaultContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty property = base.CreateProperty(member, memberSerialization);
            property.Ignored = false;
            return property;
        }
    }
}