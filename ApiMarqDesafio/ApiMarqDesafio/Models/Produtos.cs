using ApiMarqDesafio.Controllers;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ApiMarqDesafio.Models
{
    public class Produtos
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Marca { get; set; }
        public const string ALIAS = "P";

        public static bool produtoValido(int id, SqlConnection sqlCon)
        {
            return NotasFiscaisController.checaExistencia(new string[] { $"{Produtos.ALIAS}.Id = {id}" }, $"dbo.Produtos {Produtos.ALIAS}", sqlCon);
        }
    }
}