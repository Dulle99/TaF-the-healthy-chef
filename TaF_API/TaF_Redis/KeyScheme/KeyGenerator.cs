using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaF_Redis.Types;

namespace TaF_Redis.KeyScheme
{
    public static class KeyGenerator
    {

        public static string CreateKeyForRecomendedCookingRecepies()
        {
            return "recomendedCookingRecepies:";            
        }

        public static string CreateKeyForRecomendedBlogs()
        {
            return "recomendedBlogs:";
        }

        public static string CreateKeyForAuthorPersonalContent(string username, ContentType contentType)
        {
            string typeOfConent = TypeResolver.ResolveContentType(contentType);
            return "author:" + username + ":" + typeOfConent;
        }

        public static string CreateKeyForUsersSavedContent(string username, ContentType contentType)
        {
            string typeOfConent = TypeResolver.ResolveContentType(contentType);
            return "user:" + username + "SavedContents:" + typeOfConent;
        }

        public static string CreateKeyForRecommendedContent(ContentType contentType)
        {
            string typeOfConent = TypeResolver.ResolveContentType(contentType);
            return "recommendedContent:" + typeOfConent;   
        }

        public static string CreateKeyForFilteredContent(FilterType filterType, ContentType contentType)
        {
            return TypeResolver.ResolveFilterType(filterType) + ":" + TypeResolver.ResolveContentType(contentType);
        }

        public static string CreateKeyForContent(ContentType contentType, Guid contentId)
        {
            string typeOfContent = TypeResolver.ResolveContentType(contentType);
            return typeOfContent + ":" + contentId.ToString() + ":Id";
        }
    }
}
