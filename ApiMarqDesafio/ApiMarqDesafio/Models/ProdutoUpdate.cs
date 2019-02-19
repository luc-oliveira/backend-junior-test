using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApiMarqDesafio.Models
{
    public class ProdutoUpdate
    {
        public int Id { get; set; }
        public int Quantidade { get; set; }
        public decimal Preco { get; set; }
    }
}