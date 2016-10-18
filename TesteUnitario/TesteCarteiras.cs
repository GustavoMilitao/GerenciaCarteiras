using Microsoft.VisualStudio.TestTools.UnitTesting;
using API.Entities;

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
    }
}
