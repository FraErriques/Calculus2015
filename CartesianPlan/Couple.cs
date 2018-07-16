using System;

namespace CartesianPlan
{
	/// <summary>
	/// Couple is an (abscissa, ordinate) coordinates couple in the reference cartesian plan.
	/// </summary>
	public class Couple : CartesianPlan.CartesianReference
	{
		// data
		private float x;
		private float y;

		// properties
		/// <summary>
		/// the following properties expose( readonly) the two plan-coordinates of the current point.
		/// Note that each point is an instance of the Couple class, while device dependent concepts,
		/// like device coordinates, omothetia factors, are father-class pertinence and are thought
		/// static, since all the son-instances, which are the drawn points are contained in the same
		/// device. So the architecture is the same father attributes for all the sons. If no static fields
		/// are used each son has its own copy of the father. Here this is avoided by letting the father's
		/// attribute static. So each point (son's instance) shares the father's static attributes, which
		/// represent the phisical-hardware features of the graphical device.
		/// </summary>
		public float getX
		{
			get{ return this.x;}
		}
		public float getY
		{
			get{ return this.y;}
		}
		/// <summary>
		/// the following two properties are referred to the father-class CartesianReference
		/// these two classes( father-CartesianReference and son Couple) are thought thinking
		/// that the phisical device is unique, while the drawable points are many.
		/// So everithing that is device-concerning is a class-level attribute, then static
		/// What is point-concerning is at instance-level, then non-static
		/// the two helper properties expose( readonly) the omothetia-factors, which are
		/// intrinsically device-dependent, then static.
		/// </summary>
		public static float getOmothetiaX
		{
			get{ return CartesianPlan.CartesianReference.omothetiaFactorX;}
		}
		public static float getOmothetiaY
		{
			get{ return CartesianPlan.CartesianReference.omothetiaFactorY;}
		}
		//
		/// <summary>
		/// helpers to retrieve the shift in each coordinate.
		/// </summary>
		public static float getShiftX
		{
			get{ return CartesianPlan.CartesianReference.shiftAbscissa;}
		}
		public static float getShiftY
		{
			get{ return CartesianPlan.CartesianReference.shiftOrdinate;}
		}

		// methods
		/// constructor, devoted to coordinate transformation
		/// the omothetiaFactor has dimension d/f, where d:=device and f:=functionMeasure.
		/// the input parametra (x,y) have dimension functionMeasure. So, when multiplied by omothetia factor
		/// they acquire measure f*(d/f)~d
		/// then the device-origin coordinate is summed for traslation, and the result has dimension device, as 
		/// required, since the device-origin coordinate has dimension device herself
		public Couple( float x, float y)
		{
			this.x =
				CartesianPlan.CartesianReference.shiftAbscissa + x * CartesianPlan.CartesianReference.omothetiaFactorX;
			this.y =
				CartesianPlan.CartesianReference.maxY -( CartesianPlan.CartesianReference.shiftOrdinate + y*CartesianPlan.CartesianReference.omothetiaFactorY);
		}// END ctor, devoted to coordinate transformation

		/// <summary>
		/// this is an helper method, just to know if the desired( calculated) point is contained
		/// in the prepared board or lies outside it, which lets unpossible showing it on the current
		/// board. Many times the image interval required by the user is not appropriate to contain
		/// the actual image-function points. This helper can be useful to log undrawable points.
		/// Each point should be checked and then, depending on the check-result, plotted or logged.
		/// </summary>
		/// <returns></returns>
		public bool isContained( )
		{// an helper method, to know if current point is drawable
			bool res = 
				(	this.x>=0 && this.x<=CartesianPlan.CartesianReference.maxX  &&
					this.y>=0 && this.y<=CartesianPlan.CartesianReference.maxY        );
			return res;
		}// end isContained method


	}// end class couple
}// end namespace CartesianPlan
