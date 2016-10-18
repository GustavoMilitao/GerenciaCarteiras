using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Entidades.Entities
{
    public class BasicEntity
    {
        public string id { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
        public string resource { get; set; }
        public string resource_path { get; set; }
    }
}