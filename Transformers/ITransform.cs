using System.Collections.Generic;

namespace ANDREICSLIB.Transformers
{
    /// <summary>
    /// 
    /// </summary>
    public interface ITransform
    {
        /// <summary>
        /// Saves the specified filename.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <param name="content">The content.</param>
        /// <param name="startChildrenPoint">The start children point.</param>
        /// <param name="header">if set to <c>true</c> [header].</param>
        /// <param name="uniqueColumn">The unique column.</param>
        /// <returns></returns>
        Result Save(string filename, Dictionary<string, object> content, List<string> startChildrenPoint, bool header,
            int? uniqueColumn);
    }
}