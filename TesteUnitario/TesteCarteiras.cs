using Microsoft.VisualStudio.TestTools.UnitTesting;
using API.Entities;
using System.IO;
using System.Collections.Generic;
using System;

namespace TesteUnitario
{
    [TestClass]
    public class TesteCarteiras
    {
        [TestMethod]
        public void TestarTransferenciaFundos()
        {
            ChamadaAPI.ApiKey = "B424tQaMrUZuDb6S";
            ChamadaAPI.ApiSecret = "rcmaDnds9ElW5ngxvlaApdwy6sxTz4VZ";
            ChamadaAPI.TransferirFundosContas("Principal", @"C:\LOGS\transferenciaTeste.txt");
         
        }

        [TestMethod]
        public void TestarVesaoBitMinerContas()
        {
            string enderecoArquivo = @"C:\LOGS\payoutsnew.txt";
            StreamReader sr = new StreamReader(enderecoArquivo);
            StreamWriter sw = new StreamWriter(@"C:\LOGS\payoutswithlevel");
            string linha = string.Empty;
            string response= string.Empty;
            string versaobitminer = string.Empty;
            int pos = 0;
            while (!string.IsNullOrEmpty((linha = sr.ReadLine())))
            {
                 response = ChamadaAPI.GetResponseBitMinerByAddress(linha);
                pos = response.IndexOf("BitMiner v1.");
                versaobitminer = response.Substring(pos, 13);
                sw.WriteLine(linha + " : " + versaobitminer);
            }
            sr.Close();
            sw.Close();
        }

        [TestMethod]
        public void GetResponsePayouts()
        {
            int offSet = 0;
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
                linhas = r.Split(new string[] { "<td>" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string s in linhas)
                {
                    ChamadaAPI.SW.WriteLine(s);
                }
                //beforeLevel = r.IndexOf("BitMiner 1.");
                //nivel = r.Substring(beforeLevel, beforeLevel + 12);
                //listaniveis.Add(nivel);
            }
            ChamadaAPI.fecharArquivo();
            //return Json(new { sucesso = true, paginacao = new JavaScriptSerializer().Serialize(paginas) });
        }
    }
}
