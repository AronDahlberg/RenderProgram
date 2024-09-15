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
            Console.WriteLine("Current shape: " + Program.CurrentShape.GetType().Name);
            Console.WriteLine("r: Return to main menu");

            foreach (var shape in Program.Settings.Shapes)
            {
                Console.WriteLine($"{shape.Id}: {shape.Name}");
            }
        }

        public static void ExecuteMenuAction(string input)
        {
            switch (input)
            {
                case "r": Program.ChangeMenu(0); break;

                default: Program.ChangeShape(Program.Settings.Shapes.FirstOrDefault(shape => shape.Id == int.Parse(input)).Name); break;
            }
        }
    }
}
