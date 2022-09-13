using B2VideoUploader.Model;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        public delegate void OnConnectionStatusUpdated(bool isValid, string errMsg);

        public ConnectionSettingsValidator(Config config, BlackBlazeB2Api b2api, CredentialConfigForm editConnectionSettingsForm)
        {
            this.config = config;
            b2Api = b2api;
            this.editConnectionSettingsForm = editConnectionSettingsForm;
        }


        public async Task<(bool, string)> ValidateLoginConfiguration(OnConnectionStatusUpdated? connectionStatusUpdated = null)
        {
            var loginResponse = await b2Api.blackBlazeLogin(config.ApplicationId, config.ApplicationKey);
            var status = (string)(loginResponse["status"] ?? string.Empty);
            if (status.StartsWith("4") || status.StartsWith("5"))
            {
                if (connectionStatusUpdated != null)
                {
                    connectionStatusUpdated(false, (string)(loginResponse["message"] ?? string.Empty));
                }
                return (false, (string)(loginResponse["message"] ?? string.Empty));
            }
            if (connectionStatusUpdated != null)
            {
                connectionStatusUpdated(true, String.Empty);
            }
            return (true, String.Empty);
        }

        public async Task<(bool, string)> ValidateBucketConfiguration()
        {
            return (true, String.Empty);
        }



        /**
         * returns true if connecion settings are changed
         */
        public void EditConnectionSettingsPrompt(OnConnectionStatusUpdated? connectionStatusUpdated = null) {

            editConnectionSettingsForm.ShowEditCredentialsForm(config.ApplicationId, config.ApplicationKey, config.BucketId, HandleSaveCredentials);
            ValidateLoginConfiguration(connectionStatusUpdated);
        }

        public void HandleSaveCredentials(string applicationId, string applicationKey, string bucketId)
        {
            config.OverWriteSecrets(applicationId, applicationKey, bucketId);
        }


    }
}
