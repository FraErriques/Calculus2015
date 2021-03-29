using System;


namespace ComplexField
{
    public static class ContourIntegral
    {


#region Notes

// Trapezium Integration. NB.: (u(x,y)+I*v(x,y) )*(dx+I*dy)==u*dx-v*dy + I*( u*dy+v*dx)
// Purpose of this routine -Trapezia- is to provide a numerical solution, suitable for the cases where no primitive can be found.
// The technique of Trapezia is to subdivide the domain in "n" equal Deltas and evaluate the integrand at the extrema of each trapezium
// instead of evaluating a primitive at the boundary, as in Stokes' theorems. Therefore the first step is to choose a contour, over which
// the integration will be performed. Due to Cauchy theorem, for olomorphic functions there's no involvment of the contour when integrating 
// over omotopic paths. Nonetheless the utility of numerical methods -as Trapezia- is to allow the evaluation even of functions 
// not endowed of an elementary primitive. For that purpose, we cannot rely on the behaviour (of the primitive) at the extrema. We have to
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
// It will call  both {Integrate_equi_trapezium_RealPart,Integrate_equi_trapezium_ImmaginaryPart}.
// Have fun.
//
// As an aid to successive implementation, I'm providing a set of the function that are needed by this algorithm. Thei are u,v,x,y,dx,dy
// the functions choosen for the example are f(z)=z which implies u(x,y)=x, v(x,y)=y; the choice for the contour is x(t)=t,y(t)=2*t+1,dx=dt which means dx=1
// dy=2*dt which means dy=2.
// The remaining functions, which are 

#endregion Notes



        // fPtr Types
        //general setting for contour-integrals of elementary functions.
        public delegate double fPtr_U_or_V_( double x, double y );// the real or immaginary part of the image.
        public delegate double fPtr_Jordan_parametriz_( double t );// the x(t) or y(t) or dx(t) or dy(t) of the Jordan parametric contour.

        #region exampleFunctions
        //private static double x( double t)
        //{// x(t)=t
        //    return t;
        //}// x(t)
        //private static double y( double t )
        //{// y(t)=2*t+1
        //    return 2 * t + 1;
        //}// y(t)
        //private static double dx( double t )
        //{// x(t)=t->dx(t)=x'(t)dt=1*dt
        //    return +1.0;
        //}// x(t)
        //private static double dy( double t )
        //{// y(t)=2*t+1 ->dy(y)=y'(t)dt==2*dt
        //    return +2.0;
        //}// y(t)
        ///// <summary>
        ///// the functions choosen for the example are f(z)=z which implies u(x,y)=x, v(x,y)=y; 
        ///// the choice for the contour is x(t)=t,y(t)=2*t+1,dx=dt which means dx=1
        ///// dy=2*dt which means dy=2.
        ///// </summary>
        //private static double genericIntegrand_u_part( double x, double y )
        //{// f(z)==z -> Re(f(z))==Re(f(x+I*y))==x.
        //    return x;
        //}// u(x,y)
        ////
        //private static double genericIntegrand_v_part( double x, double y )
        //{// f(z)==z -> Im(f(z))==Im(f(x+I*y))==y.
        //    return y;
        //}// v(x,y)
        #endregion exampleFunctions



        /// <summary>
        /// Trapezium Integration. NB.: (u(x,y)+I*v(x,y) )*(dx+I*dy)==u*dx-v*dy + I*( u*dy+v*dx)
        /// ...
        /// </summary>
        private static double Integrate_equi_trapezium_RealPart(
            double t0, double tn, // extrema in the pull-back t in [t0,tn]
            fPtr_U_or_V_ u_Part,
            fPtr_U_or_V_ v_Part,
            fPtr_Jordan_parametriz_ x_coordinate,
            fPtr_Jordan_parametriz_ y_coordinate,
            fPtr_Jordan_parametriz_ dx_differential,
            fPtr_Jordan_parametriz_ dy_differential,
            Int64 n )// #trapezia in the partition
        {// RealPart == ==u*dx-v*dy
            double DeltaT = (tn - t0) / (double)n,
            res = 0.0,
            t = DeltaT;// the boundaries {t0,tn} are computed separately, after the core-loop. So trapezium starts at 1*DeltaT.
            // kordell starts here.
            for (; t < tn; t += DeltaT)// stop at the second to last, i.e. <tn. The boudaries are computed separately: t=t0, t=tn.
            {// sum all the internal sides
                res += u_Part(x_coordinate(t), y_coordinate(t)) * dx_differential(t) - v_Part(x_coordinate(t), y_coordinate(t)) * dy_differential(t);
            }
            // post kordell adjustments
            res *= DeltaT; // multiply them for the common base
            res += (
                u_Part(x_coordinate(t0), y_coordinate(t0)) * dx_differential(t0) - v_Part(x_coordinate(t0), y_coordinate(t0)) * dy_differential(t0) +
                +u_Part(x_coordinate(tn), y_coordinate(tn)) * dx_differential(tn) - v_Part(x_coordinate(tn), y_coordinate(tn)) * dy_differential(tn)
                      ) * 0.5 * DeltaT; // add extrema * base/2
            // ready
            return res;
        }//


