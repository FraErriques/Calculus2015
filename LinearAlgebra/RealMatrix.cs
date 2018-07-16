//# define debug

namespace LinearAlgebra
{

    /// <summary>
    /// a Matrix of real numbers (double type)
    /// manages both square and rectangular matrices
    /// determinant
    /// inversion
    /// main feature is : Jordan Gauss elimination.
    /// </summary>
	public class RealMatrix
    {

        #region Data

        /// <summary>
        /// lines cardinality.
        /// </summary>
		protected readonly int rows, cols;
        /// <summary>
        /// the Tensor.
        /// </summary>
		protected double[,] m;

        /// <summary>
        /// readonly row cardinality
        /// </summary>
        public int Get_rows
        {
            get
            {
                return this.rows;
            }
        }

        /// <summary>
        /// readonly column cardinality
        /// </summary>
        public int Get_cols
        {
            get
            {
                return this.cols;
            }
        }

        #endregion Data



        #region Ctors
        //   Constructors and Destructors   BEGIN

        /// <summary>
        /// Ctor : only specify rowsXcols, and get a zeros-filled matrix.
        /// </summary>
        /// <param name="Prows"></param>
        /// <param name="Pcols"></param>
		public RealMatrix   ( int Prows, int Pcols )
		{
			this.rows = Prows;
			this.cols = Pcols;
			m = allocate(); // prepare memory locations
			int  row, col;
			for (row=0; row<rows; row++)  // fill with zeroes
			{
                for ( col = 0; col < cols; col++ )
                {
                    m[row, col] = 0.0;
                }// end for col
			}// end for row
		}// end Ctor.



        /// <summary>
        /// Ctor : deserialize from a stream.
        /// </summary>
        /// <param name="Prows"></param>
        /// <param name="Pcols"></param>
        /// <param name="where"></param>
		public RealMatrix   (int Prows, int Pcols, string where )
		{
			this.rows = Prows;
			this.cols = Pcols;
			m = allocate(); // prepare
            //
            if ( ! validate( Prows, Pcols, where ) ) // validate the data file on suggested dimensions (rows, cols)
            {
                throw new System.Exception( "suggested dimensions (rows, cols) are inconsistent " +
                                            "with the matrix contained in the data file, or datafile corrupted." );
            }// else ok, can Construct:
            //
            string validatedStream_fullPath = this.validatedStremFullPathProducer( where );
            bool fileWasReadable = this.readfileByTokens( Prows, Pcols, validatedStream_fullPath );
            if ( ! fileWasReadable )
            {
                throw new System.Exception( "Stream NOT readable ! " );
            }// else ok.
		}// end Ctor with deserialization from stream.



        /// <summary>
        /// get the memory-address of a variable already containing an appropriate double[,].
        /// </summary>
        /// <param name="prototype">the appropriate double[,]</param>
        /// <param name="Prows">rows</param>
        /// <param name="Pcols">columns</param>
		public RealMatrix( double[,] prototype, int Prows, int Pcols)
		{
			if( prototype.Length != Prows*Pcols)
			{
				throw new System.Exception( "invalid matrix passed");
			}
			this.rows = Prows;
			this.cols = Pcols;
			this.m = prototype;
		}


		
		
        /// <summary>
        /// create in RAM and, if desired, dump on a file. Tou can choose to randomize.
        /// </summary>
        /// <param name="Prows"></param>
        /// <param name="Pcols"></param>
        /// <param name="randomized"></param>
        /// <param name="where"></param>
        public RealMatrix(  int Prows, int Pcols,
			bool randomized,
			string where			 ) // NULL==where means "no dump desired"
		{
			this.rows = Prows;// init dimensions
			this.cols = Pcols;
			m = allocate();  // prepare the memory locations
			if (randomized)
			{
				generator (); // fill in RAM with random numbers
			}
			else
			{
				input();      // fill in RAM getting each entry from stdin
			}
			if ( null != where ) // means "no dump desired"
			{
				writefile (where); // dump on a file
				Wolfram_writefile( where+"_Wolfram.txt" ); // dump on a file in Mathematica format
			}
			else
			{
				return;
			}
		}


		 

        /// <summary>
        /// Copy Constructor
        /// </summary>
        /// <param name="original"></param>
		public RealMatrix ( RealMatrix original)
		{
			this.rows = original.rows;// init dimensions
			this.cols = original.cols;
			m = allocate();  // prepare the memory locations
			int  row, col;
			for (row=0; row<rows; row++)  //  input  the matrix, copying the original
			{
				for (col=0; col<cols; col++)
				{ m[row,col] = original.m[row,col]; }
			}
		}// end Copy Constructor



		
        /// <summary>
        /// Copy Constructor, with row exchange. Determinant preserved via a sign exchange.
        /// 
        ///      ooooooooo                  ooooooooo
        ///      ooooooooo                  ooooooooo
        ///      o from  o                  o  -to  o
        ///      ooooooooo ----------->     ooooooooo
        ///      o  to   o                  o from  o
        ///      ooooooooo                  ooooooooo
        /// 
        /// </summary>
        /// <param name="original">the original matrix, before the row exchange</param>
        /// <param name="from">the position of the row to be moved</param>
        /// <param name="to">the position of the destination where to move the row</param>
		public RealMatrix ( RealMatrix original, int from, int to )
		{
			if( to>=original.rows || from>=original.rows)
			{
				throw new System.Exception( "illegal row index in exchange request");
			}
			this.rows = original.rows;// init dimensions
			this.cols = original.cols;
			m = allocate();  // prepare the memory locations
			int  row, col;
			for (row=0; row<rows; row++)  //  input  the matrix, copying the original
			{
				if( row!=from && row!=to)
				{
					for (col=0; col<cols; col++)
					{
						m[row,col] = original.m[row,col];
					}
				}
			}
			for (col=0; col<cols; col++)
			{
				m[to,col] = original.m[from,col];
			}
			for (col=0; col<cols; col++)
			{
				m[from,col] = - original.m[to,col];
			}// each row  change needs a sign change, to preserve the determinant
		}// end Copy Constructor, with row exchange



		


