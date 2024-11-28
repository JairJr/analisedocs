using Azure;
using Azure.AI.DocumentIntelligence;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Desafio2AI102.Pages
{
    public class IndexModel : PageModel
    {
        private readonly string AzureBlobConnectionString = Utils.ConfigurationUtils.GetAzureStorageConnectionString();
        private readonly string BlobContainerName = Utils.ConfigurationUtils.GetBlobContainerName();
        private readonly string AzureAIEndpoint = Utils.ConfigurationUtils.GetAzureAIEndpoint();
        private readonly string AzureAIKey = Utils.ConfigurationUtils.GetAzureAIKey();



        [BindProperty]
        public IFormFile UploadedFile { get; set; }
        public string? Message { get; set; }
        public string? BlobUrl { get; set; }
        public CreditCardInfo creditCardInfo { get; set; }

        public void OnGet()
        {
            Console.WriteLine(AzureBlobConnectionString);
            Console.WriteLine(BlobContainerName);
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (UploadedFile == null || UploadedFile.Length == 0)
            {
                Message = "Por favor, envie um arquivo válido.";
                return Page();
            }

            string fileName = Path.GetFileName(UploadedFile.FileName);

            try
            {
                // Upload para o Azure Blob Storage
                BlobUrl = await UploadBlobToAzure(UploadedFile, fileName);

                // Analisar o cartão de crédito
                creditCardInfo = await AnalyzeCreditCard(BlobUrl);

                return Page();
            }
            catch (Exception ex)
            {
                Message = $"Erro ao processar o arquivo: {ex.Message}";
                return Page();
            }
        }

        private async Task<string> UploadBlobToAzure(IFormFile file, string fileName)
        {
            BlobContainerClient containerClient = new BlobContainerClient(AzureBlobConnectionString, BlobContainerName);
            await containerClient.CreateIfNotExistsAsync();

            BlobClient blobClient = containerClient.GetBlobClient(fileName);

            using (var stream = file.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, overwrite: true);
            }

            return blobClient.Uri.ToString();
        }

        private async Task<CreditCardInfo> AnalyzeCreditCard(string blobUrl)
        {

            AzureKeyCredential credential = new AzureKeyCredential(AzureAIKey);
            DocumentIntelligenceClient client = new DocumentIntelligenceClient(new Uri(AzureAIEndpoint), credential);

            AnalyzeDocumentContent content = new AnalyzeDocumentContent()
            {
                UrlSource = new Uri(blobUrl)
            };

            Operation<AnalyzeResult> operation = await client.AnalyzeDocumentAsync(WaitUntil.Completed, "prebuilt-creditCard", content);

            AnalyzeResult result = operation.Value;

            foreach (var document in result.Documents)
            {
                var fields = document.Fields;

                return new CreditCardInfo
                {
                    CardName = fields.TryGetValue("CardHolderName", out var nameField) ? nameField.Content : null,
                    CardNumber = fields.TryGetValue("CardNumber", out var numberField) ? numberField.Content : null,
                    ExpireDate = fields.TryGetValue("ExpirationDate", out var dateField) ? dateField.Content : null,
                    BankName = fields.TryGetValue("IssuingBank", out var bankField) ? bankField.Content : null
                };
            }

            return null;
        }

        public class CreditCardInfo
        {
            public string? CardName { get; set; }
            public string? CardNumber { get; set; }
            public string? ExpireDate { get; set; }
            public string? BankName { get; set; }
        }
    }
}
