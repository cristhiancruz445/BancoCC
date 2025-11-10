using MySql.Data.MySqlClient;
using Org.BouncyCastle.Bcpg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZstdSharp.Unsafe;

namespace BancoCC
{
    public class TelaInicial
    {
        public static void MostrarTelaInicial(int numeroContaLogado)
        {
            Console.Clear();
            Console.WriteLine("=============================");
            Console.WriteLine("   Bem-vindo ao BancoCC!    ");
            Console.WriteLine("=============================");
            Console.WriteLine($"Consta Conectada: {numeroContaLogado}");
            Console.WriteLine("Como podemos lhe ajudar?");
            Console.WriteLine("1 - Ver saldo");
            Console.WriteLine("2 - Fazer um depósito");
            Console.WriteLine("3 - Fazer um saque");
            Console.WriteLine("4 - Ver extrato");
            Console.WriteLine("5 - Realizar uma transferência");
            Console.WriteLine("0 - Sair");
            int opcao = int.Parse(Console.ReadLine());

            switch (opcao)
            {
                case 0: Environment.Exit(0); break;
                case 1: ContaVerSaldo(numeroContaLogado); break;
                //case 2:ContaDepositar(); break;
                //case 3:ContaSacar(); break;
                //case 4:ContaExtrato(); break;
                //case 5:ContaTransferir(); break;

                default:
                    Console.WriteLine("Opção inválida. Tente novamente.");
                    System.Threading.Thread.Sleep(2000);
                    MostrarTelaInicial(numeroContaLogado);
                    break;



            }
        }
        public static void ContaVerSaldo(int numeroContaLogado)
        {
            Console.Clear();
            Console.WriteLine("---Consulta de Saldo ---");
            ObterSaldo(numeroContaLogado);
        }
        public static void ObterSaldo(int numeroContaLogado)
        {
            string conexaoString = "server=localhost;user=root;password=25127809Joci;database=bancocc";
            string consulta = "SELECT saldo FROM usuario WHERE numero_conta = @numeroConta";
            try
            {
                using (MySqlConnection conexao = new MySqlConnection(conexaoString))
                {
                    conexao.Open();
                    using (MySqlCommand comando = new MySqlCommand(consulta, conexao))
                    {
                        comando.Parameters.AddWithValue("@numeroConta", numeroContaLogado);
                        object resultado = comando.ExecuteScalar();
                        if (resultado != null)
                        {
                            decimal saldo = Convert.ToDecimal(resultado);
                            Console.WriteLine($"Seu saldo atual é: R$ {saldo:F2}");
                        }
                        else
                        {
                            Console.WriteLine("Conta não encontrada.");
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine($"Erro de MySql ao obter saldo: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro geral ao obter saldo: {ex.Message}");

            }
        }
    }
}
