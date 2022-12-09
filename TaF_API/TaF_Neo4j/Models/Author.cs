using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaF_Neo4j.Models
{
    public class Author
    {
        #region Fields
       
        public string Username { get; set; }

        public byte[] Password { get; set; }

        public byte[] PasswordSalt { get; set; }

        public string Name { get; set; }

        public string Lastname { get; set; }

        public string Biography { get; set; }

        public List<Award> Awards { get; set; }

        public int YearsOfExpiriance { get; set; }

        public string ProfilePictureFilePath { get; set; }

        #endregion Fields

        #region Constructor
        public Author()
        {
            this.PasswordSalt = Guid.NewGuid().ToByteArray();
        }
        #endregion Constructor
    }
}
