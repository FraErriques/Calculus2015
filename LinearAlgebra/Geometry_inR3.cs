

namespace LinearAlgebra
{
	/// <summary>
	/// Geometry_inR3 is a class that exposes static calculation methods
	/// for affine geometry in the ordinary space.
	/// No class-wise or instance-wise data is needed, so all of the involved data is exchanged
	/// through input-parameters and return-value, by each method.
	/// Each method is static. There is nothing to instantiate.
	/// </summary>
	public class Geometry_inR3
	{
		// this class has no constructor, since it has no data.
		// All the data needed for linear-algebra calculations are method-local.
		// All the methods are R3 specific algorithms; each of them is static.




        /// <summary>
        /// equazioni parametriche della retta per due punti
        /// </summary>
        /// <param name="ax">coordinate x of point a</param>
        /// <param name="ay">coordinate y of point a</param>
        /// <param name="az">coordinate z of point a</param>
        /// <param name="bx">coordinate x of point b</param>
        /// <param name="by">coordinate y of point b</param>
        /// <param name="bz">coordinate z of point b</param>
        /// <returns>the result will be stored in the format {{a*t, x0},  {b*t, y0}, {c*t, z0} }</returns>
        static public double[,] parametricLineForTwoPoints(
					double ax, double ay, double az,
					double bx, double by, double bz   )
		{// the result will be stored in the format
		    //   {{a*t, x0},
			//	  {b*t, y0},
			//	  {c*t, z0} }
			double[,] parametricLineCoefficients = new double[3,2];// so the first column are the independent-parameter-coefficients, while the second column are the traslation terms
			// independent parameter coefficients
			parametricLineCoefficients[0,0] = bx-ax;//==x1-x0==DeltaX==a
			parametricLineCoefficients[1,0] = by-ay;//==y1-y0==DeltaY==b
			parametricLineCoefficients[2,0] = bz-az;//==z1-z0==DeltaZ==c
			// traslation vector components
			parametricLineCoefficients[0,1] = ax;//==x0==abscissa of the point, crossed when the parameter has value==0.0
			parametricLineCoefficients[1,1] = ay;//==y0==ordinate of the point, crossed when the parameter has value==0.0
			parametricLineCoefficients[2,1] = az;//==z0==quota of the point, crossed when the parameter has value==0.0
			// check if the line is not degenerate
			if( System.Math.Abs( parametricLineCoefficients[0,0])<1.0e-40 &&
				System.Math.Abs( parametricLineCoefficients[1,0])<1.0e-40 &&
				System.Math.Abs( parametricLineCoefficients[2,0])<1.0e-40     )
			{
				throw new System.Exception("degenerate line: it's only a point");
			}
			// ready
			return parametricLineCoefficients;
		}// end parametricLineForTwoPoints()







		
        
