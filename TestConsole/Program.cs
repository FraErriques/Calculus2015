using System;
using System.Runtime.CompilerServices;
using System.Threading;


namespace TestConsole
{

       
    
    class Program
    {
        public static System.Threading.Thread localThread = null;

        public class workerContainer
        {
            public static void calcBkLoop()
            {
                try
                {
                    while ( localThread.ThreadState != System.Threading.ThreadState.SuspendRequested)
                    {
                        System.Console.WriteLine("\n from calcBkLoop CurrentThread.ManagedThreadId== " + System.Threading.Thread.CurrentThread.ManagedThreadId.ToString()
                            +"___localThread.ManagedThreadId== " + localThread.ManagedThreadId.ToString() );
                        System.Console.WriteLine("\n from calcBkLoop CurrentThread.ThreadState== " + System.Threading.Thread.CurrentThread.ThreadState.ToString()
                            + "___localThread.ThreadState== " + localThread.ThreadState.ToString());
                        // do something
                    }// while !suspended
                    if (localThread.ThreadState == System.Threading.ThreadState.SuspendRequested)
                    {
                        System.Console.WriteLine("\n Suspended.");
                    }
                    // when here thread has been suspended
                }
                catch(ThreadAbortException thrEx)
                {
                    System.Console.WriteLine("\n Specialized Exception ThreadAbortException : "+ thrEx.Message); 
                }// specific catch
                catch(System.Exception ex)
                {
                    System.Console.WriteLine("\n Generic Exception : " + ex.Message);
                }// general catch
                return;
            }// calcBkLoop()
        }// class workerContainer

        public static void caller()
        {
            workerContainer wk = new workerContainer();
            // ThreadStart  delegate
            Program.localThread = new Thread(Program.workerContainer.calcBkLoop);
            System.Console.WriteLine("\n from caller  before Start CurrentThread.ManagedThreadId== " + System.Threading.Thread.CurrentThread.ManagedThreadId.ToString()
                + "___localThread.ManagedThreadId== " + localThread.ManagedThreadId.ToString());
            System.Console.WriteLine("\n from caller  before Start CurrentThread.ThreadState== " + System.Threading.Thread.CurrentThread.ThreadState.ToString()
                + "___localThread.ThreadState== " + localThread.ThreadState.ToString());
            // do something
            Program.localThread.Start();
            System.Console.WriteLine("\n from caller after Start CurrentThread.ManagedThreadId== " + System.Threading.Thread.CurrentThread.ManagedThreadId.ToString()
                + "___localThread.ManagedThreadId== " + localThread.ManagedThreadId.ToString());
            System.Console.WriteLine("\n from caller after Start CurrentThread.ThreadState== " + System.Threading.Thread.CurrentThread.ThreadState.ToString()
                + "___localThread.ThreadState== " + localThread.ThreadState.ToString());
            //
            //Program.localThread.Suspend();
            Program.localThread.Abort();
            //Program.localThread.Join();
            System.Console.WriteLine("\n from caller after Abort CurrentThread.ManagedThreadId== " + System.Threading.Thread.CurrentThread.ManagedThreadId.ToString()
                + "___localThread.ManagedThreadId== " + localThread.ManagedThreadId.ToString());
            System.Console.WriteLine("\n from caller after Abort CurrentThread.ThreadState== " + System.Threading.Thread.CurrentThread.ThreadState.ToString()
                + "___localThread.ThreadState== " + localThread.ThreadState.ToString());
        }// caller



        static void Main( string[] args )
        {
            LogSinkFs.Wrappers.LogWrappers.SectionOpen("Main()", 0);

            caller();

            //---########################################################################
            System.Console.WriteLine("\n\n\t Strike \"Enter\" to leave ");
            System.Console.ReadLine();
            LogSinkFs.Wrappers.LogWrappers.SectionClose();
        }// end main

    }// end class Program
}// end nmsp






//#region cantina



//#region ContourIntg_delegates
//private static double x(double t)
//{// x(t) cerchio di raggio 2.1 e centro (3,5).
//    return (+3.0 + 2.1 * Math.Cos(t));
//}// x(t)
//private static double y(double t)
//{// y(t)
//    return (+5.0 + 2.1 * Math.Sin(t));
//}// y(t)
//private static double dx(double t)
//{// dx(t)=x'(t)dt=...
//    return (-2.1 * Math.Sin(t));
//}// dx(t)
//private static double dy(double t)
//{// dy(y)=y'(t)dt==...
//    return (+2.1 * Math.Cos(t));
//}// dy(t)


/// <summary>
/// the functions choosen for the example are f(z)=z which implies u(x,y)=x, v(x,y)=y; 
/// the choice for the contour is x(t)=t,y(t)=2*t+1,dx=dt which means dx=1
/// dy=2*dt which means dy=2.
/// </summary>
/// <param name="x"></param>
/// <param name="y"></param>
/// <returns></returns>
//private static double genericIntegrand_u_part(double x, double y)
//{// f(z)==z^2 -> Re(f(z))==Re((x+I*y)^2)==x^2-y^2
//    return x * x - y * y;
//}// u(x,y)
//private static double genericIntegrand_v_part(double x, double y)
//{// f(z)==z^2 -> Im(f(z))==Im((x+I*y)^2)==2*x*y
//    return 2.0 * x * y;
//}// v(x,y)
// //
//private static ComplexField.Complex zSquared_AsScalar_(double x, double y)
//{
//    ComplexField.Complex z = new ComplexField.Complex(x, y);
//    return z * z;// z^2
//}// fPtr_ComplexAsScalar_
//#endregion ContourIntg_delegates




//##
//Integrazione di f[z] == 
//f[x + I*y] == (x + I*y)^2 sulla semicirconferenza superiore
//x[t] == +3.0 + 2.1*Cos[t]
//y[t] == +5.0 + 2.1*Sin[t]
//t in [0, +Pi]
//ComplexField.Complex res =
//    ComplexField.ContourIntegral.ContourIntegral_ManagementMethod(
//        new ComplexField.Complex(+3.0 + 2.1, +5.0)
//        , new ComplexField.Complex(+3.0 - 2.1, +5.0)
//        , 0.0
//        , +1.0 * Math.PI
//        , new ComplexField.ContourIntegral.fPtr_U_or_V_(genericIntegrand_u_part)
//        , new ComplexField.ContourIntegral.fPtr_U_or_V_(genericIntegrand_v_part)
//        , new ComplexField.ContourIntegral.fPtr_Jordan_parametriz_(x)
//        , new ComplexField.ContourIntegral.fPtr_Jordan_parametriz_(y)
//        , new ComplexField.ContourIntegral.fPtr_Jordan_parametriz_(dx)
//        , new ComplexField.ContourIntegral.fPtr_Jordan_parametriz_(dy)
//        , 99999); // # dx

//##
//Integrazione di f[z] == 
//f[x + I*y] == (x + I*y)^2 sulla semicirconferenza superiore
//x[t] == +3.0 + 2.1*Cos[t]
//y[t] == +5.0 + 2.1*Sin[t]
//t in [0, +Pi]
//ComplexField.Complex res_asAscalar =
//    ComplexField.ContourIntegral.ContourIntegral_AsScalar_ManagementMethod(
//        new ComplexField.Complex(+3.0 + 2.1, +5.0)
//        , new ComplexField.Complex(+3.0 - 2.1, +5.0)
//        , 0.0
//        , +1.0 * Math.PI
//        , new ComplexField.ContourIntegral.fPtr_ComplexAsScalar_(zSquared_AsScalar_)
//        , new ComplexField.ContourIntegral.fPtr_Jordan_parametriz_(x)
//        , new ComplexField.ContourIntegral.fPtr_Jordan_parametriz_(y)
//        , new ComplexField.ContourIntegral.fPtr_Jordan_parametriz_(dx)
//        , new ComplexField.ContourIntegral.fPtr_Jordan_parametriz_(dy)
//        , 99999); // # dx

//double areaTrapezio =
//    RealField.Integrate.Integrate_equi_trapezium(0, 2, 9);

// TODO : memo per change extrema parameters :

//In[7]:= x[t_] = t
//In[8]:= y[t_] = (2*t + 1)

//In[12]:= {x[0], y[0]}
//Out[12]= {0, 1}

//In[11]:= {x[11], y[11]}
//Out[11]= {11, 23}

//ComplexField.Complex res =
//    ComplexField.ContourIntegral.ContourIntegral_ManagementMethod(0.0, 11.0, 999);
//##



//System.Random myGenerator = new Random();
//double previousPiG_Level = 0.0;
//double PiG_Level = 0.0;
//PrimesFinder.Primes P = new PrimesFinder.Primes();// default Ctor is the one for reading the log.
//double Pi_64 = P.Pi_Greco(64);
//double J_64 = P.J(64); // NB. J(x)>=Pi(x)

