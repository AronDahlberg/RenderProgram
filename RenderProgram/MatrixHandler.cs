using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace RenderProgram
{
    internal static class MatrixHandler
    {

        public static void InitCharMatrix(char[,] Matrix, char input)
        {
            int length = Matrix.GetLength(0) * Matrix.GetLength(1);

            var span = MemoryMarshal.CreateSpan(ref Matrix[0, 0], length);

            span.Fill(input);
        }

        public static void InitDoubleMatrix(double[,] Matrix, double input)
        {
            int length = Matrix.GetLength(0) * Matrix.GetLength(1);

            var span = MemoryMarshal.CreateSpan(ref Matrix[0, 0], length);

            span.Fill(input);
        }

    }
}
