using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Telerik.OpenAccess.FetchOptimization;
using TrackrModels;


namespace Trackr.Source.Wizards
{
    public class PlayerModifiedByAnotherProcessException : Exception {
        public PlayerModifiedByAnotherProcessException() : base() { }
        public PlayerModifiedByAnotherProcessException(string message) : base(message) { }
    }

    public static class PlayerManager
    {
        private static Player _Player
        {
            get
            {
                return HttpContext.Current.Session["Player"] as Player;
            }
            set
            {
                HttpContext.Current.Session["Player"] = value;
            }
        }

        public static void CreatePlayer(int clubID)
        {
            _Player = new Player();
            _Player.Person = new Person();
            _Player.Person.EditToken = Guid.NewGuid();
            _Player.Person.ClubID = clubID;
        }

        public static void EditPlayer(int playerID)
        {
            using (PlayersController pc = new PlayersController())
            {
                FetchStrategy fetch = new FetchStrategy() { MaxFetchDepth = 4 };
                fetch.LoadWith<Player>(i => i.Guardians);
                fetch.LoadWith<Player>(i => i.Person);
                fetch.LoadWith<Guardian>(i => i.Person);
                fetch.LoadWith<Person>(i => i.Addresses);
                fetch.LoadWith<Person>(i => i.EmailAddresses);
                fetch.LoadWith<Person>(i => i.PhoneNumbers);
                fetch.LoadWith<Player>(i => i.PlayerPasses);
                fetch.LoadWith<Player>(i => i.TeamPlayers);
                fetch.LoadWith<PlayerPass>(i => i.TeamPlayers);
                fetch.LoadWith<PlayerPass>(i => i.Photo);

                _Player = pc.GetWhere(i => i.PlayerID == playerID, fetch).First();

                _Player.Person.EditToken = Guid.NewGuid();

                foreach (Guardian guardian in _Player.Guardians)
                {
                    guardian.EditToken = Guid.NewGuid();
                    guardian.Person.EditToken = Guid.NewGuid();

                    foreach (EmailAddress emailAddress in guardian.Person.EmailAddresses)
                    {
                        emailAddress.EditToken = Guid.NewGuid();
                    }

                    foreach (PhoneNumber phoneNumber in guardian.Person.PhoneNumbers)
                    {
                        phoneNumber.EditToken = Guid.NewGuid();
                    }

                    foreach (Address address in guardian.Person.Addresses)
                    {
                        address.EditToken = Guid.NewGuid();
                    }
                }

                foreach (EmailAddress emailAddress in _Player.Person.EmailAddresses)
                {
                    emailAddress.EditToken = Guid.NewGuid();
                }

                foreach (PhoneNumber phoneNumber in _Player.Person.PhoneNumbers)
                {
                    phoneNumber.EditToken = Guid.NewGuid();
                }

                foreach (Address address in _Player.Person.Addresses)
                {
                    address.EditToken = Guid.NewGuid();
                }

                foreach (TeamPlayer teamPlayer in _Player.TeamPlayers)
                {
                    teamPlayer.EditToken = Guid.NewGuid();
                }

                foreach (PlayerPass playerPass in _Player.PlayerPasses)
                {
                    playerPass.EditToken = Guid.NewGuid();

                    foreach (TeamPlayer teamPlayer in playerPass.TeamPlayers)
                    {
                        teamPlayer.EditToken = Guid.NewGuid();
                    }
                }
            }
        }

        public static void DisposePlayer()
        {
            HttpContext.Current.Session.Remove("Player");
        }


        public static Player Player
        {
            get { return _Player; }
        }


        #region Person
        public static void UpdatePerson(Guid editToken, string firstName, string lastName, DateTime? dateOfBirth)
        {
            Person obj = (Person)FindEditableObject(editToken);
            obj.FName = firstName;
            obj.LName = lastName;
            obj.WasModified = true;
            obj.DateOfBirth = dateOfBirth;
        }
        #endregion



