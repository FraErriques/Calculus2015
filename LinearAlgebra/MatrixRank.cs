//#define LogSinkFs

using System;
using System.Collections.Generic;
using System.Text;



namespace LinearAlgebra
{

    /// <summary>
    /// a son class, intended to evaluate the MatrixRank of the father RealMatrix.
    /// </summary>
    public class MatrixRank : LinearAlgebra.RealMatrix
    {
        #region Data

        /// <summary>
        /// classe utilizzata per individuare le combinazioni semplici(i.e. senza ripetizione) di colonne, costituenti le sottomatrici quadrate, di una rettangolare.
        /// </summary>
        public class SquareSmallerRank
        {
            /// <summary>
            /// column indexes, of present minor.
            /// </summary>
            public int[] columnIndexes;
            /// <summary>
            /// det of present minor.
            /// </summary>
            public int Rank;
            //
            private SquareSmallerRank()// Ctor() hide it.
            {
            }
            /// <summary>
            /// Ctor
            /// </summary>
            /// <param name="columnCardinality"></param>
            public SquareSmallerRank( int columnCardinality )
            {
                this.columnIndexes = new int[columnCardinality];
                this.Rank = 0;//init
            }// end Ctor
        }//end class SquareSmallerRank
        //
        private SquareSmallerRank[] RankSequence;



        /// <summary>
        /// a class used to classify minors, in the context of "gestioneQuadrata".
        /// </summary>
        public class SmallerDet
        {
            /// <summary>
            /// the number of Get_rows==Get_cols, in the present square tensor.
            /// </summary>
            public int SubMatrixSize;
            /// <summary>
            /// current Submatrix, in Smaller Det procedure.
            /// </summary>
            public LinearAlgebra.RealMatrix SubMatrix;
            /// <summary>
            /// det of present minor.
            /// </summary>
            public double Det;
            //
            private SmallerDet()// Ctor() hide it.
            {
            }
            /// <summary>
            /// Ctor
            /// </summary>
            /// <param name="columnCardinality">matrix size</param>
            /// <param name="SubMatrix">the SubMatrix</param>
            /// <param name="Det">determinant</param>
            public SmallerDet( int columnCardinality, LinearAlgebra.RealMatrix SubMatrix, double Det )
            {
                this.SubMatrixSize = columnCardinality;// it's a square subMat.
                this.SubMatrix = SubMatrix;
                this.Det = Det;
            }// end Ctor
        }//end class SquareSmallerRank
        //
        /// <summary>
        /// we don't know how many dimensions will be involved, until Rank is found.
        /// we should become certain about how many subMatrices we need to treat, in a single dimension. But it depends also on how quickly you fin a nonnul minor, so it's a dynamic data.
        /// </summary>
        public  System.Collections.ArrayList involvedDimensions;


        // debug only : TODO must be transformed in a stack, to avoid recursion.
        //public  int rangoQuadrata_recursion_ordinal = 0;// debug only


        /// <summary>
        /// an internal class, to manage the combination mechanics.
        /// </summary>
        public class CombinationParams
        {
            /// <summary>
            /// pivotano tutti gli indici, dal leftMostPivoter verso destra.
            /// </summary>
            public int leftMostPivoter;// pivotano tutti gli indici, dal leftMostPivoter verso destra.
            /// <summary>
            /// ciascuna colonna ha un massimo indice assegnabile, dipendente dalla posizione della colonna.
            /// </summary>
            public int[] assignableMaxima;// ciascuna colonna ha un massimo indice assegnabile, dipendente dalla posizione della colonna.
            /// <summary>
            /// // ciascun indice puo' assumere valori in [naturalIndex[i], assignableMaxima[i]]
            /// </summary>
            public int[] indexRange;
            /// <summary>
            /// // ciascun indice puo' assumere valori in [naturalIndex[i], assignableMaxima[i]]
            /// </summary>
            public int[] naturalIndexes;
            /// <summary>
            /// for each position, the naturalIndex is the position itsefl.
            /// </summary>
            public int thePivotingColumn;
            /// <summary>
            /// la maggiore delle dimensioni
            /// </summary>
            public int cardinalitaColonne;
            /// <summary>
            /// la minore delle dimensioni
            /// </summary>
            public int cardinalitaRighe;
            //
            private CombinationParams( )// Ctor() hide it.
            {
            }
            //
            /// <summary>
            /// Ctor() init all.
            /// </summary>
            /// <param name="rettangolareAnalizzanda"></param>
            public CombinationParams( LinearAlgebra.MatrixRank rettangolareAnalizzanda )
            {
                this.leftMostPivoter = rettangolareAnalizzanda.Get_rows - 1;// la minore delle dimensioni.
                //
                this.assignableMaxima = new int[rettangolareAnalizzanda.Get_rows];
                int assignementAccumulator=0;
                for ( int assignableMaximum=rettangolareAnalizzanda.Get_cols - 1; assignementAccumulator < rettangolareAnalizzanda.Get_rows; assignementAccumulator++ )
                {
                    assignableMaxima[rettangolareAnalizzanda.Get_rows - assignementAccumulator - 1] = assignableMaximum--;
                }// end for assignableMaxima
                //
                this.indexRange = new int[rettangolareAnalizzanda.Get_cols];// la maggiore delle dimensioni.
                for ( int curIndex=0; curIndex < rettangolareAnalizzanda.Get_cols; curIndex++ )
                {
                    this.indexRange[curIndex] = curIndex;
                }// emd for indexRange
                //
                this.naturalIndexes = new int[rettangolareAnalizzanda.Get_rows];
                for ( int columnIndex=0; columnIndex < naturalIndexes.Length; columnIndex++ )// per ciascuna colonna
                {
                    naturalIndexes[columnIndex] = columnIndex;// set non-pivot columns to their natural index.
                }//NB. all columns are non-pivot, in this phase; one of them will become, later.
                //
                this.thePivotingColumn = rettangolareAnalizzanda.Get_rows - 1;// start from the last on the right.
                //
                this.cardinalitaColonne = rettangolareAnalizzanda.Get_cols;
                this.cardinalitaRighe = rettangolareAnalizzanda.Get_rows;
                //
            }// end Ctor()
            //
        }// end  class CombinationParams
        /// <summary>
        /// instance, of the class wide data.
        /// </summary>
        public CombinationParams combinationParams;
        #endregion Data



