//#define want_Map_print
#define want_ImageSequence_print
#define want_Polydromical_PseudoAnteImages

using System;
using System.Collections.Generic;
using System.Text;



namespace RealField
{


    /// <summary>
    /// This class is intended to implement the concept( taught by Karl Weierstrass) of a continuous function, defined on a compact set.
    /// Such function has a minimum a maximum ad reaches all intermediate values in between.
    /// As a consequence of this, the image set is a compact itself.
    /// These assertions will be proved by means of sequences, due to the fact that every bounded sequence admits a converging sub-sequence.
    /// The numerical aspect of the procedure is based on the floating point comparison and appropriate choice of the sequence.
    /// </summary>
    public class WeierstrassContinuity
    {

        #region data

        private class PointCouple
        {
            public double abscissa;
            public double ordinate;
            //
            public PointCouple( double abscissa, double ordinate )
            {
                this.abscissa = abscissa;
                this.ordinate = ordinate;
            }// end Ctor
        }// end nested class PointCouple

        private class PseudoAnteImages
        {
            public PointCouple ImageSequencePoint;
            public System.Collections.ArrayList PseudoAnteImagesCollection;
            //
            public PseudoAnteImages(PointCouple currentImageSequencePoint)
            {
                this.ImageSequencePoint = currentImageSequencePoint;
            }// end Ctor
        }// end nested class PseudoAnteImages

        public enum ImageSequenceChoice
        {
            McLaurinExp = 0,
            Sequence_E =1,
            EquiStep=2,
            SequenceEngineMinusLogOneMinusX=3,
            TODO=4
        }//end  Enum ImageSequenceChoice.

        private PointCouple[] Map;// map of {x,f(x)}
        private int MapCardinality;// point cardinality in function map.
        //
        private PointCouple[] ImageSequence;//  yn
        private int ImageSequenceCardinality;// point cardinality in image sequence.
        private ImageSequenceChoice ImageSequenceChosen;
        private string ImageSequenceDescription = null;
        //
        private PseudoAnteImages[] DomainSequence;// xn
        // Cannot specify its cardinality: it depends on how many Map-points will result to have an image, near enough to each point in the Image Sequence.
        // It can go from noone to many; for each (yn).
        //
        private PointCouple[] Domain_Sub_Sequence;// xn*
        // It has the same cardinality as the ImageSequence. Except when the procedure doesn't find any similar pseudo-ante-image. This last case is just a matter of Numerical Analysis.
        // No continuous function has holes. But we are mapping a C0 function with a finite DeltaX, and with machine-precision rational numbers; so holes come out.
        //
        private double A, B;// domain extrema.
        private double stepDx;
        private double toleranceInComparisonPseudoAnteImage;
        //
        public delegate double Functional_form( double x );// this is a type declaration, for a funcion pointer. Such type have to be public, to be referred to from the caller.
        public delegate double SequenceModelXK( double x, double k );// this is a type declaration, for a funcion pointer. Such type have to be public, to be referred to from the caller.
        private Functional_form currentFunctionalForm;// this is the instance to the currently used delegate, for the continuous function, between compact sets.
        private SequenceModelXK currentImageSequence;// this is the instance to the currently used delegate, for the {yn} sequence, in the Image.
        private double firstParamInImageSeq_NotIndex;// multi_purpose variable; in McLaurinExp means x in x^k/k! ; in (1+alpha/m)^n means alpha. In general it's the first param in f(x,n).
        //
        private double targetPointInImage;
        #endregion data

        #region Ctor & Manager




        /// <summary>
        /// Ctor: the domain must be specified here.
        /// </summary>
        public WeierstrassContinuity(
            double a, double b, int RequestedMapCardinality,
            Functional_form the_continuousFunction,
            double targetPointInImage,
            int howManyImageSequenceElements,
            ImageSequenceChoice the_imageSequence,
            double par_toleranceInComparisonPseudoAnteImage
            )
        {
            if ( this.isTheSame( a, b ) )// a==b
            {
                throw new System.Exception( "Domain: empty interval." );
            }// else can continue.
            //
            this.currentFunctionalForm = the_continuousFunction;
            //
            this.MapCardinality = RequestedMapCardinality;
            //
            if ( a < b )
            {
                this.A = a;
                this.B = b;
            }
            else if ( a > b )
            {
                this.A = b;
                this.B = a;
            }
            //
            this.targetPointInImage = targetPointInImage;
            this.ImageSequenceCardinality = howManyImageSequenceElements;// decide how many points, in image sequence.
            this.ImageSequenceChosen = the_imageSequence;// the method this.Sequencemanager will select the appropriate things to do, due to this choice.
            this.toleranceInComparisonPseudoAnteImage = par_toleranceInComparisonPseudoAnteImage;// the tolerance in comparison yn=?=f(xn).
        }// end Ctor


