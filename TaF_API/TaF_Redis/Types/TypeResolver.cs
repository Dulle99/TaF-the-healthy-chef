using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaF_Redis.Types
{
    public static class TypeResolver
    {
        public static string ResolveContentType(ContentType contentType)
        {
            if (contentType == ContentType.blog)
                return "blog";
            else if (contentType == ContentType.cookingRecepie)
                return "cookingRecepie";
            else if (contentType == ContentType.savedBlog)
                return "savedBlog";
            else
                return "savedCookingReceie";
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