        /// <summary>
        /// Trapezium Integration. NB.: (u(x,y)+I*v(x,y) )*(dx+I*dy)==u*dx-v*dy + I*( u*dy+v*dx)
        /// ...
        /// </summary>
        private static double Integrate_equi_trapezium_ImmaginaryPart(
            double t0, double tn, // extrema in the pull-back t in [t0,tn]
            fPtr_U_or_V_ u_Part,
            fPtr_U_or_V_ v_Part,
            fPtr_Jordan_parametriz_ x_coordinate,
            fPtr_Jordan_parametriz_ y_coordinate,
            fPtr_Jordan_parametriz_ dx_differential,
            fPtr_Jordan_parametriz_ dy_differential,
            Int64 n )// #trapezia in the partition
        {// ImmaginaryPart== [I*]( u*dy+v*dx)
            double DeltaT = (tn - t0) / (double)n,
            res = 0.0,
            t = DeltaT;// the boundaries {t0,tn} are computed separately, after the core-loop. So trapezium starts at 1*DeltaT.
            // kordell starts here.
            for (; t < tn; t += DeltaT)// stop at the second to last, i.e. <tn. The boudaries are computed separately: t=t0, t=tn.
            {// sum all the internal sides
                res += u_Part(x_coordinate(t), y_coordinate(t)) * dy_differential(t) + v_Part(x_coordinate(t), y_coordinate(t)) * dx_differential(t);
            }
            // post kordell adjustments
            res *= DeltaT; // multiply them for the common base
            res += (
                u_Part(x_coordinate(t0), y_coordinate(t0)) * dy_differential(t0) + v_Part(x_coordinate(t0), y_coordinate(t0)) * dx_differential(t0) +
                +u_Part(x_coordinate(tn), y_coordinate(tn)) * dy_differential(tn) + v_Part(x_coordinate(tn), y_coordinate(tn)) * dx_differential(tn)
                      ) * 0.5 * DeltaT; // add extrema * base/2
            // ready
            return res;
        }//


        private static bool extremaCheck(
            ComplexField.Complex z0,
            ComplexField.Complex z1,
            double t0, double tn, // extrema in the pull-back
            fPtr_Jordan_parametriz_ x_coordinate,
            fPtr_Jordan_parametriz_ y_coordinate
            )
        {
            double localEpsilon = +1.0E-13;
            double z0_x_byJordan = x_coordinate(t0);
            double z0_y_byJordan = y_coordinate(t0);
            double z1_x_byJordan = x_coordinate(tn);
            double z1_y_byJordan = y_coordinate(tn);
            //
            double delta_Z0x = Math.Abs(z0_x_byJordan - z0.get_real);
            double delta_Z0y = Math.Abs(z0_y_byJordan - z0.get_immaginary);
            double delta_Z1x = Math.Abs(z1_x_byJordan - z1.get_real);
            double delta_Z1y = Math.Abs(z1_y_byJordan - z1.get_immaginary);
            //
            bool res = (
                   delta_Z0x   < localEpsilon
                && delta_Z0y   < localEpsilon
                && delta_Z1x   < localEpsilon
                && delta_Z1y   < localEpsilon
                );
            //ready.
            return res;
        }// extremaCheck


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sigma">the Real part of the complex argument</param>
        /// <param name="t">the Immaginary part of the complex argument</param>
        /// <param name="xRplus_threshold">the x0 in R+ used as stop-value in the improper integration to +Infinity</param>
        /// <param name="n">the number of DeltaX to step into, i.e. the number of trapezia to be calculated in the decomposition</param>
        /// <returns>the Complex value of the Gamma[sigma+I*t]</returns>
        public static ComplexField.Complex ContourIntegral_ManagementMethod(
            ComplexField.Complex z0,
            ComplexField.Complex z1,
            double t0, double tn, // extrema in the pull-back
            fPtr_U_or_V_ u_Part,
            fPtr_U_or_V_ v_Part,
            fPtr_Jordan_parametriz_ x_coordinate,
            fPtr_Jordan_parametriz_ y_coordinate,
            fPtr_Jordan_parametriz_ dx_differential,
            fPtr_Jordan_parametriz_ dy_differential,
            Int64 n )// #trapezia in the partition
        {
            bool extremaAdequacy = extremaCheck(
                z0, z1,
                t0, tn,
                x_coordinate,
                y_coordinate
            );
            if (!extremaAdequacy) { throw new System.Exception("Integration extrema do not match, between coChain and Jordan-path."); }

            ComplexField.Complex res = new ComplexField.Complex(
                Integrate_equi_trapezium_RealPart(
                    t0,tn
                    ,u_Part
                    ,v_Part
                    , x_coordinate
                    , y_coordinate
                    , dx_differential
                    , dy_differential
                    ,n )

                , Integrate_equi_trapezium_ImmaginaryPart(
                    t0, tn
                    , u_Part
                    , v_Part
                    , x_coordinate
                    , y_coordinate
                    , dx_differential
                    , dy_differential
                    , n)
            );
            // ready.
            return res;
        }// ContourIntegral_ManagementMethod


         

    }// class
}// nmsp
