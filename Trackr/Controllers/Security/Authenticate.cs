using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Trackr.Controllers.Security
{
    public static class Authenticate
    {
        private static Dictionary<Guid, int> KnownUsers = new Dictionary<Guid, int>();

        public static int GetUserID(Guid token)
        {
            return KnownUsers[token];
        }

        public static bool TokenExists(Guid token)
        {
            return KnownUsers.ContainsKey(token);
        }

        public static Guid RegenerateToken(int userID)
        {
            List<Guid> currentTokens = KnownUsers.Where(i => i.Value == userID).Select(i => i.Key).ToList();

            foreach (Guid currentToken in currentTokens)
            {
                KnownUsers.Remove(currentToken);
            }

            Guid token = Guid.NewGuid();
            KnownUsers.Add(token, userID);
            return token;
        }

    }
}