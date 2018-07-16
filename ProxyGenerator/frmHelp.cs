using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ProxyGenerator
{
    public partial class frmHelp : Form
    {

        /// <summary>
        /// 0 == ParameterFiltering
        /// 1 == BooleanStrings
        /// 2 == Integer Types
        /// 3 == Objects
        /// 4 == Sql Identities as Numeric(x,y)
        /// </summary>
        public frmHelp(byte helpSubject)
        {
            InitializeComponent();
            //
            switch (helpSubject)
            {
                case 0:// parameter filtering
                    {
                        this.txtHelp.Text =
                            "Sql Parameter filtering is used to transform application layer data into db layer data. " +
                            "The transformation might not be trivial, due to dbNull logic and type-ranges.\r\n An example are " +
                            "the row counters, implemented by sql identities(1,1). To address one of them, a positive integer " +
                            "is needed. A value<=0 requires a mapping on dbNull.\r\n Analogous treatment is appropriate to map application " +
                            "level value types onto db level nullable types.\r\n " +
                            "Application booleans are by default mapped on strings, whose null reference maps to dbNull.";
                        break;
                    }
                case 1:// boolean strings
                    {
                        this.txtHelp.Text =
                            "A true-false value on the database is represented by a bit-type. Such db-data-type" +
                            "is nullable, thus generating a three state object: {0,1,DbNull}. When nullability is used" +
                            "by the database-Administrator, value type booleans of .Net become unsufficient to map the" +
                            "above mentioned three state objects. To adequately map such db-bits a string type is suitable." +
                            "We assume the convention to use the operator ToString() on C# bools, to generate \"True\" and \"False\" " +
                            "for 1 and 0 respectively. The dbNullvalue is mapped to each string different from the above mentioned ones." +
                            "so \"\" and null strings will correspond to dbNullvalue as well as any other string.";
                        break;
                    }
                case 2:// integer types
                    {
                        this.txtHelp.Text =
                            "When integral types are used as keys( both primary or foreign), generally positive " +
                            "values only are assigned them. If the procedure uses the acrual value, no matter what " +
                            "You pass down; but many DBAs use to chek if isDbNull on such parameters, to decide how " +
                            "to behave in the sql code. In such cases I took advantage of filters such as " +
                            "if(x<=0){pass down dbNull.}. At application level the best way to let the user decide" +
                            "how to use a value type, is to break up the domain into intervals. The unallowed ones" +
                            "lead to dbNull.";
                        break;
                    }
                case 3:// objects
                    {
                        this.txtHelp.Text =
                            "This measure passes down dbNull each time the application sets to null a reference "+
                            "type parameter. This includes the strings that, though being references, are copied "+
                            "when passed to a function call.";
                        break;
                    }
                case 4:// sql identities as numeric(x,y)
                    {
                        this.txtHelp.Text =
                            "I happened to work in a project where the Database Administrator used to declare the" +
                            "Ids as numeric(x,y). Such definition leads to a C# decumal type, which is a twelve " +
                            "byte fixed point non integral type. From the applicatoin point of view it's a waste " +
                            "of resources. If You want all the numeric(x,y) translated to Int64 instead of decimal," +
                            "choose this option.\r\n Since settings affect all the parameters of each selected" +
                            " procedure, You will have to change manually procedures that contain both Int64 and" +
                            " decimal parameters.";
                        break;
                    }
            }// end switch
        }// end Ctor


    }// end class
}
