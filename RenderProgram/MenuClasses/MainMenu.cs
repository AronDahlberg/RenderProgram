namespace RenderProgram.MenuClasses
{
    internal static class MainMenu
    {
        public static void ExecuteMenuAction(string input)
        {
            int command = int.Parse(input);

            switch (command)
            {
                case 0: Program.Exit(); break;

                case 1: Render.Run(); break;

                default: Program.ChangeMenu(Program.CurrentMenu?.Text?.Options?.FirstOrDefault(menu => menu.Id == command)?.MenuSwitch); break;
            }
        }
    }
}
