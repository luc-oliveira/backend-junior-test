using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ApiMarqDesafio.Models;

namespace ApiMarqDesafio.Controllers
{
    enum CAMPOS
    {
        IDEMPRESA = 100,
        DATAINICIAL = 101,
        DATAFINAL = 102
    }
    public class NotasFiscaisController : ApiController
    {
        // GET: api/NotasFiscais
        public HttpResponseMessage Get()
        {
            HttpResponseMessage response = null;
            SqlConnection sqlCon = null;
            try
            {
                sqlCon = new SqlConnection(ConfigurationManager.ConnectionStrings["MarqDatabaseConnectionString"].ConnectionString);
                sqlCon.Open();
                //Define objeto de retorno
                List<dynamic> mesesProdutos = new List<dynamic>();
                //Filtros enviados
                Dictionary<string, string> filtros = Request.GetQueryNameValuePairs().ToDictionary(e => e.Key, e => e.Value);
                string outValue = string.Empty;
                //Pega Id da empresa filtrada
                int IdEmpresa = 0;
                if (!filtros.TryGetValue(((int)CAMPOS.IDEMPRESA).ToString(), out outValue) || !int.TryParse(outValue, out IdEmpresa) || !Empresas.empresaValida(IdEmpresa, sqlCon))
                {
                    throw new Exception("É necessário enviar um ID Empresa válido!");
                }
                //Pega data inicial para filtro
                DateTime dtInicial = new DateTime();
                if (filtros.TryGetValue(((int)CAMPOS.DATAINICIAL).ToString(), out outValue))
                {
                    try
                    {
                        dtInicial = DateTime.ParseExact(outValue, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    }
                    catch { throw new Exception("Data Inicial em formato inválido!"); }
                }
                else throw new Exception("É necessário informar a Data Inicial!");
                //Pega data final para filtro
                DateTime dtFinal = new DateTime();
                if (filtros.TryGetValue(((int)CAMPOS.DATAFINAL).ToString(), out outValue))
                {
                    try
                    {
                        dtFinal = DateTime.ParseExact(outValue, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    }
                    catch { throw new Exception("Data Final em formato inválido!"); }
                }
                else throw new Exception("É necessário informar a Data Final!");

                string[] select = new string[]
                {
                $"Nome = UPPER({Produtos.ALIAS}.Nome)",
                $"Marca = UPPER({Produtos.ALIAS}.Marca)",
                $"MediaPreco = SUM({NotasFiscaisProdutos.ALIAS}.Preco) / SUM({NotasFiscaisProdutos.ALIAS}.Quantidade)",
                $"Mes = FORMAT({NotasFiscais.ALIAS}.DataHora, 'MM-yyyy')"
                };
                string[] join = new string[]
                {
                $"JOIN dbo.NotasFiscais {NotasFiscais.ALIAS} ON {NotasFiscais.ALIAS}.Id = {NotasFiscaisProdutos.ALIAS}.IdNota",
                $"JOIN dbo.Produtos {Produtos.ALIAS} ON {Produtos.ALIAS}.Id = {NotasFiscaisProdutos.ALIAS}.IdProduto"
                };
                string[] where = new string[]
                {
                $"{NotasFiscais.ALIAS}.IdEmpresa = {IdEmpresa}",
                $"{NotasFiscais.ALIAS}.DataHora BETWEEN '{dtInicial.ToString("yyyy-dd-MM")} 00:00:00' AND '{dtFinal.ToString("yyyy-dd-MM")} 23:59:00'"
                };
                string[] groupBy = new string[]
                {
                $"{Produtos.ALIAS}.Id, UPPER({Produtos.ALIAS}.Nome)",
                $"UPPER({Produtos.ALIAS}.Marca)",
                $"FORMAT({NotasFiscais.ALIAS}.DataHora, 'MM-yyyy')"
                };
                string[] orderBy = new string[]
                {
                $"FORMAT({NotasFiscais.ALIAS}.DataHora, 'MM-yyyy') ASC",
                $"UPPER({Produtos.ALIAS}.Nome) ASC, UPPER({Produtos.ALIAS}.Marca) ASC"
                };

                string script = getScript(select, $"dbo.NotasFicaisProdutos {NotasFiscaisProdutos.ALIAS}", join, where, groupBy, orderBy);
                List<IDataRecord> list = new List<IDataRecord>();
                SqlCommand sqlCommand = new SqlCommand(script, sqlCon);
                using (SqlDataReader sqlR = sqlCommand.ExecuteReader())
                {
                    list = sqlR.Cast<IDataRecord>().ToList();
                }

                mesesProdutos = list.GroupBy(g => g["Mes"]).Select(s => new
                {
                    Mes = formataMes(s.Key.ToString()),
                    Produtos = s.Select(p => new
                    {
                        Nome = Convert.ToString(p["Nome"]),
                        Marca = p["Marca"].Equals(DBNull.Value) ? "" : Convert.ToString(p["Marca"]),
                        MediaPreco = Convert.ToDecimal(p["MediaPreco"])
                    }).ToList()
                }).ToList<dynamic>();

                response = Request.CreateResponse(HttpStatusCode.OK, mesesProdutos);
            }
            catch (Exception e)
            {
                response = Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e.Message);
            }
            finally
            {
                if (sqlCon != null) sqlCon.Close();
            }

            return response;
        }
        // POST: api/NotasFiscais
        public HttpResponseMessage Post([FromBody]NotasFiscais value)
        {
            HttpResponseMessage response = null;
            SqlConnection sqlCon = null;
            try
            {
                response = Request.CreateResponse(HttpStatusCode.OK, "Nota Fiscal cadastrada com sucesso!");
            }
            catch (Exception e)
            {
                response = Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e.Message);
            }
            finally
            {
                if (sqlCon != null) sqlCon.Close();
            }

            return response;
        }

