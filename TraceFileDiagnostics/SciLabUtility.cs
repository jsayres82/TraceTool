using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNetScilab;

namespace TraceFileReader
{
    public class SciLabUtility
    {

        private static Scilab m_oSCilab;

        public Scilab Engine { get { return m_oSCilab; } }

        public SciLabUtility()
        {
            m_oSCilab = new Scilab(true);
        }


        //=============================================================================
        /*
         * A small example to call scilab from C#
         * read & write matrix of double, string, boolean, int(32)
         */
        public void example_readwriteMatrixOfDouble(List<string> xList, List<double> yList)
        {
            //=============================================================================
            // Send a command to scilab
            // Here , we want to display SCI variable
            m_oSCilab.SendScilabJob("disp(\'SCI = \');");
            m_oSCilab.SendScilabJob("disp(SCI);");
            //=============================================================================
            double[] yValMatrix = new double[xList.Count];
            double[] xValMatrix = new double[xList.Count];
            DateTime firstTime;

            int index = 0;
            firstTime = Convert.ToDateTime(xList[index]);
            foreach (string x in xList)
            {
                yValMatrix[index] = yList[index];
                xValMatrix[index++] = Convert.ToDouble((Convert.ToDateTime(x)-firstTime).TotalMilliseconds);
            }

            // Write a matrix of double named in scilab
            m_oSCilab.createNamedMatrixOfDouble("X", index, 1, xValMatrix);
            m_oSCilab.createNamedMatrixOfDouble("Y", index, 1, yValMatrix);

            // display matrix of double by scilab
            m_oSCilab.SendScilabJob("disp(\'X =\');");
            m_oSCilab.SendScilabJob("table = [X, Y]");
            //=============================================================================
            if (m_oSCilab.getNamedVarType("table") == (int)DotNetScilab.ScilabType.sci_matrix)
            {
                Console.WriteLine("Table is a matrix of double");
            }
            //=============================================================================
            
            // get dimensions of a named matrix of double
            int[] DimTable = m_oSCilab.getNamedVarDimension("table");

            // get named matrix of double
            double[] table = m_oSCilab.readNamedMatrixOfDouble("table");

            // display matrix of double from C#
            Console.WriteLine("");
            Console.WriteLine("(C#) B =");
            for (int i = 0; i < DimTable[0]; i++)
            {
                for (int j = 0; j < DimTable[1]; j++)
                {
                    Console.Write(table[j * DimTable[0] + i] + " ");
                }

                Console.WriteLine("");
            }

            // display matrix of double by scilab
            m_oSCilab.SendScilabJob("disp(\'table =\');");
            m_oSCilab.SendScilabJob("disp(table);");
            m_oSCilab.SendScilabJob("plot(X,Y);");//'time(mS)', 'position(in)', 'linear motion');");
            while (m_oSCilab.HaveAGraph())
            {
                m_oSCilab.doEvent();
            }
            //=============================================================================
        }
        //=============================================================================
        public void example_readwriteMatrixOfString()
        {
            //=============================================================================
            string[] strA = new string[] { "Scilab", "The", "open",
                                        "source", "for", "numerical",
                                        "computation" , ":", ")"};
            int mstrA = 3, nstrA = 3;

            // Write a matrix of string named in scilab
            m_oSCilab.createNamedMatrixOfString("string_A", mstrA, nstrA, strA);

            // display matrix of string by scilab
            m_oSCilab.SendScilabJob("disp(\'string_A =\');");
            m_oSCilab.SendScilabJob("disp(string_A);");
            //=============================================================================
            if (m_oSCilab.getNamedVarType("string_A") == (int)DotNetScilab.ScilabType.sci_strings)
            {
                Console.WriteLine("string_A is a matrix of strings");
            }
            //=============================================================================
            m_oSCilab.SendScilabJob("string_B = convstr(string_A,\'u\');");

            // get dimensions of a named matrix of string
            int[] DimstrB = m_oSCilab.getNamedVarDimension("string_B");

            // get named matrix of string
            string[] strB = m_oSCilab.readNamedMatrixOfString("string_B");

            Console.WriteLine("");
            Console.WriteLine("(C#) strB =");
            for (int i = 0; i < DimstrB[0]; i++)
            {
                for (int j = 0; j < DimstrB[1]; j++)
                {
                    Console.Write(strB[j * DimstrB[0] + i] + " ");
                }

                Console.WriteLine("");
            }

            // display matrix of string by scilab
            m_oSCilab.SendScilabJob("disp(\'string_B =\');");
            m_oSCilab.SendScilabJob("disp(string_B);");
            //=============================================================================
        }
        //=============================================================================
        public void example_readwriteMatrixOfBoolean()
        {
            //=============================================================================
            Boolean[] bA = new Boolean[] { false, true, false,
                                           true, false, true};
            int mbA = 2, nbA = 3;

            // Write a matrix of string named in scilab
            m_oSCilab.createNamedMatrixOfBoolean("boolean_A", mbA, nbA, bA);

            // display matrix of string by scilab
            m_oSCilab.SendScilabJob("disp(\'boolean_A =\');");
            m_oSCilab.SendScilabJob("disp(boolean_A);");
            //=============================================================================
            // check if av
            if (m_oSCilab.existNamedVariable("boolean_A") == true)
            {
                Console.WriteLine("boolean_A exists in scilab");
            }

            if (m_oSCilab.existNamedVariable("boolean_B") == false)
            {
                Console.WriteLine("boolean_B does not exist in scilab");
            }
            //=============================================================================
            if (m_oSCilab.getNamedVarType("boolean_A") == (int)DotNetScilab.ScilabType.sci_boolean)
            {
                Console.WriteLine("boolean_A is a matrix of boolean");
            }
            //=============================================================================
            m_oSCilab.SendScilabJob("boolean_B = ~boolean_A;");
            // get dimensions of a named matrix of boolean
            int[] DimbB = m_oSCilab.getNamedVarDimension("boolean_B");

            // get named matrix of boolean
            Boolean[] bB = m_oSCilab.getNamedMatrixOfBoolean("boolean_B");

            Console.WriteLine("");
            Console.WriteLine("(C#) bB =");
            for (int i = 0; i < DimbB[0]; i++)
            {
                for (int j = 0; j < DimbB[1]; j++)
                {
                    Console.Write(bB[j * DimbB[0] + i] + " ");
                }

                Console.WriteLine("");
            }

            // display matrix of string by scilab
            m_oSCilab.SendScilabJob("disp(\'boolean_B =\');");
            m_oSCilab.SendScilabJob("disp(boolean_B);");
            //=============================================================================
        }
        //=============================================================================
        public void example_doplot3d()
        {
            m_oSCilab.SendScilabJob("plot3d()");
            while (m_oSCilab.HaveAGraph())
            {
                m_oSCilab.doEvent();
            }
        }
        //=============================================================================
        public void example_readwriteMatrixOfInt()
        {
            //=============================================================================
            int[] A = new int[] { 1, 2, 3, 4, 5, 6 };
            int mA = 2, nA = 3;

            // Write a matrix of double named in scilab
            m_oSCilab.createNamedMatrixOfInt32("int32_A", mA, nA, A);

            // display matrix of double by scilab
            m_oSCilab.SendScilabJob("disp(\'int32_A =\');");
            m_oSCilab.SendScilabJob("disp(int32_A);");
            //=============================================================================
            if (m_oSCilab.getNamedVarType("int32_A") == (int)DotNetScilab.ScilabType.sci_ints)
            {
                Console.WriteLine("int32_A is a matrix of int(32)");
            }
            //=============================================================================
            m_oSCilab.SendScilabJob("int32_B = int32_A + 1;");

            // get dimensions of a named matrix of double
            int[] DimB = m_oSCilab.getNamedVarDimension("int32_B");

            // get named matrix of double
            int[] B = m_oSCilab.readNamedMatrixOfInt32("int32_B");

            // display matrix of double from C#
            Console.WriteLine("");
            Console.WriteLine("(C#) int32_B =");
            for (int i = 0; i < DimB[0]; i++)
            {
                for (int j = 0; j < DimB[1]; j++)
                {
                    Console.Write(B[j * DimB[0] + i] + " ");
                }

                Console.WriteLine("");
            }

            // display matrix of double by scilab
            m_oSCilab.SendScilabJob("disp(\'int32_B =\');");
            m_oSCilab.SendScilabJob("disp(int32_B);");
            //=============================================================================
        }
        //=============================================================================
        public void example_readwriteComplexMatrixOfDouble()
        {
            //=============================================================================
            double[] realPartA = new double[] { 1, 2, 3, 4, 5, 6 };
            double[] imagPartA = new double[] { 6, 5, 4, 3, 2, 1 };
            int mA = 2, nA = 3;

            // Write a matrix of double named in scilab
            m_oSCilab.createNamedComplexMatrixOfDouble("cplx_A", mA, nA, realPartA, imagPartA);

            // display matrix of double by scilab
            m_oSCilab.SendScilabJob("disp(\'cplx_A =\');");
            m_oSCilab.SendScilabJob("disp(cplx_A);");
            //=============================================================================
            m_oSCilab.SendScilabJob("cplx_B = cplx_A * 2;");

            // get dimensions of a named matrix of double
            int[] DimB = m_oSCilab.getNamedVarDimension("cplx_B");

            // get named matrix of double
            double[] realPartB = m_oSCilab.readNamedComplexMatrixOfDoubleRealPart("cplx_B");
            double[] imagPartB = m_oSCilab.readNamedComplexMatrixOfDoubleImgPart("cplx_B");

            // display matrix of double from C#
            Console.WriteLine("");
            Console.WriteLine("(C#) cplx_B =");
            for (int i = 0; i < DimB[0]; i++)
            {
                for (int j = 0; j < DimB[1]; j++)
                {
                    Console.Write(realPartB[j * DimB[0] + i] + " + i *" + imagPartB[j * DimB[0] + i] + " ");
                }

                Console.WriteLine("");
            }

            // display matrix of double by scilab
            m_oSCilab.SendScilabJob("disp(\'cplx_B =\');");
            m_oSCilab.SendScilabJob("disp(cplx_B);");
            //=============================================================================
        }
        //=============================================================================
        /*
         * A small example to call scilab from C#
         * read & write matrix of double, string, boolean, int(32)
         */
        public void example_readwriteMatrixOfDouble()
        {
            //=============================================================================
            // Send a command to scilab
            // Here , we want to display SCI variable
            m_oSCilab.SendScilabJob("disp(\'SCI = \');");
            m_oSCilab.SendScilabJob("disp(SCI);");
            //=============================================================================
            double[] A = new double[] { 1, 2, 3, 4, 5, 6 };
            int mA = 2, nA = 3;

            // Write a matrix of double named in scilab
            m_oSCilab.createNamedMatrixOfDouble("A", mA, nA, A);

            // display matrix of double by scilab
            m_oSCilab.SendScilabJob("disp(\'A =\');");
            m_oSCilab.SendScilabJob("disp(A);");
            //=============================================================================
            if (m_oSCilab.getNamedVarType("A") == (int)DotNetScilab.ScilabType.sci_matrix)
            {
                Console.WriteLine("A is a matrix of double");
            }
            //=============================================================================
            m_oSCilab.SendScilabJob("B = A + 1;");

            // get dimensions of a named matrix of double
            int[] DimB = m_oSCilab.getNamedVarDimension("B");

            // get named matrix of double
            double[] B = m_oSCilab.readNamedMatrixOfDouble("B");

            // display matrix of double from C#
            Console.WriteLine("");
            Console.WriteLine("(C#) B =");
            for (int i = 0; i < DimB[0]; i++)
            {
                for (int j = 0; j < DimB[1]; j++)
                {
                    Console.Write(B[j * DimB[0] + i] + " ");
                }

                Console.WriteLine("");
            }

            // display matrix of double by scilab
            m_oSCilab.SendScilabJob("disp(\'B =\');");
            m_oSCilab.SendScilabJob("disp(B);");
            //=============================================================================
        }
        //=============================================================================
        public void plot3D()
        {
            example_readwriteMatrixOfDouble();

            example_readwriteMatrixOfString();

            example_readwriteMatrixOfBoolean();

            example_readwriteMatrixOfInt();

            example_readwriteComplexMatrixOfDouble();

            example_doplot3d();
        }

        public void plot3DMotion()
        {
            m_oSCilab.execScilabScript("C:/Users/jsayres/Documents/Visual Studio 2012/Projects/ServiceTool - Copy/ServiceTool./SciLabScripts/anim_cylinder_translate.sce");
            while (m_oSCilab.HaveAGraph())
            {
                m_oSCilab.doEvent();
            }
        }
    }
}
