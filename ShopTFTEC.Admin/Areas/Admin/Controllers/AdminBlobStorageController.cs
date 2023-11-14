using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System.Configuration;
using Microsoft.AspNetCore.Authorization;
using IdentityServerHost.Quickstart.UI;
using ShopTFTEC.Admin.Models;

namespace ShopTFTEC.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class AdminBlobStorageController : Controller
    {
        
        private readonly BlobServiceClient blobServiceClient;
        static BlobContainerClient blobContainer;
        private readonly ConfigurationImagens _blobContainerName;

        public AdminBlobStorageController(BlobServiceClient blobServiceClient, IOptions<ConfigurationImagens> myConfiguration)
        {
            this.blobServiceClient = blobServiceClient;
            _blobContainerName = myConfiguration.Value;
        }

        public async Task<ActionResult> Index()
        {
            try
            {
                blobContainer = blobServiceClient.GetBlobContainerClient(_blobContainerName.RepositorioBlob);
                await blobContainer.CreateIfNotExistsAsync(PublicAccessType.Blob);

                // Gets all Block Blobs in the blobContainerName and passes them to the view
                List<Uri> allBlobs = new List<Uri>();
                foreach (BlobItem blob in blobContainer.GetBlobs())
                {
                    if (blob.Properties.BlobType == BlobType.Block)
                        allBlobs.Add(blobContainer.GetBlobClient(blob.Name).Uri);
                }

                return View(allBlobs);
            }
            catch (Exception ex)
            {
                ViewData["message"] = ex.Message;
                ViewData["trace"] = ex.StackTrace;
                return View("Error");
            }
        }

        public async Task<IActionResult> UploadFiles(List<IFormFile> files)
        {
            if (files == null || files.Count == 0)
            {
                ViewData["Erro"] = "Error: Arquivo(s) não selecionado(s)";
                return View(ViewData);
            }

            if (files.Count > 10)
            {
                ViewData["Erro"] = "Error: Quantidade de arquivos excedeu o limite";
                return View(ViewData);
            }

            foreach (var formFile in files)
            {
                if (formFile.FileName.Contains(".jpg") || formFile.FileName.Contains(".gif") ||
                         formFile.FileName.Contains(".png"))
                {
                    var fileName = Path.GetFileName(formFile.FileName);
                    var fileType = Path.GetExtension(fileName);
                    var newFileName = String.Concat(Convert.ToString(Guid.NewGuid()), fileType);

                    Stream stream = formFile.OpenReadStream();

                    BlobClient blob = blobContainer.GetBlobClient(formFile.FileName);

                    blob.Upload(stream, overwrite:true);
                    var fileUrl = blob.Uri.AbsoluteUri;
                }
            }

            //monta a ViewData que será exibida na view como resultado do envio 
            ViewData["Resultado"] = $"{files.Count} arquivos foram enviados ao blobStorage!";

            //ViewBag.Arquivos = filePathsName;

            //retorna a viewdata
            return View(ViewData);
        }

        public async Task<ActionResult> GetImagens()
        {
            blobContainer = blobServiceClient.GetBlobContainerClient(_blobContainerName.RepositorioBlob);
            await blobContainer.CreateIfNotExistsAsync(PublicAccessType.Blob);

            // Gets all Block Blobs in the blobContainerName and passes them to the view
            List<Uri> allBlobs = new List<Uri>();
            foreach (BlobItem blob in blobContainer.GetBlobs())
            {
                if (blob.Properties.BlobType == BlobType.Block)
                    allBlobs.Add(blobContainer.GetBlobClient(blob.Name).Uri);
            }

            if (allBlobs.Count() == 0)
            {
                ViewData["Erro"] = $"Nenhum arquivo encontrado na pasta do blob";
            }

            //model.Files = files;
            //return View(model);

            return View(allBlobs);
        }

        public async Task<ActionResult> DeleteImage(string name)
        {
            try
            {
                Uri uri = new Uri(name);
                string filename = Path.GetFileName(uri.LocalPath);

                var blob = blobContainer.GetBlobClient(filename);
                await blob.DeleteIfExistsAsync();

                return View("Index");
            }
            catch (Exception ex)
            {
                ViewData["message"] = ex.Message;
                ViewData["trace"] = ex.StackTrace;
                return View("Error");
            }
        }

        [HttpPost]
        public async Task<ActionResult> DeleteAll()
        {
            try
            {
                foreach (var blob in blobContainer.GetBlobs())
                {
                    if (blob.Properties.BlobType == BlobType.Block)
                    {
                        await blobContainer.DeleteBlobIfExistsAsync(blob.Name);
                    }
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewData["message"] = ex.Message;
                ViewData["trace"] = ex.StackTrace;
                return View("Error");
            }
        }
    }
}