        private double Image_min_finder()
        {
            double res = 0.0;
            //
            for ( int c=0; c < this.MapCardinality; c++ )
            {
                if ( this.Map[c].ordinate < res )
                {
                    res = this.Map[c].ordinate;
                }// else skip
            }// end for.
            // ready
            return res;
        }// end Image_min_finder()

        private double Image_max_finder()
        {
            double res = 0.0;
            //
            for ( int c=0; c < this.MapCardinality; c++ )
            {
                if ( this.Map[c].ordinate > res )
                {
                    res = this.Map[c].ordinate;
                }// else skip
            }// end for.
            // ready
            return res;
        }// end Image_max_finder()



        public bool SequenceManager()
        {
            try
            {
                this.MapProducer();//--------- produce the map {x,f(x)}------------------
                //-----------------------------------------------------------------------------------------------------------------------------------------------------------------------
                switch ( this.ImageSequenceChosen )
                {
                    // this switch implements the ImageSequenceProducer for each specific ImageSequence chosen.
                    case ImageSequenceChoice.McLaurinExp:
                        {
                            RealField.AvailableAlgorithms.SequenceEngine_McLaurinExp seqDefault_McLaurinExp = new AvailableAlgorithms.SequenceEngine_McLaurinExp();
                            this.currentImageSequence = new SequenceModelXK( seqDefault_McLaurinExp.f_SequenceEngine_McLaurinExp );
                            ImageSequenceDescription = this.currentImageSequence.Target.ToString();
                            this.firstParamInImageSeq_NotIndex = Math.Log( targetPointInImage );// NB. for method McLaurinExp(), it means which x in x^k/k!
                            this.ImageSequence = new PointCouple[this.ImageSequenceCardinality];
                            for ( int c=0; c < this.ImageSequenceCardinality; c++ )
                            {
                                this.ImageSequence[c] = new PointCouple( c, this.currentImageSequence( this.firstParamInImageSeq_NotIndex, c ) );//point to ImageSequence, and point in Img.
                            }
                            break;
                        }//end case ImageSequenceChoice.McLaurinExp:
                    case ImageSequenceChoice.Sequence_E:
                        {
                            
                            RealField.AvailableAlgorithms.SequenceEngine_E seqOneplusOneOnNatN = new AvailableAlgorithms.SequenceEngine_E();
                            this.currentImageSequence = new SequenceModelXK( seqOneplusOneOnNatN.f_SequenceEngine_E );
                            ImageSequenceDescription = this.currentImageSequence.Target.ToString();
                            this.firstParamInImageSeq_NotIndex = Math.Log( targetPointInImage );// NB. for method (1+alpha/n)^n it means alpha -> goes to Exp[alpha].
                            this.ImageSequence = new PointCouple[this.ImageSequenceCardinality];
                            for ( int c=0; c < this.ImageSequenceCardinality; c++ )
                            {
                                this.ImageSequence[c] = new PointCouple( c, this.currentImageSequence( this.firstParamInImageSeq_NotIndex, c ) );//point to ImageSequence, and point in Img.
                            }
                            break;
                        }//end case ImageSequenceChoice.Sequence_E:
                    case ImageSequenceChoice.EquiStep:
                        {
                            this.ImageSequenceDescription = "current function is SequenceEngine_EquiStep: Image_[a,b] sequence== a + i*(b-a)/n  ,{i,0,n}";
                            double Image_min = Image_min_finder();//this.A, this.B,//----NO--!!!! questi sono gli estremi nel dominio !!!!
                            double Image_max = Image_max_finder();
                            RealField.AvailableAlgorithms.SequenceEngine_EquiStep seq_EquiStep =
                                new AvailableAlgorithms.SequenceEngine_EquiStep(
                                    //this.A, this.B,//----NO--!!!! questi sono gli estremi nel dominio !!!!
                                    Image_min, Image_max,
                                    this.ImageSequenceCardinality );
                            this.ImageSequence = new PointCouple[this.ImageSequenceCardinality];
                            for ( int i=0; i < this.ImageSequenceCardinality; i++ )
                            {
                                this.ImageSequence[i] = new PointCouple( i, seq_EquiStep.f_SequenceEngine_EquiStep( i ) );// { i, a+i*(b-a)/n }
                            }
                            break;
                        }//end case  ImageSequenceChoice.EquiStep:
                    case ImageSequenceChoice.SequenceEngineMinusLogOneMinusX:
                        {
                            RealField.AvailableAlgorithms.SequenceEngineMinusLogOneMinusX inst_MinusLogOneMinusX = new AvailableAlgorithms.SequenceEngineMinusLogOneMinusX();
                            this.currentImageSequence = new SequenceModelXK( inst_MinusLogOneMinusX.f_SequenceEngineMinusLogOneMinusX );
                            ImageSequenceDescription = this.currentImageSequence.Target.ToString();
                            this.firstParamInImageSeq_NotIndex = +1.0 - Math.Exp( -targetPointInImage );// Pb. targetPointInImage==-Log(1-x) -> x==1-Exp(-x).
                            this.ImageSequence = new PointCouple[this.ImageSequenceCardinality];
                            for ( int c=0; c < this.ImageSequenceCardinality; c++ )
                            {
                                this.ImageSequence[c] = new PointCouple( c, this.currentImageSequence( this.firstParamInImageSeq_NotIndex, c ) );//point to ImageSequence, and point in Img.
                            }
                            break;
                        }
                    case ImageSequenceChoice.TODO:
                    default:
                        {
                            throw new System.Exception( " TODO : read the code and decide which Image Sequence to use; consequently choose the initial point {x0}." );
                            // break; implicit.
                        }//end case ImageSequenceChoice.McLaurinExp:
                }//end switch ( this.ImageSequenceChosen )
                //-----------------------------------------------------------------------------------------------------------------------------------------------------------------------
                this.DomainSequenceProducer();//-------------produce the sequence in the domain: as many points, as the points in image sequence, but polydromical-----------------------
                this.Domain_Sub_SequenceProducer();
                this.ShowOrder();//-----------------------------------------------------------------------------------print results-----------------
                //------------------------------
                //-------------ready------------
                return true;//-----if got here -> ok.
            }
            catch( System.Exception ex)
            {
                string msg = ex.Message;
                return false;//-----if got here -> NOT_ok.
            }
        }// end SequenceManager.