        /// <summary>
        /// equazioni cartesiane dei due piani generatori di una retta.
        /// i.e. equazioni cartesiane di due piani, appartenenti al fascio proprio generato da una retta, individuata da due suoi punti.
        /// </summary>
        /// <param name="ax">coordinate x of point a</param>
        /// <param name="ay">coordinate y of point a</param>
        /// <param name="az">coordinate z of point a</param>
        /// <param name="bx">coordinate x of point b</param>
        /// <param name="by">coordinate y of point b</param>
        /// <param name="bz">coordinate z of point b</param>
        /// <returns>the result will be stored in the format {{a*x, b*y, c*z, d}, {e*x, f*y, g*z, h}}</returns>
        static public double[,] cartesianLineForTwoPoints(
            double ax, double ay, double az,
			double bx, double by, double bz   )
		{// the result will be stored in the format {{a*x, b*y, c*z, d}, {e*x, f*y, g*z, h}}
			double[,] cartesianLineCoefficients = new double[2,4];// so each row represents a cartesian equation, refferred to one of the generator planes
			// the procedure consists in finding two points, each of which does not belong to the line. The two points must
			// also lie on different plans, each of which contains the line. Such plans characterize a proper bundle, whose
			// generating line is the line between the points in input.
			// Operatively: 
			//				- the first point will be found from the parametric form of the input line, for a given
			//				  value of the parameter, altering only the third coordinate. Such point evidently lies out
			//				  of the line.
			//
			//				- the second point will be found from the cartesian form of the first plane. From it will
			//				  be found the parametric form of the plane, by means of the Monge parametrization, which
			//				  is globally possible in a plane. In such equations, for a given couple of values of the
			//				  two parameters, altering the dependent-coordinate equation only, the second point will
			//				  be found. Such point evidently lies out of the first plane. Since the first plane
			//				  contains the input-line, the second point lies out also of the input-line.
			//
			//				- now we got two set of three points. Two of them are the one that generate the input-line.
			//				  Considering them along with each of the recently calculated points, I will have two cartesian
			//				  equations, representing the plans that generate the input-line. Such representation of the
			//				  line is alternative to the parametric one, and is particularly useful when looking for the 
			//				  intersection from a line and a plane. Enjoy :-)
			//
            //
			double[,] parametricFormOfInputLine = parametricLineForTwoPoints( ax, ay, az,
				bx, by, bz   );
			double[] firstPoint = new double[3];
			double t = 1.0;// choosen value for the "t" parameter
			firstPoint[0] = parametricFormOfInputLine[0,0]*t + parametricFormOfInputLine[0,1];
			firstPoint[1] = parametricFormOfInputLine[1,0]*t + parametricFormOfInputLine[1,1];
			firstPoint[2] = parametricFormOfInputLine[2,0]*t + parametricFormOfInputLine[2,1];
			// NB exit technique
			// select the coordinate to be incremented, to find a point outside the line.
			// such coordinate must have a non-zero delta.
			double Dx = bx - ax;
			double Dy = by - ay;
			double Dz = bz - az;
			bool oneCoordinateHasBeenIncremented = false;
			if( System.Math.Abs(Dx) < 1.0e-80 )
			{
				firstPoint[0]++;
				oneCoordinateHasBeenIncremented = true;
			}
			else if( System.Math.Abs(Dy) < 1.0e-80 )
			{
				firstPoint[1]++;
				oneCoordinateHasBeenIncremented = true;
			}
			else if( System.Math.Abs(Dz) < 1.0e-80 )
			{
				firstPoint[2]++;
				oneCoordinateHasBeenIncremented = true;
			}
			if( ! oneCoordinateHasBeenIncremented )
			{// if all the coordinates have a non-zero delta, then each of them equivalently can be incremented
				firstPoint[0]++;
			}
			//
			double[] firstPlanCartesianEquation = cartesianPlaneThroughThreePoints_withoutNormalization(
				ax, ay, az,
				bx, by, bz,
				firstPoint[0], firstPoint[1], firstPoint[2]	);

			// qui bisogna chiamare il metodo che genera le equazioni parametriche di un piano a partire dalla sua equazione cartesiana
			double[,] firstPlanParametricEquation = parametricPlanFromCartesianPlan( firstPlanCartesianEquation );// [3,3]
			double[] secondPoint = new double[3];
			double u = 1.0;// choosen value for the "t" parameter

			// riconoscimento della coordinata dipendente: possono farlo tutte e sole quelle che hanno
			// coefficiente non nullo nell'equazione cartesiana del piano. Scegliero' la prima
			// nel programma.
			int candidateCoordinate=0;
			for(; candidateCoordinate<3; candidateCoordinate++)
			{
				if( System.Math.Abs( firstPlanCartesianEquation[candidateCoordinate])>1.0e-13)// !=0.0
					break;
			}
			if( 3<=candidateCoordinate)
			{
				throw new System.Exception( " all the coordinates have zero-coefficients in the plane cartesian equation");
			}
            //
			secondPoint[0] = firstPlanParametricEquation[0,0]*t + firstPlanParametricEquation[0,1]*u + firstPlanParametricEquation[0,2];
			secondPoint[1] = firstPlanParametricEquation[1,0]*t + firstPlanParametricEquation[1,1]*u + firstPlanParametricEquation[1,2];
			secondPoint[2] = firstPlanParametricEquation[2,0]*t + firstPlanParametricEquation[2,1]*u + firstPlanParametricEquation[2,2];
			// NB exit technique
			secondPoint[candidateCoordinate]++;
			// ready to build second plane cartesian equation
			double[] secondPlanCartesianEquation = cartesianPlaneThroughThreePoints_withoutNormalization(
				ax, ay, az,
				bx, by, bz,
				secondPoint[0], secondPoint[1], secondPoint[2]	);
			// prepare return variable
			// first plane cartesian coefficients
			cartesianLineCoefficients[0,0] = firstPlanCartesianEquation[0];
			cartesianLineCoefficients[0,1] = firstPlanCartesianEquation[1];
			cartesianLineCoefficients[0,2] = firstPlanCartesianEquation[2];
			cartesianLineCoefficients[0,3] = firstPlanCartesianEquation[3];
			// second plane cartesian coefficients
			cartesianLineCoefficients[1,0] = secondPlanCartesianEquation[0];
			cartesianLineCoefficients[1,1] = secondPlanCartesianEquation[1];
			cartesianLineCoefficients[1,2] = secondPlanCartesianEquation[2];
			cartesianLineCoefficients[1,3] = secondPlanCartesianEquation[3];
			// ready
			return cartesianLineCoefficients;
		}// end cartesianLineForTwoPoints()


		

		/// <summary>
		///   Metodo che genera le equazioni parametriche di un piano a partire dalla equazione cartesiana.
		///   The result will be stored in the form:
		/// 			   { x:{a*t, b*u, c},
		/// 				 y:{d*t, e*u, f},
		/// 				 z:{g*t, h*u, i}   }
		///  particularly the Monge parametrization will be used, which is a map where a coordinate
		///  function dependes monodromically on the other parameters, each of which is explicit.
		///  such parametrization assumes the form:
		///  x = x(t,u) = t
		///  y = y(t,u) = u
		///  z = z(t,u) = f(x,y) = f(x(t,u),y(t,u))= f(t,u)
		/// 
		///  nevertheless in the input cartesian plane equation some of the coefficients may be zero.
		///  So as first will be located the first non-zero coefficient, then the others will be made
		///  explicit using this denominator. Suppose that in the equation ax+by+cz+d=0 c is not zero.
		///  Then z = (-a/c)*x + (-b/c)*y + (-d/c)
		///  { {(-a/c)}, {(-b/c)}, {(-d/c)} } this is the "ratios' array", where "c" will be the
		/// first non-zero coefficient.
		/// </summary>
        /// <param name="cartesianPlan">the equation ax+by+cz+d=0</param>
        /// <returns>the result will be stored in the form:
        /// 		   { x:{a*t, b*u, c},
        /// 			 y:{d*t, e*u, f},
        /// 	 		 z:{g*t, h*u, i}   }
        /// </returns>
		static public double[,] parametricPlanFromCartesianPlan( double[] cartesianPlan)
		{
			double[,] parametricPlan = new double[3,3];// the result will be stored in the form:
			//			   { x:{a*t, b*u, c},
			//				 y:{d*t, e*u, f},
			//				 z:{g*t, h*u, i}   }
			// particularly the Monge parametrization will be used, which is a map where a coordinate
			// function dependes monodromically on the other parameters, each of which is explicit.
			// such parametrization assumes the form:
			// x = x(t,u) = t
			// y = y(t,u) = u
			// z = z(t,u) = f(x,y) = f(x(t,u),y(t,u))= f(t,u)
			//
			// nevertheless in the input cartesian plane equation some of the coefficients may be zero.
			// So as first will be located the first non-zero coefficient, then the others will be made
			// explicit using this denominator. Suppose that in the equation ax+by+cz+d=0 c is not zero.
			// Then z = (-a/c)*x + (-b/c)*y + (-d/c)
			// { {(-a/c)}, {(-b/c)}, {(-d/c)} } this is the "ratios' array", where "c" will be the
			// first non-zero coefficient.
			int denominatorPosition = 2;// start from "z" coefficient
			for( ; denominatorPosition>=0; denominatorPosition--)
			{
				if( System.Math.Abs(cartesianPlan[denominatorPosition])>1.0e-14 )// if |coefficient|>0
				{break;}// denominatorPosition will have the correct index( the one of the first non-zero coefficient )
			}
			if( denominatorPosition<0) throw new System.Exception( "piano degenere");
			int cleanCounter=0;// this variable has the role of index in the ratios'array; the denominator's index must
			//	be skipped, but this variable will not feel the effect of it. It's a clean-counter.
			double[] ratiosArray = new double[3];
			for( int dirtyCounter=0; dirtyCounter<cartesianPlan.Length; dirtyCounter++)
			{
				if( dirtyCounter != denominatorPosition)
				{// the ratio must have a sign-change due to equation-member-change
					ratiosArray[cleanCounter++] =  - cartesianPlan[dirtyCounter] / cartesianPlan[denominatorPosition];
				}// cleanCounter is incremented at left-side, after the assignment
				//else skip the denominator
			}
            //
			bool isFirstTime = true;
			for( int solutionRow=0; solutionRow<3; solutionRow++)
			{
				if( solutionRow != denominatorPosition)
				{
					if( isFirstTime)// give parameter "t"==1,0
					{
						parametricPlan[solutionRow,0] = 1.0;// "t"
						parametricPlan[solutionRow,1] = 0.0;
						parametricPlan[solutionRow,2] = 0.0;
						isFirstTime = false;// "t" already assigned; goto "u"
					}
					else// give parameter "u"==0,1
					{
						parametricPlan[solutionRow,0] = 0.0;
						parametricPlan[solutionRow,1] = 1.0;// "u"
						parametricPlan[solutionRow,2] = 0.0;
					}
				}
				else// dependent variable
				{
					parametricPlan[solutionRow,0] = ratiosArray[0];// "t,u,k" coefficients in the dependent coordinate
					parametricPlan[solutionRow,1] = ratiosArray[1];//
					parametricPlan[solutionRow,2] = ratiosArray[2];//
				}
			}
			// done
			return parametricPlan;
		}// end parametricPlanFromCartesianPlan()






