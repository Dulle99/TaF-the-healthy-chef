using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace TaF_Neo4j.DTOs.UserDTO
{
    public class UserUpdateProfilePicture
    {
        #region Fields

        public string Username { get; set; }

        public IFormFile ProfilePicture { get; set; }

        public string TypeOfUser { get; set; }

        #endregion Fields

        #region Constructor

        public UserUpdateProfilePicture() { }

        #endregion Constructor
    }
}
