using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;

using System.Text;
using System.Windows.Forms;


namespace winPrimes
{


    public partial class frmWinPrimes : Form
    {


        /// <summary>
        /// Ctor
        /// </summary>
        public frmWinPrimes()
        {
            InitializeComponent();
        }




        public void writeSomeMessage(string theMessage)
        {
            this.txtBoard.Text += "\r\n" + theMessage;
        }//



        #region consultation

        private void mnuAvailableThreshold_Click(object sender, EventArgs e)
        {
            string board_message;
            bool canStartNewCalc = Process.DataProduction.checkWorkerThreadStatus(out board_message);
            this.txtBoard.Text += board_message;
            if (!canStartNewCalc)
            {
                return;// on the form.
            }
            else
            {
                this.txtBoard.Text += Process.Consultation.getAvailableThreshold();
            }
        }



        private void mnuRead_Click(object sender, EventArgs e)
        {
            string board_message;
            bool canStartNewCalc = Process.DataProduction.checkWorkerThreadStatus(out board_message);
            this.txtBoard.Text += board_message;
            if (!canStartNewCalc)
            {
                return;// on the form.
            }
            else
            {
                frmOrdinalAcquirer ordAcq = new frmOrdinalAcquirer();
                ordAcq.ShowDialog(this);
                // on re-entry
                this.txtBoard.Text += Process.Consultation.readAtSpecifiedOrdinal(ordAcq.theOrdinal);
            }
        }




        private void mnuReadRange_Click(object sender, EventArgs e)
        {
            string board_message;
            bool canStartNewCalc = Process.DataProduction.checkWorkerThreadStatus(out board_message);
            this.txtBoard.Text += board_message;
            if (!canStartNewCalc)
            {
                return;// on the form.
            }
            else
            {
                frmOrdinalAcquirer ordAcq = new frmOrdinalAcquirer();
                ordAcq.ShowDialog(this);
                // on re-entry
                Int64 min = ordAcq.theOrdinal;
                //
                ordAcq = null;
                ordAcq = new frmOrdinalAcquirer();
                ordAcq.ShowDialog(this);
                // on re-entry
                Int64 max = ordAcq.theOrdinal;
                //
                this.txtBoard.Text += Process.Consultation.readInOrdinalRange(min, max);
            }
        }//






        #endregion consultation





        #region dataproduction

        private void mnuEnrich_Click(object sender, EventArgs e)
        {
            string board_message;
            bool canStartNewCalc = Process.DataProduction.checkWorkerThreadStatus(out board_message);
            this.txtBoard.Text += board_message;
            if (!canStartNewCalc)
            {
                return;// on the form.
            }
            else
            {
                frmOrdinalAcquirer ordAcq = new frmOrdinalAcquirer();
                ordAcq.Text = "Threshold in Naturals";
                ordAcq.ShowDialog(this);
                //// on re-entry
                this.txtBoard.Text += Process.DataProduction.startCalculationThread( ordAcq.theOrdinal);
            }
        }//


        private void mnuStopCalculation_Click(object sender, EventArgs e)
        {
            try
            {
                this.txtBoard.Text += Process.DataProduction.voluntarilyStopCalculation();
            }
            catch (System.Exception ex)
            {// trap all. Tested that such Abort related exceptions do not harm the database.
                string s = ex.Message;//dbg
            }
        }//

        #endregion dataproduction





        #region calc

