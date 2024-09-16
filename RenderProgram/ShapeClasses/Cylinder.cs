using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RenderProgram.ShapeClasses
{
    internal class Cylinder
    {
        private double ConsoleAspectRatio { get; set; } = 2;
        public double Radii { get; set; } = 2;
        public double ThetaIntervallRaw { get; set; } = 0.7;
        public double PhiIntervallRaw { get; set; } = 0.2;

        public string[] RenderShape(double renderQuality, double a, double b, double alpha, double focalLenght)
        {
            // Larger renderQuality = more plotted points
            double ThetaIntervall = ThetaIntervallRaw / renderQuality;
            double PhiIntervall = PhiIntervallRaw / renderQuality;

            // 2D matrix as 1D array
            string[] outputArray = new string[Render.ScreenWidth * Render.ScreenHeight];
            double[] zbufferArray = new double[Render.ScreenWidth * Render.ScreenHeight];

            Span<string> output = outputArray.AsSpan();
            Span<double> zbuffer = zbufferArray.AsSpan();

            output.Fill(" ");
            zbuffer.Fill(0.0);

            double sinA = Math.Sin(a), cosA = Math.Cos(a);
            double sinB = Math.Sin(b), cosB = Math.Cos(b);

            double sinAlpha = Math.Sin(alpha), cosAlpha = Math.Cos(alpha);

            double sqrt2 = Math.Sqrt(2);

            for (double theta = 0; theta < 2 * Math.PI; theta += ThetaIntervall)
            {
                double sinTheta = Math.Sin(theta), cosTheta = Math.Cos(theta);

                for (double phi = 0; phi < Math.PI; phi += PhiIntervall)
                {
                    double sinPhi = Math.Sin(phi), cosPhi = Math.Cos(phi);

                    double squareX = 0;
                    double squareY = 0;

                    if (theta < Math.PI / 2)
                    {
                        // Right side: (1, 0) to (1, 1)
                        squareX = Math.PI / 4;
                        squareY = -theta + (Math.PI / 4);
                    }
                    else if (theta < Math.PI)
                    {
                        // Top side: (1, 1) to (-1, 1)
                        squareX = (Math.PI / 4) - (theta - (Math.PI / 2));
                        squareY = Math.PI / 4;
                    }
                    else if (theta < Math.PI + (Math.PI / 2))
                    {
                        // Left side: (-1, 1) to (-1, -1)
                        squareX = -(Math.PI / 4);
                        squareY = (Math.PI / 2) - (theta - (Math.PI / 2)) + (Math.PI / 4);
                    }
                    else
                    {
                        // Bottom side: (-1, -1) to (1, -1)
                        squareX = (Math.PI / 4) - (theta - ((Math.PI) + (Math.PI / 2)));
                        squareY = -(Math.PI / 4);
                    }

                    double x = squareX * (cosB * cosPhi + sinA * sinB * sinPhi) - squareY * cosA * sinB;
                    double y = squareX * (sinB * cosPhi - sinA * cosB * sinPhi) + squareY * cosA * cosB;
                    double z = Render.CameraDistance + cosA * squareX * sinPhi + squareY * sinA;
                    double ooz = 1 / z; // one over z

                    int xProjection = (int)(Render.ScreenWidth / 2 + (Render.ScreenHeight * focalLenght) * ooz * x * ConsoleAspectRatio);
                    int yProjection = (int)(Render.ScreenHeight / 2 - (Render.ScreenHeight * focalLenght) * ooz * y);

                    double luminance = ((cosTheta * cosPhi * cosB * cosAlpha) - (((sinTheta * cosA) - (cosTheta * sinPhi * sinA)) * sinB * cosAlpha) + (cosTheta * cosPhi * sinB) + (((sinTheta * cosA) - (cosTheta * sinPhi * sinA)) * cosB) - (((sinTheta * sinA) + (cosTheta * sinPhi * cosA)) * sinAlpha));

                    if (xProjection >= 0 && xProjection < Render.ScreenWidth && yProjection >= 0 &&
                        yProjection < Render.ScreenHeight)
                    {
                        int index = xProjection + yProjection * Render.ScreenWidth;

                        if (ooz > zbuffer[index])
                        {
                            zbuffer[index] = ooz;

                            string charString = "  ....,,,,--~~~::;=!*#$@";

                            int luminanceIndex = (int)((luminance + sqrt2) * 8.48); // 8.48 = charString.Length / (2* Sqrt(2))

                            output[index] = charString[luminanceIndex].ToString();
                        }
                    }
                }
            }

            return output.ToArray();
        }
        public double CalculateFocalLenght()
        {
            // Focal lenght set so that the object fills 2/3 of the screen at the start of rendering
            return Render.CameraDistance * 3 / (8 * (Radii));
        }
    }
}
