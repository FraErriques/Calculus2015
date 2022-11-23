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
// NB. in future implementations the management method, which calls both {RealPartIntegral,ImmaginaryPartIntegral} could spawn separate threads for them and then join the results.
// So this class instead will provide a management method -the only public one- wich will have signature:
// ComplexField.Complex ContourIntegral(  t_inf, t_sup, n_card_trapezia, f_pointer_x(t), f_pointer_y(t), f_pointer_dx(t), f_pointer_dy(t), f_pointer_u(x,y), f_pointer_v(x,y) )
// It will call  both {Integrate_equi_trapezium_RealPart,Integrate_equi_trapezium_ImmaginaryPart}.
// Have fun.
//
// As an aid to successive implementation, I'm providing a set of the function that are needed by this algorithm. They are u,v,x,y,dx,dy
// the functions choosen for the example are f(z)=z which implies u(x,y)=x, v(x,y)=y; the choice for the contour is x(t)=t,y(t)=2*t+1,dx=dt which means dx=1
// dy=2*dt which means dy=2.
// The remaining functions, which are 

#endregion Notes



        // fPtr Types
        //general setting for contour-integrals of elementary functions.
        public delegate double fPtr_U_or_V_( double x, double y );// the real or immaginary part of the image.
        public delegate ComplexField.Complex fPtr_ComplexAsScalar_( double x, double y );
        public delegate double fPtr_Jordan_parametriz_( double t );// the x(t) or y(t) or dx(t) or dy(t) of the Jordan parametric contour.

        #region exampleFunctions
        private static double x( double t )
        {// x(t)=t
            return t;
        }// x(t)
        private static double y( double t )
        {// y(t)=2*t+1
            return 2 * t + 1;
        }// y(t)
        private static double dx( double t )
        {// x(t)=t->dx(t)=x'(t)dt=1*dt
            return +1.0;
        }// x(t)
        private static double dy( double t )
        {// y(t)=2*t+1 ->dy(y)=y'(t)dt==2*dt
            return +2.0;
        }// y(t)
        /// <summary>
        /// the functions choosen for the example are f(z)=z which implies u(x,y)=x, v(x,y)=y; 
        /// the choice for the contour is x(t)=t,y(t)=2*t+1,dx=dt which means dx=1
        /// dy=2*dt which means dy=2.
        /// </summary>
        private static double genericIntegrand_u_part( double x, double y )
        {// f(z)==z -> Re(f(z))==Re(f(x+I*y))==x.
            return x;
        }// u(x,y)
        private static double genericIntegrand_v_part( double x, double y )
        {// f(z)==z -> Im(f(z))==Im(f(x+I*y))==y.
            return y;
        }// v(x,y)
        //
        private static ComplexField.Complex zSquared_AsScalar_( double x, double y )
        {
            ComplexField.Complex z = new ComplexField.Complex(x, y);
            return z * z;// z^2
        }// fPtr_ComplexAsScalar_
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
            double DeltaT = (tn - t0) / (double)n;
            double res = 0.0;
            double t = t0 + DeltaT;// the boundaries {t0,tn} are computed separately, after the core-loop. So trapezium starts at 1*DeltaT.
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
            double DeltaT = (tn - t0) / (double)n;
            double res = 0.0;
            double t = t0 + DeltaT;// the boundaries {t0,tn} are computed separately, after the core-loop. So trapezium starts at 1*DeltaT.
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



        /// <summary>
        /// Trapezium Integration. NB.: f(z)*(dx+I*dy)
        /// ComplexImageAsScalar
        /// </summary>
        private static ComplexField.Complex Integrate_equi_trapezium_ComplexImageAsScalar(
            double t0, double tn, // extrema in the pull-back t in [t0,tn]
            fPtr_ComplexAsScalar_ complexAsScalar,
            fPtr_Jordan_parametriz_ x_coordinate,
            fPtr_Jordan_parametriz_ y_coordinate,
            fPtr_Jordan_parametriz_ dx_differential,
            fPtr_Jordan_parametriz_ dy_differential,
            Int64 n )// #trapezia in the partition
        {// RealPart == ==u*dx-v*dy
            double DeltaT = (tn - t0) / (double)n;
            double t = t0 + DeltaT;// the boundaries {t0,tn} are computed separately, after the core-loop. So trapezium starts at 1*DeltaT.
            ComplexField.Complex res = new ComplexField.Complex(0,0);
            // kordell starts here.
            for (; t < tn; t += DeltaT)// stop at the second to last, i.e. <tn. The boudaries are computed separately: t=t0, t=tn.
            {// sum all the internal sides
                res += complexAsScalar(x_coordinate(t), y_coordinate(t)) * new ComplexField.Complex(dx_differential(t),dy_differential(t));
            }
            // post kordell adjustments
            res *= DeltaT; // multiply them for the common base
            res += (
                    complexAsScalar(x_coordinate(t0), y_coordinate(t0)) * new ComplexField.Complex(dx_differential(t0),dy_differential(t0))+
                    complexAsScalar(x_coordinate(tn), y_coordinate(tn)) * new ComplexField.Complex(dx_differential(tn), dy_differential(tn))
                    ) * 0.5 * DeltaT; // add extrema * base/2
            // ready
            return res;
        }//



        /// <summary>
        /// le coordinate sono riferite al piano ARGOMENTO, non al piano immagine.
        /// gli estremi sulla CoCatena e sulla Catena devono coincidere come segue:
        /// z0=={x[t0]+I*y[t0]}
        /// z1=={x[tn]+I*y[tn]}
        /// </summary>
        /// <param name="z0"> z0=={x[t0]+I*y[t0]} in the ARGUMENT plane</param>
        /// <param name="z1"> z1=={x[tn]+I*y[tn]} in the ARGUMENT plane</param>
        /// <param name="t0">questi punti sono nello spazio separato del Pull-Back</param>
        /// <param name="tn">questi punti sono nello spazio separato del Pull-Back</param>
        /// <param name="x_coordinate">function pointer to a coordinate function x[t]</param>
        /// <param name="y_coordinate">function pointer to a coordinate function y[t]</param>
        /// <returns></returns>
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
        /// This function evaluates a contour integral in the complex plane, via the numerical method of Riemann partition
        /// on the pullback of a parametrization of the Chain.
        /// The caller is required to provide suitable function-pointers to the real and the immaginary part of the CoChain.
        /// f[z]==f[x+I*y]==u(x,y)+I*v(x,y) the u() and the v() have to be implemented, before calling this function.
        /// The implementation is:
        /// (u(x,y)+I*v(x,y))*(dx+I*dy)==u*dx-v*dy + I*( u*dy+v*dx)
        /// </summary>
        /// <param name="z0">start point of the Chain, in the argument plane</param>
        /// <param name="z1">end point of the Chain, in the argument plane</param>
        /// <param name="t0">start point in the pullback of the chain</param>
        /// <param name="tn">end point in the pullback of the chain</param>
        /// <param name="u_Part">real part of the image</param>
        /// <param name="v_Part">immaginary part of the image</param>
        /// <param name="x_coordinate">the first(abscissa) coordinate function, in the parametrization</param>
        /// <param name="y_coordinate">the second(ordinate) coordinate function, in the parametrization</param>
        /// <param name="dx_differential">the differential(i.e. measure) of the first(abscissa) coordinate function, in the parametrization</param>
        /// <param name="dy_differential">the differential(i.e. measure) of the second(ordinate) coordinate function, in the parametrization</param>
        /// <param name="n">the cardinality of the Riemann style partition, in the pullback of the Chain</param>
        /// <returns> a Complex:: instance, containing the integral result</returns>
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



        public static ComplexField.Complex ContourIntegral_AsScalar_ManagementMethod(
            ComplexField.Complex z0,
            ComplexField.Complex z1,
            double t0, double tn, // extrema in the pull-back
            fPtr_ComplexAsScalar_ complexAsScalar,
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
            if (!extremaAdequacy)//NB. le coordinate sono riferite al piano dell'argomento
            {// log & return null
                //Process::LogWrappers::SectionContent_fromMultipleStrings(0,1,new std::string("Integration extrema do not match, between coChain and Jordan-path."));
                //return nullptr;
            }//{ throw new System.Exception("Integration extrema do not match, between coChain and Jordan-path."); }
            //
            ComplexField.Complex res = new ComplexField.Complex(
                Integrate_equi_trapezium_ComplexImageAsScalar(
                    t0,tn
                    ,complexAsScalar
                    , x_coordinate
                    , y_coordinate
                    , dx_differential
                    , dy_differential
                    ,n )
            );
            //Process::LogWrappers::SectionClose();
            // ready.
            return res;// Caller has to delete.
        }// ContourIntegral_AsScalar_ManagementMethod
         

    }// class
}// nmsp
