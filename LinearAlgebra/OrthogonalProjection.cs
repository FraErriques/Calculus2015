
//namespace LinearAlgebra
//{
//    /// <summary>
//    /// Each variety-point is projected by the line through it, and orthogonal to the board-plane.
//    /// It's like a central projection, with the projection-center located at infinity.
//    /// All the projection lines are projectively-parallel.
//    /// </summary>
//    public class OrthogonalProjection : ProjectionTechniques
//    {
//        /// <summary>
//        /// Constructor; its purpose is to initialize all of the projection data that will shared
//        /// by the sons among the hierarchy.
//        /// </summary>
//        public OrthogonalProjection(			// the central projector-point coordinates
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
//            // using the shared data: "projection-board"
//            // this method will find the normal line to projection board, through
//            // the "toBeProjectedPoint".
//            // the normal line to projection board is the parametric-line wich has the 
//            // first three coefficients of the cartesian equation of the projection-board
//            // as director-cosines.
//            // The affine to it, passing through the "toBeProjectedPoint" has parametric
//            // equations given by the method LinearAlgebra.Geometry_inR3.parametricLineSubjectToAnAffinity(...)
//            // Such line must be considered to find its intersection with the projection-board.
//            //
//            // implementation
//            //
//            double[,] parametricLineOrthogonalToBoard = new double[3,2];
//            // preserve the orientation, by leaving the director-cosines
//            parametricLineOrthogonalToBoard[0,0] = this.cartesianProjectionPlane[0];
//            parametricLineOrthogonalToBoard[1,0] = this.cartesianProjectionPlane[1];
//            parametricLineOrthogonalToBoard[2,0] = this.cartesianProjectionPlane[2];
//            //
//            // the traslation terms will be found by the following call
//            double[,] orthogonalToBoardTraslatedThroughVarietyPoint =
//                LinearAlgebra.Geometry_inR3.parametricLineSubjectToAnAffinity(
//                parametricLineOrthogonalToBoard,
//                toBeProjectedPoint  );
//            // look for the intersection with the board
//            double[] theIntersection =
//                LinearAlgebra.Geometry_inR3.intersectionBetweenCartesianPlaneAndParametricLine(
//                this.cartesianProjectionPlane,
//                orthogonalToBoardTraslatedThroughVarietyPoint   );
//            // ready
//            return theIntersection;
//        }// end projectedPoint


//    }// END class OrthogonalProjection
//}// end namespace LinearAlgebra
