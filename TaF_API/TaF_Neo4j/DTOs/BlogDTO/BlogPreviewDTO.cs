using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaF_Neo4j.DTOs.BlogDTO
{
    public class BlogPreviewDTO
    {
        #region Fields

        public string AuthorUsername { get; set; }

        public byte[] AuthorProfilePicture { get; set; }

        public Guid BlogId { get; set; }

        public string BlogTitle { get; set; }

        public string PartOfBlogContent { get; set; }

        public DateTime PublicationDate { get; set; }

        public int ReadingTime { get; set; }

        public float AverageRate { get; set; }

        public byte[] BlogPicture { get; set; }

        #endregion Fields

        #region Constructor

        public BlogPreviewDTO() 
        {
            this.ReadingTime = 1;
            this.AverageRate = 0;
        }

        public BlogPreviewDTO(string authorUsername, byte[] profilePicture, Guid blogId, string blogTitle,
                             string blogContent, DateTime publicationDate, int readingTime, float averageRate, byte[] blogPicture)
        {
            this.AuthorUsername = authorUsername;
            this.AuthorProfilePicture = profilePicture;
            this.BlogId = blogId;
            this.BlogTitle = blogTitle;
            this.PartOfBlogContent = blogContent.Length < 300 ? blogContent + "..." : blogContent.Substring(0,
                                                                                                    blogContent.Length < 350 ? blogContent.Length : 350) + "...";
            this.PublicationDate = publicationDate;
            this.ReadingTime = readingTime;
            this.AverageRate = averageRate;
            this.BlogPicture = blogPicture;
        }

        public BlogPreviewDTO(Guid blogId, string blogTitle,
                             string blogContent, DateTime publicationDate, int readingTime, byte[] blogPicture)
        {
            this.BlogId = blogId;
            this.BlogTitle = blogTitle;
            this.PartOfBlogContent = blogContent.Substring(0, blogContent.Length / 4);
            this.PublicationDate = publicationDate;
            this.ReadingTime = readingTime;
            this.BlogPicture = blogPicture;
        }

        public BlogPreviewDTO(string authorUsername, byte[] profilePicture, string blogTitle,
                              string blogContent, DateTime publicationDate, int readingTime, byte[] blogPicture) 
        {
            this.AuthorUsername = authorUsername;
            this.AuthorProfilePicture = profilePicture;
            this.BlogTitle = blogTitle;
            this.PartOfBlogContent = blogContent.Substring(0, blogContent.Length / 4);
            this.PublicationDate = publicationDate;
            this.ReadingTime = readingTime;
            this.BlogPicture = blogPicture;
        }

        #endregion Constructor
    }
}
