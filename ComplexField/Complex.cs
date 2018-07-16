
namespace ComplexField
{

	public class Complex
	{
		private double real;
		private double immaginary;
		public struct Argument
		{
			public bool		isTheNullVector;
			public double	degrees;
			public double	radiants;
		}// end  struct Argument
		private const double epsilon = System.Double.Epsilon;// 4.94065645841247E-324 
		// read only property
		public double get_real
		{
			get
			{
				return this.real;
			}
		}
		// read only property
		public double get_immaginary
		{
			get
			{
				return this.immaginary;
			}
		}

		// Constructors
		// no default constructor. The complex number must be initialized in both its real and immaginary part.
		public Complex( double real, double immaginary)
		{
			this.real = real; 
			this.immaginary = immaginary;
		}// end constructor

		public Complex( Complex that)//copy-constructor
		{
			this.real = that.real;
			this.immaginary = that.immaginary;
		}// end copy-constructor
		// end Constructors

		// operators
		// operator +
		public static Complex operator + ( Complex left, Complex right)
		{// (a,b)+(c,d)= (a+c) + i (b+d)
			return new Complex( left.real + right.real,
				left.immaginary + right.immaginary    );
		}// binary +
		public static Complex operator + ( double left, Complex right)
		{// (a )+(c,d)= (a+c) + i ( d)
			return new Complex( left  + right.real,
				+ right.immaginary    );
		}// binary +
		public static Complex operator + ( Complex left, double right)
		{// (a,b)+(c )= (a+c) + i (b )
			return new Complex( left.real + right,
				left.immaginary     );
		}// binary +
		public static Complex operator + ( Complex z)
		{// +(a,b) = (+a,+b)
			return z;// == new Complex( z.real, z.immaginary );
		}// unary +
		// end operator +

		// operator -
		public static Complex operator - ( Complex left, Complex right)
		{// (a,b)-(c,d)= (a-c) + i (b-d)
			return new Complex( left.real - right.real,
				left.immaginary - right.immaginary    );
		}// end binary -
		public static Complex operator - ( double left, Complex right)
		{// (a )-(c,d)= (a-c) + i ( -d)
			return new Complex( left  - right.real,
				- right.immaginary    );
		}// end binary -
		public static Complex operator - ( Complex left, double right)
		{// (a,b)-(c )= (a-c) + i (b )
			return new Complex( left.real - right ,
				left.immaginary          );
		}// end binary -
		public static Complex operator - ( Complex z)
		{// -(a,b) = (-a,-b)
			return new Complex( -z.real, -z.immaginary );
		}// end unary -
		// operator -

		// operator *
		public static Complex operator * ( Complex left, Complex right)
		{// (a,b)(c,d)= (ac-bd) + i (ad+bc)
			return new Complex( left.real*right.real       - left.immaginary*right.immaginary,
				left.real*right.immaginary + left.immaginary*right.real         );
		}
		public static Complex operator * ( double left, Complex right)
		{// (a,0)(c,d)= (ac ) + i (ad )
			return new Complex( left *right.real,
				left *right.immaginary  );
		}
		public static Complex operator * ( Complex left, double right)
		{// (a,b)(c )= (ac ) + i ( bc)
			Complex rightComp = new Complex( right,0);
			return left * rightComp;
		}
		// end operator *

		// operator /
		public static Complex operator / ( Complex left, Complex right)
		{// (a,b) / (c,d)= (a,b)/(c,-d) * (c,-d)/(c,d) = (a,b)(c,-d)/(c^2,d^2)
			//  es.   1/i = -i
			// implementazione:
			// - prodotto del dividendo per il coniugato del divisore
			// - dividere parte reale e parte immaginaria del dividendo per il moduloquadro del divisore
			Complex denominatorSquareModulus = right * right.coniugate();
			Complex numerator = left * right.coniugate();
			double den = denominatorSquareModulus.get_real;
			Complex result = numerator / den;
			return result;
		}
		public static Complex operator / ( double left, Complex right)
		{// (a ) / (c,d)= (a )/(c,-d) * (c,-d)/(c,d) = (a,0)(c,-d)/(c^2+d^2,0)
			//  es.   1/i = -i
			// implementazione:
			// - prodotto del dividendo per il coniugato del divisore
			// - dividere parte reale e parte immaginaria del dividendo per il moduloquadro del divisore
			double denominatorSquareModulus = ((Complex)(right * right.coniugate())).get_real;
			Complex result = left * right.coniugate();
			return new Complex( result.real/denominatorSquareModulus, result.immaginary/denominatorSquareModulus);
		}
		public static Complex operator / ( Complex left, double right)
		{// (a,b) / (c,0)= (a,b)/(c,0) = (a,b)(c,0)/(c^2,0)
			//  es.   1/i = -i
			// implementazione:
			// - dividere parte reale e parte immaginaria per il divisore( right)
			return new Complex( left.real/right, left.immaginary/right);
		}
		// end operator /


        


