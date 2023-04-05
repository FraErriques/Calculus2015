namespace ProxyGenerator
{
    partial class frmDbCockpit
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.databaseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectDatabaseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fileSystemToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.chooseWorkingFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lbxProcedures = new System.Windows.Forms.ListBox();
            this.btnConfirmSelection = new System.Windows.Forms.Button();
            this.rbtMultipleResultset = new System.Windows.Forms.RadioButton();
            this.grbReadWrite = new System.Windows.Forms.GroupBox();
            this.chkNonDefaultFiltering = new System.Windows.Forms.CheckBox();
            this.rbtNoResultset = new System.Windows.Forms.RadioButton();
            this.chkAllowTransaction = new System.Windows.Forms.CheckBox();
            this.rbtSingleResultset = new System.Windows.Forms.RadioButton();
            this.grbConnectionString = new System.Windows.Forms.GroupBox();
            this.lblPassword = new System.Windows.Forms.Label();
            this.lblUsername = new System.Windows.Forms.Label();
            this.lblServer = new System.Windows.Forms.Label();
            this.lblDatabase = new System.Windows.Forms.Label();
            this.cmbConnectionString = new System.Windows.Forms.ComboBox();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.txtUsername = new System.Windows.Forms.TextBox();
            this.txtServer = new System.Windows.Forms.TextBox();
            this.txtDatabase = new System.Windows.Forms.TextBox();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.btnExit = new System.Windows.Forms.Button();
            this.menuStrip1.SuspendLayout();
            this.grbReadWrite.SuspendLayout();
            this.grbConnectionString.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.databaseToolStripMenuItem,
            this.fileSystemToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(768, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // databaseToolStripMenuItem
            // 
            this.databaseToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.selectDatabaseToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.databaseToolStripMenuItem.Name = "databaseToolStripMenuItem";
            this.databaseToolStripMenuItem.Size = new System.Drawing.Size(67, 20);
            this.databaseToolStripMenuItem.Text = "Database";
            // 
            // selectDatabaseToolStripMenuItem
            // 
            this.selectDatabaseToolStripMenuItem.Name = "selectDatabaseToolStripMenuItem";
            this.selectDatabaseToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.selectDatabaseToolStripMenuItem.Text = "Bind to Database";
            this.selectDatabaseToolStripMenuItem.Click += new System.EventHandler(this.selectDatabaseToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.exitToolStripMenuItem.Text = "E&xit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // fileSystemToolStripMenuItem
            // 
            this.fileSystemToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.chooseWorkingFolderToolStripMenuItem});
            this.fileSystemToolStripMenuItem.Name = "fileSystemToolStripMenuItem";
            this.fileSystemToolStripMenuItem.Size = new System.Drawing.Size(75, 20);
            this.fileSystemToolStripMenuItem.Text = "FileSystem";
            // 
            // chooseWorkingFolderToolStripMenuItem
            // 
            this.chooseWorkingFolderToolStripMenuItem.Name = "chooseWorkingFolderToolStripMenuItem";
            this.chooseWorkingFolderToolStripMenuItem.Size = new System.Drawing.Size(198, 22);
            this.chooseWorkingFolderToolStripMenuItem.Text = "Choose Working Folder";
            this.chooseWorkingFolderToolStripMenuItem.Click += new System.EventHandler(this.chooseWorkingFolderToolStripMenuItem_Click);
            // 
            // lbxProcedures
            // 
            this.lbxProcedures.FormattingEnabled = true;
            this.lbxProcedures.HorizontalScrollbar = true;
            this.lbxProcedures.Location = new System.Drawing.Point(12, 37);
            this.lbxProcedures.Name = "lbxProcedures";
            this.lbxProcedures.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lbxProcedures.Size = new System.Drawing.Size(352, 329);
            this.lbxProcedures.TabIndex = 2;
            // 
            // btnConfirmSelection
            // 
            this.btnConfirmSelection.Location = new System.Drawing.Point(576, 344);
            this.btnConfirmSelection.Name = "btnConfirmSelection";
            this.btnConfirmSelection.Size = new System.Drawing.Size(75, 23);
            this.btnConfirmSelection.TabIndex = 3;
            this.btnConfirmSelection.Text = "Confirm Selection";
            this.btnConfirmSelection.UseVisualStyleBackColor = true;
            this.btnConfirmSelection.Click += new System.EventHandler(this.btnConfirmSelection_Click);
            // 
            // rbtMultipleResultset
            // 
            this.rbtMultipleResultset.AutoSize = true;
            this.rbtMultipleResultset.Location = new System.Drawing.Point(15, 30);
            this.rbtMultipleResultset.Name = "rbtMultipleResultset";
            this.rbtMultipleResultset.Size = new System.Drawing.Size(184, 17);
            this.rbtMultipleResultset.TabIndex = 4;
            this.rbtMultipleResultset.TabStop = true;
            this.rbtMultipleResultset.Text = "Multiple Resultset: return Dataset.";
            this.rbtMultipleResultset.UseVisualStyleBackColor = true;
            // 
            // grbReadWrite
            // 
            this.grbReadWrite.Controls.Add(this.chkNonDefaultFiltering);
            this.grbReadWrite.Controls.Add(this.rbtNoResultset);
            this.grbReadWrite.Controls.Add(this.chkAllowTransaction);
            this.grbReadWrite.Controls.Add(this.rbtSingleResultset);
            this.grbReadWrite.Controls.Add(this.rbtMultipleResultset);
            this.grbReadWrite.Location = new System.Drawing.Point(372, 190);
            this.grbReadWrite.Name = "grbReadWrite";
            this.grbReadWrite.Size = new System.Drawing.Size(382, 148);
            this.grbReadWrite.TabIndex = 5;
            this.grbReadWrite.TabStop = false;
            this.grbReadWrite.Text = "Proxy Characteristics";
            // 
            // chkNonDefaultFiltering
            // 
            this.chkNonDefaultFiltering.AutoSize = true;
            this.chkNonDefaultFiltering.Location = new System.Drawing.Point(240, 60);
            this.chkNonDefaultFiltering.Name = "chkNonDefaultFiltering";
            this.chkNonDefaultFiltering.Size = new System.Drawing.Size(125, 17);
            this.chkNonDefaultFiltering.TabIndex = 11;
            this.chkNonDefaultFiltering.Text = "Non Default Filtering.";
            this.chkNonDefaultFiltering.UseVisualStyleBackColor = true;
            this.chkNonDefaultFiltering.CheckedChanged += new System.EventHandler(this.chkNonDefaultFiltering_CheckedChanged);
            // 
            // rbtNoResultset
            // 
            this.rbtNoResultset.AutoSize = true;
            this.rbtNoResultset.Location = new System.Drawing.Point(15, 90);
            this.rbtNoResultset.Name = "rbtNoResultset";
            this.rbtNoResultset.Size = new System.Drawing.Size(197, 17);
            this.rbtNoResultset.TabIndex = 10;
            this.rbtNoResultset.TabStop = true;
            this.rbtNoResultset.Text = "No Resultset: return a status-integer.";
            this.rbtNoResultset.UseVisualStyleBackColor = true;
            // 
            // chkAllowTransaction
            // 
            this.chkAllowTransaction.AutoSize = true;
            this.chkAllowTransaction.Location = new System.Drawing.Point(240, 30);
            this.chkAllowTransaction.Name = "chkAllowTransaction";
            this.chkAllowTransaction.Size = new System.Drawing.Size(113, 17);
            this.chkAllowTransaction.TabIndex = 6;
            this.chkAllowTransaction.Text = "Allow Transaction.";
            this.chkAllowTransaction.UseVisualStyleBackColor = true;
            // 
            // rbtSingleResultset
            // 
            this.rbtSingleResultset.AutoSize = true;
            this.rbtSingleResultset.Location = new System.Drawing.Point(15, 60);
            this.rbtSingleResultset.Name = "rbtSingleResultset";
            this.rbtSingleResultset.Size = new System.Drawing.Size(190, 17);
            this.rbtSingleResultset.TabIndex = 5;
            this.rbtSingleResultset.TabStop = true;
            this.rbtSingleResultset.Text = "Single Resultset: return DataTable.";
            this.rbtSingleResultset.UseVisualStyleBackColor = true;
            // 
            // grbConnectionString
            // 
            this.grbConnectionString.Controls.Add(this.lblPassword);
            this.grbConnectionString.Controls.Add(this.lblUsername);
            this.grbConnectionString.Controls.Add(this.lblServer);
            this.grbConnectionString.Controls.Add(this.lblDatabase);
            this.grbConnectionString.Controls.Add(this.cmbConnectionString);
            this.grbConnectionString.Controls.Add(this.txtPassword);
            this.grbConnectionString.Controls.Add(this.txtUsername);
            this.grbConnectionString.Controls.Add(this.txtServer);
            this.grbConnectionString.Controls.Add(this.txtDatabase);
            this.grbConnectionString.Location = new System.Drawing.Point(372, 32);
            this.grbConnectionString.Name = "grbConnectionString";
            this.grbConnectionString.Size = new System.Drawing.Size(382, 150);
            this.grbConnectionString.TabIndex = 6;
            this.grbConnectionString.TabStop = false;
            this.grbConnectionString.Text = "Connection String";
            // 
            // lblPassword
            // 
            this.lblPassword.AutoSize = true;
            this.lblPassword.Location = new System.Drawing.Point(184, 95);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new System.Drawing.Size(63, 13);
            this.lblPassword.TabIndex = 8;
            this.lblPassword.Text = "lblPassword";
            // 
            // lblUsername
            // 
            this.lblUsername.AutoSize = true;
            this.lblUsername.Location = new System.Drawing.Point(184, 48);
            this.lblUsername.Name = "lblUsername";
            this.lblUsername.Size = new System.Drawing.Size(65, 13);
            this.lblUsername.TabIndex = 7;
            this.lblUsername.Text = "lblUsername";
            // 
            // lblServer
            // 
            this.lblServer.AutoSize = true;
            this.lblServer.Location = new System.Drawing.Point(15, 95);
            this.lblServer.Name = "lblServer";
            this.lblServer.Size = new System.Drawing.Size(48, 13);
            this.lblServer.TabIndex = 6;
            this.lblServer.Text = "lblServer";
            // 
            // lblDatabase
            // 
            this.lblDatabase.AutoSize = true;
            this.lblDatabase.Location = new System.Drawing.Point(15, 48);
            this.lblDatabase.Name = "lblDatabase";
            this.lblDatabase.Size = new System.Drawing.Size(63, 13);
            this.lblDatabase.TabIndex = 5;
            this.lblDatabase.Text = "lblDatabase";
            // 
            // cmbConnectionString
            // 
            this.cmbConnectionString.FormattingEnabled = true;
            this.cmbConnectionString.Location = new System.Drawing.Point(15, 19);
            this.cmbConnectionString.Name = "cmbConnectionString";
            this.cmbConnectionString.Size = new System.Drawing.Size(345, 21);
            this.cmbConnectionString.TabIndex = 4;
            this.cmbConnectionString.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(184, 120);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Size = new System.Drawing.Size(176, 20);
            this.txtPassword.TabIndex = 3;
            this.txtPassword.UseSystemPasswordChar = true;
            this.txtPassword.WordWrap = false;
            // 
            // txtUsername
            // 
            this.txtUsername.Location = new System.Drawing.Point(184, 67);
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.Size = new System.Drawing.Size(176, 20);
            this.txtUsername.TabIndex = 2;
            this.txtUsername.WordWrap = false;
            // 
            // txtServer
            // 
            this.txtServer.Location = new System.Drawing.Point(15, 120);
            this.txtServer.Name = "txtServer";
            this.txtServer.Size = new System.Drawing.Size(146, 20);
            this.txtServer.TabIndex = 1;
            this.txtServer.WordWrap = false;
            // 
            // txtDatabase
            // 
            this.txtDatabase.Location = new System.Drawing.Point(15, 67);
            this.txtDatabase.Name = "txtDatabase";
            this.txtDatabase.Size = new System.Drawing.Size(146, 20);
            this.txtDatabase.TabIndex = 0;
            this.txtDatabase.WordWrap = false;
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(679, 344);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(75, 23);
            this.btnExit.TabIndex = 7;
            this.btnExit.Text = "&Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // frmDbCockpit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(768, 375);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.grbConnectionString);
            this.Controls.Add(this.grbReadWrite);
            this.Controls.Add(this.btnConfirmSelection);
            this.Controls.Add(this.lbxProcedures);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(790, 420);
            this.MinimumSize = new System.Drawing.Size(784, 414);
            this.Name = "frmDbCockpit";
            this.Text = "Proxy Generator";
            this.Load += new System.EventHandler(this.DbCockpit_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.grbReadWrite.ResumeLayout(false);
            this.grbReadWrite.PerformLayout();
            this.grbConnectionString.ResumeLayout(false);
            this.grbConnectionString.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem databaseToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem selectDatabaseToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ListBox lbxProcedures;
        private System.Windows.Forms.Button btnConfirmSelection;
        private System.Windows.Forms.RadioButton rbtMultipleResultset;
        private System.Windows.Forms.GroupBox grbReadWrite;
        private System.Windows.Forms.RadioButton rbtSingleResultset;
        private System.Windows.Forms.GroupBox grbConnectionString;
        private System.Windows.Forms.ComboBox cmbConnectionString;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.TextBox txtUsername;
        private System.Windows.Forms.TextBox txtServer;
        private System.Windows.Forms.TextBox txtDatabase;
        private System.Windows.Forms.Label lblDatabase;
        private System.Windows.Forms.Label lblPassword;
        private System.Windows.Forms.Label lblUsername;
        private System.Windows.Forms.Label lblServer;
        private System.Windows.Forms.CheckBox chkAllowTransaction;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.RadioButton rbtNoResultset;
        private System.Windows.Forms.CheckBox chkNonDefaultFiltering;
        private System.Windows.Forms.ToolStripMenuItem fileSystemToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem chooseWorkingFolderToolStripMenuItem;
        private System.Windows.Forms.Button btnExit;
    }
}