        #endregion Ctor & Manager

        #region Calculus

        /// <summary>
        /// Produces a sequence of R-point couples, consisting of (abscissam f(abscissa))==(abscissa, ordinate).
        /// Such couple-sequence is the tool to use the sequences in the image and in the domain, since we cannot ralate them through an inverse funcion, as there's no injevtivity guarantee.
        /// The first sequence will be produced to step into the image set, since we can do it monotonically. 
        /// If we started from the domain, the possible lack of injectivity would let the sequence step back and forth through the image.
        /// Once that the image is stepped throug, we need the anteimage of each step.No inverse function is available.
        /// The strategy will be to:
        ///     - select the point from the image sequence
        ///     - step into the Map, to find all couples that have image points adequately close to the selected image point
        ///     - between all these couples, select one and only one ante-image( always with the same criterion. eg. the first, etc..). This is the sub-sequence extraction.
        ///     - find a sequence that steps through the domain
        ///     - extract only the indexes, where the domain element is adequately near to the domain sequence point. We finally have the equation of a sequence that converges to an extremant.
        ///     - the sequence that converges to an extremum is the image sequence; we can do one of it for the maximum; one for the minimum and one for each intermediate value.
        /// </summary>
        /// <returns></returns>
        private PointCouple[] MapProducer()
        {
            this.stepDx = ( this.B - this.A ) / ( this.MapCardinality - 1 );// n_points -> (n-1)_stepsDx.
            this.Map = new PointCouple[ this.MapCardinality];
            //
            for ( int c=0; c < this.MapCardinality; c++ )
            {
                this.Map[c] = new PointCouple( this.A + c * this.stepDx, this.currentFunctionalForm( this.A + c * this.stepDx ) );
            }// end for
            //
            return this.Map;
        }// end MapProducer