        /// <summary>
        ///  operator ^ represents Power; eg. a^b==Exp( b*Log(a))
        /// </summary>
        /// <param name="theBase">base</param>
        /// <param name="theExp">exponent</param>
        /// <returns>power</returns>
		public static Complex operator ^ ( Complex theBase, Complex theExp)
		{// (a,b) ^ (c,d)= Exp( (c,d)*Log((a,b)) )
			//  es.   2^3 = 2^(3*Log2(2))
			// implementazione:
			return(
				new Complex( ComplexField.Functions.Exp(	theExp *
				ComplexField.Functions.Log( theBase)
				)// end Exponential call
				)// end new
				);// end return
		}// end operator ^


		public static Complex operator ^ ( double theBase, Complex theExp)
		{// (a,0) ^ (c,d)= Exp( (c,d)*Log((a )) )
			//  es.   2^3 = 2^(3*Log2(2))
			// implementazione:
			return(
				new Complex( ComplexField.Functions.Exp(	theExp *
				ComplexField.Functions.Log( new ComplexField.Complex( theBase, 0.0) )
				)// end Exponential call
				)// end new
				);// end return
		}
		public static Complex operator ^ ( Complex theBase, double theExp)
		{// (a,b) ^ (c )= Exp( (c,0)*Log((a,b)) )
			//  es.   2^3 = 2^(3*Log2(2))
			// implementazione:
			return(
				new Complex( ComplexField.Functions.Exp(	theExp *
				ComplexField.Functions.Log( theBase)
				)// end Exponential call
				)// end new
				);// end return
		}
		// end operator ^

		// operator(s) if(Complex)
		// Returns true if the considered complex numer has immaginary-part coefficient zero.
		public static bool operator true( Complex z)
		{
			if( System.Math.Abs( z.immaginary) < epsilon)
			{
				return true;
			}
			return false;
		}

		// Returns false if the considered complex numer has immaginary-part coefficient NON zero.
		public static bool operator false( Complex z)
		{
			if( System.Math.Abs( z.immaginary) < epsilon)
			{
				return true;
			}
			return false;
		}
		// end operator if(Complex)

		// operator cast, explicit
		public static explicit operator double( Complex z)
		{
			if( System.Math.Abs( z.immaginary) < epsilon)
			{
				return z.get_real;
			}
			throw new System.Exception( "the considered complex number is NOT real. Cannot convert");
		}
		// end operator cast, explicit

		// helper method modulus
		public double modulus()
		{// the Euclidean-R2-length
			return System.Math.Sqrt( 
				System.Math.Pow( this.real, 2.0)		+
				System.Math.Pow( this.immaginary, 2.0)
				);
		}//
		//
		public static double modulus( Complex z)
		{// the Euclidean-R2-length
			return System.Math.Sqrt( 
				System.Math.Pow( z.real, 2.0)		+
				System.Math.Pow( z.immaginary, 2.0)
				);
		}//
		// end helper method modulus

		public Complex coniugate()
		{// (a,b).coniugate = (a,-b)
			return new Complex( this.real, -this.immaginary);
		}// end coniugate

		public Argument argument()
		{// this.argument() represents the angle formed by the (this.real, this.immaginary) R2 vector with
			// the abscissa positive semi-axis, with variation between (-Pi, Pi]
			Argument theArgument = new Argument();
			// if x==0
			if( System.Math.Abs( this.real) < epsilon )
			{
				if( System.Math.Abs( this.immaginary) < epsilon )
				{
					theArgument.isTheNullVector = true;
					return theArgument;
				}
				else if( this.immaginary > 0.0)
				{
					theArgument.radiants = System.Math.PI/2.0;
					theArgument.isTheNullVector = false;
				}
				else if( this.immaginary < 0.0)
				{
					theArgument.radiants = - System.Math.PI/2.0;
					theArgument.isTheNullVector = false;
				}
			}// end  if x==0
			else if( this.real > 0.0)
			{// if x>0
				theArgument.radiants = System.Math.Atan( this.immaginary / this.real );
				theArgument.isTheNullVector = false;
			}// end if x>0
			else if( this.real < 0.0)
			{// if x<0
				if( System.Math.Abs( this.immaginary) < epsilon )
				{
					theArgument.radiants = System.Math.PI;
					theArgument.isTheNullVector = false;
				}
				else if( this.immaginary > 0.0)
				{
					theArgument.radiants = System.Math.PI + System.Math.Atan( this.immaginary / this.real );
					theArgument.isTheNullVector = false;
				}
				else if( this.immaginary < 0.0)
				{
					theArgument.radiants = -System.Math.PI + System.Math.Atan( this.immaginary / this.real );
					theArgument.isTheNullVector = false;
				}
			}// end if x<0
			// deg:180 = rad:PI  -> deg=rad*180/PI
			theArgument.degrees = theArgument.radiants * 180.0 / System.Math.PI;
			return theArgument;
		}// end argument method


		public override string ToString()
		{
			return "("+this.real.ToString()+") +i*( "+this.immaginary.ToString()+")";
		}

		//		public override bool Equals(object obj)
		//		{
		//			return object.ReferenceEquals( this, obj);
		//		}

		//		public override bool Equals(object obj)
		//		{
		//			bool result;
		//			Comlex B = (Complex)obj;
		//
		//			if( B.r
		//		}




		// end helper methods

	}// end class



}// end nmsp