        /// <summary>
        /// Copy Constructor, with column exchange. Determinant preserved via a sign exchange.
        /// 
        ///         ooooooooooo                 ooooooooooo
        ///         ooooooooooo                 ooooooooooo
        ///         o   ooo  oo                 o   ooo   o
        ///         o f ooo   o                 o - ooo f o
        ///         o r ooo t o                 o t ooo r o
        ///         o o ooo o o ----------->    o o ooo o o
        ///         o m ooo   o                 o   ooo m o
        ///         o   ooo   o                 o   ooo   o
        ///         ooooooooooo                 ooooooooooo
        ///         ooooooooooo                 ooooooooooo
        /// 
        /// </summary>
        /// <param name="original">the original matrix, before the column exchange</param>
        /// <param name="from">the position of the column to be moved</param>
        /// <param name="to">the position of the destination where to move the column</param>
        /// <param name="isOnColumn"> this is a flag-parameter, which is neither evaluated. Its mere presence indicates that the exchange indexes indicate columns.</param>
		public RealMatrix ( RealMatrix original, int from, int to, bool isOnColumn )
		{
			if( to>=original.cols || from>=original.cols)
			{
				throw new System.Exception( "illegal column index in exchange request");
			}
			this.rows = original.rows;// init dimensions
			this.cols = original.cols;
			m = allocate();  // prepare the memory locations
			int  row, col;
			for (col=0; col<cols; col++)  //  input  the matrix, copying the original
			{
				if( col!=from && col!=to)
				{
					for (row=0; row<rows; row++)
					{
						m[row,col] = original.m[row,col];
					}
				}
			}
			for (row=0; row<rows; row++)
			{
				m[row,to] = original.m[row,from];
			}
			for (row=0; row<rows; row++)
			{
				m[row,from] = - original.m[row,to];
			}// each column change needs a sign change, to preserve the determinant
		}// end Copy Constructor, with column exchange



		/// <summary>
		/// Destructor.
		/// </summary>
		~RealMatrix( )
		{
			deallocate();
		}
		// Constructors and Destructors   END




		// START memory management routines

		// called by the Constructors
		private double[,] allocate( )
		{
			m = new double[rows,cols];
			//  #################################################### cosi' e' in C++
			//   m = new double*[rows];
			//   if ( NULL == m ) {return NULL;}
			//   for (size_t  j=0; j<rows; j++)
			//   {
			//      m[j] = new double[cols];
			//      if ( NULL == m[j] ) {return NULL;}
			//   }
			return m;
		}

		// called by the Destructor


        /// <summary>
        /// called by the Destructor: it frees the memory of the underlying matrix.
        /// </summary>
		protected void deallocate( )
		{
			m = null;
			//  ####################################################  cosi' e' in C++
			//   size_t  row;
			//   if ( NULL == m ) return;
			//   for (row=0; row<rows; row++)  //  free  the matrix
			//      { delete[] m[row]; }
			//   delete[] m;
		}

        /// <summary>
        /// a public Proxy for deallocate()
        /// </summary>
        public void deallocator_publicProxy()
        {
            this.deallocate();
        }
		
        // END memory management routines


#endregion Ctors




        #region Algorithms



        /// <summary>
        /// tringularize and then det(tringularized), delivering the triangular matrix, in a copy-variable,
        /// </summary>
        /// <param name="copy"></param>
        /// <returns></returns>
		public double det( out RealMatrix copy )
		{
			if (rows != cols)
			{// determinant only on square matrices
				throw new System.Exception("determinant only on square matrices");
			}
			int n = this.rows;// ==cols
            if ( n == 0 )
            {
                throw new System.Exception( "\nzero-size matrix is NO NUMBER\n" );
            }
			copy = this.Gauss_Jordan_elimination();// the copy will be triangular
			// calculate det on the triangular matrix
			int  index;
			double det = 1.0;  // init to product invariant (both for real and complex cases)!
			for (index=0; index<n; index++)
			{
				det *= copy.m[index,index];// det on a triangular matrix
			}
            //
			return det;
		}// end public double det( out RealMatrix copy )



        /// <summary>
        /// tringularize and then det(tringularized), delivering only the scalar determinant.
        /// </summary>
        /// <returns></returns>
		public double det( )
		{
			if (rows != cols)
			{// determinant only on square matrices
				throw new System.Exception("determinant only on square matrices");
			}
			int n = rows; // =cols
            if ( n == 0 )
            {
                throw new System.Exception( "\nzero-size matrix is NO NUMBER\n" );
            }
            RealMatrix copy = this.Gauss_Jordan_elimination();// the copy will be triangular, but will not be delivered.
			# if debug
				copy.show();
			#endif
			// calculate det on the triangular matrix
			int  index;
			double det = 1.0;  // init to product invariant (both for real and complex cases)!
			for (index=0; index<n; index++)
			{ 
				det *= copy.m[index,index];// det on a triangular matrix
			}
            //
			return det;
		}// end public double det( )



