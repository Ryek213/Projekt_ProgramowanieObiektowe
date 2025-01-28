﻿using System.Linq.Expressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using MySql.Data.MySqlClient;

namespace Test
{
    internal class Program
    {

        static void Main(string[] args)
        {
            string connectionString = "server=localhost;database=mydb;uid=root;";
            try
            {
                MySqlConnection conn = new MySqlConnection(connectionString);
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("SELECT * FROM Users", conn);
                using (var myReader = cmd.ExecuteReader())
                {
                    while (myReader.Read())
                    {
                        var user = myReader.GetString(0);
                        var pass = myReader.GetString(1);
                        Console.WriteLine(user + " " + pass);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }






















            //Console.WriteLine("Type the IP Address: ");
            //var defaultIP = "192.168.0.190";
            //Console.Write(defaultIP);

            //string[] input = new string[2];
            //input[0] = defaultIP;
            //input[1] = "";
            //while (true)
            //{
            //    // Wczytaj klawisz
            //    var key = Console.ReadKey(true);

            //    // Enter
            //    if (key.Key == ConsoleKey.Enter)
            //    {
            //        Console.WriteLine();
            //        break;
            //    }
            //    // Backspace
            //    else if (key.Key == ConsoleKey.Backspace)
            //    {
            //        if (Console.CursorLeft == 0)
            //            continue;

            //        input[0] = string.Join("", input[0].Take(input[0].Length - 1));

            //        Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
            //        for (int i = 0; i < input[1].Length+1; i++) Console.Write(" ");
            //        Console.SetCursorPosition(Console.CursorLeft - input[1].Length - 1, Console.CursorTop);
            //        Console.Write(input[1]);
            //        Console.SetCursorPosition(Console.CursorLeft - input[1].Length, Console.CursorTop);
            //    }
            //    // Delete
            //    else if (key.Key == ConsoleKey.Delete)
            //    {
            //        if (Console.CursorLeft == (input[0].Length + input[1].Length))
            //            continue;

            //        input[1] = input[1].Substring(1);

            //        for (int i = 0; i < input[1].Length + 1; i++) Console.Write(" ");
            //        Console.SetCursorPosition(Console.CursorLeft - input[1].Length - 1, Console.CursorTop);
            //        Console.Write(input[1]);
            //        Console.SetCursorPosition(Console.CursorLeft - input[1].Length, Console.CursorTop);
            //    }
            //    // Strzałka w lewo
            //    else if (key.Key == ConsoleKey.LeftArrow)
            //    {
            //        if (Console.CursorLeft == 0)
            //            continue;
            //        Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
            //        input[1] = input[0][input[0].Length - 1] + input[1];
            //        input[0] = string.Join("", input[0].Take(input[0].Length - 1));
            //    }
            //    // Strzałka w prawo
            //    else if (key.Key == ConsoleKey.RightArrow)
            //    {
            //        if (Console.CursorLeft == (input[0].Length + input[1].Length))
            //            continue;
            //        Console.SetCursorPosition(Console.CursorLeft + 1, Console.CursorTop);
            //        input[0] = input[0] + input[1][0];
            //        input[1] = input[1].Substring(1);
            //    }
            //    // Dozwolone znaki do wpisywania
            //    else if (char.IsLetterOrDigit(key.KeyChar) || char.IsPunctuation(key.KeyChar) || char.IsSymbol(key.KeyChar) || char.IsWhiteSpace(key.KeyChar))
            //    {
            //        input[0] += key.KeyChar.ToString();
            //        Console.Write(key.KeyChar);
            //        for (int i = 0; i < input[1].Length; i++) Console.Write(" ");
            //        Console.SetCursorPosition(Console.CursorLeft - input[1].Length, Console.CursorTop);
            //        Console.Write(input[1]);
            //        Console.SetCursorPosition(Console.CursorLeft - input[1].Length, Console.CursorTop);
            //    }
            //}

            //string output = input[0] + input[1];
            //Console.WriteLine("You entered: " + output);
            //Console.ReadLine();
        }
    }
}
