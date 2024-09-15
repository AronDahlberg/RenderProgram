using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace RenderProgram.ShapeClasses
{
    internal class Torus
    {
        public double Radii1 { get; set; } = 1;
        public double Radii2 { get; set; } = 2;
        public double ThetaIntervallRaw { get; set; } = 0.7;
        public double PhiIntervallRaw { get; set; } = 0.2;
        public string[] RenderShape(double renderQuality, double a, double b)
        {
            double ThetaIntervall = ThetaIntervallRaw / renderQuality;
            double PhiIntervall = PhiIntervallRaw / renderQuality;

            string[] outputArray = new string[Render.ScreenWidth * Render.ScreenHeight];
            double[] zbufferArray = new double[Render.ScreenWidth * Render.ScreenHeight];

            Span<string> output = outputArray.AsSpan();
            Span<double> zbuffer = zbufferArray.AsSpan();

            output.Fill(" ");
            zbuffer.Fill(0.0);

            double sinA = Math.Sin(a), cosA = Math.Cos(a);
            double sinB = Math.Sin(b), cosB = Math.Cos(b);

            double distance1 = Render.ScreenHeight * Render.CameraDistance * 3 / (8 * (Radii1 + Radii2));

            for (double theta = 0; theta < 2 * Math.PI; theta += ThetaIntervall)
            {
                double sinTheta = Math.Sin(theta), cosTheta = Math.Cos(theta);

                for (double phi = 0; phi < 2 * Math.PI; phi += PhiIntervall)
                {
                    double sinPhi = Math.Sin(phi), cosPhi = Math.Cos(phi);

                    double circleX = Radii2 + Radii1 * cosTheta;
                    double circleY = Radii1 * sinTheta;

                    double x = circleX * (cosB * cosPhi + sinA * sinB * sinPhi) - circleY * cosA * sinB;
                    double y = circleX * (sinB * cosPhi - sinA * cosB * sinPhi)
                      + circleY * cosA * cosB;
                    double z = Render.CameraDistance + cosA * circleX * sinPhi + circleY * sinA;
                    double ooz = 1 / z;

                    double distance1xOoz = distance1 * ooz;

                    int xProjection = (int)(Render.ScreenWidth / 2 + (distance1xOoz * x));
                    int yProjection = (int)(Render.ScreenHeight / 2 - (distance1xOoz * y));

                    double luminance = cosPhi * cosTheta * sinB - cosA * cosTheta * sinPhi - sinA * sinTheta +
                        cosB * (cosA * sinTheta - cosTheta * sinA * sinPhi);


                    if (xProjection >= 0 && xProjection < Render.ScreenWidth && yProjection >= 0 &&
                        yProjection < Render.ScreenHeight)
                    {
                        int index = xProjection + yProjection * Render.ScreenWidth;

                        if (ooz > zbuffer[index])
                        {
                            zbuffer[index] = ooz;
                            int luminanceIndex = (int)(luminance * 8);
                            output[index] = luminanceIndex > 0
                                ? ".,-~:;=!*#$@"[luminanceIndex].ToString()
                                : ".";
                        }
                    }
                }
            }

            return output.ToArray();
        }
    }
}