        /// <summary>
        /// this call performs also the sign exchange: on line-exchange a sign exchange, on the elements of the moved line, is needed to preserve the determinant.
        /// </summary>
        /// <param name="diagonalCoordinates"></param>
        /// <param name="copyMatrix"></param>
        /// <returns></returns>
		private bool lineExchange( int diagonalCoordinates, ref RealMatrix copyMatrix )
		{
			bool substitutionFound = false;// return value
			// by row
			for( int candidateSubstitute=diagonalCoordinates+1;
				 candidateSubstitute<this.rows;
				 candidateSubstitute++	)
			{
				if( System.Math.Abs( copyMatrix.m[candidateSubstitute, diagonalCoordinates])>1.0e-80 )
				{// this call performs also the sign exchange
					copyMatrix = new RealMatrix( copyMatrix, candidateSubstitute, diagonalCoordinates );
					substitutionFound = true;
					break;// substitution found
				}
			}
			# if debug
				copyMatrix.show();
			#endif
			//
			// by column
			if( substitutionFound == false)
			{
				for( int candidateSubstitute=diagonalCoordinates+1;
					 candidateSubstitute<this.cols;
					 candidateSubstitute++)
				{
					if( System.Math.Abs( copyMatrix.m[diagonalCoordinates, candidateSubstitute])>1.0e-80 )
					{// this call performs also the sign exchange
						copyMatrix = new RealMatrix( copyMatrix, candidateSubstitute, diagonalCoordinates, true );// true means "the exchange is performed on columns"
						substitutionFound = true;
						break;// substitution found
					}
				}
			}// else substitution already found by row
			# if debug
				copyMatrix.show();
			#endif
			//
			return substitutionFound;
		}// end method lineExchange




        /// <summary>
        /// the main linear algebra technique.
        /// </summary>
        /// <returns></returns>
		public RealMatrix Gauss_Jordan_elimination( )
		{
			int  TransformedRow, OriginalRow, column;// OriginalRow is a second row-index, required for the algorithm
			double coefficient = 0.0;// coefficient is the normalization-coefficient
			RealMatrix copyMatrix = new RealMatrix( this); // Copy Constructor call
            //
			//The algorithm performs a linear combination of each "TransformedRow" with "OriginalRows".
			//Each row, except the uppermost, will be a TransformedRow.
			//The TransformedRow at position "n" will be combined with all the rows above her.
			//Each OriginalRow above the current TransformedRow will be combined with its own coefficient.
			//So the coefficient is racalculated each time the OriginalRow changes.
			//Only the TransformedRow is written in the matrix. OriginalRow stays the same, until she becomes
			//a TransformedRow herself.
			//The transformation loop begins from the top of the matrix; the first row (the uppermost) is skipped;
			//the second one is combined with the first one. The third is combined with the first and then with
			//the second. And so on, until the last one.
			//Each transformation step involves:
			//	-	recalculation of the coefficient
			//	-	addition of the OriginalRow( multiplied by coefficient) to the TransformedRow, column by column
			//The return-value copyMatrix will contain the diagonalized matrix, after the complete
			//transformation loop.
            //
			int minorDimension = (this.rows<=this.cols) ? this.rows : this.cols;// this is necessary to let the algorithm suitable also for non-square matrices
			// preliminar adaption, to let the diagonal elements non-zero: by row
			for( int analizedRow=0; analizedRow<minorDimension; analizedRow++)
			{
				if( System.Math.Abs( copyMatrix.m[analizedRow, analizedRow])<=1.0e-80 )
				{
					bool repaired = this.lineExchange( analizedRow, ref copyMatrix);
				}// end if trovato elemento diagonale da sostituire
			}// end for ricerca elementi diagonali critici per riga
			# if debug
				copyMatrix.show();
			#endif
			// preliminar adaption, to let the diagonal elements non-zero: by column
			for( int analizedCol=0; analizedCol<minorDimension; analizedCol++)
			{
				if( System.Math.Abs( copyMatrix.m[analizedCol, analizedCol])<=1.0e-80 )
				{
					bool repaired = this.lineExchange( analizedCol, ref copyMatrix);
				}// end if trovato elemento diagonale da sostituire
			}// end for ricerca elementi diagonali critici per colonna
			# if debug
				copyMatrix.show();
			#endif
            //
			// ######################## Kordell begin ##################################
			for ( TransformedRow=1; TransformedRow<this.rows; TransformedRow++) // skip first row and count from the second until the last one
			{
				for (OriginalRow=0; OriginalRow<TransformedRow; OriginalRow++) // for each row above the "TransformedRow" one
				{// the n^ "TransformedRow" must be re-normalized n-1 times
					if( OriginalRow<minorDimension)// check
					{// on rectangular matrices this would go out of bounds, without this check
						if( System.Math.Abs(copyMatrix.m[OriginalRow,OriginalRow])>1.0e-80)
                        {// each time a row is linearly transformed, it's with a new coefficient
							coefficient = - copyMatrix.m[TransformedRow,OriginalRow] / copyMatrix.m[OriginalRow,OriginalRow];
						}
                        else// adjust to let possible this transformation, whose coefficient was a division by zero
						{
							bool repaired = this.lineExchange( OriginalRow, ref copyMatrix);
							continue;
						}// end adjust to let possible this transformation, whose coefficient was a division by zero
					}// else skip: we're on a rectangular, over the last step.
					for (column=0; column<this.cols; column++)// for each column
					{// add to current "TransformedRow" the linear transormation of current "OriginalRow"
						copyMatrix.m[TransformedRow,column] += coefficient * copyMatrix.m[OriginalRow,column];
					}// after n-1 linear combinations the n^ "TransformedRow" is explicit
				}// after n-1 transformations with rows above("OriginalRow"), the TransformedRow is ready
                # if debug
                    copyMatrix.show();// if_#debug show intermediate matrix, just after the completion of a single row transformation. On a nxn we'll see n intermediates.
                #endif
			}// all the rows, except the uppermost one, must be transformed
			// ######################## Kordell end   ##################################
            //
			# if debug
				copyMatrix.show();
			#endif
			// ready
			return copyMatrix;
		}// end Jordann-Gauss elimination method




 
        /// <summary>
        /// transpose "this".
        /// </summary>
        /// <returns>a copy, containing Transopose(this).</returns>
        public RealMatrix transpose( )
        {
            RealMatrix copy = new RealMatrix( this.cols, this.rows ); // create [cols,rows] to transpose [row,cols]
            int  row, col;
            for ( row = 0; row < this.rows; row++ )
            {
                for ( col = 0; col < this.cols; col++ )
                {
                    copy.m[col, row] = this.m[row, col];
                }
            }
            return copy;
        }// end transpose()


		

