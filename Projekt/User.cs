using System.ComponentModel.Design;
using System.Globalization;
using System.Reflection.Metadata.Ecma335;
using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;

namespace Projekt
{
    internal class User
    {
        public static string? user;
        readonly static char[] allowedUsernameCharacters = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
        
        // Logowanie
        public static void SignIn()
        {
            Console.Clear();

            // Podanie nazwy użytkownika
            string username;
            while (true)
            {
                Console.WriteLine("Podaj nazwę użytkownika: ");
                Console.Write("> ");
                username = Console.ReadLine();

                // Sprawdzenie, czy użytkownik o takiej nazwie już istnieje
                if (!UserExists(username))
                {
                    Console.Clear();
                    Console.WriteLine("Użytkownik o takiej nazwie nie istnieje");
                }
                else break;
            }
            Console.Clear();

            // Podanie hasła
            string password;
            while (true)
            {
                Console.WriteLine("Podaj hasło: ");
                Console.Write("> ");
                password = Console.ReadLine();

                if (!IsPasswordCorrect(username, password))
                {
                    Console.Clear();
                    Console.WriteLine("Podano niepoprawne hasło");
                }
                else break;
            }

            user = username;
            Console.Clear();
            Console.WriteLine("Witaj, {0}", user);
        }

        // Rejestracja
        public static void SignUp()
        {
            Console.Clear();

            // Ustawianie nazwy użytkownika
            string username;
            while (true)
            {
                while (!SetUsername(out username))
                {
                    Console.Clear();
                    Console.WriteLine("Nazwa nie spełnia wymagań");
                }

                // Sprawdzenie, czy użytkownik o takiej nazwie już istnieje
                if (UserExists(username))
                {
                    Console.Clear();
                    Console.WriteLine("Użytkownik o takiej nazwie już istnieje");
                }
                else break;
            }
            Console.Clear();

            // Ustawianie hasła
            string password;
            while (!SetPassword(out password))
            {
                Console.Clear();
                Console.WriteLine("Hasło nie spełnia wymagań");
            }

            // Dodanie użytkownika do danych
            try
            {
                Program.conn.Open();
                MySqlCommand command = new MySqlCommand("INSERT INTO Users (Username, Password) VALUES (@username, @password)", Program.conn);
                command.Parameters.AddWithValue("@username", username);
                command.Parameters.AddWithValue("@password", password);
                command.ExecuteNonQuery();
                Program.conn.Close();
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.Clear();
        }

        // Wylogowywanie
        public static void SignOut()
        {
            user = null;
            Console.Clear();
        }

        // Zmiana nazwy użytkownika
        public static void ChangeUsername(out Controls.Options option)
        {
            Console.Clear();

            // Ustawianie nowej nazwy użytkownika
            string username;
            bool czyIstnieje = false;
            while (true)
            {
                username = null;
                do
                {
                    if (!czyIstnieje)
                    {
                        Console.Clear();
                    }
                    czyIstnieje = false;
                    if (username != null)
                    {
                        Console.WriteLine("Nazwa nie spełnia wymagań");
                    }
                    Console.WriteLine("Wpisz [0], aby anulować");
                    if (username == "0")
                    {
                        Console.Clear();
                        Console.WriteLine("Anulowano zmianę nazwy użytkownika");
                        option = Controls.Options.Cancel;
                        return;
                    }
                }
                while (!SetUsername(out username));

                // Sprawdzenie, czy użytkownik o takiej nazwie już istnieje
                if (UserExists(username))
                {
                    Console.Clear();
                    Console.WriteLine("Użytkownik o takiej nazwie już istnieje");
                    czyIstnieje = true;
                }
                else break;
            }

            // Potwierdzenie hasłem
            string password;
            Console.Clear();
            do
            {
                
                Console.WriteLine("Aby potwierdzić wpisz ponownie hasło");
                Console.WriteLine("Wpisz [0], aby anulować");
                Console.Write("> ");
                password = Console.ReadLine();
                if (password == "0")
                {
                    Console.Clear();
                    Console.WriteLine("Anulowano zmianę nazwy użytkownika");
                    option = Controls.Options.Cancel;
                    return;
                }
                if (!IsPasswordCorrect(user, password))
                {
                    Console.Clear();
                    Console.WriteLine("Podano niepoprawne hasło");
                }
                else break;
            }
            while (true);

            // Zapisanie nowej nazwy do danych
            try
            {
                Program.conn.Open();
                MySqlCommand command = new MySqlCommand("UPDATE Users SET Username = @username WHERE Username = @user", Program.conn);
                command.Parameters.AddWithValue("@username", username);
                command.Parameters.AddWithValue("@user", user);
                command.ExecuteNonQuery();
                Program.conn.Close();
            }
            catch (MySqlException e)
            {
                Console.WriteLine(e.Message);
            }

            user = null;
            option = Controls.Options.Continue;
            Console.Clear();
        }

        // Zmiana hasła użytkownika
        public static void ChangePassword(out Controls.Options option)
        {
            Console.Clear();

            // Potwierdzenie zmiany starym hasłem
            while (true)
            {
                Console.WriteLine("Aby kontynuować, podaj stare hasło");
                Console.WriteLine("Wpisz [0], aby anulować");
                Console.Write("> ");
                string oldPassword = Console.ReadLine();

                if (oldPassword == "0")
                {
                    Console.Clear();
                    Console.WriteLine("Anulowano zmianę hasła użytkownika");
                    option = Controls.Options.Cancel;
                    return;
                }
                else if (!IsPasswordCorrect(user, oldPassword))
                {
                    Console.Clear();
                    Console.WriteLine("Podano niepoprawne hasło");
                }
                else break;
            }

            // Ustawienie nowego hasła użytkownika
            string password = null;
            do
            {
                Console.Clear();
                if (password != null)
                {
                    Console.Clear();
                    Console.WriteLine("Hasło nie spełnia wymagań");
                }
                Console.WriteLine("Wpisz [0], aby anulować");
                if (password == "0")
                {
                    Console.Clear();
                    Console.WriteLine("Anulowano zmianę hasła użytkownika");
                    option = Controls.Options.Cancel;
                    return;
                }
            }
            while (!SetPassword(out password));

            // Potwierdzenie zmiany poprzez wpisanie ponownie nowego hasła
            Console.Clear();
            while (true)
            {
                Console.WriteLine("Aby kontynuować, podaj ponownie nowe hasło");
                Console.WriteLine("Wpisz [0], aby anulować");
                Console.Write("> ");
                string newPassword = Console.ReadLine();

                if (newPassword == "0")
                {
                    Console.Clear();
                    Console.WriteLine("Anulowano zmianę hasła użytkownika");
                    option = Controls.Options.Cancel;
                    return;
                }
                else if (newPassword != password)
                {
                    Console.Clear();
                    Console.WriteLine("Podano niepoprawne hasło");
                }
                else break;
            }

            // Zapisanie nowego hasła do danych
            try
            {
                Program.conn.Open();
                MySqlCommand command = new MySqlCommand("UPDATE Users SET Password = @password WHERE Username = @user", Program.conn);
                command.Parameters.AddWithValue("@password", password);
                command.Parameters.AddWithValue("@user", user);
                command.ExecuteNonQuery();
                Program.conn.Close();
            }
            catch (MySqlException e)
            {
                Console.WriteLine(e.Message);
            }
            user = null;
            option = Controls.Options.Continue;
            Console.Clear();
        }

        // Usunięcie konta użytkownika z danych
        public static void DeleteUser(out Controls.Options option)
        {
            Console.Clear();

            // Potwierdzenie usunięcia hasłem
            while (true)
            {
                Console.WriteLine("Aby kontynuować, podaj hasło");
                Console.WriteLine("Wpisz [0], aby anulować");
                Console.Write("> ");
                string password = Console.ReadLine();

                if (password == "0")
                {
                    Console.Clear();
                    Console.WriteLine("Anulowano usunięcie konta");
                    option = Controls.Options.Cancel;
                    return;
                }
                else if (!IsPasswordCorrect(user, password))
                {
                    Console.Clear();
                    Console.WriteLine("Podano niepoprawne hasło");
                }
                else break;
            }

            // Potwierdzenie usunięcia wpisaniem 'Potwierdzam'
            Console.Clear();
            while (true)
            {
                Console.WriteLine("Aby kontynuować, wpisz 'Potwierdzam'");
                Console.WriteLine("Wpisz [0], aby anulować");
                Console.Write("> ");
                string input = Console.ReadLine();

                if (input == "0")
                {
                    Console.Clear();
                    Console.WriteLine("Anulowano usunięcie konta");
                    option = Controls.Options.Cancel;
                    return;
                }
                else if (input != "Potwierdzam")
                {
                    Console.Clear();
                    Console.WriteLine("Spróbuj jeszcze raz");
                }
                else break;
            }

            // Usunięcie konta użytkownika z danych
            try
            {
                Program.conn.Open();
                MySqlCommand command = new MySqlCommand("DELETE FROM Users WHERE Username = @user", Program.conn);
                command.Parameters.AddWithValue("@user", user);
                command.ExecuteNonQuery();
                Program.conn.Close();
            }
            catch (MySqlException e)
            {
                Console.WriteLine(e.Message);
            }
            user = null;
            option = Controls.Options.Continue;
            Console.Clear();
        }

        // Sprawdzenie, czy użytkownik o takiej nazwie już istnieje
        static bool UserExists(string username)
        {
            bool flag = false;
            try
            {
                Program.conn.Open();
                MySqlCommand command = new MySqlCommand("SELECT Username FROM Users", Program.conn);
                using (var mr = command.ExecuteReader())
                {
                    while (mr.Read())
                    {
                        if (username.Equals(mr["Username"].ToString()))
                        {
                            flag = true;
                            break;
                        }
                    }
                }
                Program.conn.Close();
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            return flag;
        }

        // Ustawianie nazwy użytkownika
        private static bool SetUsername(out string username)
        {
            Console.WriteLine("Nazwa użytkownika powinna zawierać znaki: [a-z], [A-Z], [0-9], powinna zawierać przynajmniej 4 znaków oraz nie powinna zawierać znaków pustych");
            Console.WriteLine("Podaj nazwę użytkownika: ");
            Console.Write("> ");
            username = Console.ReadLine();

            // Sprawdzenie wymagań nazwy
            bool containsAllowedCharacters = username.All(c => allowedUsernameCharacters.Contains(char.ToLower(c)));
            bool isCorrectLength = username.Length >= 4;
            bool containsBlankSpace = username.Contains(' ');
            if (!containsAllowedCharacters || !isCorrectLength || containsBlankSpace) return false;
            else return true;
        }

        // Ustawianie hasła użytkownika
        private static bool SetPassword(out string password)
        {
            Console.WriteLine("Hasło powinno zawierać przynajmniej 5 znaków i żadnych znaków pustych");
            Console.WriteLine("Podaj hasło: ");
            Console.Write("> ");
            password = Console.ReadLine();

            // Sprawdzenie wymagań hasła
            bool isCorrectLength = password.Length >= 5;
            bool containsBlankSpace = password.Contains(' ');
            if (!isCorrectLength || containsBlankSpace) return false;
            else return true;
        }

        // Sprawdzenie, czy podane hasło zgadza się z hasłem dla danego użytkownika w danych logowania
        static bool IsPasswordCorrect(string username, string password)
        {
            bool isPasswordCorrect = false;
            try
            {
                Program.conn.Open();
                MySqlCommand command = new MySqlCommand("SELECT * FROM Users", Program.conn);
                using (var mr = command.ExecuteReader())
                {
                    while (mr.Read())
                    {
                        bool userFound = username.Equals(mr["Username"].ToString());
                        if (userFound)
                        {
                            isPasswordCorrect = password.Equals(mr["Password"].ToString());
                            break;
                        }
                    }
                }
                Program.conn.Close();
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            return isPasswordCorrect;
        }
    }
}