        private PseudoAnteImages[] DomainSequenceProducer()
        {
            this.DomainSequence = new PseudoAnteImages[this.ImageSequenceCardinality];// DomainSequence has rank one.
            //
            for ( int c=0; c < this.ImageSequence.Length; c++ )
            {
                this.DomainSequence[c] = new PseudoAnteImages( this.ImageSequence[c]);// just get a copy, to associate with its pseudo ante images.
                this.DomainSequence[c].PseudoAnteImagesCollection = new System.Collections.ArrayList();// prepare an empty collection, for all the potential pseudo ante images.
                //---now fill it up, with its pseudo ante images.
                this.GetNearAnteImages( this.DomainSequence[c] );// find xn whose f(xn)==yn.
            }// end for each image sequence point.
            // ready.
            return this.DomainSequence;
        }// end DomainSequenceProducer


        /// <summary>
        /// Called with the generic point yn, member of {yn} ImageSequence.
        /// It responds with an array of {xn}(equipped with the associated {n,yn}), whose images are near to yn.
        /// From such {xn} arguments, will be extracted the one responding to the criterion, devoted to give back monodromy, to the pseudo-inverse.
        /// </summary>
        /// <param name="yn"></param>
        /// <returns></returns>
        private PseudoAnteImages GetNearAnteImages( PseudoAnteImages currentImageSequenceElement )
        {
            for ( int c=0; c < this.MapCardinality; c++ )
            {
                if ( this.AreNear( this.Map[c].ordinate, currentImageSequenceElement.ImageSequencePoint.ordinate ) )
                {
                    currentImageSequenceElement.PseudoAnteImagesCollection.Add( this.Map[c].abscissa );// just found a suitable xn, for the current yn: save it!
                }// else skip this step, in the Map of the analyzed function.
            }// end for each step in the Map of the analyzed function.
            // ready.
            return currentImageSequenceElement;
        }// end GetNearAnteImages


        /// <summary>
        /// get multiple pseudo ante images, from non monothonical function, which has a non monodromical pseudo inverse.
        /// </summary>
        /// <param name="fxn"></param>
        /// <param name="yn"></param>
        /// <returns></returns>
        private bool AreNear( double fxn, double yn )
        {
            if ( System.Math.Abs( fxn - yn ) < this.toleranceInComparisonPseudoAnteImage )
            {
                return true;// pseudo ante image found -> return.
            }// else
            return false;// not a suitable pseudo ante image.
        }// end AreNear


        /// <summary>
        /// extracts a converging sub-sequence, from a limited sequence.
        /// Whenever a sequence is bounded in a compact set( on a complete field), there's (at least) one converging extracted sub sequence (there are many).
        /// </summary>
        /// <returns></returns>
        private PointCouple[] Domain_Sub_SequenceProducer()  // xn*
        {
            // operating sequence:
            //-------------------------------------
            // scan the {xn} sequence
            // extract the first of the pseudo-inverses, for each element.
            // this extraction associates (xn*) to (n), in the following way: (n)->(yn)->{xn1,...,xnk}--->(xn*). This subsequence has cardinality==ImageSequenceCardinality, in this context
            // where we approximate infinite sequences, with finite ones.
            //
            this.Domain_Sub_Sequence = new PointCouple[this.ImageSequenceCardinality];// as many as the {yn}.
            //
            for ( int imageSequenceElement=0; imageSequenceElement < this.ImageSequenceCardinality; imageSequenceElement++ )
            {
                if ( 0 < this.DomainSequence[imageSequenceElement].PseudoAnteImagesCollection.Count )// se esistono pseudoanteimmagini
                {// estrai la sottosuccessione, prendendo la prima( i.e. quella per x minimo).
                    this.Domain_Sub_Sequence[imageSequenceElement] = new PointCouple(
                        imageSequenceElement
                        , (double)(this.DomainSequence[imageSequenceElement].PseudoAnteImagesCollection[0])//  prendendo la prima( i.e. quella per x minimo).
                    );
                }// else no psudo ante image to show
                else
                {
                    this.Domain_Sub_Sequence[imageSequenceElement] = new PointCouple(
                        imageSequenceElement
                        , double.NaN // due to Numerical Analysis reasons, there's a "machine-hole" here.
                    );
                }//
            }// end for each imageSequenceElement.
            //
            return this.Domain_Sub_Sequence;
        }// end Domain_Sub_SequenceProducer()




        ///// <summary>
        ///// now devoted to the sequence McLaurin of Exp: centered in zero, calculated in 1. Converges to Exp[1]==E.
        ///// Sono ridotte all'ordine "seed", della serie sopracitata.
        ///// </summary>
        ///// <param name="seed"></param>
        ///// <returns></returns>
        //public double SequenceEngine_McLaurinExp( int seed )// TODO test and rename : function pointer to_which_sequence_engine; treat target==0 case.
        //{
        //    double res = 0.0;
        //    //
        //    for ( int c=0; c <= seed; c++ )
        //    {
        //        res += 1.0 / this.Factorial( c );
        //    }
        //    //
        //    return res;
        //}// end SequenceEngine


