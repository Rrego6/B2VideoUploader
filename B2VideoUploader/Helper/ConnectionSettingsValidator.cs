using B2VideoUploader.Model;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace B2VideoUploader.Helper
{
    public class ConnectionSettingsValidator
    {

        private readonly Config config;
        private readonly BlackBlazeB2Api b2Api;
        private readonly CredentialConfigForm editConnectionSettingsForm;

        public static string SecretsConfigurationLocation = "settings.ini";

        public ConnectionSettingsValidator(Config config, BlackBlazeB2Api b2api, CredentialConfigForm editConnectionSettingsForm)
        {
            this.config = config;
            b2Api = b2api;
            this.editConnectionSettingsForm = editConnectionSettingsForm;
        }


        public async Task<(bool, string)> ValidateLoginConfiguration()
        {
            var loginResponse = await b2Api.blackBlazeLogin(config.ApplicationId, config.ApplicationKey);
            var status = (string)(loginResponse["status"] ?? string.Empty);
            if (status.StartsWith("4") || status.StartsWith("5"))
            {
                return (false, (string)(loginResponse["message"] ?? string.Empty));
            }
            return (true, String.Empty);
        }

        public async Task<bool> ValidateBucketConfiguration()
        {
            return true;
        }

        /**
         * returns true if connecion settings are changed
         */
        public void EditConnectionSettingsPrompt() {
            editConnectionSettingsForm.ShowDialog();
            editConnectionSettingsForm.Activate();
        }


    }
}
