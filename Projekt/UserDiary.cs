using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace Projekt
{
    internal class UserDiary
    {
        public static string? chosenUser;
        public static List<string> publicUserList = [];

        // Wybór użytkownika
        public static void ChooseUser(out Controls.Options option)
        {
            Console.Clear();

            // Załaduj publicznych użytkowników
            publicUserList.Clear();
            try
            {
                Program.conn.Open();
                MySqlCommand command = new MySqlCommand("SELECT Username FROM Users WHERE Public = 1 AND Username != @user", Program.conn);
                command.Parameters.AddWithValue("@user", User.user);
                using (var mr = command.ExecuteReader())
                {
                    while (mr.Read())
                    {
                        publicUserList.Add(mr.GetString(0));
                    }
                }

                Program.conn.Close();
            }
            catch (MySqlException e)
            {
                Console.WriteLine(e.Message);
            }

            // Załaduj opcje wyboru
            HashSet<int> options = [0];
            for (int i = 1; i < publicUserList.Count + 1; i++) options.Add(i);

            // Załaduj opcje do menu
            string menu = "Wybór użytkownika\nMenu:\n";
            foreach (int i in options)
            {
                if (i == 0) menu += "0. Cofnij\n-----------------------------------\n";
                else menu += i.ToString() + ". " + publicUserList[i - 1] + "\n";
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
                        Console.WriteLine("Anulowano wybór użytkownika");
                        option = Controls.Options.Cancel;
                        return;
                    }
                default:
                    {
                        Console.Clear();
                        chosenUser = publicUserList[userOption - 1];
                        option = Controls.Options.Continue;
                        return;
                    }
            }
        }
    }
}