        //public double SequenceEngine_E( int enne )// TODO test and rename
        //{
        //    double res = 0.0;
        //    //
        //    res = Math.Pow( 1.0 + 1.0 / enne, enne );
        //    //
        //    return res;
        //}// end SequenceEngine


        ///// <summary>
        ///// fattoriale : NB. il tipo int32 consente di usarla fino a 16 (no more!).
        ///// </summary>
        ///// <param name="n"></param>
        ///// <returns></returns>
        //private int Factorial( int n )
        //{
        //    if ( n < 0 )
        //    {
        //        throw new System.Exception( "factorial available here, only for nonnegative integers." );
        //    }// else continue.
        //    //
        //    int theFact = +1;// will grow by product.
        //    for ( int k=n; k > 1; k-- )
        //    {
        //        theFact *= k;
        //    }// for.
        //    //ready.
        //    return theFact;
        //}// end fact



        /// <summary>
        /// evaluates if two real points are to be considered the same one, with respect to a given Comparison-Threshold.
        /// </summary>
        /// <param name="v"></param>
        /// <param name="w"></param>
        /// <returns></returns>
        private bool isTheSame( double v, double w )
        {
            double ComparisonThreshold = +1.0E-81;
            if ( Math.Abs( v - w ) < ComparisonThreshold )
            {
                return true;
            }
            // else
            return false;
        }// end isTheSame


        #endregion Calculus


        #region Console Print

        private void Show_Not_Order()
        {
        }// end Show_Not_Order


        private void ShowOrder()
        {
            //
            System.Console.WriteLine( "\n\t domain: {0}  {1} ", this.A, this.B );
            System.Console.WriteLine( "\n\t continuous function: {0} ", this.currentFunctionalForm.Target.ToString() );
            System.Console.WriteLine( "\n\t function map cardinality = {0} ", this.MapCardinality );
            System.Console.WriteLine( "\n\n------------------------------------------------------------------------\n" );
            System.Console.WriteLine( "\n\t desired image point {0} ", this.targetPointInImage );
            if ( null != this.ImageSequenceDescription )
            {
                System.Console.WriteLine( "\n\t image sequence: {0} ", this.ImageSequenceDescription );
            }
            else
            {
                System.Console.WriteLine( "\n\t {0}", "no description available, for current image sequence." );
            }
            System.Console.WriteLine( "\n\t image sequence cardinality = {0} ", this.ImageSequenceCardinality );
            System.Console.WriteLine( "\n\n\n" );
            ////
            //
            //
            //
            //
            //
            //
            #if want_Map_print
            double n = this.Map.Length / this.Map.Rank;//-----------------------------show map-------------------------
            System.Console.WriteLine( "\n\t -------------Map Show--------------------------------------------");
            for ( int c=0; c < n; c++ )
            {
                System.Console.WriteLine( "\n\t ({0}) __  {1}\t{2}", c+1, this.Map[c].abscissa, this.Map[c].ordinate );
            }
            System.Console.WriteLine( "\n\t ------END-------Map Show--------------------------------------------" );
            System.Console.WriteLine( "\n\n" );
            //------end-----------------------------------------------------------show map-------------------------
            #endif
            //
            //
            //
            //
            //
            //
            //
            //
            //
            #if  want_ImageSequence_print
            //---------------------------------------------show image sequence---------------------
            System.Console.WriteLine( "\n\n\t -------------ImageSequence Show--------------------------------------------" );
            for ( int c=0; c < this.ImageSequence.Length; c++ )
            {
                System.Console.WriteLine( "\n\t ({0}) __  {1}\t{2}", c + 1, this.ImageSequence[c].abscissa, this.ImageSequence[c].ordinate );
            }
            System.Console.WriteLine( "\n\t ------END--------ImageSequence Show-----------------------------------------------" );
            System.Console.WriteLine( "\n\n" );
            //----------------------------end-------show image sequence---------------------
            #endif
            //
            //
            //
            //
            //
            //
            #if want_Polydromical_PseudoAnteImages
            //------------------------------------print Ante images---------------------------
            System.Console.WriteLine( "\n\t -------------print Ante images--------------------------------------------" );
            this.printAllPseudoAnteImages();
            System.Console.WriteLine( "\n\t ------END--------print Ante images----------------------------------------------" );
            System.Console.WriteLine( "\n\n" );
            //----------END--------------------------print Ante images---------------------------
            #endif
            //
            //
            //
            //
            //
            //----------------------------------printDomain_Sub_Sequence------------------------
            System.Console.WriteLine( "\n\t -----------printDomain_Sub_Sequence----------------------------------------" );
            this.printDomain_Sub_Sequence();
            System.Console.WriteLine( "\n\t ------END-------printDomain_Sub_Sequence----------------------------------------------" );
            System.Console.WriteLine( "\n\n" );
            //----------END----------------------printDomain_Sub_Sequence---------------------
            //
            //
            //
            //
            //
        }// end ShowOrder







