using Alura.ListaLeitura.Modelos;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Lista = Alura.ListaLeitura.Modelos.ListaLeitura;

namespace Alura.ListaLeitura.HttpClients
{
    public class LivroApiClient
    {
        private readonly HttpClient _httpClient;

        public LivroApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<LivroApi> GetLivroAsync(int id)
        {
            var resposta = await _httpClient.GetAsync($"Livros/{id}");
            resposta.EnsureSuccessStatusCode();
            return await resposta.Content.ReadAsAsync<LivroApi>();
        }

        public async Task<byte[]> GetCapaLivroAsync(int id)
        {
            HttpResponseMessage resposta = await _httpClient.GetAsync($"Livros/{id}/capa");
            resposta.EnsureSuccessStatusCode();
            return await resposta.Content.ReadAsByteArrayAsync();
        }

        public async Task<Lista> GetListaLeituraAsync(TipoListaLeitura tipo)
        {
            var resposta = await _httpClient.GetAsync($"listasleitura/{tipo}");
            resposta.EnsureSuccessStatusCode();
            return await resposta.Content.ReadAsAsync<Lista>();
        }

        public async Task DeleteLivroAsync(int id)
        {
            HttpResponseMessage resposta = await _httpClient.DeleteAsync($"Livros/{id}");
            resposta.EnsureSuccessStatusCode();
        }

        private string EnvolveComAspasDuplas(string valor)
        {
            return $"\u0022{valor}\u0022";
        }

        private HttpContent CreateMultipartContent(Livro livro)
        {
            var content = new MultipartFormDataContent();

            content.Add(new StringContent(livro.Titulo), EnvolveComAspasDuplas("titulo"));
            content.Add(new StringContent(livro.Lista.ParaString()), EnvolveComAspasDuplas("lista"));

            if (livro.Id > 0)
                content.Add(new StringContent(Convert.ToString(livro.Id)), EnvolveComAspasDuplas("id"));

            if (!string.IsNullOrEmpty(livro.Subtitulo))
                content.Add(new StringContent(livro.Subtitulo), EnvolveComAspasDuplas("subtitulo"));

            if (!string.IsNullOrEmpty(livro.Resumo))
                content.Add(new StringContent(livro.Resumo), EnvolveComAspasDuplas("resumo"));

            if (!string.IsNullOrEmpty(livro.Autor))
                content.Add(new StringContent(livro.Autor), EnvolveComAspasDuplas("autor"));

            if (livro.ImagemCapa != null)
            {
                var imageContent = new ByteArrayContent(livro.ImagemCapa);
                imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/png");
                content.Add(imageContent, EnvolveComAspasDuplas("capa"), EnvolveComAspasDuplas("capa.png"));
            }

            return content;
        }

        public async Task PostLivroAsync(LivroUpload livro)
        {
            HttpContent content = CreateMultipartContent(livro.ToLivro());
            var resposta = await _httpClient.PostAsync("livros", content);
            resposta.EnsureSuccessStatusCode();
            if (resposta.StatusCode != System.Net.HttpStatusCode.Created)
            {
                throw new InvalidOperationException("Código de Status Http 201 esperado!");
            }
        }

        public async Task PutLivroAsync(LivroUpload livro)
        {
            HttpContent content = CreateMultipartContent(livro.ToLivro());
            var resposta = await _httpClient.PutAsync("livros", content);
            resposta.EnsureSuccessStatusCode();
            if (resposta.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new InvalidOperationException("Código de Status Http 200 esperado!");
            }

        }

    }
}
