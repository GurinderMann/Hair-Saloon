using Parrucchiere.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.Services.Description;
using System.Data.Entity;
using System.Security.Cryptography;
using System.Text;

namespace Parrucchiere.Controllers
{
    public class ProfiloController : Controller
    {
        ModelDbContext db = new ModelDbContext();
        // GET: Profilo
        public ActionResult Index()
        {

            var u = User.Identity.Name;

            var utente = db.Utenti.Where(us => us.Username == u).FirstOrDefault();
            var user = db.Utenti.Where(ur => ur.IdUtente == utente.IdUtente);




            var prenotazioni = db.Prenotazioni
            .Where(p => p.FkUtente == utente.IdUtente)
            .OrderByDescending(p => p.Data)
            .Take(5)
            .Select(p => new SelectListItem
            {
                Text = p.Data.ToString()
            })
            .ToList();
            ViewBag.TipoOptions = prenotazioni;
            return View(user);
        }

        //Edit del profilo tranne la password
        public ActionResult Edit(int id)
        {
            var u = db.Utenti.Find(id);
            return View(u);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit (Utenti u, int id) 
        {
            var user = db.Utenti.Find(id);
            if (user != null) 
            {
                user.Nome = u.Nome;
                user.Email = u.Email;
                user.Cognome = u.Cognome;
                user.Telefono = u.Telefono;
                user.Username = u.Username;
                db.Entry(user).State =EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("Index");   
            }
            return View(u);
        }


        //Delete del utente
        public ActionResult Delete(int id )
        {
            var user = db.Utenti.Find(id);
            
            if (user != null) 
            {
                var p = db.Prenotazioni.Where(pr => pr.FkUtente == user.IdUtente).ToList();
                var r = db.Recensioni.Where(re => re.FkUtente == user.IdUtente).ToList() ;
                var ris = db.Risposte.Where(ri => ri.FkUtente == user.IdUtente).ToList();
                var dom = db.Domande.Where(d => d.FkUtente == user.IdUtente).ToList();
              
                    foreach (var item in p)
                    {
                        db.Prenotazioni.Remove(item);
                       
                    }

                    foreach (var item in r)
                    {
                        db.Recensioni.Remove(item);

                    }

                    foreach (var item in dom)
                    {
                        db.Domande.Remove(item);
                    }

                    foreach (var item in ris)
                    {
                        db.Risposte.Remove(item);
                    }


                    db.Utenti.Remove(user);
                    db.SaveChanges() ;
                    FormsAuthentication.SignOut();
                    return RedirectToAction("Login", "Login");
                
               
            }

            return View();
        }


        //Edit per la password
        public ActionResult EditPass(int id)
        {
            var user = db.Utenti.Find(id);
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditPass(int id, string currentPassword, string newPassword, string confirmPassword)
        {
            var user = db.Utenti.Find(id);

            if (user != null)
            {
                // Recupera il sale memorizzato per l'utente dal database
                byte[] saltBytes = Convert.FromBase64String(user.Salt);

                // Calcola l'hash della password inserita dall'utente utilizzando lo stesso sale
                byte[] passwordBytes = Encoding.UTF8.GetBytes(currentPassword);
                byte[] combinedBytes = new byte[saltBytes.Length + passwordBytes.Length];
                Buffer.BlockCopy(saltBytes, 0, combinedBytes, 0, saltBytes.Length);
                Buffer.BlockCopy(passwordBytes, 0, combinedBytes, saltBytes.Length, passwordBytes.Length);

                using (SHA256 sha256 = SHA256.Create())
                {
                    byte[] hashedBytes = sha256.ComputeHash(combinedBytes);
                    string hashedPassword = Convert.ToBase64String(hashedBytes);

                    // Verifica se l'hash calcolato corrisponde all'hash memorizzato nel database
                    if (user.Password == hashedPassword)
                    {
                        // Verifica se la nuova password e la password di conferma corrispondono
                        if (newPassword == confirmPassword)
                        {
                            bool passVer = true;

                            if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 8)
                            {
                                ModelState.AddModelError("Password", "La password deve essere di almeno 8 caratteri.");
                                passVer = false;
                            }
                            else if (!newPassword.Any(char.IsUpper) || !newPassword.Any(char.IsDigit))
                            {
                                ModelState.AddModelError("Password", "La password deve contenere almeno una lettera maiuscola e un numero.");
                                passVer = false;
                            }

                            if(passVer ) 
                            {
                                byte[] salt = new byte[16];
                                using (RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider())
                                {
                                    rngCsp.GetBytes(salt);
                                }

                                // Calcola l'hash della nuova password con il nuovo sale
                                byte[] newPasswordBytes = Encoding.UTF8.GetBytes(newPassword);
                                byte[] newCombinedBytes = new byte[salt.Length + newPasswordBytes.Length];
                                Buffer.BlockCopy(salt, 0, newCombinedBytes, 0, salt.Length);
                                Buffer.BlockCopy(newPasswordBytes, 0, newCombinedBytes, salt.Length, newPasswordBytes.Length);

                                using (SHA256 newSha256 = SHA256.Create())
                                {
                                    byte[] newHashedBytes = newSha256.ComputeHash(newCombinedBytes);
                                    string newHashedPassword = Convert.ToBase64String(newHashedBytes);

                                    // Aggiorna la password e il sale nel database
                                    user.Password = newHashedPassword;
                                    user.Salt = Convert.ToBase64String(salt);
                                    db.Entry(user).State = EntityState.Modified;
                                    db.SaveChanges();

                                    return RedirectToAction("Index");
                                }
                            // Genera un nuovo "sale" casuale
                         
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("ConfirmPassword", "La nuova password e la conferma della password devono corrispondere.");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("CurrentPassword", "La password attuale è incorretta.");
                    }
                }
            }

            return View(user);
        }



    }
}