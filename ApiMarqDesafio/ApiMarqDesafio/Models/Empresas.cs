using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using ApiMarqDesafio.Controllers;

namespace ApiMarqDesafio.Models
{
    public class Empresas
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Endereco { get; set; }
        public const string ALIAS = "E";

        public static bool empresaValida(int IdEmpresa, SqlConnection sqlCon)
        {
            return NotasFiscaisController.checaExistencia(new string[] { $"{Empresas.ALIAS}.Id = {IdEmpresa}" }, $"dbo.Empresas {Empresas.ALIAS}", sqlCon);
        }
    }
}