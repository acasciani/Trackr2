using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Trackr
{
    public class Permissions
    {
        public class UserManagement
        {
            public const string CreateUser = "UserManagement.CreateUser";
            public const string EditUser = "UserManagement.EditUser";
            public const string ViewUsers = "UserManagement.ViewUsers";
        }

        public class PlayerManagement
        {
            public const string CreatePlayer = "PlayerManagement.CreatePlayer";
            public const string EditPlayer = "PlayerManagement.EditPlayer";
            public const string ViewPlayers = "PlayerManagement.ViewPlayers";

            public const string RegisterNewPlayer = "PlayerManagement.RegisterNewPlayer";
            public const string ReRegisterPlayer = "PlayerManagement.ReRegisterPlayer";
        }
    }
}