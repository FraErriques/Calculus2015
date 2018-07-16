using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;



namespace GraphicsLib
{
	/// <summary>
    /// NB. the class cannot be "abstract" in vs2008, since the designer does not admit
    /// to treat an abstract object.
    /// The f() method, which was abstract and was the reason to let the whole class abstract,
    /// can conveniently be classified "virtual".
    /// 
    /// NB. the calculation methods, that perform the drawing, must have in the signature the arguments
    /// of the delegates, and be called by an actual delegate, receiving its arguments. eg.
    /// actualDelegate( object sender, System.EventArgs e)
    /// calls 
    /// drawer( object sender, System.EventArgs e, ..., some other necessary parameters)
    /// the call of a drawer-kind method, without providing the delegate parameters, causes
    /// a re-paint system event, which cancels the pictureBox.
    /// 
    /// old comment:
	/// GraphicsDevice is an abstract form, it's the blackboard template.
	/// Logging: to configure the service, use the App.Config of the AppDomain entry point.
	/// </summary>
	public class GraphicsDevice : System.Windows.Forms.Form
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		// board
		private System.Windows.Forms.PictureBox pctBoard;
		// txt range
		protected System.Windows.Forms.TextBox txtDesiredMinX;
		protected System.Windows.Forms.TextBox txtDesiredMaxX;
		protected System.Windows.Forms.TextBox txtDesiredMinY;
		protected System.Windows.Forms.TextBox txtDesiredMaxY;
		// btn calc
		private System.Windows.Forms.Button btnCalc_f0;
		private System.Windows.Forms.Button btnCalc_f1;
		private System.Windows.Forms.Button btnCalc_f2;
		private System.Windows.Forms.Button btnClear;
		// txt dx
		protected System.Windows.Forms.TextBox txt_dxF0;
		protected System.Windows.Forms.TextBox txt_dxF1;
		protected System.Windows.Forms.TextBox txt_dxF2;
		// algorithmic globals
		private float ax,ay, bx,by;
		private float _maxX;
		// suggested dx is 1/omothetiaFactorX
		protected float dx_f0;
		protected float dx_f1;
		protected float dx_f2;
		//
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		// the calc delegate
		private delegate float current_f( float x);



