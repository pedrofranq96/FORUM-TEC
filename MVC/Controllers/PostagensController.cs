using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using NEGOCIO.Models;
using Newtonsoft.Json;

namespace RedeForum.Controllers
{
    public class PostagensController : Controller
    {
        private string _url = "https://localhost:7096/";

        private const string SESSION_JWT = "SessionJWT";
        private const string SESSION_EMAIL = "SessionEmail";
        private const string REDIRECIONAR_LOGADO = "RedirecinarLogado";
        private IHttpClientFactory _httpClient { get; set; }

        public PostagensController(IHttpClientFactory httpClient)
        {
            _httpClient = httpClient;
        }


        // GET: Posts
        public async Task<IActionResult> Index()
        {

            ViewBag.userEmail = HttpContext.Session.GetString(SESSION_EMAIL); 

            var jwt = HttpContext.Session.GetString(SESSION_JWT);
            if (jwt == null)
            {
                HttpContext.Session.SetString(REDIRECIONAR_LOGADO, "Postagens");
                return RedirectToAction("Login", "Postagens");
            }

            IEnumerable<Postagem> lista = null;
            var client = _httpClient.CreateClient("PegaListaPosts");
            HttpRequestMessage requestMessage = new HttpRequestMessage();

            requestMessage.Headers.Add("Accept", "application/json");
            requestMessage.RequestUri = new Uri(_url + "api/Postagens");
            requestMessage.Method = HttpMethod.Get;
            client.DefaultRequestHeaders.Clear();

            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwt);

            HttpResponseMessage apiResponse = null;
            apiResponse = client.SendAsync(requestMessage).Result;
            var apiContentRaw = apiResponse.Content.ReadAsStringAsync().Result;

            lista = JsonConvert.DeserializeObject<IEnumerable<Postagem>>(apiContentRaw);

            if (lista != null) { return View(lista); }
            else { lista = new List<Postagem>(); }

            return View(lista);
        }

        // GET: Posts/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            var jwt = HttpContext.Session.GetString(SESSION_JWT);
            if (jwt == null)
            {
                HttpContext.Session.SetString(REDIRECIONAR_LOGADO, "Postagens");
                return RedirectToAction("Login", "Home");
            }

            var client = _httpClient.CreateClient("PegaPostporId");
            HttpRequestMessage requestMessage = new HttpRequestMessage();

            requestMessage.Headers.Add("Accept", "application/json");

