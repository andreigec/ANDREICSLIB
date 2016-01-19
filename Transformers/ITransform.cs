using System.Collections.Generic;

namespace ANDREICSLIB.Transformers
{
    public interface ITransform
    {
        Result Save(string filename, Dictionary<string, object> content, List<string> startChildrenPoint, bool header, int? uniqueColumn);
    }
}
