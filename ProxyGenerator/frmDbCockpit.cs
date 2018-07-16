using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ProxyGenerator
{

    public partial class frmDbCockpit : Form
    {

        #region types&data

        const int nTokens = 13;// number od CodeBlock-templates.
        private string configured_CodeBlocksPath = null;// path where the CodeBlocks reside in.
        private string chosen_ProxiesFullPath = null;// path to write Proxies to
        private System.Collections.ArrayList configuredStrings = null;// set of ConnectionStrings written down in the App.config.
        // selected connection string.
        private string connectionStringName = null;
        private string connectionStringValue = null;
        System.Data.SqlClient.SqlConnection conn = null;// used to retrieve stored-names and signatures.
        //
        //---parameter filtering---
        /// 1 == BooleanStrings
        /// 2 == Integral Types
        /// 3 == Objects
        /// 4 == Sql Identities as Numeric(x,y)
        private bool _filtering_booleanStrings = false;
        private bool _filtering_integralTypes = false;
        private bool _filtering_objects = false;
        private bool _filtering_sqlIdentitiesAsNumeric = false;
        //
        public struct Couple
        {
            // private Couple() { }// no empty construction allowed // Cannot avoid this on a struct
            public Couple(string name, int id)
            {
                this.name = name;
                this.id = id;
            }// end Ctor
            public Couple(string name, string connectionString)
            {
                this.name = name;
                this.id = connectionString;
            }// end Ctor

            public string Name
            {
                get
                {
                    return this.name;
                }// read only
            }
            public int Id
            {
                get
                {
                    return (int)(this.id);
                }// read only
            }
            public string ConnectionString
            {
                get
                {
                    return (string)(this.id);
                }// read only
            }

            public override string ToString()
            {
                return this.name;
            }

            // private fields
            private string name;
            private object id;
        }// end Couple

        #endregion types&data



        #region traduzione


        /// <summary>
        /// HashTable of CodeBlocks.
        /// Read-Write branches are resolved here.
        /// Transaction branch is resolved here. 
        /// Nullable booleans are resolved when parsing each SqlParam. Not yet possible here.
        /// </summary>
        /// <param name="isRead"></param>
        /// <returns></returns>
        private System.Collections.Hashtable takeTokens(
            byte retTypeSwitch,// tre casi: {0=MultipleResultSet, 1=SingleResultSet, 2=NoResultSet}
            bool allowTransaction
          )
        {
            System.Collections.Hashtable theTokens =
                new System.Collections.Hashtable(15);// approximate number of CodeBlocks.
            //
            try
            {
                theTokens[@"01_ClassEnvelope_UntilSignature_.txt"] =
                    System.IO.File.ReadAllText(
                        this.configured_CodeBlocksPath +
                        @"01_ClassEnvelope_UntilSignature_.txt");
                //
                theTokens[@"02_ProxyParameterList_dynamc.txt"] =
                    System.IO.File.ReadAllText(
                        this.configured_CodeBlocksPath +
                        @"02_ProxyParameterList_dynamc.txt");
                //
                if (0 == retTypeSwitch || 1 == retTypeSwitch) // 0=MultipleResultSet, 1=SingleResultSet
                {
                    if (allowTransaction)
                    {// allow a transaction if a notNull one is provided.
                        theTokens[@"Read\03trx_ReadSqlCommand.txt"] =
                            System.IO.File.ReadAllText(
                                this.configured_CodeBlocksPath +
                                @"Read\03trx_ReadSqlCommand.txt");
                    }
                    else// No transaction allowed.
                    {
                        theTokens[@"Read\03_ReadSqlCommand.txt"] =
                            System.IO.File.ReadAllText(
                                this.configured_CodeBlocksPath +
                                @"Read\03_ReadSqlCommand.txt");
                    }
                    theTokens[@"Read\04_ResultSet.txt"] =
                        System.IO.File.ReadAllText(
                            this.configured_CodeBlocksPath +
                            @"Read\04_ResultSet.txt");
                }
                else if (2 == retTypeSwitch) // 2=NoResultSet
                {
                    if (allowTransaction)
                    {// allow a transaction if a notNull one is provided.
                        theTokens[@"Write\03trx_WriteSqlCommand.txt"] =
                            System.IO.File.ReadAllText(
                                this.configured_CodeBlocksPath +
                                @"Write\03trx_WriteSqlCommand.txt");
                    }
                    else// No transaction allowed.
                    {
                        theTokens[@"Write\03_WriteSqlCommand.txt"] =
                            System.IO.File.ReadAllText(
                                this.configured_CodeBlocksPath +
                                @"Write\03_WriteSqlCommand.txt");
                    }
                    theTokens[@"Write\04_IntegerResult.txt"] =
                        System.IO.File.ReadAllText(
                            this.configured_CodeBlocksPath +
                            @"Write\04_IntegerResult.txt");
                }
                // no else: no more cases.
                //
                theTokens[@"05_SqlParameter_many.txt"] =
                    System.IO.File.ReadAllText(
                        this.configured_CodeBlocksPath +
                        @"05_SqlParameter_many.txt");
                //
                theTokens[@"06_ProxyParValidity_.txt"] =
                    System.IO.File.ReadAllText(
                        this.configured_CodeBlocksPath +
                        @"06_ProxyParValidity_.txt");
                //
                theTokens[@"07A_NotnullAssignmentBool_.txt"] =
                    System.IO.File.ReadAllText(
                        this.configured_CodeBlocksPath +
                        @"07A_NotnullAssignmentBool_.txt");
                //
                theTokens[@"07B_NotnullAssignmentNotbool_.txt"] =
                    System.IO.File.ReadAllText(
                        this.configured_CodeBlocksPath +
                        @"07B_NotnullAssignmentNotbool_.txt");
                //
                theTokens[@"08_OpenTry.txt"] =
                    System.IO.File.ReadAllText(
                        this.configured_CodeBlocksPath +
                        @"08_OpenTry.txt");
                //
                if (0 == retTypeSwitch || 1 == retTypeSwitch) // 0=MultipleResultSet, 1=SingleResultSet
                {
                    theTokens[@"Read\09_ReadTry.txt"] =
                        System.IO.File.ReadAllText(
                            this.configured_CodeBlocksPath +
                            @"Read\09_ReadTry.txt");
                }
                else if (2 == retTypeSwitch) // 2=NoResultSet
                {
                    theTokens[@"Write\09_WriteTry.txt"] =
                        System.IO.File.ReadAllText(
                            this.configured_CodeBlocksPath +
                            @"Write\09_WriteTry.txt");
                }
                // no else: no more cases.
                //
                theTokens[@"10_OutParRecovery_optional.txt"] =
                    System.IO.File.ReadAllText(
                        this.configured_CodeBlocksPath +
                        @"10_OutParRecovery_optional.txt");
                //
                theTokens[@"11_CloseTry_OpenCatch.txt"] =
                    System.IO.File.ReadAllText(
                        this.configured_CodeBlocksPath +
                        @"11_CloseTry_OpenCatch.txt");
                //
                if (0 == retTypeSwitch || 1 == retTypeSwitch) // 0=MultipleResultSet, 1=SingleResultSet
                {
                    theTokens[@"Read\12_ReadCatch.txt"] =
                        System.IO.File.ReadAllText(
                            this.configured_CodeBlocksPath +
                            @"Read\12_ReadCatch.txt");
                    //
                    if (allowTransaction)
                    {// allow a transaction if a notNull one is provided.
                        theTokens[@"Read\13trx_Read_Shutting.txt"] =
                            System.IO.File.ReadAllText(
                                this.configured_CodeBlocksPath +
                                @"Read\13trx_Read_Shutting.txt");
                    }
                    else// No transaction allowed.
                    {
                        theTokens[@"Read\13_Read_Shutting.txt"] =
                            System.IO.File.ReadAllText(
                                this.configured_CodeBlocksPath +
                                @"Read\13_Read_Shutting.txt");
                    }
                }
                else if (2 == retTypeSwitch) // 2=NoResultSet
                {
                    theTokens[@"Write\12_WriteCatch.txt"] =
                        System.IO.File.ReadAllText(
                            this.configured_CodeBlocksPath +
                            @"Write\12_WriteCatch.txt");
                    //
                    if (allowTransaction)
                    {// allow a transaction if a notNull one is provided.
                        theTokens[@"Write\13trx_Write_Shutting.txt"] =
                            System.IO.File.ReadAllText(
                                this.configured_CodeBlocksPath +
                                @"Write\13trx_Write_Shutting.txt");
                    }
                    else// No transaction allowed.
                    {
                        theTokens[@"Write\13_Write_Shutting.txt"] =
                            System.IO.File.ReadAllText(
                                this.configured_CodeBlocksPath +
                                @"Write\13_Write_Shutting.txt");
                    }
                }
                // no else: no more cases.
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(
                    "Add in the App.config the directory where the CodeBlocks are stored.\r\n"+
                    "Add the path at the tag <CodeBlocks>.\r\n\r\n" +
                    ex.Message,
                    "Choose CodeBlocks Folder" // caption==titolo.
                    );
                theTokens = null;// no CodeBlocks found.
            }
            // no finally
            //----ready------
            return theTokens;// HashTable of CodeBlock templates.
        }// end takeTokens




        private void scanSingleSqlParameter(
            System.Data.SqlClient.SqlParameter parameterToBeScanned,
            ref string _placeholderName_,
            ref string _placeholderProxyParameter_,
            ref string _placeholderDirection_,
            ref string _placeholderDbtype_,
            ref string _placeholderCtype_,
            ref string _placeholderProxyParValidity_,
            ref int    sqlParameterSize,
            //
            ref string  actualValueAssignment, 
            string      NotnullAssignmentBool,
            string      NotnullAssignmentNotbool
          )
        {
            // usage ( parameterToBeScanned.ParameterName)  -> _placeholderName_
            _placeholderName_ = parameterToBeScanned.ParameterName.Substring(1);// cut @.
            //
            sqlParameterSize = parameterToBeScanned.Size;// track size for outStrs.
            // work on Direction.
            switch (parameterToBeScanned.Direction)
            {
                case System.Data.ParameterDirection.Input:
                    {
                        _placeholderDirection_ = "Input";
                        break;
                    }
                case System.Data.ParameterDirection.InputOutput:
                    {
                        _placeholderDirection_ = "InputOutput";
                        break;
                    }
                case System.Data.ParameterDirection.Output:
                    {
                        _placeholderDirection_ = "Output";
                        break;
                    }
                case System.Data.ParameterDirection.ReturnValue:
                    {
                        _placeholderDirection_ = "ReturnValue";
                        break;
                    }
            }// end usage parameterToBeScanned.Direction -> _placeholderDirection_
            //
            // work on DbType.
            switch (parameterToBeScanned.DbType)
            {
                case (System.Data.DbType.String):
                case (System.Data.DbType.StringFixedLength):
                case (System.Data.DbType.AnsiString):
                case (System.Data.DbType.AnsiStringFixedLength):
                    {
                        _placeholderCtype_ = "string";
                        _placeholderProxyParameter_ = "\t\t\t";
                        if ("Input" != _placeholderDirection_)
                            _placeholderProxyParameter_ += "ref ";
                        _placeholderProxyParameter_ += "string " + _placeholderName_ + ",\r\n";
                        _placeholderDbtype_ = "String";
                        // Value
                        if ("Output" != _placeholderDirection_)
                        {// no Value assignment in Out direction.
                            if (this._filtering_objects )// required dbNull on null reference-types.
                            {
                                _placeholderProxyParValidity_ = "null!=" + _placeholderName_ + " && \"\"!=" + _placeholderName_;
                                actualValueAssignment = actualValueAssignment.Replace("_placeholderProxyParValidity_", _placeholderProxyParValidity_);
                                actualValueAssignment = actualValueAssignment.Replace("_placeholderNotnullAssignment_",
                                    NotnullAssignmentNotbool.Replace("_placeholderName_", _placeholderName_));
                                actualValueAssignment = actualValueAssignment.Replace("_placeholderName_", _placeholderName_);
                            }
                            else// NOT required dbNull on null reference-types.
                            {
                                actualValueAssignment = "\t\t\t"+NotnullAssignmentNotbool.Replace("_placeholderName_", _placeholderName_)+"\r\n";// no filtering
                            }
                        }// else managed just below. Requires to include "InputOutput" category. Not a plain-vanilla "else".
                        if( ("Output" == _placeholderDirection_) ||// Output strings require length specification.
                             ("InputOutput" == _placeholderDirection_)
                            )
                        {
                            actualValueAssignment += "\t\t\tpar" + _placeholderName_ + ".Size=// TODO// Output params require length specification.\r\n";
                        }
                        break;
                    }
                case (System.Data.DbType.Binary):
                    {
                        _placeholderCtype_ = "byte[]";
                        _placeholderProxyParameter_ = "\t\t\t";
                        if ("Input" != _placeholderDirection_)
                            _placeholderProxyParameter_ += "ref ";
                        _placeholderProxyParameter_ += "byte[] " + _placeholderName_ + ",\r\n";
                        _placeholderDbtype_ = "Binary";
                        // Value
                        if ("Output" != _placeholderDirection_)
                        {// no Value assignment in Out direction.
                            if (this._filtering_objects)// required dbNull on null reference-types.
                            {
                                _placeholderProxyParValidity_ = "null!=" + _placeholderName_ + " && 0!=" + _placeholderName_ + ".Length";
                                actualValueAssignment = actualValueAssignment.Replace("_placeholderProxyParValidity_", _placeholderProxyParValidity_);
                                actualValueAssignment = actualValueAssignment.Replace("_placeholderNotnullAssignment_",
                                    NotnullAssignmentNotbool.Replace("_placeholderName_", _placeholderName_));
                                actualValueAssignment = actualValueAssignment.Replace("_placeholderName_", _placeholderName_);
                            }
                            else// NOT required dbNull on null reference-types.
                            {
                                actualValueAssignment = "\t\t\t" + NotnullAssignmentNotbool.Replace("_placeholderName_", _placeholderName_) + "\r\n";// no filtering
                            }
                        }// else managed just below. Requires to include "InputOutput" category. Not a plain-vanilla "else".
                        if (("Output" == _placeholderDirection_) ||// Output strings require length specification.
                             ("InputOutput" == _placeholderDirection_)
                            )
                        {
                            actualValueAssignment += "\t\t\tpar" + _placeholderName_ + ".Size=// TODO// Output params require length specification.\r\n";
                        }
                        break;
                    }
                case (System.Data.DbType.Boolean):
                    {
                        if (this._filtering_booleanStrings)// three states to map db_bits: "True","False", null.
                        {
                            _placeholderCtype_ = "string";
                        }
                        else// two states to map db_bits: value type bool, true or false.
                        {
                            _placeholderCtype_ = "bool";
                        }
                        _placeholderProxyParameter_ = "\t\t\t";
                        if ("Input" != _placeholderDirection_)
                        {
                            _placeholderProxyParameter_ += "ref ";
                        }// else proxy-parameter by value.
                        _placeholderProxyParameter_ += _placeholderCtype_ + " " + _placeholderName_ + ",\r\n";// NB. bool fatto con stringhe per preservare la nullability.
                        _placeholderDbtype_ = "Boolean";
                        // Value
                        if (this._filtering_booleanStrings)// three states to map db_bits: "True","False", null.
                        {
                            _placeholderProxyParValidity_ = "null!=" + _placeholderName_ + " && \"\"!=" + _placeholderName_;
                            actualValueAssignment = actualValueAssignment.Replace("_placeholderProxyParValidity_", _placeholderProxyParValidity_);
                            actualValueAssignment = actualValueAssignment.Replace("_placeholderNotnullAssignment_",
                                NotnullAssignmentBool.Replace("_placeholderName_", _placeholderName_));
                            actualValueAssignment = actualValueAssignment.Replace("_placeholderName_", _placeholderName_);
                        }
                        else// value type booleans cannot map dbNull. They only map true and false.
                        {
                            actualValueAssignment = "\t\t\t" + NotnullAssignmentNotbool.Replace("_placeholderName_", _placeholderName_) + "\r\n";// no filtering
                        }
                        if (("Output" == _placeholderDirection_) ||// Output params require length specification.
                             ("InputOutput" == _placeholderDirection_)
                            )
                        {
                            actualValueAssignment += "\t\t\tpar" + _placeholderName_ + ".Size = 5;// maxLength between \"True\" and \"False\".";
                        }
                        break;
                    }
                case (System.Data.DbType.Byte):
                    {
                        _placeholderCtype_ = "byte";
                        _placeholderProxyParameter_ = "\t\t\t";
                        if ("Input" != _placeholderDirection_)
                            _placeholderProxyParameter_ += "ref ";
                        _placeholderProxyParameter_ += "byte " + _placeholderName_ + ",\r\n";
                        _placeholderDbtype_ = "Byte";
                        // Value                        
                        _placeholderProxyParValidity_ = "byte.MinValue < " + _placeholderName_;// esclude -127==byte.MinValue.
                        actualValueAssignment = actualValueAssignment.Replace("_placeholderProxyParValidity_", _placeholderProxyParValidity_);
                        actualValueAssignment = actualValueAssignment.Replace("_placeholderNotnullAssignment_",
                            NotnullAssignmentNotbool.Replace("_placeholderName_", _placeholderName_));
                        actualValueAssignment = actualValueAssignment.Replace("_placeholderName_", _placeholderName_);
                        break;
                    }
                case (System.Data.DbType.Currency):
                    {
                        _placeholderCtype_ = "decimal";
                        _placeholderProxyParameter_ = "\t\t\t";
                        if ("Input" != _placeholderDirection_)
                            _placeholderProxyParameter_ += "ref ";
                        _placeholderProxyParameter_ += "decimal " + _placeholderName_ + ",\r\n";
                        _placeholderDbtype_ = "Currency";
                        // Value
                        _placeholderProxyParValidity_ = "System.Decimal.MinValue < " + _placeholderName_;
                        actualValueAssignment = actualValueAssignment.Replace("_placeholderProxyParValidity_", _placeholderProxyParValidity_);
                        actualValueAssignment = actualValueAssignment.Replace("_placeholderNotnullAssignment_",
                            NotnullAssignmentNotbool.Replace("_placeholderName_", _placeholderName_));
                        actualValueAssignment = actualValueAssignment.Replace("_placeholderName_", _placeholderName_);
                        break;
                    }
                case (System.Data.DbType.Date):
                    {
                        _placeholderCtype_ = "System.DateTime";
                        _placeholderProxyParameter_ = "\t\t\t";
                        if ("Input" != _placeholderDirection_)
                            _placeholderProxyParameter_ += "ref ";
                        _placeholderProxyParameter_ += "System.DateTime " + _placeholderName_ + ",\r\n";
                        _placeholderDbtype_ = "DateTime";
                        // Value
                        _placeholderProxyParValidity_ = "System.DateTime.MinValue<" + _placeholderName_;
                        actualValueAssignment = actualValueAssignment.Replace("_placeholderProxyParValidity_", _placeholderProxyParValidity_);
                        actualValueAssignment = actualValueAssignment.Replace("_placeholderNotnullAssignment_",
                            NotnullAssignmentNotbool.Replace("_placeholderName_", _placeholderName_));
                        actualValueAssignment = actualValueAssignment.Replace("_placeholderName_", _placeholderName_);
                        break;
                    }
                case (System.Data.DbType.DateTime):
                    {
                        _placeholderCtype_ = "System.DateTime";
                        _placeholderProxyParameter_ = "\t\t\t";
                        if ("Input" != _placeholderDirection_)
                            _placeholderProxyParameter_ += "ref ";
                        _placeholderProxyParameter_ += "System.DateTime " + _placeholderName_ + ",\r\n";
                        _placeholderDbtype_ = "DateTime";
                        // Value
                        _placeholderProxyParValidity_ = "System.DateTime.MinValue<" + _placeholderName_;
                        actualValueAssignment = actualValueAssignment.Replace("_placeholderProxyParValidity_", _placeholderProxyParValidity_);
                        actualValueAssignment = actualValueAssignment.Replace("_placeholderNotnullAssignment_",
                            NotnullAssignmentNotbool.Replace("_placeholderName_", _placeholderName_));
                        actualValueAssignment = actualValueAssignment.Replace("_placeholderName_", _placeholderName_);
                        break;
                    }
                case (System.Data.DbType.Decimal):
                    {
                        if (this._filtering_sqlIdentitiesAsNumeric)
                        {
                            _placeholderCtype_ = "Int64";
                        }
                        else
                        {
                            _placeholderCtype_ = "Decimal";
                        }
                        _placeholderProxyParameter_ = "\t\t\t";
                        if ("Input" != _placeholderDirection_)
                            _placeholderProxyParameter_ += "ref ";
                        _placeholderProxyParameter_ += _placeholderCtype_+" " + _placeholderName_ + ",\r\n";
                        _placeholderDbtype_ = "Decimal";
                        // Value
                        if (this._filtering_sqlIdentitiesAsNumeric && this._filtering_integralTypes)
                        {
                            _placeholderProxyParValidity_ = "0L < " + _placeholderName_;
                            actualValueAssignment = actualValueAssignment.Replace("_placeholderProxyParValidity_", _placeholderProxyParValidity_);
                            actualValueAssignment = actualValueAssignment.Replace("_placeholderNotnullAssignment_",
                                NotnullAssignmentNotbool.Replace("_placeholderName_", _placeholderName_));
                            actualValueAssignment = actualValueAssignment.Replace("_placeholderName_", _placeholderName_);
                        }
                        else// no filtering
                        {
                            actualValueAssignment = "\t\t\t"+NotnullAssignmentNotbool.Replace("_placeholderName_", _placeholderName_)+"\r\n";// no filtering
                        }
                        break;
                    }
                case (System.Data.DbType.Double):
                    {
                        _placeholderCtype_ = "double";
                        _placeholderProxyParameter_ = "\t\t\t";
                        if ("Input" != _placeholderDirection_)
                            _placeholderProxyParameter_ += "ref ";
                        _placeholderProxyParameter_ += _placeholderCtype_ + " " +_placeholderName_ + ",\r\n";
                        _placeholderDbtype_ = "Double";
                        // Value
                        _placeholderProxyParValidity_ =
                            "double.MinValue!=" + _placeholderName_ +
                            " && double.MaxValue!=" + _placeholderName_ +
                            " && double.NaN!=" + _placeholderName_ +
                            " && double.PositiveInfinity!=" + _placeholderName_;
                        actualValueAssignment = actualValueAssignment.Replace("_placeholderProxyParValidity_", _placeholderProxyParValidity_);
                        actualValueAssignment = actualValueAssignment.Replace("_placeholderNotnullAssignment_",
                            NotnullAssignmentNotbool.Replace("_placeholderName_", _placeholderName_));
                        actualValueAssignment = actualValueAssignment.Replace("_placeholderName_", _placeholderName_);
                        break;
                    }
                case (System.Data.DbType.Guid):
                    {
                        _placeholderCtype_ = "System.Guid";
                        _placeholderProxyParameter_ = "\t\t\t";
                        if ("Input" != _placeholderDirection_)
                            _placeholderProxyParameter_ += "ref ";
                        _placeholderProxyParameter_ += _placeholderCtype_ + " " +_placeholderName_ + ",\r\n";
                        _placeholderDbtype_ = "Guid";
                        // Value
                        _placeholderProxyParValidity_ = "System.Guid.Empty != " + _placeholderName_;
                        actualValueAssignment = actualValueAssignment.Replace("_placeholderProxyParValidity_", _placeholderProxyParValidity_);
                        actualValueAssignment = actualValueAssignment.Replace("_placeholderNotnullAssignment_",
                            NotnullAssignmentNotbool.Replace("_placeholderName_", _placeholderName_));
                        actualValueAssignment = actualValueAssignment.Replace("_placeholderName_", _placeholderName_);
                        break;
                    }
                case (System.Data.DbType.Int16):
                    {
                        _placeholderCtype_ = "Int16";
                        _placeholderProxyParameter_ = "\t\t\t";
                        if ("Input" != _placeholderDirection_)
                            _placeholderProxyParameter_ += "ref ";
                        _placeholderProxyParameter_ += _placeholderCtype_ + " " + _placeholderName_ + ",\r\n";
                        _placeholderDbtype_ = "Int16";
                        // Value
                        if (this._filtering_integralTypes )
                        {
                            _placeholderProxyParValidity_ = "0<" + _placeholderName_;
                            actualValueAssignment = actualValueAssignment.Replace("_placeholderProxyParValidity_", _placeholderProxyParValidity_);
                            actualValueAssignment = actualValueAssignment.Replace("_placeholderNotnullAssignment_",
                                NotnullAssignmentNotbool.Replace("_placeholderName_", _placeholderName_));
                            actualValueAssignment = actualValueAssignment.Replace("_placeholderName_", _placeholderName_);
                        }
                        else// no filtering
                        {
                            actualValueAssignment = "\t\t\t"+NotnullAssignmentNotbool.Replace("_placeholderName_", _placeholderName_)+"\r\n";// no filtering
                        }
                        break;
                    }
                case (System.Data.DbType.Int32):
                    {
                        _placeholderCtype_ = "Int32";
                        _placeholderProxyParameter_ = "\t\t\t";
                        if ("Input" != _placeholderDirection_)
                            _placeholderProxyParameter_ += "ref ";
                        _placeholderProxyParameter_ += _placeholderCtype_ + " " + _placeholderName_ + ",\r\n";
                        _placeholderDbtype_ = "Int32";
                        // Value
                        if (this._filtering_integralTypes )// means if( (int<=0) must go as dbNull)
                        {
                            _placeholderProxyParValidity_ = "0<" + _placeholderName_;
                            actualValueAssignment = actualValueAssignment.Replace("_placeholderProxyParValidity_", _placeholderProxyParValidity_);
                            actualValueAssignment = actualValueAssignment.Replace("_placeholderNotnullAssignment_",
                                NotnullAssignmentNotbool.Replace("_placeholderName_", _placeholderName_));
                            actualValueAssignment = actualValueAssignment.Replace("_placeholderName_", _placeholderName_);
                        }
                        else// no filtering
                        {
                            actualValueAssignment = "\t\t\t"+NotnullAssignmentNotbool.Replace("_placeholderName_", _placeholderName_)+"\r\n";// no filtering
                        }
                        break;
                    }
                case (System.Data.DbType.Int64):
                    {
                        _placeholderCtype_ = "Int64";
                        _placeholderProxyParameter_ = "\t\t\t";
                        if ("Input" != _placeholderDirection_)
                            _placeholderProxyParameter_ += "ref ";
                        _placeholderProxyParameter_ += _placeholderCtype_ + " " + _placeholderName_ + ",\r\n";
                        _placeholderDbtype_ = "Int64";
                        // Value
                        if (this._filtering_integralTypes)
                        {
                            _placeholderProxyParValidity_ = "0L<" + _placeholderName_;
                            actualValueAssignment = actualValueAssignment.Replace("_placeholderProxyParValidity_", _placeholderProxyParValidity_);
                            actualValueAssignment = actualValueAssignment.Replace("_placeholderNotnullAssignment_",
                                NotnullAssignmentNotbool.Replace("_placeholderName_", _placeholderName_));
                            actualValueAssignment = actualValueAssignment.Replace("_placeholderName_", _placeholderName_);
                        }
                        else// no filtering
                        {
                            actualValueAssignment = "\t\t\t"+NotnullAssignmentNotbool.Replace("_placeholderName_", _placeholderName_)+"\r\n";// no filtering
                        }
                        break;
                    }
                case (System.Data.DbType.Object):
                    {
                        _placeholderCtype_ = "object";
                        _placeholderProxyParameter_ = "\t\t\t";
                        if ("Input" != _placeholderDirection_)
                            _placeholderProxyParameter_ += "ref ";
                        _placeholderProxyParameter_ += _placeholderCtype_ + " " + _placeholderName_ + ",\r\n";
                        _placeholderDbtype_ = "Object";
                        // Value
                        if (this._filtering_objects)
                        {
                            _placeholderProxyParValidity_ = "null!=" + _placeholderName_;// nonNull reference.
                            actualValueAssignment = actualValueAssignment.Replace("_placeholderProxyParValidity_", _placeholderProxyParValidity_);
                            actualValueAssignment = actualValueAssignment.Replace("_placeholderNotnullAssignment_",
                                NotnullAssignmentNotbool.Replace("_placeholderName_", _placeholderName_));
                            actualValueAssignment = actualValueAssignment.Replace("_placeholderName_", _placeholderName_);
                        }
                        else// no filtering
                        {
                            actualValueAssignment = "\t\t\t"+NotnullAssignmentNotbool.Replace("_placeholderName_", _placeholderName_)+"\r\n";// no filtering
                        }
                        break;
                    }
                case (System.Data.DbType.SByte):
                    {
                        _placeholderCtype_ = "System.SByte";
                        _placeholderProxyParameter_ = "\t\t\tSystem.SByte " + _placeholderName_ + ",\r\n";
                        _placeholderDbtype_ = "SByte";
                        break;
                    }
                case (System.Data.DbType.Single):
                    {
                        _placeholderCtype_ = "float";// == System.Single
                        _placeholderProxyParameter_ = "\t\t\t";
                        if ("Input" != _placeholderDirection_)
                            _placeholderProxyParameter_ += "ref ";
                        _placeholderProxyParameter_ += _placeholderCtype_ + " " + _placeholderName_ + ",\r\n";
                        _placeholderDbtype_ = "Single";
                        // Value
                        _placeholderProxyParValidity_ =
                            "float.MinValue!=" + _placeholderName_ +
                            " && float.MaxValue!=" + _placeholderName_ +
                            " && float.NaN!=" + _placeholderName_ +
                            " && float.PositiveInfinity!=" + _placeholderName_;
                        actualValueAssignment = actualValueAssignment.Replace("_placeholderProxyParValidity_", _placeholderProxyParValidity_);
                        actualValueAssignment = actualValueAssignment.Replace("_placeholderNotnullAssignment_",
                            NotnullAssignmentNotbool.Replace("_placeholderName_", _placeholderName_));
                        actualValueAssignment = actualValueAssignment.Replace("_placeholderName_", _placeholderName_);
                        break;
                    }
                case (System.Data.DbType.Time):
                    {
                        _placeholderCtype_ = "System.DateTime";
                        _placeholderProxyParameter_ = "\t\t\t";
                        if ("Input" != _placeholderDirection_)
                            _placeholderProxyParameter_ += "ref ";
                        _placeholderProxyParameter_ += _placeholderCtype_ + " " + _placeholderName_ + ",\r\n";
                        _placeholderDbtype_ = "Time";
                        break;
                    }
                case (System.Data.DbType.UInt16):
                    {
                        _placeholderCtype_ = "UInt16";
                        _placeholderProxyParameter_ = "\t\t\t";
                        if ("Input" != _placeholderDirection_)
                            _placeholderProxyParameter_ += "ref ";
                        _placeholderProxyParameter_ += _placeholderCtype_ + " " + _placeholderName_ + ",\r\n";
                        _placeholderDbtype_ = "UInt16";
                        break;
                    }
                case (System.Data.DbType.UInt32):
                    {
                        _placeholderCtype_ = "UInt32";
                        _placeholderProxyParameter_ = "\t\t\t";
                        if ("Input" != _placeholderDirection_)
                            _placeholderProxyParameter_ += "ref ";
                        _placeholderProxyParameter_ += _placeholderCtype_ + " " + _placeholderName_ + ",\r\n";
                        _placeholderDbtype_ = "UInt32";
                        break;
                    }
                case (System.Data.DbType.UInt64):
                    {
                        _placeholderCtype_ = "UInt64";
                        _placeholderProxyParameter_ = "\t\t\t";
                        if ("Input" != _placeholderDirection_)
                            _placeholderProxyParameter_ += "ref ";
                        _placeholderProxyParameter_ += _placeholderCtype_ + " " + _placeholderName_ + ",\r\n";
                        _placeholderDbtype_ = "UInt64";
                        break;
                    }
                case (System.Data.DbType.VarNumeric):
                    {
                        _placeholderCtype_ = "double";
                        _placeholderProxyParameter_ = "\t\t\t";
                        if ("Input" != _placeholderDirection_)
                            _placeholderProxyParameter_ += "ref ";
                        _placeholderProxyParameter_ += _placeholderCtype_ + " " + _placeholderName_ + ",\r\n";
                        _placeholderDbtype_ = "VarNumeric";
                        // Value
                        _placeholderProxyParValidity_ =
                            "double.MinValue!=" + _placeholderName_ +
                            " && double.MaxValue!=" + _placeholderName_ +
                            " && double.NaN!=" + _placeholderName_ +
                            " && double.PositiveInfinity!=" + _placeholderName_;
                        actualValueAssignment = actualValueAssignment.Replace("_placeholderProxyParValidity_", _placeholderProxyParValidity_);
                        actualValueAssignment = actualValueAssignment.Replace("_placeholderNotnullAssignment_",
                            NotnullAssignmentNotbool.Replace("_placeholderName_", _placeholderName_));
                        actualValueAssignment = actualValueAssignment.Replace("_placeholderName_", _placeholderName_);
                        break;
                    }
                case (System.Data.DbType.Xml):
                    {
                        _placeholderCtype_ = "System.Xml.XmlDocumentFragment";
                        _placeholderProxyParameter_ = "\t\t\t";
                        if ("Input" != _placeholderDirection_)
                            _placeholderProxyParameter_ += "ref ";
                        _placeholderProxyParameter_ += _placeholderCtype_ + " " + _placeholderName_ + ",\r\n";
                        _placeholderDbtype_ = "Xml";
                        break;
                    }
                default:
                    {
                        _placeholderCtype_ = "object";
                        _placeholderProxyParameter_ = "\t\t\t";
                        if ("Input" != _placeholderDirection_)
                            _placeholderProxyParameter_ += "ref ";
                        _placeholderProxyParameter_ += _placeholderCtype_ + " " + _placeholderName_ + ",\r\n";
                        _placeholderDbtype_ = "Object";
                        // Value
                        if (this._filtering_objects)
                        {
                            _placeholderProxyParValidity_ = "null!=" + _placeholderName_;// nonNull reference.
                            actualValueAssignment = actualValueAssignment.Replace("_placeholderProxyParValidity_", _placeholderProxyParValidity_);
                            actualValueAssignment = actualValueAssignment.Replace("_placeholderNotnullAssignment_",
                                NotnullAssignmentNotbool.Replace("_placeholderName_", _placeholderName_));
                            actualValueAssignment = actualValueAssignment.Replace("_placeholderName_", _placeholderName_);
                        }
                        else// no filtering
                        {
                            actualValueAssignment = "\t\t\t"+NotnullAssignmentNotbool.Replace("_placeholderName_", _placeholderName_)+"\r\n";// no filtering
                        }
                        break;
                    }
                    //  mappo tutti i tipi distinguendo la gestione dei dbNULL per in e per out.
            }// end SWITCH parameterToBeScanned.DbType -> _placeholderDbtype_
            //
        }// end scanSingleSqlParameter



        /// <summary>
        /// Takes the CodeBlock-hashtable.
        /// Scans each selectedProcedure-signature.
        /// Generates the C#-proxy of each.
        /// </summary>
        /// <param name="codeBlockTemplates">the CodeBlock-hashtable.</param>
        /// <returns>boolean writing-result</returns>
        private bool produceProxies(
            System.Collections.Hashtable codeBlockTemplates)
        {
            if (null == codeBlockTemplates)
                return false;// cannot generate code.
            bool result = true;// used in & at each proxy-generation.
            ListBox.SelectedObjectCollection selectedObjects = this.lbxProcedures.SelectedItems;
            //
            for (int c = 0; c < selectedObjects.Count; c++)//-------per ogni procedura selezionata------------
            {
                Couple couple = (Couple)(selectedObjects[c]);
                string name = couple.Name;
                int id = couple.Id;
                //
                bool connIsOpen = this.tryOpenConnection();
                if (!connIsOpen)
                    return false;
                //---funzione che restituisce il vettore di sqlParameters associato alla procedura-----
                System.Data.SqlClient.SqlParameter[] currentProcPars =
                    DataAccess.DISCOVER_PARAMETERS_SERVICE.DiscoverSpParameterSet(
                        this.conn,// member
                        couple.Name,
                        false); // include @RETURN_VALUE
                if (null == currentProcPars)// failed
                    continue;// with next loop-step.
                //-------------------------------------------------------------
                //---Chiamata alla procedura che prende a parametro il vettore di SqlParameter
                //----- e genera in memoria una copia del CodeBlock-template e vi applica le placeholder-replace.
                //
                //
                //------ per ogni procedura selezionata--(siamo gia' dentro questo ciclo)-----
                //---------per ogni sqlParameter----------------------------------------------
                //----Proxy params---------
                string proxyParameterList = "";// da costruire ex-novo. Tutto dinamico.
                //--END--Proxy params---------
                //---sqlParam---
                string sqlParameterList = "";// la template e' codeBlockTemplates[4]. Ad ogni giro placeholder_replacement e accodamento.
                string sqlParameterValueAssignment = (string)codeBlockTemplates["06_ProxyParValidity_.txt"];
                string NotnullAssignmentBool = (string)codeBlockTemplates["07A_NotnullAssignmentBool_.txt"];
                string NotnullAssignmentNotbool = (string)codeBlockTemplates["07B_NotnullAssignmentNotbool_.txt"];
                //-END--sqlParam---
                //--out--sqlParam recovery---
                string outParamsRecoveryList = "";// la template e' 10_OutParRecovery_optional.txt
                //
                for (int currentParameter = 0; currentParameter < currentProcPars.Length; currentParameter++)
                {
                    //
                    string actualSqlParameterBlock = (string)codeBlockTemplates["05_SqlParameter_many.txt"];
                    string actualoutParamRecovery = (string)codeBlockTemplates["10_OutParRecovery_optional.txt"];
                    //
                    string _placeholderName_ = null;
                    string _placeholderProxyParameter_ = null;
                    string _placeholderDirection_ = null;
                    string _placeholderDbtype_ = null;
                    string _placeholderCtype_ = null;
                    string _placeholderProxyParValidity_ = null;
                    string actualValueAssignment = sqlParameterValueAssignment;// take from prototype.
                    int sqlParameterSize = -1;
                    //-------------call scanSingleSqlParameter------------------------------------
                    this.scanSingleSqlParameter(
                        //
                        currentProcPars[currentParameter],
                        ref _placeholderName_,
                        ref _placeholderProxyParameter_,
                        ref _placeholderDirection_,
                        ref _placeholderDbtype_,
                        ref _placeholderCtype_,
                        ref _placeholderProxyParValidity_,
                        ref sqlParameterSize,
                        // Value assignment
                        ref actualValueAssignment,
                        NotnullAssignmentBool,
                        NotnullAssignmentNotbool
                        );
                    //----a valle della chiamata ho i placeholders valorizzati. Li uso per sostituire.-----
                    //--------nel proxyParameterList--------
                    proxyParameterList += _placeholderProxyParameter_;// tutto dinamico
                    //
                    //--------nel SqlParameterBlock--------
                    actualSqlParameterBlock = actualSqlParameterBlock.Replace("_placeholderName_", _placeholderName_);
                    actualSqlParameterBlock = actualSqlParameterBlock.Replace("_placeholderDirection_", _placeholderDirection_);
                    actualSqlParameterBlock = actualSqlParameterBlock.Replace("_placeholderDbtype_", _placeholderDbtype_);
                    sqlParameterList += actualSqlParameterBlock;// accodamento.
                    if (_placeholderDirection_ == "Input" || _placeholderDirection_ == "InputOutput")
                    {
                        sqlParameterList += actualValueAssignment;// accodamento ValueAssignment.
                    }// else "Output" || "Return_Value" -> do not assign the Value.
                    // Assign length, on out strings.
                    if (_placeholderDirection_ == "Output" || _placeholderDirection_ == "InputOutput")
                    {
                        if (_placeholderDbtype_ == "String" || _placeholderDbtype_ == "Boolean")
                        {
                            if (0 >= sqlParameterSize)
                            {
                                sqlParameterSize = 50;// default to patch "boolean-strings".
                                sqlParameterList += "\t\t\tpar" + _placeholderName_ + ".Size = " + sqlParameterSize.ToString() + ";\r\n";// size is compulsory for OUT strings.";
                            }// else sqlParameterSize is already correct.
                        }// else length is fixed.
                    }
                    //
                    //-------actualoutParamRecovery---rilettura parametri output------------
                    if ("Input" != _placeholderDirection_)// Out, InOut, Ret -> accodamento.
                    {
                        actualoutParamRecovery = actualoutParamRecovery.Replace("_placeholderName_", _placeholderName_);
                        actualoutParamRecovery = actualoutParamRecovery.Replace("_placeholderCtype_", _placeholderCtype_);
                        if ("Decimal" == _placeholderDbtype_)
                        {
                            if (this._filtering_sqlIdentitiesAsNumeric)
                            {
                                int indexWhereCtypeBegins = actualoutParamRecovery.LastIndexOf("(Int64)");
                                if (0 <= indexWhereCtypeBegins)
                                {
                                    int CtypeLength = "(Int64)".Length;
                                    string coda = actualoutParamRecovery.Substring(indexWhereCtypeBegins + CtypeLength);// until end
                                    actualoutParamRecovery = actualoutParamRecovery.Substring(0, indexWhereCtypeBegins + CtypeLength);
                                    actualoutParamRecovery += "(decimal)";
                                    actualoutParamRecovery += coda;
                                }// else mistake not found -> no correction.
                            }// else do nothing
                        }// else e' gia' completo.
                        if ("Boolean" == _placeholderDbtype_)
                        {
                            if (this._filtering_booleanStrings)
                            {
                                actualoutParamRecovery = actualoutParamRecovery.Substring(0, actualoutParamRecovery.LastIndexOf(';'));
                                actualoutParamRecovery += ".ToString();\r\n";
                                actualoutParamRecovery += "                //\r\n";
                            }// else e' gia' completo.
                        }// else e' gia' completo.
                        // anyway: accodamento;
                        outParamsRecoveryList += actualoutParamRecovery;// accodamento.
                    }// else Input -> nessun accodamento.
                }//----end for each parameter in current proc---------------------------
                //-------concatenare i codeBlock-template con i blocchi customizzati e fare la scrittura--
                //--- blocco di scrittura---------------
                //--------distinzione fra codeBlock originali e modificati-------------
                string[] actualBlocks = new string[nTokens];
                actualBlocks[00] = (string)codeBlockTemplates["01_ClassEnvelope_UntilSignature_.txt"];
                actualBlocks[00] = actualBlocks[00].Replace("_placeholderProcname_", couple.Name);
                if (this.rbtMultipleResultset.Checked) // rbtMultipleResultset
                {
                    actualBlocks[00] = actualBlocks[00].Replace("_placeholderReturnType_", "System.Data.DataSet");
                }
                else if (this.rbtSingleResultset.Checked) // rbtSingleResultset
                {
                    actualBlocks[00] = actualBlocks[00].Replace("_placeholderReturnType_", "System.Data.DataTable");
                }
                else if (this.rbtNoResultset.Checked) // rbtNoResultset
                {
                    actualBlocks[00] = actualBlocks[00].Replace("_placeholderReturnType_", "int");
                }
                // else already thrown 
                //--------------
                if (proxyParameterList.Length >= 3)// never less than 3 chars for a proxyInputPar( e.g. int, bit,..)
                {// without this "if", throws on no-proxy-pars.
                    proxyParameterList = proxyParameterList.Substring(0, proxyParameterList.Length - 3);// blocco dinamico, incastrato qui.// proxyParameterList
                }// else no input params for this proxy.
                actualBlocks[00] += proxyParameterList;
                if (this.chkAllowTransaction.Checked)
                {
                    if (proxyParameterList.Length >= 3)// never less than 3 chars for a proxyInputPar( e.g. int, bit,..)
                        actualBlocks[00] += ",\r\n";// if prev params, put separators.
                    // anyway( prev params or not) concat the following.
                    actualBlocks[00] += "\t\t\tSystem.Data.SqlClient.SqlTransaction trx";
                }// else no transaction to be appended to the proxy-params-list.
                //

                actualBlocks[01] = (string)codeBlockTemplates["02_ProxyParameterList_dynamc.txt"];
                ////
                if (this.rbtMultipleResultset.Checked || this.rbtSingleResultset.Checked )// has resultset( single or multiple).
                {
                    if (this.chkAllowTransaction.Checked)
                    {
                        actualBlocks[02] = (string)codeBlockTemplates[@"Read\03trx_ReadSqlCommand.txt"];
                    }
                    else// no transaction
                    {
                        actualBlocks[02] = (string)codeBlockTemplates[@"Read\03_ReadSqlCommand.txt"];
                    }
                    actualBlocks[02] = actualBlocks[02].Replace("_placeholderConnStrName_", this.connectionStringName);
                    actualBlocks[02] = actualBlocks[02].Replace("_placeholderProcname_", couple.Name);
                    ////
                    actualBlocks[03] = (string)codeBlockTemplates[@"Read\04_ResultSet.txt"];
                    if (this.rbtMultipleResultset.Checked)
                    {
                        actualBlocks[03] = actualBlocks[03].Replace("_placeholderReturnType_", "System.Data.DataSet");
                    }
                    else if (this.rbtSingleResultset.Checked)
                    {
                        actualBlocks[03] = actualBlocks[03].Replace("_placeholderReturnType_", "System.Data.DataTable");
                    }
                    // no else: already filtered.
                }
                else// this.rbtNoResultset -> write -> status-int result.
                {
                    if (this.chkAllowTransaction.Checked)
                    {
                        actualBlocks[02] = (string)codeBlockTemplates[@"Write\03trx_WriteSqlCommand.txt"];
                    }
                    else// no transaction
                    {
                        actualBlocks[02] = (string)codeBlockTemplates[@"Write\03_WriteSqlCommand.txt"];
                    }
                    actualBlocks[02] = actualBlocks[02].Replace("_placeholderConnStrName_", this.connectionStringName);
                    actualBlocks[02] = actualBlocks[02].Replace("_placeholderProcname_", couple.Name);
                    ////
                    actualBlocks[03] = (string)codeBlockTemplates[@"Write\04_IntegerResult.txt"];
                }
                ////
                actualBlocks[04] = sqlParameterList;// blocco customizzato per accodamento. -> 5 non preso in orginale.
                ////
                //  i 6,7A,7B sno ValueAssignment e sono accodati dinamicamente al 5.
                ////
                actualBlocks[05] = (string)codeBlockTemplates["08_OpenTry.txt"];
                ////
                if (this.rbtMultipleResultset.Checked || this.rbtSingleResultset.Checked)// has resultset( single or multiple).
                {
                    actualBlocks[06] = (string)codeBlockTemplates[@"Read\09_ReadTry.txt"];
                }
                else// this.rbtNoResultset -> write -> status-int result.
                {
                    actualBlocks[06] = (string)codeBlockTemplates[@"Write\09_WriteTry.txt"];
                }
                ////
                actualBlocks[07] = outParamsRecoveryList;//--rilettura parametri output--(may be empty)-------
                ////
                actualBlocks[08] = (string)codeBlockTemplates["11_CloseTry_OpenCatch.txt"];
                ////
                if (this.rbtMultipleResultset.Checked || this.rbtSingleResultset.Checked)// isRead
                {
                    actualBlocks[09] = (string)codeBlockTemplates[@"Read\12_ReadCatch.txt"];
                    actualBlocks[09] = actualBlocks[09].Replace("_placeholderProcname_", couple.Name);
                    ////
                    if (this.chkAllowTransaction.Checked)
                    {
                        actualBlocks[10] = (string)codeBlockTemplates[@"Read\13trx_Read_Shutting.txt"];
                    }
                    else// no transaction
                    {
                        actualBlocks[10] = (string)codeBlockTemplates[@"Read\13_Read_Shutting.txt"];
                    }
                }
                else// this.rbtNoResultset -> write -> status-int result.
                {
                    actualBlocks[09] = (string)codeBlockTemplates[@"Write\12_WriteCatch.txt"];
                    actualBlocks[09] = actualBlocks[09].Replace("_placeholderProcname_", couple.Name);
                    ////
                    if (this.chkAllowTransaction.Checked)
                    {
                        actualBlocks[10] = (string)codeBlockTemplates[@"Write\13trx_Write_Shutting.txt"];
                    }
                    else// no transaction
                    {
                        actualBlocks[10] = (string)codeBlockTemplates[@"Write\13_Write_Shutting.txt"];
                    }
                }
                ////
                actualBlocks[11] = "";// for future developements.
                actualBlocks[12] = "";
                //--------------------- ready to write down.------------------------
                System.IO.StreamWriter sw = null;
                try
                {
                    //--- parametrizzo il ProxyName sul ProcedureName--------------
                    sw = new System.IO.StreamWriter(
                        this.chosen_ProxiesFullPath + @"\" + couple.Name + "_SERVICE.cs",
                        false);// is append.
                    if (null != sw)
                    {
                        for (int currentCodeBlock = 0; currentCodeBlock < nTokens; currentCodeBlock++)
                        {
                            sw.Write(actualBlocks[currentCodeBlock]);
                        }
                        sw.Flush();
                        sw.Close();
                        result &= true;
                    }//----end---scrittura------
                    else
                    {
                        result &= false;
                    }
                }
                catch (System.Exception ex)
                {
                    string outerMessage = ex.Message;
                    string stackTrace = ex.StackTrace;
                    string innerMessage = null;
                    if (null != ex.InnerException)
                    {
                        innerMessage = ex.InnerException.Message;
                    }
                    result &= false;
                    //
                    MessageBox.Show(
                        "Cannot open file in write-mode, to generate the code.\r\n\r\n\r\n"+
                        "-----------------------------------------------------------------------\r\n"+
                        "Check that the directory or files are not write protected and\r\n"+
                        "that your account has write permissions there.\r\n\r\n\r\n"+
                        "-----------------------------------------------------------------------\r\n" +
                        outerMessage +
                        "-----------------------------------------------------------------------\r\n" +
                        innerMessage +
                        "-----------------------------------------------------------------------\r\n" +
                        stackTrace +"\r\n",
                        //
                        "Error" // caption
                    );
                }
                //finally
                //{       nothing to do.
                //}
            }//-----end for each selected procedure---------------------------------------
            //
            return result;
        }// end produceProxies


        #endregion traduzione



        #region interfaccia


        public frmDbCockpit()
        {
            InitializeComponent();
        }


        private void patchWithEndingBackSl( ref string par)
        {
            char lastPathChar = par[ par.Length - 1];
            if ('\\' != lastPathChar)
                par += @"\";
        }// end patchWithEndingBackSl


        private void readDefaultProxyPath()
        {
            try
            {
                // read into a member, the path to write down the proxies into.
                ConfigurationLayer.ConfigurationService proxyPath = new ConfigurationLayer.ConfigurationService(
                    "fileSystemPaths/Proxies");
                this.chosen_ProxiesFullPath =
                    proxyPath.GetStringValue("path");
                // append the backslash, if absent.
                patchWithEndingBackSl(ref this.chosen_ProxiesFullPath);
            }
            catch (System.Exception ex)
            {
                string s = ex.Message;// dbg & log.
                this.chosen_ProxiesFullPath = null;
            }
            if (null == this.chosen_ProxiesFullPath)// search for most common environment variables:
            {// TEMP or TMP.
                string environmentToQuery = "TEMP";
                this.chosen_ProxiesFullPath =
                    System.Environment.GetEnvironmentVariable(
                        environmentToQuery, System.EnvironmentVariableTarget.User);
                // append the backslash, if absent.
                patchWithEndingBackSl(ref this.chosen_ProxiesFullPath);
                //
                if (null == this.chosen_ProxiesFullPath || "" == this.chosen_ProxiesFullPath)
                {
                    environmentToQuery = "TMP";
                    this.chosen_ProxiesFullPath =
                        System.Environment.GetEnvironmentVariable(
                            environmentToQuery, System.EnvironmentVariableTarget.User);
                    // append the backslash, if absent.
                    patchWithEndingBackSl(ref this.chosen_ProxiesFullPath);
                }// else ProxyPath already found-----------------------------
            }// else ProxyPath already found in App.Config-------------------
            //NB. if neither App.config, nor EnvironmentVariables have been found, the choise will be requested
            //    at the user interface, on db-connection opening.
            //    In either case, menuStrip voice is available to override existing configuration.
        }// end readDefaultProxyPath


        private void DbCockpit_Load(object sender, EventArgs e)
        {
            //---start file system path search------------------
            this.readDefaultProxyPath();
            //---end file system path search------------------
            //
            // read into a member, the path of code-blocks.
            ConfigurationLayer.ConfigurationService cs = new ConfigurationLayer.ConfigurationService(
                "fileSystemPaths/CodeBlocks");
            this.configured_CodeBlocksPath = cs.GetStringValue("path");
            // append the backslash, if absent.
            patchWithEndingBackSl( ref this.configured_CodeBlocksPath);
            //
            System.Collections.Specialized.NameValueCollection connectionStringsSection =
                ConfigurationLayer2008.CustomSectionInOneShot.GetCustomSectionInOneShot(
                    "ProxyGeneratorConnections/strings");// this section name is compulsory for client applications, fed by ProxyGenerator.
            //
            int cardConfiguredStrings = connectionStringsSection.Count;
            this.configuredStrings = new System.Collections.ArrayList();
            //new string[cardConfiguredStrings];
            this.cmbConnectionString.Items.Clear();
            for (int c = 0; c < cardConfiguredStrings; c++)
            {
                Couple tmp = new Couple(
                    connectionStringsSection.AllKeys[c],
                    connectionStringsSection[ connectionStringsSection.AllKeys[c]]
                );
                this.configuredStrings.Add(tmp);// add to member
                this.cmbConnectionString.Items.Add(tmp);  // add to combo
            }
            ////------SQLEXPRESS connString has a different syntax and requires adaptation. TODO
            // generally SQLEXPRESS connects to .mdf file.
            this.cmbConnectionString.SelectedIndex = 0;// the first configured string is selected by default.
        }



        /// <summary>
        /// change displayed tokens, according to new selected connection string.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.activateSelectedConnString(
                ((Couple)this.cmbConnectionString.SelectedItem).ConnectionString
            );
        }



        /// <summary>
        /// ricongiunge i campi splittati nelle textbox -> compone la stringa di connessione selezionata da combo.
        /// Verra' passata per nome ai proxies.
        /// L'applicazione che usa i proxies deve avere una sezione ConnectionStrings che contenga
        /// le stringhe presenti nel config del ProxyGenerator.
        /// </summary>
        /// <returns></returns>
        private string composeSelectedConnString()
        {
            string composedConnectionString = (
                this.lblDatabase.Text + this.txtDatabase.Text + ";" +
                this.lblServer.Text + this.txtServer.Text + ";" +
                this.lblUsername.Text + this.txtUsername.Text + ";" +
                this.lblPassword.Text + this.txtPassword.Text
            );
            //
            this.connectionStringName = this.cmbConnectionString.SelectedItem.ToString();
            this.connectionStringValue = composedConnectionString;
            return composedConnectionString;
        }//



        /// <summary>
        /// fa lo split della stringa di connessione, nei campi.
        /// </summary>
        /// <param name="selectedConnection"></param>
        private void activateSelectedConnString(string selectedConnection)
        {
            string[] choosenConnection = selectedConnection.Split(';');
            //
            this.lblDatabase.Text = choosenConnection[0].Substring(0, choosenConnection[0].IndexOf('=') + 1);
            this.txtDatabase.Text = choosenConnection[0].Substring(choosenConnection[0].IndexOf('=') + 1);
            //
            this.lblServer.Text = choosenConnection[1].Substring(0, choosenConnection[1].IndexOf('=') + 1);
            this.txtServer.Text = choosenConnection[1].Substring(choosenConnection[1].IndexOf('=') + 1);
            //
            this.lblUsername.Text = choosenConnection[2].Substring(0, choosenConnection[2].IndexOf('=') + 1);
            this.txtUsername.Text = choosenConnection[2].Substring(choosenConnection[2].IndexOf('=') + 1);
            //
            this.lblPassword.Text = choosenConnection[3].Substring(0, choosenConnection[3].IndexOf('=') + 1);
            this.txtPassword.Text = choosenConnection[3].Substring(choosenConnection[3].IndexOf('=') + 1);
            //
        }




        private void selectDatabaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //------start send warning------------------------------------------------------
            //---if neither App.config nor EnvironmentVariable information were found -> warn to choose ProxyPath.
            if (null == this.chosen_ProxiesFullPath)
            {
                MessageBox.Show(
                    "Choose the directory where You want the proxies be writtenn.",
                    "Choose Working Folder" // caption==titolo.
                );
                //
                this.folderBrowserDialog1.ShowDialog(this);// choose path to dump proxies on.
                this.chosen_ProxiesFullPath = this.folderBrowserDialog1.SelectedPath;
            }// else a valid choice has already been performed. Overload by menu, always allowed.
            //------end send warning---------------------------------------------------------
            //
            this.tryOpenConnection();// write into member.
            // NB. on requested db, not on model or master
            System.Data.DataTable dt = DataAccess.GET_PROCEDURES_SERVICE.GET_PROCEDURES(
                this.conn);
            if (null == dt)
                return;// exit without binding on no-conn.
            //------------------- end query x elenco stored -----------------------------------------//
            //
            this.lbxProcedures.Items.Clear();
            for (int c = 0; c < dt.Rows.Count; c++)
            {
                Couple couple = new Couple(
                    dt.Rows[c]["name"].ToString(),
                    (int)dt.Rows[c]["id"]
                );
                this.lbxProcedures.Items.Add(couple);
            }
        }//


        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();// calls this.Dispose();
        }

        //----NB. nothing to do here. Every index selected raises the event. It's good to do NOT have
        // a delegate. Wait confirmation button.
        //
        //private void lbxProcedures_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    ListBox.SelectedIndexCollection  selectedIndices = this.lbxProcedures.SelectedIndices;
        //    ListBox.SelectedObjectCollection selectedObjects = this.lbxProcedures.SelectedItems;
        //}


        /// <summary>
        /// initializes a data member, that will be reopened each time.
        /// </summary>
        private bool tryOpenConnection()
        {
            bool result = false;
            //
            if (null != this.conn)
            {
                if (System.Data.ConnectionState.Open == this.conn.State)
                    return true;//already up & running.
                // else !=null but closed -> entrare in try{} per aprire.
            }
            else
            {
                this.conn = new System.Data.SqlClient.SqlConnection();
            }
            //
            try
            {//---compose the connection string with the tokens contained in the text boxes------
                conn.ConnectionString = this.composeSelectedConnString();
                conn.Open();
                //this.connectionStringName     valorizzati entrambi dalla composeSelectedConnString().
                //this.connectionStringValue
                result = true;
            }
            catch( System.Exception ex)
            {
                MessageBox.Show(
                    "Error while trying open selected db-connection.\r\n" +
                    "Check the service of the selected sql-insance be running.\r\n" +
                    "Check IP/hostname.\r\n" +
                    "Check Sql-Instance-name.\r\n" +
                    "Check username, password.\r\n" +
                    "If You don't see the desired connection string in the drop-down, it's because You forgot to add it to the app.config file.\r\n" +
                    "Remember to adjust the \"CodeBlocks\\path\" entry in the app.config file, for your needs.\r\n" +
                    ex.Message + "\r\n\t" +
                    ex.Source,
                    "Error");
                result = false;
            }
            //
            return result;
        }// end tryOpenConnection


        private void btnConfirmSelection_Click(object sender, EventArgs e)
        {
            bool tryOpenResult = this.tryOpenConnection();
            if (!tryOpenResult)
                return;// on form
            //
            ListBox.SelectedObjectCollection selectedObjects = this.lbxProcedures.SelectedItems;
            //
            //-----------validation-------------------------------------
            if (0 == selectedObjects.Count)
            {
                
                MessageBox.Show("You have to choose at least one stored procedure, for C#-proxy code generation.", "Error");
                return;
            }
            if (false == this.rbtMultipleResultset.Checked && false == this.rbtSingleResultset.Checked && false == this.rbtNoResultset.Checked )
            {
                MessageBox.Show(
                    "You have to choose the category( MultipleResultset, SingleResultSet, NoResultset).\r\n\r\n\r\n" +
                    "Some documentation about usage:\r\n"+
                    "-------------------------------------------------------:\r\n" +
                    "Resultset means that the considered procedure(s) perform some query,\r\n"+
                    "then You expect one ore more table-like resultsets.\r\n" +
                    "If your procedure performs a single query choose SingleResultSet.The resultset is a single DataTable\r\n"+
                    "It's the most frequent kind of \"Load-type\" procedure.\r\n" +
                    "Instead if You plan to use a procedure that performs more than one query\r\n"+
                    "choose \"MultipleResultset\"." +
                    "Your multiple resultsets will be stored one in each contained DataTable,\r\n"+
                    "following the Microsoft agreement that the first one is called \"Table\",\r\n"+
                    "the second  \"Table1\", and so on.\r\n" +
                    "NoResultset means that your procedure performs some insert or update operation\r\n"+
                    "or some filling up of OUT direction scalar parameters."+
                    "In such situation You will receive a detailed error-map as return value.\r\n"+
                    "It's formed of an integer value, whose meaning is quoted in the proxy source.\r\n"+
                    "This should help to monitor exceptions while writing on the database.\r\n"+
                    "A proxy generated with Resultset options can perform writings and then also fill\r\n"+
                    "a resultset, but such proxy will miss the detailed exception-mapping integer as return value.\r\n"+
                    "In such cases I advice to split the activity into two procedures of the appropriate cetegories."+
                    "\r\n\r\n" +
                    "-------------------------------------------------------:\r\n" +
                    "The admitTransaction flag is compatible with both the categories.\r\n"+
                    "It means that the procedure will not manage an autonomous connection,\r\n"+
                    "but will make use of the transaction parameter, if it's not null.\r\n"+
                    "A null value of such parameter lets the procedure manage\r\n"+
                    "an autonomous connection just as the admitTransaction flag were not checked."+
                    "\r\n\r\n" +
                    "-------------------------------------------------------:\r\n" +
                    "When many stored procedures are choosen from the list, the selected options\r\n"+
                    "apply to all of the generated proxies.",
                    //
                    "Error");
                return;
            }
            //-------END----validation-------------------------------------
            //---Take blocks into memory-------------------------------------//
            //------they will be used as templates, making a copy and replacing the placeholders
            //------into each one of them.
            //---The template-blocks are unique.
            //------each procedure will build its customized blocks, link up them into a single string
            //------and write down to disk.
            //
            // tre casi: {0=MultipleResultSet, 1=SingleResultSet, 2=NoResultSet}
            byte retTypeSwitch = 0;// by default return a DataSet. It's expensive, but always fits.
            if (this.rbtMultipleResultset.Checked)
            {
                retTypeSwitch = 0;
            }
            else if (this.rbtSingleResultset.Checked)
            {
                retTypeSwitch = 1;
            }
            else if (this.rbtNoResultset.Checked)
            {
                retTypeSwitch = 2;
            }
            // no else: no more cases.
            System.Collections.Hashtable codeBlockTemplates = this.takeTokens(
                retTypeSwitch,// tre casi: {0=MultipleResultSet, 1=SingleResultSet, 2=NoResultSet}
                this.chkAllowTransaction.Checked //  allow transaction
            );
            //--END-Take blocks into memory-------------------------------------//
            if (null == codeBlockTemplates)
                return;// on the user interface. No more action:cannot generate code.
            // else ready to
            bool proxyWritingResult =
                this.produceProxies(codeBlockTemplates);
            if (!proxyWritingResult)
                MessageBox.Show("In some of the C#-Proxies occurred an error.", "Warning");
            else
                MessageBox.Show("All of the C#-Proxies were correctly generated.", "OK");
        }// end btnConfirmSelection_Click


        private void chkNonDefaultFiltering_CheckedChanged(object sender, EventArgs e)
        {
            if (this.chkNonDefaultFiltering.Checked)
            {
                frmNonDefaultFiltering nonDefaultFilters =
                    //---parameter filtering---
                    /// 1 == BooleanStrings
                    /// 2 == Integer Types
                    /// 3 == Objects
                    /// 4 == Sql Identities as Numeric(x,y)
                    new frmNonDefaultFiltering(
                        );
                nonDefaultFilters.ShowDialog(this);
                //
                //---start------- on exit of child modal form, read results.-------
                //---parameter filtering---
                /// 1 == BooleanStrings
                /// 2 == Integral Types
                /// 3 == Objects
                /// 4 == Sql Identities as Numeric(x,y)
                this._filtering_booleanStrings = nonDefaultFilters.BooleanStrings;
                this._filtering_integralTypes = nonDefaultFilters.IntegralTypes;
                this._filtering_objects = nonDefaultFilters.Objects;
                this._filtering_sqlIdentitiesAsNumeric = nonDefaultFilters.SqlIdentitiesAsNumeric;
                //---end on exit of child modal form, read results.-------
            }
            else
            {
                // do nothing, but reset to default all class members about filtering.
            }
        }// end chkNonDefaultFiltering_CheckedChanged

        private void chooseWorkingFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //
            this.folderBrowserDialog1.ShowDialog( this);// choose path to dump proxies on.
            this.chosen_ProxiesFullPath = this.folderBrowserDialog1.SelectedPath;
            // onCancel -> path=="" -> call readDefaultProxyPath
            if (null == this.chosen_ProxiesFullPath || "" == this.chosen_ProxiesFullPath)
                this.readDefaultProxyPath();
        }// end chooseWorkingFolderToolStripMenuItem_Click


        #endregion interfaccia



    }// end class
}
