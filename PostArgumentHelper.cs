using System.Collections.Generic;
using System.Linq;

namespace JqueryWebControls
{
    internal class PostArgumentHelper
    {
        public static string GetValueByName(Dictionary<string, string> dictionary, string name)
        {
            var item = dictionary.Where(x => x.Key.EndsWith(name, true, null)).SingleOrDefault();
            return item.Value ?? string.Empty;
        }
    }
}
