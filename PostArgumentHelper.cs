using System.Collections.Generic;
using System.Linq;

namespace JqueryWebControls
{
    internal class PostArgumentHelper
    {
        public static string GetValueByName(IList<ArgumentItem> argumentItems, string name)
        {
            var item = argumentItems.Where(x => x.Name.EndsWith(name, true, null)).SingleOrDefault();
            return item.Value ?? string.Empty;
        }
    }
}
