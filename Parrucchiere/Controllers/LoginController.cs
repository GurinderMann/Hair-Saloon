using Parrucchiere.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Parrucchiere.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        ModelDbContext db = new ModelDbContext();

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult login()
        {
            return View();
        }



        [HttpPost]
        public ActionResult Login(Utenti u)
        {
            var user = db.Utenti.FirstOrDefault(usr => usr.Username == u.Username);

            if (user != null)
            {
                // Recupera il sale memorizzato per l'utente dal database
                byte[] saltBytes = Convert.FromBase64String(user.Salt);

                // Calcola l'hash della password inserita dall'utente utilizzando lo stesso sale
                byte[] passwordBytes = Encoding.UTF8.GetBytes(u.Password);
                byte[] combinedBytes = new byte[saltBytes.Length + passwordBytes.Length];
                Buffer.BlockCopy(saltBytes, 0, combinedBytes, 0, saltBytes.Length);
                Buffer.BlockCopy(passwordBytes, 0, combinedBytes, saltBytes.Length, passwordBytes.Length);

                using (SHA256 sha256 = SHA256.Create())
                {
                    byte[] hashedBytes = sha256.ComputeHash(combinedBytes);
                    string hashedPassword = Convert.ToBase64String(hashedBytes);

                    // Confronta l'hash calcolato con l'hash memorizzato nel database
                    if (user.Password == hashedPassword)
                    {
                        FormsAuthentication.SetAuthCookie(user.Username, false);
                        return RedirectToAction("Index", "Home");
                    }
                }
            }

            ModelState.AddModelError("Login", "Credenziali non valide. Riprova.");
            return View();
        }



        public ActionResult Registrati()
        {
            return View();
        }
      


        [HttpPost]
        public ActionResult Registrati([Bind(Exclude = "Role")] Utenti u, string ConfermaPassword)
        {
            var utenti = db.Utenti.ToList();
            bool passVer = true; 

            if (string.IsNullOrWhiteSpace(u.Password) || u.Password.Length < 8)
            {
                ModelState.AddModelError("Password", "La password deve essere di almeno 8 caratteri.");
                passVer = false;
            }
            else if (!u.Password.Any(char.IsUpper) || !u.Password.Any(char.IsDigit))
            {
                ModelState.AddModelError("Password", "La password deve contenere almeno una lettera maiuscola e un numero.");
                passVer = false;
            }

            if (u.Password != ConfermaPassword)
            {
                ModelState.AddModelError("Password", "Le password non corrispondono.");
                passVer = false;
            }


           




            if (passVer) 
            {
                bool usernameInUso = false;
                bool emailInUso = false;
                bool telefonoInUso = false;

                foreach (var utente in utenti)
                {
                    if (u.Username == utente.Username)
                    {
                        usernameInUso = true;
                        ModelState.AddModelError("Username", "Username già in uso. Scegliere un altro username.");
                    }

                    if (u.Email == utente.Email)
                    {
                        emailInUso = true;
                        ModelState.AddModelError("Email", "Email già presente.");
                    }

                    if (u.Telefono == utente.Telefono)
                    {
                        telefonoInUso = true;
                        ModelState.AddModelError("Telefono", "Il numero inserito è già presente.");
                    }
                }
                if (usernameInUso || emailInUso || telefonoInUso)
                {

                    return View(u);
                }
                // Genera un "sale" casuale
                byte[] salt = new byte[16];
                using (RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider())
                {
                    rngCsp.GetBytes(salt);
                }

                // Calcola l'hash della password con il sale
                byte[] passwordBytes = Encoding.UTF8.GetBytes(u.Password);
                byte[] combinedBytes = new byte[salt.Length + passwordBytes.Length];
                Buffer.BlockCopy(salt, 0, combinedBytes, 0, salt.Length);
                Buffer.BlockCopy(passwordBytes, 0, combinedBytes, salt.Length, passwordBytes.Length);

                using (SHA256 sha256 = SHA256.Create())
                {
                    byte[] hashedBytes = sha256.ComputeHash(combinedBytes);
                    u.Password = Convert.ToBase64String(hashedBytes);
                }

                u.Salt = Convert.ToBase64String(salt);

                u.Role = "User";
                db.Utenti.Add(u);
                db.SaveChanges();
                return RedirectToAction("Login", "Login");
            }

            return View(u);



        }



        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();

            return RedirectToAction("Index", "Home");
        }
    }
}