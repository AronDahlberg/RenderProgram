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
            foreach (var parameter in Program.Settings.GeneralParameters)
            {
                Console.WriteLine($"{parameter.Id}: {parameter.Name} = {parameter.Value}");
            }
            foreach (var parameter in Program.CurrentShape.Parameters)
            {
                Console.WriteLine($"{parameter.Id}: {parameter.Name} = {parameter.Value}");
            }
        }
    }
}
