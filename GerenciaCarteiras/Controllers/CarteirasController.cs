using Coinbase.ObjectModel;
using Entidades.Entities;
using GerenciaCarteiras.Models;
using API.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace GerenciaCarteiras.Controllers
{
    public class CarteirasController : Controller
    {
        public ActionResult ListaCarteiras(string apiKey, string apiSecret)
        {
            ListaCarteirasModel model = new ListaCarteirasModel();
            ChamadaAPI.ApiKey = apiKey;
            ChamadaAPI.ApiSecret = apiSecret;

            List<Wallet> walletList = ChamadaAPI.ListarCarteiras();
            //List<Wallet> walletList = new List<Wallet>() {
            //    new Wallet() { id = "teste", name = "teste", balance = new Balance(), native_balance = new Balance() },
            //    new Wallet() { id = "teste", name = "teste", balance = new Balance(), native_balance = new Balance() }
            //    };
            model.Carteiras = walletList;

            return View("~/Views/Default/Default.cshtml", model);
        }

        public ActionResult Payouts()
        {
            return View("~/Views/Carteiras/Payouts.cshtml");
        }

        public JsonResult GetResponsePayouts(int offSet)
        {
            List<string> paginas = new List<string>();
            List<string> listaniveis = new List<string>();
            string response = String.Empty;
            do
            {
                response = ChamadaAPI.PostResponsePayouts(offSet);
                paginas.Add(response);
                offSet += 100;
            } while (!String.IsNullOrEmpty(response));
            string nivel = String.Empty;
            int beforeLevel = 0;
            string[] linhas;
            ChamadaAPI.EnderecoArquivo = @"C:\LOGS\payouts.txt";
            ChamadaAPI.TratarDiretorio();
            foreach (string r in paginas)
            {
                linhas = r.Split(new string[]{ "<td>" }, StringSplitOptions.RemoveEmptyEntries);
                foreach(string s in linhas)
                {
                    ChamadaAPI.SW.WriteLine(s);
                }
                //beforeLevel = r.IndexOf("BitMiner 1.");
                //nivel = r.Substring(beforeLevel, beforeLevel + 12);
                //listaniveis.Add(nivel);
            }
            ChamadaAPI.fecharArquivo();
            //return Json(new { sucesso = true, paginacao = new JavaScriptSerializer().Serialize(paginas) });
            return null;
        }

        public ActionResult TratarPayouts(string endereco)
        {
            return Json(new { sucesso = true, response = ChamadaAPI.GetResponseBitMinerByAddress(endereco) });
        }

        public JsonResult SalvarArquivo(string[] listaEnderecosComLevel, string caminhoArq)
        {
            try
            {
                ChamadaAPI.EnderecoArquivo = caminhoArq;
                ChamadaAPI.TratarDiretorio();
                foreach(string endereco in listaEnderecosComLevel)
                {
                    ChamadaAPI.SW.WriteLine(endereco);
                }
                ChamadaAPI.fecharArquivo();
                return Json(new { sucesso = true });
            }
            catch(Exception ex)
            {
                return Json(new { sucesso = false, mensagem = ex.Message });
            }
        }

        public JsonResult ListarEnderecos(string accountId)
        {
            return Json(new { listaEnderecos = ChamadaAPI.ListarEnderecosPorIDCarteira(accountId) });
        }

        public JsonResult AddressResponseBitMiner(string address)
        {
            try
            {
                return Json(new { sucesso = true, response = ChamadaAPI.GetResponseBitMinerByAddress(address) });
            }
            catch (Exception ex)
            {
                return Json(new { sucesso = false, mensagem = ex.Message });
            }
        }


        public ActionResult Menu(string apiKey, string apiSecret)
        {
            ListaCarteirasModel model = new ListaCarteirasModel();
            ChamadaAPI.ApiKey = apiKey;
            ChamadaAPI.ApiSecret = apiSecret;

            List<Wallet> walletList = ChamadaAPI.ListarCarteiras();
            model.Carteiras = walletList;
            return View("~/Views/Carteiras/Menu.cshtml", model);
        }

        public JsonResult SalvarCarteirasArqTexto(string listaCarteiras, string caminhoArquivo)
        {
            try
            {
                List<string> lista = new JavaScriptSerializer().Deserialize<List<Wallet>>(listaCarteiras).Select(a => a.id).ToList();
                ChamadaAPI.ListarEnderecosPorListaCarteira(lista, caminhoArquivo);
                return Json(new { sucesso = true });
            }
            catch (Exception e)
            {
                return Json(new { sucesso = false, mensagem = e.Message });
            }
        }

        public JsonResult CriarEnderecosCarteiraBitMinerELogar(string idCarteira, string enderecoArquivo, int quantidadeEnderecos)
        {
            try
            {
                List<Address> lista = ChamadaAPI.CriarEnderecosCarteiraBitMinerELogar(idCarteira, enderecoArquivo, quantidadeEnderecos);
                return Json(new { sucesso = true, listaEnderecos = new JavaScriptSerializer().Serialize(lista) });
            }
            catch(Exception e)
            {
                return Json(new { sucesso = false, mensagem = e.Message });
            }
        }

        public ActionResult Enderecos(string apiKey, string apiSecret)
        {
            ListaCarteirasModel model = new ListaCarteirasModel();
            ChamadaAPI.ApiKey = apiKey;
            ChamadaAPI.ApiSecret = apiSecret;

            List<Wallet> walletList = ChamadaAPI.ListarCarteiras();
            model.Carteiras = walletList;
            return View("~/Views/Carteiras/Enderecos.cshtml", model);
        }

        public JsonResult Retirar(string enderecoArquivo)
        {
            try
            {
                string endereco = String.Empty;
                ChamadaAPI.EnderecoArquivo = @"C:\LOGS\retirada.txt";
                ChamadaAPI.TratarDiretorio();
                if (ChamadaAPI.TratarDiretorioRetorno(enderecoArquivo))
                {
                    StreamReader SR = new StreamReader(enderecoArquivo);
                    while ((endereco = SR.ReadLine()) != null)
                    {
                        if (!String.IsNullOrEmpty(endereco))
                        {
                            ChamadaAPI.CriarOuLogarContaBitMinerSync(endereco);
                            ChamadaAPI.RetirarMinimoBitMinerSync(endereco);
                        }
                    }
                    ChamadaAPI.fecharArquivo();
                    return Json(new { sucesso = true });
                }
                return Json(new { sucesso = false, mensagem = "Diretório incorreto" });
            }
            catch (Exception ex)
            {
                return Json(new { sucesso = false, mensagem = ex.Message });
            }
        }
    }
}