using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Teste_Frontend.Data;
using Teste_Frontend.Models;

namespace Teste_Frontend.Controllers
{
    public class ProdutosController : Controller
    {
        #region Propriedades
        private readonly Teste_FrontendContext _context;
        private readonly string ENDPOINT = "http://localhost:63960/v1/Produtos/";
        private readonly HttpClient httpClient = null;
        #endregion

        #region Construtores
        public ProdutosController(Teste_FrontendContext context)
        {
            _context = context;
            httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(ENDPOINT);
        }
        #endregion

        #region Actions
        public async Task<IActionResult> Index()
        {
            try
            {
                List<ProdutosViewModel> produtos = null;
                HttpResponseMessage response = await httpClient.GetAsync(ENDPOINT);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    produtos = JsonConvert.DeserializeObject<List<ProdutosViewModel>>(content);
                }
                else
                {
                    ModelState.AddModelError(null, "Erro ao processar a solicitação!");
                }
                return View(produtos);
            }
            catch (Exception ex)
            {
                string mensagem = ex.Message;
                throw ex;
            }
        }

        public async Task<IActionResult> Get(int id)
        {
            try
            {
                ProdutosViewModel result = await Pesquisar(id);
                return View(result);
            }
            catch (Exception ex)
            {
                string mensagem = ex.Message;
                throw ex;
            }
        }
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                ProdutosViewModel result = await Pesquisar(id);
                return View(result);
            }
            catch (Exception ex)
            {
                string mensagem = ex.Message;
                throw ex;
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete([Bind("id,Nome,Descricao,Preco,Estoque")] ProdutosViewModel produtos)
        {
            try
            {
                int id = produtos.id;

                //string json = JsonConvert.SerializeObject(produtos);
                //byte[] buffer = Encoding.UTF8.GetBytes(json);
                //ByteArrayContent bytecontent = new ByteArrayContent(buffer);
                //bytecontent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                                
                //string url = ENDPOINT + "excluir";
                string url = $"{ENDPOINT}alterar/id:int?id={id}";

                HttpResponseMessage response = await httpClient.DeleteAsync(url);
                //if (!response.IsSuccessStatusCode)
                //    ModelState.AddModelError(null, "Erro ao processar a solicitação");

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                string mensagem = ex.Message;
                throw ex;
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                ProdutosViewModel result = await Pesquisar(id);
                return View(result);
            }
            catch (Exception ex)
            {
                string mensagem = ex.Message;
                throw ex;
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit([Bind("id,Nome,Descricao,Preco,Estoque")] ProdutosViewModel produtos)
        {
            try
            {
                string json = JsonConvert.SerializeObject(produtos);
                byte[] buffer = Encoding.UTF8.GetBytes(json);
                ByteArrayContent bytecontent = new ByteArrayContent(buffer);
                bytecontent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                string url = ENDPOINT + "alterar";
                HttpResponseMessage response = await httpClient.PutAsync(url, bytecontent);
                if (!response.IsSuccessStatusCode)

                    ModelState.AddModelError(null, "Erro ao processar a solicitação");
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                string mensagem = ex.Message;
                throw ex;
            }
        }

        public IActionResult Create()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                string mensagem = ex.Message;
                throw ex;
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([Bind("Nome,Descricao,Preco,Estoque")] ProdutosViewModel produtos)
        {
            try
            {
                string json = JsonConvert.SerializeObject(produtos);
                byte[] buffer = Encoding.UTF8.GetBytes(json);
                ByteArrayContent bytecontent = new ByteArrayContent(buffer);
                bytecontent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                string url = ENDPOINT;
                HttpResponseMessage response = await httpClient.PostAsync(url, bytecontent);
                if (!response.IsSuccessStatusCode)

                    ModelState.AddModelError(null, "Erro ao processar a solicitação");
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                string mensagem = ex.Message;
                throw ex;
            }
        }

        #endregion

        #region Métodos Auxiliares
        private async Task<ProdutosViewModel> Pesquisar(int id)
        {
            try
            {
                ProdutosViewModel result = new ProdutosViewModel();
                string url = $"{ENDPOINT}id:int?id={id}";
                HttpResponseMessage response = await httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    ProdutosViewModel[] produtos = JsonConvert.DeserializeObject<ProdutosViewModel[]>(content);
                    result.id = produtos[0].id;
                    result.Nome = produtos[0].Nome;
                    result.Descricao = produtos[0].Descricao;
                    result.Preco = produtos[0].Preco;
                    result.Estoque = produtos[0].Estoque;
                }
                else
                {
                    ModelState.AddModelError(null, "Erro ao processar a solicitação!");
                }

                return result;
            }
            catch (Exception ex)
            {
                string mensagem = ex.Message;
                throw ex;
            }
        }
        #endregion
    }
}
