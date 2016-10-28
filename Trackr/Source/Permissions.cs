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

            public const string CreateUsersFromGuardians = "UserManagement.CreateUsersFromGuardians";
            public const string SpoofUsers = "UserManagement.SpoofUsers";
        }

        public class PlayerManagement
        {
            public const string CreatePlayer = "PlayerManagement.CreatePlayer";
            public const string EditPlayer = "PlayerManagement.EditPlayer";
            public const string EditPlayerBasic = "PlayerManagement.EditPlayer.Basic";
            public const string ViewPlayers = "PlayerManagement.ViewPlayers";

            public const string RegisterNewPlayer = "PlayerManagement.RegisterNewPlayer";
            public const string ReRegisterPlayer = "PlayerManagement.ReRegisterPlayer";
        }

        public class Scheduler
        {
            public const string ViewSchedule = "Scheduler.ViewSchedule";
        }

        public class TeamManagement
        {
            public const string ViewTeams = "TeamManagement.ViewTeams";
            public const string CreateTeam = "TeamManagement.CreateTeam";
            public const string EditTeam = "TeamManagement.EditTeam";
        }
    }
}