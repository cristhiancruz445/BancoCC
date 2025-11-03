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
        string conn = "Server=localhost;Database=BancoCC;User ID=root;Password=25127809Joci;";
        public static void MostrarTelaLogin()
        {
            Console.WriteLine("Seja bem vindo ao BancoCC!");
            Console.WriteLine("Por favor insira suas credenciais para começar.");
            Console.Write("Usuário: ");
            string usuario = Console.ReadLine();
            Console.Write("Senha: ");
            string senha = Console.ReadLine();

            if (
        }

        public static void CriarConta()
        {
            Console.WriteLine("Criar sua conta Grátis: ");
            Console.WriteLine("Digite seu nome de usuário: ");
            string userName = Console.ReadLine();
            Console.WriteLine("Qual senha deseja colocar? ");
            string password = Console.ReadLine();

            

        }
    }
}
