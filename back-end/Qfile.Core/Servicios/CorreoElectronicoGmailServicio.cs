using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit;
using MimeKit;

namespace Qfile.Core.Servicios
{
    public class CorreoElectronicoGmailServicio : ICorreoElectronicoServicio
    {
        private const string _smtpServidor = "smtp.gmail.com";
        private const int _smtpPuerto = 587;
        private const string _email = "qfile.dev@gmail.com";
        private const string _password = "nntqilwedwbitgyu";


        public CorreoElectronicoGmailServicio()
        {
            
        }

        public async Task<bool> Enviar(string para, string asunto, string mensaje)
        {
            if (String.IsNullOrEmpty(para))
                return false;

            MimeMessage objetoMensaje = new MimeMessage();
            objetoMensaje.From.Add(new MailboxAddress("QFILE", _email));
            objetoMensaje.To.Add(new MailboxAddress("Destino", para));
            objetoMensaje.Subject = asunto;

            BodyBuilder cuerpoMensaje = new BodyBuilder();
            cuerpoMensaje.TextBody = mensaje;
            //cuerpoMensaje.HtmlBody = mensaje; // para cuando necesitemos utlizar plantillas
            objetoMensaje.Body = cuerpoMensaje.ToMessageBody();

            SmtpClient clienteSmtp = new SmtpClient();
            clienteSmtp.CheckCertificateRevocation = false;
            clienteSmtp.Connect(_smtpServidor, _smtpPuerto, MailKit.Security.SecureSocketOptions.StartTls);
            clienteSmtp.Authenticate(_email, _password);
            clienteSmtp.Send(objetoMensaje);
            clienteSmtp.Disconnect(true);

            return true;
        }
    }
}