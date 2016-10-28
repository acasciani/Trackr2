﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Telerik.OpenAccess.FetchOptimization;
using TrackrModels;
using Telerik.OpenAccess;
using Trackr.Utils;
using ProtoBuf;

namespace Trackr.Source.Wizards
{
    public class TeamModifiedByAnotherProcessException : Exception
    {
        public TeamModifiedByAnotherProcessException() : base() { }
        public TeamModifiedByAnotherProcessException(string message) : base(message) { }
    }

    public class TeamManager
    {
        private static Team _Team
        {
            get
            {
                return HttpContext.Current.Session["Team"] as Team;
            }
            set
            {
                HttpContext.Current.Session["Team"] = value;
            }
        }

        private static List<RegistrationRule> _RegistrationRules
        {
            get
            {
                return HttpContext.Current.Session["Team_RegistrationRule"] as List<RegistrationRule>;
            }
            set
            {
                HttpContext.Current.Session["Team_RegistrationRule"] = value;
            }
        }

        private static FetchStrategy TeamFetchStrategy
        {
            get
            {
                FetchStrategy fetch = new FetchStrategy() { MaxFetchDepth = 2 };
                fetch.LoadWith<RegistrationRule>(i => i.OldTeam);
                return fetch;
            }
        }

        public static void CreateTeam()
        {
            _Team = new Team();
            _RegistrationRules = new List<RegistrationRule>();
        }

        public static void EditTeam(int teamID)
        {
            using (ClubManagement cm = new ClubManagement())
            {
                var temp = cm.Teams.LoadWith(TeamFetchStrategy).Where(i => i.TeamID == teamID).First();
                _Team = cm.CreateDetachedCopy<Team>(temp, TeamFetchStrategy);

                _RegistrationRules = cm.RegistrationRules.LoadWith(TeamFetchStrategy).Where(i => i.NewTeamID == teamID).ToList();
            }
        }

        public static void DisposeTeam()
        {
            HttpContext.Current.Session.Remove("Team");
            HttpContext.Current.Session.Remove("Team_RegistrationRule");
        }

        public static Team Team
        {
            get { return _Team; }
        }

        public static List<RegistrationRule> RegistrationRules
        {
            get { return _RegistrationRules; }
        }



        #region Info
        public static void UpdateInfo(int programID, string teamName, DateTime activeFrom, DateTime activeTo, short minRosterSize, short maxRosterSize, DateTime ageCutoff)
        {
            Team.ProgramID = programID;
            Team.TeamName = teamName;
            Team.StartYear = activeFrom;
            Team.EndYear = activeTo;
            Team.MinRosterSize = minRosterSize;
            Team.MaxRosterSize = maxRosterSize;
            Team.AgeCutoff = ageCutoff;
        }
        #endregion

        
        #region Guardians
        public static Guid AddRegistrationRule(int? oldTeamID, DateTime registrationOpens, DateTime registrationCloses, DateTime? ageCutoff)
        {
            RegistrationRule obj = new RegistrationRule() { EditToken = Guid.NewGuid() };
            obj.WasModified = true;

            obj.OldTeamID = oldTeamID;
            obj.RegistrationOpens = 

            _RegistrationRules.Add(obj);

            return obj.EditToken;
        }

        public static void DeleteGuardian(Guid guardianEditToken)
        {
            Guardian obj = (Guardian)FindEditableObject(guardianEditToken);
            if (obj.GuardianID > 0)
            {
                obj.Active = false;
                obj.WasModified = true;
            }
            else
            {
                _Player.Guardians.Remove(obj);
            }
        }
        #endregion
        */

        public static int SaveData(int modifiedByUser)
        {
            //returns team id
            using (ClubManagement cm = new ClubManagement())
            {
                try
                {
                    Team freshCopy = _Team.TeamID > 0 ? cm.Teams.LoadWith(TeamFetchStrategy).Where(i => i.TeamID == _Team.TeamID).First() : new Team();

                    DateTime modifiedAt = DateTime.Now.ToUniversalTime();

                    freshCopy.ProgramID = Team.ProgramID;
                    freshCopy.TeamName = Team.TeamName;
                    freshCopy.StartYear = Team.StartYear;
                    freshCopy.EndYear = Team.EndYear;
                    freshCopy.MinRosterSize = Team.MinRosterSize;
                    freshCopy.MaxRosterSize = Team.MaxRosterSize;
                    freshCopy.AgeCutoff = Team.AgeCutoff;


                    // everything up to this point is fresh
                    if (freshCopy.TeamID == 0)
                    {
                        cm.Add(freshCopy);
                        cm.SaveChanges();

                        int teamID = freshCopy.TeamID;
                        return teamID;
                    }
                    else
                    {
                        cm.SaveChanges();
                        return freshCopy.TeamID;
                    }
                }
                catch (Exception ex)
                {
                    cm.ClearChanges();
                    throw ex;
                }
            }
        }
    }
}