        /// <summary>
        /// the scalar product between vectors (isomorphic obviously)
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
		public static double scalar( double[] first, double[] second )
		{
			if( first.Length != second.Length)
			{
				throw new System.Exception("Only isomorphic vectors can combine by scalar product. It's an internal product."); 
			}
			double res = 0.0;
			int index;
            for ( index = 0; index < first.Length; index++ )
            {
                res += first[index] * second[index];
            }
			return res;
		}// end scalar product.





        /// <summary>
        /// due-norma; la norma indotta dal prodotto scalare negli spazi euclidei.
        /// e' la norma classica negli spazi di Hilbert.
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
		public static double modulo( double[] v)
		{
			double dueNorma = 0.0;
			for( int c=0; c<v.Length; c++)
			{
				dueNorma += v[c]*v[c];
			}
			return System.Math.Sqrt( dueNorma);
		}// end modulo




        /// <summary>
        /// prodotto righe per colonne.
        /// </summary>
        /// <param name="second"></param>
        /// <returns></returns>
        public RealMatrix operator_mul( RealMatrix second )
        {
            if ( cols != second.rows )
            {
                throw new System.Exception( "incompatible indexes for product" );
            }// check tensor product adequacy.
            //RealMatrix  trasp_second = transpose ( second );(*) metodi equivalenti
            RealMatrix  res = new RealMatrix( rows, second.cols );
            int rA, rB;
            for ( rA = 0; rA < rows; rA++ )
            {
                for ( rB = 0; rB < second.cols; rB++ )
                //{ res.m[rA,rB] = scalar ( this.getRow( rA), trasp_second.getRow( rB) ); }(*) metodi equivalenti
                {
                    res.m[rA, rB] = scalar( this.getRow( rA ), second.getCol( rB ) );
                }// end for columns.
            }// end for rows.
            return res;
        }/// end prodotto righe per colonne.

			
        
		RealMatrix   operator_add ( RealMatrix second )
		{
			if (rows!=second.rows || cols!=second.cols){throw new System.Exception("incompatible indexes for sum");}
			RealMatrix res = new RealMatrix( rows, cols);
			int rA, rB;
			for ( rA=0; rA<rows; rA++ )
			{
				for ( rB=0; rB<cols; rB++ )
				{ res.m[rA,rB] = m[rA,rB] + second.m[rA,rB]; }
			}
			return res;
		}



		RealMatrix   operator_sub ( RealMatrix second )
		{
			if (rows!=second.rows || cols!=second.cols){throw new System.Exception("incompatible indexes for sum");}
			RealMatrix res = new RealMatrix( rows, cols);
			int rA, rB;
			for ( rA=0; rA<rows; rA++ )
			{
				for ( rB=0; rB<cols; rB++ )
				{ res.m[rA,rB] = m[rA,rB] - second.m[rA,rB]; }
			}
			return res;
		}


		RealMatrix   operator_div ( RealMatrix second )
		{
			if (second.rows != second.cols) {throw new System.Exception("the matrix to be inverted must be square");}
			if (cols != second.rows) {throw new System.Exception("indexes are incompatible for division; must be: [a,n]*[n,n]");}
			RealMatrix inverse = second.inverse();
			RealMatrix res = this.operator_mul( inverse);
			return res;
		}


		bool operator_compare(  RealMatrix second )
		{
			const double epsilon = 1.0e-35;
			int  row, col;
			for (row=0; row<rows; row++)
			{
				for (col=0; col<cols; col++)
				{
					if( System.Math.Abs( m[row,col] - second.m[row,col] ) > epsilon)
					{ return false; }
				}
			}
			return true;
		}



        /// <summary>
        /// assigns an in-memory matrix(i.e. second) to "this".
        /// </summary>
        /// <param name="second"></param>
        /// <returns></returns>
		public RealMatrix operator_assignment( RealMatrix second )
		{
			if ( rows != second.rows || cols != second.cols )
			{throw new System.Exception("only isomorphic matrixes can be copied");}
            //
			int  row, col;
			for (row=0; row<this.rows; row++) // copy element by element
			{
				for (col=0; col<this.cols; col++)
				{
                    this.m[row,col] = second.m[row,col];
                }// end element assignement.
			}
			return this;
		}// end operator_assignment


         
        /// <summary>
        /// assigns a scalar element, to the position (iRow,jCol)
        /// </summary>
        /// <param name="iRow">row position</param>
        /// <param name="jCol">col position</param>
        /// <param name="theElement">elemnt content</param>
        /// <returns>bool esito</returns>
        public bool element_assignment( int iRow, int jCol, double theElement )
        {
            bool esito = false;
            //
            if(
                    iRow >= this.rows
                ||  jCol >= this.cols
                )
            {
                esito = false;
                //throw new System.Exception( "insertion coordinates( iRow, jCol) are not compatible with _this.layout_" );
            }// else continue;
            this.m[iRow, jCol] = theElement;
            esito = true;
            // ready.
            return esito;
        }// end element_assignment
	


