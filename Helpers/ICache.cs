using System;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace ANDREICSLIB.Helpers
{
    /// <summary>
    /// 
    /// </summary>
    public interface ICache
    {
        /// <summary>
        /// pass in an async function that returns a string - can compress if type is known
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="compress">if set to <c>true</c> [compress].</param>
        /// <param name="memberName">Name of the member.</param>
        /// <param name="obeyDataContracts">if set to <c>true</c> [obey data contracts].</param>
        /// <returns></returns>
        Task<string> Cache(Expression<Func<Task<string>>> action, bool compress = false,
            [CallerMemberName] string memberName = "", bool obeyDataContracts = true);

        /// <summary>
        /// pass in an async function that takes T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action">The action.</param>
        /// <param name="memberName">Name of the member.</param>
        /// <param name="obeyDataContracts">if set to <c>true</c> [obey data contracts].</param>
        /// <returns></returns>
        Task<T> Cache<T>(Expression<Func<Task<T>>> action, [CallerMemberName] string memberName = "", bool obeyDataContracts = true) where T : class;

        /// <summary>
        /// pass in a sync function
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action">The action.</param>
        /// <param name="memberName">Name of the member.</param>
        /// <param name="obeyDataContracts">if set to <c>true</c> [obey data contracts].</param>
        /// <returns></returns>
        Task<T> Cache<T>(Expression<Func<T>> action, [CallerMemberName] string memberName = "", bool obeyDataContracts = true) where T : class;

        /// <summary>
        ///     return a value using a key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cacheKey"></param>
        /// <returns></returns>
        Task<T> Get<T>(string cacheKey) where T : class;

        /// <summary>
        /// set a value using a key and a value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <param name="obeyDataContracts">if set to <c>true</c> [obey data contracts].</param>
        /// <returns></returns>
        Task<bool> Set<T>(string key, T value, bool obeyDataContracts = true);

        bool Enabled { get; set; }
    }
}