//for (long thresholdInNaturals = 0; thresholdInNaturals < 10; thresholdInNaturals++)
//{
//    double pollutedInteger = thresholdInNaturals + myGenerator.NextDouble();
//    //Console.Write("\t GetCumulatedOrdinalNotOverThreshold({0}) == {1}  the Inf={2}"
//    //    , thresholdInNaturals
//    //    , P.GetCumulatedOrdinalNotOverThreshold(thresholdInNaturals, out theInf)
//    //    , theInf
//    //    );
//    PiG_Level = P.Pi_Greco(thresholdInNaturals);
//    if (Math.Abs(PiG_Level - previousPiG_Level) > double.Epsilon)
//    {
//        Console.WriteLine();
//    }// else continue on the same line;
//    previousPiG_Level = PiG_Level;// update the PiGreco level.
//    Console.Write("____\t PiGreco({0}) == {1} "
//        , thresholdInNaturals
//        , P.Pi_Greco(thresholdInNaturals)
//        );
//    PiG_Level = P.Pi_Greco(pollutedInteger);
//    if (Math.Abs(PiG_Level - previousPiG_Level) > double.Epsilon)
//    {
//        Console.WriteLine();
//    }// else continue on the same line;
//    previousPiG_Level = PiG_Level;// update the PiGreco level.
//    //
//    Console.Write("____\t PiGreco({0}) == {1} \t Pi_calculated_onJ({0})=={2}"
//        , pollutedInteger
//        , P.Pi_Greco( pollutedInteger)
//        , P.Pi_calculated_onJ( pollutedInteger)
//        );
//    //
//    Console.Write("____\t J({0}) == {1}"
//        , pollutedInteger
//        , P.J(pollutedInteger) // NB. J(x)>=Pi(x)
//        );
//}


//ComplexField.Complex res =
//    ComplexField.ContourIntegral.ContourIntegral_ManagementMethod(
//        new ComplexField.Complex(+3.0 + 2.1, +5.0)
//        , new ComplexField.Complex(+3.0 - 2.1, +5.0)
//        , 0.0
//        , +1.0*Math.PI
//        , new ComplexField.ContourIntegral.fPtr_U_or_V_(genericIntegrand_u_part)
//        , new ComplexField.ContourIntegral.fPtr_U_or_V_(genericIntegrand_v_part)
//        , new ComplexField.ContourIntegral.fPtr_Jordan_parametriz_(x)
//        , new ComplexField.ContourIntegral.fPtr_Jordan_parametriz_(y)
//        , new ComplexField.ContourIntegral.fPtr_Jordan_parametriz_(dx)
//        , new ComplexField.ContourIntegral.fPtr_Jordan_parametriz_(dy)
//        , 99999); // # dx

////double areaTrapezio =
////    RealField.Integrate.Integrate_equi_trapezium(0, 2, 9);

//// TODO : memo per change extrema parameters :

////In[7]:= x[t_] = t
////In[8]:= y[t_] = (2*t + 1)

////In[12]:= {x[0], y[0]}
////Out[12]= {0, 1}

////In[11]:= {x[11], y[11]}
////Out[11]= {11, 23}

////ComplexField.Complex res =
////    ComplexField.ContourIntegral.ContourIntegral_ManagementMethod(0.0, 11.0, 999);

////ComplexField.Complex res =
////     ComplexField.GammaViaIntegral.GammaSpecialF_viaIntegral(+4.0, +0.0, 9999, 99000);// successo pieno!!!
////// ComplexField.GammaViaIntegral.GammaSpecialF_viaIntegral( -3.9, +11.85, 9999, 20000);// NO : Re[s]>0 
//////ComplexField.GammaViaIntegral.GammaSpecialF_viaIntegral(+3.9, +11.85, 9999, 990000);// errore == 1.31926*10^-7 - 5.8894*10^-8 \[ImaginaryI]  OK
////// ComplexField.GammaViaIntegral.GammaSpecialF_viaIntegral(+39.9, +181.15, 9999, 999999999 );// 
////ComplexField.Complex prodGamma = ComplexField.Functions.Gamma_function(new ComplexField.Complex(+4.0, +0.0), 99000);// NB! converge anche per Re[s]<0

//long[,] num = new long[2, 2]{{+2,+1}, {+3,+2}};
//long[,] den = new long[2, 2]{{+2,+1}, {+3,+1}};
//RealField.RatioReducingInstrument Q = new RealField.RatioReducingInstrument( num, den);
//RealField.RatioReducingResults res = Q.RationalReductionOnOmegaData();



//double t = 0.0;
//double Delta_t = +1.0E-1;
//double x = default(double);
//double y = default(double);
////
//for (; t <= System.Math.PI; t += Delta_t)
//{
//    x = Math.Cos(t);// assume the Radius==+1.
//    y = Math.Sin(t);// assume the Radius==+1.
//    //
//    double arg = System.Math.Atan2(y, x);
//    double partOfPi = arg / Math.PI;
//    //
//    // output
//    System.Text.StringBuilder sb = new System.Text.StringBuilder();
//    sb.Append( "\n\t t=");
//    sb.Append( t.ToString() );
//    sb.Append( " x=Cos(t)=");
//    sb.Append( x.ToString() );
//    sb.Append( " y=Sin(t)=");
//    sb.Append( y.ToString() );
//    sb.Append( " arg=Atan2(y, x)=");
//    sb.Append( arg.ToString() );
//    sb.Append( " partOfPi=arg/Math.PI=");
//    sb.Append( partOfPi.ToString() );
//    string theMessage = sb.ToString();
//    LogSinkFs.Wrappers.LogWrappers.SectionContent(theMessage, 0);
//    Console.WriteLine("\n\t t={0} x=Cos(t)={1} y=Sin(t)={2} arg=Atan2(y, x)={3} partOfPi=arg/Math.PI={4}"
//        , t, x, y, arg, partOfPi);
//}// end for.
////


//double t = +0.0;
//for (double sigma = +0.81; sigma < 1.21; sigma += +0.01)
//{
//    Console.WriteLine("Zeta[{0}+I*{1}]== {2}", sigma, t,
//        ComplexField.Functions.Zeta_functionalEquation(
//            new ComplexField.Complex(sigma, t)
//            , 99999L)
//    );
//}




//In[5]:= Gamma[3.19`+ 6.2` \[ImaginaryI]]
//Out[5]= -0.0172331 + 0.0130457 \[ImaginaryI] 
//Console.WriteLine(ComplexField.Functions.Gamma_function(
//    new ComplexField.Complex(+3.19, +6.2)
//    , 99999L
//));

// TODO PrimesFinder.Integrate.


//PrimesFinder.Primes primeCoreClassInstance = new PrimesFinder.Primes();
////for (int c = 100; c < 1000; c += 100)
//// 2.000.000.000
//// 3.000.000.000
////   100.000.000
//for (long c = 2; c < 300; c += 1 )
//{
//    double tmpCalc_Pi_calculated_onJ = primeCoreClassInstance.Pi_calculated_onJ(c);
//    double tmpCalc_Pi_Greco = primeCoreClassInstance.Pi_Greco(c);
//    Console.WriteLine(" n in N, n={0} \t P(J({1}))={2} \t Pi_Greco({1})={3} \t Round[P(J({1}))]={4} \t Delta={5}"
//         , c, c, tmpCalc_Pi_calculated_onJ
//         , tmpCalc_Pi_Greco
//         //, primeCoreClassInstance.Pi_Greco_nonInterpolata(c)
//         , Math.Round(tmpCalc_Pi_calculated_onJ)
//         , tmpCalc_Pi_calculated_onJ - tmpCalc_Pi_Greco
//     );
//}

//int c = 1;
//for( ; c < 30; c++)
//{
//    Console.WriteLine(" Moebius_mi({0})== {1}",c, PrimesFinder.ElementFullCalculation.Moebius_Mi(c));
//}

//const int righe_num = 4;
//const int righe_den = 2;
//long[,] num = new long[righe_num, 2] { { +1, 7 }, { 2, 2 }, { -3, 23 }, { 5, 4 } };
//long[,] den = new long[righe_den, 2] { { 2, 9 }, { 3, 19 } };
//long[, ,] theRedFraction = PrimesFinder.ElementFullCalculation.RationalReductionOnOmegaData(num, den);

//Console.WriteLine("\r\n\r\n\r\n");
//int max_factor_cardinality = Math.Max(righe_num, righe_den);
//for( c = 0; c < righe_num; c++)
//{
//    if (c > 0)
//    {
//        Console.Write(" * ");
//    }// else don't.
//    Console.Write("{0}^{1}", num[c, 0], num[c, 1]);
//}
//Console.WriteLine();
//for (int k = 0; k < max_factor_cardinality * 3 * 2 + 1; k++)
//{
//    Console.Write("-");
//}
//Console.WriteLine();
//for (c = 0; c < righe_den; c++)
//{
//    if (c > 0)
//    {
//        Console.Write(" * ");
//    }// else don't.
//    Console.Write("{0}^{1}", den[c, 0], den[c, 1]);
//}

//LinearAlgebra.RealMatrix Mata = new LinearAlgebra.RealMatrix(3, 3, @"C:\root\projects\Calculus_2015_\TestConsole\dat\mata3x3_20170315_.txt");
//Mata.show();
//double det = Mata.det();
//LinearAlgebra.RealMatrix Mata_diagonalized_ = Mata.Gauss_Jordan_elimination();
//Mata_diagonalized_.show();