        #region Guardians
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


        #region Player passes
        public static Guid AddPlayerPass()
        {
            PlayerPass obj = new PlayerPass() { EditToken = Guid.NewGuid() };
            obj.WasModified = true;
            obj.Active = true;

            _Player.PlayerPasses.Add(obj);

            return obj.EditToken;
        }

        public static void DeletePlayerPass(Guid playerPassEditToken)
        {
            PlayerPass obj = (PlayerPass)FindEditableObject(playerPassEditToken);
            if (obj.PlayerPassID > 0)
            {
                obj.Active = false;
                obj.WasModified = true;
            }
            else
            {
                _Player.PlayerPasses.Remove(obj);
            }
        }

        public static void UpdatePlayerPass(Guid editToken, string passNumber, DateTime expires)
        {
            PlayerPass obj = (PlayerPass)FindEditableObject(editToken);
            obj.WasModified = true;
            obj.PassNumber = passNumber;
            obj.Expires = expires;
        }

        public static void UpdatePlayerPassPhoto(Guid editToken, byte[] photo)
        {
            PlayerPass obj = (PlayerPass)FindEditableObject(editToken);
            obj.WasModified = true;
            obj.Photo = photo;
        }
        #endregion


        #region Teams
        public static Guid AddTeamPlayer()
        {
            TeamPlayer obj = new TeamPlayer() { EditToken = Guid.NewGuid() };
            obj.WasModified = true;
            obj.Active = true;

            _Player.TeamPlayers.Add(obj);

            return obj.EditToken;
        }

        public static void DeleteTeamPlayer(Guid teamPlayerEditToken)
        {
            TeamPlayer obj = (TeamPlayer)FindEditableObject(teamPlayerEditToken);
            if (obj.TeamPlayerID > 0)
            {
                obj.Active = false;
                obj.WasModified = true;
            }
            else
            {
                _Player.TeamPlayers.Remove(obj);
            }
        }

        public static void UpdateTeamPlayer(Guid editToken, int teamID, bool isSecondary, Guid? playerPassEditToken)
        {
            TeamPlayer obj = (TeamPlayer)FindEditableObject(editToken);
            obj.WasModified = true;
            obj.IsSecondary = isSecondary;
            obj.TeamID = teamID;

            // remove old one
            if (obj.PlayerPass != null)
            {
                obj.PlayerPass.WasModified = true;
            }

            obj.PlayerPass = null;

            _Player.TeamPlayers.Remove(obj);

            if (playerPassEditToken.HasValue)
            {
                PlayerPass playerPass = (PlayerPass)FindEditableObject(playerPassEditToken.Value);
                playerPass.TeamPlayers.Add(obj);
            }
            else
            {
                _Player.TeamPlayers.Add(obj);
            }
        }
        #endregion