        /// <summary>
        /// get a subMatrix, specifying which row and which column to eliminate.
        /// </summary>
        /// <param name="delendaRow"></param>
        /// <param name="delendaCol"></param>
        /// <returns></returns>
		public RealMatrix reductor ( int delendaRow, int delendaCol )
		{
			int  row_this, col_this,  row_smaller, col_smaller;
			int smallerRowCardinality, smallerColCardinality;
            if ( delendaRow < 0 )//NB. delendaLine<0 means:"delete no such line!", i.e. delendaRow<0 -> keep all the rows.
			{// 
				smallerRowCardinality = this.rows;
			}
			else
			{
				smallerRowCardinality = this.rows-1;
			}
            if ( delendaCol < 0 )//NB. delendaLine<0 means:"delete no such line!", i.e. delendaRow<0 -> keep all the rows.
			{// 
				smallerColCardinality = this.cols;
			}
			else
			{
				smallerColCardinality = this.cols-1;
			}
			RealMatrix smaller = new RealMatrix( smallerRowCardinality, smallerColCardinality);// return value
			// kordell
			for (	row_this=0, row_smaller=0;
				row_this<rows;
				row_this++, row_smaller++  )
			{
				if ( row_this == delendaRow )
				{ row_smaller--; continue; } // skip the selected row, if it must be deleted
				for (   col_this=0, col_smaller=0;
					col_this<cols;
					col_this++, col_smaller++  )
				{
					if ( col_this == delendaCol )
					{ col_smaller--; continue; } // skip the selected col, if it must be deleted
					smaller.m[row_smaller,col_smaller] = m[row_this,col_this]; // main task
				}// end for on columns
			}// end for on rows
			// end kordell
			return smaller;
		}// end method reductor




        /// <summary>
        /// inversion, for non-singular square matrices. Trasposta del cofattore, diviso determinante.
        /// </summary>
        /// <returns></returns>
		public RealMatrix inverse( )
		{
			if(rows!=cols){throw new System.Exception("only square matrices can be inverted (if non singular)");}
			RealMatrix cof = new RealMatrix( rows, rows); // square !
			RealMatrix smaller = null;// the call to "reductor" method will fill this variable
			int  row, col;
			double coefficient;
			for (row=0; row<rows; row++)  // build the cofactor matrix
			{
				for (col=0; col<rows; col++) // cols==rows !  square !
				{
					smaller = reductor ( row, col );// row, col parameters are actual values of lines to be eliminated, to build the actual reduct matrix
                    coefficient = System.Math.Pow( -1.0, row + 1.0 + col + 1.0 );//  -1^(i+j) NB. i,j start from 1; row,col from 0.
					// each element of cof matrix is the det of the appropriate minor * -1^(i+j)
					cof.m[row,col] = coefficient * smaller.det();
				}
			}
            //
            RealMatrix trasp_cof = cof.transpose(); // transpose (cof);// trasposta della matrice cofattore.
			double det_orig = det();   // det of original matrix
			if (0.0==det_orig)
			    {throw new System.Exception("a square matrix can be inverted only if NON SINGULAR");}
			for (row=0; row<rows; row++)  // the transposed of cofactor must be divided by the det of original matrix.
			{
				for (col=0; col<rows; col++) // cols==rows !  square !
                {// the transposed of cofactor must be divided by the det of original matrix.
                    trasp_cof.m[row, col] /= det_orig; // normalize the transposed of cofactor, by the det of original matrix.
				}
			}
            return trasp_cof; // already normalized, by the det of original matrix.
		}// end inverse
		
        #endregion Algorithms





        #region InOutAndMemoryManag


        /// <summary>
        /// get a row vector.
        /// </summary>
        /// <param name="theRow"></param>
        /// <returns></returns>
        public double[] getRow( int theRow )
        {// throws on invalid index
            double[] res = new double[this.cols];
            for ( int c=0; c < this.cols; c++ )
            {
                res[c] = this.m[theRow, c];
            }
            return res;
        }// end get a row vector.


        /// <summary>
        /// get a column vector.
        /// </summary>
        /// <param name="theCol"></param>
        /// <returns></returns>
        public double[] getCol( int theCol )
        {// throws on invalid index
            double[] res = new double[this.rows];
            for ( int c=0; c < this.rows; c++ )
            {
                res[c] = this.m[c, theCol];
            }
            return res;
        }// end get a column vector.


        /// <summary>
        /// write down a column content, at position "theCol-index".
        /// </summary>
        /// <param name="theCol">column index</param>
        /// <param name="colContent">xolumn content</param>
        /// <returns>false on invalid column index.</returns>
        public bool putCol( int theCol, double[] colContent )
        {// boolean_false on invalid index
            if ( colContent.Length != this.rows )
            {
                return false;
            }//
            //
            for ( int c=0; c < this.rows; c++ )
            {
                this.m[c, theCol] = colContent[c];
            }
            return true;
        }// end put a column vector.