        // PUT: api/NotasFiscais
        public HttpResponseMessage Put([FromBody]NotasFiscais value)
        {
            HttpResponseMessage response = null;
            SqlConnection sqlCon = null;
            try
            {
                sqlCon = new SqlConnection(ConfigurationManager.ConnectionStrings["MarqDatabaseConnectionString"].ConnectionString);
                sqlCon.Open();
                //Checa existência da nota fiscal
                if (!NotasFiscais.nfValida(value.Id, sqlCon))
                {
                    throw new Exception("Nota fiscal não encontrada!");
                }
                //Verifica se chave estrangeira é válida
                if (!checaExistencia(new string[] {
                    $"{NotasFiscais.ALIAS}.Id = {value.Id}",
                    $"{NotasFiscais.ALIAS}.IdEmpresa = {value.IdEmpresa}"
                }, $"dbo.NotasFiscais {NotasFiscais.ALIAS}", sqlCon))
                {
                    throw new Exception("Id Empresa não é válido!");
                }
                //Checa se nota fiscal é a última enviada pela empresa
                SqlCommand sqlCommand = new SqlCommand($"SELECT TOP 1 {NotasFiscais.ALIAS}.Id FROM dbo.NotasFiscais {NotasFiscais.ALIAS} " +
                    $"WHERE {NotasFiscais.ALIAS}.IdEmpresa = {value.IdEmpresa} ORDER BY {NotasFiscais.ALIAS}.DataHora DESC", sqlCon);
                int id = 0;
                using (SqlDataReader sqlR = sqlCommand.ExecuteReader())
                {
                    sqlR.Read();
                    id = Convert.ToInt32(sqlR[0]);
                }
                if (id != value.Id)
                {
                    throw new Exception("Somente é permitido atualizar a nota fiscal caso esta seja a última nota enviada pela empresa!");
                }
                //Pega objeto a ser atualizado
                sqlCommand = new SqlCommand($"SELECT {NotasFiscais.ALIAS}.* FROM dbo.NotasFiscais {NotasFiscais.ALIAS}" +
                    $" WHERE {NotasFiscais.ALIAS}.Id = {value.Id}", sqlCon);
                IDataRecord nf = null;
                using (SqlDataReader sqlR = sqlCommand.ExecuteReader())
                {
                    nf = sqlR.Cast<IDataRecord>().FirstOrDefault();
                }
                //Atualiza
                List<string> updates = new List<string>();
                if (value.DataHora != new DateTime() && !value.DataHora.Equals(Convert.ToDateTime(nf["DataHora"])))
                {
                    updates.Add($"{NotasFiscais.ALIAS}.DataHora = '{value.DataHora.ToString("yyyy-dd-MM HH:mm:ss")}'");
                }
                if (value.Total != Convert.ToDecimal(nf["Total"]))
                {
                    updates.Add($"{NotasFiscais.ALIAS}.Total = {value.Total.ToString(CultureInfo.GetCultureInfo("en-GB"))}");
                }

                sqlCommand = new SqlCommand($"UPDATE {NotasFiscais.ALIAS} SET {string.Join(", ", updates)} " +
                    $"FROM dbo.NotasFiscais {NotasFiscais.ALIAS} WHERE {NotasFiscais.ALIAS}.Id = {value.Id}", sqlCon);
                try
                {
                    sqlCommand.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw new Exception($"Não foi possível atualizar a nota fiscal: {ex.Message}");
                }

                response = Request.CreateResponse(HttpStatusCode.OK, "Nota Fiscal atualizada com sucesso!");
            }
            catch (Exception e)
            {
                response = Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e.Message);
            }
            finally
            {
                if (sqlCon != null) sqlCon.Close();
            }

