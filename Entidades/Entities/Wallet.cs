using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades.Entities
{
    public class Wallet : BasicEntity, IEquatable<Wallet>
    {
        #region Propriedades básicas
        public string name { get; set; }
        public bool primary { get; set; }
        public string type { get; set; }
        public string currency { get; set; }
        public Balance balance { get; set; }
        public Balance native_balance { get; set; }
        #endregion

        #region Propriedades adicionadas
        public List<Address> Addresses { get; set; }

        #endregion

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            Wallet objAsWallet = obj as Wallet;
            if (objAsWallet == null) return false;
            else return Equals(objAsWallet);
        }

        public bool Equals(Wallet other)
        {
            if (other == null) return false;
            return (this.id.Equals(other.id));
        }


    }
}