        /// <summary>
        /// write down a row content, at position "theRow-index".
        /// </summary>
        /// <param name="theRow">row index</param>
        /// <param name="rowContent">row content</param>
        /// <returns>false on invalid row index.</returns>
        public bool putRow( int theRow, double[] rowContent )
        {// boolean_false on invalid index
            if ( rowContent.Length != this.cols )
            {
                return false;
            }//
            //
            for ( int c=0; c < this.cols; c++ )
            {
                this.m[theRow, c] = rowContent[c];
            }
            return true;
        }// end put a row vector.




        /// <summary>
        /// write a matrix on the Console.
        /// </summary>
		public void show( )
		{// print a matrix on the console
			int  row, col;
			if ( m == null ) return;			
			System.Console.WriteLine( "\n\n");
			for (row=0; row<rows; row++)  //  show matrix
			{
				for (col=0; col<cols; col++)
				{
					System.Console.Write( "\t" + m[row,col]);
					if (col==cols-1)
					{System.Console.WriteLine( "\n");}
				}
			}
		}


        /// <summary>
        /// write a matrix on the LogSinkFileSystem.
        /// </summary>
        public void show_LogSinkFs()
        {// print a matrix on the sink filesystem log.
            LogSinkFs.Wrappers.LogWrappers.SectionOpen( "LinearAlgebra::RealMatrix::show_LogSinkFs()", 0 );
            int  row, col;
            if ( m == null )
                return;
            string matrixOnLog = "";// init to empty string, then operate adding chunks.
            //
            for ( row = 0; row < rows; row++ )  //  show matrix
            {
                for ( col = 0; col < cols; col++ )
                {// LogSinkFs writes a stream-line each SectionContent(); so accumulate until a matrix line ends, and the log it and reset the buffer for the next line.
                    matrixOnLog += "\t" + m[row, col];// accumulate each column entry, in the container line.
                    //
                    if ( col == cols - 1 )// if current column is the last of the line,
                    {
                        LogSinkFs.Wrappers.LogWrappers.SectionContent( matrixOnLog, 0 );// flush the buffer and reset it.
                        matrixOnLog = "";// init to empty string, then operate adding chunks. Reset the buffer for the next line.
                    }// end if is last column.
                }// end for each column.
            }// end for each row.
            // ready.
            LogSinkFs.Wrappers.LogWrappers.SectionClose();
        }// end method show_LogSinkFs().



        /// <summary>
        /// write a matrix on the Console.
        /// </summary>
        public void show( double treshold )
        {// print a matrix on the console
            int row, col;
            if ( m == null )
                return;
            System.Console.WriteLine( "\n\n" );
            for ( row = 0; row < rows; row++ )  //  show matrix
            {
                for ( col = 0; col < cols; col++ )
                {
                    if ( row == col )// if it's a principal diagonal element
                    {
                        if ( System.Math.Abs( m[row, col] - 1.0 ) < treshold )// if it's near enough to one
                        {
                            System.Console.Write( "\t" + 1.0 );// show it like a sharp one
                        }
                        else// if it's NOT near enough to one
                        {
                            System.Console.Write( "\t" + m[row, col] );// show it like what it really is
                        }
                    }// end print principal-diagonal element.
                    else if ( System.Math.Abs( m[row, col] ) > treshold )// if it's outside the principal diagonal and if it's too big to appear like a zero
                    {
                        System.Console.Write( "\t" + m[row, col] );// show it like what it really is
                    }
                    else// if it's outside the principal diagonal and if its modulus is small enough to give it a zero's reputation
                    {
                        System.Console.Write( "\t" + 0.0 );// show it like a sharp zero
                    }
                    if ( col == cols - 1 )// if the column is the last one
                    {
                        System.Console.WriteLine( "\n" );// go to next row
                    }// if last col
                }// for col
            }// for row
        }// end  show( double treshold )




        /// <summary>
        /// read a matrix from the Console.
        /// </summary>
		private void input( )
		{// input each element of the matrix from the console
			int  row, col;
			for (row=0; row<rows; row++)  //  input  the matrix
			{
				for (col=0; col<cols; col++)
				{
					System.Console.WriteLine( "Enter element["+row+"]["+col+"]\t");
					m[row,col] = double.Parse( System.Console.ReadLine() );
				}
			}
		}


        /// <summary>
        ///  insert a single element [row,col]
        /// </summary>
        /// <param name="what">the element, to be inserted.</param>
        /// <param name="row">the row coordinate of the insertion</param>
        /// <param name="col">the col coordinate of the insertion</param>
        /// <returns></returns>
		public bool insert (double what, int row, int col) // cannot be const
		{// insert a single element [row,col]
			if ( row>=rows || col>=cols )
			    {return false;}// invalid index required
			m[row,col] = what;
			return true;
		}



