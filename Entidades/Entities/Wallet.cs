using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinBaseMinering.Entities
{
    public class Wallet
    {
        #region Propriedades básicas
        public string id { get; set; }
        public string name { get; set; }
        public bool primary { get; set; }
        public string type { get; set; }
        public string currency { get; set; }
        public Balance balance { get; set; }
        public Balance native_balance { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
        public string resource { get; set; }
        public string resource_path { get; set; }
        #endregion

        #region Propriedades adicionadas
        public List<Address> Addresses { get; set; }
        #endregion
    }
}