        /// <summary>
        /// se tre vettori di R3 sono linearmente dipendenti, sono complanari.
        /// </summary>
        /// <param name="ax">coordinate x of point a</param>
        /// <param name="ay">coordinate y of point a</param>
        /// <param name="az">coordinate z of point a</param>
        /// <param name="bx">coordinate x of point b</param>
        /// <param name="by">coordinate y of point b</param>
        /// <param name="bz">coordinate z of point b</param>
        /// <param name="cx">coordinate x of point c</param>
        /// <param name="cy">coordinate y of point c</param>
        /// <param name="cz">coordinate z of point c</param>
        /// <returns>bool</returns>
        static public bool complanaritaThreeR3Points(
            double ax, double ay, double az,
            double bx, double by, double bz,
            double cx, double cy, double cz
            )
        {
            LinearAlgebra.RealMatrix linearDependenceCheck = new RealMatrix( 3, 3 );
            bool matrixIsBuilt = true;
            // first row == first point
            matrixIsBuilt &= linearDependenceCheck.insert( ax, 0, 0 );
            matrixIsBuilt &= linearDependenceCheck.insert( ay, 0, 1 );
            matrixIsBuilt &= linearDependenceCheck.insert( az, 0, 2 );
            // second row == second point
            matrixIsBuilt &= linearDependenceCheck.insert( bx, 1, 0 );
            matrixIsBuilt &= linearDependenceCheck.insert( by, 1, 1 );
            matrixIsBuilt &= linearDependenceCheck.insert( bz, 1, 2 );
            // third row == third point
            matrixIsBuilt &= linearDependenceCheck.insert( cx, 2, 0 );
            matrixIsBuilt &= linearDependenceCheck.insert( cy, 2, 1 );
            matrixIsBuilt &= linearDependenceCheck.insert( cz, 2, 2 );
            //
            linearDependenceCheck.show();
            //
            double determinante = linearDependenceCheck.det();
            if ( System.Math.Abs( determinante ) < 1.0E-12 )
            {
                return true;// appartengono ad un piano.
            }// else continue.
            return false;// non sono complanari
        }// end complanaritaThreeR3Points()
            




