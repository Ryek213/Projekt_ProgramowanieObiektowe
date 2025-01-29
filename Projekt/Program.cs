using System.Data;
using System.Linq;
using System.Text.Json;
using MySql.Data.MySqlClient;

namespace Projekt
{
    internal class Program
    {
        const string connectionString = "server=localhost;database=mydb;uid=root;";
        public static MySqlConnection conn = new MySqlConnection(connectionString);
        static void Main(string[] args)
        {
            Controls.Menu menu = Controls.Menu.Start;
            while (true)
            {
                switch (menu)
                {
                    case Controls.Menu.None:
                        {
                            return;
                        }
                    case Controls.Menu.Start:
                        {
                            menu = Controls.StartMenu();
                            break;
                        }
                    case Controls.Menu.Main:
                        {
                            menu = Controls.MainMenu();
                            break;
                        }
                    case Controls.Menu.UserSettings:
                        {
                            menu = Controls.UserSettingsMenu();
                            break;
                        }
                    case Controls.Menu.Diaries:
                        {
                            menu = Controls.Diaries();
                            break;
                        }
                    case Controls.Menu.Entries:
                        {
                            menu = Controls.Entries();
                            break;
                        }
                }
            }
        }
    }
}
