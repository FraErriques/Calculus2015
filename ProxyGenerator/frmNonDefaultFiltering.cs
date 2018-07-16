using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ProxyGenerator
{

    public partial class frmNonDefaultFiltering : Form
    {
        //---parameter filtering---
        /// 1 == BooleanStrings
        /// 2 == Integral Types
        /// 3 == Objects
        /// 4 == Sql Identities as Numeric(x,y)
        private bool booleanStrings = false;
        private bool integralTypes = false;
        private bool objects = false;
        private bool sqlIdentitiesAsNumeric = false;

        public bool BooleanStrings
        {
            get { return booleanStrings; }
            //set { booleanStrings = value; }
        }
        public bool IntegralTypes
        {
            get { return integralTypes; }
            //set { integralTypes = value; }
        }
        public bool Objects
        {
            get { return objects; }
            //set { objects = value; }
        }
        public bool SqlIdentitiesAsNumeric
        {
            get { return sqlIdentitiesAsNumeric; }
            //set { sqlIdentitiesAsNumeric = value; }
        }


        /// <summary>
        /// the row in the listBox.
        /// </summary>
        public struct ListBoxItem
        {
            private byte id;
            private string name;
            public int Id
            {
                get { return id; }
                // NO set { id = value; }
            }
            public string Name
            {
                get { return name; }
                // NO set { name = value; }
            }

            public ListBoxItem(
                byte id,
                string name
                )
            {
                this.id = id;
                this.name = name;
            }// end Ctor

            /// <summary>
            /// used by the ListBox control, to show the description.
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                return this.name;
            }// end ToString

        }// end ListBoxItem


        /// <summary>
        ///---Ctor---
        /// </summary>
        public frmNonDefaultFiltering(
            )
        {
            InitializeComponent();
            //
            //---parameter filtering---
            /// 1 == BooleanStrings
            /// 2 == Integral Types
            /// 3 == Objects
            /// 4 == Sql Identities as Numeric(x,y)
            System.Data.DataTable tblVociMenu = new DataTable("VociMenu");
            tblVociMenu.Columns.Add("id", typeof(byte));
            tblVociMenu.Columns.Add("name", typeof(string));
            object[] rowTemplate = new object[2];
            //
            rowTemplate[0] = 1;
            rowTemplate[1] = "BooleanStrings";
            tblVociMenu.Rows.Add(rowTemplate);
            //
            rowTemplate[0] = 2;
            rowTemplate[1] = "Integral Types";
            tblVociMenu.Rows.Add(rowTemplate);
            //
            rowTemplate[0] = 3;
            rowTemplate[1] = "Objects";
            tblVociMenu.Rows.Add(rowTemplate);
            //
            rowTemplate[0] = 4;
            rowTemplate[1] = "Sql Identities as Numeric(x,y)";
            tblVociMenu.Rows.Add(rowTemplate);
            //
            this.lbxParameterTypes.Items.Clear();
            for (int c = 0; c < tblVociMenu.Rows.Count; c++)
            {
                ListBoxItem listBoxItem = new ListBoxItem(
                    (byte)(tblVociMenu.Rows[c]["id"]),
                    (string)(tblVociMenu.Rows[c]["name"])
                );
                this.lbxParameterTypes.Items.Add(listBoxItem);
            }
        }/// end default Ctor.




        /// 1 == BooleanStrings
        /// 2 == Integral Types
        /// 3 == Objects
        /// 4 == Sql Identities as Numeric(x,y)
        private void btnOk_Click(object sender, EventArgs e)
        {
            // write ref pars
            for( int c=0; c<this.lbxParameterTypes.SelectedItems.Count; c++)
            {
                switch (((ListBoxItem)(this.lbxParameterTypes.SelectedItems[c])).Name)
                {
                    case "BooleanStrings":
                        {
                            this.booleanStrings = true;
                            break;
                        }
                    case "Integral Types":
                        {
                            this.integralTypes = true;
                            break;
                        }
                    case "Objects":
                        {
                            this.objects = true;
                            break;
                        }
                    case "Sql Identities as Numeric(x,y)":
                        {
                            this.sqlIdentitiesAsNumeric = true;
                            break;
                        }
                }// end switch
            }// end for each selected ListItem
            // ready
            this.mitExit_Click(this, null);//close
        }// end ok

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.mitExit_Click(this, null);
        }// end cancel

        private void mitExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }// end mitExit_Click

        /// <summary>
        /// 0 == ParameterFiltering
        /// 1 == BooleanStrings
        /// 2 == Integer Types
        /// 3 == Objects
        /// 4 == Sql Identities as Numeric(x,y)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mitParameterFiltering_Click(object sender, EventArgs e)
        {
            frmHelp help =
                new frmHelp( 0);
            help.Show();
        }// end mitParameterFiltering_Click

        private void mitBooleanStrings_Click(object sender, EventArgs e)
        {
            frmHelp help =
                new frmHelp( 1);
            help.Show();
        }// end mitBooleanStrings_Click

        private void mitIntegralTypes_Click(object sender, EventArgs e)
        {
            frmHelp help =
                new frmHelp(2);
            help.Show();
        }// end mitIntegralTypes_Click

        private void mitObjects_Click(object sender, EventArgs e)
        {
            frmHelp help =
                new frmHelp(3);
            help.Show();
        }// end mitObjects_Click

        private void mitSqlIdentitiesAsNumericxy_Click(object sender, EventArgs e)
        {
            frmHelp help =
                new frmHelp(4);
            help.Show();
        }// end mitSqlIdentitiesAsNumericxy_Click


    }// end class
}// end nmsp
