using System;
namespace Desafio2AI102.Utils
{
    public static class ConfigurationUtils
    {
        public static string GetAzureStorageConnectionString()
        {
            // Obtém a connection string da variável de ambiente
            string connectionString = Environment.GetEnvironmentVariable("AZUREBLOBCONNECTIONSTRING");

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("A variável de ambiente 'AZUREBLOBCONNECTIONSTRING' não está definida.");
            }

            return connectionString;
        }

        public static string GetBlobContainerName()
        {
            // Obtém o nome do container da variável de ambiente
            string containerName = Environment.GetEnvironmentVariable("BLOBCONTAINERNAME");

            if (string.IsNullOrEmpty(containerName))
            {
                throw new InvalidOperationException("A variável de ambiente 'BLOBCONTAINERNAME' não está definida.");
            }

            return containerName;
        }

        public static string GetAzureAIEndpoint()
        {
            // Obtém o endpoint da API de IA da variável de ambiente
            string endpoint = Environment.GetEnvironmentVariable("AZUREAIENDPOINT");

            if (string.IsNullOrEmpty(endpoint))
            {
                throw new InvalidOperationException("A variável de ambiente 'AZUREAIENDPOINT' não está definida.");
            }

            return endpoint;
        }

        public static string GetAzureAIKey()
        {
            // Obtém o endpoint da API de IA da variável de ambiente
            string key = Environment.GetEnvironmentVariable("AZUREAIKEY");

            if (string.IsNullOrEmpty(key))
            {
                throw new InvalidOperationException("A variável de ambiente 'AZUREAIKEY' não está definida.");
            }

            return key;
        }
    }
}
