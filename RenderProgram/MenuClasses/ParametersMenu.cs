using System.Reflection;

namespace RenderProgram.MenuClasses
{
    internal static class ParametersMenu
    {
        public static void PrintMenu()
        {
            int generalParametersLenght = Program.Settings.GeneralParameters.Parameters.Count;

            Console.WriteLine("r: Return to main menu");

            // Write all non-object specific paramaters
            foreach (var parameter in Program.Settings.GeneralParameters.Parameters)
            {
                Console.WriteLine($"{parameter.Id}: {parameter.Name} = {parameter.Value}");
            }

            // Find and write all object specific paramaters
            PropertyInfo[] properties = Program.CurrentShape.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            for (int i = 0; i < properties.Length; i++)
            {
                PropertyInfo property = properties[i];
                Console.WriteLine($"{i + generalParametersLenght}: {property.Name} = {property.GetValue(Program.CurrentShape)}");
            }
        }
        public static void ExecuteMenuAction(string input)
        {
            switch (input)
            {
                case "r": Program.ChangeMenu(0); break;

                default:
                    var splitinput = input.Split(':');
                    int parameterIndex = int.Parse(splitinput[0]);
                    double value = double.Parse(splitinput[1]);

                    int generalParametersLenght = Program.Settings.GeneralParameters.Parameters.Count;
                    Parameter parameter;

                    // Editing paramaters depening on if index falls within range of non-object specific paramaters (general paramaters)
                    if (parameterIndex < generalParametersLenght)
                    {
                        Program.Settings.GeneralParameters.Parameters[parameterIndex].Value = value;
                    }
                    else
                    {
                        PropertyInfo[] properties = Program.CurrentShape.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
                        properties[parameterIndex - generalParametersLenght].SetValue(Program.CurrentShape, value);
                    }
                    break;
            }
        }
    }
}
