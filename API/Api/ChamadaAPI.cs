using Coinbase;
using Coinbase.ObjectModel;
using Entidades.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Script.Serialization;
using Util.Aplicacao;

namespace API.Entities
{
    public static class ChamadaAPI
    {
        public static StreamWriter SW { get; set; }
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
                    {
                        do
                        {
                            response = api.SendRequest($"/accounts?&limit=100", null, RestSharp.Method.GET);
                        } while (response.Data == null);
                    }
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
                    resultGetAddress = api.SendRequest($"accounts/{idCarteira}/addresses", null, RestSharp.Method.GET);
                } while (resultGetAddress.Data == null);
                return JsonConvert.DeserializeObject<List<Address>>(resultGetAddress.Data.ToString());
            }
            return new List<Address>();
        }

        public static void ListarEnderecosPorListaCarteira(List<string> listaCarteiras, string caminhoArquivo)
        {
            try
            {
                EnderecoArquivo = caminhoArquivo;
                TratarDiretorio();
                List<Address> listaEnderecos;
                if (listaCarteiras.Count > 0)
                {
                    foreach (string idCarteira in listaCarteiras)
                    {
                        if (!String.IsNullOrEmpty(idCarteira))
                        {
                            CoinbaseResponse resultGetAddress = new CoinbaseResponse();
                            CoinbaseApi api = new CoinbaseApi(ApiKey, ApiSecret, Aplicacao.URLAPI);
                            do
                            {
                                resultGetAddress = api.SendRequest($"accounts/{idCarteira}/addresses", null, RestSharp.Method.GET);
                            } while (resultGetAddress.Data == null);
                            listaEnderecos = JsonConvert.DeserializeObject<List<Address>>(resultGetAddress.Data.ToString());
                            if(listaEnderecos != null && listaEnderecos.Count > 0)
                                SW.WriteLine(listaEnderecos.FirstOrDefault().address);
                        }
                    }
                }
                ChamadaAPI.fecharArquivo();
            }
            catch (Exception e)
            {
                throw e;
            }
        }


        public static void TratarDiretorio()
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
        }

        public static bool TratarDiretorioRetorno(string diretorio)
        {
            string arquivo = EnderecoArquivo.Contains(".txt") ? EnderecoArquivo : EnderecoArquivo + ".txt";
            DirectoryInfo directoryArq = new DirectoryInfo(arquivo);
            if (new DirectoryInfo(directoryArq.FullName.Replace(directoryArq.Name, "")).Exists)
            {
                return true;
            }
            return false;
        }

        public static void IniciarMineracao(string nomeCarteira, int quantidadeCarteira)
        {
            try
            {
                if (!String.IsNullOrEmpty(ApiKey) && !String.IsNullOrEmpty(ApiSecret))
                {
                    TratarDiretorio();

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
                                resultCreateAccount = api.SendRequest($"accounts", opcoes, RestSharp.Method.POST);
                            }
                            while (resultCreateAccount.Data == null);
                            if (resultCreateAccount.Data != null)
                            {
                                carteira = JsonConvert.DeserializeObject<Wallet>(resultCreateAccount.Data.ToString());
                            }
                            do
                            {
                                resultGetAddress = api.SendRequest($"accounts/{carteira.id}/addresses", opcoes, RestSharp.Method.POST);
                            } while (resultGetAddress.Data == null);

                            carteira.Addresses = new List<Address>();
                            if (resultGetAddress.Data != null)
                            {
                                carteira.Addresses.Add(JsonConvert.DeserializeObject<Address>(resultGetAddress.Data.ToString()));
                            }
                            CriarOuLogarContaBitMinerAsync(carteira.Addresses.FirstOrDefault().address);
                            SW.WriteLine(carteira.Addresses.FirstOrDefault() + ";" + carteira.id + ";");
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

        private static async void CriarOuLogarContaBitMinerAsync(string enderecoCarteira)
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

        private static async void RetirarMinimoBitMinerAsync(string enderecoCarteira)
        {
            try
            {
                string responseString = String.Empty;
                using (var client = new HttpClient())
                {
                    var values = new Dictionary<string, string>
                {
                    { "task", "withdraw" },
                    { "amount", "0.005" }
                };

                    var content = new FormUrlEncodedContent(values);

                    var response = await client.PostAsync("https://bitminer.io/", content);

                    responseString = await response.Content.ReadAsStringAsync();
                }
                SW.WriteLine(enderecoCarteira + ":" + responseString == "1" ? "sucesso" : "falha");
            }
            catch (Exception ex)
            {
                SW.Close();
                throw ex;
            }
        }

        public static List<Address> CriarEnderecosCarteiraBitMinerELogar(string idCarteira, string enderecoArquivo, int quantidadeEnderecos)
        {
            List<Address> listaRetorno = new List<Address>();
            Address endereco;
            EnderecoArquivo = enderecoArquivo;
            TratarDiretorio();
            CoinbaseResponse resultCreateAddress = new CoinbaseResponse();
            var api = new CoinbaseApi(ApiKey, ApiSecret, Aplicacao.URLAPI);
            for (int i = 0; i < quantidadeEnderecos; i++)
            {
                do
                {
                    var opcoes = new
                    {
                        name = "Endereco" + i
                    };
                resultCreateAddress = api.SendRequest($"accounts/{idCarteira}/addresses", opcoes, RestSharp.Method.POST);
                } while (resultCreateAddress.Data == null);
                 endereco = JsonConvert.DeserializeObject<Address>(resultCreateAddress.Data.ToString());
                listaRetorno.Add(endereco);
                CriarOuLogarContaBitMinerSync(endereco.address);
                SW.WriteLine(endereco.address);
            }
            fecharArquivo();
            return listaRetorno;
        }


        public static void CriarOuLogarContaBitMinerSync(string enderecoCarteira)
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

                var responseString = response.Content.ReadAsStringAsync().Result;
            }
        }

        public static void RetirarMinimoBitMinerSync(string enderecoCarteira)
        {
            try
            {
                string responseString = String.Empty;

                var baseAddress = new Uri("https://bitminer.io");
                var cookieContainer = new CookieContainer();
                using (var handler = new HttpClientHandler()
                {
                    CookieContainer = cookieContainer,
                    UseCookies = true,
                    AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
                })

                using (var client = new HttpClient(handler))
                {
                    client.BaseAddress = baseAddress;
                    cookieContainer.Add(baseAddress, new Cookie("addr", enderecoCarteira));

                    addHeadersBitMinerByClient(client);

                    var values = new Dictionary<string, string>
                    {
                        { "task", "withdraw" },
                        { "amount", "0.005" }
                    };
                    var content = new FormUrlEncodedContent(values);

                    var response = client.PostAsync("/", content).Result;

                    responseString = response.Content.ReadAsStringAsync().Result;
                }
                SW.WriteLine(enderecoCarteira + ":" + (responseString == "1" ? "sucesso" : "falha"));
            }
            catch (Exception ex)
            {
                SW.Close();
                throw ex;
            }
        }

        public static void fecharArquivo()
        {
            SW.Close();
        }

        public static String GetResponseBitMinerByAddress(string address)
        {
            var baseAddress = new Uri("https://bitminer.io");
            var cookieContainer = new CookieContainer();
            using (var handler = new HttpClientHandler()
            {
                CookieContainer = cookieContainer,
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

        public static String GetResponsePayouts()
        {
            var baseAddress = new Uri("https://bitminer.io/payouts");
            using (var handler = new HttpClientHandler()
            {
                CookieContainer = new CookieContainer(),
                UseCookies = true,
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            })

            using (var client = new HttpClient(handler))
            {
                client.BaseAddress = baseAddress;

                addHeadersBitMinerByClient(client);

                var response = client.GetAsync("/").Result;

                String responseString = response.Content.ReadAsStringAsync().Result;

                return responseString;
            }
        }

        public static String PostResponsePayouts(int offSet)
        {
            var baseAddress = new Uri("https://bitminer.io/payouts");
            var values = new Dictionary<string, string>
                    {
                        { "task", "morepayouts" },
                        { "offset", offSet.ToString() }
                    };
            var content = new FormUrlEncodedContent(values);
            using (var handler = new HttpClientHandler()
            {
                CookieContainer = new CookieContainer(),
                UseCookies = true,
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            })

            using (var client = new HttpClient(handler))
            {
                client.BaseAddress = baseAddress;

                addHeadersBitMinerByClient(client);

                var response = client.PostAsync("/", content).Result;

                String responseString = response.Content.ReadAsStringAsync().Result;

                return responseString;
            }
        }

        public static void TransferirFundosContas(string nomeContaPrincipal, string caminhoArquivo)
        {
            try
            {
                EnderecoArquivo = caminhoArquivo;
                TratarDiretorio();
                List<Wallet> listaContas = ListarCarteiras();
                Wallet carteiraDestino = listaContas.Where(w => w.name == nomeContaPrincipal).FirstOrDefault();
                if (carteiraDestino != null)
                {
                    listaContas.Remove(carteiraDestino);
                    foreach (Wallet w in listaContas)
                    {
                        TransferirFundos(w, carteiraDestino, w.balance.amount);
                    }
                }
                else
                {
                    SW.WriteLine("Carteira principal não encontrada");
                }
                fecharArquivo();
            }
            catch(Exception e)
            {
                fecharArquivo();
                throw e;
            }
        }

        private static void TransferirFundos(Wallet carteiraSaida, Wallet carteiraDestino, double fundos)
        {
            try
            {
                if (carteiraSaida.id != carteiraDestino.id)
                {
                    CoinbaseResponse transactionResponse = new CoinbaseResponse();
                    var api = new CoinbaseApi(ApiKey, ApiSecret, Aplicacao.URLAPI);

                    var opcoes = new
                    {
                        type = "transfer",
                        to = carteiraDestino.id,
                        amount = fundos.ToString(),
                        currency = "BTC",
                        description = ""
                    };
                    
                    do
                    {
                        transactionResponse = api.SendRequest($"accounts/{carteiraSaida.id}/transactions", opcoes, RestSharp.Method.POST);
                    } while (transactionResponse.Data == null);
                    Transaction transacao = JsonConvert.DeserializeObject<Transaction>(transactionResponse.Data.ToString());
                    SW.WriteLine("Transação de "+ fundos+ " BTC da carteira " + carteiraSaida.name 
                        +" para a carteira "+ carteiraDestino.name +" gerou o status : " + transacao.status);
                }
                else
                {
                    SW.WriteLine("{Carteira de destino}");
                }
            }
            catch(Exception e)
            {
                fecharArquivo();
                throw e;
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