        /// <summary>
        /// equazioni parametriche del piano, per tre vettori linermente indipendenti.
        /// </summary>
        /// <param name="directionCosine_A">coordinates of the vector point a</param>
        /// <param name="directionCosine_B">coordinates of the vector point b</param>
        /// <param name="translation">coordinates of the vector point translation</param>
        /// <returns>the result will be stored in the form:
        ///     {
        /// 	    x(t,u)== t*ax + u*bx + cx
        /// 	    y(t,u)== t*ay + u*by + cy
        /// 	    z(t,u)== t*az + u*bz + cz
        /// 	}
        /// </returns>
        static public double[,] parametricPlaneThroughThreePoints_withoutNormalization(
            double[] directionCosine_A,
            double[] directionCosine_B,
            double[] translation
           )
        {
            // steps:
            // (I)  verify linear independence of the two director-cosine vectors( a,b).
            // (II) set up the resulting equations in the form:
            //     {
            // 	    x(t,u)== t*ax + u*bx + cx
            // 	    y(t,u)== t*ay + u*by + cy
            // 	    z(t,u)== t*az + u*bz + cz
            // 	   }
            //
            // verifica sui minori 2x2
            LinearAlgebra.RealMatrix linearIndependenceCheck = new RealMatrix( 2, 3 );
            bool matrixIsBuilt = true;
            // first row == directionCosine_A
            matrixIsBuilt &= linearIndependenceCheck.insert( directionCosine_A[0], 0, 0 );
            matrixIsBuilt &= linearIndependenceCheck.insert( directionCosine_A[1], 0, 1 );
            matrixIsBuilt &= linearIndependenceCheck.insert( directionCosine_A[2], 0, 2 );
            // second row == directionCosine_B
            matrixIsBuilt &= linearIndependenceCheck.insert( directionCosine_B[0], 1, 0 );
            matrixIsBuilt &= linearIndependenceCheck.insert( directionCosine_B[1], 1, 1 );
            matrixIsBuilt &= linearIndependenceCheck.insert( directionCosine_B[2], 1, 2 );
            // third point is the translation vector; it has no role in independency check.
            linearIndependenceCheck.show();
            //
            //compute on 2x2 minors; the name represents the kept columns; (-1) as delendaRow means do NOT delete any row.
            LinearAlgebra.RealMatrix minorUnoDue = linearIndependenceCheck.reductor( -1, 2 );// means get a minor, keeping all the rows and descarding column[2].
            double detMinorUnoDue = minorUnoDue.det();
            //
            LinearAlgebra.RealMatrix minorUnoTre = linearIndependenceCheck.reductor( -1, 1 );// means get a minor, keeping all the rows and descarding column[1].
            double detMinorUnoTre = minorUnoTre.det();
            //
            LinearAlgebra.RealMatrix minorDueTre = linearIndependenceCheck.reductor( -1, 0 );// means get a minor, keeping all the rows and descarding column[0].
            double detMinorDueTre = minorDueTre.det();
            //
            //double isLinearindependentSet = linearIndependenceCheck.det();
            if (// the three 2x2 minors, have to be all nulls, to have linear dependency.
                   System.Math.Abs( detMinorUnoDue ) < 1.0E-11
                && System.Math.Abs( detMinorUnoTre ) < 1.0E-11
                && System.Math.Abs( detMinorDueTre ) < 1.0E-11 
            )
            {
                throw new System.Exception( "invalid vector set : there is no linear independence." );
            }// else continue.
            //
            double[,] threeParametricPlaneEquations = new double[3, 3];
            //
            threeParametricPlaneEquations[0, 0] = directionCosine_A[0];
            threeParametricPlaneEquations[0, 1] = directionCosine_B[0];
            threeParametricPlaneEquations[0, 2] = translation[0];
            //
            threeParametricPlaneEquations[1, 0] = directionCosine_A[1];
            threeParametricPlaneEquations[1, 1] = directionCosine_B[1];
            threeParametricPlaneEquations[1, 2] = translation[1];
            //
            threeParametricPlaneEquations[2, 0] = directionCosine_A[2];
            threeParametricPlaneEquations[2, 1] = directionCosine_B[2];
            threeParametricPlaneEquations[2, 2] = translation[2];
            //
            //ready.
            return threeParametricPlaneEquations;
        }// end parametricPlaneThroughThreePoints_withoutNormalization()




        /// <summary>
        /// 
        /// </summary>
        /// <param name="directionCosine_A_preNormalization"></param>
        /// <param name="directionCosine_B_preNormalization"></param>
        /// <param name="translation"></param>
        /// <returns></returns>
        static public double[,] parametricPlaneThroughThreePoints_Normalized(
            double[] directionCosine_A_preNormalization,
            double[] directionCosine_B_preNormalization,
            double[] translation
           )
        {
            // steps:
            // (I)   normalization (i.e. let them versors) of the two direction cosine vectors.
            // (II)  verify linear independence of the two director-cosine vectors( a,b).
            // (III) set up the resulting equations in the form:
            //     {
            // 	    x(t,u)== t*ax + u*bx + cx
            // 	    y(t,u)== t*ay + u*by + cy
            // 	    z(t,u)== t*az + u*bz + cz
            // 	   }
            // 	
            //
            double[] directionCosine_A;
            double[] directionCosine_B;
            //
            LinearAlgebra.PointR3 directionCosine_A_vec = new PointR3( directionCosine_A_preNormalization );
            double normaVec_A = directionCosine_A_vec.NormaEuclidea();
            directionCosine_A = directionCosine_A_vec.omothetia( 1.0 / normaVec_A ).getPointCoordinates;
            //
            LinearAlgebra.PointR3 directionCosine_B_vec = new PointR3( directionCosine_B_preNormalization );
            double normaVec_B = directionCosine_B_vec.NormaEuclidea();
            directionCosine_B = directionCosine_B_vec.omothetia( 1.0 / normaVec_B ).getPointCoordinates;
            //
            // verifica sui minori 2x2
            LinearAlgebra.RealMatrix linearIndependenceCheck = new RealMatrix( 2, 3 );
            bool matrixIsBuilt = true;
            // first row == directionCosine_A
            matrixIsBuilt &= linearIndependenceCheck.insert( directionCosine_A[0], 0, 0 );
            matrixIsBuilt &= linearIndependenceCheck.insert( directionCosine_A[1], 0, 1 );
            matrixIsBuilt &= linearIndependenceCheck.insert( directionCosine_A[2], 0, 2 );
            // second row == directionCosine_B
            matrixIsBuilt &= linearIndependenceCheck.insert( directionCosine_B[0], 1, 0 );
            matrixIsBuilt &= linearIndependenceCheck.insert( directionCosine_B[1], 1, 1 );
            matrixIsBuilt &= linearIndependenceCheck.insert( directionCosine_B[2], 1, 2 );
            // third point is the translation vector; it has no role in independency check.
            linearIndependenceCheck.show();
            //
            //compute on 2x2 minors; the name represents the kept columns; (-1) as delendaRow means do NOT delete any row.
            LinearAlgebra.RealMatrix minorUnoDue = linearIndependenceCheck.reductor( -1, 2 );// means get a minor, keeping all the rows and descarding column[2].
            double detMinorUnoDue = minorUnoDue.det();
            //
            LinearAlgebra.RealMatrix minorUnoTre = linearIndependenceCheck.reductor( -1, 1 );// means get a minor, keeping all the rows and descarding column[1].
            double detMinorUnoTre = minorUnoTre.det();
            //
            LinearAlgebra.RealMatrix minorDueTre = linearIndependenceCheck.reductor( -1, 0 );// means get a minor, keeping all the rows and descarding column[0].
            double detMinorDueTre = minorDueTre.det();
            //
            //double isLinearindependentSet = linearIndependenceCheck.det();
            if (// the three 2x2 minors, have to be all nulls, to have linear dependency.
                   System.Math.Abs( detMinorUnoDue ) < 1.0E-11
                && System.Math.Abs( detMinorUnoTre ) < 1.0E-11
                && System.Math.Abs( detMinorDueTre ) < 1.0E-11
            )
            {
                throw new System.Exception( "invalid vector set : there is no linear independence." );
            }// else continue.
            //
            double[,] threeParametricPlaneEquations = new double[3, 3];
            //
            threeParametricPlaneEquations[0, 0] = directionCosine_A[0];
            threeParametricPlaneEquations[0, 1] = directionCosine_B[0];
            threeParametricPlaneEquations[0, 2] = translation[0];
            //
            threeParametricPlaneEquations[1, 0] = directionCosine_A[1];
            threeParametricPlaneEquations[1, 1] = directionCosine_B[1];
            threeParametricPlaneEquations[1, 2] = translation[1];
            //
            threeParametricPlaneEquations[2, 0] = directionCosine_A[2];
            threeParametricPlaneEquations[2, 1] = directionCosine_B[2];
            threeParametricPlaneEquations[2, 2] = translation[2];
            //
            //ready.
            return threeParametricPlaneEquations;
        }// end parametricPlaneThroughThreePoints_Normalized()





       
        
