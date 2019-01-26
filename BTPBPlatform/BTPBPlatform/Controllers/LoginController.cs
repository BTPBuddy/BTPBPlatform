using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BTPBCommon.Clients;
using BTPBCommon.Exceptions;
using BTPBPlatform.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Session;
using Newtonsoft.Json;

namespace BTPBPlatform.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Logout()
        {
            SessionUtils.UnauthenticateSession(HttpContext.Session);
            return RedirectToAction("Login", "Home", new { fail = false, message = "Vous vous êtes déconnecté" });
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
                    if (cUser.Authenticate(user))
                    {
                        HttpContext.Session.SetString("user", JsonConvert.SerializeObject(cUser));
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        throw new BTPBInvalidPasswordException();
                    }
                }
                else
                {
                    throw new DBReadException();
                }
                
            }
            catch (BTPBInvalidPasswordException)
            {
                return RedirectToAction("Login", "Home", new { fail = true,
                    message = "Mot de passe incorrect" });
            }
            catch (BTPBInvalidUsernameException)
            {
                return RedirectToAction("Login", "Home", new { fail = true,
                    message = "Votre nom d'utilisateur ne correspond pas à votre ID client" });
            }
            catch (BTPBException)
            {
                return RedirectToAction("Login", "Home", new { fail = true,
                    message = "Vos identifants sont incorrectes" });
            }
        }

    }
}