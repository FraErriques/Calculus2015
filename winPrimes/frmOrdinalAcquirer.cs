using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;

using System.Text;
using System.Windows.Forms;

namespace winPrimes
{
    public partial class frmOrdinalAcquirer : Form
    {
        // public to be readable from the caller.
        public Int64 theOrdinal = default(Int64);


        public frmOrdinalAcquirer()
        {
            InitializeComponent();
        }

        private void btnAcquirer_Click(object sender, EventArgs e)
        {
            try
            {
                // possible triplets' separators are: {' ', '#', '.', ','}
                string filtered_Ordinal = this.txtAcquirer.Text.Replace(" ", "");
                filtered_Ordinal = filtered_Ordinal.Replace("#", "");
                filtered_Ordinal = filtered_Ordinal.Replace(".", "");
                filtered_Ordinal = filtered_Ordinal.Replace(",", "");
                //
                this.theOrdinal = Int64.Parse(filtered_Ordinal);
                this.Close();
            }
            catch
            {
                this.txtAcquirer.Text = "Wrong input. Natural number required.";
                this.txtAcquirer.BackColor = System.Drawing.Color.Yellow;
                return;// on the child form.
            }
        }//


    }


}