//double t = 0.0;
//double Delta_t = +1.0E-1;
//double x = default(double);
//double y = default(double);
////
//for (; t <= System.Math.PI; t += Delta_t)
//{
//    x = Math.Cos(t);// assume the Radius==+1.
//    y = Math.Sin(t);// assume the Radius==+1.
//    //
//    double arg = System.Math.Atan2(y, x);
//    double partOfPi = arg / Math.PI;
//    //
//    // output
//    System.Text.StringBuilder sb = new System.Text.StringBuilder();
//    sb.Append( "\n\t t=");
//    sb.Append( t.ToString() );
//    sb.Append( " x=Cos(t)=");
//    sb.Append( x.ToString() );
//    sb.Append( " y=Sin(t)=");
//    sb.Append( y.ToString() );
//    sb.Append( " arg=Atan2(y, x)=");
//    sb.Append( arg.ToString() );
//    sb.Append( " partOfPi=arg/Math.PI=");
//    sb.Append( partOfPi.ToString() );
//    string theMessage = sb.ToString();
//    LogSinkFs.Wrappers.LogWrappers.SectionContent(theMessage, 0);
//    Console.WriteLine("\n\t t={0} x=Cos(t)={1} y=Sin(t)={2} arg=Atan2(y, x)={3} partOfPi=arg/Math.PI={4}"
//        , t, x, y, arg, partOfPi);
//}// end for.


//double[,] proto = new double[9, 3]
//    {
//     {0.765456376022406,     0.338355595403516,       0.512278095126282}, //-
//     {0.671985504996025,     0.594393482708555,       0.12178110756063},  //-
//     {0.671985504996025,     0.594393482708555,       0.12178110756063}, 
//     {0.671985504996025,     0.594393482708555,       0.12178110756063}, 
//     {0.671985504996025,     0.594393482708555,       0.12178110756063}, 
//     {0.671985504996025,     0.594393482708555,       0.12178110756063}, 
//     {8.671985504996025,     0.594393482708555,       0.12178110756063},  //-
//     {0.671985504996025,     0.594393482708555,       0.12178110756063}, 
//     {0.671985504996025,     0.594393482708555,       0.12178110756063}
//    };

//LinearAlgebra.MatrixRank Rank_matA = new LinearAlgebra.MatrixRank(proto, 9, 3);
//int risposta_rango_matA_ = Rank_matA.Rango();
//System.Console.WriteLine("\n\n\t int risposta_rango_matA_ = {0}", risposta_rango_matA_);



////for (int c = -21; c < +10; c++)
////{
////    double par = +1.1 * Math.Pow(+10.0, (double)c);
////    Console.WriteLine(" Magnitudo_Order( {0} ) == {1} ", par, RealField.TestsOnRationals.getMagnitudoOrder(par));
////}

////RealField.TestsOnRationals.Prune_and_RationalReduce_test(99, +1E-15, false);

////string myFactoriz = "2^3 * 922^-3 *92^7*-1^3 *2^-13";
////RealField.TestsOnRationals.Prune_and_RationalReduce_test(myFactoriz, +1E-19, true);

////for (int c = 0; c < 10; c++)
////{
////    string curSignedArray = RealField.TestsOnRationals.MonteCarloExtract_singleFactorizazion();
////    RealField.TestsOnRationals.Prune_and_RationalReduce_test(curSignedArray, +1E-16, false);
////}

//long[,] mySignedArray = new long[,] { { 922, 3 }, { 97, -5 } };
//long[,] mySignedArray = new long[,] { { 3, 12 }, { -3, -23 } };
//RealField.TestsOnRationals.Prune_and_RationalReduce_test(mySignedArray, +1E-19, true);

//for (int c = -21; c < +10; c++)
//{
//    double par = +3.0*Math.Pow(+10.0, (double)c);
//    Console.WriteLine(" Magnitudo_Order( {0} ) == {1} ", par, RealField.TestsOnRationals.getMagnitudoOrder(par));
//}

//for (int c = 0; c < 100; c++)
//{
//    double thePar = Common.MonteCarlo.MonteCarlo_threadLocked.next_probabilityMeasure() * 0.1 *
//        Common.MonteCarlo.MonteCarlo_threadLocked.next_int(1, 999999);
//    Console.WriteLine("  AnalyzedEntity = {0}  MagnitudoOrder = {1}   counter=={2}", thePar, RealField.TestsOnRationals.getMagnitudoOrder( thePar), c);
//}


//
//long[,] zorro = testQ.factorizationConcatenator(a, b);

//sviluppo.Rationals_Q_ Q = new sviluppo.Rationals_Q_(" -2^3  * 7^2 * 3^4 *  2^7 * 5^3");
//Q.Prune();


//Test_RationalReduce_caller();

//string[] myArray = new string[] { "zamarro", "sanamartola", "antani", "123", "@23", "ANTANI" };
//int Array_Length = 6;
//Common.BubbleSort.Lexicography_BubbleSort mySorter = new Common.BubbleSort.Lexicography_BubbleSort(Array_Length);
//for (int c = 0; c < Array_Length; c++)
//{
//    Common.BubbleSort.LexicographyBrick curFactor = new Common.BubbleSort.LexicographyBrick();
//    curFactor.field = myArray[c];
//    mySorter.Add(curFactor, c);
//}
//mySorter.show();// pre-order
//mySorter.BubbleSort_movingEngine_();
//Console.WriteLine("\r\n\r\n##############################\r\n\r\n");
//mySorter.show();// post-order




//long[,] myArray = new long[,] { { 3, 1 }, { -1, 3 }, { -999, 3 }, { 15, 3 }, { -16, 3 }, { 2, 5 } };
//int Array_Length = 6;
//Common.BubbleSort.BubbleSort_Specific_forFactorCouple mySorter =
//    new Common.BubbleSort.BubbleSort_Specific_forFactorCouple(Array_Length);
//for (int c = 0; c < Array_Length; c++)
//{
//    Common.BubbleSort.FactorCouple curFactor = new Common.BubbleSort.FactorCouple(myArray[c, 0], myArray[c, 1]);
//    mySorter.Add(curFactor, c);
//}
//mySorter.show();// pre-order
//mySorter.BubbleSort_movingEngine_();
//Console.WriteLine("\r\n\r\n##############################\r\n\r\n");
//mySorter.show();// post-order


//long[,] myArray = new long[,] { { 3, 1 },{-2,-1}, { -3, 3 }, { -3, 2 }, { -3, -3 }, { -3, -2 } };
//Console.WriteLine("follows the original input SignedArray");
//// Console output : it's useful to call it once before the sorting and once kust after.
//for (int c = 0; c < myArray.Length/2; c++)
//{
//    Console.Write(myArray[c,0] + "^" + myArray[c,1] + " * ");
//}
//Console.WriteLine();
////
//Common.BubbleSort.BubbleSort_Specific_forFactorCouple mySorter =
//    new Common.BubbleSort.BubbleSort_Specific_forFactorCouple( myArray );
//Console.WriteLine("follows the SignedArray after BubbleSortSpecific_FactorCouple Ctor() ");
//mySorter.show();// pre-order
//mySorter.BubbleSort_movingEngine_();
//Console.WriteLine("\r\n\r\n##############################\r\n\r\n");
//Console.WriteLine("follows the SignedArray post Bubble-Sort ordering ");
//mySorter.show();// post-order
//long[,] res = mySorter.returnSignedArray();

//sviluppo.Rationals_Q_ Q = new sviluppo.Rationals_Q_("3^1 *-2^-1* -3^3 * -3^2 * -3^-3 * -3^-2");
//Q.Prune();
//// Console output : it's useful to call it once before the sorting and once just after.
//Console.WriteLine("\r\n\r\nfollows the pruned SignedArray");
//for (int c = 0; c < Q.Num.Length/2 ; c++)
//{
//    Console.Write(Q.Num[c, 0] + "^" + Q.Num[c, 1] + " * ");
//}
//Console.Write("\r\n--------------------------------------------------------\r\n");
//for (int c = 0; c < Q.Den.Length / 2; c++)
//{
//    Console.Write(Q.Den[c, 0] + "^" + Q.Den[c, 1] + " * ");
//}
//Console.WriteLine();
////
//long[,] SignedArray = RealField.Rationals_Q_.factorizationParser("3^1  *-2^-1* -3^3 * -3^2 * -3^-3 * -3^-2");
//double muledUP = RealField.Rationals_Q_.mulUp(SignedArray);
//RealField.Rationals_Q_ fractionManager = new RealField.Rationals_Q_(muledUP, +1L);
//fractionManager.ToFractionString();
//fractionManager.RationalReductionOnIntegers();
//string fractionSimplified = fractionManager.ToSimplifiedFractionString();
//Console.WriteLine("\r\n\r\nfollows the simplified fraction : " + fractionSimplified);


