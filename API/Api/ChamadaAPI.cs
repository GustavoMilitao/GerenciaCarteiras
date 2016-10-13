using Coinbase;
using Coinbase.ObjectModel;
using CoinBaseMinering.Entities;
using CoinBaseMinering.Util;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace ListarCarteirasBitMiner.Entities
{
    public static class ChamadaAPI
    {
        private static StreamWriter SW { get; set; }
        public static string EnderecoArquivo { get; set; }
        public static string ApiKey { get; set; }
        public static string ApiSecret { get; set; }

        public static List<Wallet> ListarCarteiras()
        {
            if (!String.IsNullOrEmpty(ApiKey) && !String.IsNullOrEmpty(ApiSecret))
            {
                CoinbaseApi api = new CoinbaseApi(ApiKey, ApiSecret, Aplicacao.URLAPI);
                CoinbaseResponse response = new CoinbaseResponse();
                List<Wallet> listaRetorno = new List<Wallet>();
                do
                {
                    if (response.Pagination == null)
                        do
                        {
                            response = api.SendRequest("accounts?&limit=100", null, RestSharp.Method.GET);
                        } while (response.Data == null);
                    else if (response != null)
                    {
                        do
                        {
                            response = api.SendRequest(response.Pagination.NextUri.Replace("/v2/", "") + "&limit=100", null, RestSharp.Method.GET);
                        }
                        while (response.Data == null);
                    }
                    listaRetorno.AddRange(JsonConvert.DeserializeObject<List<Wallet>>(response.Data.ToString()));
                }
                while (!String.IsNullOrEmpty(response.Pagination.NextUri));
                //response = new CoinbaseResponse();
                //foreach (Wallet wallet in listaRetorno)
                //{
                //    response = new CoinbaseResponse();
                //    wallet.Addresses = new List<Address>();
                //    do
                //    {
                //        if (response.Pagination == null)
                //            response = api.SendRequest($"accounts/{wallet.id}/addresses?&limit=100", null, RestSharp.Method.GET);
                //        else if (response != null)
                //            response = api.SendRequest(response.Pagination.NextUri.Replace("/v2/", "") + "&limit=100", null, RestSharp.Method.GET);
                //        wallet.Addresses.AddRange(JsonConvert.DeserializeObject<List<Address>>(response.Data.ToString()));
                //    } while (!String.IsNullOrEmpty(response.Pagination.NextUri));
                //}
                return listaRetorno;
            }
            else
            {
                throw new Exception("Chave da API inválida ou não adicionada.");
            }
        }

        public static List<Address> ListarEnderecosPorIDCarteira(string idCarteira)
        {
            if (!String.IsNullOrEmpty(idCarteira))
            {
                CoinbaseResponse resultGetAddress = new CoinbaseResponse();
                CoinbaseApi api = new CoinbaseApi(ApiKey, ApiSecret, Aplicacao.URLAPI);
                do
                {
                    resultGetAddress = api.SendRequest($"/accounts/{idCarteira}/addresses", null, RestSharp.Method.GET);
                } while (resultGetAddress.Data == null);
                return JsonConvert.DeserializeObject<List<Address>>(resultGetAddress.Data.ToString());
            }
            return new List<Address>();
        }

        public static void IniciarMineracao(string nomeCarteira, int quantidadeCarteira)
        {
            try
            {
                if (!String.IsNullOrEmpty(ApiKey) && !String.IsNullOrEmpty(ApiSecret))
                {
                    string arquivo = EnderecoArquivo.Contains(".txt") ? EnderecoArquivo : EnderecoArquivo + ".txt";
                    DirectoryInfo directoryArq = new DirectoryInfo(arquivo);
                    if (new DirectoryInfo(directoryArq.FullName.Replace(directoryArq.Name, "")).Exists)
                    {
                        SW = new StreamWriter(arquivo);
                    }
                    else
                    {
                        throw new Exception("Diretório incorreto");
                    }

                    if (quantidadeCarteira > 0)
                    {
                        var api = new CoinbaseApi(ApiKey, ApiSecret, Aplicacao.URLAPI);
                        if (nomeCarteira == null)
                            nomeCarteira = String.Empty;
                        var jsdesserializador = new JavaScriptSerializer();
                        CoinbaseResponse resultCreateAccount = new CoinbaseResponse();
                        CoinbaseResponse resultGetAddress = new CoinbaseResponse();
                        Wallet carteira = new Wallet();
                        for (int i = 0; i < quantidadeCarteira; i++)
                        {
                            var opcoes = new
                            {
                                name = nomeCarteira + i
                            };
                            do
                            {
                                resultCreateAccount = api.SendRequest($"/accounts", opcoes, RestSharp.Method.POST);
                            }
                            while (resultCreateAccount.Data == null);
                            if (resultCreateAccount.Data != null)
                            {
                                 carteira = JsonConvert.DeserializeObject<Wallet>(resultCreateAccount.Data.ToString());
                            }
                            do
                            {
                                 resultGetAddress = api.SendRequest($"/accounts/{carteira.id}/addresses", opcoes, RestSharp.Method.POST);
                            } while (resultGetAddress.Data == null);

                            carteira.Addresses = new List<Address>();
                            if (resultGetAddress.Data != null)
                            {
                                carteira.Addresses.Add(JsonConvert.DeserializeObject<Address>(resultGetAddress.Data.ToString()));
                            }
                            CriarContaBitMiner(carteira.Addresses.FirstOrDefault().address);
                            SW.WriteLine(carteira.Addresses.FirstOrDefault() + ";" + carteira.id+";");
                        }
                        SW.Close();
                    }
                }
                else
                {
                    throw new Exception("Chave da API inválida ou não adicionada.");
                }
            }
            catch
            {
                throw new Exception("Diretório incorreto");
            }
        }

        private static async void CriarContaBitMiner(string enderecoCarteira)
        {
            using (var client = new HttpClient())
            {
                var values = new Dictionary<string, string>
                {
                    { "task", "sign" },
                    { "addr", enderecoCarteira }
                };

                var content = new FormUrlEncodedContent(values);

                var response = await client.PostAsync("https://bitminer.io/", content);

                var responseString = await response.Content.ReadAsStringAsync();
            }
        }

        public static void LogarBitMiner(string enderecoCarteira)
        {
            using (var client = new HttpClient())
            {
                var values = new Dictionary<string, string>
                {
                    { "task", "sign" },
                    { "addr", enderecoCarteira }
                };

                var content = new FormUrlEncodedContent(values);

                var response = client.PostAsync("https://bitminer.io/", content).Result;
            }
        }

        public static String GetResponseLoggedInBitminer(string address)
        {
            var baseAddress = new Uri("https://bitminer.io");
            var cookieContainer = new CookieContainer();
            using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer,
            UseCookies = true,
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            })

            using (var client = new HttpClient(handler))
            {
                client.BaseAddress = baseAddress;
                cookieContainer.Add(baseAddress, new Cookie("addr", address));

                addHeadersBitMinerByClient(client);

                var response = client.GetAsync("/").Result;

                String responseString = response.Content.ReadAsStringAsync().Result;

                return responseString;
            }
        }

        private static void addHeadersBitMinerByClient(HttpClient client)
        {
            client.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(
                    "text/html"
                    ));

            client.DefaultRequestHeaders.Accept.Add(
        new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(
            "application/xhtml+xml"
            ));

            client.DefaultRequestHeaders.Accept.Add(
            new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(
                "application/xml"
                ));

            client.DefaultRequestHeaders.Accept.Add(
            new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(
                "image/webp"
                ));

            client.DefaultRequestHeaders.Accept.Add(
            new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(
                "*/*"
                ));

            client.DefaultRequestHeaders.AcceptEncoding.Add(
                new System.Net.Http.Headers.StringWithQualityHeaderValue("gzip")
                );

            client.DefaultRequestHeaders.AcceptEncoding.Add(
                new System.Net.Http.Headers.StringWithQualityHeaderValue("deflate")
                );

            client.DefaultRequestHeaders.AcceptEncoding.Add(
                new System.Net.Http.Headers.StringWithQualityHeaderValue("sdch")
                );

            client.DefaultRequestHeaders.AcceptEncoding.Add(
                new System.Net.Http.Headers.StringWithQualityHeaderValue("br")
                );

            client.DefaultRequestHeaders.AcceptLanguage.Add(
                new System.Net.Http.Headers.StringWithQualityHeaderValue("pt-BR")
                );

            client.DefaultRequestHeaders.AcceptLanguage.Add(
                new System.Net.Http.Headers.StringWithQualityHeaderValue("en-US")
                );

            client.DefaultRequestHeaders.AcceptLanguage.Add(
                new System.Net.Http.Headers.StringWithQualityHeaderValue("en")
                );

            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/53.0.2785.143 Safari/537.36");

            client.DefaultRequestHeaders.Connection.Add("keep-alive");

            client.DefaultRequestHeaders.Host = "bitminer.io";

            client.DefaultRequestHeaders.TransferEncodingChunked = null;
        }
    }
}
