using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Entidades.Entities
{
    public class Transaction : BasicEntity
    {
        #region basic properties
        public string type { get; set; }
        public string status { get; set; }
        public double amount { get; set; }
        public double native_amount { get; set; }
        public string description { get; set; }
        public bool instant_exchange { get; set; }
        public string details { get; set; }
        public string network { get; set; }
        public string email { get; set; }
        #endregion

        #region another properties
        public Wallet to { get; set; }
        public Wallet from { get; set; }
        #endregion
    }
}