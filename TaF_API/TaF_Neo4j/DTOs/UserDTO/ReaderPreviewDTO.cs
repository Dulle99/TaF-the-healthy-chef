using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaF_Neo4j.DTOs.UserDTO
{
    public class ReaderPreviewDTO
    {
        #region Fields

        public string Username { get; set; }

        public string Name { get; set; }

        public string Lastname { get; set; }

        public string NameOfSchool { get; set; }

        public string TypeOfStudent { get; set; }

        public byte[] ProfilePicture { get; set; }

        #endregion Fields

        #region Constructor

        public ReaderPreviewDTO() { }

        #endregion Constructor
    }
}