        /// <summary>
        /// deserialize a matrix, from a stream.
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
		public bool readfileByChars( string where )
		{  // File format is NOT compulsory:
			//      The only costraint is that the number of elements must be rows*cols
			//      Entries can be divided both by spaces, or tabs (one or more of each)
			System.IO.StreamReader  input;
			input = new System.IO.StreamReader( where);
			int row, col;
			for ( row=0; row<rows; row++)
			{
				for ( col=0; col<cols; col++ )
				{
					System.Text.StringBuilder temp = new System.Text.StringBuilder();
					int tmpChar;
					for( tmpChar=-1;; )// until space or tab
					{
						tmpChar = input.Read();
                        if ( -1 == tmpChar )
                        {// EOF
                            break;// exit on EOF
                        }
						if( 32==tmpChar || 9==tmpChar )// if it's space or tab
						{
							for( ; tmpChar==9||tmpChar==32; )
							{
								tmpChar = input.Peek();// read but do not consume
								if(tmpChar!=9 && tmpChar!=32)
								{
									tmpChar = ' ';// trailing spaces do not affect double.Parse, other characters do
									break;
								}
								else
								{
									input.Read();// consume separators
								}
							}
							tmpChar = ' ';// trailing spaces do not affect double.Parse, other characters do
							break;
						}
						if( 10==tmpChar )
						{
							tmpChar = ' ';// trailing spaces do not affect double.Parse, other characters do
							break;
						}
						if( 13==tmpChar)
						{
							tmpChar = input.Peek();// read but do not consume
							if( 10==tmpChar)
							{
								tmpChar = ' ';// trailing spaces do not affect double.Parse, other characters do
								input.Read();
							}
							// anyway
							break;
						}
						temp.Append( (char)tmpChar);
					}// end for each character
                    //
                    char[] separators = new char[4] { ( char )9, ( char )32, ( char )10, ( char )13 };
                    string bufferizedSymbols = temp.ToString();
                    string[] tokens = bufferizedSymbols.Split( separators, System.StringSplitOptions.RemoveEmptyEntries );
                    if ( 0 == tokens.Length )
                    {// reached an empty symbol.
                        continue;// so go to the next loop round, without assignement of any variable.
                    }// else TryParse

                    string curTok = bufferizedSymbols;// TODO debug

//bool parsingWasPossible = double.TryParse( bufferizedSymbols, out m[row, col] );
                    // if( ! parsingWasPossible ) do_something


				}// end for columns
			}// end for rows
            //
			input.Close();
			return true;
		}// end readfile





        // private filename utility.
        private string validatedStremFullPathProducer( string where )
        {
            string validatedStream_fullPath = null;
            int extensionStartIndex = where.LastIndexOf( '.' );
            string inputExtension = where.Substring( extensionStartIndex );
            validatedStream_fullPath = where.Substring( 0, extensionStartIndex );
            validatedStream_fullPath += "_validated_";
            validatedStream_fullPath += inputExtension;
            //
            return validatedStream_fullPath;
        }// end validatedStremFullPathProducer



		 


        /// <summary>
        /// validation of a file content.
        /// </summary>
        /// <param name="validationRows">rows cardinality</param>
        /// <param name="validationCols">columns cardinality</param>
        /// <param name="where">file fullpath</param>
        /// <returns></returns>
        public bool validate( int validationRows, int validationCols, string where )
		{  // File format is NOT compulsory:
			//      The only costraint is that the number of elements must be rows*cols
			//      Entries can be divided both by spaces, or tabs (one or more of each)
			System.IO.StreamReader  input;
			try
			{
				input = new System.IO.StreamReader( where);
			}
			catch( System.Exception)
            {// on unable to open input.
				return false;
			}
            //
            System.IO.StreamWriter validatedStream;
            try
			{
                string validatedStream_fullPath = this.validatedStremFullPathProducer( where );
                //
				validatedStream = new System.IO.StreamWriter(
                    validatedStream_fullPath,
                    false,// must append.
                    System.Text.Encoding.Default );
			}
			catch( System.Exception)
			{// on unable to open output.
				return false;
			}
            //
			int elements = 0;
			int curRows = 0;
			string  tmp="";
			//
			for(;;)
			{
				tmp = input.ReadLine();
				if(tmp==null)// NB. this is the EOF condition.
				{// End Of File (EOF) reached -> don't parse anymore.
					break;
				}// else, still got something to parse.
				char[] separators = new char[4]{ (char)9, (char)32, (char)10, (char)13};
                string[] tokens = tmp.Split( separators, System.StringSplitOptions.RemoveEmptyEntries );// read maximum the declared columns.
                if ( 0 == tokens.Length )
                {// reached an empty row.
                    continue;// so go to the next loop round, without increment of any variable.
                }
				if (tokens.Length < validationCols)
                {// wrong tensor found. This could be only columns in defect, since for columns in excess there is a parameter in the Split() call.
                    return false;// exit on wrong tensor. This could be only columns in defect, since for columns in excess there is a parameter in the Split() call.
				}// else ok
				++curRows;// another row, correctly read.
                // N.B. syntax note:  result = (boolean_evaluation) ? value_to_be_assigned_onTrue : value_to_be_assigned_onFalse ; 
                // if tokens in row were too few, already caught. If == ok! If tokens too many, cut on last useful one.
                int appropriateElements =  (tokens.Length >= validationCols) ? validationCols : tokens.Length;// see note above.
                elements += appropriateElements;// all the elements of a correct row, are added to the tensor cardinality ( i.e. Length over all tensor dimensions).
                for ( int curRowToken=0; curRowToken < validationCols; curRowToken++ )
                {
                    validatedStream.Write( tokens[curRowToken] );
                    validatedStream.Write( " " );
                }
                validatedStream.WriteLine();// end of row, in the matrix-file.
                // read maximum the declared rows.
                if ( validationRows <= curRows )// skip possible rows in excess.
                {// read maximum the declared rows.
                    break;// read maximum the declared rows.
                }
			}// end for
			input.Close();
            validatedStream.Flush();
            validatedStream.Close();
            if ( elements != validationRows * validationCols || curRows != validationRows )
            {
                return false;// wrong tensor found. This should be only rows in defect. Other wrong tensor cases are managed before.
            }// else return "tensor is valid".
			return true;
		}// end validate





         
        /// <summary>
        /// validation of a file content.
        /// </summary>
        /// <param name="validationRows">rows cardinality</param>
        /// <param name="validationCols">columns cardinality</param>
        /// <param name="where">file fullpath</param>
        /// <returns></returns>
        public bool readfileByTokens( int validationRows, int validationCols, string where )
        {  // File format is NOT compulsory:
            //      The only costraint is that the number of elements must be rows*cols
            //      Entries can be divided both by spaces, or tabs (one or more of each)
            System.IO.StreamReader  input;
            try
            {
                input = new System.IO.StreamReader( where );
            }
            catch ( System.Exception )
            {// on unable to open input.
                return false;
            }
            //
            int elements = 0;
            int curRows = 0;
            string  tmp="";
            //
            for ( ; ; )
            {
                tmp = input.ReadLine();
                if ( tmp == null )// NB. this is the EOF condition.
                {// End Of File (EOF) reached -> don't parse anymore.
                    break;
                }// else, still got something to parse.
                char[] separators = new char[4] { ( char )9, ( char )32, ( char )10, ( char )13 };
                string[] tokens = tmp.Split( separators, System.StringSplitOptions.RemoveEmptyEntries );// read maximum the declared columns.
                if ( 0 == tokens.Length )
                {// reached an empty row.
                    continue;// so go to the next loop round, without increment of any variable.
                }
                if ( tokens.Length < validationCols )
                {// wrong tensor found. This could be only columns in defect, since for columns in excess there is a parameter in the Split() call.
                    return false;// exit on wrong tensor. This could be only columns in defect, since for columns in excess there is a parameter in the Split() call.
                }// else ok
                for ( int curRowToken=0; curRowToken < validationCols; curRowToken++ )
                {
                    bool wasParseable = double.TryParse( tokens[curRowToken], out this.m[curRows, curRowToken] );
                    if ( !wasParseable )
                    {
                        throw new System.Exception( " Stream NOT Parseable ! " );
                    }// else ok
                }// end of row, in the matrix-file.
                ++curRows;// another row, correctly read.
                // N.B. syntax note:  result = (boolean_evaluation) ? value_to_be_assigned_onTrue : value_to_be_assigned_onFalse ; 
                // if tokens in row were too few, already caught. If == ok! If tokens too many, cut on last useful one.
                int appropriateElements =  ( tokens.Length >= validationCols ) ? validationCols : tokens.Length;// see note above.
                elements += appropriateElements;// all the elements of a correct row, are added to the tensor cardinality ( i.e. Length over all tensor dimensions).
                // read maximum the declared rows.
                if ( validationRows <= curRows )// skip possible rows in excess.
                {// read maximum the declared rows.
                    break;// read maximum the declared rows.
                }
            }// end for
            input.Close();
            if ( elements != validationRows * validationCols || curRows != validationRows )
            {
                return false;// wrong tensor found. This should be only rows in defect. Other wrong tensor cases are managed before.
            }// else return "tensor is valid".
            return true;
        }// end readfileByTokens



