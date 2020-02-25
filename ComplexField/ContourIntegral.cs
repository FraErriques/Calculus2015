//using System;


//namespace ComplexField
//{
//    public static class ContourIntegral
//    {
//        // Types
//        // public delegate ComplexField.Complex Functional_form( ComplexField.Complex z ); general setting for contour-integrals of elementary functions.
//        //public delegate double u( double x, double y );// the real part of the image.
//        //public delegate double v( double x, double y );// the immaginary part of the image.

#region Notes

// Trapezium Integration. NB.: (u(x,y)+I*v(x,y) )*(dx+I*dy)==u*dx-v*dy + I*( u*dy+v*dx)
// Purpose of this routine -Trapezia- is to provide a numerical solution, suitable for the cases where no primitive can be found.
// The technique of Trapezia is to subdivide the domain in n equal Deltas and evaluate the integrand at the extrema of each trapezium
// instead of evaluating a primitive at the boundary, as in Stokes' theorems. Therefore the first step is to choose a contour, over which
// the integration will be performed. Due to Cauchy theorem, for olomorphic functions there's no involvment of the contour when integrating 
// over omotopic paths. Nonetheless the utility of numerical methods -as Trapezia- is to allow the evaluation even of functions 
// non endowed of an elementary primitive. For that purpose, we cannot rely on the behaviour (of the primitive) at the extrema. We have to
// explicitly follow a path that connects (in a Jordan way) the extrema. First step will be the choice of a contour and of its parametrization.
// In the C-plane the contour will be as L:{x(t)=f(t), y(t)=g(t)} and so dx=x'(t)dt , dy=y'(t)dt. Due to this, the two coordinate functions:
// u=u(x,y) and v(x,y) will have to be restricted to the path as in u=u(x(t),y(t)) , v=v(x(t),y(t)) 
// Therefore the classical:  (u(x,y)+I*v(x,y) )*(dx+I*dy)==u*dx-v*dy + I*( u*dy+v*dx)
// will become: u(x(t),y(t))*dx-v(x(t),y(t))*dy + I*( u(x(t),y(t))*dy+v(x(t),y(t))*dx) which will be
// u(x(t),y(t))*x'(t)dt-v(x(t),y(t))*y'(t)dt + I*( u(x(t),y(t))*y'(t)dt+v(x(t),y(t))*x'(t)dt) 
// the two not-homogeneous quantities that must be calculated separately are:
//      u(x(t),y(t))*x'(t)dt-v(x(t),y(t))*y'(t)dt   for the Real-part
//      u(x(t),y(t))*y'(t)dt+v(x(t),y(t))*x'(t)dt   for the Immaginary-part
//
// The routine in this class is intended to be for elementary functions, so they will accept just the complex argument and no other parameter whatsoever.
// The user will have to provide function-pointers (i.e. delegates) to the following methods:
// double x(double t) it's the x-coordinate function -abscissa- of the parametrization for the Jordan curve, in the C-plane
// double y(double t) it's the y-coordinate function -ordinate- of the parametrization for the Jordan curve, in the C-plane
// double dx(double t) it's the x-differential; it has to contain everything that is a factor of dt es. dx=2*t*dt -> dx returns(2*t), accepting t.
// double dy(double t) it's the y-differential; it has to contain everything that is a factor of dt es. dy=3*Sin(t)*dt -> dy returns(3*Sin(t)), accepting t.
// The user will have to provide also function-pointers to:
// double u(double x, double y) which will be fed as u(x(t),y(t)) and the same for v(x(t),y(t)).
// This class instead will provide:
// a method double RealPartIntegral which performs: u*dx-v*dy in terms of u(x(t),y(t))*dx(t)-v(x(t),y(t))*dy(t)
// the RealPartIntegral method will accept: t_inf, t_sup, n_card_trapezia, f_pointer_x(t), f_pointer_y(t), f_pointer_dx(t), f_pointer_dy(t), f_pointer_u(x,y), f_pointer_v(x,y)
// a method double ImmaginaryPartIntegral which performs: I*( u*dy+v*dx) in terms of u(x(t),y(t))*dy(t)+v(x(t),y(t))*dx(t)
// the ImmaginaryPartIntegral method will accept: t_inf, t_sup, n_card_trapezia, f_pointer_x(t), f_pointer_y(t), f_pointer_dx(t), f_pointer_dy(t), f_pointer_u(x,y), f_pointer_v(x,y)
// NB. in future implementations the management method, which calls both {RealPartIntegral,ImmaginaryPartIntegral} could spawn separate threads for then and then join the results.
// So this class instead will provide a management method -the only public one- wich will have signature:
// ComplexField.Complex ContourIntegral(  t_inf, t_sup, n_card_trapezia, f_pointer_x(t), f_pointer_y(t), f_pointer_dx(t), f_pointer_dy(t), f_pointer_u(x,y), f_pointer_v(x,y) )
// It will call  both {RealPartIntegral,ImmaginaryPartIntegral}.
// Have fun.

