using System;

namespace CartesianPlan
{
	/// <summary>
	/// CartesianReference represents the working plan, both from the algorithmic point of
	/// view ( functional measure unit) and from the technical one (pixels on the graphics device)
	/// </summary>
	public class CartesianReference
	{
		// data
		static protected int	maxX;// phisical device max abscissa( i.e. pixel cardinality in this coordinate)
		static protected int	maxY;// phisical device max ordinate
		//
		static protected float  desiredMinX;// user required min abscissa
		static protected float  desiredMaxX;// user required max abscissa
		static protected float  desiredMinY;// user required min ordinate
		static protected float  desiredMaxY;// user required max ordinate
		//
		// Definitions: 
		//				omothetiaFactorJ:= (maxJ-0)/(desiredMaxJ-desiredMinJ)
		//				At numberer there is a device discrete measure, say pixel cardinality.
		//				At denominator there is the continuous measure of the function domain.
		//				----
		// The ratio represents how many device entities are devoted to represent a function unit.
		// Examples:
		//	es.1	(600-0)/(4-2) = 300 means 300 pixels devoted to each unit in the domain.
		//	es.2	(600-0)/6000 = 1/10 means each pixel devoted to ten function units.
		// NB. when applying the omothetiaFactorJ to the corresponding coordinate, what
		//		happens in terms of measure is:
		//			function_coordinate_j*(maxJ)/(desiredMaxJ-desiredMinJ)
		//	dimensionally means (I interpret the dimension of each term)
		//		function_measure*device_measure/function_measure~device_measure
		// the result has dimension "device", so it's ready to be plotted.
		static protected float  omothetiaFactorX;// omothetia factor, for scaled rendering on abscissa coordinate
		static protected float  omothetiaFactorY;// omothetia factor, for scaled rendering on ordinate coordinate
		//
		static protected float  originAbscissa;// x coordinate of the origin, respect to the device coordinates( in functional coordinates it's zero)
		static protected float  originOrdinate;// y coordinate of the origin, respect to the device coordinates( in functional coordinates it's zero)
		//
		// Definitions: 
		//				device origin:= the (0,0) point on the hardware device:
		//				it's unmovable and is the North-Ovest corner of the screen.
		//				----
		//				function origin:= the (desiredMinX,desiredMinY) point,
		//				requested from the user.
		//				----
		// The correction factor is the opposite (i.e. -1* ) of the minimum of the
		// required interval. This because the omothetiaFactor considers the device-origin
		// coinciding with the function-origin. If no such coincidence, there must be a shift
		// which corrects for the effective function start point. The device origin is unmovable,
		// but the function origin is shifted each time a minimum required differs from zero.
		// A request to have an [a, b] interval( in wathever coordinate) means that the device-
		// origin represents "a" and not zero, in such coordinate. So after the omothetia filtering
		// there must be a shift of (-1)*a, meaning that the point must not be positioned as the
		// function origin were zero, as the omothetia filtering expects, but the point positioning
		// must start from "a". To do this I must eliminate the "a" portion, i.e. the portion
		// between zero and "a" by applying a shift of "(-1)*a". An analogous argument must
		// be applyied when the required field is [-a, b] (assuming always a>0 and b>0); this
		// time however the device origin represents "-a", so the count from zero must be added
		// of the measure |-a-0|=a, so the shift is (-1)*(-a)=a. In general, considering
		// an interval [q,w], with q and w of any sign, the shift will be (-1)*q, with 
		// omothetia filtering, as shown below:
		static protected float  shiftAbscissa;// x shift in the device positioning:device origin and function origin are not the same.
		static protected float  shiftOrdinate;// y shift in the device positioning
		//
		/// init flag: points cannot lay out of a reference-system
		/// there are two constructors: the parameter-provided one is intended to build the cartesian-reference
		/// on the first call, when the ranges of the two coordinates must be decided and the axes must be built.
		/// Once initialized the cartesian-reference-plan, each point can be added to it.
		static private bool initialized = false;
		//
		static public int getMaxX
		{
			get
			{
				return CartesianPlan.CartesianReference.maxX;
			}
		}

		// methods
		// set current device dimensions into CartesianReference instance
		public CartesianReference( )
		{
			if( ! initialized)
				throw new System.Exception( "this constructor is intended only to build successive points");
		}

