using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaF_Neo4j.DTOs.UserDTO
{
    public class UserLoginCredentials
    {
        #region Fields

        public string Username { get; set; }

        public string Password { get; set; }

        #endregion Fields

        #region Constructor

        public UserLoginCredentials() {  }

        #endregion Constructor

    }
}
