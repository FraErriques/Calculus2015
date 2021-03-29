namespace winPrimes
{
    partial class frmOrdinalAcquirer
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
            this.txtAcquirer = new System.Windows.Forms.TextBox();
            this.btnAcquirer = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txtAcquirer
            // 
            this.txtAcquirer.Location = new System.Drawing.Point(21, 12);
            this.txtAcquirer.Name = "txtAcquirer";
            this.txtAcquirer.Size = new System.Drawing.Size(240, 20);
            this.txtAcquirer.TabIndex = 0;
            // 
            // btnAcquirer
            // 
            this.btnAcquirer.Location = new System.Drawing.Point(171, 38);
            this.btnAcquirer.Name = "btnAcquirer";
            this.btnAcquirer.Size = new System.Drawing.Size(90, 23);
            this.btnAcquirer.TabIndex = 1;
            this.btnAcquirer.Text = "Go";
            this.btnAcquirer.UseVisualStyleBackColor = true;
            this.btnAcquirer.Click += new System.EventHandler(this.btnAcquirer_Click);
            // 
            // frmOrdinalAcquirer
            // 
            this.AcceptButton = this.btnAcquirer;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(269, 66);
            this.Controls.Add(this.btnAcquirer);
            this.Controls.Add(this.txtAcquirer);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(285, 105);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(285, 105);
            this.Name = "frmOrdinalAcquirer";
            this.Text = "Ordinal  Acquirer";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtAcquirer;
        private System.Windows.Forms.Button btnAcquirer;
    }
}