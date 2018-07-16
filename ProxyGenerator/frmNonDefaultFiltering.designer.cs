namespace ProxyGenerator
{
    partial class frmNonDefaultFiltering
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
            this.lbxParameterTypes = new System.Windows.Forms.ListBox();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.mnuHelp = new System.Windows.Forms.MenuStrip();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mitParameterFiltering = new System.Windows.Forms.ToolStripMenuItem();
            this.mitBooleanStrings = new System.Windows.Forms.ToolStripMenuItem();
            this.mitIntegralTypes = new System.Windows.Forms.ToolStripMenuItem();
            this.mitObjects = new System.Windows.Forms.ToolStripMenuItem();
            this.mitSqlIdentitiesAsNumericxy = new System.Windows.Forms.ToolStripMenuItem();
            this.mitExit = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuHelp.SuspendLayout();
            this.SuspendLayout();
            // 
            // lbxParameterTypes
            // 
            this.lbxParameterTypes.FormattingEnabled = true;
            this.lbxParameterTypes.HorizontalScrollbar = true;
            this.lbxParameterTypes.Location = new System.Drawing.Point(12, 45);
            this.lbxParameterTypes.Name = "lbxParameterTypes";
            this.lbxParameterTypes.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lbxParameterTypes.Size = new System.Drawing.Size(193, 108);
            this.lbxParameterTypes.TabIndex = 1;
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(12, 176);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 2;
            this.btnOk.Text = "&Ok";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(130, 176);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "&Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // mnuHelp
            // 
            this.mnuHelp.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.helpToolStripMenuItem});
            this.mnuHelp.Location = new System.Drawing.Point(0, 0);
            this.mnuHelp.Name = "mnuHelp";
            this.mnuHelp.Size = new System.Drawing.Size(217, 24);
            this.mnuHelp.TabIndex = 4;
            this.mnuHelp.Text = "mnuHelp";
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mitParameterFiltering,
            this.mitBooleanStrings,
            this.mitIntegralTypes,
            this.mitObjects,
            this.mitSqlIdentitiesAsNumericxy,
            this.mitExit});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(40, 20);
            this.helpToolStripMenuItem.Text = "&Help";
            // 
            // mitParameterFiltering
            // 
            this.mitParameterFiltering.Name = "mitParameterFiltering";
            this.mitParameterFiltering.Size = new System.Drawing.Size(226, 22);
            this.mitParameterFiltering.Text = "&Parameter Filtering";
            this.mitParameterFiltering.Click += new System.EventHandler(this.mitParameterFiltering_Click);
            // 
            // mitBooleanStrings
            // 
            this.mitBooleanStrings.Name = "mitBooleanStrings";
            this.mitBooleanStrings.Size = new System.Drawing.Size(226, 22);
            this.mitBooleanStrings.Text = "&Boolean Strings";
            this.mitBooleanStrings.Click += new System.EventHandler(this.mitBooleanStrings_Click);
            // 
            // mitIntegralTypes
            // 
            this.mitIntegralTypes.Name = "mitIntegralTypes";
            this.mitIntegralTypes.Size = new System.Drawing.Size(226, 22);
            this.mitIntegralTypes.Text = "Integer Types";
            this.mitIntegralTypes.Click += new System.EventHandler(this.mitIntegralTypes_Click);
            // 
            // mitObjects
            // 
            this.mitObjects.Name = "mitObjects";
            this.mitObjects.Size = new System.Drawing.Size(226, 22);
            this.mitObjects.Text = "Objects";
            this.mitObjects.Click += new System.EventHandler(this.mitObjects_Click);
            // 
            // mitSqlIdentitiesAsNumericxy
            // 
            this.mitSqlIdentitiesAsNumericxy.Name = "mitSqlIdentitiesAsNumericxy";
            this.mitSqlIdentitiesAsNumericxy.Size = new System.Drawing.Size(226, 22);
            this.mitSqlIdentitiesAsNumericxy.Text = "Sql Identities as Numeric(x,y)";
            this.mitSqlIdentitiesAsNumericxy.Click += new System.EventHandler(this.mitSqlIdentitiesAsNumericxy_Click);
            // 
            // mitExit
            // 
            this.mitExit.Name = "mitExit";
            this.mitExit.Size = new System.Drawing.Size(226, 22);
            this.mitExit.Text = "Exit";
            this.mitExit.Click += new System.EventHandler(this.mitExit_Click);
            // 
            // frmNonDefaultFiltering
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(217, 216);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.lbxParameterTypes);
            this.Controls.Add(this.mnuHelp);
            this.MainMenuStrip = this.mnuHelp;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(225, 243);
            this.MinimumSize = new System.Drawing.Size(225, 243);
            this.Name = "frmNonDefaultFiltering";
            this.Text = "Activate Filtering Voices";
            this.mnuHelp.ResumeLayout(false);
            this.mnuHelp.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox lbxParameterTypes;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.MenuStrip mnuHelp;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mitParameterFiltering;
        private System.Windows.Forms.ToolStripMenuItem mitBooleanStrings;
        private System.Windows.Forms.ToolStripMenuItem mitIntegralTypes;
        private System.Windows.Forms.ToolStripMenuItem mitObjects;
        private System.Windows.Forms.ToolStripMenuItem mitSqlIdentitiesAsNumericxy;
        private System.Windows.Forms.ToolStripMenuItem mitExit;
    }
}