#endregion Notes


//        /// <summary>
//        /// NB.: (u(x,y)+I*v(x,y) )*(dx+I*dy)==u*dx-v*dy + I*( u*dy+v*dx)
//        /// <returns></returns>
//        public static double genericIntegrand_u_part( 
//            double sigma, double t // the complex argument. It is integrated towards.
//         )
//        {
//            // NB.: (u(x,y)+I*v(x,y) )*(dx+I*dy)==u*dx-v*dy + I*( u*dy+v*dx)
//            // TODO
//            return double.NaN;
//        }// u_part



//        /// <summary>
//        /// NB.: (u(x,y)+I*v(x,y) )*(dx+I*dy)==u*dx-v*dy + I*( u*dy+v*dx)
//        /// </summary>
//        /// <param name="x"></param>
//        /// <param name="y"></param>
//        /// <returns></returns>
//        public static double genericIntegrand_v_part(
//            double sigma, double t // the complex argument. It is integrated towards.
//         )
//        {
//            // NB.: (u(x,y)+I*v(x,y) )*(dx+I*dy)==u*dx-v*dy + I*( u*dy+v*dx)
//            // TODO
//            return double.NaN;
//        }// v_part




//        /// <summary>
//        /// Trapezium Integration. NB.: (u(x,y)+I*v(x,y) )*(dx+I*dy)==u*dx-v*dy + I*( u*dy+v*dx)
//        /// ...
//        /// </summary>
//        public static double Integrate_equi_trapezium_RealPart(
//            double sigma, double t,
//            double t0, double tn, // extrema in the pull-back
//            Int64 n )// #trapezia in the partition
//        {
//            double DeltaT = (tn-t0) / (double)n,
//            res = 0.0,
//t = double.Epsilon;// start from zero+ since we have Log[x] in the computation.
//            // kordell starts here.
//            for (; x < xRplus_threshold; x += DeltaX)
//            {// sum all the internal sides
//                res += genericIntegrand_u_part(sigma, t);
//            }
//            // post kordell adjustments
//            res *= DeltaX; // multiply them for the common base
//            res += (
//                        // GammaIntegrand_u_part( sigma, t, double.Epsilon)
//                        genericIntegrand_u_part(sigma, t) +
//                        genericIntegrand_u_part(sigma, t) // + GammaIntegrand_u_part( sigma, t, xRplus_threshold)
//                      ) * 0.5 * DeltaX; // add extrema * base/2
//            // ready
//            return res;
//        }//


//        /// <summary>
//        /// Trapezium Integration. NB.: (u(x,y)+I*v(x,y) )*(dx+I*dy)==u*dx-v*dy + I*( u*dy+v*dx)
//        /// ...
//        /// </summary>
//        public static double Integrate_equi_trapezium_ImmaginaryPart(
//            double sigma, double t,
//            double t0, double tn, // extrema in the pull-back
//            Int64 n )// #trapezia in the partition
//        {
//            double DeltaT = (tn - t0) / (double)n,
//            res = 0.0,
//            x = double.Epsilon;// start from zero+ since we have Log[x] in the computation.
//            // kordell starts here.
//            for (; x < xRplus_threshold; x += DeltaX)
//            {// sum all the internal sides
//                res += genericIntegrand_v_part(sigma, t);
//            }
//            // post kordell adjustments
//            res *= DeltaX; // multiply them for the common base
//            res += (
//                        genericIntegrand_v_part(sigma, t) +
//                        + genericIntegrand_v_part(sigma, t)
//                      ) * 0.5 * DeltaX; // add extrema * base/2
//            // ready
//            return res;
//        }//


//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="sigma">the Real part of the complex argument</param>
//        /// <param name="t">the Immaginary part of the complex argument</param>
//        /// <param name="xRplus_threshold">the x0 in R+ used as stop-value in the improper integration to +Infinity</param>
//        /// <param name="n">the number of DeltaX to step into, i.e. the number of trapezia to be calculated in the decomposition</param>
//        /// <returns>the Complex value of the Gamma[sigma+I*t]</returns>
//        public static ComplexField.Complex genericContour_Integral(
//            double sigma, double t,
//            double t0, double tn, // extrema in the pull-back
//            Int64 n )// #trapezia in the partition
//        {
//            ComplexField.Complex res = new ComplexField.Complex(
//                Integrate_equi_trapezium_RealPart( sigma, t, xRplus_threshold, n)
//                , Integrate_equi_trapezium_ImmaginaryPart(sigma, t, xRplus_threshold, n)
//            );
//            // ready.
//            return res;
//        }// GammaSpecialF_viaIntegral


//    }// class
//}// nmsp
