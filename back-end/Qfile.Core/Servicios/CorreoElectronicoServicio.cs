using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Qfile.Core.Servicios
{
    public class CorreoElectronicoServicio : ICorreoElectronicoServicio
    {
        private const string _de = "ealexander.uo@gmail.com";
        private const string _key = "";

        private SendGridClient _cliente;

        public CorreoElectronicoServicio()
        {
            _cliente = new SendGridClient(_key);
        }

        public async Task<bool> Enviar(string para, string asunto, string mensaje)
        {
            var from = new EmailAddress(_de);
            var to = new EmailAddress(para);
            var subject = asunto;
            var plainTextContent = mensaje;
            var htmlContent = mensaje;

            var msg = MailHelper.CreateSingleEmail(
                    from,
                    to,
                    subject,
                    plainTextContent,
                    htmlContent
                );

            var response = await _cliente.SendEmailAsync(msg);

            return true;
        }
    }
}