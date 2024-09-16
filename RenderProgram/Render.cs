using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace RenderProgram
{
    internal static class Render
    {
        private static bool IsRunning { get; set; }
        private static double FrameTime { get; set; }
        private static AutoResetEvent TimingSignal { get; } = new(false);
        private static Stopwatch Watch { get; } = new();
        private static double A { get; set; } = 0.0;
        private static double B { get; set; } = 0.0;
        private static double Alpha { get; set; } = Math.PI / 2;
        public static int ScreenWidth { get; set; }
        public static int ScreenHeight { get; set; }
        public static double AIntervall { get; set; }
        public static double BIntervall { get; set; }
        public static double CameraDistance { get; set; }
        public static double CameraSpeed { get; set; }
        public static async void Run()
        {
            IsRunning = true;

            // Find all parameters from the json file
            var generalParameters = Program.Settings.GeneralParameters.Parameters;

            AIntervall = generalParameters.FirstOrDefault(parameter => parameter.Name == "AutoRotationSpeedY").Value;
            BIntervall = generalParameters.FirstOrDefault(parameter => parameter.Name == "AutoRotationSpeedX").Value;

            double renderQuality = generalParameters.FirstOrDefault(parameter => parameter.Name == "RenderQuality").Value;

            CameraDistance = generalParameters.FirstOrDefault(parameter => parameter.Name == "CameraDistance").Value;
            CameraSpeed = generalParameters.FirstOrDefault(parameter => parameter.Name == "CameraSpeed").Value;

            FrameTime = 1000.0 / generalParameters.FirstOrDefault(parameter => parameter.Name == "Fps").Value;

            string objectClass = $"{Program.ShapeNamespace}.{Program.CurrentShape.GetType().Name}";
            MethodInfo renderObjectMethod = Type.GetType(objectClass).GetMethod("RenderShape", BindingFlags.Public | BindingFlags.Instance);
            MethodInfo calculateFocalLenghtMethod = Type.GetType(objectClass).GetMethod("CalculateFocalLenght", BindingFlags.Public | BindingFlags.Instance);


            double? focalLenght = calculateFocalLenghtMethod.Invoke(Program.CurrentShape, null) as double?;

            Task handleInputTask = Task.Run(() => HandleInput());
            Task timingTask = Task.Run(() => FPSControlLoop());

            while (IsRunning)
            {
                // Wait for the timing signal to start the next frame
                TimingSignal.WaitOne();

                ScreenWidth = Console.WindowWidth;
                ScreenHeight = Console.WindowHeight;

                A += AIntervall;
                B += BIntervall;

                // Render object and frame
                object result = renderObjectMethod.Invoke(Program.CurrentShape, new object[] { renderQuality, A, B, Alpha, focalLenght });

                PrintFrame((result as string[]).AsSpan());
            }

            // Wait for all tasks to finish
            await Task.WhenAll(handleInputTask,  timingTask);
        }
        private static void FPSControlLoop()
        {
            while (IsRunning)
            {
                Watch.Restart();

                // Allow the render task to proceed (render and print the next frame)
                TimingSignal.Set();

                // Cap the FPS
                Watch.Stop();
                double elapsedMilliseconds = Watch.Elapsed.TotalMilliseconds;
                double sleepTime = FrameTime - elapsedMilliseconds;

                if (sleepTime > 0)
                {
                    Thread.Sleep((int)sleepTime);
                }
            }
        }
        private static void HandleInput()
        {
            while (IsRunning)
            {
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(true);

                    switch (key.Key)
                    {
                        case ConsoleKey.Escape:
                            IsRunning = false;
                            TimingSignal.Set();
                            break;
                        case ConsoleKey.W:
                            CameraDistance -= CameraSpeed / 100; break;
                        case ConsoleKey.S:
                            CameraDistance += CameraSpeed / 100; break;
                        case ConsoleKey.A: // Move light source to the left
                            Alpha += CameraSpeed / 200;
                            break;
                        case ConsoleKey.D: // Move light source to the right
                            Alpha -= CameraSpeed / 200;
                            break;
                    }
                }

                Thread.Sleep(10);
            }
        }
        private static void PrintFrame(Span<string> frame)
        {
            StringBuilder outputString = new();

            outputString.Append("\x1b[H");

            // Extract all values into stringbuilder
            for (int j = 0; j < ScreenHeight; j++)
            {
                for (int i = 0; i < ScreenWidth; i++)
                {
                    outputString.Append(frame[i + j * ScreenWidth]);
                }
                outputString.Append('\n');
            }

            // Remove trailing '\n'
            outputString.Length--;

            Console.Write(outputString.ToString());
        }
    }
}