            return response;
        }

        // DELETE: api/NotasFiscais/id
        public HttpResponseMessage Delete(int id)
        {
            HttpResponseMessage response = null;
            SqlConnection sqlCon = null;
            try
            {
                sqlCon = new SqlConnection(ConfigurationManager.ConnectionStrings["MarqDatabaseConnectionString"].ConnectionString);
                sqlCon.Open();
                if (!NotasFiscais.nfValida(id, sqlCon))
                {
                    throw new Exception("Nota fiscal não encontrada!");
                }
                //Exclui registros de Notas Fiscais Produtos associados a Nota Fiscal excluída
                string script = $"DELETE FROM dbo.NotasFicaisProdutos WHERE IdNota = {id}";
                SqlCommand sqlCommand = new SqlCommand(script, sqlCon);
                try
                {
                    sqlCommand.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw new Exception($"Falha de comunicação, não foi possível excluir os registros associados a Nota Fiscal: {ex.Message}");
                }
                //Exclui nota fiscal
                script = $"DELETE FROM dbo.NotasFiscais WHERE Id = {id}";
                sqlCommand = new SqlCommand(script, sqlCon);
                try
                {
                    sqlCommand.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw new Exception($"Falha de comunicação, não foi possível excluir a Nota Fiscal: {ex.Message}");
                }

                response = Request.CreateResponse(HttpStatusCode.OK, "Nota Fiscal excluída com sucesso!");
            }
            catch (Exception ex)
            {
                response = Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
            finally
            {
                if (sqlCon != null) sqlCon.Close();
            }

            return response;
        }
        internal static bool checaExistencia(string[] where, string from, SqlConnection sqlCon)
        {
            bool flag = false;
            string[] select = new string[] { "COUNT(*)" };
            try
            {
                string script = getScript(select, from, null, where);
                SqlCommand sqlCommand = new SqlCommand(script, sqlCon);
                using (SqlDataReader sqlR = sqlCommand.ExecuteReader())
                {
                    sqlR.Read();
                    int result = Convert.ToInt32(sqlR[0]);
                    flag = result > 0;
                }
            }
            catch { flag = false; }
            return flag;
        }
        internal string formataMes(string text)
        {
            int mes = Convert.ToInt32(text.Split('-').First());
            string mesExtenso = DateTimeFormatInfo.CurrentInfo.GetMonthName(mes).ToUpper();
            return $"{mesExtenso} {text.Split('-').Last()}";
        }
        internal static string getScript(string[] select, string from, string[] join = null, string[] where = null, string[] groupBy = null, string[] orderBy = null)
        {
            //Monta query baseada nos parâmetros enviados
            string script = string.Empty;
            //--Select
            script = $"SELECT {string.Join(", ", select)} FROM {from}";
            //--Join
            script += join == null ? "" : $" {string.Join(" ", join)}";
            //--Where
            script += where == null ? "" : $" WHERE {string.Join(" AND ", where)}";
            //--GroupBy
            script += groupBy == null ? "" : $" GROUP BY {string.Join(", ", groupBy)}";
            //--OrderBy
            script += orderBy == null ? "" : $" ORDER BY {string.Join(", ", orderBy)}";

            return script;
        }
    }
}
