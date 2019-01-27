using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using BTPBCommon.Clients;
using BTPBCommon.Exceptions;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace BTPBPlatform.Models
{
    /// <summary>
    /// Represents a platform user View-model 
    /// </summary>
    public class ClientUser
    {
        /// <summary>
        /// The username of the of the user being signed in.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// The full name of the user being signed in.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The ID of the client the user being signed in belongs to.
        /// </summary>
        public int ClientId { get; set; }

        /// <summary>
        /// The password of the client being signed in.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// The user type of the client being signed in. 
        /// </summary>
        public string UserType { get; set; }

        /// <summary>
        /// Authenticates this view model with the credentials obtained from the database.
        /// </summary>
        /// <param name="user">The <see cref="BTPBCommon.Clients.User"/> object retrived from the databse.</param>
        /// <returns>True if the credentials provided during sign-in match the credentials stored in the DB, false otherwise</returns>
        public bool Authenticate(User user)
        {
            if (this.ClientId != user.ClientId)
            {
                throw new BTPBInvalidUsernameException();
            }
            string salted = this.Password + user.Salt;
            string hashed = Authentication.Sha2_256(salted);
            bool authenticated = hashed.Equals(user.Password);
            if (authenticated)
            {
                this.Password = hashed;
                this.Title = user.Title;
                this.UserType = user.UserType.Key;
            }
            return authenticated;
        }
    }
}