//public class MyRec
//{
//    public long[,] tensor_a;
//    public long[,] tensor_b;
//}
////
//public static MyRec aMethod( int aPar )
//{
//    MyRec res = new MyRec();
//    //  TODO------
//    //ready
//    return res;
//}// end : public static MyRec aMethod( int aPar ).


//int accTest = 0;
//int start_num = -1800;
//int stop_num = -1788;
//int start_den = 8266;
//int stop_den = 8268;
////
//int accRound = 0;
//for (int num = start_num; num <= stop_num; num++)
//{
//    for (int den = start_den; den <= stop_den; den++)
//    {
//        if (den != 0 && num != 0)// skip the zero, both on denominator and on numerator.
//        {// skip the zero, both on denominator and on numerator.
//            if (Program.Test_RationalReduce_(num, den))
//            {
//                accTest++;
//            }//else do not increment.
//            accRound++;
//            //Console.WriteLine("Success accumulator = {0}", accTest);
//        }// skip the zero, both on denominator and on numerator.
//        else// skip the zero, both on denominator and on numerator.
//        {// skip the zero, both on denominator and on numerator.
//            continue;// skip the zero, both on denominator and on numerator.
//        }// end : skip the zero, both on denominator and on numerator.
//    }// end for
//}// end for
//double success = (double)accTest / (double)accRound;
//Console.WriteLine(" Cardinality of run tests == {0}", accRound);
//Console.WriteLine(" Success percentage = {0}%", success * 100);


//public static long[,] factorizationParser( string factorizationString )
//{

//    string[] factors = factorizationString.Split(new char[] { '*' }, StringSplitOptions.RemoveEmptyEntries);// first round : extract pi^ei
//    long[,] res = new long[factors.Length, 2];// columns are fixed 2, since they are base and exponent.
//    //
//    for (int c = 0; c < factors.Length; c++)
//    {
//        string[] BaseExp = factors[c].Split(new char[] { '^' }, StringSplitOptions.RemoveEmptyEntries);// second round : separate pi and ei, for each couple pi^ei.
//        res[c, 0] = long.Parse(BaseExp[0]);// convert base; throws on syntax error.
//        res[c, 1] = long.Parse(BaseExp[1]);// convert exponent; throws on syntax error.
//        BaseExp = null;// garbage collect.
//    }// end for.
//    // ready.
//    return res;
//}// end public long[,] factorizationParser( string factorizationString )



//public static long mulUp( long[,] theFactorization )
//{// on integers -> only positive exponents are admitted.
//    long res = 1L;// we will multiply on it.
//    for (int c = 0; c < theFactorization.Length / 2; c++)
//    {
//        if (theFactorization[c, 1] < 0L)
//        {
//            throw new System.Exception("Only integer factorizations in this method : no negative exponents allowed.");
//        }// end throw.
//        res *= (long)System.Math.Pow(theFactorization[c, 0], theFactorization[c, 1]);// i.e. base^exp.
//    }//
//    // ready.
//    return res;
//}// end public static long addUp( long[,] theFactorization )


//string factoriz = "-1^7*  -3^3*  -2^ 1  ";
//long[,] factors = TestConsole.Program.factorizationParser(factoriz);
//long addedUp = Program.mulUp(factors);


////{ { +1, 7 }, { -3, 23 }, { 2, 19 }, { 5, 4 } };

////string[,] num = new string[2, 2] { { "num_fact_1", "num_mult_1" }, { "num_fact_2", "num_mult_2" } };
////string[,] den = new string[5, 2] { { "den_fact_1", "den_mult_1" }, { "den_fact_2", "den_mult_2" }, { "den_fact_3", "den_mult_3" }, { "den_fact_4", "den_mult_4" }, { "den_fact_5", "den_mult_5" } };
////long[,] num = new long[4, 2] { { +1, 7 }, { 2, 2 }, { -3, 23 }, { 5, 4 } };
////long[,] den = new long[2, 2] { { 2, 9 }, { 3, 19 } };
////long[,] num = new long[4, 2] { { +1, 7 }, { 2, 19 }, { -3, 23 }, { 5, 4 } };
////long[,] den = new long[3, 2] {            { 2, 9 },   { 3, 19 }, { 5, 2 } };
//long[,] num = new long[4, 2] { { +1, 7 }, { -3, 23 }, { 2, 19 }, { 5, 4 } };
//long[,] den = new long[3, 2] { { 2, 9 }, { 5, 2 }, { 3, 19 } };
////long[,] small =  new long[6, 2] { { 12, 3 }, { 3, 2 }, { 1, 1 }, { 7, 2 }, { 101, 1 }, { 17, 1 } };
////long[,] aaaa = new long[4, 2] { { -3, -3 }, { 3, 2 }, { -5, -1 }, { 5, 4 } };
////long[,] eg = new long[11, 2] { { -2, -3 }, { 2, 2 }, { 1, 3 }, { -2, -7 }, { 1, 3 }, { 1, -4 }, { 1, 3 }, { 11, 3 }, { -1, -4 }, { -2, 3 }, { 2, 7 } };
////long[,] respNum, respDen;

////RealField.Rationals_Q_.RationalReductionOnOmegaData(num, den);
////RealField.Rationals_Q_.RationalReductionOnOmegaData_old(num, den);
//RealField.Rationals_Q_ Q = new RealField.Rationals_Q_(-999, 15);
//Q.RationalReductionOnIntegers();
//Console.WriteLine(Q.ToSimplifiedFractionString());
////
//RealField.Rationals_Q_ R = new RealField.Rationals_Q_(999, -15);
//R.RationalReductionOnIntegers();
//Console.WriteLine(R.ToSimplifiedFractionString());
////
//RealField.Rationals_Q_ S = new RealField.Rationals_Q_(-999, -15);
//S.RationalReductionOnIntegers();
//Console.WriteLine(S.ToSimplifiedFractionString());
////
//RealField.Rationals_Q_ T = new RealField.Rationals_Q_(+999, +15);
//T.RationalReductionOnIntegers();
//Console.WriteLine(T.ToSimplifiedFractionString());
////



