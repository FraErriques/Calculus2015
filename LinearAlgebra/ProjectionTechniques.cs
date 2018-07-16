



//namespace LinearAlgebra
//{
//    /// <summary>
//    /// Father class. Data, common to all of the projection techniques, will be stored here
//    /// and shared by the sons.
//    /// This class hierarchy is intended to represent the projection engine. It implements
//    /// the major techniques used in building-engineering and architecture.
//    /// It borns with only the two most frequently used algorithms, but the class-hierarchy
//    /// architecture is full-extensible and provides a solid basys to host other algorithms
//    /// when they will be thought. Such flexibility is provided by means of object oriented
//    /// polimorphism. The hierarchy is composed of a father class, which is the common storage
//    /// area for static data( class-wise data). Each son class will implement its own version
//    /// of the projection algorithm. By now (August 2005) there will be only two sons: 
//    /// CentralProjection and OrthogonalAssonometry. The former is the classic projective-geometry
//    /// concept, from Girard Desargues. The latter is a more building-project used technique and
//    /// is also called military-assonometry.
//    /// </summary>
//    public abstract class ProjectionTechniques : RepresentationTechniques
//    {
//        /// data
//        // projection point coordinates
//        protected  double projectionPointX;
//        protected  double projectionPointY;
//        protected  double projectionPointZ;
//        // projection board plane: first point coordinates
//        protected  double planeX0;
//        protected  double planeY0;
//        protected  double planeZ0;
//        // projection board plane: second point coordinates
//        protected  double planeX1;
//        protected  double planeY1;
//        protected  double planeZ1;
//        // projection board plane: third point coordinates
//        protected  double planeX2;
//        protected  double planeY2;
//        protected  double planeZ2;
//        // projection plane parameters: parametric plane equations
//        // first indipendent parameter, in the three cartesian coordinates
//        protected  double projectionPlanTau_x;
//        protected  double projectionPlanTau_y;
//        protected  double projectionPlanTau_z;
//        // second indipendent parameter, in the three cartesian coordinates
//        protected  double projectionPlanUpsilon_x;
//        protected  double projectionPlanUpsilon_y;
//        protected  double projectionPlanUpsilon_z;
//        // traslation term, in the three cartesian coordinates
//        protected  double projectionPlanTraslation_x;
//        protected  double projectionPlanTraslation_y;
//        protected  double projectionPlanTraslation_z;
//        // plane cartesian equation
//        protected  double[] cartesianProjectionPlane = null;


//        // constructor
//        /// <summary>
//        /// Its purpose is to initialize all of the data that will shared by the sons among
//        /// the hierarchy.
//        /// </summary>
//        public ProjectionTechniques( 
//            // the central projector-point coordinates
//            double projectionPointX,
//            double projectionPointY,
//            double projectionPointZ,
//            // board plane: first point coordinates
//            double planeX0,
//            double planeY0,
//            double planeZ0,
//            // board plane: second point coordinates
//            double planeX1,
//            double planeY1,
//            double planeZ1,
//            // board plane: third point coordinates
//            double planeX2,
//            double planeY2,
//            double planeZ2     )
//        {
//            lock( typeof( ProjectionTechniques))// lock the father, the real data-container
//            {
//                {// subscope to accomodate all the static-data initialization
//                    // the central projector-point coordinates
//                    this.projectionPointX = projectionPointX;
//                    this.projectionPointY = projectionPointY;
//                    this.projectionPointZ = projectionPointZ;
//                    // board plane: first point coordinates
//                    this.planeX0 = planeX0;
//                    this.planeY0 = planeY0;
//                    this.planeZ0 = planeZ0;
//                    // board plane: second point coordinates
//                    this.planeX1 = planeX1;
//                    this.planeY1 = planeY1;
//                    this.planeZ1 = planeZ1;
//                    // board plane: third point coordinates
//                    this.planeZ0 = planeZ0;
//                    this.planeZ1 = planeZ1;
//                    this.planeZ2 = planeZ2;
//                    // calculated measures
//                    double[,] parametricProjectionPlane = null;
//                    // build plane cartesian equation
//                    try
//                    {
//                        this.cartesianProjectionPlane =
//                            LinearAlgebra.Geometry_inR3.cartesianPlaneThroughThreePoints_withoutNormalization(
//                            this.planeX0, this.planeY0, this.planeZ0,
//                            this.planeX1, this.planeY1, this.planeZ1,
//                            this.planeX2, this.planeY2, this.planeZ2   );
//                        // build plane parametric equations
//                        parametricProjectionPlane =
//                            LinearAlgebra.Geometry_inR3.parametricPlanFromCartesianPlan(
//                            this.cartesianProjectionPlane );
//                    }
//                    catch( System.Exception ex)
//                    {
//                        LogService.LogWrappers.SectionContent( "in constructor ProjectionTechniques: "+ex.Message , 2);
//                        throw ex;
//                    }
//                    // first indipendent parameter ("t"), in the three coordinate functions( x=x(t,u), y=y(t,u), z=z(t,u) )
//                    this.projectionPlanTau_x = parametricProjectionPlane[0,0];
//                    this.projectionPlanTau_y = parametricProjectionPlane[1,0];
//                    this.projectionPlanTau_z = parametricProjectionPlane[2,0];
//                    // second indipendent parameter ("u"), in the three coordinate functions( x=x(t,u), y=y(t,u), z=z(t,u) )
//                    this.projectionPlanUpsilon_x = parametricProjectionPlane[0,1];
//                    this.projectionPlanUpsilon_y = parametricProjectionPlane[1,1];
//                    this.projectionPlanUpsilon_z = parametricProjectionPlane[2,1];
//                    // traslation term, in the three cartesian coordinates. Affinity term.
//                    this.projectionPlanTraslation_x = parametricProjectionPlane[0,2];
//                    this.projectionPlanTraslation_y = parametricProjectionPlane[1,2];
//                    this.projectionPlanTraslation_z = parametricProjectionPlane[2,2];
//                }
//            }// end lock
//        }// END  constructor


//    }// END class ProjectionTechniques (the abstract father)


//}// END namespace LinearAlgebra
