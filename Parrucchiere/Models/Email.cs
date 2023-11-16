using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MimeKit;
using MailKit.Net.Smtp;

namespace Parrucchiere.Models
{
    public class Email
    { 

        public string smtpServer = "smtp.gmail.com";
        public  int smtpPort = 587;
        public string smtpUsername = "parrucchiereg12@gmail.com";

       //Inserire qua dentro la password del Readme
        public string smtpPassword = "muyt nvwv uutm efeh";
        //Email informativa per la prenotazione andata a buon fine del appuntamento
        public void SendConfirmationEmail(string recipientEmail, Prenotazioni appointment)
        {
           

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
                message.Subject = "Conferma appuntamento";

                //Imposto il messaggio da inviare nella mail
                var body = new TextPart("plain")
                {
                    Text = "Gentile cliente il suo appuntamento è stato confermato in data: " + appointment.Data + ".\n\nGrazie per aver prenotato da noi."
                };

                message.Body = body;

                //Invio del messaggio
                smtpClient.Send(message);

                //Disconnessione dal client
                smtpClient.Disconnect(true);
            }
        }

        //Email informativa sulla modifica del appuntamento
        public void SendEditEmail(string recipientEmail, Prenotazioni appointment)
        {
         
          
            using (var smtpClient = new SmtpClient())
            {
                smtpClient.Connect(smtpServer, smtpPort, false);
                smtpClient.Authenticate(smtpUsername, smtpPassword);

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("Magic Hair Saloon", smtpUsername));
                message.To.Add(new MailboxAddress("Recipient", recipientEmail));
                message.Subject = "Modifica del suo appuntamento";

                var body = new TextPart("plain")
                {
                    Text = "Gentile cliente il suo appuntamento è stato modificato e spostato  in data: " + appointment.Data + ".\n\nGrazie per aver prenotato da noi."
                };

                message.Body = body;

                smtpClient.Send(message);
                smtpClient.Disconnect(true);
            }
        }


        //Email informativa sulla cancellazione del appuntamento
        public void SendDeleteEmail(string recipientEmail, Prenotazioni appointment)
        {


          
            using (var smtpClient = new SmtpClient())
            {
                smtpClient.Connect(smtpServer, smtpPort, false);
                smtpClient.Authenticate(smtpUsername, smtpPassword);

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("Magic Hair Saloon", smtpUsername));
                message.To.Add(new MailboxAddress("Recipient", recipientEmail));
                message.Subject = "Cancellazione del suo appuntamento";

                var body = new TextPart("plain")
                {
                    Text = "Gentile cliente, le confermiamo la cancellazione del suo appuntamento in data: " + appointment.Data + ".\n\nCi dispiace per l'inconveniente e speriamo di vederla presto."
                };

                message.Body = body;

                smtpClient.Send(message);
                smtpClient.Disconnect(true);
            }
        }


        
       
    }
}