///// <summary>
///// Buble-sort based rational-Q-Prune.
///// </summary>
///// <param name="SignedArray"></param>
///// <param name="Num"></param>
///// <param name="Den"></param>
//public static void Prune( long[,] SignedArray, out long[,] Num, out long[,] Den )
//{
//    long[] tmp = new long[2];
//    bool swap = true;
//    int for_loop_accumulator = 0;
//    int signFlag = +1;
//    for (int c = 0; c < SignedArray.Length / 2; c++)//---sign flag management------
//    {
//        if (SignedArray[c, 0] < 0 // se la base e' negativa
//            && (System.Math.Abs(SignedArray[c, 1] / 2.0 - SignedArray[c, 1] / 2) > double.Epsilon) // e l'esponente e' dispari
//            )
//        {
//            signFlag *= -1;// a negative factor in the product.
//        }// else skip sign change, on positive factors.
//    }// Sign-flag management.
//    //
//    while (swap && for_loop_accumulator < SignedArray.Length / 2 - 1)// n-1 for_loops are sufficient.
//    {
//        swap = false;// reset at each new "for".
//        //
//        for (int c = 0; c < SignedArray.Length / 2 - 1; c++)
//        {
//            if (System.Math.Abs(SignedArray[c, 0]) < System.Math.Abs(SignedArray[c + 1, 0]))
//            {
//                // NO swap in this case, since the existing order is the required one.
//            }
//            else if (System.Math.Abs(SignedArray[c, 0]) > System.Math.Abs(SignedArray[c + 1, 0]))
//            {
//                tmp[0] = System.Math.Abs(SignedArray[c + 1, 0]);//---temporary is necessary to do not loose the overwritten data.
//                tmp[1] = SignedArray[c + 1, 1];
//                //
//                SignedArray[c + 1, 0] = System.Math.Abs(SignedArray[c, 0]);
//                SignedArray[c + 1, 1] = SignedArray[c, 1];
//                //
//                SignedArray[c, 0] = tmp[0];
//                SignedArray[c, 1] = tmp[1];
//                //
//                swap = true;// this is the real swap. If it takes place at least once in a "for", the pruning is still active.
//            }
//            else if (System.Math.Abs(SignedArray[c, 0]) == System.Math.Abs(SignedArray[c + 1, 0]))
//            {
//                SignedArray[c, 0] = System.Math.Abs(SignedArray[c, 0]);
//                SignedArray[c, 1] = SignedArray[c, 1];
//                //
//                SignedArray[c + 1, 0] = System.Math.Abs(SignedArray[c + 1, 0]);
//                SignedArray[c + 1, 1] = SignedArray[c + 1, 1];
//                //
//                // non si falsifica se è stato almeno una volta vero nel "for", quindi non mettere l'istruzione: swap = false
//                // resta a false Se_solo non stato vero neanche una volta in tutto il "for".
//            }
//        }// for
//        //
//        for_loop_accumulator++;// count the cardinality of the pruning loops : each "for" is a pruning turn.
//    }// while swap &&...
//    //-- end Bubble-Sort
//    //---start compiling numerator and denominator, based on the BubbleSorted SignedArray.
//    long prevNumBase = +1L;
//    long curNumBase = +1L;
//    int numeratorFactorCardinality = +1;
//    long prevDenBase = +1L;
//    long curDenBase = +1L;
//    int denominatorFactorCardinality = +1;
//    for (int c = 0; c < SignedArray.Length / 2; c++)
//    {
//        if (SignedArray[c, 1] > 0)// evaluate exponent's sign.
//        {
//            curNumBase = SignedArray[c, 0];
//            if (curNumBase > +1L && curNumBase != prevNumBase)
//            {
//                numeratorFactorCardinality++;
//            }// else : the product invariant doesn't count.
//            prevNumBase = curNumBase;// update.
//        }
//        else if (SignedArray[c, 1] < 0)// evaluate exponent's sign.
//        {
//            curDenBase = SignedArray[c, 0];
//            if (curDenBase > +1L && curDenBase != prevDenBase)
//            {
//                denominatorFactorCardinality++;
//            }// else : the product invariant doesn't count.
//            prevDenBase = curDenBase;// update.
//        }
//        else //  (SignedArray[c, 1] == 0) a productInvariant(i.e. +1) gets no place in numerator neither in denominator.
//        {
//            // do nothing.
//        }
//    }// end num & denom cardinality evaluation.
//    Num = new long[numeratorFactorCardinality, 2];//  raggruppare i fattori ripetuti; già aggiunta una posiz. per l'uno.
//    Den = new long[denominatorFactorCardinality, 2];// raggruppare i fattori ripetuti; già aggiunta una posiz. per l'uno.
//    prevNumBase = +1L;
//    curNumBase = +1L;
//    prevDenBase = +1L;
//    curDenBase = +1L;
//    int curGroupedElement_Num = 0;
//    int curGroupedElement_Den = 0;
//    //
//    Num[curGroupedElement_Num, 0] = signFlag;
//    Num[curGroupedElement_Num, 1] = +1L;
//    curGroupedElement_Num++; // invariant inserted.
//    //
//    Den[curGroupedElement_Den, 0] = +1L;
//    Den[curGroupedElement_Den, 1] = +1L;
//    curGroupedElement_Den++; // invariant inserted.
//    //
//    for (int c = 0; c < SignedArray.Length / 2; c++)
//    {
//        if (SignedArray[c, 1] > 0)// evaluate exponent's sign.
//        {
//            curNumBase = SignedArray[c, 0];
//            if (curNumBase > +1L && curNumBase != prevNumBase)
//            {
//                Num[curGroupedElement_Num, 0] = curNumBase;
//                Num[curGroupedElement_Num, 1] = SignedArray[c, 1];
//                curGroupedElement_Num++; // new base found.
//            }// else : there is already such a bese -> Sum exponents.
//            else
//            {
//                if (curGroupedElement_Num - 1 > 0 && SignedArray[c, 0] != +1)
//                {
//                    Num[curGroupedElement_Num - 1, 1] += SignedArray[c, 1];// NB. : +=  -> Sum exponents on prev(i.e. -1 not a next position, the same prev) base.
//                }// else don't chenge exponent to the signFlag.
//            }
//            prevNumBase = curNumBase;// update.
//        }
//        else if (SignedArray[c, 1] < 0)// evaluate exponent's sign.
//        {
//            curDenBase = SignedArray[c, 0];
//            if (curDenBase > +1L && curDenBase != prevDenBase)
//            {
//                Den[curGroupedElement_Den, 0] = curDenBase;
//                Den[curGroupedElement_Den, 1] = SignedArray[c, 1];
//                curGroupedElement_Den++; // new base found.
//            }// else : there is already such a bese -> Sum exponents.
//            else
//            {
//                if (curGroupedElement_Den - 1 > 0 && SignedArray[c, 0] != +1)
//                {
//                    Den[curGroupedElement_Den - 1, 1] += SignedArray[c, 1];// NB. : +=  -> Sum exponents on prev(i.e. -1 not a next position, the same prev) base.
//                }// else don't chenge exponent to the signFlag.
//            }
//            prevDenBase = curDenBase;// update.
//        }
//        else //  (SignedArray[c, 1] == 0) a productInvariant(i.e. +1) gets no place in numerator neither in denominator.
//        {
//            // do nothing.
//        }
//    }// end num & denom cardinality evaluation.
//    // ready.
//}// end public static void Prune( long[,] SignedArray, out long[,] Num, out long[,] Den )



///// <summary>
///// Pattern Recognition
///// </summary>
///// <param name="x"></param>
///// <returns></returns>
//public static string RationalRepresentation( double x )
//{
//    string res = "";
//    string x_in_txt = x.ToString();
//    string[] intPart_fractPart = x_in_txt.Split(new char[2] { '.', ',' }, StringSplitOptions.RemoveEmptyEntries);
//    char curChar, prevChar;
//    int[] changeBits = new int[intPart_fractPart[1].Length];// analyze figure change only in the fractionary part.
//    for (int k = 0; k < changeBits.Length; k++) { changeBits[k] = 1; }// init.
//    prevChar = '#';// first decimal figure is always of change.
//    for (int c = 0; c < intPart_fractPart[1].Length; c++)
//    {// analyze figure change only in the fractionary part.
//        curChar = (char)intPart_fractPart[1][c];
//        if (curChar != prevChar )
//        {// the last digit has not to seem periodic, until it really is.
//            changeBits[c] = 1;
//        }
//        else if (curChar == prevChar )
//        {
//            changeBits[c] = 0;
//        }
//        prevChar = curChar;// update
//    }// end for 
//    // ready.
//    return res;
//}// end public static string RationalRepresentation( double x )

// RationalRepresentation(54E+99); NB. solo 16 figure rappresentative.
//RationalRepresentation(18.123123123);



//long[, ,] theRedFraction = PrimesFinder.ElementFullCalculation.RationalReductionOnOmegaData(num, den);

//Console.WriteLine("\r\n\r\n\r\n");
//int righe_num = 4;
//int righe_den = 6;
//int max_factor_cardinality = Math.Max(righe_num, righe_den);
//int c = 0;
//for (; c < righe_num; c++)
//{
//    if (c > 0)
//    {
//        Console.Write(" * ");
//    }// else don't.
//    Console.Write("{0}^{1}", num[c, 0], num[c, 1]);
//}
//Console.WriteLine();
//for (int k = 0; k < max_factor_cardinality * 3 * 2 + 1; k++)
//{
//    Console.Write("-");
//}
//Console.WriteLine();
//for (c = 0; c < righe_den; c++)
//{
//    if (c > 0)
//    {
//        Console.Write(" * ");
//    }// else don't.
//    Console.Write("{0}^{1}", den[c, 0], den[c, 1]);
//}

////#################
//Console.WriteLine("\r\n\r\n\r\n");
//int tensorRows = theRedFraction.Length / (2 * 2);// rows' number.
//c = 0;
//int emptyCountr = 0;
//for (; c < tensorRows; c++)
//{
//    if (c > 0 && theRedFraction[0, c, 0] != 0)
//    {
//        Console.Write(" * ");
//    }// else don't.
//    if (theRedFraction[0, c, 0] != 0)
//    {
//        Console.Write("{0}^{1}", theRedFraction[0, c, 0], theRedFraction[0, c, 1]);
//    }// else don't.
//    else { emptyCountr++; }
//}
//Console.WriteLine();
//for (int k = 0; k < (tensorRows - emptyCountr) * 3 * 2 + 1; k++)
//{
//    Console.Write("-");
//}
//Console.WriteLine();
//for (c = 0; c < tensorRows; c++)
//{
//    if (c > 0 && theRedFraction[1, c, 0] != 0)
//    {
//        Console.Write(" * ");
//    }// else don't.
//    if (theRedFraction[1, c, 0] != 0)
//    {
//        Console.Write("{0}^{1}", theRedFraction[1, c, 0], (-1) * theRedFraction[1, c, 1]);
//    }// else don't.
//}


//PrimesFinder.Primes P = new PrimesFinder.Primes();
//for (double x = 60.0; x < 70; x += 1)
//{
//    Console.WriteLine(" Pi_Greco_nonInterpolata({0}) == {1}", x, P.Pi_Greco_nonInterpolata(x));
//    Console.WriteLine(" J_nonInterpolata({0}) == {1}", x, P.J_nonInterpolata(x));
//    // Console.WriteLine(" J({0}) == {1}", x, P.J(x));
//}
//PrimesFinder.TestsOnNumberTheory.PiGreco_and_J();

//PrimesFinder.TestsOnNumberTheory.Tests_on_Omegas_Mi_Lambda();

//int omega_small = PrimesFinder.ElementFullCalculation.Omega_small(0L);
//long omega_big = PrimesFinder.ElementFullCalculation.Omega_big(0L);
//int Lambda = PrimesFinder.ElementFullCalculation.Lambda_Liouville(0L);

