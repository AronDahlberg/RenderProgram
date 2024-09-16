using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RenderProgram.ShapeClasses
{
    internal class ShadedTorus
    {
        private double ConsoleAspectRatio { get; set; } = 2;
        public double Radii1 { get; set; } = 1;
        public double Radii2 { get; set; } = 2;
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

                for (double phi = 0; phi < 2 * Math.PI; phi += PhiIntervall)
                {
                    double sinPhi = Math.Sin(phi), cosPhi = Math.Cos(phi);

                    double circleX = Radii2 + Radii1 * cosTheta;
                    double circleY = Radii1 * sinTheta;

                    double x = circleX * (cosB * cosPhi + sinA * sinB * sinPhi) - circleY * cosA * sinB;
                    double y = circleX * (sinB * cosPhi - sinA * cosB * sinPhi) + circleY * cosA * cosB;
                    double z = Render.CameraDistance + cosA * circleX * sinPhi + circleY * sinA;
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

                            int luminanceIndex = (int)((luminance + sqrt2) * 70.71); // 70.71 = <maximumLuminecence> / (2* Sqrt(2))

                            output[index] = $"\x1b[48;2;{luminanceIndex + 55};{luminanceIndex + 20};{luminanceIndex}m \x1b[m";
                        }
                    }
                }
            }

            return output.ToArray();
        }
        public double CalculateFocalLenght()
        {
            // Focal lenght set so that the object fills 2/3 of the screen at the start of rendering
            return Render.CameraDistance * 3 / (8 * (Radii1 + Radii2));
        }
    }
}