        /// <summary>
        /// equazione cartesiana di un piano per tre punti non allineati; in questo metodo i tre punti sono qualsiasi terna di vettori linearmente indipendenti.
        /// </summary>
        /// <param name="ax">coordinate x of point a</param>
        /// <param name="ay">coordinate y of point a</param>
        /// <param name="az">coordinate z of point a</param>
        /// <param name="bx">coordinate x of point b</param>
        /// <param name="by">coordinate y of point b</param>
        /// <param name="bz">coordinate z of point b</param>
        /// <param name="cx">coordinate x of point c</param>
        /// <param name="cy">coordinate y of point c</param>
        /// <param name="cz">coordinate z of point c</param>
        /// <returns>the return value is a double[4] array, whose elements are the coefficiens of the cartesian plane equation;
        /// (in the order {x,y,z,k} where k is the traslation term )</returns>
        static public double[] cartesianPlaneThroughThreePoints_withoutNormalization(
            double ax, double ay, double az,
			double bx, double by, double bz,
			double cx, double cy, double cz
            )
		{// the return value is a double[4] array, whose elements are the coefficiens of the cartesian plane equation (in the order {x,y,z,k} where k is the traslation term )
			double[] PlaneCartesianEquation = new double[4];// three cosines and one traslation
			RealMatrix calc = new RealMatrix( 3, 4); // calculation matrix has a border of (x,y,z,1) first row, and (1,1,1,1) last column.
			bool matrixIsBuilt = true;
			// matrix [3,4]. Is omitted the first row ({x,y,z,1}, with respect to wich the determinant is developed.
			// Such determinant, imposed to be zero, represents the cartesian plane equation.
			// first row(all columns but last) == first point
			matrixIsBuilt &= calc.insert(  ax, 0, 0);
			matrixIsBuilt &= calc.insert(  ay, 0, 1);
			matrixIsBuilt &= calc.insert(  az, 0, 2);
			// second row(all columns but last) == second point
			matrixIsBuilt &= calc.insert(  bx, 1, 0);
			matrixIsBuilt &= calc.insert(  by, 1, 1);
			matrixIsBuilt &= calc.insert(  bz, 1, 2);
			// third row(all columns but last) == third point
			matrixIsBuilt &= calc.insert(  cx, 2, 0);
			matrixIsBuilt &= calc.insert(  cy, 2, 1);
			matrixIsBuilt &= calc.insert(  cz, 2, 2);
			// fourth column == all 1.0 elements
			matrixIsBuilt &= calc.insert( 1.0, 0, 3);
			matrixIsBuilt &= calc.insert( 1.0, 1, 3);
			matrixIsBuilt &= calc.insert( 1.0, 2, 3);
            //
			// debug
			calc.show();
            //
			if( ! matrixIsBuilt) throw new System.Exception( "matrix has NOT been built");
			// the cartesian equation has coefficient for [x,y,z,k] in this order, where k is the traslation term.
            //
			bool hasAtLeastOneNonNullCoefficient = false;
			// debug
			//calc.reductor(-1, 0).show();
			PlaneCartesianEquation[0] = + calc.reductor(-1, 0).det();
            //
			// debug
			//calc.reductor(-1, 1).show();
			PlaneCartesianEquation[1] = - calc.reductor(-1, 1).det();
            //
			// debug
			//calc.reductor(-1, 2).show();
			PlaneCartesianEquation[2] = + calc.reductor(-1, 2).det();
			for( int c=0; c<3; c++)
			{
				if( System.Math.Abs(PlaneCartesianEquation[c]) > +1.0e-45 )
				{
					hasAtLeastOneNonNullCoefficient = true;
					break;// plane is valid
				}// else continue
			}
			if( ! hasAtLeastOneNonNullCoefficient)
			{
				throw new System.Exception(" piano degenere");
			}// else continue
            //
			// debug
			//calc.reductor(-1, 3).show();
			PlaneCartesianEquation[3] = - calc.reductor(-1, 3).det();
			//
			return PlaneCartesianEquation;
		}// end cartesianPlaneThroughThreePoints()








