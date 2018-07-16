
//namespace LinearAlgebra
//{
//    /// <summary>
//    /// central projection by matrix inversion
//    /// </summary>
//    public class CentralProjection : ProjectionTechniques
//    {
//        /// <summary>
//        /// Constructor; its purpose is to initialize all of the projection data that will shared
//        /// by the sons among the hierarchy.
//        /// </summary>
//        public CentralProjection(
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



//        // engine implementation
//        public override double[] representedPoint( double[] toBeProjectedPoint)
//        {
//            // using the shared data: "projection-board" and "projection-point"
//            // this method will find the line through the  "projection-point"
//            // and through the "toBeProjectedPoint". 
//            // Then such line will be considered to find its intersection with
//            // the projection board.
//            // the methods to be called are all in the class:
//            //       LinearAlgebra.Geometry_inR3
//            // the algorithmic sequence should be as explained above.
//            //
//            //    implementation
//            double[,] theTwoPlansGeneratingTheLine =
//                LinearAlgebra.Geometry_inR3.cartesianLineForTwoPoints(
//                // the central projector
//                this.projectionPointX,
//                this.projectionPointY,
//                this.projectionPointZ,
//                // the point on the variety
//                toBeProjectedPoint[0],
//                toBeProjectedPoint[1],
//                toBeProjectedPoint[2]   );
//            // find the projected point
//            double[] firstPlane  = new double[4] {
//                                                     theTwoPlansGeneratingTheLine[0,0],
//                                                     theTwoPlansGeneratingTheLine[0,1],
//                                                     theTwoPlansGeneratingTheLine[0,2],
//                                                     theTwoPlansGeneratingTheLine[0,3]  
//                                                 };
//            double[] secondPlane = new double[4] {
//                                                     theTwoPlansGeneratingTheLine[1,0],
//                                                     theTwoPlansGeneratingTheLine[1,1],
//                                                     theTwoPlansGeneratingTheLine[1,2],
//                                                     theTwoPlansGeneratingTheLine[1,3] 
//                                                 };
//            // This method inverts the matrix composed of the three cartesian equations( two for the line and
//            // one for the plane); only the coefficients of the three coordinates will be considered. The
//            // traslation terms are descarded and will compose the solution vector.
//            double[] theProjectedPointCoordinates =
//                LinearAlgebra.Geometry_inR3.intersectionPointBetweenCartesianLineAndCartesianPlane(
//                    // the projection-board-plane
//                    this.cartesianProjectionPlane,
//                    // the first line-generator-plane
//                    firstPlane,
//                    // the second line-generator-plane
//                    secondPlane    );
//            // ready
//            return theProjectedPointCoordinates;
//        }// end calculation method: projectedPoint

//    }// END class CentralProjection


//}// end namespace LinearAlgebra
