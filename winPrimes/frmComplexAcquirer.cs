using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;


namespace winPrimes
{


    public partial class frmComplexAcquirer : Form
    {
        // public to be readable from the caller.
        public double Re;
        public double Im;

        public frmComplexAcquirer()
        {
            InitializeComponent();
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
                //
                string filtered_RealPart = this.txtReal.Text.Replace(" ","");
                filtered_RealPart = filtered_RealPart.Replace("#", "");
                //
                string filtered_ImmaginaryPart = this.txtImmaginary.Text.Replace(" ", "");// allow blank(i.e. '') as figure separator.
                filtered_ImmaginaryPart = filtered_ImmaginaryPart.Replace("#", "");// allow '#' as figure separator.
                //
                this.Re = double.Parse(filtered_RealPart);
                this.Im = double.Parse(filtered_ImmaginaryPart);
                //
                this.Close();
            }
            catch
            {
                this.txtReal.Text = this.txtImmaginary.Text = "Real number required.";
                this.txtReal.BackColor = this.txtImmaginary.BackColor = System.Drawing.Color.Yellow;
                return;// on the child form.
            }
        }//


    }//


}
