using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace Activos_PrestamosOET.Controllers
{
    public class SendMailerController : Controller
    {
        //
        // GET: /SendMailer/
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ViewResult Index(Activos_PrestamosOET.Models.MailModel _objModelMail)
        {
            if (ModelState.IsValid)
            {
                /*
                MailMessage mail = new MailMessage();
                mail.To.Add(_objModelMail.To);
                mail.From = new MailAddress(_objModelMail.From);
                mail.Subject = _objModelMail.Subject;
                string Body = _objModelMail.Body;
                mail.Body = Body;
                mail.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com";
                
                smtp.Port = 587;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new System.Net.NetworkCredential("oet.email@gmail.com", "Doroteos");// Enter seders User name and password
                smtp.EnableSsl = true;
                smtp.Send(mail);
                return View("Index", _objModelMail);
                 * */
                MailMessage mail = new MailMessage();
                mail.To.Add(_objModelMail.To);
                mail.From = new MailAddress(_objModelMail.From);
                mail.Subject = _objModelMail.Subject;
                string Body = _objModelMail.Body;
                System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage();

                message.From = new System.Net.Mail.MailAddress("oet.email@gmail.com");
                message.To.Add(new System.Net.Mail.MailAddress("oet.email@gmail.com"));

                message.IsBodyHtml = true;
                //message.BodyEncoding = Encoding.UTF8;
                message.Subject = "subject";
                message.Body = "hello receiver";
                
                System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient();
                client.Credentials = new System.Net.NetworkCredential("oet.email@gmail.com", "Doroteos");
                client.Host = "smtp.gmail.com";
                client.Send(message);
                return View("Index", mail);
            }
            else
            {
                return View();
            }
        }
	}
}