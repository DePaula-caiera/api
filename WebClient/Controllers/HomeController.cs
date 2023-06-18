using Microsoft.AspNetCore.Mvc;
using Models;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Text.Json;
using WebClient.Models;

namespace WebClient.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;     
        private List<Cidade>? Cidades;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            using (var httpClient = new HttpClient())
            {               
                try
                {
                    using var resposta = await httpClient.GetAsync(
                        $"https://localhost:7075/paises/55/estados/11/cidades");
                    {
                        if (resposta.StatusCode == HttpStatusCode.OK)
                        {
                          var conteudo = resposta.Content.ReadAsStringAsync().Result;

                            if(!String.IsNullOrEmpty(conteudo)) 
                            {
                                Cidades = JsonSerializer.Deserialize<List<Cidade>>(conteudo, new JsonSerializerOptions
                                {
                                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                                });
                            }                              
                        }
                        else
                        {
                            ViewBag.ErroBusca = "Ocorreu um erro ao filtrar as cidades.";
                        }
                    }
                }
                catch (Exception)
                {
                    ViewBag.ErroBusca = "Ocorreu um erro ao filtrar as cidades.";
                }
            }
            return View(Cidades);
        }
        [HttpPost]
        public async Task <IActionResult> Index(int idCidade, string nome, int estado, int pais, int populacao)
        {
            Cidade cidade = new()
            {
                Id = idCidade,
                Nome = nome,
                Populacao = populacao
            };

            using (var httpClient = new HttpClient())
            {
                StringContent? conteudo = new(
                    JsonSerializer.Serialize(cidade)
                    , Encoding.UTF8, "application/json");

                try
                {
                    using var resposta = await httpClient.PostAsync(
                        $"https://localhost:7075/paises/{pais}/estados/{estado}/cidades",
                        conteudo);
                    {
                        if (resposta.StatusCode == HttpStatusCode.Created)
                        {
                            ViewBag.MensagemGravacao = "Gravado com sucesso.";
                            //Sucesso
                        }
                        else
                        {
                            ViewBag.MensagemGravacao = "Ocorreu um erro.";
                            //Erro
                        }
                    }
                }
                catch (Exception) 
                {
                    ViewBag.MensagemGravacao = "Ocorreu um erro.";
                }

            }
            return await Index();
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