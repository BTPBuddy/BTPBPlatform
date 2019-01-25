using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BTPBCommon.Clients;
using BTPBCommon.Exceptions;
using BTPBPlatform.Models;
using Microsoft.AspNetCore.Mvc;

namespace BTPBPlatform.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(ClientUser cUser)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    User user = new User(cUser.Username);
                    string salted = cUser.Password + user.Salt;
                    string hashed = Authentication.Sha2_256(salted);
                    if (user.Password.Equals(hashed))
                    {
                        return Json(user);
                    }
                    else
                    {
                        throw new BTPBInvalidLoginException();
                    }
                }
                else
                {
                    throw new BTPBInvalidLoginException();
                }
                
            }
            catch (BTPBException ex)
            {
                return Json(ex);
            }
        }

    }
}