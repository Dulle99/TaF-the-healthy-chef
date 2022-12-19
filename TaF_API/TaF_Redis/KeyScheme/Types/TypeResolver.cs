using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaF_Redis.KeyScheme.Types
{
    public static class TypeResolver
    {
        public static string ResolveContentType(ContentType contentType)
        {
            if (contentType == ContentType.blogs)
                return ":blogs";
            else if (contentType == ContentType.cookingRecepies)
                return ":cookingRecepies";
            else if (contentType == ContentType.savedBlogs)
                return ":savedBlogs";
            else
                return ":savedCookingReceies";
        }

        public static string ResolveFilterType(FilterType filterType)
        {
            if (filterType == FilterType.latestContent)
                return "latestContent:";
            else if (filterType == FilterType.oldestContent)
                return "oldestContent:";
            else if (filterType == FilterType.mostPopular)
                return "mostPopularContent:";
            else
                return "fastestToCook:";
        }
    }
}
