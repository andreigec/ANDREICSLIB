using System;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace ANDREICSLIB.Helpers
{
    public interface IPersistantCache
    {
        Task<string> Cache(Expression<Func<Task<string>>> action, bool compress = false, [CallerMemberName] string memberName = "");
        Task<T> Cache<T>(Expression<Func<Task<T>>> action, [CallerMemberName] string memberName = "") where T : class;
        T Cache<T>(Expression<Func<T>> action, [CallerMemberName] string memberName = "") where T : class;
        T Get<T>(string cacheKey) where T : class;
        void Set(string key, object value);
    }
}