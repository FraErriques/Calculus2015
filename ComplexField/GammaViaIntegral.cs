using System;


namespace ComplexField
{


    public static class GammaViaIntegral
    {

        #region Notes

        // memo: an implementation in the f:R->R case.
        // restrizione ad R+, dell'integrando della Gamma di Eulero. TODO: Complex analytical continuation.
        // 
        //public static double IntegrandoGamma(double t)
        //{// Note: Gamma[z] is the Mellin transform of Exp[-t]. For real "t" it interpolates Factorial(t).
        //    // There's a product representation which highlights the poles on R- since they appear as zeros in the denominator. Such representation is due to Gauss
        //    // and is based on explicitating the limit(1-t/n)^n==Exp[-t] inside the Gamma integral.
        //    double z = 7.0;// Gamma(7)==6!=720  TODO customize, after the implementation of a new equi_trapezium for Complex functions.
        //    return Math.Pow(t, z - 1) * Math.Exp(-t);
        //}//

        // the implementation in the f:R->R case.
        //public static double Integrate_equi_trapezium_RealPart( double a, double b, Int64 n )
        //{
        //    u functional_form = new u(Gamma_u_part);// NB.  Modificare questa riga del sorgente per variare l'integrando della parte reale, i.e. u=u(x,y)
        //    double delta = (b - a) / (double)n,
        //    res = 0.0,
        //    x = a + delta;
        //    //
        //    for (; x < b; x += delta)
        //    {// sum all the internal sides
        //        res += functional_form(sigma, t);
        //    }
        //    res *= delta; // multiply them for the common base
        //    res += (functional_form(a) + functional_form(b)) * 0.5 * delta; // add extrema * base/2
        //    //
        //    return res;
        //}//        
        #endregion Notes


        /// <summary>
        /// NB. no dependence on y, but: dependence on x(for saturation) and from s=:sigma+I*t
        /// u(sigma,t) is the real part of the image of the integrand of the Gamma function. This has to be saturated in "x" in R+, which leads us to use "Trapezia".
        /// NB. in the following notation s=(sigma,t) is the complex argument, which is not integrated towards. The x that appears in the notation is a
        /// mute-integration-variable, which gets saturated over R+ in the process and appears no more, in the result.
        /// u[x_, sigma_, t_] = \[ExponentialE]^(-x + (-1 + sigma) Log[x]) Cos[t Log[x]]
        /// written in simplified way : Exp[(sigma-1)*Log[x]-x]*Cos[t*Log[x]]
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private static double GammaIntegrand_u_part(
            double sigma, double t, // the complex argument. It's not integrated towards. It's a parameter.
            double x // the Gamma special function is defined(also) as an improper integral on R+, towards the mute variable x>0. The "trapezia" method will 
            // pass the actual value of x, at each step. The values of (sigma,t) will be the same during all of the steps in a trapezia procedure. Tehy change just 
            // when the request is to represent a different point.
         )
        {// NB. note there is no dependence on y in the integration measure. So dy is identically 0.
            // GammaIntegrand_RealPart == u(sigma,t) == Exp[(sigma-1.0)*Log[x]-x]*Cos[t*Log[x]]
            return Math.Exp((sigma - 1.0) * Math.Log(x) - x) * Math.Cos(t * Math.Log(x));
            // the returned value is a real number, which is a chunk of the real part of the result of Gamma[s], namely is Gamma[s,x0]==Gamma[sigma,t,x0]
            // the actual value of x0 will be updated at each call and used as a basis of a trapezium. The height is Dx. The sum of the measures of the involved
            // trapezia, will constitute the Real-Part of Gamma[s] where there's no more dependence on the saturation variable x. There's instead dependence on 
            // the selected value of s in C. Note: Gamma[s] is real for real-s and so Gamma benefits of Schwartz symmetry, i.e. Gamma[conjugate[s]]==conjugate[Gamma[s]
            // Gamma is n times meromorphic on R-, namely on x=-n ,n in N.
        }// u_part



        /// <summary>
        /// v[x_, sigma_, t_] = \[ExponentialE]^(-x + (-1 + sigma) Log[x]) Sin[t Log[x]]
        /// written in simplified way : Exp[(sigma-1)*Log[x]-x]*Sin[t*Log[x]]
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private static double GammaIntegrand_v_part(
            double sigma, double t, // the complex argument. It's not integrated towards. It's a parameter.
            double x // the Gamma special function is defined(also) as an improper integral on R+, towards the mute variable x>0. The "trapezia" method will 
            // pass the actual value of x, at each step. The values of (sigma,t) will be the same during all of the steps in a trapezia procedure. Tehy change just 
            // when the request is to represent a different point.
         )
        {
            // Exp[(sigma-1.0)*Log[x]-x]*Sin[t*Log[x]]
            return Math.Exp((sigma - 1.0) * Math.Log(x) - x) * Math.Sin(t * Math.Log(x));
        }// v_part




