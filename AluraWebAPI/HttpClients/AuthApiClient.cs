using Alura.ListaLeitura.Seguranca;
using System.Net.Http;
using System.Threading.Tasks;

namespace Alura.ListaLeitura.HttpClients
{
    public class AuthApiClient
    {

        private readonly HttpClient _httpClient;

        public AuthApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;

        }

        public async Task<LoginResult> PostLoginAsync(LoginModel loginModel)
        {
            var resposta = await _httpClient.PostAsJsonAsync("login", loginModel);
            return new LoginResult
            {
                Succeeded = resposta.IsSuccessStatusCode,
                Token = await resposta.Content.ReadAsStringAsync()
            };

        }

    }

    public class LoginResult
    {
        public bool Succeeded { get; set; }
        public string Token { get; set; }
    }
}
