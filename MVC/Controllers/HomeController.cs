using Microsoft.AspNetCore.Mvc;
using MVC.Models;
using MVC.Models.ViewModels;
using NEGOCIO.Models;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Text;

namespace MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHttpClientFactory _httpClient;

        private readonly ILogger<HomeController> _logger;

        private string _url = "https://localhost:7096/";

        private const string SESSION_JWT = "SessionJWT";
        private const string SESSION_EMAIL = "SessionEmail";
        private const string REDIRECIONAR_LOGADO = "RedirecinarLogado";

        public HomeController(ILogger<HomeController> logger,
            IHttpClientFactory httpClient)
        {
            _logger = logger;
            _httpClient = httpClient;
        }


        [HttpGet]
        public IActionResult Index()
        {
            var jwt = HttpContext.Session.GetString(SESSION_JWT);
            if (jwt != null)
            {
                TempData["Usuario"] = $"Você está logado como {HttpContext.Session.GetString(SESSION_EMAIL)}";
                TempData["JWT"] = $" Seu JWT: {HttpContext.Session.GetString(SESSION_JWT)}";
            }
            else
            {
                HttpContext.Session.SetString(REDIRECIONAR_LOGADO, "Postagens");
                return RedirectToAction("Login");
            }


            return View();
        }

        [HttpGet]
        public IActionResult Cadastro()
        {
            return View();
        }



        [HttpPost]
        public IActionResult Cadastro(CadastroViewModel viewModel)
        {

            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Dados Inválidos";
                return View();
            }

            var usuario = new Usuario
            {
                Email = viewModel.Email,
                Nome = viewModel.Nome,
                Senha = viewModel.Senha
            };

            try
            {
                var client = _httpClient.CreateClient("Cadastro");
                string stringData = JsonConvert.SerializeObject(usuario);
                var contentData = new StringContent(stringData, Encoding.UTF8, "application/json");
                var endereco = _url + "api/usuarios";

                HttpResponseMessage response = client.PostAsync(endereco, contentData).Result;
                var apiContent = response.Content.ReadAsStringAsync().Result;

                TempData["Cadastro"] = "Cadastro realizado com sucesso!";
            }
            catch (Exception ex) { }

            return View(viewModel);
        }






        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }


        [HttpPost]
        public IActionResult Login(LoginViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Dados Inválidos";
                return View();
            }

            var usuario = new Usuario
            {
                Email = viewModel.Email,
                //Nome = viewModel.Nome,
                Nome = "",
                Senha = viewModel.Senha
            };

            try
            {
                //var client = _httpClient.CreateClient("Cadastro");
                var client = _httpClient.CreateClient("Login");
                string stringData = JsonConvert.SerializeObject(usuario);
                var contentData = new StringContent(stringData, Encoding.UTF8, "application/json");
                // var endereco = _url + "api/usuarios";
                var endereco = _url + "api/usuarios/token";

                HttpResponseMessage response = client.PostAsync(endereco, contentData).Result;
                var apiContent = response.Content.ReadAsStringAsync().Result;

                if (apiContent != null)
                {

                    switch (apiContent)
                    {
                        case "Usuário não existe":
                            TempData["token"] = apiContent;
                            break;
                        case "Senha inválida":
                            TempData["token"] = apiContent;
                            break;
                        default:
                            TempData["token"] = apiContent;
                            HttpContext.Session.SetString(SESSION_JWT, apiContent);
                            HttpContext.Session.SetString(SESSION_EMAIL, viewModel.Email);

                            var controlador = HttpContext.Session.GetString(REDIRECIONAR_LOGADO);
                            if (controlador != null)
                            {
                                return RedirectToAction("Index", controlador);
                            }
                            return RedirectToAction("Index");
                            break;
                    }

                }

            }
            catch (Exception ex) { }

            return View(viewModel);
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}