using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using MySql.Data.MySqlClient;

namespace Projekt
{
    internal class Diary(int ID_diary, string Name, string Username)
    {
        public static Diary? chosenDiary;
        public static List<Diary> diaryList = [];
        public int ID_diary { get; set; } = ID_diary;
        public string Name { get; set; } = Name;
        public string Username { get; set; } = Username;

        // Wybór dziennika z listy podanego użytkownika lub stwórz nowy
        public static void ChooseDiary(string username, out Controls.Options option)
        {
            Console.Clear();
            while (true)
            {
                // Załaduj dzienniki użytkownika
                LoadDiaries(username);

                // Załaduj opcje wyboru
                HashSet<int> options = [0, 1];
                for (int i = 2; i < diaryList.Count + 2; i++) options.Add(i);

                // Załaduj opcje do menu
                string menu = "Menu:\n";
                foreach (int i in options)
                {
                    if (i == 0) menu += "0. Cofnij\n";
                    else if (i == 1) menu += "1. Nowy dziennik\n---------------------------------\n";
                    else menu += i.ToString() + ". " + diaryList[i - 2].Name + "\n";
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
                            CreateDiary(User.user);
                            break;
                        }
                    default:
                        {
                            Console.Clear();
                            chosenDiary = diaryList[userOption - 2];
                            option = Controls.Options.Continue;
                            return;
                        }
                }
            }
        }
        // Tworzenie dziennika
        public static void CreateDiary(string username)
        {
            Console.Clear();
            while (true)
            {
                string diaryName;

                // Podanie nazwy nowego dziennika
                while (true)
                {
                    Console.WriteLine("Podaj nazwę nowego dziennika");
                    Console.WriteLine("Wpisz [0], aby anulować");
                    Console.Write("> ");
                    diaryName = Console.ReadLine();
                    if (diaryName == "0")
                    {
                        Console.Clear();
                        Console.WriteLine("Anulowano tworzenie nowego dziennika");
                        return;
                    }
                    if (diaryName.Length < 1)
                    {
                        Console.Clear();
                        Console.WriteLine("Nazwa nie może być pusta");
                        continue;
                    }
                    if(diaryName.Length != diaryName.Trim().Length)
                    {
                        Console.Clear();
                        Console.WriteLine("Nie może być pustych znaków na początku i końcu nazwy");
                    }
                    break;
                }

                // Sprawdzenie, czy dziennik o takiej nazwie już istnieje dla bieżącego użytkownika
                bool czyIstnieje = false;
                try
                {
                    Program.conn.Open();
                    MySqlCommand command = new MySqlCommand("SELECT Name FROM Diaries WHERE Username = @username", Program.conn);
                    command.Parameters.AddWithValue("@username", username);
                    using (var mr = command.ExecuteReader())
                    {
                        while (mr.Read())
                        {
                            if(mr.GetString(0).Equals(diaryName))
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

                // Jeśli nie istnieje to wstaw, w przeciwnym wypadku ponów
                if(!czyIstnieje)
                {
                    try
                    {
                        Program.conn.Open();

                        MySqlCommand command = new MySqlCommand("INSERT INTO Diaries (Name, Username) VALUES (@name, @username)", Program.conn);
                        command.Parameters.AddWithValue("@name", diaryName);
                        command.Parameters.AddWithValue("@username", username);
                        command.ExecuteNonQuery();

                        Program.conn.Close();
                    }
                    catch (MySqlException e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("Dziennik o podanej nazwie już istnieje");
                    continue;
                }
                break;
            }
        }
        // Zmiana nazwy dziennika
        public static void ChangeDiaryName(out Controls.Options option)
        {
            Console.Clear();
            while (true)
            {
                string diaryName;

                // Ustawienie nowej nazwy dziennika
                while (true)
                {
                    Console.WriteLine("Podaj nową nazwę dziennika");
                    Console.WriteLine("Wpisz [0], aby anulować");
                    Console.Write("> ");
                    diaryName = Console.ReadLine();
                    if (diaryName == "0")
                    {
                        Console.Clear();
                        Console.WriteLine("Anulowano zmianę nazwy dziennika");
                        option = Controls.Options.Cancel;
                        return;
                    }
                    if (diaryName.Length < 1)
                    {
                        Console.Clear();
                        Console.WriteLine("Nazwa nie może być pusta");
                        continue;
                    }
                    if(diaryName == chosenDiary.Name)
                    {
                        Console.Clear();
                        Console.WriteLine("Nazwa nie może być taka sama");
                        continue;
                    }
                    if (diaryName.Length != diaryName.Trim().Length)
                    {
                        Console.Clear();
                        Console.WriteLine("Nie może być pustych znaków na początku i końcu nazwy");
                    }
                    break;
                }

                // Sprawdzenie, czy dziennik o takiej nazwie już istnieje dla bieżącego użytkownika
                bool czyIstnieje = false;
                try
                {
                    Program.conn.Open();
                    MySqlCommand command = new MySqlCommand("SELECT Name FROM Diaries WHERE Username = @username", Program.conn);
                    command.Parameters.AddWithValue("@username", User.user);
                    using (var mr = command.ExecuteReader())
                    {
                        while (mr.Read())
                        {
                            if (mr.GetString(0).Equals(diaryName))
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

                        MySqlCommand command = new MySqlCommand("UPDATE Diaries SET Name = @name WHERE ID_diary = @id", Program.conn);
                        command.Parameters.AddWithValue("@name", diaryName);
                        command.Parameters.AddWithValue("@id", chosenDiary.ID_diary);
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
                    Console.WriteLine("Dziennik o podanej nazwie już istnieje");
                    continue;
                }
            }
        }
        // Usunięcie dziennika
        public static void DeleteDiary(Diary diary, out Controls.Options option)
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
                    Console.WriteLine("Anulowano usunięcie dziennika");
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

            // Usunięcie dziennika z bazy
            try
            {
                Program.conn.Open();
                MySqlCommand command = new MySqlCommand("DELETE FROM Diaries WHERE ID_diary = @id", Program.conn);
                command.Parameters.AddWithValue("@id", diary.ID_diary);
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
        // Wybór dziennika użytkownika z widocznością publiczną
        public static void ChoosePublicDiary(string username, out Controls.Options option)
        {
            Console.Clear();
            while (true)
            {
                // Załaduj dzienniki użytkownika
                LoadDiaries(username);

                // Załaduj opcje wyboru
                HashSet<int> options = [0];
                for (int i = 1; i < diaryList.Count + 1; i++) options.Add(i);

                // Załaduj opcje do menu
                string menu = "Wybór dziennika\nMenu:\n";
                foreach (int i in options)
                {
                    if (i == 0) menu += "0. Cofnij\n---------------------------------\n";
                    else menu += i.ToString() + ". " + diaryList[i - 1].Name + "\n";
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
                            chosenDiary = diaryList[userOption - 1];
                            option = Controls.Options.Continue;
                            return;
                        }
                }
            }
        }
        // Ładowanie dzienników podanego użytkownika do listy
        static void LoadDiaries(string username)
        {
            diaryList.Clear();
            try
            {
                Program.conn.Open();
                MySqlCommand command = new MySqlCommand("SELECT * FROM Diaries WHERE Username = @username", Program.conn);
                command.Parameters.AddWithValue("@username", username);
                using (var mr = command.ExecuteReader())
                {
                    while (mr.Read())
                    {
                        diaryList.Add(new Diary(mr.GetInt32(0), mr.GetString(1), mr.GetString(2)));
                    }
                }
                
                Program.conn.Close();
            }
            catch (MySqlException e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