        public static int SaveData(int modifiedByUser)
        {
            //returns player id
            using (PlayersController pc= new PlayersController())
            using(ClubManagement cm = new ClubManagement())
            {
                FetchStrategy fetch = new FetchStrategy() { MaxFetchDepth = 4 };
                fetch.LoadWith<Player>(i => i.Guardians);
                fetch.LoadWith<Player>(i => i.Person);
                fetch.LoadWith<Guardian>(i => i.Person);
                fetch.LoadWith<Person>(i => i.Addresses);
                fetch.LoadWith<Person>(i => i.EmailAddresses);
                fetch.LoadWith<Person>(i => i.PhoneNumbers);
                fetch.LoadWith<Player>(i => i.PlayerPasses);
                fetch.LoadWith<Player>(i => i.TeamPlayers);
                fetch.LoadWith<PlayerPass>(i => i.TeamPlayers);
                fetch.LoadWith<PlayerPass>(i => i.Photo);

                Player freshCopy = _Player.PlayerID > 0 ? pc.GetWhere(i => i.PlayerID == _Player.PlayerID, fetch).First() : new Player(){Person = new Person()};

                //check if any guardians are outdated
                List<int> guardianIDs = _Player.Guardians.Where(i=>i.GuardianID > 0).Select(i=>i.GuardianID).Distinct().ToList();
                var guardians = cm.Guardians.Where(i => guardianIDs.Contains(i.GuardianID)).Select(i => new { GuardianID = i.GuardianID, LastModifiedAt = i.LastModifiedAt }).ToDictionary(i => i.GuardianID);
                foreach (Guardian guardian in _Player.Guardians)
                {
                    if (guardians.ContainsKey(guardian.GuardianID) && guardians[guardian.GuardianID].LastModifiedAt > guardian.LastModifiedAt)
                    {
                        throw new PlayerModifiedByAnotherProcessException("A guardian of this player was modified by another process.");
                    }
                }

                //check if any email addresses are outdated
                List<int> emailAddressIDs = _Player.Person.EmailAddresses.Union(_Player.Guardians.Select(i => i.Person).SelectMany(i => i.EmailAddresses)).Where(i => i.EmailAddressID > 0).Select(i => i.EmailAddressID).Distinct().ToList();
                var emails = cm.EmailAddresses.Where(i => emailAddressIDs.Contains(i.EmailAddressID)).Select(i => new { EmailAddressID = i.EmailAddressID, LastModifiedAt = i.LastModifiedAt }).ToDictionary(i => i.EmailAddressID);
                foreach (EmailAddress emailAddress in _Player.Person.EmailAddresses.Union(_Player.Guardians.Select(i => i.Person).SelectMany(i => i.EmailAddresses)))
                {
                    if (emails.ContainsKey(emailAddress.EmailAddressID) && emails[emailAddress.EmailAddressID].LastModifiedAt > emailAddress.LastModifiedAt)
                    {
                        throw new PlayerModifiedByAnotherProcessException("An email address of this player or their guardian(s) was modified by another process.");
                    }
                }

                //check if any phone numbers are outdated
                List<int> phoneNumberIDs = _Player.Person.PhoneNumbers.Union(_Player.Guardians.Select(i => i.Person).SelectMany(i => i.PhoneNumbers)).Where(i => i.PhoneNumberID > 0).Select(i => i.PhoneNumberID).Distinct().ToList();
                var phoneNumbers = cm.PhoneNumbers.Where(i => phoneNumberIDs.Contains(i.PhoneNumberID)).Select(i => new { PhoneNumberID = i.PhoneNumberID, LastModifiedAt = i.LastModifiedAt }).ToDictionary(i => i.PhoneNumberID);
                foreach (PhoneNumber phoneNumber in _Player.Person.PhoneNumbers.Union(_Player.Guardians.Select(i => i.Person).SelectMany(i => i.PhoneNumbers)))
                {
                    if (phoneNumbers.ContainsKey(phoneNumber.PhoneNumberID) && phoneNumbers[phoneNumber.PhoneNumberID].LastModifiedAt > phoneNumber.LastModifiedAt)
                    {
                        throw new PlayerModifiedByAnotherProcessException("A phone number of this player or their guardian(s) was modified by another process.");
                    }
                }

                //check if any addresses are outdated
                List<int> addressIDs = _Player.Person.Addresses.Union(_Player.Guardians.Select(i => i.Person).SelectMany(i => i.Addresses)).Where(i => i.AddressID > 0).Select(i => i.AddressID).Distinct().ToList();
                var addresses = cm.Addresses.Where(i => addressIDs.Contains(i.AddressID)).Select(i => new { AddressID = i.AddressID, LastModifiedAt = i.LastModifiedAt }).ToDictionary(i => i.AddressID);
                foreach (Address address in _Player.Person.Addresses.Union(_Player.Guardians.Select(i => i.Person).SelectMany(i => i.Addresses)))
                {
                    if (addresses.ContainsKey(address.AddressID) && addresses[address.AddressID].LastModifiedAt > address.LastModifiedAt)
                    {
                        throw new PlayerModifiedByAnotherProcessException("An address of this player or their guardian(s) was modified by another process.");
                    }
                }

                // check if any playerpasses are outdated
                List<int> playerPassIDs = _Player.PlayerPasses.Where(i => i.PlayerPassID > 0).Select(i => i.PlayerPassID).Distinct().ToList();
                var playerPasses = cm.PlayerPasses.Where(i => playerPassIDs.Contains(i.PlayerPassID)).Select(i => new { PlayerPassID = i.PlayerPassID, LastModifiedAt = i.LastModifiedAt }).ToDictionary(i => i.PlayerPassID);
                foreach (PlayerPass playerPass in _Player.PlayerPasses)
                {
                    if (playerPasses.ContainsKey(playerPass.PlayerPassID) && playerPasses[playerPass.PlayerPassID].LastModifiedAt > playerPass.LastModifiedAt)
                    {
                        throw new PlayerModifiedByAnotherProcessException("A player pass of this player was modified by another process.");
                    }
                }

                // check if any teamplayers are outdated
                List<int> teamPlayerIDs = _Player.TeamPlayers.Union(_Player.PlayerPasses.SelectMany(i=>i.TeamPlayers)).Where(i => i.TeamPlayerID > 0).Select(i => i.TeamPlayerID).Distinct().ToList();
                var teamPlayers = cm.TeamPlayers.Where(i => teamPlayerIDs.Contains(i.TeamPlayerID)).Select(i => new { TeamPlayerID = i.TeamPlayerID, LastModifiedAt = i.LastModifiedAt }).ToDictionary(i => i.TeamPlayerID);
                foreach (TeamPlayer teamPlayer in _Player.TeamPlayers.Union(_Player.PlayerPasses.SelectMany(i=>i.TeamPlayers)))
                {
                    if (teamPlayers.ContainsKey(teamPlayer.TeamPlayerID) && teamPlayers[teamPlayer.TeamPlayerID].LastModifiedAt > teamPlayer.LastModifiedAt)
                    {
                        throw new PlayerModifiedByAnotherProcessException("A team player of this player was modified by another process.");
                    }
                }

                DateTime modifiedAt = DateTime.Now.ToUniversalTime();

                // everything up to this point is fresh
                if (_Player.PersonID == 0 || (_Player.PersonID > 0 && _Player.Person.WasModified))
                {
                    freshCopy.Person.ClubID = _Player.Person.ClubID;
                    freshCopy.Person.DateOfBirth = _Player.Person.DateOfBirth;
                    freshCopy.Person.FName = _Player.Person.FName;
                    freshCopy.Person.Gender = _Player.Person.Gender;
                    freshCopy.Person.LName = _Player.Person.LName;
                    freshCopy.Person.UserID = _Player.Person.UserID;
                }

                IList<EmailAddress> _freshPlayerEmails = freshCopy.Person.EmailAddresses;
                Copy(_freshPlayerEmails, _Player.Person.EmailAddresses);

                IList<PhoneNumber> _freshPlayerPhones = freshCopy.Person.PhoneNumbers;
                Copy(_freshPlayerPhones, _Player.Person.PhoneNumbers);

                IList<Address> _freshPlayerAddresses = freshCopy.Person.Addresses;
                Copy(_freshPlayerAddresses, _Player.Person.Addresses);

                foreach (Guardian guardian in _Player.Guardians)
                {
                    Guardian _freshCopy = guardian.GuardianID == 0 ? new Guardian(){Person=new Person()} : freshCopy.Guardians.First(i=>i.GuardianID == guardian.GuardianID);

                    if (guardian.PersonID == 0 || (guardian.PersonID > 0 && guardian.Person.WasModified))
                    {
                        _freshCopy.Person.ClubID = guardian.Person.ClubID;
                        _freshCopy.Person.DateOfBirth = guardian.Person.DateOfBirth;
                        _freshCopy.Person.FName = guardian.Person.FName;
                        _freshCopy.Person.Gender = guardian.Person.Gender;
                        _freshCopy.Person.LName = guardian.Person.LName;
                        _freshCopy.Person.MInitial = guardian.Person.MInitial;
                        _freshCopy.Person.UserID = guardian.Person.UserID;
                    }

                    if (guardian.GuardianID == 0 || (guardian.GuardianID > 0 && guardian.WasModified))
                    {
                        _freshCopy.SortOrder = guardian.SortOrder;
                        _freshCopy.Active = guardian.Active;
                    }

                    IList<EmailAddress> _freshCopyGuardianEmails = _freshCopy.Person.EmailAddresses;
                    Copy(_freshCopyGuardianEmails, guardian.Person.EmailAddresses);

                    IList<Address> _freshCopyGuardianAddresses = _freshCopy.Person.Addresses;
                    Copy(_freshCopyGuardianAddresses, guardian.Person.Addresses);

                    IList<PhoneNumber> _freshCopyGuardianPhones = _freshCopy.Person.PhoneNumbers;
                    Copy(_freshCopyGuardianPhones, guardian.Person.PhoneNumbers);

                    if (_freshCopy.GuardianID == 0)
                    {
                        freshCopy.Guardians.Add(_freshCopy);
                    }
                }

                IEnumerable<TeamPlayer> _allUnionedFreshTeamPlayers = freshCopy.TeamPlayers.Union(freshCopy.PlayerPasses.SelectMany(i => i.TeamPlayers)).ToList(); // get duplicate via to list

                IList<TeamPlayer> _freshTeamPlayers = freshCopy.TeamPlayers;
                Copy(_freshTeamPlayers, _Player.TeamPlayers, _allUnionedFreshTeamPlayers);

                foreach (PlayerPass playerPass in _Player.PlayerPasses)
                {
                    PlayerPass _freshCopy = playerPass.PlayerPassID == 0 ? new PlayerPass() : freshCopy.PlayerPasses.First(i => i.PlayerPassID == playerPass.PlayerPassID);

                    if (playerPass.PlayerPassID == 0 || playerPass.WasModified)
                    {
                        _freshCopy.Active = playerPass.Active;
                        _freshCopy.Expires = playerPass.Expires;
                        _freshCopy.PassNumber = playerPass.PassNumber;
                        _freshCopy.Photo = playerPass.Photo;
                    }

                    IList<TeamPlayer> _freshPlayerPassTeamPlayers = _freshCopy.TeamPlayers;
                    Copy(_freshPlayerPassTeamPlayers, playerPass.TeamPlayers, _allUnionedFreshTeamPlayers);

                    if (_freshCopy.PlayerPassID == 0)
                    {
                        freshCopy.PlayerPasses.Add(_freshCopy);
                    }
                }

                freshCopy.Person.LastModifiedAt = modifiedAt;
                freshCopy.Person.LastModifiedBy = modifiedByUser;
                freshCopy.Guardians.ToList().ForEach(i => { i.LastModifiedAt = modifiedAt; i.LastModifiedBy = modifiedByUser; });
                freshCopy.Guardians.Select(i => i.Person).ToList().ForEach(i => { i.LastModifiedAt = modifiedAt; i.LastModifiedBy = modifiedByUser; });
                freshCopy.Person.EmailAddresses.Union(freshCopy.Guardians.Select(i => i.Person).SelectMany(i => i.EmailAddresses)).ToList().ForEach(i => { i.LastModifiedAt = modifiedAt; i.LastModifiedBy = modifiedByUser; });
                freshCopy.Person.PhoneNumbers.Union(freshCopy.Guardians.Select(i => i.Person).SelectMany(i => i.PhoneNumbers)).ToList().ForEach(i => { i.LastModifiedAt = modifiedAt; i.LastModifiedBy = modifiedByUser; });
                freshCopy.Person.Addresses.Union(freshCopy.Guardians.Select(i => i.Person).SelectMany(i => i.Addresses)).ToList().ForEach(i => { i.LastModifiedAt = modifiedAt; i.LastModifiedBy = modifiedByUser; });

                freshCopy.PlayerPasses.ToList().ForEach(i => { i.LastModifiedAt = modifiedAt; i.LastModifiedBy = modifiedByUser; });
                freshCopy.TeamPlayers.Union(freshCopy.PlayerPasses.SelectMany(i => i.TeamPlayers)).ToList().ForEach(i => { i.LastModifiedAt = modifiedAt; i.LastModifiedBy = modifiedByUser; });

                if (freshCopy.PlayerID == 0)
                {
                    return pc.AddNew(freshCopy).PlayerID;
                }
                else
                {
                    pc.Update(freshCopy);
                    return freshCopy.PlayerID;
                }
            }
        }

