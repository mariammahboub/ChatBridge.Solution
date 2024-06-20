using Core.DTOs;
using Core.Interfaces;
using MimeKit;
using MimeKit.Text;
using MailKit.Net.Smtp;
using System;
using System.Threading.Tasks;

namespace Services
{
    public class EmailService : IEmailService
    {
        #region Constructor and Dependencies

        private readonly EmailConfigurationDto _emailConfigurationDto;

        public EmailService(EmailConfigurationDto emailConfigurationDto)
        {
            _emailConfigurationDto = emailConfigurationDto;
        }

        #endregion

        #region Send Email

        public async Task SendEmailAsync(MessageDto message)
        {
            var emailMessage = CreateEmailMessage(message);
            await SendAsync(emailMessage);
        }

        #endregion

        #region Create Email Message

        private MimeMessage CreateEmailMessage(MessageDto message)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("email", _emailConfigurationDto.From));
            emailMessage.To.AddRange(message.To);
            emailMessage.Subject = message.Subject;
            emailMessage.Body = new TextPart(TextFormat.Text) { Text = message.Content };
            return emailMessage;
        }

        #endregion

        #region Send Async

        private async Task SendAsync(MimeMessage mailMessage)
        {
            using var client = new SmtpClient();
            try
            {
                await client.ConnectAsync(_emailConfigurationDto.SmtpServer, _emailConfigurationDto.Port, true);
                client.AuthenticationMechanisms.Remove("XOAUTH2");
                await client.AuthenticateAsync(_emailConfigurationDto.UserName, _emailConfigurationDto.Password);
                await client.SendAsync(mailMessage);
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                client.Disconnect(true);
                client.Dispose();
            }
        }

        #endregion
    }
}
