using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades.Entities
{
    public class Address : BasicEntity
    {
        #region Propriedades básicas
        public string address { get; set; }
        public string name { get; set; }
        public string network { get; set; }
        #endregion
    }
}
