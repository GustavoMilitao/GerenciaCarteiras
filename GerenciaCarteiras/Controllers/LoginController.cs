using CoinBaseMinering.Util;
using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using GerenciaCarteiras.Models;
using ListarCarteirasBitMiner.Entities;
using CoinBaseMinering.Entities;
using System.Collections.Generic;
using System.Web.Security;
using Coinbase;

namespace GerenciaCarteiras.Controllers
{
    public class LoginController : Controller
    {
        public ActionResult Login()
        {
            List<String> identificadoresUsuarioLogado = ObterUsuarioLogado(System.Web.HttpContext.Current.User.Identity.Name);
            if (identificadoresUsuarioLogado == null)
                return View();
            else
                return RedirectToAction("ListaCarteiras","Carteiras", 
                    new { apiKey = identificadoresUsuarioLogado[0], apiSecret = identificadoresUsuarioLogado[1] });
        }

        public JsonResult Logar(string apiKey, string apiSecret)
        {
            if (!String.IsNullOrEmpty(apiKey) && !String.IsNullOrEmpty(apiSecret))
            {
                CoinbaseApi api = new CoinbaseApi(apiKey, apiSecret, Aplicacao.URLAPI);
                var response = api.SendRequest("accounts?&limit=100", null, RestSharp.Method.GET);
                if (response.Data != null)
                {
                    string identificador = System.Web.HttpContext.Current.User.Identity.Name;
                    if (!String.IsNullOrEmpty(identificador))
                    {

                        if (!String.IsNullOrEmpty(identificador))
                        {

                        }
                        return Json(new { sucesso = true, urlListar = Url.Action("ListaCarteiras", "Carteiras") });
                    }
                    else
                    {
                        FormsAuthentication.SetAuthCookie(apiKey + ";" + apiSecret, true);
                        return Json(new { sucesso = true, urlListar = Url.Action("ListaCarteiras", "Carteiras") });
                    }
                }
                else
                {
                    return Json(new { sucesso = false, mensagem = "Chave de API ou segredo de API inválido." });
                }
            }
            return Json(new { sucesso = false, mensagem = "Chave de API ou segredo de API não pode ser vazia." });
        }

        private List<String> ObterUsuarioLogado(string identificador)
        {
            if (!String.IsNullOrEmpty(identificador))
            {
                return identificador.Split(';').ToList();
            }
            return null;
        }

        public ActionResult Sair()
        {
            FormsAuthentication.SignOut();
            return Login();
        }
    }
}