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
                return Json(new { sucesso = true, response = ChamadaAPI.GetResponseLoggedInBitminer(address).Result });
            }
            catch(Exception ex)
            {
                return Json(new { sucesso = false, mensagem = ex.Message });
            }
        }
    }
}