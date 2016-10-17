using Coinbase.ObjectModel;
using CoinBaseMinering.Entities;
using GerenciaCarteiras.Models;
using ListarCarteirasBitMiner.Entities;
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