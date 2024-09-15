using System.Reflection;

namespace RenderProgram
{
    internal class Program
    {
        private static bool IsRunning { get; set; } = true;
        private static string AppSettingsFilePath { get; } = "AppSettings.json";
        public static AppSettings Settings { get; set; }
        public static Menu CurrentMenu { get; set; }
        public static Shape CurrentShape { get; set; }
        static void Main(string[] args)
        {
            Settings = JSONDeserializer.ReadSettings(AppSettingsFilePath);
            ChangeMenu(0);
            ChangeShape(1);

            while (IsRunning)
            {
                WriteMenu(CurrentMenu);

                int input = int.Parse(Console.ReadLine());

                ExecuteMenuAction(CurrentMenu, input);
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

            // Print anything else if it exists
            string className = $"RenderProgram.MenuClasses.{menu}";
            MethodInfo executeMethod = Type.GetType(className).GetMethod("ExecuteMenuAction", BindingFlags.Public | BindingFlags.Static);

            if (executeMethod != null)
            { 
                executeMethod.Invoke(null, null); 
            }
        }
        private static void ExecuteMenuAction(Menu menu, int input)
        {
            string className = $"RenderProgram.MenuClasses.{menu}";
            MethodInfo executeMethod = Type.GetType(className).GetMethod("ExecuteMenuAction", BindingFlags.Public | BindingFlags.Static);
            executeMethod.Invoke(null, new object[] { input });
        }
        public static void ChangeMenu(int menuId)
        {
            CurrentMenu = Settings.Menus.FirstOrDefault(menu => menu.Id == menuId);
        }

        public static void ChangeShape(int shapeId)
        {
            CurrentShape = Settings.Shapes.FirstOrDefault(shape => shape.Id == shapeId);
        }

        public static void Exit()
        {
            IsRunning = false;
        }
    }
}
