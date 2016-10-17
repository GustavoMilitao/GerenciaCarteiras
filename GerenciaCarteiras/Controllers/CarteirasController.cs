using Coinbase.ObjectModel;
using CoinBaseMinering.Entities;
using GerenciaCarteiras.Models;
using ListarCarteirasBitMiner.Entities;
using System;
using System.Collections.Generic;
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
            catch(Exception ex)
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

        public JsonResult Retirar(string listaCarteirasString)
        {
            try
            {
                List<Wallet> listaCarteiras = new JavaScriptSerializer().Deserialize<List<Wallet>>(listaCarteirasString);
                ChamadaAPI.EnderecoArquivo = @"D:\LOGS\retiradas.txt";
                ChamadaAPI.TratarDiretorio();
                foreach (Wallet w in listaCarteiras)
                {
                    if (w.Addresses.Count > 0)
                    {
                        ChamadaAPI.CriarOuLogarContaBitMinerSync(w.Addresses.FirstOrDefault(a => a != null).id);
                        ChamadaAPI.RetirarMinimoBitMinerSync(w.Addresses.First(a => a != null).id);
                    }
                    else
                    {
                        ChamadaAPI.SW.WriteLine("Carteira com Id: " + w.id + " está sem endereços");
                    }
                }
                ChamadaAPI.fecharArquivo();
                return Json(new { sucesso = true });
            }
            catch(Exception ex)
            {
                return Json(new { sucesso = false, mensagem = ex.Message });
            }
        }
    }
}