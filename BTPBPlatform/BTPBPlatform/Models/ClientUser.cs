using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BTPBCommon.Clients;
using BTPBCommon.Exceptions;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace BTPBPlatform.Models
{
    public class ClientUser
    {
        public string Username { get; set; }
        public string Title { get; set; }
        public int ClientId { get; set; }
        public string Password { get; set; }
        public string UserType { get; set; }

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