        private static void Copy(IList<EmailAddress> freshCopies, IList<EmailAddress> dirtyCopies)
        {
            foreach (EmailAddress dirtyCopy in dirtyCopies)
            {
                if (!dirtyCopy.WasModified)
                {
                    continue;
                }

                EmailAddress _freshCopy = dirtyCopy.EmailAddressID == 0 ? new EmailAddress() : freshCopies.First(i => i.EmailAddressID == dirtyCopy.EmailAddressID);

                _freshCopy.Email = dirtyCopy.Email;
                _freshCopy.IsHTML = dirtyCopy.IsHTML;
                _freshCopy.SortOrder = dirtyCopy.SortOrder;
                _freshCopy.Active = dirtyCopy.Active;

                if (_freshCopy.EmailAddressID == 0)
                {
                    freshCopies.Add(_freshCopy);
                }
            }
        }

        private static void Copy(IList<PhoneNumber> freshCopies, IList<PhoneNumber> dirtyCopies)
        {
            foreach (PhoneNumber dirtyCopy in dirtyCopies)
            {
                if (!dirtyCopy.WasModified)
                {
                    continue;
                }

                PhoneNumber _freshCopy = dirtyCopy.PhoneNumberID == 0 ? new PhoneNumber() : freshCopies.First(i => i.PhoneNumberID == dirtyCopy.PhoneNumberID);

                _freshCopy.TenDigit = dirtyCopy.TenDigit;
                _freshCopy.Extension = dirtyCopy.Extension;
                _freshCopy.SortOrder = dirtyCopy.SortOrder;
                _freshCopy.Active = dirtyCopy.Active;

                if (_freshCopy.PhoneNumberID == 0)
                {
                    freshCopies.Add(_freshCopy);
                }
            }
        }

