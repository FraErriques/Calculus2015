
namespace LinearAlgebra
{


    /// <summary>
    /// a point in the plane.
    /// </summary>
    public class PointR2 // for future developements in the complex plane
    {
        private double x;
        private double y;


        
        
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public PointR2( double x, double y )
        {
            this.x = x;
            this.y = y;
        }// Ctor


        /// <summary>
        /// read-only x coordinate.
        /// </summary>
        public double getX
        {
            get
            {
                return this.x;
            }
        }


        /// <summary>
        /// read only y coordinate
        /// </summary>
        public double getY
        {
            get
            {
                return this.y;
            }
        }

        /// <summary>
        /// get the two coordinates, together.
        /// </summary>
        public double[] getCoordinates
        {
            get
            {
                double[] theTwoCoordinates = new double[2];
                theTwoCoordinates[0] = this.x;
                theTwoCoordinates[1] = this.y;
                return theTwoCoordinates;
            }
        }// getCoordinates


        /// <summary>
        /// scalar product between this and w.
        /// </summary>
        /// <param name="w">the second factor; the first factor is "this"</param>
        /// <returns></returns>
        public double scalarProduct( PointR2 w )
        {
            PointR3 v_inR3 = new PointR3( this.x, this.y, 0 );
            PointR3 w_inR3 = new PointR3( w.x, w.y, 0 );
            double scalar_inR3 = v_inR3.scalarProduct( w_inR3 );
            // ready.
            return scalar_inR3;
        }// end scalarProduct



        /// <summary>
        /// vector product between this and w, assuminig both in the x-y plane, i.e. z==0.
        /// </summary>
        /// <param name="w">the second factor; the first factor is "this"; the result will have the form (0,0,k) with k in R.</param>
        /// <returns></returns>
        public PointR3 vectorProduct( PointR2 w )
        {
            PointR3 v_inR3 = new PointR3( this.x, this.y, 0 );
            PointR3 w_inR3 = new PointR3( w.x, w.y, 0 );
            PointR3 external_inR3 = v_inR3.vectorProduct( w_inR3 );
            //ready
            return external_inR3;
        }// end vectorProduct()



    }// END class PointR2


}// END namespace LinearAlgebra
