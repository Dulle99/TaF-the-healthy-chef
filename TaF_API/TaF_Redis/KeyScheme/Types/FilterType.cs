using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaF_Redis.KeyScheme.Types
{
    public enum FilterType
    {
        latestContent,
        oldestContent,
        mostPopular,
        fastestToCook
    }
}
