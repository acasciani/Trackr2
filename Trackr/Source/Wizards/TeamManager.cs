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

        private static FetchStrategy TeamFetchStrategy
        {
            get
            {
                FetchStrategy fetch = new FetchStrategy() { MaxFetchDepth = 5 };
                return fetch;
            }
        }

        public static void CreateTeam()
        {
            _Team = new Team();
            //_Player.EditToken = Guid.NewGuid();
            //_Player.Person.ClubID = clubID;
        }

        public static void EditTeam(int teamID)
        {
            using (ClubManagement cm = new ClubManagement())
            {
                var temp = cm.Teams.LoadWith(TeamFetchStrategy).Where(i => i.TeamID == teamID).First();
                _Team = cm.CreateDetachedCopy<Team>(temp, TeamFetchStrategy);
            }
        }

        public static void DisposeTeam()
        {
            HttpContext.Current.Session.Remove("Team");
        }

        public static Team Team
        {
            get { return _Team; }
        }


        #region Info
        public static void UpdateInfo(Guid editToken, string firstName, string lastName, char? middleInitial, DateTime? dateOfBirth)
        {
            /*
            Person obj = (Person)FindEditableObject(editToken);
            obj.MInitial = middleInitial;
            obj.FName = firstName;
            obj.LName = lastName;
            obj.WasModified = true;
            obj.DateOfBirth = dateOfBirth;
             */
        }
        #endregion

        /*
        #region Guardians
        public static void AddGuardians(List<int> personIDs)
        {
            using (ClubManagement cm = new ClubManagement())
            {
                var people = cm.People.Where(i => personIDs.Contains(i.PersonID)).ToList();
                people.ForEach(i => i.EditToken = Guid.NewGuid());

                foreach (Person person in people)
                {
                    Person clone = Serializer.DeepClone<Person>(person);

                    Guardian obj = new Guardian() { EditToken = Guid.NewGuid() };

                    obj.Person = clone;
                    obj.WasModified = true;
                    obj.Active = true;

                    _Player.Guardians.Add(obj);
                }
            }
        }

        public static Guid AddGuardian(int clubID)
        {
            Guardian obj = new Guardian() { EditToken = Guid.NewGuid() };
            obj.Person = new Person() { EditToken = Guid.NewGuid(), ClubID = clubID };
            obj.WasModified = true;
            obj.Active = true;

            _Player.Guardians.Add(obj);

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
