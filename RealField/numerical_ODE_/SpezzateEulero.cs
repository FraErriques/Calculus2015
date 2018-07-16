using System;
using System.Collections.Generic;
using System.Text;

namespace RealField.numerical_ODE_
{

    public class SpezzateEulero
    {
        private double x0;
        private double y0;
        private double h;// only constant dx supported.
        public delegate double FPointer( double xn, double yn );
        FPointer fPointer;


        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="x0"></param>
        /// <param name="y0"></param>
        /// <param name="h"></param>
        /// <param name="f"></param>
        public SpezzateEulero(
            double x0, double y0
            , double h
            , FPointer f
            )
        {
            this.x0 = x0;
            this.y0 = y0;
            this.h = h;
            this.fPointer = f;
        }// end Ctor.


        /// <summary>
        /// metodo che utilizza la derivata, quale step forward della funzione incognita.
        /// y'[x]~y[x+Dx]~y[x]+Dx*slope
        /// the difference between methods( Eulero, Runge-Kutta, ...) is how slope is intended.
        /// Runge-Kutta method uses a rather sophisticated expansion of the slope, while Eulero 
        /// uses just the differential.
        /// </summary>
        /// <param name="xn"></param>
        /// <param name="yn"></param>
        /// <returns></returns>
        private double y_np1( double xn, double yn )
        {
            double res_y_np1 =
                yn + this.h * this.fPointer( xn, yn );
            return res_y_np1;
        }


        /// <summary>
        /// key method:
        ///     -   it builds the grid, which numerically represents the unknown integral curve.
        /// </summary>
        /// <param name="cardinality"></param>
        /// <returns></returns>
        public System.Collections.ArrayList scatter( int cardinality )
        {
            System.Collections.ArrayList theScatter = new System.Collections.ArrayList( cardinality );
            //--insertion of boundary value conditions-----once only, then the method forwards---
            Point curPoint = new Point();
            Point nextPoint = new Point();
            curPoint.xn = this.x0;
            curPoint.yn = this.y0;// bootstrap on (0 == c).
            theScatter.Add( curPoint );// boundary value conditions insertion.
            //---end of boundary value conditions insertion.
            //
            for ( int c = 0; c < cardinality; c++ )
            {
                nextPoint.xn = curPoint.xn + h;
                nextPoint.yn = this.y_np1( curPoint.xn, curPoint.yn );
                //
                theScatter.Add( nextPoint );
                curPoint = nextPoint;// update.
                //
            }// end for
            // ready
            return theScatter;
        }// end scatter.


        public void format( System.Collections.ArrayList points )
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;// NB. decimal point.
            System.Console.WriteLine( "\n\n\n  {" );
            for ( int c = 0; c < points.Count; c++ )
            {
                System.Console.Write( "{" );
                System.Console.Write( "{0},{1}",
                    ( ( Point )( points[c] ) ).xn,
                    ( ( Point )( points[c] ) ).yn
                );
                System.Console.WriteLine( "}," );
            }// end for.
            System.Console.WriteLine( "} \n\n\n" );
        }

        ///// <summary>
        ///// es. di fpointer per  y'[x]==y[x]
        ///// </summary>
        ///// <param name="xn"></param>
        ///// <param name="yn"></param>
        ///// <returns></returns>
        //private static double y1_equals_y(double xn, double yn)
        //{
        //    return yn;
        //}// end numerical y'[x]==f(x,y[x]).


    }// end class SpezzateEulero


}