        private static void Copy(IList<Address> freshCopies, IList<Address> dirtyCopies)
        {
            foreach (Address dirtyCopy in dirtyCopies)
            {
                if (!dirtyCopy.WasModified)
                {
                    continue;
                }

                Address _freshCopy = dirtyCopy.AddressID == 0 ? new Address() : freshCopies.First(i => i.AddressID == dirtyCopy.AddressID);

                _freshCopy.Street1 = dirtyCopy.Street1;
                _freshCopy.Street2 = dirtyCopy.Street2;
                _freshCopy.City = dirtyCopy.City;
                _freshCopy.State = dirtyCopy.State;
                _freshCopy.ZipCode = dirtyCopy.ZipCode;
                _freshCopy.SortOrder = dirtyCopy.SortOrder;
                _freshCopy.Active = dirtyCopy.Active;

                if (_freshCopy.AddressID == 0)
                {
                    freshCopies.Add(_freshCopy);
                }
            }
        }

        private static void Copy(IList<PlayerPass> freshCopies, IList<PlayerPass> dirtyCopies)
        {
            foreach (PlayerPass dirtyCopy in dirtyCopies)
            {
                if (!dirtyCopy.WasModified)
                {
                    continue;
                }

                PlayerPass _freshCopy = dirtyCopy.PlayerPassID == 0 ? new PlayerPass() : freshCopies.First(i => i.PlayerPassID == dirtyCopy.PlayerPassID);

                _freshCopy.Active = dirtyCopy.Active;
                _freshCopy.Expires = dirtyCopy.Expires;
                _freshCopy.PassNumber = dirtyCopy.PassNumber;
                _freshCopy.Photo = dirtyCopy.Photo;

                if (_freshCopy.PlayerPassID == 0)
                {
                    freshCopies.Add(_freshCopy);
                }
            }
        }

