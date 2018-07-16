using System;
using System.Collections.Generic;
using System.Text;


namespace RealField
{

    namespace AvailableAlgorithms
    {

        #region TaylorSeries


        public class SequenceEngineSinX
        {
            public double f_SequenceEngineSinX( double x, double kappa )
            {
                double res = 0.0;
                RealField.AvailableAlgorithms.FactorialN FactorialInstance = new RealField.AvailableAlgorithms.FactorialN();
                //
                for ( int k=0; k <= kappa; k++ )
                {
                    res += Math.Pow( -1.0, k ) * Math.Pow( x, 2 * k + 1 ) / FactorialInstance.f_Factorial( 2 * k + 1 );
                }
                //
                return res;
            }

            public override string ToString()
            {
                return "current function is SequenceEngineSinX== Sum[Pow(x,2k+1)/((2k+1)!), {k,0,n}];";
            }// end decorator.
        }//end class.


        /// <summary>
        /// McLaurinSeries of -Log[1-x]. It converges only in (-1, 1). The image is (-2, +Infinity).
        /// </summary>
        public class SequenceEngineMinusLogOneMinusX
        {
            public double f_SequenceEngineMinusLogOneMinusX( double x, double kappa )
            {
                double res = 0.0;
                //
                for ( int k=0; k <= kappa; k++ )
                {
                    res += Math.Pow( x, k + 1 ) / ( k + 1 );
                }
                //
                return res;
            }

            public override string ToString()
            {
                return "current function is McLaurinSeries of -Log[1-x]. It converges only in (-1, 1). The image is (-2, +Infinity). Implementation x^k/k, {k,1,n}];";
            }// end decorator.
        }//end class.


        public class SequenceEngine_McLaurinExp
        {
            /// <summary>
            /// now devoted to the sequence McLaurin of Exp: centered in zero, calculated in 1. Converges to Exp[1]==E.
            /// Sono ridotte all'ordine "seed", della serie sopracitata.
            /// </summary>
            /// <param name="seed"></param>
            /// <returns></returns>
            public double f_SequenceEngine_McLaurinExp( double x, double kappa )
            {
                double res = 0.0;
                RealField.AvailableAlgorithms.FactorialN FactorialInstance = new RealField.AvailableAlgorithms.FactorialN();
                //
                for ( int c=0; c <= kappa; c++ )
                {
                    res += Math.Pow( x, c ) / FactorialInstance.f_Factorial( c );
                }
                //
                return res;
            }// end f(x)

            public override string ToString()
            {
                return "current function is SequenceEngine_McLaurinExp== Sum[x^k/k!, {k,0,n}];";
            }// end decorator.
        }//end class.





        #endregion TaylorSeries

        #region SequencesNotSeries

        public class SequenceEngine_E
        {
            public double f_SequenceEngine_E( double alpha, double enne )
            {
                double res = 0.0;
                //
                res = Math.Pow( 1.0 + alpha / enne, enne );// converges to E^alpha.
                //
                return res;
            }// end f(x)

            public override string ToString()
            {
                return "current function is SequenceEngine_E== Math.Pow( 1.0 + alpha / enne, enne );";
            }// end decorator.
        }//end class.


        public class FactorialN
        {
            /// <summary>
            /// fattoriale : with double.
            /// </summary>
            /// <param name="n"></param>
            /// <returns></returns>
            public double f_Factorial( double n )
            {
                if ( n < 0 )
                {
                    throw new System.Exception( "factorial available here, only for nonnegative integers." );
                }// else continue.
                //
                double theFact = +1.0;// will grow by product.
                for ( double k=n; k > 1.0; k-- )
                {
                    theFact *= k;
                }// for.
                //ready.
                return theFact;
            }// end end f(x) fact

            public override string ToString()
            {
                return "current function is FactorialN== Factorial(N);";
            }// end decorator.
        }//end class.



        public class SequenceEngine_EquiStep
        {
            private double a,b;
            private int n;
            //
            public SequenceEngine_EquiStep( double a, double b, int n )
            {
                this.a = a;
                this.b = b;
                this.n = n;
            }// end Ctor

            public double f_SequenceEngine_EquiStep( int i )
            {
                double res = 0.0;
                //
                res = this.a + i*(this.b-this.a)/this.n;//  ,{i,0,n}; converges to "b"
                //
                return res;
            }// end f_SequenceEngine_EquiStep

            public override string ToString()
            {
                return "current function is SequenceEngine_EquiStep: Image_[a,b] sequence== a + i*(b-a)/n  ,{i,0,n}";
            }// end decorator.
        }//end class.



        #endregion SequencesNotSeries


        #region RealFunctionsOfRealVar


        public class Bisettrice
        {
            public double f_Bisettrice( double x )
            {
                return x;
            }// end f(x)

            public override string ToString()
            {
                return "current function is Bisettrice del primo e terzo quadrante; y==x.";
            }// end decorator.
        }//end class.


        public class SetteSinSetteX
        {
            public double f_SetteSinSetteX( double x )
            {
                return 7.0 * Math.Sin( 7.0 * x );
            }// end f(x)

            public override string ToString()
            {
                return "current function is SetteSinSetteX== 7*Sin(7*x);";
            }// end decorator.
        }//end class.


        public class SinX
        {
            public double f_SinX( double x )
            {
                return System.Math.Sin( x );
            }// end f(x)

            public override string ToString()
            {
                return "current function is SinX== Sin(x);";
            }// end decorator.
        }//end class.


        #endregion RealFunctionsOfRealVar

    }// end nmsp AvailableAlgorithms
    
}// end nmsp RealField
