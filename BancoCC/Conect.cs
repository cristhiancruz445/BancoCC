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
        public bool ExecutarComando(string sql, Action<MySqlCommand> parametros = null)
        {
            using (MySqlConnection conexao = new MySqlConnection(conexaoString))
            {
                try
                {
                    conexao.Open();
                    using (MySqlCommand comando = new MySqlCommand(sql, conexao))
                    {
                        if (parametros != null) parametros(comando);
                        comando.ExecuteNonQuery();
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Erro ao executar comando: " + ex.Message);
                    return false;
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