        private static void Copy(IList<TeamPlayer> freshCopies, IList<TeamPlayer> dirtyCopies, IEnumerable<TeamPlayer> allUnionedFreshTeamPlayers)
        {
            List<int> teamPlayerIDsToKeep = dirtyCopies.Select(i => i.TeamPlayerID).ToList();

            foreach (TeamPlayer freshCopy in freshCopies.ToList())
            {
                if (!teamPlayerIDsToKeep.Contains(freshCopy.TeamPlayerID))
                {
                    freshCopies.Remove(freshCopy);
                }
            }

            foreach (TeamPlayer dirtyCopy in dirtyCopies)
            {
                if (!dirtyCopy.WasModified)
                {
                    continue;
                }

                TeamPlayer _freshCopy = dirtyCopy.TeamPlayerID == 0 ? new TeamPlayer() : allUnionedFreshTeamPlayers.First(i => i.TeamPlayerID == dirtyCopy.TeamPlayerID);

                _freshCopy.Active = dirtyCopy.Active;
                _freshCopy.IsSecondary = dirtyCopy.IsSecondary;
                _freshCopy.TeamID = dirtyCopy.TeamID;

                if (_freshCopy.TeamPlayerID == 0 || freshCopies.Count(i => i.TeamPlayerID == _freshCopy.TeamPlayerID) == 0)
                {
                    freshCopies.Add(_freshCopy);
                }
            }
        }


