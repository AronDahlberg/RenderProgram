using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RenderProgram.MenuClasses
{
    internal static class MainMenu
    {
        public static void ExecuteMenuAction(int input)
        {
            switch (input)
            {
                case 0: Program.Exit(); break;

                case 1: //Render.Run(); break;

                default: Program.ChangeMenu(Program.CurrentMenu.Text.Options.FirstOrDefault(menu => menu.Id == input).MenuSwitch); break;
            }
        }
    }
}
