using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RenderProgram
{
    internal static class Render
    {
        private static bool IsRunning { get; set; }
        private static double FrameTime { get; set; }
        private static AutoResetEvent TimingSignal { get; } = new(false);
        private static Stopwatch Watch { get; } = new();
        private static int ScreenWidth { get; set; }
        private static int ScreenHeight { get; set; }
        private static string ShapeNamespace { get; } = "RenderProgram.ShapeClasses";
        public static async void Run()
        {
            IsRunning = true;

            var generalParameters = Program.Settings.GeneralParameters.Parameters;
            var shapeParameters = Program.CurrentShape.Parameters;

            double a = 0.0;
            double b = 0.0;
            double aIntervall = generalParameters.FirstOrDefault(parameter => parameter.Name == "AutoRotationSpeedY").Value;
            double bIntervall = generalParameters.FirstOrDefault(parameter => parameter.Name == "AutoRotationSpeedX").Value;

            double renderQuality = generalParameters.FirstOrDefault(parameter => parameter.Name == "RenderQuality").Value;

            double thetaIntervallRaw = shapeParameters.FirstOrDefault(parameter => parameter.Name == "ThetaIntervall").Value;
            double phiIntervallRaw = shapeParameters.FirstOrDefault(parameter => parameter.Name == "PhiIntervall").Value;

            double thetaIntervall = thetaIntervallRaw / renderQuality;
            double phiIntervall = phiIntervallRaw / renderQuality;

            double radii1 = shapeParameters.FirstOrDefault(parameter => parameter.Name == "Radii1").Value;
            double radii2 = shapeParameters.FirstOrDefault(parameter => parameter.Name == "Radii2").Value;

            double cameraDistance = generalParameters.FirstOrDefault(parameter => parameter.Name == "CameraDistance").Value;

            FrameTime = 1000.0 / Program.Settings.GeneralParameters.Parameters.FirstOrDefault(parameter => parameter.Name == "Fps").Value;

            string objectClass = $"{ShapeNamespace}.{Program.CurrentShape.Name}";

            Task handleInputTask = Task.Run(() => HandleInput());
            Task timingTask = Task.Run(() => FPSControlLoop());

            while (IsRunning)
            {
                // Wait for the timing signal to start the next frame
                TimingSignal.WaitOne();

                ScreenWidth = Console.WindowWidth;
                ScreenHeight = Console.WindowHeight;

                a += aIntervall;
                b += bIntervall;

                //RenderFrame(a, b, thetaIntervall, phiIntervall, radii1, radii2, cameraDistance);
                MethodInfo RenderObjectMethod = Type.GetType(objectClass).GetMethod("RenderObject", BindingFlags.Public | BindingFlags.Static);
                object result = RenderObjectMethod.Invoke(null, null);
                string[] frameArray = result as string[];
                Span<string> frame = frameArray.AsSpan();
                RenderFrame(frame);
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
                    }
                }

                Thread.Sleep(10);
            }
        }
        private static void RenderFrameDe(double a, double b, double thetaIntervall, double phiIntervall, double radii1, double radii2, double cameraDistance)
        {
            string[] frameArray = new string[ScreenWidth * ScreenHeight];
            double[] zbufferArray = new double[ScreenWidth * ScreenHeight];

            Span<string> frame = frameArray.AsSpan();
            Span<double> zbuffer = zbufferArray.AsSpan();

            frame.Fill(" ");
            zbuffer.Fill(0.0);

            double sinA = Math.Sin(a), cosA = Math.Cos(a);
            double sinB = Math.Sin(b), cosB = Math.Cos(b);

            double distance1 = ScreenHeight * cameraDistance * 3 / (8 * (radii1 + radii2));

            for (double theta = 0; theta < 2 * Math.PI; theta += thetaIntervall)
            {
                double sinTheta = Math.Sin(theta), cosTheta = Math.Cos(theta);

                for (double phi = 0; phi < 2 * Math.PI; phi += phiIntervall)
                {
                    double sinPhi = Math.Sin(phi), cosPhi = Math.Cos(phi);

                    double circleX = radii2 + radii1 * cosTheta;
                    double circleY = radii1 * sinTheta;

                    double x = circleX * (cosB * cosPhi + sinA * sinB * sinPhi) - circleY * cosA * sinB;
                    double y = circleX * (sinB * cosPhi - sinA * cosB * sinPhi)
                      + circleY * cosA * cosB;
                    double z = cameraDistance + cosA * circleX * sinPhi + circleY * sinA;
                    double ooz = 1 / z;

                    double distance1xOoz = distance1 * ooz;

                    int xProjection = (int)(ScreenWidth / 2 + (distance1xOoz * x));
                    int yProjection = (int)(ScreenHeight / 2 - (distance1xOoz * y));

                    double luminance = cosPhi * cosTheta * sinB - cosA * cosTheta * sinPhi - sinA * sinTheta +
                        cosB * (cosA * sinTheta - cosTheta * sinA * sinPhi);


                    if (xProjection >= 0 && xProjection < ScreenWidth && yProjection >= 0 &&
                        yProjection < ScreenHeight)
                    {
                        int index = xProjection + yProjection * ScreenWidth;

                        if (ooz > zbuffer[index])
                        {
                            zbuffer[index] = ooz;
                            int luminanceIndex = (int)(luminance * 8);
                            frame[index] = luminanceIndex > 0
                                ? ".,-~:;=!*#$@"[luminanceIndex].ToString()
                                : ".";
                        }
                    }
                }
            }

            StringBuilder outputString = new();

            outputString.Append("\x1b[H");

            for (int j = 0; j < ScreenHeight; j++)
            {
                for (int i = 0; i < ScreenWidth; i++)
                {
                    outputString.Append(frame[i + j * ScreenWidth]);
                }
                outputString.Append('\n');
            }

            outputString.Length--;

            Console.Write(outputString.ToString());
        }
        private static void RenderFrame(Span<string> frame)
        {
            StringBuilder outputString = new();

            outputString.Append("\x1b[H");

            for (int j = 0; j < ScreenHeight; j++)
            {
                for (int i = 0; i < ScreenWidth; i++)
                {
                    outputString.Append(frame[i + j * ScreenWidth]);
                }
                outputString.Append('\n');
            }

            outputString.Length--;

            Console.Write(outputString.ToString());
        }
    }
}
