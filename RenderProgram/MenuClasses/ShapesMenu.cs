using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RenderProgram.MenuClasses
{
    internal static class ShapesMenu
    {
        public static void PrintMenu()
        {
            Console.WriteLine("r: Return to main menu");

            foreach (var shape in Program.Settings.Shapes)
            {
                Console.WriteLine($"{shape.Id}: {shape.Name}");
            }
        }
    }
}