        /// <summary>
        /// serialize a matrix on a stream.
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
		public bool writefile( string where  )
		{
			int  row, col;
			System.IO.StreamWriter  output;
			output = new System.IO.StreamWriter( where);
			//
			for (row=0; row<rows; row++)  //  fill a matrix on the filesystem
			{
				for (col=0; col<cols; col++)
				{
					output.Write( m[row,col].ToString() ); // in C++ out << m[row,col];
					if (col==cols-1)
					{
						output.Write( "\r\n");
					}
					else
					{
						output.Write( "\t");
					}
				}// columns
			}// rows
			output.Close();
			return true;
		}// end writefile



        /// <summary>
        /// serialize a matrix on a stream, following the Wolfram convention, which enumerates vector elements within curl braces {}.
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
		public bool Wolfram_writefile( string where  )
		{
			int  row, col;
			System.IO.StreamWriter  output;
			output = new System.IO.StreamWriter( where);
			//
			output.Write( "{ \r\n { ");
			for (row=0; row<rows; row++)  //  fill a matrix on the filesystem
			{
				for (col=0; col<cols; col++)
				{
                    output.Write( m[row, col].ToString() ); // in C++ out << m[row,col];
					if (col==cols-1)
					{
						if( row<rows-1)
						{
							output.Write( " }# \r\n { ");
						}
						else
						{
							output.Write( " } \r\n");
						}
					}
					else
					{
						output.Write( "#\t");
					}
				}// columns
			}// rows
			output.Write( "}\r\n");
			output.Close();
			return true;
		}// end Wolfram_writefile



        /// <summary>
        /// randomize a matrix.
        /// </summary>
		private void generator( )
		{
			System.Random rand = new System.Random();// this constructor already uses a time-dependent seed
			//const double rand_reductor = (9.0)/(double)RAND_MAX; this was C++
			int  row, col;
			//srand( (unsigned)time( NULL ) ); // seed with clock: different seed each time.  // C++
			for (row=0; row<rows; row++)
			{  //  fill a matrix with randomized entries
				for (col=0; col<cols; col++)
				{
					m[row,col] = rand.NextDouble();
				}// columns
			}// rows
		}// end generator


        /// <summary>
        /// gets the real number at coordinates Matrix[i,j]
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <returns></returns>
		public double get_element( int i, int j)
		{
			if( i<this.rows && j<this.cols)
				return this.m[i,j];
			throw new System.Exception("suggested indexes( row and/or col) are inconsistent with the matrix dimensions.");
        }


        #endregion InOutAndMemoryManag


    }// end class RealMatrix
} //closing namespace LinearAlgebra
