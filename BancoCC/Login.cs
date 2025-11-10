using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BancoCC
{
    class Login
    {
        public static void TelaLogin()
        {
            Console.Clear();
            Console.WriteLine("Seja bem vindo ao BancoCC!");
            Console.WriteLine("Por favor insira suas credenciais para começar.");
            Console.Write("Usuário: ");
            string usuario = Console.ReadLine();
            Console.Write("Senha: ");
            string senha = Console.ReadLine();
            Console.Write("Número da Conta: ");
            string numeroConta = Console.ReadLine();

            if(ValidarUsuario(usuario, senha, int.Parse(numeroConta)))
            {
                Console.WriteLine("Login realizado com sucesso!");
                TelaInicial.MostrarTelaInicial(int.Parse(numeroConta));
            }
            else
            {
                Console.WriteLine("Credenciais inválidas. Tente novamente.");
                System.Threading.Thread.Sleep(2000);
                TelaLogin();
            }

        }

        public static void CriarConta()
        {
            Console.Clear();
            Console.WriteLine("Criar sua conta Grátis: ");
            Console.WriteLine("=======================");
            Console.Write("Digite seu nome de usuário: ");
            string userName = Console.ReadLine();
            Console.Write("Qual senha deseja colocar? ");
            string password = Console.ReadLine();

            AdicionarUser(userName, password);
           
        }
        public static void TemConta()
        {
            Console.WriteLine("Você já possui uma conta conosco? ");
            Console.WriteLine("1 - Sim");
            Console.WriteLine("2 - Não");
            Console.WriteLine("0 - Sair");
            int opcao = int.Parse(Console.ReadLine());

            switch (opcao)
            {
                case 1:
                    TelaLogin();
                    break;
                case 2:
                    CriarConta();
                    break;
                case 0:
                    Environment.Exit(0);
                    break;
            }
        }
        public static void AdicionarUser(string userName, string password)
        {
            string conn = "Server=localhost;Database=BancoCC;User ID=root;Password=25127809Joci;";
            string sql = "INSERT INTO usuario (Nome, senha, Numero_conta) VALUES (@Nome, @senha,@Numero_conta)";
            try
            {
                using (MySqlConnection conexao = new MySqlConnection(conn))
                {
                    conexao.Open();
                    using (MySqlCommand cmd = new MySqlCommand(sql, conexao))
                    {
                        cmd.Parameters.AddWithValue("@Nome", userName);
                        cmd.Parameters.AddWithValue("@senha", password);
                        cmd.Parameters.AddWithValue("@Numero_conta", new Random().Next(100000, 999999));

                        int linhasAfetadas = cmd.ExecuteNonQuery();

                        Console.WriteLine($"Usuário cadastrado com sucesso! Linhas Afetadas: {linhasAfetadas}");
                    }
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine($"Erro de MySql ao inserir: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro geral: {ex.Message}");
            }
        }
        public static bool ValidarUsuario(string userName, string password, int numeroConta)
        {
            string conn = "Server=localhost;Database=BancoCC;User ID=root;Password=25127809Joci;";
            string sql = "SELECT COUNT(*) FROM usuario WHERE Nome = @Nome AND senha = @senha AND Numero_conta = @numeroConta";

            try
            {
                using (MySqlConnection conexao = new MySqlConnection(conn))
                {
                    conexao.Open();
                    using (MySqlCommand cmd = new MySqlCommand(sql, conexao))
                    {
                        cmd.Parameters.AddWithValue("@Nome", userName);
                        cmd.Parameters.AddWithValue("@senha", password);
                        cmd.Parameters.AddWithValue("@numeroConta", numeroConta);

                        object result = cmd.ExecuteScalar();

                        int count = Convert.ToInt32(result);
                        return count > 0;
                    }
               }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao verificar login:{ex.Message}");
                return false;
            }
            }
        }
    }