//PrimesFinder.TestsOnNumberTheory.PrintFactorization(1L);
//PrimesFinder.TestsOnNumberTheory.PrintFactorization(3L * 3L * 3L * 3L * 3L);
//PrimesFinder.TestsOnNumberTheory.PrintFactorization(3L);
//PrimesFinder.TestsOnNumberTheory.PrintFactorization(5L);
//PrimesFinder.TestsOnNumberTheory.PrintFactorization(19L);
//PrimesFinder.TestsOnNumberTheory.PrintFactorization(2L);
////int omega_small = PrimesFinder.ElementFullCalculation.Omega_small(302870L);
////long omega_big = PrimesFinder.ElementFullCalculation.Omega_big(302870L);
////int Lambda = PrimesFinder.ElementFullCalculation.Lambda_Liouville(302870L);
////int DiracDelta_onOmegas = PrimesFinder.ElementFullCalculation.DiracDelta_Omegas(302870L);
////DiracDelta_onOmegas = PrimesFinder.ElementFullCalculation.DiracDelta_Omegas(302870L);

//System.Console.WriteLine("\n\n\n");
//for (int c = 1; c < 100; c++)
//{
//    System.Console.WriteLine("Omega_small({0}) == {1}", c, PrimesFinder.ElementFullCalculation.Omega_small(c));
//}
//System.Console.WriteLine("\n\n\n");
//for (int c = 1; c < 100; c++)
//{
//    System.Console.WriteLine("Omega_big({0}) == {1}", c, PrimesFinder.ElementFullCalculation.Omega_big(c));
//}
//System.Console.WriteLine("\n\n\n");
//for (int c = 1; c < 100; c++)
//{
//    System.Console.WriteLine("Mi_Moebius({0}) == {1}", c, PrimesFinder.ElementFullCalculation.Moebius_Mi( c) );
//}
//System.Console.WriteLine("\n\n\n");
//for (int c = 1; c < 100; c++)
//{
//    System.Console.WriteLine("Mertens_M({0}) == {1}", c, PrimesFinder.ElementFullCalculation.Mertens_M(c));
//}
//PrimesFinder.TestsOnNumberTheory.PrintFactorization(4096L);

//long[,] myOmegaData = PrimesFinder.ElementFullCalculation.OmegaData( 2L*2L*2L * 3L*3L*3L*3L*3L*3L*3L* 5L*5L*5L*5L*5L*5L*5L*5L*5L  );
//double[] theProd = new double[myOmegaData.Length / 2];
//double ToTProd = 1.0;
//System.Console.WriteLine("\n\n");
//System.Console.WriteLine("\t|-----------------------------------------------------");
//for (int c = 0; c < myOmegaData.Length / 2; c++)
//{
//    theProd[c] = Math.Pow( (double)myOmegaData[c, 0], (double)myOmegaData[c, 1] );
//    System.Console.WriteLine("\t|p^k\t==\t{0}^{1}\t==\t{2}", myOmegaData[c, 0], myOmegaData[c, 1], theProd[c]);
//    ToTProd *= theProd[c];
//    System.Console.WriteLine("\t|-----------------------------------------------------");
//}
//System.Console.WriteLine("\t|\ttotal facor product\t==\t{0}", ToTProd);
//System.Console.WriteLine("\t|-----------------------------------------------------");

//    //long dividendo, LastCandidateDivider, Quozienteintero;
//    //dividendo = 27;

//    ////long resto = dividendo % 5L;
//    ////double resto_d = 13.1415 % 5.1415;

//    //bool isPrime = PrimesFinder.ElementFullCalculation.ElementFullEvaluation(dividendo, out LastCandidateDivider, out Quozienteintero);

//    //try
//    //{
//    //    string response = Process.Calculation.FactorResearcher(dividendo);
//    //}
//    //catch (System.Exception ex)
//    //{
//    //    System.Exception innerEx = ex.InnerException;
//    //    string msg = innerEx.Message;
//    //    string source = innerEx.Source;
//    //    string toString = innerEx.ToString();
//    //}

//Random myGenerator = new Random();
//double epsilon = myGenerator.NextDouble();
////for (int c = 0; c < 100; c++)
////{
////    Console.WriteLine("\n\t Epsilon == {0} ", myGenerator.NextDouble());
////}

//double previousPiG_Level = 0.0;
//double PiG_Level = 0.0;
//PrimesFinder.Primes P = new PrimesFinder.Primes();// default Ctor is the one for reading the log.
//double Pi_64 = P.Pi_Greco(64);
//double J_64 = P.J(64); // NB. J(x)>=Pi(x)


//for (long thresholdInNaturals = 0; thresholdInNaturals < 100; thresholdInNaturals++)
//{
//    double pollutedInteger = thresholdInNaturals + myGenerator.NextDouble();
//    //Console.Write("\t GetCumulatedOrdinalNotOverThreshold({0}) == {1}  the Inf={2}"
//    //    , thresholdInNaturals
//    //    , P.GetCumulatedOrdinalNotOverThreshold(thresholdInNaturals, out theInf)
//    //    , theInf
//    //    );
//    PiG_Level = P.Pi_Greco(thresholdInNaturals);
//    if (Math.Abs(PiG_Level - previousPiG_Level) > double.Epsilon)
//    {
//        Console.WriteLine();
//    }// else continue on the same line;
//    previousPiG_Level = PiG_Level;// update the PiGreco level.
//    Console.Write("____\t PiGreco({0}) == {1} "
//        , thresholdInNaturals
//        , P.Pi_Greco(thresholdInNaturals)
//        );
//    PiG_Level = P.Pi_Greco(pollutedInteger);
//    if (Math.Abs(PiG_Level - previousPiG_Level) > double.Epsilon)
//    {
//        Console.WriteLine();
//    }// else continue on the same line;
//    previousPiG_Level = PiG_Level;// update the PiGreco level.
//    //
//    Console.Write("____\t PiGreco({0}) == {1}"
//        , pollutedInteger
//        , P.Pi_Greco(pollutedInteger )
//        );
//    //
//    Console.Write("____\t J({0}) == {1}"
//        , pollutedInteger
//        , P.J(pollutedInteger) // NB. J(x)>=Pi(x)
//        );
//}

//long Pi_99_ = primes.GetCumulatedOrdinalNotOverThreshold(99);
//long Pi_100_ = primes.GetCumulatedOrdinalNotOverThreshold(100L);
//long Pi_97_ = primes.GetCumulatedOrdinalNotOverThreshold(97L);
////
//long[] testL = new long[100];
//for (int c = 0; c < 100; c++)
//{
//    testL[c] = primes.GetCumulatedOrdinalNotOverThreshold( (long)(c+100) );
//}


//double Pi_99_58 = primes.PiGreco(99.58);
//double Pi_100_05 = primes.PiGreco(100.05);
//double Pi_97_00 = primes.PiGreco(97.0);
////
//double Pi_1_99 = primes.PiGreco(1.99);
//double Pi_2_00 = primes.PiGreco(2.0);
//double Pi_2_01 = primes.PiGreco(2.01);
//
//long threshold = 999999999L;
//primes.readPreviousRecordfromAnywhere();

//double[] Euler_prod = primes.Euler_product( threshold);
//double[] Dirich_sum = primes.Dirichlet_sum( threshold);
//double[] delta = new double[2];
//delta[0] = Euler_prod[0] - Dirich_sum[0];
//delta[1] = Euler_prod[1] - Dirich_sum[1];
//System.Console.WriteLine("\n\t soglia={0} , Errore={1} +i* {2} ", thres

// time distances can be calculated both with LocalizedSingleDate and with GenericSingleDate (i.e. the father). When the ZeitRaum is invoked from the son class, the father's method is executed the same way as from father's instances.
//Common.CalendarLib.LocalizedSingleDate left =  new Common.CalendarLib.LocalizedSingleDate(11, 12, 2016, "ITCAL", 2016, 2017);
//Common.CalendarLib.LocalizedSingleDate right = new Common.CalendarLib.LocalizedSingleDate(26, 03, 2017, "ITCAL", 2016, 2017);
//int ZeitRaum = right - left;
//Common.CalendarLib.GenericSingleDate leftBound = new Common.CalendarLib.GenericSingleDate(11, 12, 2016);
//Common.CalendarLib.GenericSingleDate rightBound = new Common.CalendarLib.GenericSingleDate(26, 03, 2017);
//int daysElapsed = rightBound - leftBound;

//LinearAlgebra.RealMatrix Mata = new LinearAlgebra.RealMatrix(3, 3, @"C:\root\projects\Calculus_2015_\TestConsole\dat\mata3x3_20170315_.txt");
//double det = Mata.det();
//LinearAlgebra.RealMatrix Mata_diagonalized_ = Mata.Gauss_Jordan_elimination();
//Mata_diagonalized_.show();
//Mata.inverse().show();

//// comparazione fra Prodotto di Eulero e Serie di Dirichlet.
//PrimesFinder.Primes primes = new PrimesFinder.Primes( +2.0, 0.0 );
//long threshold = 999999999L;
//double[] Euler_prod = primes.Euler_product( threshold);
//double[] Dirich_sum = primes.Dirichlet_sum( threshold);
//double[] delta = new double[2];
//delta[0] = Euler_prod[0] - Dirich_sum[0];
//delta[1] = Euler_prod[1] - Dirich_sum[1];
//System.Console.WriteLine("\n\t soglia={0} , Errore={1} +i* {2} ", threshold, delta[0], delta[1]);

