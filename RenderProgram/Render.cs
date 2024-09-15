using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RenderProgram
{
    internal static class Render
    {
        private static bool IsRunning { get; set; } = true;
        private static double FrameTime { get; set; }
        private static AutoResetEvent TimingSignal { get; } = new(false);
        private static Stopwatch Watch { get; } = new();
        private static char[,] Frame {  get; set; }
        public static async void Run()
        {
            FrameTime = 1000.0 / Program.Settings.GeneralParameters.Parameters.FirstOrDefault(parameter => parameter.Name == "Fps").Value;

            Task handleInputTask = Task.Run(() => HandleInput());
            Task timingTask = Task.Run(() => FPSControlLoop());

            while (IsRunning)
            {
                // Wait for the timing signal to start the next frame
                TimingSignal.WaitOne();

                RenderFrame();
                PrintFrame();
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
                            break;
                    }
                }

                Thread.Sleep(10);
            }
        }
        private static void RenderFrame()
        {

        }
        private static void PrintFrame()
        {

        }
    }
}