		// Ctor
		public GraphicsDevice()
		{
			// Required for Windows Form Designer support
			InitializeComponent();
		}// end Ctor


		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.pctBoard = new System.Windows.Forms.PictureBox();
			this.txtDesiredMinX = new System.Windows.Forms.TextBox();
			this.txtDesiredMaxX = new System.Windows.Forms.TextBox();
			this.txtDesiredMinY = new System.Windows.Forms.TextBox();
			this.txtDesiredMaxY = new System.Windows.Forms.TextBox();
			this.btnClear = new System.Windows.Forms.Button();
			this.btnCalc_f0 = new System.Windows.Forms.Button();
			this.txt_dxF0 = new System.Windows.Forms.TextBox();
			this.txt_dxF1 = new System.Windows.Forms.TextBox();
			this.btnCalc_f1 = new System.Windows.Forms.Button();
			this.btnCalc_f2 = new System.Windows.Forms.Button();
			this.txt_dxF2 = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// pctBoard
			// 
			this.pctBoard.Location = new System.Drawing.Point(8, 8);
			this.pctBoard.Name = "pctBoard";
			this.pctBoard.Size = new System.Drawing.Size(856, 490);
			this.pctBoard.TabIndex = 0;
			this.pctBoard.TabStop = false;
			// 
			// txtDesiredMinX
			// 
			this.txtDesiredMinX.Location = new System.Drawing.Point(872, 24);
			this.txtDesiredMinX.Name = "txtDesiredMinX";
			this.txtDesiredMinX.Size = new System.Drawing.Size(56, 20);
			this.txtDesiredMinX.TabIndex = 1;
			this.txtDesiredMinX.Text = "MinX";
			// 
			// txtDesiredMaxX
			// 
			this.txtDesiredMaxX.Location = new System.Drawing.Point(872, 56);
			this.txtDesiredMaxX.Name = "txtDesiredMaxX";
			this.txtDesiredMaxX.Size = new System.Drawing.Size(56, 20);
			this.txtDesiredMaxX.TabIndex = 2;
			this.txtDesiredMaxX.Text = "MaxX";
			// 
			// txtDesiredMinY
			// 
			this.txtDesiredMinY.Location = new System.Drawing.Point(872, 104);
			this.txtDesiredMinY.Name = "txtDesiredMinY";
			this.txtDesiredMinY.Size = new System.Drawing.Size(56, 20);
			this.txtDesiredMinY.TabIndex = 3;
			this.txtDesiredMinY.Text = "MinY";
			// 
			// txtDesiredMaxY
			// 
			this.txtDesiredMaxY.Location = new System.Drawing.Point(872, 136);
			this.txtDesiredMaxY.Name = "txtDesiredMaxY";
			this.txtDesiredMaxY.Size = new System.Drawing.Size(56, 20);
			this.txtDesiredMaxY.TabIndex = 4;
			this.txtDesiredMaxY.Text = "MaxY";
			// 
			// btnClear
			// 
			this.btnClear.Location = new System.Drawing.Point(872, 360);
			this.btnClear.Name = "btnClear";
			this.btnClear.Size = new System.Drawing.Size(56, 20);
			this.btnClear.TabIndex = 8;
			this.btnClear.Text = "cls";
			this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
			// 
			// btnCalc_f0
			// 
			this.btnCalc_f0.Location = new System.Drawing.Point(872, 408);
			this.btnCalc_f0.Name = "btnCalc_f0";
			this.btnCalc_f0.Size = new System.Drawing.Size(56, 20);
			this.btnCalc_f0.TabIndex = 5;
			this.btnCalc_f0.Text = "f0";
			this.btnCalc_f0.Click += new System.EventHandler(this.btnCalc_f0_Click);
			// 
			// txt_dxF0
			// 
			this.txt_dxF0.Location = new System.Drawing.Point(872, 232);
			this.txt_dxF0.Name = "txt_dxF0";
			this.txt_dxF0.Size = new System.Drawing.Size(56, 20);
			this.txt_dxF0.TabIndex = 9;
			this.txt_dxF0.Text = ".01";
			// 
			// txt_dxF1
			// 
			this.txt_dxF1.Location = new System.Drawing.Point(872, 272);
			this.txt_dxF1.Name = "txt_dxF1";
			this.txt_dxF1.Size = new System.Drawing.Size(56, 20);
			this.txt_dxF1.TabIndex = 10;
			this.txt_dxF1.Text = ".01";
			// 
			// btnCalc_f1
			// 
			this.btnCalc_f1.Location = new System.Drawing.Point(872, 440);
			this.btnCalc_f1.Name = "btnCalc_f1";
			this.btnCalc_f1.Size = new System.Drawing.Size(56, 20);
			this.btnCalc_f1.TabIndex = 6;
			this.btnCalc_f1.Text = "f1";
			this.btnCalc_f1.Click += new System.EventHandler(this.btnCalc_f1_Click);
			// 
			// btnCalc_f2
			// 
			this.btnCalc_f2.Location = new System.Drawing.Point(872, 472);
			this.btnCalc_f2.Name = "btnCalc_f2";
			this.btnCalc_f2.Size = new System.Drawing.Size(56, 20);
			this.btnCalc_f2.TabIndex = 7;
			this.btnCalc_f2.Text = "f2";
			this.btnCalc_f2.Click += new System.EventHandler(this.btnCalc_f2_Click);
			// 
			// txt_dxF2
			// 
			this.txt_dxF2.Location = new System.Drawing.Point(872, 312);
			this.txt_dxF2.Name = "txt_dxF2";
			this.txt_dxF2.Size = new System.Drawing.Size(56, 20);
			this.txt_dxF2.TabIndex = 11;
			this.txt_dxF2.Text = ".01";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(872, 216);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(56, 14);
			this.label1.TabIndex = 12;
			this.label1.Text = "dx f0";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(872, 256);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(56, 14);
			this.label2.TabIndex = 13;
			this.label2.Text = "dx f1";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(872, 296);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(56, 14);
			this.label3.TabIndex = 14;
			this.label3.Text = "dx f2";
			// 
			// GraphicsDevice
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(936, 517);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.txt_dxF2);
			this.Controls.Add(this.txt_dxF1);
			this.Controls.Add(this.txt_dxF0);
			this.Controls.Add(this.txtDesiredMaxY);
			this.Controls.Add(this.txtDesiredMinY);
			this.Controls.Add(this.txtDesiredMaxX);
			this.Controls.Add(this.txtDesiredMinX);
			this.Controls.Add(this.btnCalc_f2);
			this.Controls.Add(this.btnCalc_f1);
			this.Controls.Add(this.btnCalc_f0);
			this.Controls.Add(this.btnClear);
			this.Controls.Add(this.pctBoard);
			this.Name = "GraphicsDevice";
			this.Text = "GraphicsDevice";
			this.ResumeLayout(false);

		}
		#endregion



		/// <summary>
		/// Nota Bene: questo metodo deve essere reimplementato dalla form figlia, che
		/// concretizza la forma funzionale.
		/// </summary>
		/// <param name="x">x</param>
		/// <returns>y(x)=f(x)</returns>
        public virtual float f(float x)
        {
            return 0.0F;
        }

		/// <summary>
		/// f1(x) = (f(x+dx)-f(x))/(dx)
		/// </summary>
		/// <param name="x">x</param>
		/// <returns>y1(x)=f1(x)</returns>
		public float f1( float x)// NB. per ora e' in test dx_f1<>dx_f0 -> valutare!
		{
			return ( (f(x+dx_f1)-f(x))/(dx_f1) );
		}

		/// <summary>
		/// f2(x) = (f1(x+dx)-f1(x))/(dx)
		/// </summary>
		/// <param name="x">x</param>
		/// <returns>y2(x)=f2(x)</returns>
		public float f2( float x)
		{
			return ( (f1(x+dx_f1)-f1(x))/(dx_f1) );
		}



		private void commonInitModule(object sender, System.EventArgs e)
		{
			this.txtDesiredMinX.BackColor = System.Drawing.Color.White;
			this.txtDesiredMaxX.BackColor = System.Drawing.Color.White;
			this.txtDesiredMinY.BackColor = System.Drawing.Color.White;
			this.txtDesiredMaxY.BackColor = System.Drawing.Color.White;
			this.txt_dxF0.BackColor = System.Drawing.Color.White;
			this.txt_dxF1.BackColor = System.Drawing.Color.White;
			//
			Graphics gr = this.pctBoard.CreateGraphics();			
			System.Drawing.SolidBrush my_brush = new System.Drawing.SolidBrush( System.Drawing.Color.Black );// System.Drawing.Brush my_brush = new System.Drawing.Brush();  questo e' il padre abstract
			System.Drawing.Pen my_pen = new System.Drawing.Pen (my_brush, (float)1.3);
			// axes
			CartesianPlan.CartesianReference currentDevice = null;
			try
			{
				currentDevice = new CartesianPlan.CartesianReference(
					this.pctBoard,
					float.Parse(this.txtDesiredMinX.Text), float.Parse(this.txtDesiredMaxX.Text),
					float.Parse(this.txtDesiredMinY.Text), float.Parse(this.txtDesiredMaxY.Text)    );
				float[] abscissa = currentDevice.abscissaAxis();
				if(null!=abscissa)
				{
					gr.DrawLine( my_pen, abscissa[0], abscissa[1], abscissa[2], abscissa[3] );
				}// else no axes
				float[] ordinate = currentDevice.ordinateAxis();
				if(null!=ordinate)
				{
					gr.DrawLine( my_pen, ordinate[0], ordinate[1], ordinate[2], ordinate[3] );
				}// else no axes
				ax = float.Parse(this.txtDesiredMinX.Text);
				_maxX = float.Parse(this.txtDesiredMaxX.Text);
				//
				dx_f0 = float.Parse( this.txt_dxF0.Text );
				dx_f1 = float.Parse( this.txt_dxF1.Text );
				dx_f2 = float.Parse( this.txt_dxF2.Text );
			}// end try
			catch
			{// invalid ranges in text boxes
				this.txtDesiredMinX.BackColor = System.Drawing.Color.Red;
				this.txtDesiredMaxX.BackColor = System.Drawing.Color.Red;
				this.txtDesiredMinY.BackColor = System.Drawing.Color.Red;
				this.txtDesiredMaxY.BackColor = System.Drawing.Color.Red;
				this.txt_dxF0.BackColor = System.Drawing.Color.Red;
				this.txt_dxF1.BackColor = System.Drawing.Color.Red;
			}
		}// end commonInitModule

		

		private void calculationModule( object sender, System.EventArgs e, int derivationOrder)
		{// calcolo della fn(x)=yn(x)
			this.commonInitModule( sender, e);// axes
			//
			Graphics gr = this.pctBoard.CreateGraphics();			
			System.Drawing.SolidBrush my_brush = new System.Drawing.SolidBrush( System.Drawing.Color.Black );// System.Drawing.Brush my_brush = new System.Drawing.Brush();  questo e' il padre abstract
			System.Drawing.Pen my_pen = new System.Drawing.Pen (my_brush, (float)1.3);

			try
			{
				// function plot
				GraphicsDevice.current_f the_f = null;
				float the_dx = 0.0F;
				switch( derivationOrder)
				{
					case 0:
					{
						the_f = new current_f( this.f );
						the_dx = this.dx_f0;
                        LogSinkFs.Wrappers.LogWrappers.SectionContent(
							"plot of f0(x). Omothetia factors are x:" + 
							CartesianPlan.Couple.getOmothetiaX.ToString() + "  y:" +
							CartesianPlan.Couple.getOmothetiaY.ToString() + " shifts are x:"+
							CartesianPlan.Couple.getShiftX.ToString()  + "  y:" +
							CartesianPlan.Couple.getShiftY.ToString(),
							2 );
						break;
					}
					case 1:
					{
						the_f = new current_f( this.f1 );
						the_dx = this.dx_f1;
                        LogSinkFs.Wrappers.LogWrappers.SectionContent(
						    "plot of f1(x). Omothetia factors are x:" + 
							CartesianPlan.Couple.getOmothetiaX.ToString() + "  y:" +
							CartesianPlan.Couple.getOmothetiaY.ToString() + " shifts are x:"+
							CartesianPlan.Couple.getShiftX.ToString()  + "  y:" +
							CartesianPlan.Couple.getShiftY.ToString(),
							2);
						break;
					}
					case 2:
					{
						the_f = new current_f( this.f2 );
						the_dx = this.dx_f2;
                        LogSinkFs.Wrappers.LogWrappers.SectionContent(
                            "plot of f2(x). Omothetia factors are x:" + 
							CartesianPlan.Couple.getOmothetiaX.ToString() + "  y:" +
							CartesianPlan.Couple.getOmothetiaY.ToString() + " shifts are x:"+
							CartesianPlan.Couple.getShiftX.ToString()  + "  y:" +
							CartesianPlan.Couple.getShiftY.ToString(),
							2 );
						break;
					}
					default:
					{
						throw new System.Exception( "not supported");
					}
				}

				for( ; ax<_maxX; )
				{
					ay = the_f(ax );
					bx = ax + the_dx;
					by = the_f(bx );

					CartesianPlan.Couple alpha = new CartesianPlan.Couple( ax, ay);
					CartesianPlan.Couple beta  = new CartesianPlan.Couple( bx, by);
					if( alpha.isContained() && beta.isContained() )
					{
						gr.DrawLine (	my_pen, 
										alpha.getX,
										alpha.getY,
										beta.getX,
										beta.getY    );
                        LogSinkFs.Wrappers.LogWrappers.SectionContent(
						    "   SUCCESS: device coordinates:" + 
							" ax = " + alpha.getX.ToString()  +
							" ay = " + alpha.getY.ToString()  +
							" bx = " + beta.getX.ToString()   +
							" by = " + beta.getY.ToString()   +
							"\t\t"							  +
							" Cartesian coordinates:  ax="+ax.ToString()+" "     +
							"ay="+ay.ToString()+" "								 +
							"bx="+bx.ToString()+" "								 +
							"by="+by.ToString(),
							0 );

					}
					else// undrawable point; it's outside the reference
					{
                        LogSinkFs.Wrappers.LogWrappers.SectionContent(
                                        "   FAILURE: device coordinates:" + 
										" ax = " + alpha.getX.ToString()  +
										" ay = " + alpha.getY.ToString()  +
										" bx = " + beta.getX.ToString()   +
										" by = " + beta.getY.ToString()   +
										"\t\t" +
										" Cartesian coordinates:  ax="+ax.ToString()+" "     +
										"ay="+ay.ToString()+" "		+
										"bx="+bx.ToString()+" "     +
										"by="+by.ToString(),
										1 );
					}
					ax = bx;// the start point of next segment must be the last point of the previous segment,
					ay = by;// to have continuity
				}// end for plot
			}// end try
			catch
			{// invalid ranges in text boxes
				this.txtDesiredMinX.BackColor = System.Drawing.Color.Yellow;
			}
            LogSinkFs.Wrappers.LogWrappers.SectionClose();
		}// end btnCalculate_Click



		/// <summary>
		/// Rappresentazione di un vettore di punti precalcolati. Ha senso su dati campionati.
		/// Deve essere legato a un evento di chiamata. ATTUALMENTE E' SCOLLEGATO.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		/// <param name="points"></param>
		private void pointsPlot( object sender, System.EventArgs e, float[,] points)
		{// plot della f(x)=y(x) precalcolata
			this.commonInitModule( sender, e);// axes
			//
			Graphics gr = this.pctBoard.CreateGraphics();
			System.Drawing.SolidBrush my_brush = new System.Drawing.SolidBrush( System.Drawing.Color.Black );// System.Drawing.Brush my_brush = new System.Drawing.Brush();  questo e' il padre abstract
			System.Drawing.Pen my_pen = new System.Drawing.Pen (my_brush, (float)1.3);

			try
			{
				// function plot
				int nPoints = points.Length / 2;
				for( int c=0; c<nPoints-1; c++)
				{
					CartesianPlan.Couple alpha = new CartesianPlan.Couple( points[c,0]  , points[c,1]  );
					CartesianPlan.Couple beta  = new CartesianPlan.Couple( points[c+1,0], points[c+1,1]);
					if( alpha.isContained() && beta.isContained() )
					{
						gr.DrawLine (	my_pen, 
										alpha.getX,
										alpha.getY,
										beta.getX,
										beta.getY    );
                        LogSinkFs.Wrappers.LogWrappers.SectionContent(
                            "   SUCCESS: device coordinates:" + 
							" ax = " + alpha.getX.ToString() +
							" ay = " + alpha.getY.ToString() +
							" bx = " + beta.getX.ToString()  +
							" by = " + beta.getY.ToString()  +
							"\t\t" +
							" Cartesian coordinates:  ax="+ax.ToString()+" "     +
							"ay="+ay.ToString()+" "		+
							"bx="+bx.ToString()+" "     +
							"by="+by.ToString(),
							0 );
					}
					else// undrawable point
					{
                        LogSinkFs.Wrappers.LogWrappers.SectionContent(
                                        "   FAILURE: device coordinates:" + 
										" ax = " + alpha.getX.ToString()  +
										" ay = " + alpha.getY.ToString()  +
										" bx = " + beta.getX.ToString()   +
										" by = " + beta.getY.ToString()   +
										"\t\t" +
										" Cartesian coordinates:  ax="+ax.ToString()+" "     +
										"ay="+ay.ToString()+" "		+
										"bx="+bx.ToString()+" "     +
										"by="+by.ToString(),
										1 );
					}// end else_undrawable_point
				}// end for plot
			}// end try
			catch
			{// invalid ranges in text boxes
				this.txtDesiredMinX.BackColor = System.Drawing.Color.Yellow;
			}
            LogSinkFs.Wrappers.LogWrappers.SectionClose();		
		}// end btnCalculate_Click



		private void btnCalc_f0_Click(object sender, System.EventArgs e)
		{
			this.calculationModule( sender, e, 0);
			//
			// esempio di chiamata del metodo di rappresentazione dati campionati. Per utilizzarlo
			// riempire la matrice "points" con i dati del proprio campione.
//			float[,] points = new float[4,2];
//			points[0,0] = -1.0F; points[0,1] = (float)Math.Sin( (double)points[0,0]);
//			points[1,0] =  1.0F; points[1,1] = (float)Math.Sin( (double)points[1,0]);
//			points[2,0] =  2.0F; points[2,1] = (float)Math.Sin( (double)points[2,0]);
//			points[3,0] =  3.0F; points[3,1] = (float)Math.Sin( (double)points[3,0]);
//			this.pointsPlot( sender, e, points);
		}// btnCalc_f0_Click

		private void btnCalc_f1_Click(object sender, System.EventArgs e)
		{
			this.calculationModule( sender, e, 1);
		}// btnCalc_f1_Click

		private void btnCalc_f2_Click(object sender, System.EventArgs e)
		{
			this.calculationModule( sender, e, 2);
		}// btnCalc_f2_Click


		private void btnClear_Click(object sender, System.EventArgs e)
		{
			Graphics gr = this.pctBoard.CreateGraphics();
			gr.Clear( System.Drawing.Color.LightGray );
		}// btnClear_Click
		
		




	}//
}//
