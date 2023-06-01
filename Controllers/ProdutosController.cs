using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Teste_Frontend.Data;
using Teste_Frontend.Models;

using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Drawing;

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

        public async Task<IActionResult> GerarPDF()
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
                    return new EmptyResult();
                }

                Document doc = new Document(PageSize.A4, 10, 10, 10, 10);
                MemoryStream stream = new MemoryStream();
                PdfWriter writer = PdfWriter.GetInstance(doc, stream);

                doc.Open();

                BaseFont bfHeader = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, false);
                Font header = new(bfHeader, 16, Font.BOLD, BaseColor.BLACK);

                BaseFont bfLinha = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, false);
                Font Linha = new(bfLinha, 11, Font.NORMAL, BaseColor.BLACK);

                BaseFont bfNegrito = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, false);
                Font Negrito = new(bfNegrito, 11, Font.BOLD, BaseColor.BLACK);

                var paragrafo = new Paragraph("Relatório de Produtos - " + DateTime.Now, header);
                paragrafo.Alignment = Element.ALIGN_CENTER;

                doc.Add(paragrafo);
                doc.Add(new Paragraph(" "));

                PdfPTable table = new PdfPTable(5);
                table.TotalWidth = PageSize.A4.Width;
                float[] widths = new float[] { 10, 30, 30, 15, 15 };
                table.SetWidths(widths);
                table.HorizontalAlignment = 1;
                table.SpacingBefore = 20f;
                table.SpacingAfter = 30f;

                PdfPCell cell = new PdfPCell();
                cell.Colspan = 5;
                cell.Border = 0;
                cell.HorizontalAlignment = 1;

                PdfPCell _c = new PdfPCell();

                _c = new PdfPCell(new Phrase("Id", Negrito)) { VerticalAlignment = Element.ALIGN_MIDDLE, HorizontalAlignment = Element.ALIGN_CENTER };
                table.AddCell(_c);

                _c = new PdfPCell(new Phrase("Nome", Negrito)) { VerticalAlignment = Element.ALIGN_MIDDLE, HorizontalAlignment = Element.ALIGN_LEFT };
                table.AddCell(_c);

                _c = new PdfPCell(new Phrase("Descrição", Negrito)) { VerticalAlignment = Element.ALIGN_MIDDLE, HorizontalAlignment = Element.ALIGN_LEFT };
                table.AddCell(_c);

                _c = new PdfPCell(new Phrase("Preço", Negrito)) { VerticalAlignment = Element.ALIGN_MIDDLE, HorizontalAlignment = Element.ALIGN_RIGHT };
                table.AddCell(_c);

                _c = new PdfPCell(new Phrase("Estoque", Negrito)) { VerticalAlignment = Element.ALIGN_MIDDLE, HorizontalAlignment = Element.ALIGN_RIGHT };
                table.AddCell(_c);

                foreach (ProdutosViewModel produto in produtos)
                {
                    _c = new PdfPCell(new Phrase(produto.id.ToString(), Linha)) { VerticalAlignment = Element.ALIGN_MIDDLE, HorizontalAlignment = Element.ALIGN_CENTER };
                    table.AddCell(_c);

                    _c = new PdfPCell(new Phrase(produto.Nome, Linha)) { VerticalAlignment = Element.ALIGN_MIDDLE, HorizontalAlignment = Element.ALIGN_LEFT };
                    table.AddCell(_c);

                    _c = new PdfPCell(new Phrase(produto.Descricao, Linha)) { VerticalAlignment = Element.ALIGN_MIDDLE, HorizontalAlignment = Element.ALIGN_LEFT };
                    table.AddCell(_c);

                    _c = new PdfPCell(new Phrase(produto.Preco.ToString(), Linha)) { VerticalAlignment = Element.ALIGN_MIDDLE, HorizontalAlignment = Element.ALIGN_RIGHT };
                    table.AddCell(_c);

                    _c = new PdfPCell(new Phrase(produto.Estoque.ToString(), Linha)) { VerticalAlignment = Element.ALIGN_MIDDLE, HorizontalAlignment = Element.ALIGN_RIGHT };
                    table.AddCell(_c);
                }

                doc.Add(table);
                doc.Close();

                HttpContext.Response.ContentType = "application/pdf";
                HttpContext.Response.Headers.Add("content-disposition", "attachment;filename=Teste_Anselmo.pdf");
                HttpContext.Response.Body.WriteAsync(stream.GetBuffer(), 0, stream.GetBuffer().Length);

                return new EmptyResult();
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
                string url = $"{ENDPOINT}excluir?id={id}";

                HttpResponseMessage response = await httpClient.DeleteAsync(url);
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