        /// <summary>
        /// threaded
        /// 
        /// compares Riemann sum to Euler product, for Re(s)>1.
        /// 
        /// TODO : implement for Re(s) smaller or equal to 1. Use analytic continuation.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mnuEulerRiemannEquation_Click(object sender, EventArgs e)
        {
            string board_message;
            bool canStartNewCalc = Process.DataProduction.checkWorkerThreadStatus(out board_message);
            this.txtBoard.Text += board_message;
            if (!canStartNewCalc)
            {
                return;// on the form.
            }
            else
            {
                frmComplexAcquirer ca = new frmComplexAcquirer();
                ca.ShowDialog();
                // on re-entry:
                double sigma = ca.Re;
                double t = ca.Im;
                this.txtBoard.Text += "\r\n " + sigma.ToString() + "  +i*  " + t.ToString();
                //
                frmOrdinalAcquirer oa = new frmOrdinalAcquirer();
                oa.Text = "Threshold in Naturals";
                oa.ShowDialog();
                // on re-entry:
                Int64 threshold = oa.theOrdinal;
                this.txtBoard.Text += "\r\n threshold in Naturals = " + threshold.ToString();
                //
                //---prepare params--------------
                Process.Calculation.CommonCalculationInput theInput = new Process.Calculation.CommonCalculationInput();
                theInput.sigma = sigma;
                theInput.t = t;
                theInput.naturalThreshold = threshold;
                // call Process
                Process.Calculation.CalcThreadManager(
                    theInput,
                    Process.Calculation.CalculationActions.EulerRiemannEquation,
                    new Process.Calculation.AsynchronousMessageWriterPtr(this.writeSomeMessage)
                ); // no more direct call to RiemannZetaSum()
                //
                //----this was let asynchronous----------------look for it on the log(s)-------------------


                //frmComplexAcquirer ca = new frmComplexAcquirer();
                //ca.ShowDialog();
                //// on re-entry:
                //double sigma = ca.Re;
                //double t = ca.Im;
                //this.txtBoard.Text += "\r\n " + sigma.ToString() + "  +i*  " + t.ToString();
                ////
                //frmOrdinalAcquirer oa = new frmOrdinalAcquirer();
                //oa.Text = "Threshold in Naturals";
                //oa.ShowDialog();
                //// on re-entry:
                //Int64 threshold = oa.theOrdinal;
                //this.txtBoard.Text += "\r\n threshold in Naturals = " + threshold.ToString();
                //// call Process
                //this.txtBoard.Text += Process.Calculation.EulerRiemannEquation(ca.Re, ca.Im, oa.theOrdinal);
            }
        }//



        private void riemannZetaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.txtBoard.Text += "\r\n TODO to be implemented.";
        }//