//static double ExpMinusXSinX(double x)
//{
//    return Math.Exp(-1.0 * x) * Math.Sin(x);
//}// end f

//    int ENNE = 999;
//    for (int c = 0; c < ENNE; c++)
//    {
//        double image = ExpMinusXSinX((double)c);
//        string shiftingToken = "";
//        if( image > +1.0E-200) // assume it's positive
//        {
//            shiftingToken = "___________________________________POSITIVE_______________";
//        }
//        else if( image < -1.0E-200) // assume it's negative
//        {
//            shiftingToken = "_NEGATIVE_";
//        }
//        else // assume it's zero(==0)
//        {
//            shiftingToken = "_ZERO_";
//        }
//        System.Console.WriteLine("\n\t ExpMinusXSinX({0}) == {1} {2}", (double)c, image, shiftingToken );
//    }// for
//
//Common.CalendarLib.LocalizedSingleDate ld = new Common.CalendarLib.LocalizedSingleDate(1, 4, 2017, "ITCAL", 2017, 2020);
//string myNation = ld.CurrentNationFullName();
//int myJday = ld.Jday();
//Common.CalendarLib.LocalizedSingleDate.WeekDays myWeekDay = ld.WeekDayName();
//Common.CalendarLib.LocalizedSingleDate theDateThatWillBe = ld.Workd(988);


//Common.CalendarLib.GenericSingleDate gd = new Common.CalendarLib.GenericSingleDate(1, 4, 2017);
//Common.CalendarLib.GenericSingleDate myTranslatedDate = gd.AddTime( 988, 'd', Common.CalendarLib.Rules.EndOfMonth_methods.ForceEnd, true);// true stands for: use leap.
//int GmyJday = gd.Jday();
//Common.CalendarLib.LocalizedSingleDate.WeekDays GmyWeekDay = gd.WeekDayName();
//// Common.CalendarLib.LocalizedSingleDate theDateThatWillBe = ld.Workd(988); this is just what you can not do using GenericSingleDate in place of LocalizedSingleDate.

//Common.CalendarLib.GenericSingleDate leftBound =  new Common.CalendarLib.GenericSingleDate( 1,  4, 2017);
//Common.CalendarLib.GenericSingleDate rightBound = new Common.CalendarLib.GenericSingleDate(14, 12, 2019);
//int daysElapsed = rightBound - leftBound;

//RealField.JensenInequality_ Jensen = new RealField.JensenInequality_( 1.7398, 2.987654,
//    new RealField.JensenInequality_.Function_pointer( TestConsole.Program.f ),
//    .1
//    );
////double[,] thePoints = Jensen.ClassManager();
//for( double t=0.0; t<=1.0; t+=0.01)
//{
//    //System.Console.WriteLine( " parameter \"t\"=={0}, Jensen.Abscissas( t )=={1},, Jensen.reverse_Abscissas( t )=={2}",
//    //            t,
//    //            Jensen.Abscissas( t ),
//    //            Jensen.reverse_Abscissas( t )
//    //        );
//    //
//    System.Console.WriteLine( " parameter \"t\"=={0}, Jensen.Abscissas( t )=={1},, Jensen.Transform_on_Integral( t )=={2}",
//                t,
//                Jensen.reverse_Abscissas( t ),
//                Jensen.Transform_on_Integral( t)
//            );
//}
//private static double y1_equals_x2_plus_y2_(double xn, double yn)
//{
//    return (
//        System.Math.Pow(xn, 2.0)
//        + System.Math.Pow(yn, 2.0)
//    );
//}// end numerical y'[x]==f(x,y[x]).


///// <summary>
/////  TODO : let it a function pointer
/////  whether it's a convex xor concave function, the inequality changes direction
///// </summary>
///// <param name="x"></param>
///// <returns></returns>
//public static double f( double x )
//{
//    return Math.Exp( x );// TODO : let it a function pointer
//}// end f()


/*
 * 
 *             //////------Image Sequence----------------------------------------------------------------------------------------------------------------------------------------------------------
            ////RealField.AvailableAlgorithms.SequenceEngine_McLaurinExp instanceMcLaurinExp = new RealField.AvailableAlgorithms.SequenceEngine_McLaurinExp();
            ////RealField.WeierstrassContinuity.SequenceModelXK theCurrentImageSequence = new RealField.WeierstrassContinuity.SequenceModelXK( instanceMcLaurinExp.f_SequenceEngine_McLaurinExp );
            //////----END--Image Sequence----------------------------------------------------------------------------------------------------------------------------------------------------------
            ////
            //////------Image Sequence----------------------------------------------------------------------------------------------------------------------------------------------------------
            ////RealField.AvailableAlgorithms.SequenceEngine_E instanceSequenceEngine_E = new RealField.AvailableAlgorithms.SequenceEngine_E();
            ////RealField.WeierstrassContinuity.SequenceModelXK theCurrentImageSequence = new RealField.WeierstrassContinuity.SequenceModelXK( instanceSequenceEngine_E.f_SequenceEngine_E );
            //////----END--Image Sequence----------------------------------------------------------------------------------------------------------------------------------------------------------
            //////-----------------------------continuousFunctionInstance---------------------------------------------------------
            ////RealField.AvailableAlgorithms.SinX continuousFunctionInstance = new RealField.AvailableAlgorithms.SinX();
            ////RealField.WeierstrassContinuity.Functional_form theCurrentFunction = new RealField.WeierstrassContinuity.Functional_form( continuousFunctionInstance.f_SinX );// point to f(x)
            //////--------------------------END---continuousFunctionInstance---------------------------------------------------------
            //////-----------------------------continuousFunctionInstance---------------------------------------------------------
            ////RealField.AvailableAlgorithms.Bisettrice continuousFunctionInstance = new RealField.AvailableAlgorithms.Bisettrice();
            ////RealField.WeierstrassContinuity.Functional_form theCurrentFunction = new RealField.WeierstrassContinuity.Functional_form( continuousFunctionInstance.f_Bisettrice );//point to f(x)
            //////--------------------------END---continuousFunctionInstance---------------------------------------------------------
            ////
            //////-----------------------------continuousFunctionInstance---------------------------------------------------------
            //RealField.AvailableAlgorithms.SetteSinSetteX continuousFunctionInstance = new RealField.AvailableAlgorithms.SetteSinSetteX();
            //RealField.WeierstrassContinuity.Functional_form theCurrentFunction = new RealField.WeierstrassContinuity.Functional_form( continuousFunctionInstance.f_SetteSinSetteX );//point f(x).
            //////--------------------------END---continuousFunctionInstance---------------------------------------------------------
            ////
            ////-------------------------------------- Ctor-----
            //RealField.WeierstrassContinuity Wc = new RealField.WeierstrassContinuity(
            //    +0.0, // left domain boundary
            //    +2.0*Math.PI, // right domain boundary
            //    19200, // point cardinality in the domain
            //    theCurrentFunction, // f
            //    //---------------------------------------------------------------------------------------------------------------
            //    +5.4,// desired objective point, in the Image, to converge to with the sequence.---------------------------------
            //    //---------------------------------------------------------------------------------------------------------------
            //    19  // Image sequence point cardinality
            //    ,RealField.WeierstrassContinuity.ImageSequenceChoice.McLaurinExp ,
            //    +1.0E-3 // tolerance in comparisons yn=?=f(xn)
            //  ); // ------------------------------------------------------------------------------end-------------------------- Ctor------
            //Wc.SequenceManager();
 * 
 * 
 * 
 * 
 * 
 */


/*
 * 
 * Runge Kutta tests
 * 
 *   /// <summary>
 *   /// instance maker.
 *   /// </summary>
 *   class Program
 *   {
 * 
 *       /// <summary>
 *       /// y'[x]==y[x]
 *       /// </summary>
 *       /// <param name="xn"></param>
 *       /// <param name="yn"></param>
 *       /// <returns></returns>
 *       private static double y1_equals_y(double xn, double yn)
 *       {
 *           return yn;
 *       }// end numerical y'[x]==f(x,y[x]).
 * 
 * 
 *       /// <summary>
 *       /// y'[x]==x^2+(y[x])^2
 *       /// </summary>
 *       /// <param name="xn"></param>
 *       /// <param name="yn"></param>
 *       /// <returns></returns>
 *       private static double y1_equals_x2_plus_y2_(double xn, double yn)
 *       {
 *           return (
 *               System.Math.Pow(xn, 2.0)
 *               + System.Math.Pow(yn, 2.0)
 *           );
 *       }// end numerical y'[x]==f(x,y[x]).
 * 
 * 
 *       static void Main(string[] args)
 *       {
 *           numerical_ODE_lib_.SpezzateEulero ode_Eulero = new numerical_ODE_lib_.SpezzateEulero(
 *               0.9299999999999999, 25.10717785326667,
 *               0.01,
 *               new numerical_ODE_lib_.SpezzateEulero.FPointer( y1_equals_x2_plus_y2_)
 *           );
 *           System.Collections.ArrayList res_Eulero = ode_Eulero.scatter(100);
 *           ode_Eulero.format( res_Eulero);
 *           //
 *           //-------------------------------------------------
 *           //
 *           numerical_ODE_lib_.Runge_Kutta_4_ ode_Runge_Kutta = new numerical_ODE_lib_.Runge_Kutta_4_(
 *               0.9299999999999999, 25.10717785326667,
 *               0.01,
 *               new numerical_ODE_lib_.Runge_Kutta_4_.FPointer( y1_equals_x2_plus_y2_)
 *           );
 *           System.Collections.ArrayList res_Runge_Kutta = ode_Runge_Kutta.scatter(100);
 *           ode_Runge_Kutta.format(res_Runge_Kutta);
 *           //
 *           // ready.
 *           System.Console.ReadLine();
 *       }// end main.
 * 
 *   }// end class Program( main).
 * 
 */

