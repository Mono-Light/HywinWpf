using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messager.Classes
{
    public class User
    {
        public string username;
    }

    public class LocalUser : User
    {
        public string serverPublicKey; 
        private static LocalUser localUser;

        public static LocalUser GetLocalUser()
        {
            return localUser;
        }



    }

}
