using Entidades.Entities;
using GridMvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GerenciaCarteiras.Models
{
    public class ListaCarteirasModel
    {
        public List<Wallet> Carteiras { get; set; }
    }
}