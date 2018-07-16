using System;
using System.Collections.Generic;
using System.Text;


namespace RealField
{


    public class JensenInequality_
    {
        //data:
        private double a;// inf( interval)
        private double b;// sup( interval)
        public delegate double Function_pointer( double argument );
        private double dx;// the step measure.
        private Function_pointer function_pointer;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"> inf( interval)</param>
        /// <param name="b"> sup( interval)</param>
        /// <param name="function_pointer">function_pointer to the present algorithm</param>
        /// <param name="dx">the step</param>
        public JensenInequality_(
        double a,       // inf( interval)
        double b,       // sup( interval)
        Function_pointer function_pointer,
        double dx       // the step measure.
            )
        {
            this.a = a;
            this.b = b;
            this.function_pointer = function_pointer;
            this.dx = dx;
        }// end Ctor


        public double[,] ClassManager()
        {
            if ( b <= a )
            {
                throw new System.Exception(
                    "data must be in the form: /// param_a=:inf( interval), param_b=:sup( interval), param_function_pointer=:function_pointer to the present algorithm, param_dx=:the step.");
            }// else continue:
            double cardinality = ( b - a ) / dx;
            int cardinality_N = ( int )cardinality;
            // one vector for the abscissas and two vectors for the ordinates; one for the secant points and one for the curve points, each of the same cardinality.
            double[,] res = new double[3, cardinality_N];
            //
            //for ( int c=0; c < cardinality_N; c++ )
            double t = 0.0;
            for ( int c=0; c < cardinality_N; c++ )
            {
                res[0, c] = Abscissas( t );
                t += 1 / cardinality;
                //
                res[1, c] = Integral_on_Transform( t );// a + c * this.dx 
                res[2, c] = Equazione_Secante( res[0,c] );
            }
            //
            return res;
        }// end ClassManager()





        public double Transform_on_Integral( double t )
        {
            return this.function_pointer( this.reverse_Abscissas( t ) );
        }// end Transform_on_Integral(

        public double Integral_on_Transform( double t )
        {
            return ( t ) * this.function_pointer( a ) + ( 1.0 - t ) * function_pointer( b );
        }// end Integral_on_Transform(

        public double Abscissas( double t )
        {
            return ( 1.0 - t ) * ( a ) + ( t ) * ( b );
        }// end Abscissas(

        public double reverse_Abscissas( double t )
        {
            return ( t ) * ( a ) + ( 1.0 - t ) * ( b );
        }// end Abscissas(

        public double Equazione_Secante( double x )
        {
            double coeffAngolare = ( this.function_pointer( b ) - this.function_pointer( a ) ) / ( b - a );
            return coeffAngolare * ( x - a ) + this.function_pointer( a );
        }// end Equazione_Secante(


    }// end class JensenInequality_


}// end nmsp  RealField
