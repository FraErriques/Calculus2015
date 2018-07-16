namespace winPrimes
{
    partial class frmComplexAcquirer
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
            this.txtReal = new System.Windows.Forms.TextBox();
            this.txtImmaginary = new System.Windows.Forms.TextBox();
            this.btnSubmit = new System.Windows.Forms.Button();
            this.lblReal = new System.Windows.Forms.Label();
            this.lblImmaginary = new System.Windows.Forms.Label();
            this.lblJoint = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // txtReal
            // 
            this.txtReal.Location = new System.Drawing.Point(10, 28);
            this.txtReal.Name = "txtReal";
            this.txtReal.Size = new System.Drawing.Size(160, 20);
            this.txtReal.TabIndex = 0;
            // 
            // txtImmaginary
            // 
            this.txtImmaginary.Location = new System.Drawing.Point(210, 28);
            this.txtImmaginary.Name = "txtImmaginary";
            this.txtImmaginary.Size = new System.Drawing.Size(160, 20);
            this.txtImmaginary.TabIndex = 1;
            // 
            // btnSubmit
            // 
            this.btnSubmit.Location = new System.Drawing.Point(295, 54);
            this.btnSubmit.Name = "btnSubmit";
            this.btnSubmit.Size = new System.Drawing.Size(75, 23);
            this.btnSubmit.TabIndex = 2;
            this.btnSubmit.Text = "Go";
            this.btnSubmit.UseVisualStyleBackColor = true;
            this.btnSubmit.Click += new System.EventHandler(this.btnSubmit_Click);
            // 
            // lblReal
            // 
            this.lblReal.AutoSize = true;
            this.lblReal.Location = new System.Drawing.Point(10, 9);
            this.lblReal.Name = "lblReal";
            this.lblReal.Size = new System.Drawing.Size(65, 13);
            this.lblReal.TabIndex = 3;
            this.lblReal.Text = "sigma=Re(s)";
            // 
            // lblImmaginary
            // 
            this.lblImmaginary.AutoSize = true;
            this.lblImmaginary.Location = new System.Drawing.Point(210, 9);
            this.lblImmaginary.Name = "lblImmaginary";
            this.lblImmaginary.Size = new System.Drawing.Size(38, 13);
            this.lblImmaginary.TabIndex = 4;
            this.lblImmaginary.Text = "t=Im(s)";
            // 
            // lblJoint
            // 
            this.lblJoint.AutoSize = true;
            this.lblJoint.Location = new System.Drawing.Point(183, 30);
            this.lblJoint.Name = "lblJoint";
            this.lblJoint.Size = new System.Drawing.Size(22, 13);
            this.lblJoint.TabIndex = 5;
            this.lblJoint.Text = "+ i*";
            // 
            // frmComplexAcquirer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(386, 88);
            this.Controls.Add(this.lblJoint);
            this.Controls.Add(this.lblImmaginary);
            this.Controls.Add(this.lblReal);
            this.Controls.Add(this.btnSubmit);
            this.Controls.Add(this.txtImmaginary);
            this.Controls.Add(this.txtReal);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(394, 115);
            this.MinimumSize = new System.Drawing.Size(394, 115);
            this.Name = "frmComplexAcquirer";
            this.Text = "s =: sigma +i*t";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtReal;
        private System.Windows.Forms.TextBox txtImmaginary;
        private System.Windows.Forms.Button btnSubmit;
        private System.Windows.Forms.Label lblReal;
        private System.Windows.Forms.Label lblImmaginary;
        private System.Windows.Forms.Label lblJoint;
    }
}