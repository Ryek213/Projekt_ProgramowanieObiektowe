using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekt
{
    internal class Controls
    {
        public enum Menu
        {
            None = 0,
            Start,
            Main,
            UserSettings
        }
        public enum Options
        {
            Cancel = 0,
            Continue
        }
        static HashSet<byte> startOptions = new HashSet<byte> { 1, 2, 3 };
        static HashSet<byte> menuOptions = new HashSet<byte> { 1, 2, 3, 4, 5 };
        static HashSet<byte> userSettingsOptions = new HashSet<byte> { 1, 2, 3, 4 };
        // Menu Start
        public static Menu StartMenu() {
            while (true)
            {
                // Wybieranie opcji menu startowego
                Console.WriteLine("Menu Start:");
                Console.WriteLine("1. Zaloguj się");
                Console.WriteLine("2. Zarejestruj się");
                Console.WriteLine("3. Wyjście");
                byte option = MenuOptionIsValid(startOptions);

                switch (option)
                {
                    // Logowanie
                    case 1:
                        {
                            User.SignIn();
                            return Menu.Main;
                        }
                    // Rejestracja
                    case 2:
                        {
                            User.SignUp();
                            return Menu.Start;
                        }
                    // Zakończenie programu
                    case 3:
                        {
                            ExitProgram(User.user);
                            return Menu.None;
                        }
                }
            }
        }
        // Menu Główne
        public static Menu MainMenu()
        {
            while (true)
            {
                Console.WriteLine("Menu:");
                Console.WriteLine("1. Wybierz dziennik");
                Console.WriteLine("2. Znajdź innych użytkowników");
                Console.WriteLine("--------------------------");
                Console.WriteLine("3. Ustawienia konta");
                Console.WriteLine("4. Wyloguj się");
                Console.WriteLine("5. Wyjście");
                byte option = MenuOptionIsValid(menuOptions);

                switch (option)
                {
                    // Zarządzanie kontem
                    case 3:
                        {
                            Console.Clear();
                            return Menu.UserSettings;
                        }
                    // Wylogowywanie
                    case 4:
                        {
                            User.SignOut();
                            return Menu.Start;
                        }
                    // Zakończenie programu
                    case 5:
                        {
                            ExitProgram(User.user);
                            return Menu.None;
                        }
                }
            }
        }
        // Menu zarządzania kontem użytkownika
        public static Menu UserSettingsMenu()
        {
            while (true)
            {
                Console.WriteLine("Menu:");
                Console.WriteLine("1. Zmień nazwę");
                Console.WriteLine("2. Zmień hasło");
                Console.WriteLine("3. Usuń konto");
                Console.WriteLine("4. Wyjście");
                byte option = MenuOptionIsValid(userSettingsOptions);

                switch (option)
                {
                    // Zmiana nazwy użytkownika
                    case 1:
                        {
                            Options opt;
                            User.ChangeUsername(out opt);
                            if (opt == Options.Continue)
                                return Menu.Start;
                            else break;
                        }
                    // Zmiana hasła użytkownika
                    case 2:
                        {
                            Options opt;
                            User.ChangePassword(out opt);
                            if (opt == Options.Continue)
                                return Menu.Start;
                            else break;
                        }
                    case 3:
                        {
                            Options opt;
                            User.DeleteUser(out opt);
                            if (opt == Options.Continue)
                                return Menu.Start;
                            else break;
                        }
                    // Zakończenie programu
                    case 4:
                        {
                            ExitProgram(User.user);
                            return Menu.None;
                        }
                }
            }
        }
        static byte MenuOptionIsValid(HashSet<byte> options)
        {
            byte option;
            bool isNumber;
            bool inOptions;
            do
            {
                Console.Write("> ");
                isNumber = byte.TryParse(Console.ReadLine(), out option);
                inOptions = options.Contains(option);
            }
            while (!isNumber || !inOptions);

            return option;
        }

        static void ExitProgram(string? user)
        {
            if (user != null) Console.WriteLine("Do zobaczenia, {0}!", user);
            else Console.WriteLine("Do zobaczenia!");
        }
    }
}