            requestMessage.RequestUri = new Uri(_url + $"api/Postagens/{id}");
            requestMessage.Method = HttpMethod.Get;
            client.DefaultRequestHeaders.Clear();


            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwt);

            HttpResponseMessage apiResponse = null;
            apiResponse = client.SendAsync(requestMessage).Result;
            var apiContentRaw = apiResponse.Content.ReadAsStringAsync().Result;

            var postRecebido = JsonConvert.DeserializeObject<Postagem>(apiContentRaw);

            if (postRecebido != null)
            { return View(postRecebido); }
            else
            { TempData["RespostaAPI"] = "Não foi possível acessar a API!"; return RedirectToAction("Index"); }
        }

        // GET: Posts/Create
        public IActionResult Create()
        {
            var jwt = HttpContext.Session.GetString(SESSION_JWT);
            if (jwt == null)
            {
                HttpContext.Session.SetString(REDIRECIONAR_LOGADO, "Postagens");
                return RedirectToAction("Login", "Home");
            }
            return View();
        }

        // POST: Posts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,EmailUsuario,Texto,HoraPost,CaminhoImg")] Postagem postagem, IFormFile ImageFile)
        {
            var jwt = HttpContext.Session.GetString(SESSION_JWT);
            if (jwt == null)
            {
                HttpContext.Session.SetString(REDIRECIONAR_LOGADO, "Postagens");
                return RedirectToAction("Login", "Home");
            }

            if (ModelState.IsValid)
            {
                
                postagem.HoraPost = DateTime.Now;
                postagem.CaminhoImg = UploadImage(ImageFile);
                postagem.Id = Guid.NewGuid();
            }
            postagem.EmailUsuario = HttpContext.Session.GetString(SESSION_EMAIL);
            var postJson = JsonConvert.SerializeObject(postagem);

            try
            {
                var client = _httpClient.CreateClient("CreatePostagem");
                var contentData = new StringContent(postJson, Encoding.UTF8, "application/json");
                var endereco = _url + "api/postagens";
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwt);
                HttpResponseMessage response = client.PostAsync(endereco, contentData).Result;
                var apiContent = response.Content.ReadAsStringAsync().Result;
                var postCriado = JsonConvert.DeserializeObject<Postagem>(apiContent);
                TempData["RespostaAPI"] = $"Post criado";
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                TempData["RespostaAPI"] = "Não foi possível acessar a API!";
                return RedirectToAction("Index", postagem);

            }
        }

        private static string UploadImage(IFormFile imageFile)
        {

            var reader = imageFile.OpenReadStream();
            var cloundStorageAccount = CloudStorageAccount.Parse(@"DefaultEndpointsProtocol=https;AccountName=guism;AccountKey=8a9KnwP/5vusjh2xr3uvxt/4M2jIwkDBPpaPUpdlzaIHv0DRhud3RInTR6gbtutxhvb42CD9J8lk+AStfxaAAA==;EndpointSuffix=core.windows.net");
            var blobClient = cloundStorageAccount.CreateCloudBlobClient();
            var container = blobClient.GetContainerReference("posts-images");
            container.CreateIfNotExists();
            var blob = container.GetBlockBlobReference(Guid.NewGuid().ToString());
            //CloudBlockBlob blob = container.GetBlockBlobReference(content);
            Thread.Sleep(2000);
            blob.UploadFromStream(reader);

            var uri = blob.Uri.ToString();

            return uri;
        }

        // GET: Posts/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {

            var jwt = HttpContext.Session.GetString(SESSION_JWT);
            if (jwt == null)
            {
                HttpContext.Session.SetString(REDIRECIONAR_LOGADO, "Postagens");
                return RedirectToAction("Login", "Home");
            }
            var client = _httpClient.CreateClient("PegaPostagem");
            HttpRequestMessage requestMessage = new HttpRequestMessage();
            requestMessage.Headers.Add("Accept", "application/json");
            requestMessage.RequestUri = new Uri(_url + $"api/Postagens/{id}");
            requestMessage.Method = HttpMethod.Get;
            client.DefaultRequestHeaders.Clear();

            client.DefaultRequestHeaders.Authorization =new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwt);

            HttpResponseMessage apiResponse = null;
            apiResponse = client.SendAsync(requestMessage).Result;
            var apiContentRaw = apiResponse.Content.ReadAsStringAsync().Result;

            var postRecebido = JsonConvert.DeserializeObject<Postagem>(apiContentRaw);
            if (postRecebido != null)
            {
                return View(postRecebido);
            }
            else
            {
                TempData["RespostaAPI"] = "Não foi possível acessar a API!";
                return RedirectToAction("Index");
            }
        }

        // POST: Posts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,EmailUsuario,Texto,HoraPost,CaminhoImg")] Postagem postagem, IFormFile ImageFile)
        {
            var jwt = HttpContext.Session.GetString(SESSION_JWT);
            if (jwt == null)
            {
                HttpContext.Session.SetString(REDIRECIONAR_LOGADO, "Postagens");
                return RedirectToAction("Login", "Home");
            }

            if (id != postagem.Id)
            {
                return NotFound();
            }

            if (ImageFile != null)
            {
               postagem.CaminhoImg = UploadImage(ImageFile);
            }
            var postJson = JsonConvert.SerializeObject(postagem);

            try
            {
                var client = _httpClient.CreateClient("CreatePostagens");
                var contentData = new StringContent(postJson, Encoding.UTF8, "application/json");
                var endereco = _url + $"api/Postagens/{id}";
                client.DefaultRequestHeaders.Authorization =new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwt);
                HttpResponseMessage response = client.PutAsync(endereco, contentData).Result;

                var respostaApi = response.StatusCode;
                if(respostaApi == System.Net.HttpStatusCode.NoContent)
                {
                    TempData["RespostaAPI"] = $"O post foi editado com sucesso!";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception)
            {
                TempData["RespostaAPI"] = "Não foi possível acessar a API!";
                return RedirectToAction("Index");

            }          
            return View(postagem);
        }

        // GET: Posts/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            var jwt = HttpContext.Session.GetString(SESSION_JWT);
            if (jwt == null)
            {
                HttpContext.Session.SetString(REDIRECIONAR_LOGADO, "Postagens");
                return RedirectToAction("Login", "Home");
            }

            var client = _httpClient.CreateClient("PegaPostagem");
            HttpRequestMessage requestMessage = new HttpRequestMessage();

            requestMessage.Headers.Add("Accept", "application/json");
            requestMessage.RequestUri = new Uri(_url + $"api/Postagens/{id}");
            requestMessage.Method = HttpMethod.Get;
            client.DefaultRequestHeaders.Clear();

            client.DefaultRequestHeaders.Authorization =new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwt);
            HttpResponseMessage apiResponse = null;
            apiResponse = client.SendAsync(requestMessage).Result;
            var apiContentRaw = apiResponse.Content.ReadAsStringAsync().Result;
            var postRecebido = JsonConvert.DeserializeObject<Postagem>(apiContentRaw);

            if (postRecebido != null)
            {
                return View(postRecebido);
            }
            else
            {
                TempData["RespostaAPI"] = "Não foi possível acessar a API!";
                return RedirectToAction("Index");
            }

        }

        // POST: Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var jwt = HttpContext.Session.GetString(SESSION_JWT);
            if (jwt == null)
            {
                HttpContext.Session.SetString(REDIRECIONAR_LOGADO, "Postagens");
                return RedirectToAction("Login", "Home");
            }

            try
            {
                var client = _httpClient.CreateClient("DeletePostagens");
                var endereco = _url + $"api/Postagens/{id}";
                client.DefaultRequestHeaders.Authorization =new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwt);

                HttpResponseMessage response = client.DeleteAsync(endereco).Result;

                var respostaApi = response.StatusCode;
                TempData["RespostaAPI"] = response.ToString();
                if (respostaApi == System.Net.HttpStatusCode.NoContent)
                {
                    TempData["RespostaAPI"] = $"Post removido com sucesso!";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception)
            {
                TempData["RespostaAPI"] = "Não foi possível acessar a API!";
                return RedirectToAction("Index");
                throw;
            }
            return RedirectToAction("Index");
        }

    }
}