        #region Ctors
        //   Constructors and Destructors   BEGIN


        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="Prows"></param>
        /// <param name="Pcols"></param>
        public MatrixRank( int Prows, int Pcols )
            : base( Prows, Pcols )
        {
        }// Ctor()


        /// <summary>
        /// Ctor : deserialize from a stream.
        /// </summary>
        /// <param name="Prows"></param>
        /// <param name="Pcols"></param>
        /// <param name="where"></param>
        public MatrixRank( int Prows, int Pcols, string where )
            : base( Prows, Pcols, where )
        {
        }// end Ctor with deserialization from stream.


        /// <summary>
        /// get the memory-address of a variable already containing an appropriate double[,].
        /// </summary>
        /// <param name="prototype">the appropriate double[,]</param>
        /// <param name="Prows">Get_rows</param>
        /// <param name="Pcols">columns</param>
		public MatrixRank( double[,] prototype, int Prows, int Pcols) : base( prototype, Prows, Pcols)
		{
		}// END : get the memory-address of a variable already containing an appropriate double[,].

		
        /// <summary>
        /// create in RAM and, if desired, dump on a file. Tou can choose to randomize.
        /// </summary>
        /// <param name="Prows"></param>
        /// <param name="Pcols"></param>
        /// <param name="randomized"></param>
        /// <param name="where"></param>
        public MatrixRank( int Prows, int Pcols,
            bool randomized,
            string where ) // NULL==where means "no dump desired"
            : base( Prows, Pcols, randomized, where )
        {
        }// END : create in RAM and, if desired, dump on a file. Tou can choose to randomize.


