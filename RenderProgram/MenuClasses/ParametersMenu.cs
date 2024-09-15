using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RenderProgram.MenuClasses
{
    internal static class ParametersMenu
    {
        public static void PrintMenu()
        {
            Console.WriteLine("r: Return to main menu");

            foreach (var parameter in Program.Settings.GeneralParameters.Parameters)
            {
                Console.WriteLine($"{parameter.Id}: {parameter.Name} = {parameter.Value}");
            }
            foreach (var parameter in Program.CurrentShape.Parameters)
            {
                Console.WriteLine($"{parameter.Id}: {parameter.Name} = {parameter.Value}");
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

                    if (parameterIndex < generalParametersLenght)
                    {
                        parameter = Program.Settings.GeneralParameters.Parameters[parameterIndex];
                    }
                    else
                    {
                        parameter = Program.CurrentShape.Parameters[parameterIndex - generalParametersLenght];
                    }

                    Program.EditSettings(parameter, value);

                    break;
            }
        }
    }
}
