using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BancoCC
{
    public class Conect
    {
        private string conexaoString = "server=localhost;user=root;password=25127809Joci;database=bancocc";
        private MySqlConnection GetConnection()
        {
            return new MySqlConnection(conexaoString);
        }
        public bool ExecutarComando(string sql, Action<MySqlCommand> parametros = null)
        {
            using (var conexao = GetConnection())
            {
                using (var cmd = new MySqlCommand(sql, conexao))
                {
                    // Adiciona os parâmetros passados pela função lambda
                    if (parametros != null) parametros(cmd);

                    try
                    {
                        conexao.Open();
                        // ExecuteNonQuery retorna o NÚMERO DE LINHAS AFETADAS
                        int linhasAfetadas = cmd.ExecuteNonQuery();

                        // --- ESTA É A CORREÇÃO ---
                        // Retorna 'true' SOMENTE se 1 ou mais linhas mudaram.
                        return linhasAfetadas > 0;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Erro no ExecutarComando: {ex.Message}");
                        return false;
                    }
                }
            }
        }
        public object ExecutarConsulta(string sql, Action<MySqlCommand> parametros = null)
        {
            using (MySqlConnection conexao = new MySqlConnection(conexaoString))
            {
                try
                {
                    conexao.Open();
                    using (MySqlCommand comando = new MySqlCommand(sql, conexao))
                    {
                        if (parametros != null) parametros(comando);
                        return comando.ExecuteScalar();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Erro ao executar consulta: " + ex.Message);
                    return null;
                }
            }
        }
        public void ExecutarLeitura(string consulta, Action<MySqlCommand> prepararComando, Action<MySqlDataReader> lerResultados)
        {
            using (MySqlConnection conexao = new MySqlConnection(conexaoString))
            {
                try
                {
                    conexao.Open();
                    using (MySqlCommand comando = new MySqlCommand(consulta, conexao))
                    {
                        prepararComando?.Invoke(comando);
                        using (MySqlDataReader leitor = comando.ExecuteReader())
                        {
                            lerResultados?.Invoke(leitor);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Erro ao executar leitura: " + ex.Message);
                }
            }
        }



    }
}               


