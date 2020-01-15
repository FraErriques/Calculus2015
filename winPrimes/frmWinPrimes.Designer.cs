namespace winPrimes
{
    partial class frmWinPrimes
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.menuStripMain = new System.Windows.Forms.MenuStrip();
            this.consultationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.availableThresholdToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.readPrimeAtOrdinalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.readRangeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dataProductionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.enrichCollectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stopCalculationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.calculationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dirichletSeriesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.eulerProductToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.eulerRiemannEquationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.riemannZetaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.logIntegralToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.percentileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.isolatedNaturalEvaluationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stopAllCalculationThreadsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.checkLogToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearBoardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dbToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dbenrichCollectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dbstopCalculationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dbavailableThresholdToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dbreadPrimeAtOrdinalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dbreadRangeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.txtBoard = new System.Windows.Forms.TextBox();
            this.factorFinderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStripMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStripMain
            // 
            this.menuStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.consultationToolStripMenuItem,
            this.dataProductionToolStripMenuItem,
            this.calculationToolStripMenuItem,
            this.dbToolStripMenuItem});
            this.menuStripMain.Location = new System.Drawing.Point(0, 0);
            this.menuStripMain.Name = "menuStripMain";
            this.menuStripMain.Size = new System.Drawing.Size(627, 24);
            this.menuStripMain.TabIndex = 0;
            this.menuStripMain.Text = "menuStripMain";
            // 
            // consultationToolStripMenuItem
            // 
            this.consultationToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.availableThresholdToolStripMenuItem,
            this.readPrimeAtOrdinalToolStripMenuItem,
            this.readRangeToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.consultationToolStripMenuItem.Name = "consultationToolStripMenuItem";
            this.consultationToolStripMenuItem.Size = new System.Drawing.Size(132, 20);
            this.consultationToolStripMenuItem.Text = "file-system consultation";
            // 
            // availableThresholdToolStripMenuItem
            // 
            this.availableThresholdToolStripMenuItem.Name = "availableThresholdToolStripMenuItem";
            this.availableThresholdToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.availableThresholdToolStripMenuItem.Text = "available threshold";
            this.availableThresholdToolStripMenuItem.Click += new System.EventHandler(this.mnuAvailableThreshold_Click);
            // 
            // readPrimeAtOrdinalToolStripMenuItem
            // 
            this.readPrimeAtOrdinalToolStripMenuItem.Name = "readPrimeAtOrdinalToolStripMenuItem";
            this.readPrimeAtOrdinalToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.readPrimeAtOrdinalToolStripMenuItem.Text = "read prime at ordinal";
            this.readPrimeAtOrdinalToolStripMenuItem.Click += new System.EventHandler(this.mnuRead_Click);
            // 
            // readRangeToolStripMenuItem
            // 
            this.readRangeToolStripMenuItem.Name = "readRangeToolStripMenuItem";
            this.readRangeToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.readRangeToolStripMenuItem.Text = "read range";
            this.readRangeToolStripMenuItem.Click += new System.EventHandler(this.mnuReadRange_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.exitToolStripMenuItem.Text = "exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.mnuExit_Click);
            // 
            // dataProductionToolStripMenuItem
            // 
            this.dataProductionToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.enrichCollectionToolStripMenuItem,
            this.stopCalculationToolStripMenuItem});
            this.dataProductionToolStripMenuItem.Name = "dataProductionToolStripMenuItem";
            this.dataProductionToolStripMenuItem.Size = new System.Drawing.Size(150, 20);
            this.dataProductionToolStripMenuItem.Text = "file-system data production";
            // 
            // enrichCollectionToolStripMenuItem
            // 
            this.enrichCollectionToolStripMenuItem.Name = "enrichCollectionToolStripMenuItem";
            this.enrichCollectionToolStripMenuItem.Size = new System.Drawing.Size(240, 22);
            this.enrichCollectionToolStripMenuItem.Text = "enrich collection";
            this.enrichCollectionToolStripMenuItem.Click += new System.EventHandler(this.mnuEnrich_Click);
            // 
            // stopCalculationToolStripMenuItem
            // 
            this.stopCalculationToolStripMenuItem.Name = "stopCalculationToolStripMenuItem";
            this.stopCalculationToolStripMenuItem.Size = new System.Drawing.Size(240, 22);
            this.stopCalculationToolStripMenuItem.Text = "stop file-system data production";
            this.stopCalculationToolStripMenuItem.Click += new System.EventHandler(this.mnuStopCalculation_Click);
            // 
            // calculationToolStripMenuItem
            // 
            this.calculationToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.dirichletSeriesToolStripMenuItem,
            this.eulerProductToolStripMenuItem,
            this.eulerRiemannEquationToolStripMenuItem,
            this.riemannZetaToolStripMenuItem,
            this.logIntegralToolStripMenuItem,
            this.percentileToolStripMenuItem,
            this.isolatedNaturalEvaluationToolStripMenuItem,
            this.factorFinderToolStripMenuItem,
            this.stopAllCalculationThreadsToolStripMenuItem,
            this.checkLogToolStripMenuItem,
            this.clearBoardToolStripMenuItem});
            this.calculationToolStripMenuItem.Name = "calculationToolStripMenuItem";
            this.calculationToolStripMenuItem.Size = new System.Drawing.Size(69, 20);
            this.calculationToolStripMenuItem.Text = "calculation";
            // 
            // dirichletSeriesToolStripMenuItem
            // 
            this.dirichletSeriesToolStripMenuItem.Name = "dirichletSeriesToolStripMenuItem";
            this.dirichletSeriesToolStripMenuItem.Size = new System.Drawing.Size(215, 22);
            this.dirichletSeriesToolStripMenuItem.Text = "Dirichlet Series";
            this.dirichletSeriesToolStripMenuItem.Click += new System.EventHandler(this.dirichletSeriesToolStripMenuItem_Click);
            // 
            // eulerProductToolStripMenuItem
            // 
            this.eulerProductToolStripMenuItem.Name = "eulerProductToolStripMenuItem";
            this.eulerProductToolStripMenuItem.Size = new System.Drawing.Size(215, 22);
            this.eulerProductToolStripMenuItem.Text = "Euler  Product";
            this.eulerProductToolStripMenuItem.Click += new System.EventHandler(this.eulerProductToolStripMenuItem_Click);
            // 
            // eulerRiemannEquationToolStripMenuItem
            // 
            this.eulerRiemannEquationToolStripMenuItem.Name = "eulerRiemannEquationToolStripMenuItem";
            this.eulerRiemannEquationToolStripMenuItem.Size = new System.Drawing.Size(215, 22);
            this.eulerRiemannEquationToolStripMenuItem.Text = "Euler Riemann equation";
            this.eulerRiemannEquationToolStripMenuItem.Click += new System.EventHandler(this.mnuEulerRiemannEquation_Click);
            // 
            // riemannZetaToolStripMenuItem
            // 
            this.riemannZetaToolStripMenuItem.Name = "riemannZetaToolStripMenuItem";
            this.riemannZetaToolStripMenuItem.Size = new System.Drawing.Size(215, 22);
            this.riemannZetaToolStripMenuItem.Text = "Riemann  Zeta";
            this.riemannZetaToolStripMenuItem.Click += new System.EventHandler(this.riemannZetaToolStripMenuItem_Click);
            // 
            // logIntegralToolStripMenuItem
            // 
            this.logIntegralToolStripMenuItem.Name = "logIntegralToolStripMenuItem";
            this.logIntegralToolStripMenuItem.Size = new System.Drawing.Size(215, 22);
            this.logIntegralToolStripMenuItem.Text = "Log Integral on Real axis";
            this.logIntegralToolStripMenuItem.Click += new System.EventHandler(this.logIntegralToolStripMenuItem_Click);
            // 
            // percentileToolStripMenuItem
            // 
            this.percentileToolStripMenuItem.Name = "percentileToolStripMenuItem";
            this.percentileToolStripMenuItem.Size = new System.Drawing.Size(215, 22);
            this.percentileToolStripMenuItem.Text = "Percentile LogIntegral";
            this.percentileToolStripMenuItem.Click += new System.EventHandler(this.percentileToolStripMenuItem_Click);
            // 
            // isolatedNaturalEvaluationToolStripMenuItem
            // 
            this.isolatedNaturalEvaluationToolStripMenuItem.Name = "isolatedNaturalEvaluationToolStripMenuItem";
            this.isolatedNaturalEvaluationToolStripMenuItem.Size = new System.Drawing.Size(215, 22);
            this.isolatedNaturalEvaluationToolStripMenuItem.Text = "Isolated Natural evaluation";
            this.isolatedNaturalEvaluationToolStripMenuItem.Click += new System.EventHandler(this.isolatedNaturalEvaluationToolStripMenuItem_Click);
            // 
            // stopAllCalculationThreadsToolStripMenuItem
            // 
            this.stopAllCalculationThreadsToolStripMenuItem.Name = "stopAllCalculationThreadsToolStripMenuItem";
            this.stopAllCalculationThreadsToolStripMenuItem.Size = new System.Drawing.Size(215, 22);
            this.stopAllCalculationThreadsToolStripMenuItem.Text = "Stop all calculation threads";
            this.stopAllCalculationThreadsToolStripMenuItem.Click += new System.EventHandler(this.stopAllCalculationThreadsToolStripMenuItem_Click);
            // 
            // checkLogToolStripMenuItem
            // 
            this.checkLogToolStripMenuItem.Name = "checkLogToolStripMenuItem";
            this.checkLogToolStripMenuItem.Size = new System.Drawing.Size(215, 22);
            this.checkLogToolStripMenuItem.Text = "Check Log";
            this.checkLogToolStripMenuItem.Click += new System.EventHandler(this.checkLogToolStripMenuItem_Click);
            // 
            // clearBoardToolStripMenuItem
            // 
            this.clearBoardToolStripMenuItem.Name = "clearBoardToolStripMenuItem";
            this.clearBoardToolStripMenuItem.Size = new System.Drawing.Size(215, 22);
            this.clearBoardToolStripMenuItem.Text = "Clear Board";
            this.clearBoardToolStripMenuItem.Click += new System.EventHandler(this.clearBoardToolStripMenuItem_Click);
            // 
            // dbToolStripMenuItem
            // 
            this.dbToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.dbenrichCollectionToolStripMenuItem,
            this.dbstopCalculationToolStripMenuItem,
            this.dbavailableThresholdToolStripMenuItem,
            this.dbreadPrimeAtOrdinalToolStripMenuItem,
            this.dbreadRangeToolStripMenuItem});
            this.dbToolStripMenuItem.Name = "dbToolStripMenuItem";
            this.dbToolStripMenuItem.Size = new System.Drawing.Size(31, 20);
            this.dbToolStripMenuItem.Text = "db";
            // 
            // dbenrichCollectionToolStripMenuItem
            // 
            this.dbenrichCollectionToolStripMenuItem.Name = "dbenrichCollectionToolStripMenuItem";
            this.dbenrichCollectionToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.dbenrichCollectionToolStripMenuItem.Text = "enrich collection";
            this.dbenrichCollectionToolStripMenuItem.Click += new System.EventHandler(this.dbenrichCollectionToolStripMenuItem_Click);
            // 
            // dbstopCalculationToolStripMenuItem
            // 
            this.dbstopCalculationToolStripMenuItem.Name = "dbstopCalculationToolStripMenuItem";
            this.dbstopCalculationToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.dbstopCalculationToolStripMenuItem.Text = "stop calculation";
            this.dbstopCalculationToolStripMenuItem.Click += new System.EventHandler(this.dbstopCalculationToolStripMenuItem_Click);
            // 
            // dbavailableThresholdToolStripMenuItem
            // 
            this.dbavailableThresholdToolStripMenuItem.Name = "dbavailableThresholdToolStripMenuItem";
            this.dbavailableThresholdToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.dbavailableThresholdToolStripMenuItem.Text = "available threshold";
            this.dbavailableThresholdToolStripMenuItem.Click += new System.EventHandler(this.dbavailableThresholdToolStripMenuItem_Click);
            // 
            // dbreadPrimeAtOrdinalToolStripMenuItem
            // 
            this.dbreadPrimeAtOrdinalToolStripMenuItem.Name = "dbreadPrimeAtOrdinalToolStripMenuItem";
            this.dbreadPrimeAtOrdinalToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.dbreadPrimeAtOrdinalToolStripMenuItem.Text = "read prime at ordinal";
            this.dbreadPrimeAtOrdinalToolStripMenuItem.Click += new System.EventHandler(this.dbreadPrimeAtOrdinalToolStripMenuItem_Click);
            // 
            // dbreadRangeToolStripMenuItem
            // 
            this.dbreadRangeToolStripMenuItem.Name = "dbreadRangeToolStripMenuItem";
            this.dbreadRangeToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.dbreadRangeToolStripMenuItem.Text = "read range";
            this.dbreadRangeToolStripMenuItem.Click += new System.EventHandler(this.dbreadRangeToolStripMenuItem_Click);
            // 
            // txtBoard
            // 
            this.txtBoard.Location = new System.Drawing.Point(12, 36);
            this.txtBoard.MaxLength = 2097152;
            this.txtBoard.Multiline = true;
            this.txtBoard.Name = "txtBoard";
            this.txtBoard.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtBoard.Size = new System.Drawing.Size(539, 272);
            this.txtBoard.TabIndex = 1;
            this.txtBoard.WordWrap = false;
            // 
            // factorFinderToolStripMenuItem
            // 
            this.factorFinderToolStripMenuItem.Name = "factorFinderToolStripMenuItem";
            this.factorFinderToolStripMenuItem.Size = new System.Drawing.Size(215, 22);
            this.factorFinderToolStripMenuItem.Text = "Factor researcher";
            this.factorFinderToolStripMenuItem.Click += new System.EventHandler(this.factorFinderToolStripMenuItem_Click);
            // 
            // frmWinPrimes
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(627, 342);
            this.Controls.Add(this.txtBoard);
            this.Controls.Add(this.menuStripMain);
            this.MainMenuStrip = this.menuStripMain;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(635, 369);
            this.MinimumSize = new System.Drawing.Size(635, 369);
            this.Name = "frmWinPrimes";
            this.Text = "Prime Sequence";
            this.menuStripMain.ResumeLayout(false);
            this.menuStripMain.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStripMain;
        private System.Windows.Forms.TextBox txtBoard;
        private System.Windows.Forms.ToolStripMenuItem consultationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem dataProductionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem calculationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem readPrimeAtOrdinalToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem availableThresholdToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem readRangeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem enrichCollectionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem stopCalculationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem eulerRiemannEquationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem riemannZetaToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem eulerProductToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem logIntegralToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem dbToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem dbenrichCollectionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem dbstopCalculationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem isolatedNaturalEvaluationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem dbreadRangeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem dbavailableThresholdToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem dbreadPrimeAtOrdinalToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem stopAllCalculationThreadsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem percentileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem checkLogToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clearBoardToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem dirichletSeriesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem factorFinderToolStripMenuItem;
    }
}

