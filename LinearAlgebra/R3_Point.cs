
namespace LinearAlgebra
{

    /// <summary>
    /// a point vector, in R3.
    /// </summary>
	public class PointR3
	{// punti, immersi in R3
		private double x;
		private double y;
		private double z;
		
        

        /// <summary>
        /// public access point: Ctor( coordinates).
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public PointR3( double x, double y, double z )
		{
			this.x = x;
			this.y = y;
			this.z = z;
		}// Ctor.
		
        
        /// <summary>
        /// public access point: Ctor.
        /// </summary>
        /// <param name="thePointCoordinates"></param>
		public PointR3( double[] thePointCoordinates )
		{
			this.x = thePointCoordinates[0];
			this.y = thePointCoordinates[1];
			this.z = thePointCoordinates[2];
		}// Ctor.


        /// <summary>
        /// copy Ctor.
        /// </summary>
        /// <param name="theOriginal">the original, from which to copy.</param>
        public PointR3( LinearAlgebra.PointR3 theOriginal )
        {
            this.x = theOriginal.getX;
            this.y = theOriginal.getY;
            this.z = theOriginal.getZ;
        }// Ctor.
		
        
        /// <summary>
        /// get the Abscissa
        /// </summary>
        public double getX
		{
			get{ return this.x;}
		}
		        
        /// <summary>
        /// get the Ordinate
        /// </summary>
        public double getY
		{
			get{ return this.y;}
		}
		
        /// <summary>
        /// get the Height
        /// </summary>
        public double getZ
		{
			get{ return this.z;}
		}
		
        

        /// <summary>
        /// get the three coordinates in one-shot 
        /// </summary>
        public double[] getPointCoordinates
		{
			get
			{
				double[] thePointCoordinates = new double[3];
				thePointCoordinates[0] = this.x;
				thePointCoordinates[1] = this.y;
				thePointCoordinates[2] = this.z;
				return thePointCoordinates;
			}
        }// end getPointCoordinates


        /// <summary>
        /// get the three coordinates, as a vector. 
        /// </summary>
        public PointR3 getPointVector()
        {
            return this;
        }// end getPointCoordinates


        /// <summary>
        /// product of a scalar for a vector, i.e. omothetia of a vector.
        /// </summary>
        public PointR3 omothetia( double theScalar)
        {
            LinearAlgebra.PointR3 stretchedVector = new PointR3(
                this.getX * theScalar,
                this.getY * theScalar,
                this.getZ * theScalar
              );
            return stretchedVector;
        }// end omothetia()


        /// <summary>
        /// scalar product between this and w.
        /// </summary>
        /// <param name="w">the second factor; the first factor is "this"</param>
        /// <returns></returns>
        public double scalarProduct( PointR3 w )
        {
            double scalar =   this.getX * w.getX
                            + this.getY * w.getY
                            + this.getZ * w.getZ;
            //
            return scalar;
        }// end scalarscalarProduct()


        /// <summary>
        /// |this|==(this_scalar_this)^(1/2).
        /// </summary>
        /// <returns>the vector length</returns>
        public double NormaEuclidea( )
        {
            double normaQuadrata = this.scalarProduct( this );
            //
            return System.Math.Sqrt( normaQuadrata );
        }// end NormaEuclidea


        /// <summary>
        /// vector product between this and w.
        /// </summary>
        /// <param name="w">the second factor; the first factor is "this"</param>
        /// <returns></returns>
        public PointR3 vectorProduct( PointR3 w )
        {
            double[,] externalProductTensor = new double[3, 3];
            double imageX, imageY, imageZ;
            // first row is dynamical: it will contain {e1, e2, e3}, once each.
            // second row is {this.x, this.y, this.z}
            // third row is {w.x, w.y, w.z}
            //
            externalProductTensor[0, 0] = 1.0;
            externalProductTensor[0, 1] = 0.0;
            externalProductTensor[0, 2] = 0.0;
            //
            externalProductTensor[1, 0] = this.x;
            externalProductTensor[1, 1] = this.y;
            externalProductTensor[1, 2] = this.z;
            //
            externalProductTensor[2, 0] = w.x;
            externalProductTensor[2, 1] = w.y;
            externalProductTensor[2, 2] = w.z;
            //
            LinearAlgebra.RealMatrix firstMatrix = new RealMatrix( externalProductTensor, 3, 3 );
            imageX = firstMatrix.det();
            //
            externalProductTensor[0, 0] = 0.0;
            externalProductTensor[0, 1] = 1.0;
            externalProductTensor[0, 2] = 0.0;
            //
            LinearAlgebra.RealMatrix secondMatrix = new RealMatrix( externalProductTensor, 3, 3 );
            imageY = secondMatrix.det();
            //
            externalProductTensor[0, 0] = 0.0;
            externalProductTensor[0, 1] = 0.0;
            externalProductTensor[0, 2] = 1.0;
            //
            LinearAlgebra.RealMatrix thirdMatrix = new RealMatrix( externalProductTensor, 3, 3 );
            imageZ = thirdMatrix.det();
            //
            PointR3 vectorProductImage = new PointR3( imageX, imageY, imageZ );
            //ready
            return vectorProductImage;
        }// end vectorProduct()




	}// END class PointR3


}// END namespace LinearAlgebra
