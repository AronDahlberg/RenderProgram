namespace RenderProgram
{
    internal class Program
    {
        private static bool IsRunning { get; set; } = true;
        static void Main(string[] args)
        {
            while (IsRunning)
            {
                WriteMenu();
                string? input = Console.ReadLine();

                switch (input)
                {
                    //menu
                }
            }
        }
        private static void WriteMenu()
        {

        }
    }
}
