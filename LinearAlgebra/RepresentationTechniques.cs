
namespace LinearAlgebra
{

	/// <summary>
	/// Engine interface:
	/// the sons will implement their constructors. This father is not buildable.
	/// </summary>
	public abstract class RepresentationTechniques
	{

		/// <summary>
		/// This method will be implemented by each son, to realize the proprer algorithm
		/// </summary>
		/// <param name="toBeRepresentedPoint">these are the three coordinates of the point on the variety</param>
		/// <returns>the three coordinates of the point on the projection board. One of these three coordinates
		/// can be discarded when plotting the point on a device. It's interesting to test which
		/// coordinate elimination damages less the perspective rendering. As a rule of thumb, if the board-plane
		/// lies in parallel to a coordinate plane the projected points will be constant in one coordinate.
		/// Such coordinate surely stores less perspective information than the other two.
		/// In case the three coordinates vary together a test is necessary to understand the 
		/// figure-specific geometry. Enjoy. :-)
		/// </returns>
		public abstract double[] representedPoint( double[] toBeRepresentedPoint);
	}// end  class RepresentationTechniques


}// end namespace LinearAlgebra
