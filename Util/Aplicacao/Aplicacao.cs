using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Util.Aplicacao
{
    public class Aplicacao
    {
        public static string URLAPI
        {
            get
            {
                return ConfigurationManager.AppSettings["urlAPI"];
            }
        }
    }
}
