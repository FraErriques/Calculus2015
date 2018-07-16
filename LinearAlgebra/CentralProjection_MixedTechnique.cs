
//namespace LinearAlgebra
//{

//    /// <summary>
//    /// it's the same projection technique as CentralProjection, but implemented solving in the parameters'space
//    /// rather then calculating an inverse matrix.
//    /// </summary>
//    public class CentralProjection_MixedTechnique : ProjectionTechniques
//    {
//        /// <summary>
//        /// Constructor; its purpose is to initialize all of the projection data that will shared
//        /// by the sons among the hierarchy.
//        /// </summary>
//        public CentralProjection_MixedTechnique(			// the central projector-point coordinates
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
//            double planeZ2     ) : base( 
//            // the central projector-point coordinates
//            projectionPointX,
//            projectionPointY,
//            projectionPointZ,
//            // board plane: first point coordinates
//            planeX0,
//            planeY0,
//            planeZ0,
//            // board plane: second point coordinates
//            planeX1,
//            planeY1,
//            planeZ1,
//            // board plane: third point coordinates
//            planeX2,
//            planeY2,
//            planeZ2     )
//        {
//        }// END  "with-input-constructor"



//        /// <summary>
//        /// this method will be implemented by each son, to realize the proprer algorithm
//        /// </summary>
//        /// <param name="toBeProjectedPoint">these are the three coordinates of the point on the variety</param>
//        /// <returns>the three coordinates of the point on the projection board. One of these three coordinates
//        /// can be discarded when plotting the point on a device. It's interesting to test which
//        /// coordinate elimination damages less the perspective rendering. As a rule of thumb, if the board-plane
//        /// lies in parallel to a coordinate plane the projected points will be constant in one coordinate.
//        /// Such coordinate surely stores less perspective information than the other two.
//        /// In case the three coordinates vary together a test is necessary to understand the 
//        /// figure-specific geometry. Enjoy. :-)
//        /// </returns>
//        public override double[] representedPoint( double[] toBeProjectedPoint)
//        {
//            double[,] theParametricLine = LinearAlgebra.Geometry_inR3.parametricLineForTwoPoints(
//                // proiettore
//                this.projectionPointX,
//                this.projectionPointY,
//                this.projectionPointZ,
//                // proiettando
//                toBeProjectedPoint[0],
//                toBeProjectedPoint[1],
//                toBeProjectedPoint[2]   );
//            // 
//            double[] theSolutionPointInTheImmersionSpace =
//                LinearAlgebra.Geometry_inR3.intersectionBetweenCartesianPlaneAndParametricLine(
//                    this.cartesianProjectionPlane,
//                    theParametricLine );
//            // ready
//            return theSolutionPointInTheImmersionSpace;
//        }// end projectedPoint


//    }// end class CentralProjection_MixedTechnique


//}// end namespace LinearAlgebra
