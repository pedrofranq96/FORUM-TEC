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
    public class PerfilsController : Controller
    {
        private string _url = "https://localhost:7096/";

        private const string SESSION_JWT = "SessionJWT";
        private const string SESSION_EMAIL = "SessionEmail";
        private const string REDIRECIONAR_LOGADO = "RedirecinarLogado";
        private IHttpClientFactory _httpClient { get; set; }

        public PerfilsController(IHttpClientFactory httpClient)
        {
            _httpClient = httpClient;
        }

        // GET: Perfils
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var jwt = HttpContext.Session.GetString(SESSION_JWT);
            if (jwt == null)
            {
                HttpContext.Session.SetString(REDIRECIONAR_LOGADO, "Perfils");
                return RedirectToAction("Login", "Home");
            }
            var email = HttpContext.Session.GetString(SESSION_EMAIL);

         

            var client = _httpClient.CreateClient("PegaPerfis");


            HttpRequestMessage requestMessage = new HttpRequestMessage();



            requestMessage.Headers.Add("Accept", "application/json");

            requestMessage.RequestUri = new Uri(_url + $"api/Perfils/{email}");
            requestMessage.Method = HttpMethod.Get;
            client.DefaultRequestHeaders.Clear();

            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwt);

            HttpResponseMessage apiResponse = null;
            apiResponse = client.SendAsync(requestMessage).Result;
            if (apiResponse.IsSuccessStatusCode)
            {
                var apiContentRaw = apiResponse.Content.ReadAsStringAsync().Result;

                var perfil = JsonConvert.DeserializeObject<Perfil>(apiContentRaw);

                if (perfil != null)
                {
                    ViewBag.profile = true;
                    return View(perfil);
                }
            }

            ViewBag.profile = false;
            return View();

        }

        // GET: Perfils/Details/5
        public async Task<IActionResult> Details(string id)
        {
            var jwt = HttpContext.Session.GetString(SESSION_JWT);
            if (jwt == null)
            {
                HttpContext.Session.SetString(REDIRECIONAR_LOGADO, "Perfils");
                return RedirectToAction("Login", "Home");
            }

            var client = _httpClient.CreateClient("PegaPerfils");
            HttpRequestMessage requestMessage = new HttpRequestMessage();

            requestMessage.Headers.Add("Accept", "application/json");

            requestMessage.RequestUri = new Uri(_url + $"api/Perfils/{id}");
            requestMessage.Method = HttpMethod.Get;
            client.DefaultRequestHeaders.Clear();

            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwt);

            HttpResponseMessage apiResponse = null;
            apiResponse = client.SendAsync(requestMessage).Result;
            var apiContentRaw = apiResponse.Content.ReadAsStringAsync().Result;

            var perfilRecebido = JsonConvert.DeserializeObject<Perfil>(apiContentRaw);

            if (perfilRecebido != null)
            {
                return View(perfilRecebido);
            }
            else
            {
                TempData["RespostaAPI"] = "Não foi possível acessar a API!";
                return RedirectToAction("Index");
            }
        }

        // GET: Perfils/Create
        public IActionResult Create()
        {
            var jwt = HttpContext.Session.GetString(SESSION_JWT);
            if (jwt == null)
            {
                HttpContext.Session.SetString(REDIRECIONAR_LOGADO, "Perfils");
                return RedirectToAction("Login", "Home");
            }
            return View();
        }

        // POST: Perfils/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EmailUsuario,Texto,Aniversario,Criacao,CaminhoImg")] Perfil perfil, IFormFile ImageFile)
        {

            var jwt = HttpContext.Session.GetString(SESSION_JWT);
            if (jwt == null)
            {
                HttpContext.Session.SetString(REDIRECIONAR_LOGADO, "Perfils");
                return RedirectToAction("Login", "Home");
            }

            if (ModelState.IsValid)
            {
                perfil.EmailUsuario = HttpContext.Session.GetString(SESSION_EMAIL);
                perfil.Criacao = DateTime.Now;
                perfil.CaminhoImg = UploadImage(ImageFile);
            }
            var perfilJson = JsonConvert.SerializeObject(perfil);
            try
            {
                var client = _httpClient.CreateClient("CreatePerfil");
                var contentData = new StringContent(perfilJson, Encoding.UTF8, "application/json");
                var endereco = _url + "api/perfils";

                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwt);
                HttpResponseMessage response = client.PostAsync(endereco, contentData).Result;
                var apiContent = response.Content.ReadAsStringAsync().Result;
                var perfilCriado = JsonConvert.DeserializeObject<Perfil>(apiContent);
                TempData["RespostaAPI"] = $"Perfil criado";
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                TempData["RespostaAPI"] = "Não foi possível acessar a API!";
                return RedirectToAction("Index", perfil);

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

        // GET: Perfils/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            var jwt = HttpContext.Session.GetString(SESSION_JWT);
            if (jwt == null)
            {
                HttpContext.Session.SetString(REDIRECIONAR_LOGADO, "Perfils");
                return RedirectToAction("Login", "Home");
            }

            var client = _httpClient.CreateClient("PegaPerfils");
            HttpRequestMessage requestMessage = new HttpRequestMessage();

            requestMessage.Headers.Add("Accept", "application/json");

            requestMessage.RequestUri = new Uri(_url + $"api/Perfils/{id}");
            requestMessage.Method = HttpMethod.Get;
            client.DefaultRequestHeaders.Clear();

            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwt);

            HttpResponseMessage apiResponse = null;
            apiResponse = client.SendAsync(requestMessage).Result;
            var apiContentRaw = apiResponse.Content.ReadAsStringAsync().Result;
            var perfilRecebido = JsonConvert.DeserializeObject<Perfil>(apiContentRaw);

            if (perfilRecebido != null)
            {
                return View(perfilRecebido);
            }
            else
            {
                TempData["RespostaAPI"] = "Não foi possível acessar a API!";
                return RedirectToAction("Index");
            }
        }

        // POST: Perfils/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("EmailUsuario,Texto,Aniversario,CaminhoImg")] Perfil perfil, IFormFile ImageFile)
        {

            var jwt = HttpContext.Session.GetString(SESSION_JWT);
            if (jwt == null)
            {
                HttpContext.Session.SetString(REDIRECIONAR_LOGADO, "Perfils");
                return RedirectToAction("Login", "Home");
            }

            if (ModelState.IsValid)
            {
                perfil.CaminhoImg = UploadImage(ImageFile);
            }
            var perfilJson = JsonConvert.SerializeObject(perfil);

            try
            {
                var client = _httpClient.CreateClient("CreatePerfils");
                var contentData = new StringContent(perfilJson, Encoding.UTF8, "application/json");
                var endereco = _url + $"api/Perfils/{id}";

                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwt);
                HttpResponseMessage response = client.PutAsync(endereco, contentData).Result;

                var respostaApi = response.StatusCode;
                if (respostaApi == System.Net.HttpStatusCode.NoContent)
                {
                    TempData["RespostaAPI"] = $"O perfil foi editado com sucesso";
                    return RedirectToAction("Index");
                }

            }
            catch (Exception)
            {
                TempData["RespostaAPI"] = "Não foi possível acessar a API!";
                return RedirectToAction("Index");
            }

            return View(perfil);
        }


        // GET: Perfils/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            var jwt = HttpContext.Session.GetString(SESSION_JWT);
            if (jwt == null)
            {
                HttpContext.Session.SetString(REDIRECIONAR_LOGADO, "Perfils");
                return RedirectToAction("Login", "Home");
            }

            var client = _httpClient.CreateClient("PegaPerfil");
            HttpRequestMessage requestMessage = new HttpRequestMessage();
            requestMessage.Headers.Add("Accept", "application/json");
            requestMessage.RequestUri = new Uri(_url + $"api/Perfils/{id}");
            requestMessage.Method = HttpMethod.Get;
            client.DefaultRequestHeaders.Clear();

            client.DefaultRequestHeaders.Authorization =new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwt);

            HttpResponseMessage apiResponse = null;
            apiResponse = client.SendAsync(requestMessage).Result;
            var apiContentRaw = apiResponse.Content.ReadAsStringAsync().Result;
            var perfilRecebido = JsonConvert.DeserializeObject<Perfil>(apiContentRaw);
            
            if (perfilRecebido != null)
            {
                return View(perfilRecebido);
            }
            else
            {
                TempData["RespostaAPI"] = "Não foi possível acessar a API!";
                return RedirectToAction("Index");
            }
        }

        // POST: Perfils/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var jwt = HttpContext.Session.GetString(SESSION_JWT);
            if (jwt == null)
            {
                HttpContext.Session.SetString(REDIRECIONAR_LOGADO, "Perfils");
                return RedirectToAction("Login", "Perfils");
            }

            try
            {
                var client = _httpClient.CreateClient("DeletePerfil");
                var endereco = _url + $"api/Perfils/{id}";
                client.DefaultRequestHeaders.Authorization =new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwt);
                HttpResponseMessage response = client.DeleteAsync(endereco).Result;

                var respostaApi = response.StatusCode;
                TempData["RespostaAPI"] = response.ToString();
                if (respostaApi == System.Net.HttpStatusCode.NoContent)
                {

                    TempData["RespostaAPI"] = $"Perfil removido com sucesso";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception)
            {
                TempData["RespostaAPI"] = "Não foi possível acessar a API!";
                return RedirectToAction("Index");
            }
            
            return RedirectToAction(nameof(Index));
        }
    }
}