        /// <summary>
        /// Copy Constructor
        /// </summary>
        /// <param name="original"></param>
        public MatrixRank( RealMatrix original )
            : base( original )
        {
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
        public MatrixRank( RealMatrix original, int from, int to )
            : base( original, from, to )
        {
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
        public MatrixRank( RealMatrix original, int from, int to, bool isOnColumn )
            : base( original, from, to, isOnColumn )
        {
        }// end Copy Constructor, with column exchange



        /// <summary>
        /// Destructor: it frees the memory of the underlying matrix.
        /// </summary>
        ~MatrixRank()
        {
            base.deallocate();// deallocate the underlying Matrix.
        }// end ~Dtor().

 #endregion Ctors




        #region Rango




        /// <summary>
        /// dimensione del massimo minore nonnullo. Ovvero numero di linee della piu' grande sottomatrice quadrata, a determinante nonnullo.
        /// La matrice analizzata (i.e. sourceMat) puo' essere rettangolare, con dimensioni qualsiasi. Il massimo rango possibile e' la minore
        /// delle dimensioni della matrice analizzata.
        /// Il caso di matrice in esame rettangolare, costituisce il caso generale. Da esso vengono prodotti molteplici sottocasi di matrice quadrata.
        /// Lo entry point del Rango deve quindi essere una funzione che smista dal caso generale, di input rettangolare, al caso semplificato di input quadrato
        /// Il caso di primo input gia' quadrato, permette soluzione diretta, mediante il medesimo metodo di individuazione delle sottomatrici, utilizzato per i cofattori dell'inversa.
        /// Il caso di primo input rettangolare, richiede invece un'iterazione, per la produzione di tutti i sottocasi quadrati, fino ad individuazione del primo minore nonnullo.
        /// Durante tale processo, sara' utilizzata la variabile "curDimension", ad indicare l'ordine delle sottomatrici quadrate in esame.
        /// Esaurite tutte le sottomatrici quadrate di ordine "curDimension", si passa alla produzione delle quadrate d'ordine "curDimension-1".
        /// Per ogni valore di "curDimension"=="k", si hanno (n k) (i.e. "n" bionomioNewton "k") quadrate kXk, estratte da una rettangolare nXk, con n>k.
        /// Nel caso si parta gia' da una quadrata kXk, si deve in primis calcolarne il determinante. Ove esso sia nullo si opera "--curDimension" e cosi' via.
        /// Le sottofunzioni ad oggi(2015.09.02) individuate sono:
        /// -   selezione di "curDimension": distingue i casi quadrata-rettangolare e imposta "curDimension"
        /// -   "producer" : genera la sottomatrice da analizzare( combinando colonne)
        /// -   "ePermutazioneDi" : valuta se una certa sequenza di indici di colonna sia una permutazione di una gia' valutata; si serve della sottofunzione "appartieneA"
        /// -   "appartieneA" : prende uno scalare, indice di una colonna, e dice se appartiene ad una collezione di indici di colonna, ricevuta a parametro. E' una funzione elementare, con 
        ///     schema {1}appartieneA({1,2,3})==true; {7}appartieneA({1,2,3})==false; la precedente funzione "ePermutazioneDi", va implementata considerando il risultato booleano cumulativo 
        ///     di "appartieneA", ovvero: {1,3,2}"ePermutazioneDi"({1,2,3})==({1}appartieneA({1,2,3})and{2}appartieneA({1,2,3})and{3}appartieneA({1,2,3}) ).
        /// </summary>
        /// <returns></returns>
        public int Rango()// LinearAlgebra.RealMatrix sourceMat )
        {// schema operativo:
            //NB. il numero antistante la riga, indica la funzione nella quale viene svolto il task.
            // legenda: 
            // (1)==EntryPoint()( i. e. rango).
            // (2)==gestioneQuadrata().
            // (3)==gestioneRettangolare().
            // (4)==producer().
            // (5)==selettore().
            // (6)==ePermutazioneDi().
            // (7)==appartieneA().
            //------------------------------------------------------------------------
            // 1) distinguere tra sourceMat quadrata e sourceMat rettangolare
            // 1) individuare la minore fra le cardinalita', di riga e di colonna e valorizzare la variabile "curDimension"
            // 2) incaso di sourceMat quadrata, ->gestioneQuadrata() e uscita.
            // 3) incaso di sourceMat rettangolare, ->gestioneRettangolare(), da cui (4)producer->(5)selettore->(6)ePermutazioneDi->appartieneA()->gestioneQuadrata()->rientro a
            //    gestioneRettangolare() per l'estrazione delle quadrate successive.
            // (4)Osservazioni sul producer(4):comporre le sottomatrici, formando le combinazioni semplici( i.e. senza ripetizione) delle "n" linee, prese "k" per volta.
            // (4)tali combinazioni semplici hanno cardinalita' (n k), nel senso del binomio di Newton; ovvero "k" elenti, estratti da un insieme di cardinalita' "n", senza possibilita'
            // (4)di reinserimento del medesimo elemento e con irrilevanza dell'ordine. Oss. k<=n.
            //
            int curDimension;
            bool isSquare = false;
            int Rango;
            LinearAlgebra.MatrixRank workCopy = null;
            //
            if ( this.Get_cols < this.Get_rows )
            {
                workCopy = new MatrixRank( this.transpose() );
                curDimension = this.Get_cols;//---###################### the minimum between the two dimensions, becomes curDimension ##############
            }// end if ( this.Get_cols < this.Get_rows )
            else if ( this.Get_cols > this.Get_rows )
            {// i.e. Get_cols > Get_rows
                workCopy = new MatrixRank( this );// keep it as is.
                curDimension = this.Get_rows;//---###################### the minimum between the two dimensions, becomes curDimension ##############
            }// end if Get_cols >= Get_rows
            else //   this.Get_cols == this.Get_rows 
            {
                workCopy = new MatrixRank( this );// keep it as is.
                curDimension = this.Get_rows;//---###################### the minimum between the two dimensions, becomes curDimension ##############
                isSquare = true;
            }// end if.
            //
            if ( isSquare )
            {
                Rango = this.gestioneQuadrata( workCopy );//----------###################### NonTailrecursion ##########################################################
            }
            else
            {
                Rango = this.gestioneRettangolare( workCopy );//--------############# Produzione sottomatrici quadrate ed iterazione su this.gestioneQuadrata() ########
            }
            //ready.
            return Rango;
        }// end Rango()





        /// <summary>
        /// gestioneQuadrata
        /// </summary>
        /// <returns></returns>
        private int gestioneQuadrata( LinearAlgebra.MatrixRank quadrataAnalizzanda )
        {
            int Rango = 0;// init
            double curSubMatrixDet = 0.0;// init
            //
            if ( quadrataAnalizzanda.Get_rows != quadrataAnalizzanda.Get_cols )
            {
                throw new System.Exception( "only square matrices can be accepted by method gestioneQuadrata." );
            }
#if debug
            quadrataAnalizzanda.show();
            System.Console.WriteLine( " dimensione matrice= {0}", quadrataAnalizzanda.Get_rows );
#endif
            //
            if ( 0 == quadrataAnalizzanda.Get_rows )// size one matrix is a scalar;
            {// zero size matrix is no-number.
                Rango = 0;
                return Rango;
            }// avoid an exception, due to zero-sized matrix.
            //
            curSubMatrixDet = quadrataAnalizzanda.det();
            if ( System.Math.Abs( curSubMatrixDet ) > +1.0E-12 )// at the first nonnull minor
            {
                Rango = quadrataAnalizzanda.Get_rows;
                return Rango;
            }// end if Rango pieno; else continue with recursion.
            //
            involvedDimensions = new System.Collections.ArrayList();// for all dimensions.
            for ( int analyzedDimension = quadrataAnalizzanda.Get_rows; analyzedDimension > 0; analyzedDimension-- )
            {
                involvedDimensions.Add( new System.Collections.ArrayList() );// add an ArrayList, for each dimension, prior scanning.
            }// end dimensional ArrayList allocation
            Rango = sottomatriciTelescopiche( quadrataAnalizzanda );//--##########------------solve by recursion-----#########################
            //
            System.Collections.ArrayList finalRankResults = new System.Collections.ArrayList();
            for ( int c=0; c < involvedDimensions.Count; c++ )
            {
                // lettoreRisultati( ( ( System.Collections.ArrayList )( involvedDimensions[c] ) ) );  dbg
                if ( 0 < ( ( System.Collections.ArrayList )( involvedDimensions[c] ) ).Count )
                {//  if: add only from populated dimensions.
                    SmallerDet curElement =  ( ( SmallerDet )( ( ( System.Collections.ArrayList )( involvedDimensions[c] ) )[0] ) );
                    finalRankResults.Add( curElement.SubMatrixSize );
                }// end if: add only from populated dimensions.
            }
            // rilettura risultati finali
            for ( int c=0; c < finalRankResults.Count; c++ )
            {
                if ( ( ( int )( finalRankResults[c] ) ) >= Rango )
                {
                    Rango = ( ( int )( finalRankResults[c] ) );
                }// else skip smaller Rank.
            }// end for each finalRankResults
            //
            //ready.
            return Rango;
        }// end gestioneQuadrata




        /// <summary>
        /// metodo ricorsivo che, partendo da una nxn, trova tutte le sottomatrici kxk, con k in [1, n-1].
        /// </summary>
        /// <param name="quadrataAnalizzanda"></param>
        /// <returns></returns>
        private int sottomatriciTelescopiche( LinearAlgebra.RealMatrix quadrataAnalizzanda )
        {
            //
            int Rank = 0;// init
#if debug
            rangoQuadrata_recursion_ordinal++;//----dbg----
            System.Console.WriteLine( " rangoQuadrata_recursion_ordinal= {0}", rangoQuadrata_recursion_ordinal );//----dbg----
#endif
            //
            for ( int curRow=0; curRow < quadrataAnalizzanda.Get_rows; curRow++ )
            {
                for ( int curCol=0; curCol < quadrataAnalizzanda.Get_cols; curCol++ )
                {
                    LinearAlgebra.RealMatrix subMatrix = quadrataAnalizzanda.reductor( curRow, curCol );// current SubMatrix
#if debug
                    System.Console.WriteLine( " dimensione sottomatrice= {0}", subMatrix.Get_rows );
                    System.Console.WriteLine( " linee eliminate row, col = {0},{1}", curRow, curCol );
                    subMatrix.show();
#endif
                    //
                    if ( 0 == subMatrix.Get_rows )// size one matrix is a scalar;
                    {// zero size matrix is no-number.
                        return -1;//-1 means 0x0 matrix dimension reached.
                    }// avoid an exception, due to zero-sized matrix.
                    //
                    //---------##########################################
                    double subMatrix_det = subMatrix.det();//############
                    //---------##########################################
                    //
                    //-----scrivere estrattore di linee coinvolte nella firma
                    //
                    // codifica:
                    // -2 means by now determinant is null.
                    // -1 means 0x0 matrix dimension reached.
                    // 0 means null scalar submatrix reached.
                    // n_in_N means a positive Rank was found.
                    //
                    if ( System.Math.Abs( subMatrix_det ) < +1.0E-12 )//-------- by now(i.e. in this dimension) determinant is null.--------------
                    {// don't log anything, since determinant is null.
                        Rank = -2;// -2 means by now determinant is null.
                    }
                    else if ( System.Math.Abs( subMatrix_det ) < +1.0E-12 && subMatrix.Get_rows == 1 )// 0 means null scalar submatrix reached.
                    {// don't log anything, since determinant is null.
                        Rank = 0;// 0 means null scalar submatrix reached.
                    }
                    else if ( System.Math.Abs( subMatrix_det ) >= +1.0E-12 )//-------#########----positive Rank found---#############--------------
                    {//----- log dimension and signature----------###########################-----------------------------------------
                        Rank = subMatrix.Get_rows;// Rank = subMatrix.Get_rows==n_in_N means a positive Rank was found.
#if debug
                        System.Console.WriteLine( " first stop at Rank={0}", Rank );
#endif
                        //---#########################################################------data structure insertion here !-at position[dimension]----##################
                        ( ( System.Collections.ArrayList )( involvedDimensions[subMatrix.Get_rows] ) ).Add( new SmallerDet(
                            subMatrix.Get_rows,
                            subMatrix,
                            subMatrix_det )
                            );
#if LogSinkFs
                        LogSinkFs.Wrappers.LogWrappers.SectionContent( "size = " + subMatrix.Get_rows, 0 );
                        subMatrix.show_LogSinkFs();
                        LogSinkFs.Wrappers.LogWrappers.SectionContent( "det = " + subMatrix_det, 0 );
#endif
                    }
                    else
                    {
                        throw new System.Exception( "exception in Rank if_instructions found" );
                    }
                    //
#if debug
                    System.Console.WriteLine( " Rank= {0}", Rank );
                    System.Console.WriteLine( " det sottomatrice= {0}", subMatrix_det );
#endif
                    //
                    //---------------------------#########################___start recursion__#########################-------------------
                    Rank = sottomatriciTelescopiche( subMatrix );//-----#########################___start recursion__################-----
                    //
                }// end for Get_cols
            }// end for Get_rows
            // ready
            return Rank;
        }// end sottomatriciTelescopiche()







        /// <summary>
        /// Tiene costanti tutti gli indici di colonna, tranne uno. Su questo itera, attribuendo tutti gli indici del range(i.e. la dimensione massima fra le due; a valle della 
        /// trasposizione e' l'indice delle colonne>righe).
        /// Cosi' facendo produce configurazioni, che vanno date in esame a "selettore", per verificare che siano effettivamente nuove.
        /// Data una rettangolare nXk (con n>k; in caso sia kxn si traspone, senza effetti sui minori), ci sono n_binomio_k combinazioni semplici( i.e. senza ripetizione)
        /// delle "n" colonne, prese "k" a "k".
        /// il producer implementa i cicli necessari a combinare gli indici di colonna; ad ogni combinazione prodotta, entra in azione il selettore, per stabilire se tale
        /// configurazione sia da utilizzare( ovvero calcolarne il determinante) o da scartare.
        /// Solo nel caso il selettore risponda di utilizzare la configuazione, essa viene restituita sotto forma di LinearAlgebra.RealMatrix; altrimenti si fa "continue" nei
        /// cicli di produzione. Osservazione: quando viene prodotta una sottomatrice quadrata, essa viene lavorata sino al piu' piccolo dei suoi minori e qundi viene associato
        /// il rango di tale sottomatrice, al vettore di indici della sua configurazione. Sarebbe problematico esplorare solo la "curDimension" e poi dover tornare sulla medesima
        /// configurazione, quando fosse intervenuto un "--curDimension".
        /// </summary>
        private int gestioneRettangolare( LinearAlgebra.MatrixRank rettangolareAnalizzanda )
        {// since columns are more than Get_rows( always after translation), the simple combinations are cardinality of sorceMat.Get_cols taken 
            // rettangolareAnalizzanda.Get_rows each time ( i.e. sorceMat.Get_cols Newton binomial rettangolareAnalizzanda.Get_rows ).
            this.combinationParams = new CombinationParams( rettangolareAnalizzanda );//  nullify on exit, for gc.
            int binomioN = binomioNewton( rettangolareAnalizzanda.Get_cols, rettangolareAnalizzanda.Get_rows );
            this.RankSequence = new SquareSmallerRank[binomioN];
            //
            // ciclo di lavorazione del vettore di configurazioni.
            for ( int SquareSmallerRank_element=0; SquareSmallerRank_element < this.RankSequence.Length; SquareSmallerRank_element++ )// ciclo di lavorazione del vettore di configurazioni.
            {// allocazione del singolo record di configurazione: chiama il Ctor() della struttura e imposta il vettore degli indici, al numero effettivo delle colonne.
                this.RankSequence[SquareSmallerRank_element] = new SquareSmallerRank( rettangolareAnalizzanda.Get_rows );
                //
                if ( SquareSmallerRank_element == 0 )
                {
                    this.RankSequence[SquareSmallerRank_element].columnIndexes = this.combinationParams.naturalIndexes;
                }
                else if ( SquareSmallerRank_element > 0 )
                {
                    for ( int c=0; c < rettangolareAnalizzanda.Get_rows; c++ )
                    {// NB. e' cruciale assegnare per singolo scalare, in modo da copiare by_value; se si assegna il vettore in un colpo, viene assegnato il puntatore
                        // e i vettori delle diverse strutture puntano alla stessa copia in memoria.
                        this.RankSequence[SquareSmallerRank_element].columnIndexes[c] = this.RankSequence[SquareSmallerRank_element - 1].columnIndexes[c];// as the preceeding one.
                    }
                    producer( this.RankSequence[SquareSmallerRank_element].columnIndexes );
                }
                //bool bocciato = this.selettore( this.RankSequence[SquareSmallerRank_element].columnIndexes ); ex codice TODO eliminare
                //if ( bocciato )
                //{
                //    throw new System.Exception("configurazione bocciata dal selettore.");
                //}
                //
#if debug
                //-----debug output
                for ( int columnIndex=0; columnIndex < rettangolareAnalizzanda.Get_rows; columnIndex++ )// per ciascuna colonna, compresa l'ultima
                {
                    System.Console.WriteLine( " record # {0} ", this.RankSequence[SquareSmallerRank_element].columnIndexes[columnIndex] );
                }// end output del vettore di indici di colonna, per debug.
                System.Console.WriteLine( " fine record nr.{0}\n\n ", SquareSmallerRank_element );
#endif
                //
            }// end for RankSequence
            //
            //----ciclo valorizzazione del campo "Rank" nel vettore RankSequence
            for ( int SquareSmallerRank_element=0; SquareSmallerRank_element < this.RankSequence.Length; SquareSmallerRank_element++ )// ciclo di lavorazione del vettore di configurazioni.
            {//----ciclo valorizzazione del campo "Rank" nel vettore RankSequence
                LinearAlgebra.MatrixRank rankBanco = new MatrixRank( this.combinationParams.cardinalitaRighe, this.combinationParams.cardinalitaRighe );
                for ( int c=0; c < this.combinationParams.cardinalitaRighe; c++ )
                {
                    bool colInserted = rankBanco.putCol( c, rettangolareAnalizzanda.getCol( this.RankSequence[SquareSmallerRank_element].columnIndexes[c] ) );
#if debug
                    System.Console.WriteLine( " record # {0} ", this.RankSequence[SquareSmallerRank_element].columnIndexes[c] );
#endif
                }// end construction by columns
#if debug
                System.Console.WriteLine( " fine record nr.{0}\n\n ", SquareSmallerRank_element );
#endif
                // now compute the rank on such SubMatrix
                this.RankSequence[SquareSmallerRank_element].Rank = gestioneQuadrata( rankBanco );
                rankBanco.deallocator_publicProxy();// gc, to avoid any risk of using old data.
                rankBanco = null;// gc, to avoid any risk of using old data.
            }// end --ciclo valorizzazione del campo "Rank" nel vettore RankSequence
            //
            //----ciclo di determinazione del massimo, fra i ranghi, nel vettore RankSequence
            int MaxDeiRanghi = 0;// init and then compare with actual values.
            int MaxRankConfiguration = 0;
            for ( int SquareSmallerRank_element=0; SquareSmallerRank_element < this.RankSequence.Length; SquareSmallerRank_element++ )// ciclo di lavorazione del vettore di configurazioni.
            {//----ciclo di determinazione del massimo, fra i ranghi, nel vettore RankSequence
                if ( MaxDeiRanghi < this.RankSequence[SquareSmallerRank_element].Rank )
                {
                    MaxDeiRanghi = this.RankSequence[SquareSmallerRank_element].Rank;
                    MaxRankConfiguration = SquareSmallerRank_element;// memorize which configuration ha s the maximum rank.
                }// end rank comparison, on the single scalar.
            }// end ---ciclo di determinazione del massimo, fra i ranghi( array), nel vettore RankSequence
            //
            this.combinationParams = null;// garbage collect.
            return MaxDeiRanghi;
        }// end gestioneRettangolare





        /// <summary>
        /// produce a configuration int[]
        /// NB. la configurazione presente, deve essere passata uguale alla precedente, in quanto sia l'incremento naturale che la ristrutturazione, vengono fatte a 
        /// partire dalla configurazione precedente. 
        /// </summary>
        private void producer( int[] columnIndexes )
        {
            //
            for ( int columnIndex=this.combinationParams.cardinalitaRighe - 1; columnIndex >= 0; columnIndex-- )// per ciascuna colonna
            {
                if ( columnIndexes[columnIndex] >= this.combinationParams.assignableMaxima[columnIndex] )// i.e. colonna non piu' incrementabile
                {
                    //System.Console.WriteLine( " before ristr: columnIndex={0}, value={1} , assignableMaxima[{2}]={3}",
                    //    columnIndex,
                    //    columnIndexes[columnIndex],
                    //    columnIndex,
                    //    this.combinationParams.assignableMaxima[columnIndex]
                    // );
                    continue;
                }
                else// i.e. colonna ancora incrementabile
                {
                    columnIndexes[columnIndex]++;// incremento colonna<ceiling.
                    int indiciADestra = columnIndex + 1;
                    int acc=0;
                    for ( ; indiciADestra < this.combinationParams.cardinalitaRighe; indiciADestra++ )
                    {
                        columnIndexes[indiciADestra] = columnIndexes[columnIndex] + ++acc;
                    }// end ristrutturazione
                    //System.Console.WriteLine( " no_ristr this.combinationParams.leftMostPivoter= {0}, columnIndex={1}", this.combinationParams.leftMostPivoter, columnIndex );
                    break;
                }
            }// end for
        }// end producer




        #region factorial

        /// <summary>
        /// fattoriale
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public ulong factUlong( ulong n )
        {
            if ( n < 0 )
            {
                throw new System.Exception( "wrong parameter to funtcion fact()" );
            }// else continue.
            //
            ulong theFact = +1;// will grow by product.
            for ( ulong k=n; k > 1; k-- )
            {
                theFact *= k;
            }// for.
            //ready.
            return theFact;
        }// end fact


        /// <summary>
        /// fattoriale
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public int fact( int n )
        {
            if ( n < 0 )
            {
                throw new System.Exception( "wrong parameter to funtcion fact()" );
            }// else continue.
            //
            int theFact = +1;// will grow by product.
            for ( int k=n; k > 1; k-- )
            {
                theFact *= k;
            }// for.
            //ready.
            return theFact;
        }// end fact



        /// <summary>
        /// cardinalita delle combinazioni senza ripetizione, di "n" elementi, presi "k" a "k" (k_smallerOrEqual_n)
        /// </summary>
        /// <param name="n">totale elementi</param>
        /// <param name="k">cardinalita del raggruppamento </param>
        /// <returns></returns>
        public ulong binomioNewton( ulong n, ulong k )
        {
            if ( k > n )
            {
                throw new System.Exception( "wrong parameter to function fact()" );
            }// else continue.
            //
            ulong numeratore = factUlong( n );
            ulong denominatore = factUlong( k ) * factUlong( n - k );
            //
            return numeratore / denominatore;
        }

        /// <summary>
        /// cardinalita delle combinazioni senza ripetizione, di "n" elementi, presi "k" a "k" (k_smallerOrEqual_n)
        /// </summary>
        /// <param name="n">totale elementi</param>
        /// <param name="k">cardinalita del raggruppamento </param>
        /// <returns></returns>
        public int binomioNewton( int n, int k )
        {
            if ( k > n )
            {
                throw new System.Exception( "wrong parameter to function fact()" );
            }// else continue.
            //
            int numeratore = fact( n );
            int denominatore = fact( k ) * fact( n - k );
            //
            return numeratore / denominatore;
        }

        #endregion factorial






        #endregion Rango


        #region cantinaExMetodiSviluppo



        ///// <summary>
        ///// per una data matrice_nxn trova tutte le n^2 sottomatrici (n-1)x(n-1).
        ///// </summary>
        ///// <param name="quadrataAnalizzanda"></param>
        //private void sottomatriciIsoDimensionali( LinearAlgebra.RealMatrix quadrataAnalizzanda )
        //{
        //    //rangoQuadrata_recursion_ordinal++;
        //    //System.Console.WriteLine( " sottomatriciIsoDimensionali_ordinal= {0}", rangoQuadrata_recursion_ordinal );
        //    //
        //    for ( int curRow=0; curRow < quadrataAnalizzanda.Get_rows; curRow++ )
        //    {
        //        for ( int curCol=0; curCol < quadrataAnalizzanda.Get_cols; curCol++ )
        //        {
        //            LinearAlgebra.RealMatrix subMatrix = quadrataAnalizzanda.reductor( curRow, curCol );// current SubMatrix
        //            System.Console.WriteLine( " dimensione sottomatrice= {0}", subMatrix.Get_rows );
        //            System.Console.WriteLine( " linee eliminate row, col = {0},{1}", curRow, curCol );
        //            subMatrix.show();
        //            //
        //            if ( 0 == subMatrix.Get_rows )// size one matrix is a scalar;
        //            {// zero size matrix is no-number.
        //                return;
        //            }// avoid an exception, due to zero-sized matrix.
        //            //
        //        }// end for Get_cols
        //    }// end for Get_rows
        //    //
        //    // ready
        //}// end sottomatriciIsoDimensionali()




        ///// <summary>
        ///// public only within test phase.
        ///// analizza un vettore di indici di colonna e risponde bool, se va lavorato o scartato.
        ///// Lo scarto avviene per (almeno)una delle seguenti condizioni:
        /////     -   sono presenti indici ripetuti, nell'elenco delle colonne
        /////     -   l'elenco delle colonne e' permutazione di un altro elenco presente in archivio.
        ///// Al fine di poter fare la seconda di queste valutazioni, viene conservato un archivio dei vettori di indici lavorati. Ovvero, quando si fa il determinante della
        ///// matrice di colonne{1,2,3] viene archiviato il vettore{1,2,3}. Risulta opportuno che tale archivio sia una variabile di istanza.
        ///// </summary>
        ///// <returns></returns>
        //public bool selettore( int[] esaminato )
        //{
        //    bool result = false;
        //    //
        //    for ( int c=0; c < this.RankSequence.Length; c++ )
        //    {
        //        if ( ePermutazioneDi( esaminato, this.RankSequence[c].columnIndexes ) )
        //        {
        //            result = false;
        //            break;
        //        }// end if
        //    }// end for each record in "diario".
        //    // ready.
        //    return result;
        //}// end selettore



        ///// <summary>
        ///// analizza un vettore di indici di colonna e risponde bool, se e' permutazione di un termine di paragone( prototipo).
        ///// </summary>
        ///// <param name="esaminato"></param>
        ///// <param name="prototipo"></param>
        ///// <returns></returns>
        //public  bool ePermutazioneDi( int[] esaminato, int[] prototipo )
        //{
        //    bool isPermutationOf = true;// NB. boolean in "and". Needs init to "true"; at the first "false", the hypothesis will be false.
        //    int commonLength;
        //    if ( esaminato.Length == prototipo.Length )
        //    {
        //        commonLength = esaminato.Length;//==protoripo.Length.
        //    }
        //    else
        //    {
        //        throw new System.Exception( "Sequenze di indici incompatibili. Debug richiesto." );
        //    }// end if.
        //    //
        //    for ( int c=0; c < commonLength; c++ )
        //    {// NB. boolean in "and".  Needs init to "true"; at the first "false", the hypothesis will be false.
        //        isPermutationOf &= appartieneA( esaminato[c], prototipo );// match each esaminato[element] to all of the prototipo[].
        //        if ( !isPermutationOf )
        //        {
        //            break;// found a non-common element-> not a permutation -> no need to go on.
        //        }// else continue.
        //    }// end for.
        //    // ready.
        //    return isPermutationOf;
        //}// end ePermutazioneDi


        ///// <summary>
        ///// funzione elementare, sulla quale si itera per valutare l'apaprteneza di un singolo indice di colonna, ad un insieme di indici.
        ///// </summary>
        ///// <param name="esaminato"></param>
        ///// <param name="prototipo"></param>
        ///// <returns></returns>
        //public  bool appartieneA( int esaminato, int[] prototipo )
        //{
        //    bool isMemberOf = false;
        //    //
        //    for ( int c=0; c < prototipo.Length; c++ )
        //    {
        //        if ( prototipo[c] == esaminato )
        //        {
        //            isMemberOf = true;
        //            break;// no need to go on.
        //        }// else still false;
        //    }// end for.
        //    //ready.
        //    return isMemberOf;
        //}// end appartieneA

        ///// <summary>
        ///// helper
        ///// </summary>
        ///// <param name="curDimension"></param>
        //public void lettoreRisultati( System.Collections.ArrayList curDimension )
        //{
        //    SmallerDet curElement;
        //    try
        //    {
        //        //
        //        for ( int c=0; c < curDimension.Count; c++ )
        //        {
        //            curElement = ( ( SmallerDet )( curDimension[c] ) );
        //            System.Console.WriteLine( " current dimension={0} determinant={1}", curElement.SubMatrixSize, curElement.Det );
        //        }// end for each element in curDimension.
        //    }
        //    catch ( System.Exception ex )
        //    {
        //        string msg = ex.Message;
        //    }
        //    finally
        //    {
        //    }
        //}// end lettoreRisultati


        #endregion cantinaExMetodiSviluppo



    }// end class

}// end nmsp