        /// <summary>
        /// Trapezium Integration. NB.: (u(x,y)+I*v(x,y) )*(dx+I*dy)==u*dx-v*dy + I*( u*dy+v*dx)
        /// In the specifica case of Gamma[z]==Gamma[x+I*y] we have that the function is defined
        /// as a special one, by means of an improper real integral, where y=0 identically and so dy=0.
        /// Therefore u*dx-v*dy + I*( u*dy+v*dx)== u*dx+I*v*dx
        /// and
        /// u(sigma,t)==  Exp[(sigma-1)*Log[x]-x]*Cos[t*Log[x]] ,where the complex variable is denoted "s" and s=:sigma+I*t to 
        /// avoid any confusion with the real variable x, which is appropriated for saturation on R+.
        /// and
        /// v(sigma,t)==  Exp[(sigma-1)*Log[x]-x]*Sin[t*Log[x]] this is the immaginary part and the result of the integration is multiplied for I==Sqrt[-1].
        /// NB. Gamma==Gamma[s]==Gamma[sigma+I*t]==Integrate[u(sigma,t)dx+I*v(sigma,t)dx] The integral is NOT with respect to the cmplex variable, but to the
        /// real variable x, and is a real integral where (dx+I*dy)==dx. The complex argument s=:sigma+I*t is not integrated towards.
        /// </summary>
        private static double Integrate_equi_trapezium_RealPart( double sigma, double t, double xRplus_threshold, Int64 n )
        {
            double DeltaX = (xRplus_threshold) / (double)n,
            res = 0.0,
            x = DeltaX;// start from DeltaX
            // kordell starts here.
            for (; x < xRplus_threshold; x += DeltaX)
            {// sum all the internal sides
                res += GammaIntegrand_u_part(sigma, t,// these values are the same in each trapezium. They are not integrated towards.
                    x);// this is the only value that gets updated between calls, since it's the integration variable,
            }
            // post kordell adjustments
            res *= DeltaX; // multiply them for the common base
            res += (
                        GammaIntegrand_u_part(sigma, t, double.Epsilon)// start from zero+ since we have Log[x] in the computation.
                        + GammaIntegrand_u_part(sigma, t, xRplus_threshold)
                      ) * 0.5 * DeltaX; // add extrema * base/2
            // ready
            return res;
        }//


        /// <summary>
        /// Trapezium Integration. NB.: (u(x,y)+I*v(x,y) )*(dx+I*dy)==u*dx-v*dy + I*( u*dy+v*dx)
        /// In the specifica case of Gamma[z]==Gamma[x+I*y] we have that the function is defined
        /// as a special one, by means of an improper real integral, where y=0 identically and so dy=0.
        /// Therefore u*dx-v*dy + I*( u*dy+v*dx)== u*dx+I*v*dx
        /// and
        /// u(sigma,t)==  Exp[(sigma-1)*Log[x]-x]*Cos[t*Log[x]] ,where the complex variable is denoted "s" and s=:sigma+I*t to 
        /// avoid any confusion with the real variable x, which is appropriated for saturation on R+.
        /// and
        /// v(sigma,t)==  Exp[(sigma-1)*Log[x]-x]*Sin[t*Log[x]] this is the immaginary part and the result of the integration is multiplied for I==Sqrt[-1].
        /// NB. Gamma==Gamma[s]==Gamma[sigma+I*t]==Integrate[u(sigma,t)dx+I*v(sigma,t)dx] The integral is NOT with respect to the cmplex variable, but to the
        /// real variable x, and is a real integral where (dx+I*dy)==dx. The complex argument s=:sigma+I*t is not integrated towards.
        /// </summary>
        private static double Integrate_equi_trapezium_ImmaginaryPart( double sigma, double t, double xRplus_threshold, Int64 n )
        {
            double DeltaX = (xRplus_threshold) / (double)n,
            res = 0.0,
            x = DeltaX;// start from DeltaX
            // kordell starts here.
            for (; x < xRplus_threshold; x += DeltaX)// strictly x<xRplus_threshold because x==xRplus_threshold is the boundary, which gets evaluaeted after.
            {// sum all the internal sides
                res += GammaIntegrand_v_part(sigma, t,// these values are the same in each trapezium. They are not integrated towards.
                    x);// this is the only value that gets updated between calls, since it's the integration variable,
            }
            // post kordell adjustments
            res *= DeltaX; // multiply them for the common base
            res += (
                        GammaIntegrand_v_part(sigma, t, double.Epsilon)// start from epsilon, since there's Log[x] with problems in zero.
                        + GammaIntegrand_v_part(sigma, t, xRplus_threshold)
                      ) * 0.5 * DeltaX; // add extrema * base/2
            // ready
            return res;
        }//


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sigma">the Real part of the complex argument</param>
        /// <param name="t">the Immaginary part of the complex argument</param>
        /// <param name="xRplus_threshold">the x0 in R+ used as stop-value in the improper integration to +Infinity</param>
        /// <param name="n">the number of DeltaX to step into, i.e. the number of trapezia to be calculated in the decomposition</param>
        /// <returns>the Complex value of the Gamma[sigma+I*t]</returns>
        public static ComplexField.Complex GammaSpecialF_viaIntegral( double sigma, double t, double xRplus_threshold, Int64 n )
        {
            ComplexField.Complex res = new ComplexField.Complex(
                Integrate_equi_trapezium_RealPart(sigma, t, xRplus_threshold, n)
                , Integrate_equi_trapezium_ImmaginaryPart(sigma, t, xRplus_threshold, n)
            );
            // ready.
            return res;
        }// GammaSpecialF_viaIntegral


    }// class
}// nmsp