        private void dirichletSeriesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmComplexAcquirer ca = new frmComplexAcquirer();
            ca.ShowDialog();
            // on re-entry:
            double sigma = ca.Re;
            double t = ca.Im;
            this.txtBoard.Text += "\r\n " + sigma.ToString() + "  +i*  " + t.ToString();
            //
            frmOrdinalAcquirer oa = new frmOrdinalAcquirer();
            oa.Text = "Threshold in Naturals";
            oa.ShowDialog();
            // on re-entry:
            Int64 threshold = oa.theOrdinal;
            this.txtBoard.Text += "\r\n threshold in Naturals = " + threshold.ToString();
            //
            //---prepare params--------------
            Process.Calculation.CommonCalculationInput theInput = new Process.Calculation.CommonCalculationInput();
            theInput.sigma = sigma;
            theInput.t = t;
            theInput.naturalThreshold = threshold;
            // call Process
            Process.Calculation.CalcThreadManager(
                theInput,
                Process.Calculation.CalculationActions.DirichletSum,
                new Process.Calculation.AsynchronousMessageWriterPtr(this.writeSomeMessage)
            ); // no more direct call to RiemannZetaSum()
            //
            //----this was let asynchronous----------------look for it on the log(s)-------------------
        }


        /// <summary>
        /// threaded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void eulerProductToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string board_message;
            bool canStartNewCalc = Process.DataProduction.checkWorkerThreadStatus(out board_message);
            this.txtBoard.Text += board_message;
            if (!canStartNewCalc)
            {
                return;// on the form.
            }
            else
            {
                frmComplexAcquirer ca = new frmComplexAcquirer();
                ca.ShowDialog();
                // on re-entry:
                double sigma = ca.Re;
                double t = ca.Im;
                this.txtBoard.Text += "\r\n " + sigma.ToString() + "  +i*  " + t.ToString();
                //
                frmOrdinalAcquirer oa = new frmOrdinalAcquirer();
                oa.Text = "Threshold in Naturals";
                oa.ShowDialog();
                // on re-entry:
                Int64 threshold = oa.theOrdinal;
                this.txtBoard.Text += "\r\n threshold in Naturals = " + threshold.ToString();
                //
                //---prepare params--------------
                Process.Calculation.CommonCalculationInput theInput = new Process.Calculation.CommonCalculationInput();
                theInput.sigma = sigma;
                theInput.t = t;
                theInput.naturalThreshold = threshold;
                // call Process
                Process.Calculation.CalcThreadManager(
                    theInput,
                    Process.Calculation.CalculationActions.EulerProduct,
                    new Process.Calculation.AsynchronousMessageWriterPtr( this.writeSomeMessage)
                ); // no more direct call to EulerProduct()
                //
                //----this was let asynchronous----------------look for it on the log(s)-------------------
            }
        }//



        /// <summary>
        /// voluntarily not threaded.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void isolatedNaturalEvaluationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmOrdinalAcquirer ordAcq = new frmOrdinalAcquirer();
            ordAcq.Text = "Isolated Natural to be evaluated.";
            ordAcq.ShowDialog(this);
            // on re-entry
            Int64 theIsolatedNatural = ordAcq.theOrdinal;
            //
            this.txtBoard.Text += Process.Calculation.IsolatedNaturalEvaluation(theIsolatedNatural);
        }//


        /// <summary>
        /// voluntarily not threaded. Prime factor researcher.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void factorFinderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmOrdinalAcquirer ordAcq = new frmOrdinalAcquirer();
            ordAcq.Text = "Isolated Natural to be evaluated.";
            ordAcq.ShowDialog(this);
            // on re-entry
            Int64 theIsolatedNatural = ordAcq.theOrdinal;
            //
            this.txtBoard.Text += Process.Calculation.FactorResearcher( theIsolatedNatural);
        }





        /// <summary>
        /// threaded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void logIntegralToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.txtBoard.Text += "\r\n LogIntegral required. The Prime Number Theorem approximation is Integrate[dt/ln(t), {t,+2,x}] where x is N threshold.";
            frmOrdinalAcquirer ordAcq = new frmOrdinalAcquirer();
            ordAcq.Text = "Natural threshold.";
            ordAcq.ShowDialog(this);
            // on re-entry
            Int64 theNaturalThreshold = ordAcq.theOrdinal;
            //---prepare params--------------
            Process.Calculation.CommonCalculationInput theInput = new Process.Calculation.CommonCalculationInput();
            theInput.integrationDomain_sup = theNaturalThreshold;
            // call Process
            Process.Calculation.CalcThreadManager(
                theInput,
                Process.Calculation.CalculationActions.LogIntegralFrom2ToX,
                new Process.Calculation.AsynchronousMessageWriterPtr( this.writeSomeMessage)
            ); // no more direct call to  .LogIntegralFrom2toX(theNaturalThreshold);
            //
            //----this was let asynchronous----------------look for it on the log(s)-------------------
            //this.txtBoard.Text += String.Format(
            //    "\r\n Integrate[ dt/ln(t), ( t, +2, {0} ) ] = {1}", theNaturalThreshold.ToString(), theMeasure.ToString());
        }


        /// <summary>
        /// threaded.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void percentileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.txtBoard.Text += "\r\n Percentile LogIntegral required. It searches the threshold inf(x), such as Integrate[dt/ln(t), {t,+2,x}]>=k ";
            frmOrdinalAcquirer ordAcq = new frmOrdinalAcquirer();
            ordAcq.Text = " k|Integ[dt/ln(t),{t,2,x}]>=k ";
            ordAcq.ShowDialog(this);
            // on re-entry
            Int64 theNaturalThreshold = ordAcq.theOrdinal;
            //---prepare params--------------
            Process.Calculation.CommonCalculationInput theInput = new Process.Calculation.CommonCalculationInput();
            theInput.naturalThreshold = theNaturalThreshold;
            // call Process
            Process.Calculation.CalcThreadManager(
                theInput,
                Process.Calculation.CalculationActions.GetThresholdGivenCardinality,
                new Process.Calculation.AsynchronousMessageWriterPtr(this.writeSomeMessage)
            ); // no more direct call to  .LogIntegralFrom2toX(theNaturalThreshold);
            //
            //----this was let asynchronous----------------look for it on the log(s)-------------------
            //this.txtBoard.Text += String.Format(
            //    "\r\n Integrate[ dt/ln(t), ( t, +2, {0} ) ] = {1}", theNaturalThreshold.ToString(), theMeasure.ToString());
        }//


        /// <summary>
        /// each of them aborted, as many as are expected to be, in config.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void stopAllCalculationThreadsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.txtBoard.Text += "\r\n" + Process.Calculation.StopAllCalculationThreads();
        }


        private void checkLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.txtBoard.Text += "\r\n\r\n--------------------Log check------------------------------";
            this.txtBoard.Text += Process.Calculation.CheckLog();
            this.txtBoard.Text += "\r\n--------------------Log check------------------------------\r\n\r\n";
        }//


        private void clearBoardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.txtBoard.Text = "";
        }//


        #endregion calc






        private void mnuExit_Click(object sender, EventArgs e)
        {
            try
            {
                this.txtBoard.Text += Process.DataProduction.voluntarilyStopCalculation();
            }
            catch (System.Exception ex)
            {// trap all. Tested that such Abort related exceptions do not harm the database.
                string s = ex.Message;//dbg
            }
            this.Dispose();
        }//




        # region database


        private void dbenrichCollectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string board_message;
            bool canStartNewCalc = Process.db_DataProduction.db_checkWorkerThreadStatus( out board_message);
            this.txtBoard.Text += board_message;
            if (!canStartNewCalc)
            {
                return;// on the form.
            }
            else
            {
                frmOrdinalAcquirer ordAcq = new frmOrdinalAcquirer();
                ordAcq.Text = "Threshold";
                ordAcq.ShowDialog(this);
                //// on re-entry
                this.txtBoard.Text += Process.db_DataProduction.db_startCalculationThread( ordAcq.theOrdinal);
            }
        }//


        private void dbstopCalculationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Process.db_DataProduction.db_calculationThread.Suspend();
                this.txtBoard.Text += Process.db_DataProduction.db_voluntarilyStopCalculation( );
            }
            catch (System.Exception ex)
            {// trap all. Tested that such Abort related exceptions do not harm the database.
                string s = ex.Message;//dbg
            }
        }


        private void dbavailableThresholdToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.txtBoard.Text += Process.dbConsultation.getAvailableThreshold();
        }

        private void dbreadPrimeAtOrdinalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmOrdinalAcquirer ordAcq = new frmOrdinalAcquirer();
            ordAcq.ShowDialog(this);
            // on re-entry
            this.txtBoard.Text += Process.dbConsultation.readAtSpecifiedOrdinal( ordAcq.theOrdinal);
        }


        private void dbreadRangeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmOrdinalAcquirer ordAcq = new frmOrdinalAcquirer();
            ordAcq.ShowDialog(this);
            // on re-entry
            Int64 min = ordAcq.theOrdinal;
            //
            ordAcq = null;
            ordAcq = new frmOrdinalAcquirer();
            ordAcq.ShowDialog(this);
            // on re-entry
            Int64 max = ordAcq.theOrdinal;
            //
            this.txtBoard.Text += Process.dbConsultation.readInOrdinalRange( min, max);
        }




        # endregion database




    }//


}
