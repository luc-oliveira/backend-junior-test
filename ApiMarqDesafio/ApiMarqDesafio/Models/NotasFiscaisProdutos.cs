using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApiMarqDesafio.Models
{
    public class NotasFiscaisProdutos
    {
        public int Id { get; set; }
        public int IdNota { get; set; }
        public int IdProduto { get; set; }
        public int Quantidade { get; set; }
        public decimal Preco { get; set; }
        public const string ALIAS = "NFP";
    }
}