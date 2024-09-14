﻿namespace RenderProgram
{
    internal class Program
    {
        private static bool IsRunning { get; set; } = true;
        private static string AppSettingsFilePath { get; } = "AppSettings.json";
        private static Menu CurrentMenu { get; set; }
        private static AppSettings Settings { get; set; }
        static void Main(string[] args)
        {
            Settings = JSONDeserializer.ReadSettings(AppSettingsFilePath);
            ChangeMenu(0);

            while (IsRunning)
            {
                WriteMenu(CurrentMenu);
                int input = int.Parse(Console.ReadLine());

                if (CurrentMenu.Name == "Main Menu")
                {
                    switch (input)
                    {
                        case 0: IsRunning = false; break;

                        case 1: Render.Run(); break;

                        default: ChangeMenu(CurrentMenu.Text.Options.FirstOrDefault(menu => menu.Id == input).MenuSwitch); break;
                    }
                }
            }
        }
        private static void WriteMenu(Menu menu)
        {
            Console.Clear();

            // Print the header if it exists
            if (menu.Text?.Header != null)
            {
                Console.WriteLine(menu.Text.Header);
            }

            // Print the options if they exist
            if (menu.Text?.Options != null)
            {
                foreach (var menuOption in menu.Text.Options)
                {
                    Console.WriteLine($"{menuOption.Id}: {menuOption.Text}");
                }
            }
        }

        public static void ChangeMenu(int menuId)
        {
            CurrentMenu = Settings.Menus.FirstOrDefault(menu => menu.Id == menuId);
        }
    }
}
