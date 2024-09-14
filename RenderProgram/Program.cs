namespace RenderProgram
{
    internal class Program
    {
        private static bool IsRunning { get; set; } = true;
        private static int CurrentMenu { get; set; } = 0;
        private static AppSettings Settings { get; set; }
        static void Main(string[] args)
        {
            while (IsRunning)
            {
                WriteMenu(CurrentMenu);
                string? input = Console.ReadLine();

                switch (input)
                {
                    //menu
                }
            }
        }
        private static void WriteMenu(int option)
        {
            Console.Clear();

            var selectedMenu = Settings.Menus.FirstOrDefault(menu => menu.Id == option);

            // Print the header if it exists
            if (selectedMenu.Text?.Header != null)
            {
                Console.WriteLine(selectedMenu.Text.Header);
            }

            // Print the options if they exist
            if (selectedMenu.Text?.Options != null)
            {
                foreach (var menuOption in selectedMenu.Text.Options)
                {
                    Console.WriteLine($"{menuOption.Id}: {menuOption.Text}");
                }
            }
        }
    }
}
