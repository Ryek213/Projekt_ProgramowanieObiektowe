using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using MySql.Data.MySqlClient;

namespace Projekt
{
    internal class Entry(int ID_entry, string Title, string Content, int ID_diary)
    {
        public static Entry? chosenEntry;
        public static List<Entry> entryList = [];
        public int ID_entry { get; set; } = ID_entry;
        public string Title { get; set; } = Title;
        public string Content { get; set; } = Content;
        public int ID_diary { get; set; } = ID_diary;

        // Wybór wpisu z listy podanego użytkownika lub stwórz nowy
        public static void ChooseEntry(Diary diary, out Controls.Options option)
        {
            Console.Clear();
            while (true)
            {
                // Załaduj wpisy użytkownika
                LoadEntries(diary);

                // Załaduj opcje wyboru
                HashSet<int> options = [0, 1];
                for (int i = 2; i < entryList.Count + 2; i++) options.Add(i);

                // Załaduj opcje do menu
                string menu = "Menu:\n";
                foreach (int i in options)
                {
                    if (i == 0) menu += "0. Cofnij\n";
                    else if (i == 1) menu += "1. Nowy wpis\n---------------------------------\n";
                    else menu += i.ToString() + ". " + entryList[i - 2].Title + "\n";
                }

                // Pobierz opcję od użytkownika
                int userOption;
                bool isNumber;
                bool inOptions;
                do
                {
                    Console.Write(menu);
                    Console.Write("> ");
                    isNumber = int.TryParse(Console.ReadLine(), out userOption);
                    inOptions = options.Contains(userOption);
                }
                while (!isNumber || !inOptions);

                switch (userOption)
                {
                    case 0:
                        {
                            Console.Clear();
                            Console.WriteLine("Anulowano wybór dziennika");
                            option = Controls.Options.Cancel;
                            return;
                        }
                    case 1:
                        {
                            CreateEntry(diary);
                            break;
                        }
                    default:
                        {
                            Console.Clear();
                            chosenEntry = entryList[userOption - 2];
                            option = Controls.Options.Continue;
                            return;
                        }
                }
            }
        }
        // Tworzenie wpisu
        public static void CreateEntry(Diary diary)
        {
            Console.Clear();

            // Podanie tytułu nowego wpisu
            string entryTitle;

            while (true)
            {
                Console.WriteLine("Podaj tytuł wpisu");
                Console.WriteLine("Wpisz [0], aby anulować");
                Console.Write("> ");
                entryTitle = Console.ReadLine();
                if (entryTitle == "0")
                {
                    Console.Clear();
                    Console.WriteLine("Anulowano tworzenie nowego wpisu");
                    return;
                }
                if (entryTitle.Length < 1)
                {
                    Console.Clear();
                    Console.WriteLine("Tytuł nie może być pusty");
                    continue;
                }
                if (entryTitle.Length != entryTitle.Trim().Length)
                {
                    Console.Clear();
                    Console.WriteLine("Nie może być pustych znaków na początku i końcu tytułu");
                }
                break;
            }

            // Podanie zawartości wpisu
            Console.WriteLine("--------------------------");
            Console.WriteLine("Zawartość:");
            string entryContent = TextEditor("");

            // Wstaw wpis do bazy
            try
            {
                Program.conn.Open();

                MySqlCommand command = new MySqlCommand("INSERT INTO DiaryEntries (Title, Content, ID_diary) VALUES (@title, @content, @id)", Program.conn);
                command.Parameters.AddWithValue("@title", entryTitle);
                command.Parameters.AddWithValue("@content", entryContent);
                command.Parameters.AddWithValue("@id", diary.ID_diary);
                command.ExecuteNonQuery();

                Program.conn.Close();
            }
            catch (MySqlException e)
            {
                Console.WriteLine(e.Message);
            }

            Console.Clear();
        }
        // Wyświetlenie tekstu wpisu
        public static void ReadEntry()
        {
            Console.Clear();

            // Wyświetlenie tekstu
            Console.WriteLine(chosenEntry.Content);

            Console.WriteLine("------------------------");
            Console.Write("Wciśnij, aby kontynuować");
            Console.ReadLine();
            Console.Clear();
        }
        // Zmiana tytułu wpisu
        public static void ChangeEntryTitle(out Controls.Options option)
        {
            Console.Clear();
            while (true)
            {
                string entryTitle;

                // Ustawienie nowego tytułu wpisu
                while (true)
                {
                    Console.WriteLine("Podaj nowy tytuł wpisu");
                    Console.WriteLine("Wpisz [0], aby anulować");
                    Console.Write("> ");
                    entryTitle = Console.ReadLine();
                    if (entryTitle == "0")
                    {
                        Console.Clear();
                        Console.WriteLine("Anulowano zmianę tytułu wpisu");
                        option = Controls.Options.Cancel;
                        return;
                    }
                    if (entryTitle.Length < 1)
                    {
                        Console.Clear();
                        Console.WriteLine("Tytuł nie może być pusty");
                        continue;
                    }
                    if (entryTitle == chosenEntry.Title)
                    {
                        Console.Clear();
                        Console.WriteLine("Tytuł nie może być taki sam");
                        continue;
                    }
                    if (entryTitle.Length != entryTitle.Trim().Length)
                    {
                        Console.Clear();
                        Console.WriteLine("Nie może być pustych znaków na początku i końcu tytułu");
                    }
                    break;
                }

                // Sprawdzenie, czy wpis o takim tytule już istnieje dla bieżącego dziennika
                bool czyIstnieje = false;
                try
                {
                    Program.conn.Open();
                    MySqlCommand command = new MySqlCommand("SELECT Title FROM DiaryEntries WHERE ID_diary = @id", Program.conn);
                    command.Parameters.AddWithValue("@id", Diary.chosenDiary.ID_diary);
                    using (var mr = command.ExecuteReader())
                    {
                        while (mr.Read())
                        {
                            if (mr.GetString(0).Equals(entryTitle))
                            {
                                czyIstnieje = true;
                                break;
                            }
                        }
                    }
                    Program.conn.Close();
                }
                catch (MySqlException e)
                {
                    Console.WriteLine(e.Message);
                }

                // Jeśli nie istnieje to zmień, w przeciwnym wypadku ponów
                if (!czyIstnieje)
                {
                    try
                    {
                        Program.conn.Open();

                        MySqlCommand command = new MySqlCommand("UPDATE DiaryEntries SET Title = @title WHERE ID_entry = @id", Program.conn);
                        command.Parameters.AddWithValue("@title", entryTitle);
                        command.Parameters.AddWithValue("@id", chosenEntry.ID_entry);
                        command.ExecuteNonQuery();

                        Program.conn.Close();
                    }
                    catch (MySqlException e)
                    {
                        Console.WriteLine(e.Message);
                    }

                    Console.Clear();
                    option = Controls.Options.Continue;
                    return;
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("Wpis o podanym tytule już istnieje");
                    continue;
                }
            }
        }
        // Edycja zawartości wpisu
        public static void EditEntryContent(Entry entry)
        {
            Console.Clear();
            // Edycja zawartości
            Console.WriteLine("Edycja:");
            string content = TextEditor(entry.Content);

            // Aktualizacja zawartości w bazie
            try
            {
                Program.conn.Open();

                MySqlCommand command = new MySqlCommand("UPDATE DiaryEntries SET Content = @content WHERE ID_entry = @id", Program.conn);
                command.Parameters.AddWithValue("@content", content);
                command.Parameters.AddWithValue("@id", entry.ID_entry);
                command.ExecuteNonQuery();

                Program.conn.Close();
            }
            catch (MySqlException e)
            {
                Console.WriteLine(e.Message);
            }

            Console.Clear();
            return;
        }
        // Usunięcie wpisu
        public static void DeleteEntry(Entry entry, out Controls.Options option)
        {
            Console.Clear();

            // Potwierdzenie usunięcia wpisaniem 'Potwierdzam'
            while (true)
            {
                Console.WriteLine("Aby kontynuować, wpisz 'Potwierdzam'");
                Console.WriteLine("Wpisz [0], aby anulować");
                Console.Write("> ");
                string input = Console.ReadLine();

                if (input == "0")
                {
                    Console.Clear();
                    Console.WriteLine("Anulowano usunięcie wpisu");
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

            // Usunięcie wpisu z bazy
            try
            {
                Program.conn.Open();
                MySqlCommand command = new MySqlCommand("DELETE FROM DiaryEntries WHERE ID_entry = @id", Program.conn);
                command.Parameters.AddWithValue("@id", entry.ID_entry);
                command.ExecuteNonQuery();
                Program.conn.Close();
            }
            catch (MySqlException e)
            {
                Console.WriteLine(e.Message);
            }

            option = Controls.Options.Continue;
            Console.Clear();
        }
        // Wybór wpisu z dziennika publicznego użytkownika
        public static void ChoosePublicEntry(Diary diary, out Controls.Options option)
        {
            Console.Clear();
            while (true)
            {
                // Załaduj wpisy użytkownika
                LoadEntries(diary);

                // Załaduj opcje wyboru
                HashSet<int> options = [0];
                for (int i = 1; i < entryList.Count + 1; i++) options.Add(i);

                // Załaduj opcje do menu
                string menu = "Wybór wpisu\nMenu:\n";
                foreach (int i in options)
                {
                    if (i == 0) menu += "0. Cofnij\n---------------------------------\n";
                    else menu += i.ToString() + ". " + entryList[i - 1].Title + "\n";
                }

                // Pobierz opcję od użytkownika
                int userOption;
                bool isNumber;
                bool inOptions;
                do
                {
                    Console.Write(menu);
                    Console.Write("> ");
                    isNumber = int.TryParse(Console.ReadLine(), out userOption);
                    inOptions = options.Contains(userOption);
                }
                while (!isNumber || !inOptions);

                switch (userOption)
                {
                    case 0:
                        {
                            Console.Clear();
                            Console.WriteLine("Anulowano wybór dziennika");
                            option = Controls.Options.Cancel;
                            return;
                        }
                    default:
                        {
                            Console.Clear();
                            chosenEntry = entryList[userOption - 1];
                            option = Controls.Options.Continue;
                            return;
                        }
                }
            }
        }
        // Ładowanie wpisów danego dziennika do listy
        static void LoadEntries(Diary diary)
        {
            entryList.Clear();
            try
            {
                Program.conn.Open();
                MySqlCommand command = new MySqlCommand("SELECT * FROM DiaryEntries WHERE ID_diary = @id", Program.conn);
                command.Parameters.AddWithValue("@id", diary.ID_diary);
                using (var mr = command.ExecuteReader())
                {
                    while (mr.Read())
                    {
                        entryList.Add(new Entry(mr.GetInt32("ID_entry"), mr.GetString("Title"), mr.GetString("Content"), mr.GetInt32("ID_diary")));
                    }
                }

                Program.conn.Close();
            }
            catch (MySqlException e)
            {
                Console.WriteLine(e.Message);
            }
        }
        // Metoda pozwalająca edytować tekst wypisany przez konsolę
        static string TextEditor(string startingText)
        {
            Console.Write(startingText);

            string[] input = [startingText, ""];
            while (true)
            {
                // Wczytaj klawisz
                var key = Console.ReadKey(true);

                // Enter
                if (key.Key == ConsoleKey.Enter)
                {
                    Console.WriteLine();
                    break;
                }
                // Backspace
                else if (key.Key == ConsoleKey.Backspace)
                {
                    if (Console.CursorLeft == 0)
                        continue;

                    input[0] = string.Join("", input[0].Take(input[0].Length - 1));

                    Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                    for (int i = 0; i < input[1].Length + 1; i++) Console.Write(" ");
                    Console.SetCursorPosition(Console.CursorLeft - input[1].Length - 1, Console.CursorTop);
                    Console.Write(input[1]);
                    Console.SetCursorPosition(Console.CursorLeft - input[1].Length, Console.CursorTop);
                }
                // Delete
                else if (key.Key == ConsoleKey.Delete)
                {
                    if (Console.CursorLeft == (input[0].Length + input[1].Length))
                        continue;

                    input[1] = input[1].Substring(1);

                    for (int i = 0; i < input[1].Length + 1; i++) Console.Write(" ");
                    Console.SetCursorPosition(Console.CursorLeft - input[1].Length - 1, Console.CursorTop);
                    Console.Write(input[1]);
                    Console.SetCursorPosition(Console.CursorLeft - input[1].Length, Console.CursorTop);
                }
                // Strzałka w lewo
                else if (key.Key == ConsoleKey.LeftArrow)
                {
                    if (Console.CursorLeft == 0)
                        continue;
                    Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                    input[1] = input[0][input[0].Length - 1] + input[1];
                    input[0] = string.Join("", input[0].Take(input[0].Length - 1));
                }
                // Strzałka w prawo
                else if (key.Key == ConsoleKey.RightArrow)
                {
                    if (Console.CursorLeft == (input[0].Length + input[1].Length))
                        continue;
                    Console.SetCursorPosition(Console.CursorLeft + 1, Console.CursorTop);
                    input[0] = input[0] + input[1][0];
                    input[1] = input[1].Substring(1);
                }
                // Dozwolone znaki do wpisywania
                else if (char.IsLetterOrDigit(key.KeyChar) || char.IsPunctuation(key.KeyChar) || char.IsSymbol(key.KeyChar) || char.IsWhiteSpace(key.KeyChar))
                {
                    input[0] += key.KeyChar.ToString();
                    Console.Write(key.KeyChar);
                    for (int i = 0; i < input[1].Length; i++) Console.Write(" ");
                    Console.SetCursorPosition(Console.CursorLeft - input[1].Length, Console.CursorTop);
                    Console.Write(input[1]);
                    Console.SetCursorPosition(Console.CursorLeft - input[1].Length, Console.CursorTop);
                }
            }

            string output = input[0] + input[1];
            return output;
        }
    }
}
