using IKimWebService.Infrastructure.CustomException;
using IKimWebService.Infrastructure.IRepository;
using IKimWebService.Infrastructure.IService;
using IKimWebService.Infrastructure.Utils;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;

namespace IKimWebService.Service
{
    public class ForgotPasswordService : IForgotPasswordService
    {
        private readonly IUserRepository _userRepository;
        private readonly IFileReaderService _fileReaderService;
        private readonly IEmailSenderService _emailSenderService;


        public ForgotPasswordService(IUserRepository userRepo, IFileReaderService fileReaderService, IEmailSenderService emailSenderService)
        {
            _userRepository = userRepo;
            _fileReaderService = fileReaderService;
            _emailSenderService = emailSenderService;
        }

        public void Initiate(Domain.ForgotPassword mForgotPassword)
        {
            if (mForgotPassword.EmailAddress.IsNullOrEmpty())
                throw new APIRequestFailedException("Email address is required!");

            var mUser = _userRepository.GetBy(mForgotPassword.EmailAddress);
            if (mUser == null || mUser.Id == default(int))
                throw new APIRequestFailedException("No user found with the provided email address.");

            mUser.PlainTextPassword = StaticMethods.GeneratePassword(8);
            mUser.Password = PasswordEncryptor.Encrypt(mUser.PlainTextPassword, ConfigurationManager.AppSettings.Get("Salt"));
            _userRepository.SetPasswordBy(mUser.Id, mUser.Password);
            SendEmailOnForgotPassword(mUser);
        }

        #region Private Method

        private string PopulateEmailTemplateForResetPassword(string template, Domain.User user)
        {
            var replacements = new Dictionary<string, string>
            {
                { "{{FirstName}}", user.FirstName },
                { "{{EmailAddress}}", user.EmailAddress },
                { "{{Password}}", user.PlainTextPassword},
            };
            foreach (var pair in replacements)
            {
                template = template.Replace(pair.Key, pair.Value);
            }
            return template;
        }

        private void SendEmailOnForgotPassword(Domain.User user)
        {
            string content = _fileReaderService.ReadFileContent("~/EmailTemplates/SendCredentialsOnForgotPassword.html");
            string finalContent = PopulateEmailTemplateForResetPassword(content, user);
            Task.Run(async () =>
            {
                try
                {
                    await _emailSenderService.SendEmailAsync("🔒 Reset Your Account Password", finalContent, user.EmailAddress);
                }
                catch (Exception ex)
                {
                    System.IO.File.AppendAllText("ErrorLog.txt", $"{DateTime.Now}: Background send failed - {ex.Message}{Environment.NewLine}");
                }
            });
        }
        #endregion
    }
}