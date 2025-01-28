using System.Data;
using System.Linq;
using System.Text.Json;

namespace Projekt
{
    internal class Program
    {
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

                }
            }
        }
    }
}
