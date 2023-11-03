using MimeKit;
using Parrucchiere.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

using MailKit.Net.Smtp;

namespace Parrucchiere.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        ModelDbContext db = new ModelDbContext();

    

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
            
            //Controllo che la password non sia null e che non sia meno di 8 caratteri
            if (string.IsNullOrWhiteSpace(u.Password) && u.Password.Length < 8)
            {
                ModelState.AddModelError("Password", "La password deve essere di almeno 8 caratteri.");
                passVer = false;
            }

            //Controllo che la password abbia una maiuscola e un numero
            else if (!u.Password.Any(char.IsUpper) && !u.Password.Any(char.IsDigit))
            {
                ModelState.AddModelError("Password", "La password deve contenere almeno una lettera maiuscola e un numero.");

                
                passVer = false;
            }

            //Controllo che la password e il conferma password siano uguali
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
                    //Controllo se esiste già un utente con lo stesso username
                    if (u.Username == utente.Username)
                    {
                        usernameInUso = true;
                        ModelState.AddModelError("Username", "Username già in uso. Scegliere un altro username.");
                    }

                    //Controllo se esiste già un utente con la stesssa mail
                    if (u.Email == utente.Email)
                    {
                        emailInUso = true;
                        ModelState.AddModelError("Email", "Email già presente.");
                    }

                    //Controllo se esiste già un utente con lo stesso numero di telefono
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


        //Recupero password tramite invio codice alla mail
        public ActionResult Forgot () 
        {
            return View(); 
        }

        [HttpPost]
        public ActionResult Forgot(string email)
        {
            //Trovo l'utente legato alla mail
            var user = db.Utenti.FirstOrDefault(usr => usr.Email == email);

            if (user != null)
            {
                // Genera un codice di reset casuale
                string resetCode = GenerateRandomResetCode();

                // Invia il codice di reset all'utente tramite email
                SendResetCodeByEmail(user.Email, resetCode);

                // Memorizza il codice di reset nel database associandolo all'utente
                user.ResetCode = resetCode;

                //Imposto una scadenza al codice di reset
                user.Scadenza = DateTime.Now.AddHours(1); 

                db.SaveChanges();

         
                return RedirectToAction("ResetPassword");
            }
            else
            {
                ModelState.AddModelError("Email", "L'indirizzo email non è associato a un account.");
            }

            return View();
        }

        //Reset della password con codice inviato nella mail e creazione nuova password
        public ActionResult ResetPassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ResetPassword(string resetCode, string newPassword, string confirmPassword)
        {
            var user = db.Utenti.FirstOrDefault(usr => usr.ResetCode == resetCode && usr.Scadenza > DateTime.Now);

            if (user != null)
            {
                // Verifica che la nuova password e la conferma siano uguali
                if (newPassword != confirmPassword)
                {
                    ModelState.AddModelError("newPassword", "Le password non corrispondono.");
                    return View();
                }

                // Genera un nuovo "sale" casuale
                byte[] salt = new byte[16];
                using (RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider())
                {
                    rngCsp.GetBytes(salt);
                }

                // Calcola l'hash della nuova password con il nuovo sale
                byte[] passwordBytes = Encoding.UTF8.GetBytes(newPassword);
                byte[] combinedBytes = new byte[salt.Length + passwordBytes.Length];
                Buffer.BlockCopy(salt, 0, combinedBytes, 0, salt.Length);
                Buffer.BlockCopy(passwordBytes, 0, combinedBytes, salt.Length, passwordBytes.Length);

                using (SHA256 sha256 = SHA256.Create())
                {
                    byte[] hashedBytes = sha256.ComputeHash(combinedBytes);
                    user.Password = Convert.ToBase64String(hashedBytes);
                }

                user.Salt = Convert.ToBase64String(salt);

                // Resetta il codice di reset e la sua scadenza
                user.ResetCode = null;
                user.Scadenza = null;

                db.SaveChanges();

                TempData["Message"] = "La password è stata reimpostata con successo.";
                return RedirectToAction("Login", "Login");
            }
            else
            {
                ModelState.AddModelError("resetCode", "Il codice di reset non è valido o è scaduto.");
            }

            return View();
        }


        //Metodo per inivare mail con codice recupero password
        private void SendResetCodeByEmail(string recipientEmail, string resetCode)
        {
            string smtpServer = "smtp.gmail.com";
            int smtpPort = 587;
            string smtpUsername = "parrucchiereg12@gmail.com";
            string smtpPassword = "muyt nvwv uutm efeh";

            // Creo un nuovo smtp client
            using (var smtpClient = new SmtpClient())
            {
                //Mi connetto al server smtp
                smtpClient.Connect(smtpServer, smtpPort, false);

                //Faccio l'autenticazione
                smtpClient.Authenticate(smtpUsername, smtpPassword);

                //Creo un nuovo messaggio
                var message = new MimeMessage();

                //Imposto il mittente della mail    
                message.From.Add(new MailboxAddress("Magic Hair Saloon", smtpUsername));

                //Imposto il ricevitore della mail 
                message.To.Add(new MailboxAddress("Recipient", recipientEmail));

                //Imposto l'oggetto della mail
                message.Subject = "Codice di recupero password";

                //Imposto il messaggio da inviare nella mail
                var body = new TextPart("plain")
                {
                    Text = "Gentile cliente il suo codice per recuperare la password è: " + resetCode
                };

                message.Body = body;

                //Invio del messaggio
                smtpClient.Send(message);

                //Disconnessione dal client
                smtpClient.Disconnect(true);
            }
        }

        //Genero un codice randomico
        private string GenerateRandomResetCode()
        {
           
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            Random random = new Random();
            return new string(Enumerable.Repeat(chars, 8).Select(s => s[random.Next(s.Length)]).ToArray());
        }

      


    }
}