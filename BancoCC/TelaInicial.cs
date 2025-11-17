using MySql.Data.MySqlClient;
using Mysqlx.Crud;
using Org.BouncyCastle.Bcpg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
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
            Console.WriteLine("=============================\n");
            Console.WriteLine("Como podemos lhe ajudar?\n");
            Console.WriteLine("1 - Ver saldo");
            Console.WriteLine("2 - Fazer um depósito");
            Console.WriteLine("3 - Fazer um saque");
            Console.WriteLine("4 - Ver extrato");
            Console.WriteLine("5 - Realizar uma transferência");
            Console.WriteLine("0 - Sair\n");
            Console.WriteLine($"Conta Conectada: {numeroContaLogado}\n");
            Console.Write("Selecione uma opção: ");
            int opcao = int.Parse(Console.ReadLine());

            switch (opcao)
            {
                case 0: Environment.Exit(0); break;
                case 1: ContaVerSaldo(numeroContaLogado); break;
                case 2: ContaDepositar(numeroContaLogado); break;
                case 3: ContaSacar(numeroContaLogado); break;
                case 4: ContaExtrato(numeroContaLogado); break;
                case 5: ContaTransferir(numeroContaLogado); break;

                default:
                    Console.WriteLine("---Opção inválida. Tente novamente---");
                    System.Threading.Thread.Sleep(2000);
                    MostrarTelaInicial(numeroContaLogado);
                    break;



            }
        }
        public static void ContaVerSaldo(int numeroContaLogado)
        {
            Console.Clear();
            Console.WriteLine("---Consulta de Saldo ---\n");
            ObterSaldo(numeroContaLogado);
        }
        public static void ObterSaldo(int numeroContaLogado)
        {
            Conect db = new Conect();
            string consulta = "SELECT saldo FROM usuario WHERE numero_conta = @numeroConta";
            object resultado = db.ExecutarConsulta(consulta, (cmd) =>
            {
                cmd.Parameters.AddWithValue("@numeroConta", numeroContaLogado);
            });
            if (resultado != null && decimal.Parse(resultado.ToString()) is decimal saldo)
            {
                Console.WriteLine($"Seu saldo atual é: R$ {saldo:F2}");
            }
            else
            {
                Console.WriteLine("Conta não encontrada.");
            }
            ;
            Console.WriteLine("\nPressione qualquer tecla para voltar ao menu principal");
            Console.ReadKey();
            MostrarTelaInicial(numeroContaLogado);
        }
        public static void ContaDepositar(int numeroContaLogado)
        {
            Console.Clear();
            Console.WriteLine("---Depositar---");
            Console.Write("\nDigite o valor a ser depositado: ");
            Decimal valorDeposito = Decimal.Parse(Console.ReadLine());

            if (valorDeposito <= 0)
            {
                Console.WriteLine("\nValor inválido. O depósito deve ser maior que zero.");
                Thread.Sleep(2000);
                ContaDepositar(numeroContaLogado);
                return;
            }
            else
            {
                RealizarDeposito(numeroContaLogado, valorDeposito);
            }


        }
        public static void RealizarDeposito(int numeroContaLogado, decimal valorDeposito)
        {
            Conect db = new Conect();
            string comando = "UPDATE usuario SET saldo = saldo + @valorDeposito WHERE numero_conta = @numeroConta";
            bool sucesso = db.ExecutarComando(comando, (cmd) =>
            {
                cmd.Parameters.AddWithValue("@numeroConta", numeroContaLogado);
                cmd.Parameters.AddWithValue("@valorDeposito", valorDeposito);
            });
            if (sucesso)
            {
                Console.WriteLine("Depósito Realizado com sucesso!");
                RegistrarTransacao(numeroContaLogado, "Depósito", valorDeposito);
                ObterSaldo(numeroContaLogado);
            }
            else
            {
                Console.WriteLine("Erro ao realizar o depósito!");
                Thread.Sleep(2000);
                TelaInicial.ContaDepositar(numeroContaLogado);
            }

        }
        public static void ContaSacar(int numeroContaLogado)
        {
            Console.Clear();
            Console.WriteLine("---Saque---");
            Console.Write("\nDigite o valor a ser sacado: ");
            decimal valorSaque = decimal.Parse(Console.ReadLine());

            if (valorSaque <= 0)
            {
                Console.WriteLine("\nSaque inválido, o valor precisa ser maior que R$0,00!");
                System.Threading.Thread.Sleep(2000);
                TelaInicial.ContaSacar(numeroContaLogado);
                return;
            }
            else
            {
                RealizarSaque(numeroContaLogado, valorSaque);
            }
        }
        public static void RealizarSaque(int numeroContaLogado, decimal valorSaque)
        {
            Console.Clear();
            Conect db = new Conect();
            string comando = "UPDATE usuario SET saldo = saldo - @valorSaque WHERE numero_conta = @numeroConta AND saldo >= @valorSaque";
            bool sucesso = db.ExecutarComando(comando, (cmd) =>
            {
                cmd.Parameters.AddWithValue("@numeroConta", numeroContaLogado);
                cmd.Parameters.AddWithValue("@valorSaque", valorSaque);
            });
            if (sucesso)
            {
                Console.WriteLine("---Saque---");
                Console.WriteLine("\nSaque realizado com sucesso!\n");
                RegistrarTransacao(numeroContaLogado, "Saque", -valorSaque);
                ObterSaldo(numeroContaLogado);

            }
            else
            {
                Console.WriteLine("Erro ao realizar o saque! Verifique se você tem saldo suficiente.");
                Thread.Sleep(2000);
                TelaInicial.ContaSacar(numeroContaLogado);
            }
            RegistrarTransacao(numeroContaLogado, "Saque", -valorSaque);


        }
        public static void ContaExtrato(int numeroContaLogado)
        {
            Console.Clear();
            Console.WriteLine("---Extrato da conta---");

            Conect db = new Conect();
            string consulta = "SELECT data_transacao, tipo, valor FROM transacoes WHERE numero_conta = @numeroConta ORDER BY data_transacao DESC";
            bool temTransacoes = false;

            db.ExecutarLeitura(consulta, (cmd) =>
            {
                cmd.Parameters.AddWithValue("@numeroConta", numeroContaLogado);
            },
            (reader) =>
            {
                while (reader.Read())
                {
                    temTransacoes = true;
                    DateTime dataTransacao = reader.GetDateTime("data_transacao");
                    string tipo = reader.GetString("tipo");
                    decimal valor = reader.GetDecimal("valor");

                    if (valor > 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"{dataTransacao.ToString("dd/MM/yyyy HH:mm")} - {tipo,-15} R$ {valor,10:F2}");
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"{dataTransacao.ToString("dd/MM/yyyy HH:mm")} - {tipo,-15} R$ {valor,10:F2}");
                    }
                    Console.ResetColor();

                }
            });
            if (!temTransacoes)
            {
                Console.WriteLine("\nNenhuma transação efetuada.\n");
                ObterSaldo(numeroContaLogado);
            }
            Console.WriteLine("\n Pressione qualquer tecla para voltar para a tela inicial");
            Console.ReadKey();
            TelaInicial.MostrarTelaInicial(numeroContaLogado);
        }
        public static void RegistrarTransacao(int numeroContaLogado, string tipo, decimal valor)
        {
            Conect db = new Conect();
            string comando = "INSERT INTO transacoes (numero_conta, tipo, valor, data_transacao) values (@numeroConta, @tipo, @valor, NOW())";

            db.ExecutarComando(comando, (cmd) =>
            {
                cmd.Parameters.AddWithValue("@numeroConta", numeroContaLogado);
                cmd.Parameters.AddWithValue("@tipo", tipo);
                cmd.Parameters.AddWithValue("@valor", valor);

            });
        }
        public static void ContaTransferir(int numeroContaLogado)
        {
            Console.Clear();
            Console.WriteLine("---Transferência---");

            Console.Write("\nDigite o NÚMERO da conta de destino: ");
            if (!int.TryParse(Console.ReadLine(), out int contaDestino))
            {
                Console.WriteLine("\nNúmero de conta inválido.");
                Thread.Sleep(2000);
                MostrarTelaInicial(numeroContaLogado);
                return;
            }

            Console.Write("Digite o VALOR da transferência: R$ ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal valor))
            {
                Console.WriteLine("\nValor inválido.");
                Thread.Sleep(2000);
                MostrarTelaInicial(numeroContaLogado);
                return;
            }

            bool sucesso = RealizarTransferencia(numeroContaLogado, contaDestino, valor);

            if (sucesso)
            {
                Console.WriteLine("\nTransferência realizada com sucesso!");
                ObterSaldo(numeroContaLogado);
            }
            else
            {
                Console.WriteLine("\nPressione qualquer tecla para voltar...");
                Console.ReadKey();
                MostrarTelaInicial(numeroContaLogado);
            }
        }
        public static bool RealizarTransferencia(int numeroContaOrigem, int numeroContaDestino, decimal valor)
        {
            if (numeroContaOrigem == numeroContaDestino)
            {
                Console.WriteLine("Erro: Não é possível transferir para a mesma conta.");
                return false;
            }
            if (valor <= 0)
            {
                Console.WriteLine("Erro: O valor da transferência deve ser maior que zero.");
                return false;
            }

            string connectionString = "server=localhost;user=root;password=25127809Joci;database=bancocc";

            using (var conexao = new MySqlConnection(connectionString))
            {
                try
                {
                    conexao.Open();
                    using (var transacao = conexao.BeginTransaction())
                    {
                        int linhasAfetadasDebito = 0;
                        string comandoDebito = "UPDATE usuario set saldo = saldo - @valor WHERE numero_conta = @contaOrigem AND Saldo >= @valor;";

                        using (var cmdDebito = new MySqlCommand(comandoDebito, conexao, transacao))
                        {
                            cmdDebito.Parameters.AddWithValue("@Valor", valor);
                            cmdDebito.Parameters.AddWithValue("@contaOrigem", numeroContaOrigem);
                            linhasAfetadasDebito = cmdDebito.ExecuteNonQuery();
                        }

                        if (linhasAfetadasDebito == 0)
                        {
                            Console.WriteLine("Erro: Saldo insuficiente ou conta de origem inválida.");
                            transacao.Rollback();
                            return false;
                        }

                        string comandoCredito = "UPDATE usuario set saldo = saldo + @valor WHERE numero_conta = @contaDestino;";
                        int linhasAfetadasCredito = 0;

                        using (var cmdCredito = new MySqlCommand(comandoCredito, conexao, transacao))
                        {
                            cmdCredito.Parameters.AddWithValue("@Valor", valor);
                            cmdCredito.Parameters.AddWithValue("@contaDestino", numeroContaDestino);
                            linhasAfetadasCredito = cmdCredito.ExecuteNonQuery();
                        }

                        if (linhasAfetadasCredito == 0)
                        {
                            Console.WriteLine("Erro: Conta de destino inválida.");
                            transacao.Rollback();
                            return false;
                        }

                        RegistrarTransacaoTransacional(numeroContaOrigem, "Transferência Enviada", -valor, conexao, transacao);
                        RegistrarTransacaoTransacional(numeroContaDestino, "Transferência Recebida", valor, conexao, transacao);

                        transacao.Commit();
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Erro crítico ao realizar a transferência: " + ex.Message);
                    return false;
                }
            } 
        }
        private static void RegistrarTransacaoTransacional(int numeroConta, string tipo, decimal valor, MySqlConnection conexao, MySqlTransaction transacao)
        {
            string comando = "INSERT INTO transacoes (numero_conta, tipo, valor, data_transacao) values (@numeroConta, @tipo, @valor, NOW())";

            using (var cmd = new MySqlCommand(comando, conexao, transacao))
            {
                cmd.Parameters.AddWithValue("@numeroConta", numeroConta);
                cmd.Parameters.AddWithValue("@tipo", tipo);
                cmd.Parameters.AddWithValue("@valor", valor);
                cmd.ExecuteNonQuery();
            }
        }
    }
}