        #region Utilities
        private static IEditable FindEditableObject(Guid editToken)
        {
            if (_Player.Person.EditToken == editToken)
            {
                return _Player.Person;
            }

            Guardian guardian = _Player.Guardians.FirstOrDefault(i => i.EditToken == editToken);
            if (guardian != null)
            {
                return guardian;
            }

            Person person = _Player.Guardians.Select(i => i.Person).FirstOrDefault(i => i.EditToken == editToken);
            if (person != null)
            {
                return person;
            }

            EmailAddress emailAddress = _Player.Person.EmailAddresses.Union(_Player.Guardians.Select(i => i.Person).SelectMany(i => i.EmailAddresses)).FirstOrDefault(i => i.EditToken == editToken);
            if (emailAddress != null)
            {
                return emailAddress;
            }

            PhoneNumber phoneNumber = _Player.Person.PhoneNumbers.Union(_Player.Guardians.Select(i => i.Person).SelectMany(i => i.PhoneNumbers)).FirstOrDefault(i => i.EditToken == editToken);
            if (phoneNumber != null)
            {
                return phoneNumber;
            }

            Address address = _Player.Person.Addresses.Union(_Player.Guardians.Select(i => i.Person).SelectMany(i => i.Addresses)).FirstOrDefault(i => i.EditToken == editToken);
            if (address != null)
            {
                return address;
            }

            TeamPlayer teamPlayer = _Player.TeamPlayers.Union(_Player.PlayerPasses.SelectMany(i => i.TeamPlayers)).FirstOrDefault(i => i.EditToken == editToken);
            if (teamPlayer != null)
            {
                return teamPlayer;
            }

            PlayerPass playerPass = _Player.PlayerPasses.FirstOrDefault(i => i.EditToken == editToken);
            if (playerPass != null)
            {
                return playerPass;
            }

            return null;
        }
        #endregion
    }
}