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
                case 2: ContaDepositar(numeroContaLogado); break;
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
                            Thread.Sleep(2000);
                            TelaInicial.MostrarTelaInicial(numeroContaLogado);
                        }
                        else
                        {
                            Console.WriteLine("Conta não encontrada.");
                            Thread.Sleep(2000);
                            TelaInicial.MostrarTelaInicial(numeroContaLogado);
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
        public static void ContaDepositar(int numeroContaLogado)
        {
            Console.Clear();
            Console.WriteLine("---Depositar---");
            Console.Write("Digite o valor a ser depositado: ");
            Decimal valorDeposito = Decimal.Parse(Console.ReadLine());

            if (valorDeposito <= 0)
            {
                Console.WriteLine("Valor inválido. O depósito deve ser maior que zero.");
                System.Threading.Thread.Sleep(2000);
                MostrarTelaInicial(numeroContaLogado);
                return;
            }
            else
            {
                RealizarDeposito(numeroContaLogado, valorDeposito);
            }

        }
        public static void RealizarDeposito(int numeroContaLogado, decimal valorDeposito)
        {
            
            string conexaoString = "server=localhost;user=root;password=25127809Joci;database=bancocc";
            string consulta = "UPDATE usuario SET saldo = saldo + @valorDeposito WHERE numero_conta = @numeroConta";

            try
            {
                using (MySqlConnection conexao = new MySqlConnection(conexaoString))
                {
                    conexao.Open();
                    { using (MySqlCommand atualizarSaldo = new MySqlCommand(consulta, conexao))
                    {
                            atualizarSaldo.Parameters.AddWithValue("@numeroConta", numeroContaLogado);
                            atualizarSaldo.Parameters.AddWithValue("@valorDeposito", valorDeposito);
                            
                            int linhasAfetadas = atualizarSaldo.ExecuteNonQuery();

                            if (linhasAfetadas > 0)
                            {
                                Console.WriteLine("Depósito realizado com sucesso!");
                                ObterSaldo(numeroContaLogado);
                            }
                            else
                            {
                                Console.WriteLine("Conta não encontrada. Depósito não realizado.");
                            }
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine($"Erro de MySql ao realizar depósito: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro geral ao realizar depósito: {ex.Message}");
            }
        }
    }
}