		// a constructor to init the plan reference
		public CartesianReference(  
			System.Windows.Forms.PictureBox pb,
			float desiredMinX,
			float desiredMaxX,
			float desiredMinY,
			float desiredMaxY    )
		{
			// caratteristiche fisiche del dispositivo
			maxX = pb.Size.Width;
			maxY = pb.Size.Height;
			// zoom richiesto dall'utente
			CartesianReference.desiredMinX = desiredMinX;
			CartesianReference.desiredMaxX = desiredMaxX;
			CartesianReference.desiredMinY = desiredMinY;
			CartesianReference.desiredMaxY = desiredMaxY;
			if( desiredMinX >= desiredMaxX || desiredMinY >= desiredMaxY )
				throw new System.Exception("Domain inconsistency.");
			// read documentation above, about omothetiaFactors.
			CartesianReference.omothetiaFactorX = ((float)maxX) / (desiredMaxX-desiredMinX);
			CartesianReference.omothetiaFactorY = ((float)maxY) / (desiredMaxY-desiredMinY);
			// read documentation above, about shiftCoordinate( where Coordinate is Abscissa or Ordinate).
			CartesianReference.shiftAbscissa =
				(-1.0F)*CartesianReference.desiredMinX*CartesianReference.omothetiaFactorX;
			CartesianReference.shiftOrdinate =
				(-1.0F)*CartesianReference.desiredMinY*CartesianReference.omothetiaFactorY;
			// set initialization flag
			initialized = true;
		}// end constructor


		
		/// <summary>
		/// origin coordinates and axis building
		/// </summary> note that the coordinates of the Axis estrema are device-absolute. They cannot be relative
		/// to themselves, obviously. So there is no call to the relative-coordinate adapter.
		/// <returns>the coordinates of the abscissa Axis estrema</returns>
		public float[] abscissaAxis()
		{
			float[] res = new float[4];
			if( CartesianPlan.CartesianReference.desiredMinY *
				CartesianPlan.CartesianReference.desiredMaxY >=0.0 )
			{return null;}// estremi dello stesso segno
			else
			{
				float downLength  = Math.Abs( CartesianReference.desiredMinY);
				float wholeLength = CartesianReference.desiredMaxY-CartesianReference.desiredMinY;
				float ratio = downLength / wholeLength;
				// the equation is o=(r.m)
				// where r:=ratio =downLength/wholeLength
				// m:=CartesianReference.maxY
				// o:=CartesianPlan.CartesianReference.originOrdinate
				// the equation is derived considering the partitioning, into two portions, of the zero-beginning ordinate space(device-space)
				CartesianPlan.CartesianReference.originOrdinate =
					(float)CartesianReference.maxY - ratio*((float)CartesianReference.maxY);
				// start point(abscissa, ordinate) of Abscissa Axis
				res[0] =  0.0F;
				res[1] = CartesianPlan.CartesianReference.originOrdinate;
				// end point (abscissa, ordinate) of Abscissa Axis
				res[2] = (float)((float)(maxX));
				res[3] = CartesianPlan.CartesianReference.originOrdinate;
			}
			return res;
		}// end abscissaAxis method


		public float[] ordinateAxis()
		{
			float[] res = new float[4];
			if( CartesianPlan.CartesianReference.desiredMinX *
				CartesianPlan.CartesianReference.desiredMaxX >=0.0 )
			{return null;}// estremi dello stesso segno
			else
			{
				float leftLength  = Math.Abs( CartesianReference.desiredMinX);
				float wholeLength = CartesianReference.desiredMaxX - CartesianReference.desiredMinX;
				float ratio = leftLength / wholeLength;
				// the equation is o=(r.m)
				// where r:=ratio =leftLength / wholeLength
				// m:=CartesianReference.maxX
				// o:=CartesianPlan.CartesianReference.originAbscissa
				// the equation is derived considering the partitioning, into two portions, of the zero-beginning abscissa space(device-space)
				CartesianPlan.CartesianReference.originAbscissa = ratio*((float)CartesianReference.maxX);
				// start point (abscissa, ordinate) of Ordinate Axis
				res[0] = CartesianPlan.CartesianReference.originAbscissa;
				res[1] =  0.0F;
				// end point (abscissa, ordinate) of Ordinate Axis
				res[2] = CartesianPlan.CartesianReference.originAbscissa;
				res[3] = (float)((float)(maxY));
			}
			return res;
		}// end ordinateAxis method


	}// end class CartesianReference
}// end namespace CartesianPlan
