using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaF_Neo4j.Models
{
    public class Reader
    {
        #region Fields

        public string Username { get; set; }

        public byte[] Password { get; set; }

        public byte[] PasswordSalt { get; set; }

        public string Name { get; set; }

        public string Lastname { get; set; }

        public string NameOfSchool { get; set; }

        public string TypeOfStudent { get; set; }

        public string ProfilePictureFilePath { get; set; }

        #endregion Fields

        #region Constructor
        public Reader()
        {
            this.PasswordSalt = Guid.NewGuid().ToByteArray();
        }
        #endregion Constructor
    }
}
