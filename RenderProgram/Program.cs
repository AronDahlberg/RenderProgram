using System.Reflection;

namespace RenderProgram
{
    internal class Program
    {
        private static bool IsRunning { get; set; } = true;
        private static string AppSettingsFilePath { get; } = "AppSettings.json";
        public static string MenuNamespace { get; } = "RenderProgram.MenuClasses";
        public static string ShapeNamespace { get; } = "RenderProgram.ShapeClasses";
        public static AppSettings Settings { get; set; }
        public static Menu CurrentMenu { get; set; }
        public static object CurrentShape { get; set; }
        static void Main(string[] args)
        {
            Settings = JSONDeserializer.ReadSettings(AppSettingsFilePath);

            // Init default values for Current<thing> properties
            ChangeMenu(0);
            ChangeShape("Torus");

            while (IsRunning)
            {
                string menuClass = $"{MenuNamespace}.{CurrentMenu.Name}";

                WriteMenu(CurrentMenu, menuClass);

                string input = Console.ReadLine();

                try
                {
                    ExecuteMenuAction(CurrentMenu, menuClass, input);
                }
                // Ignoring all menu related exceptions (All are related to user input error)
                catch (FormatException){ continue; }
                catch (NullReferenceException) { continue; }
                catch (ArgumentNullException) { continue; }
                catch (IndexOutOfRangeException) { continue; }
            }

            Console.Clear();
        }
        private static void WriteMenu(Menu menu, string menuClass)
        {
            Console.Clear();

            // Print the options if they exist
            if (menu.Text?.Options != null)
            {
                foreach (var menuOption in menu.Text.Options)
                {
                    Console.WriteLine($"{menuOption.Id}: {menuOption.Text}");
                }
            }

            // Print anything else if it exists
            MethodInfo executeMethod = Type.GetType(menuClass).GetMethod("PrintMenu", BindingFlags.Public | BindingFlags.Static);

            if (executeMethod != null)
            { 
                executeMethod.Invoke(null, null); 
            }
        }
        private static void ExecuteMenuAction(Menu menu, string menuClass, string input)
        {
            try
            {
                // Find and invoke action method of menu
                MethodInfo executeMethod = Type.GetType(menuClass).GetMethod("ExecuteMenuAction", BindingFlags.Public | BindingFlags.Static);
                executeMethod.Invoke(null, new object[] { input });
            }
            catch (TargetInvocationException tiex)
            {
                throw tiex.InnerException;
            }
        }
        public static void ChangeMenu(int menuId)
        {
            CurrentMenu = Settings.Menus.FirstOrDefault(menu => menu.Id == menuId);
        }

        public static void ChangeShape(string shapeName)
        {
            CurrentShape = Activator.CreateInstance(Type.GetType($"{ShapeNamespace}.{shapeName}"));
        }

        public static void Exit()
        {
            IsRunning = false;
        }
    }
}
