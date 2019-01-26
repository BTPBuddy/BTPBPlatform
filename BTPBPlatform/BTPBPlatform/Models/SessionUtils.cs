using BTPBCommon.Clients;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BTPBPlatform.Models
{
    /// <summary>
    /// Utility class containing useful methods for managing a browser Session.
    /// </summary>
    public static class SessionUtils
    {
        /// <summary>
        /// Checks to see if there is a user actively authenticated in the browser Session.
        /// </summary>
        /// <param name="session">The browser session</param>
        /// <returns>True if there is currently an authenticated user in the Session, false otherwise.</returns>
        public static bool SessionAuthenticated(ISession session)
        {
            return session.GetString("user") != null;
        }

        /// <summary>
        /// If there is currently a user authenticated, removes the User from the Session.
        /// </summary>
        /// <param name="session">The browser session</param>
        public static void UnauthenticateSession(ISession session)
        {
            if (SessionAuthenticated(session))
            {
                session.Remove("user");
            }
        }

        /// <summary>
        /// Returns the currently connected user's Title.
        /// </summary>
        /// <param name="session">The browser session.</param>
        /// <returns>The Title of the currently connected user.</returns>
        public static string GetSessionUserTitle(ISession session)
        {
            string json = session.GetString("user");
            ClientUser user = JsonConvert.DeserializeObject<ClientUser>(json);
            return user.Title;
        }

        /// <summary>
        /// Returns the currently connected user's Model representation.
        /// </summary>
        /// <param name="session">The browser session.</param>
        /// <returns>The model representation for the currently connected user.</returns>
        public static ClientUser FromJson(ISession session)
        {
            string json = session.GetString("user");
            if (json != null)
            {
                return JsonConvert.DeserializeObject<ClientUser>(json);
            }
            else
            {
                return null;
            }
        }
    }
}
