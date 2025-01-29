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
            UserSettings,
            Diaries,
            Entries
        }
        public enum Options
        {
            Cancel = 0,
            Continue
        }
        static readonly HashSet<byte> startOptions = [1, 2, 3];
        static readonly HashSet<byte> menuOptions = [1, 2, 3, 4, 5];
        static readonly HashSet<byte> userSettingsOptions = [1, 2, 3, 4];
        static readonly HashSet<byte> diariesOptions = [1, 2, 3, 4];
        static readonly HashSet<byte> entriesOptions = [1, 2, 3, 4, 5];
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
                    // Wybór dziennika
                    case 1:
                        {
                            Options opt;
                            Diary.ChooseDiary(User.user, out opt);
                            if (opt == Options.Continue)
                                return Menu.Diaries;
                            else break;
                        }
                    // Szukanie innych użytkowników
                    case 2:
                        {
                            while (true)
                            {
                                Options opt;
                                UserDiary.ChooseUser(out opt);
                                if (opt == Options.Continue)
                                {
                                    while (opt == Options.Continue)
                                    {
                                        Options opt1;
                                        Diary.ChoosePublicDiary(UserDiary.chosenUser, out opt1);
                                        if (opt1 == Options.Continue)
                                        {
                                            while (opt1 == Options.Continue)
                                            {
                                                Options opt2;
                                                Entry.ChoosePublicEntry(Diary.chosenDiary, out opt2);
                                                if (opt2 == Options.Cancel)
                                                    opt1 = Options.Cancel;
                                                else
                                                    Entry.ReadEntry();
                                            }
                                        }
                                        else opt = Options.Cancel;
                                    }
                                }
                                else break;
                            }
                            break;
                        }
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
                Console.WriteLine("4. Cofnij");
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
                    // Usuwanie konta użytkownika
                    case 3:
                        {
                            Options opt;
                            User.DeleteUser(out opt);
                            if (opt == Options.Continue)
                                return Menu.Start;
                            else break;
                        }
                    // Cofnięcie do poprzedniego menu
                    case 4:
                        {
                            Console.Clear();
                            return Menu.Main;
                        }
                }
            }
        }
        // Menu dziennika
        public static Menu Diaries()
        {
            while (true)
            {
                Console.WriteLine("Wybrany dziennik: " + Diary.chosenDiary.Name);
                Console.WriteLine("Menu:");
                Console.WriteLine("1. Wybierz wpis");
                Console.WriteLine("2. Zmień nazwę");
                Console.WriteLine("3. Usuń dziennik");
                Console.WriteLine("4. Cofnij");
                byte option = MenuOptionIsValid(diariesOptions);

                switch (option)
                {
                    // Wybór wpisu
                    case 1:
                        {
                            Options opt;
                            Entry.ChooseEntry(Diary.chosenDiary, out opt);
                            if (opt == Options.Continue)
                                return Menu.Entries;
                            else break;
                        }
                    // Zmiana nazwy dziennika
                    case 2:
                        {
                            Options opt;
                            Diary.ChangeDiaryName(out opt);
                            if (opt == Options.Continue)
                                return Menu.Main;
                            else break;
                        }
                    // Usunięcie dziennika
                    case 3:
                        {
                            Options opt;
                            Diary.DeleteDiary(Diary.chosenDiary, out opt);
                            if (opt == Options.Continue)
                                return Menu.Main;
                            else break;
                        }
                    // Cofnięcie do poprzedniego menu
                    case 4:
                        {
                            Console.Clear();
                            return Menu.Main;
                        }
                }
            }
        }
        // Menu dziennika
        public static Menu Entries()
        {
            while (true)
            {
                Console.WriteLine("Wybrany wpis: " + Entry.chosenEntry.Title);
                Console.WriteLine("Menu:");
                Console.WriteLine("1. Przeczytaj");
                Console.WriteLine("2. Zmień tytuł");
                Console.WriteLine("3. Edytuj tekst");
                Console.WriteLine("4. Usuń wpis");
                Console.WriteLine("5. Cofnij");
                byte option = MenuOptionIsValid(entriesOptions);

                switch (option)
                {
                    // Wyświetlenie tekstu wpisu
                    case 1:
                        {
                            Entry.ReadEntry();
                            break;
                        }
                    // Zmiana tytułu wpisu
                    case 2:
                        {
                            Options opt;
                            Entry.ChangeEntryTitle(out opt);
                            if (opt == Options.Continue)
                                return Menu.Diaries;
                            else break;
                        }
                    // Edycja zawartości
                    case 3:
                        {
                            Entry.EditEntryContent(Entry.chosenEntry);
                            return Menu.Diaries;
                        }
                    // Usunięcie wpisu
                    case 4:
                        {
                            Options opt;
                            Entry.DeleteEntry(Entry.chosenEntry, out opt);
                            if (opt == Options.Continue)
                                return Menu.Diaries;
                            else break;
                        }
                    // Cofnięcie do poprzedniego menu
                    case 5:
                        {
                            Console.Clear();
                            return Menu.Diaries;
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
