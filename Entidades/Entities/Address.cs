using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinBaseMinering.Entities
{
    public class Address
    {
        #region Propriedades básicas
        public string id { get; set; }
        public string address { get; set; }
        public string name { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
        public string network { get; set; }
        public string resource { get; set; }
        public string resource_path { get; set; }
        #endregion
    }
}