        /// <summary>
        /// equazione cartesiana di un piano per tre punti non allineati, uno dei quali e' l'origine: quindi un piano vettoriale.
        /// NB. uno dei tre punti deve essere il comune punto di applicazione degli altri due; qualora cosi'
        /// non fosse, deve preoccuparsi chi chiama questa funzione, di rendere tali i parametri, mediante sottrazione. Per convenzione di assume che il punto di applicazione sia il vettore
        /// (cx, cy, cz). Pertanto in uno spazio vettoriale si possono passare i vettori "a" e "b", intesi applicati nell'origine, ed il vettore nullo come "c". Qualora invece "c" fosse
        /// nonnullo, si dovra' passare (a-c),(b-c),(c-c)==(a-c),(b-c),(0).
        /// </summary>
        /// <returns>the return value is a double[4] array, whose elements are the coefficiens of the cartesian plane equation;
        /// (in the order {x,y,z,k} where k is the traslation term )</returns>
        static public double[] cartesianPlaneThroughThreePoints_Normalized(
            LinearAlgebra.PointR3 Point_A,
            LinearAlgebra.PointR3 Point_B
          )
        {// the return value is a double[4] array, whose elements are the coefficiens of the cartesian plane equation (in the order {x,y,z,k} where k is the traslation term )
            // steps:
            // (I)   normalization (i.e. let them versors) of the two direction cosine vectors.
            // (II)  verify linear independence of the two director-cosine vectors( a,b).
            // (III) find the normal vector, in the form:  a^b
            // (IV)  normalize it, in the form (a^b)/||a^b||
            // (V)   set up the cartesian equation, with normalized Normal vector: ((a^b)/||a^b||).(x,y,z)==0
            //
            //double[] directionCosine_A = new double[3] { ax, ay, az };
            //double[] directionCosine_B = new double[3] { bx, by, bz };
            //
            //LinearAlgebra.PointR3 directionCosine_A_vec = new PointR3( ax, ay, az );
            //double normaVec_A = directionCosine_A_vec.NormaEuclidea();
            //directionCosine_A = directionCosine_A_vec.omothetia( 1.0 / normaVec_A ).getPointCoordinates;
            ////
            //LinearAlgebra.PointR3 directionCosine_B_vec = new PointR3( bx, by, bz);
            //double normaVec_B = directionCosine_B_vec.NormaEuclidea();
            //directionCosine_B = directionCosine_B_vec.omothetia( 1.0 / normaVec_B ).getPointCoordinates;
            //
            // verifica sui minori 2x2
            LinearAlgebra.RealMatrix linearIndependenceCheck = new RealMatrix( 2, 3 );
            bool matrixIsBuilt = true;
            // first row == directionCosine_A
            matrixIsBuilt &= linearIndependenceCheck.insert( Point_A.getX, 0, 0 );
            matrixIsBuilt &= linearIndependenceCheck.insert( Point_A.getY, 0, 1 );
            matrixIsBuilt &= linearIndependenceCheck.insert( Point_A.getZ, 0, 2 );
            // second row == directionCosine_B
            matrixIsBuilt &= linearIndependenceCheck.insert( Point_B.getX, 1, 0 );
            matrixIsBuilt &= linearIndependenceCheck.insert( Point_B.getY, 1, 1 );
            matrixIsBuilt &= linearIndependenceCheck.insert( Point_B.getZ, 1, 2 );
            // third point is the translation vector; it has no role in independency check.
            linearIndependenceCheck.show();
            //
            //compute on 2x2 minors; the name represents the kept columns; (-1) as delendaRow means do NOT delete any row.
            LinearAlgebra.RealMatrix minorUnoDue = linearIndependenceCheck.reductor( -1, 2 );// means get a minor, keeping all the rows and descarding column[2].
            double detMinorUnoDue = minorUnoDue.det();
            //
            LinearAlgebra.RealMatrix minorUnoTre = linearIndependenceCheck.reductor( -1, 1 );// means get a minor, keeping all the rows and descarding column[1].
            double detMinorUnoTre = minorUnoTre.det();
            //
            LinearAlgebra.RealMatrix minorDueTre = linearIndependenceCheck.reductor( -1, 0 );// means get a minor, keeping all the rows and descarding column[0].
            double detMinorDueTre = minorDueTre.det();
            //
            //double isLinearindependentSet = linearIndependenceCheck.det();
            if (// the three 2x2 minors, have to be all nulls, to have linear dependency.
                   System.Math.Abs( detMinorUnoDue ) < 1.0E-11
                && System.Math.Abs( detMinorUnoTre ) < 1.0E-11
                && System.Math.Abs( detMinorDueTre ) < 1.0E-11
            )
            {
                throw new System.Exception( "invalid vector set : there is no linear independence." );
            }// else continue.
            //
            //double[] cartesianPlanNormalized = new double[4];
            //LinearAlgebra.PointR3 directionCosine_A_Normalized = new PointR3( directionCosine_A );
            //LinearAlgebra.PointR3 directionCosine_B_Normalized = new PointR3( directionCosine_B );
            LinearAlgebra.PointR3 cartesianCoefficients = Point_A.vectorProduct( Point_B );
            //directionCosine_A_Normalized.vectorProduct( directionCosine_B_Normalized );
            double[] cartesianPlanNormalized = (cartesianCoefficients.omothetia( 1.0 / cartesianCoefficients.NormaEuclidea() )).getPointCoordinates;
            // translation term (i.e. [4]) is always zero, since it's a vector plane.
            return cartesianPlanNormalized;
        }// end cartesianPlaneThroughThreePoints_Normalized()





		// TODO     (implementare sia il metodo per l'equazione cartesiana
		//		che quello per le equazioni parametriche e i metodi di conversione dall'una alle altre e viceversa)








