using ApiMarqDesafio.Controllers;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ApiMarqDesafio.Models
{
    public class NotasFiscais
    {
        public int Id { get; set; }
        public int IdEmpresa { get; set; }
        public DateTime DataHora { get; set; }
        public decimal Total { get; set; }
        public List<ProdutoUpdate> Produtos { get; set; }
        public const string ALIAS = "NF";

        public static bool nfValida(int id, SqlConnection sqlCon)
        {
            return NotasFiscaisController.checaExistencia(new string[] { $"{NotasFiscais.ALIAS}.Id = {id}" }, $"dbo.NotasFiscais {NotasFiscais.ALIAS}", sqlCon);
        }
    }
}