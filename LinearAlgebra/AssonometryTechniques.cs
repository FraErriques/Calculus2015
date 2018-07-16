
//namespace LinearAlgebra
//{
//    public class AssonometryTechniques : RepresentationTechniques
//    {
//        //  Assonometry category data
//        protected LinearAlgebra.RealMatrix theTransformation = null;
//        //
//        // Assonometry category constructor
//        public AssonometryTechniques( double[,] theMatrixPrototype )
//        {
//            this.theTransformation = new RealMatrix( theMatrixPrototype, 2,3 );// T:R3->R2
//        }
//        //
//        // engine implementation
//        public override double[] representedPoint( double[] toBeRenderedByAssonometryPoint)
//        {
//            // NB. NOT using the shared data: "projection-board"
//            // NB. NOT using the shared data: "projection-point"
//            // This method will find the {a,b} plane coordinates, by means of a non/isomrphic
//            // transformation T:R3->R2. The null space will absorbe the trailing coordinate.
//            // Nor a projector-point neither a board plane are involved in such transformation.<----NB
//            // The transformation is based on the criterium of a base-change. 
//            // Calling E the R3 base and A the R2 base, the transformation matrix must be such that
//            // ET=A, where T is the transformation matrix.
//            // If E is the canon-base E=I3= the identity matrix in dimension three.
//            // Thus ET=A is equivalent to (I3)T=A -> T=A
//            // Thus the transformation operator is the matrix composed of the three vectors of the 
//            // new base. We will adopt column vectors to represent the base. Accordingly, also
//            // the variety-vectors will be expressed as column-vectors.
//            // Each variety-vector will be passed to the projectedPoint virtual-method. Such
//            // method multiplies it on the left by the transformation-operator and returns
//            // the transformed vector.
//            //
//            // implementation
//            //
//            double[,] toBeProjectedPoint_asMatrix = new double[3,1];
//            toBeProjectedPoint_asMatrix[0,0] = toBeRenderedByAssonometryPoint[0];
//            toBeProjectedPoint_asMatrix[1,0] = toBeRenderedByAssonometryPoint[1];
//            toBeProjectedPoint_asMatrix[2,0] = toBeRenderedByAssonometryPoint[2];
//            LinearAlgebra.RealMatrix theVarietyPoint = new RealMatrix( toBeProjectedPoint_asMatrix, 3,1 );
//            // follows the multiplication [2,3][3,1]=[2,1]
//            LinearAlgebra.RealMatrix thePointInAssonometry_asMatrix =
//                this.theTransformation.operator_mul( theVarietyPoint);
//            double[] thePointInAssonometry = new double[3];// 3 for compatibility with the projections, but the third coordinate is unused.
//            thePointInAssonometry[0] = thePointInAssonometry_asMatrix.get_element( 0,0);
//            thePointInAssonometry[1] = thePointInAssonometry_asMatrix.get_element( 1,0);
//            thePointInAssonometry[2] = 0.0;// 3 for compatibility with the projections, but the third coordinate is unused.
//            // ready
//            return thePointInAssonometry;
//        }// end projectedPoint


//    }// end  class RepresentationTechniques


//}// end namespace LinearAlgebra