		/// <summary>
		/// this method calculates the R3 coordinates of a point, belonging to a plane whose parametric equations
		/// are known. Also known are the two coordinates in the R2 space of the parameters (the (t,u) parameters
		/// that describe the plane in the parametrization {{x=x(t,u)},{y=y(t,u)},{z=z(t,u)}}
		/// </summary>
		/// <param name="planParametricEquations">the parametrization {{x=x(t,u)},{y=y(t,u)},{z=z(t,u)}}</param>
		/// <param name="parametersR2Point">the two coordinates in the R2 space of the parameters: the (t,u) parameters</param>
		/// <returns>the three coordinates in the R3 space of the plane-immersion: the {x,y,z} coordinates, where {{x=x(t,u)},{y=y(t,u)},{z=z(t,u)}}</returns>
		static public double[] PointImageInR3_ofParametricPlan_fromParametersR2Point(
			double[,] planParametricEquations,// equazioni parametriche del piano immerso in R3
			double[]  parametersR2Point )// coordinate nello spazio R2 dei parametri
		{
			double[] pointInR3 = new double[3];
			// calculate the R3 image, through parametrization and parameters' value
			// x coordinate
			pointInR3[0] = planParametricEquations[0,0]*parametersR2Point[0] + planParametricEquations[0,1]*parametersR2Point[1] + planParametricEquations[0,2];
			// y coordinate
			pointInR3[1] = planParametricEquations[1,0]*parametersR2Point[0] + planParametricEquations[1,1]*parametersR2Point[1] + planParametricEquations[1,2];
			// z coordinate
			pointInR3[2] = planParametricEquations[2,0]*parametersR2Point[0] + planParametricEquations[2,1]*parametersR2Point[1] + planParametricEquations[2,2];
			// all point coordinates are ready
			return pointInR3;
		}// end PointImageInR3_ofParametricPlan_fromParametersR2Point









		/// <summary>
		/// this method considers the case of a line, neither belonging to a plane nor in parallel with it.
		/// such line has one and only one intersection point with the plane. This is obtained by means of 
		/// a non-omogeneous( in general. Some times it could be but in this case the solution is trivially
		/// the null-vector) liner system 3x3. Ax=b, where A is the 3x3 matrix of the cartesian coefficients.
		/// These are the three coordinates coefficients in the three input-cartesian-equations.
		/// So the system is composed of the director-cosines of the considered plane and of the 
		/// two planes which individuate the line. Note that this system has the same solution if the 
		/// equations are changed of order, since it changes also the result vector. So Ax=b is equivalent
		/// to (A~)x=(b~). Anyway A^(-1) and (A~)^(-1) will bw unique and multiplied (respectively) by
		/// b and (b~) will give the same x vector.
		/// Note that this method is intended for the case of incident line and plane only. If the input data
		/// consist of a line contained in the input plane or in parallel with it, the method will throw the
		/// appropriate exception.
		/// In the former case the exception will say that the system has one-infinity of solutions, precisely
		/// the aleph-one points hold by the line itself, which, being contained by the plane, solves the system
		/// with all herself.
		/// In the latter case the system presents no solution.
		/// </summary>
		/// <param name="theBoardPlane">the cartesian plane to be intersected with the line</param>
		/// <param name="theFirstLineGeneratingPlane">each of the two cartesian planes belonging to the proper bundle which individuates the line</param>
		/// <param name="theSecondLineGeneratingPlane">each of the two cartesian planes belonging to the proper bundle which individuates the line</param>
		/// <returns>the coordinates of the intersection point</returns>
		static public double[] intersectionPointBetweenCartesianLineAndCartesianPlane(
					double[] theBoardPlane,
					double[] theFirstLineGeneratingPlane,
					double[] theSecondLineGeneratingPlane   )
		{
			double[] theIntersectionPoint = new double[3];
			// notation:
			//			- A.x = b     the system to be solved to find the x-vector components
			//			- A ==  the matrix(3x3) from the three cartesian equations
			//			- (A^(-1)) ==  the inverse matrix of A
			//			- x ==  the unknown vector
			//			- b ==  the known-terms vector
			// steps:
			//			- create the matrix(3x3) A from the three cartesian equations (using the variables' coefficients)
			//			- create the vector(3) b from the three cartesian equations (using the traslation constants)
			//			- invert the A-matrix and obtain (A^(-1))
			//			- obtain vector(3) x by means of the scalar product (A^(-1)).b == x
			// implementation:
			//			- create the matrix(3x3) A from the three cartesian equations (using the variables' coefficients)
			double[,] theMatrixPrototype = new double[3,3];
			// first line
			theMatrixPrototype[0,0] = theBoardPlane[0];
			theMatrixPrototype[0,1] = theBoardPlane[1];
			theMatrixPrototype[0,2] = theBoardPlane[2];
			// second line
			theMatrixPrototype[1,0] = theFirstLineGeneratingPlane[0];
			theMatrixPrototype[1,1] = theFirstLineGeneratingPlane[1];
			theMatrixPrototype[1,2] = theFirstLineGeneratingPlane[2];
			// third line
			theMatrixPrototype[2,0] = theSecondLineGeneratingPlane[0];
			theMatrixPrototype[2,1] = theSecondLineGeneratingPlane[1];
			theMatrixPrototype[2,2] = theSecondLineGeneratingPlane[2];
			// now build the matrix A(3x3)
			LinearAlgebra.RealMatrix A = new RealMatrix( theMatrixPrototype, 3,3 );
			//			- create the vector(3) b from the three cartesian equations (using the traslation constants)
			double[,] b = new double[3,1];
			// NB the traslation-terms MUST be changed of sign. They go in the right member.
			b[0,0] = - theBoardPlane[3];// the traslation term in the first plane
			b[1,0] = - theFirstLineGeneratingPlane[3];// the traslation term in the second plane
			b[2,0] = - theSecondLineGeneratingPlane[3];// the traslation term in the third plane
			//			- invert the A-matrix and obtain (A^(-1))
			LinearAlgebra.RealMatrix theProduct = null;// this matrix is the (3,1) solution vector
			try
			{
				LinearAlgebra.RealMatrix A_inverse = A.inverse();
				//			- obtain vector(3) x by means of the scalar product (A^(-1)).b == x
				LinearAlgebra.RealMatrix secondFactor = new LinearAlgebra.RealMatrix( b, 3,1);
				theProduct = A_inverse.operator_mul( secondFactor);
			}
			catch( System.Exception ex)
			{
				//LogService.LogWrappers.SectionContent( "the intersection does not exist or is not unique: respectively the line is parallel to the plane or belongs to it."+ ex.Message, 2);
				throw new System.Exception("the intersection does not exist or is not unique: respectively the line is parallel to the plane or belongs to it."+ ex.Message);
			}
			// fill the result-variable
			theIntersectionPoint[0] = theProduct.get_element(0,0);
			theIntersectionPoint[1] = theProduct.get_element(1,0);
			theIntersectionPoint[2] = theProduct.get_element(2,0);
			// ready
			return theIntersectionPoint;
		}// end intersectionPointBetweenLineAndPlane