//RealField.AvailableAlgorithms.SequenceEngine_McLaurinExp mcLaurinExp = new RealField.AvailableAlgorithms.SequenceEngine_McLaurinExp();
//RealField.AvailableAlgorithms.SequenceEngineSinX mcLaurinSin = new RealField.AvailableAlgorithms.SequenceEngineSinX();
//RealField.WeierstrassContinuity.SequenceModelXK theTestSequence = new RealField.WeierstrassContinuity.SequenceModelXK( mcLaurinExp.f_SequenceEngine_McLaurinExp );
//double x = +1.0;
//for ( int k=0; k < 15; k++ )
//{
//    System.Console.WriteLine( "\n\t  kappa={0}  McLaurinE(x,kappa)==McLaurinE({1},{0})={2}", k, x, theTestSequence( x, k ) );
//}
//x = 5.4;
//for ( int k=0; k < 45; k++ )
//{
//    System.Console.WriteLine( "\n\t  kappa={0}  McLaurinE(x,kappa)==McLaurinE({1},{0})={2}   __   ERR=={3}", k, x, theTestSequence( x, k ), theTestSequence( x, k )-Math.Exp(x) );
//}
//RealField.WeierstrassContinuity.SequenceModelXK theTestSequence = new RealField.WeierstrassContinuity.SequenceModelXK( mcLaurinSin.f_SequenceEngineSinX );
//double x = +0.0;
//for ( int k=0; k < 15; k++ )
//{
//    System.Console.WriteLine( "\n\t  kappa={0}  mcLaurinSin(x,kappa)==mcLaurinSin({1},{0})={2}", k, x, theTestSequence( x, k ) );
//}
//System.Console.WriteLine( "\n\n\n" );
//x = (5.0/2.0) * Math.PI;
//for ( int k=0; k < 20; k++ )
//{
//    System.Console.WriteLine( "\n\t  kappa={0}  mcLaurinSin(x,kappa)==mcLaurinSin({1},{0})={2}   __   ERR=={3}",
//        k, x, theTestSequence( x, k ), theTestSequence( x, k ) - Math.Sin( x ) );
//}

//double target = +1.0E-4;
//double Arg_in_McLarinExp = Math.Log( target );
////
//target = +1.0E-5;
//Arg_in_McLarinExp = Math.Log( target );
////
//target = +1.0E-6;
//Arg_in_McLarinExp = Math.Log( target );
////
//target = +1.0E-7;
//Arg_in_McLarinExp = Math.Log( target );
////
//target = +1.0E-8;
//Arg_in_McLarinExp = Math.Log( target );
////
//target = +1.0E-9;
//Arg_in_McLarinExp = Math.Log( target );
////
//target = +1.0E-10;
//Arg_in_McLarinExp = Math.Log( target );
////
//target = +1.0E-11;
//Arg_in_McLarinExp = Math.Log( target );
////
//target = +1.0E-12;
//Arg_in_McLarinExp = Math.Log( target );
////
//target = +1.0E-13;
//Arg_in_McLarinExp = Math.Log( target );
////
//target = +1.0E-14;
//Arg_in_McLarinExp = Math.Log( target );
////
//target = +1.0E-15;
//Arg_in_McLarinExp = Math.Log( target );
////
//target = +1.0E-16;
//Arg_in_McLarinExp = Math.Log( target );
////
//target = +1.0E-17;
//Arg_in_McLarinExp = Math.Log( target );


//RealField.AvailableAlgorithms.SetteSinSetteX da = new RealField.AvailableAlgorithms.SetteSinSetteX();
//RealField.WeierstrassContinuity.Functional_form theCurrentFunction = new RealField.WeierstrassContinuity.Functional_form( da.f_SetteSinSetteX );
//Console.WriteLine( theCurrentFunction.Target.ToString() );

//Console.WriteLine( "\n\t SequenceEngine_McLaurinExp(ridotta_{0}) =={1}", 18,  TestConsole.Program.SequenceEngine_McLaurinExp( 18 ) );
//Console.WriteLine( "\n\t SequenceEngine_E( index_{0}) =={1}"           , 18, TestConsole.Program.SequenceEngine_E( 18 ) );

//for ( double c=0; c < 25; c++ )
//{
//    Console.WriteLine( "\n\t double_factorial({0}) == {1}  \t double_factorial({0}) == {2}", c, Factorial( c ), Factorial( (int)c) );
//}
//for ( int c=100; c < 150; c++ )
//{
//    Console.WriteLine( "\n\t SequenceEngine_E({0}) == {1}", c, SequenceEngine_E( c ) );
//}

//LinearAlgebra.RealMatrix matA = new LinearAlgebra.RealMatrix( 6, 6, true, null );
//LinearAlgebra.RealMatrix matB = matA.inverse();
//LinearAlgebra.RealMatrix matC = matA.operator_mul( matB );
//matA.show();
//matB.show();
//matC.show( +5.0E-13 );
//matC.show(  );
//
//ComplexField.Complex z = new ComplexField.Complex( 1, 2 );
//ComplexField.Complex w = new ComplexField.Complex( 3, 4 );
//ComplexField.Complex p = z * w;
//
//
//LinearAlgebra.RealMatrix matA = new LinearAlgebra.RealMatrix( 9, 3, true, @"C:\root\projects\Calculus_2015_\TestConsole\dat\matRettang_20150924_.txt" );
//LinearAlgebra.RealMatrix matA = new LinearAlgebra.RealMatrix(   9, 3,       @"C:\root\projects\Calculus_2015_\TestConsole\dat\matRettang_20150924_.txt" );


//double[,] proto = new double[9, 3]
//    {
//     {0.765456376022406,     0.338355595403516,       0.512278095126282}, //-
//     {0.671985504996025,     0.594393482708555,       0.12178110756063},  //-
//     {0.671985504996025,     0.594393482708555,       0.12178110756063}, 
//     {0.671985504996025,     0.594393482708555,       0.12178110756063}, 
//     {0.671985504996025,     0.594393482708555,       0.12178110756063}, 
//     {0.671985504996025,     0.594393482708555,       0.12178110756063}, 
//     {8.671985504996025,     0.594393482708555,       0.12178110756063},  //-
//     {0.671985504996025,     0.594393482708555,       0.12178110756063}, 
//     {0.671985504996025,     0.594393482708555,       0.12178110756063}
//    };

//LinearAlgebra.MatrixRank Rank_matA = new LinearAlgebra.MatrixRank( proto, 9, 3 );
//int risposta_rango_matA_ = Rank_matA.Rango( );
//System.Console.WriteLine( "\n\n\t int risposta_rango_matA_ = {0}", risposta_rango_matA_ );

//Rank_matA.show_LogSinkFs();

//class TestMatrix
//{
//    private readonly int rows, cols;
//    private readonly double[,] m;

//    public TestMatrix( int r, int c)
//    {
//        this.rows = r;
//        this.cols = c;
//        this.m = new double[ r, c];
//        for ( int curRow=0; curRow < this.rows; curRow++ )
//        {
//            for ( int curCol=0; curCol < this.cols; curCol++ )
//            {
//                m[curRow, curCol] = 3.0;
//            }
//        }
//    }// END public TestMatrix()

//    public TestMatrix( int r, int c, bool randomize )
//    {
//        this.rows = r;
//        this.cols = c;
//        this.m = new double[r, c];
//        for ( int curRow=0; curRow < this.rows; curRow++ )
//        {
//            for ( int curCol=0; curCol < this.cols; curCol++ )
//            {
//                m[curRow, curCol] = 4.0;
//            }
//        }
//    }// END public TestMatrix()


//    public TestMatrix( double[,] proto, int r, int c )
//    {
//        this.rows = r;
//        this.cols = c;
//        this.m = proto;
//    }// END public TestMatrix()


//    // NO
//    //private void alloc()
//    //{
//    //    this.m = new double[4, 4];
//    //}

//    // NO
//    //~TestMatrix()
//    //{
//    //    this.m = null;
//    //}

//}// end class TestMatrix
//#endregion