        private void printFirstPseudoAnteImage()
        {
            //
            for ( int c=0; c < this.ImageSequenceCardinality; c++ )
            {
                System.Console.WriteLine( "\n\t n={0} yn={1}  "
                    , this.DomainSequence[c].ImageSequencePoint.abscissa
                    , this.DomainSequence[c].ImageSequencePoint.ordinate
                    );
                if ( 0 < this.DomainSequence[c].PseudoAnteImagesCollection.Count )
                {
                    System.Console.WriteLine( "\t xn0={0}", this.DomainSequence[c].PseudoAnteImagesCollection[0] );// print only the first one (i.e. [0]), if it exists.
                }// else no psudo ante image to show
                else
                {
                    System.Console.WriteLine( "\t  no psudo ante image to show" );
                }//
            }
        }// end printAllPseudoAnteImages


        private void printAllPseudoAnteImages()
        {
            //
            for ( int imageSequenceElement=0; imageSequenceElement < this.ImageSequenceCardinality; imageSequenceElement++ )
            {
                System.Console.WriteLine( "\n\t n={0} yn={1}  "
                    , this.DomainSequence[imageSequenceElement].ImageSequencePoint.abscissa
                    , this.DomainSequence[imageSequenceElement].ImageSequencePoint.ordinate
                    );
                if ( 0 < this.DomainSequence[imageSequenceElement].PseudoAnteImagesCollection.Count )
                {
                    for ( int whichPseudoAnteImage=0; whichPseudoAnteImage < this.DomainSequence[imageSequenceElement].PseudoAnteImagesCollection.Count; whichPseudoAnteImage++ )
                    {
                        System.Console.WriteLine( "\t xn{0}={1}  _ f(xn{0})=={2}  Abs[err]==dist( target_in_Image, f(xn{0}))=={3}"
                            , whichPseudoAnteImage
                            , this.DomainSequence[imageSequenceElement].PseudoAnteImagesCollection[whichPseudoAnteImage]
                            ,this.currentFunctionalForm( (double)(this.DomainSequence[imageSequenceElement].PseudoAnteImagesCollection[whichPseudoAnteImage]) )
                            , Math.Abs( this.targetPointInImage 
                                - this.currentFunctionalForm( ( double )( this.DomainSequence[imageSequenceElement].PseudoAnteImagesCollection[whichPseudoAnteImage] ) ) )
                          );// print each of them (i.e. [whichPseudoAnteImage])
                    }
                }// else no psudo ante image to show
                else
                {
                    System.Console.WriteLine( "\t  no psudo ante image to show" );
                }//
            }
        }// end printAllPseudoAnteImages


        /// <summary>
        /// la sottosuccessione estratta, da una successione limitata in un compatto, e' scelta per essere convergente ad un valore appartenente all'insieme compatto su un campo completo.
        /// </summary>
        private void printDomain_Sub_Sequence()
        {
            for ( int imageSequenceElement=0; imageSequenceElement < this.ImageSequenceCardinality; imageSequenceElement++ )
            {
                System.Console.WriteLine( "\n\t n={0} xn*==x{0}*={1}  _ f(xn*)==f(x{0}*)=={2} "
                    , this.Domain_Sub_Sequence[imageSequenceElement].abscissa
                    , this.Domain_Sub_Sequence[imageSequenceElement].ordinate
                    , this.currentFunctionalForm( this.Domain_Sub_Sequence[imageSequenceElement].ordinate )// it's called ordinate, but it's xn in the couple {n, xn*}.
                 );
            }
        }// end printDomain_Sub_Sequence()


        #endregion Console Print




    }// end class


}// end namespace RealField