		/// <summary>
		/// Composes a linear system devoted to find the intersection between plane and line.
		/// The linear system will be composed with the cartesian equation of the plane and 
		/// with the parametrization of the line.
		/// The expressions that the three coordinate functions assume in the line parametrization
		/// will be substituted in the cartesian equation of the plane.
		/// A value of the independent parameter will be found. Substituting such value in the
		/// parametric equations of the line, the three coordinate functions will assume the 
		/// values of the intersection-point coordinates.
		/// </summary>
		/// <param name="cartesianPlane">the three coordinate-coefficients and the traslation term</param>
		/// <param name="parametricLine">the three direction-cosines and the three traslation terms</param>
		/// <returns>the three cartesian coordinates of the intersection between plane and line</returns>
		static public double[] intersectionBetweenCartesianPlaneAndParametricLine(
			double[]   cartesianPlane,
			double[,]  parametricLine   )
		{
			//
			//--------------------------------------------------------------------------------------------------
			//------------------------------- Documentazione sulla logica del metodo ---------------------------
			//--------------------------------------------------------------------------------------------------
			// presupposti teorici
			//
			// equazione cartesiana del piano {a,b,c,d}
			// che rappresenta a*x+b*y+c*z+d=0
			// equazione parametrica della retta {{A,B},{C,D},{E,F}}
			// che rappresenta
			// x=x(t) = A*t+B
			// y=y(t) = C*t+D
			// z=z(t) = E*t+F
			// sostituisco dalla II nella I e trovo un valore della 't'
			// a*(A*t+B)+b*(C*t+D)+c*(E*t+F)+d=0
			// donde
			// (a*A+b*C+c*E)*t + (a*B+b*D+c*F+d) = 0
			// quindi
			// t = -(a*B+b*D+c*F+d)/(a*A+b*C+c*E)
			// lo sostituisco nella II e trovo una terna {x,y,z}. done.
			//
			// implementazione
			// ( a*A + b*C + c*E )
			double denominator =	cartesianPlane[0]*parametricLine[0,0]+ // a*A
									cartesianPlane[1]*parametricLine[1,0]+ // b*C
									cartesianPlane[2]*parametricLine[2,0]; // c*E
			if( System.Math.Abs( denominator)<+1.0e-80)
			{
				throw new System.Exception( "la soluzione non e' unica: il piano e la retta possono essere paralleli(nessuna soluzione) o la retta contenuta nel piano( infinite soluzioni).");
			}
			// -( a*B + b*D + c*F + d )
			double numerator = -( cartesianPlane[0]*parametricLine[0,1] + // a*B
								  cartesianPlane[1]*parametricLine[1,1] + // b*D
								  cartesianPlane[2]*parametricLine[2,1] + // c*F
								  cartesianPlane[3]			 		);	  // d
			double parameterSolutionValue = numerator / denominator;
			double[] intersectionCartesianCoordinates = new double[3];
			intersectionCartesianCoordinates[0] = parametricLine[0,0]*parameterSolutionValue+parametricLine[0,1];
			intersectionCartesianCoordinates[1] = parametricLine[1,0]*parameterSolutionValue+parametricLine[1,1];
			intersectionCartesianCoordinates[2] = parametricLine[2,0]*parameterSolutionValue+parametricLine[2,1];
			return intersectionCartesianCoordinates;
		}// end method intersectionBetweenCartesianPlaneAndParametricLine



		/// <summary>
		/// the algorithm consists of preserving the directive-cosines and applying an affinity,
		/// to let a line in parallel to the input one, but passing through the desired point.
		/// If the input line has equation:
		/// {{a*t, x0},
		///  {b*t, y0},
		///  {c*t, z0} }
		///  and the desired point has coordinates {xw,yw,zw}
		///  then the resulting line will have parametric equations:
		/// {{a*t, xw},
		///  {b*t, yw},
		///  {c*t, zw} }
		/// </summary>
		/// <param name="originalParametricLine">the desired orientation</param>
		/// <param name="pointToPassThrough">the point to pass through</param>
		/// <returns>the parametric equations of the line, having the same orientation of
		/// the originalParametricLine, but passing through the pointToPassThrough</returns>
		static public double[,] parametricLineSubjectToAnAffinity( 
										double[,] originalParametricLine,
										double[] pointToPassThrough         )
		{// the result will be stored in the format:
			// {{a*t, x0},
			//  {b*t, y0},
			//  {c*t, z0} }
			// so the first column are the independent-parameter-coefficients,
			// while the second column are the traslation terms.
			double[,] parametricLineCoefficients = new double[3,2];
			// preserve the director-cosines, to preserve the original orientation
			parametricLineCoefficients[0,0] = originalParametricLine[0,0];
			parametricLineCoefficients[1,0] = originalParametricLine[1,0];
			parametricLineCoefficients[2,0] = originalParametricLine[2,0];
			// let the line pass through the required point
			parametricLineCoefficients[0,1] = pointToPassThrough[0];
			parametricLineCoefficients[1,1] = pointToPassThrough[1];
			parametricLineCoefficients[2,1] = pointToPassThrough[2];
			// ready
			return parametricLineCoefficients;
		}// end parametricLineSubjectToAnAffinity



		//  TODO: assonometrie, rette sghembe, etc.



	}// end class Geometry_inR3
}// end namespace LinearAlgebra
