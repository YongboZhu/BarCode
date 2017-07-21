using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace BarCode
{	
	/// <summary>
	/// Barcode control.
	/// </summary>
	public class wctrl_BarCode : System.Windows.Forms.UserControl
	{
		private string m_BarCode = "";

		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		/// <summary>
		/// Default constructor.
		/// </summary>
		public wctrl_BarCode()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitializeComponent call			

		}

		#region method Dispose

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#endregion

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			// 
			// BarCode
			// 
			this.Name = "BarCode";
			this.Size = new System.Drawing.Size(216, 64);

		}
		#endregion


		#region override method OnPaint

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
		
			PaintBarCode(m_BarCode,e.Graphics,this.ClientRectangle);
		}

		#endregion


		#region static method PaintBarCode

		/// <summary>
		/// Detects barcode type and paints barcode to specified graphics into specified draw rectangle.
		/// </summary>
		/// <param name="barCode">BarCode value.</param>
		/// <param name="g">Graphics where to draw.</param>
		/// <param name="drawBounds">Draw bounds.</param>
		public static void PaintBarCode(string barCode,Graphics g,Rectangle drawBounds)
		{
			if(barCode.Length == 8){
				Paint_EAN8(barCode,g,drawBounds);
			}
			else if(barCode.Length == 13){
				Paint_EAN13(barCode,g,drawBounds);
			}
		}

		#endregion


		#region static method Paint_EAN8

		/// <summary>
		/// Paint EAN8 barcode to specified graphics into specified draw rectangle.
		/// </summary>
		/// <param name="barCode">BarCode value.</param>
		/// <param name="g">Graphics where to draw.</param>
		/// <param name="drawBounds">Draw bounds.</param>
		public static void Paint_EAN8(string barCode,Graphics g,Rectangle drawBounds)
		{
			char[] symbols = barCode.ToCharArray();

			//--- Validate barCode --------------//
			if(barCode.Length != 8){
				return;
			}
			foreach(char c in symbols){
				if(!Char.IsDigit(c)){
					return;
				}
			}
			if(!Check_EAN_CheckSum(barCode)){
				return;
			}
			//----------------------------------//

			int lineWidth = 1;
			int x = drawBounds.X;
			Font font = new Font("Microsoft Sans Serif",8);
			
			// Fill backround with white color
			g.Clear(Color.White);
			
			// Paint left 'guard bars', always same '101'
			x += PaintStripes("101",g,lineWidth,x,drawBounds.Y,drawBounds.Height);
			
			// 4 symbol code
			string lines = "";
			for(int i=0;i<4;i++){
				lines += EAN_NumberToStripes("odd",symbols[i]);
			}

			// Paint human readable left-side 4 symbol code
			g.DrawString(barCode.Substring(0,4),font,new SolidBrush(Color.Black),x,drawBounds.Y + drawBounds.Height - 12);

			// Paint left-side 4 symbol stripes
			x += PaintStripes(lines,g,lineWidth,x,drawBounds.Y,drawBounds.Height - 12);

			// Paint center 'guard bars', always same '01010'
			x += PaintStripes("01010",g,lineWidth,x,drawBounds.Y,drawBounds.Height);

			// 5 symbol product code + 1 symbol parity
			lines = "";
			for(int i=4;i<8;i++){
				lines += EAN_NumberToStripes("character",symbols[i]);
			}

			// Paint human readable right-side 4 symbol code
			g.DrawString(barCode.Substring(4,4),font,new SolidBrush(Color.Black),x,drawBounds.Y + drawBounds.Height - 12);

			// Paint right-side 4 symbol stripes
			x += PaintStripes(lines,g,lineWidth,x,drawBounds.Y,drawBounds.Height - 12);
			
			// Paint right 'guard bars', always same '101'
			x += PaintStripes("101",g,lineWidth,x,drawBounds.Y,drawBounds.Height);
		}

		#endregion
		
		#region static method Paint_EAN13

		/// <summary>
		/// Paint EAN13 barcode to specified graphics into specified draw rectangle.
		/// </summary>
		/// <param name="barCode">BarCode value.</param>
		/// <param name="g">Graphics where to draw.</param>
		/// <param name="drawBounds">Draw bounds.</param>
		public static void Paint_EAN13(string barCode,Graphics g,Rectangle drawBounds)
		{            
			char[] symbols = barCode.ToCharArray();

			//--- Validate barCode --------------//
			if(barCode.Length != 13){				
				return; 
			}
			foreach(char c in symbols){
				if(!Char.IsDigit(c)){
					return;
				}
			}
			if(!Check_EAN_CheckSum(barCode)){
				return;
			}
			//-----------------------------------//

			int lineWidth = 1;
			int x = drawBounds.X;
			Font font = new Font("Microsoft Sans Serif",8);
			
			// Fill backround with white color
			g.Clear(Color.White);
			
			// Paint human readable 1 system symbol code
			g.DrawString(symbols[0].ToString(),font,new SolidBrush(Color.Black),x,drawBounds.Y + drawBounds.Height - 16);
			x += 10;

			// Paint left 'guard bars', always same '101'
			x += PaintStripes("101",g,lineWidth,x,drawBounds.Y,drawBounds.Height);

			// First number of barcode specifies how to encode each character in the left-hand 
			// side of the barcode should be encoded.
			bool[] leftSideParity = new bool[6];
			switch(symbols[0])
			{
				case '0':
					leftSideParity[0] = true;  // Odd
					leftSideParity[1] = true;  // Odd
					leftSideParity[2] = true;  // Odd
					leftSideParity[3] = true;  // Odd
					leftSideParity[4] = true;  // Odd
					leftSideParity[5] = true;  // Odd
					break;
				case '1':
					leftSideParity[0] = true;  // Odd
					leftSideParity[1] = true;  // Odd
					leftSideParity[2] = false; // Even
					leftSideParity[3] = true;  // Odd
					leftSideParity[4] = false; // Even
					leftSideParity[5] = false; // Even
					break;
				case '2':
					leftSideParity[0] = true;  // Odd
					leftSideParity[1] = true;  // Odd
					leftSideParity[2] = false; // Even
					leftSideParity[3] = false; // Even
					leftSideParity[4] = true;  // Odd
					leftSideParity[5] = false; // Even
					break;
				case '3':
					leftSideParity[0] = true;  // Odd
					leftSideParity[1] = true;  // Odd
					leftSideParity[2] = false; // Even
					leftSideParity[3] = false; // Even
					leftSideParity[4] = false; // Even
					leftSideParity[5] = true;  // Odd
					break;
				case '4':
					leftSideParity[0] = true;  // Odd
					leftSideParity[1] = false; // Even
					leftSideParity[2] = true;  // Odd
					leftSideParity[3] = true;  // Odd
					leftSideParity[4] = false; // Even
					leftSideParity[5] = false; // Even
					break;
				case '5':
					leftSideParity[0] = true;  // Odd
					leftSideParity[1] = false; // Even
					leftSideParity[2] = false; // Even
					leftSideParity[3] = true;  // Odd
					leftSideParity[4] = true;  // Odd
					leftSideParity[5] = false; // Even
					break;
				case '6':
					leftSideParity[0] = true;  // Odd
					leftSideParity[1] = false; // Even
					leftSideParity[2] = false; // Even
					leftSideParity[3] = false; // Even
					leftSideParity[4] = true;  // Odd
					leftSideParity[5] = true;  // Odd
					break;
				case '7':
					leftSideParity[0] = true;  // Odd
					leftSideParity[1] = false; // Even
					leftSideParity[2] = true;  // Odd
					leftSideParity[3] = false; // Even
					leftSideParity[4] = true;  // Odd
					leftSideParity[5] = false; // Even
					break;
				case '8':
					leftSideParity[0] = true;  // Odd
					leftSideParity[1] = false; // Even
					leftSideParity[2] = true;  // Odd
					leftSideParity[3] = false; // Even
					leftSideParity[4] = false; // Even
					leftSideParity[5] = true;  // Odd
					break;
				case '9':
					leftSideParity[0] = true;  // Odd
					leftSideParity[1] = false; // Even
					leftSideParity[2] = false; // Even
					leftSideParity[3] = true;  // Odd
					leftSideParity[4] = false; // Even
					leftSideParity[5] = true;  // Odd
					break;
			}

			// NOTE: first number isn't painted as stripes, parity will determine it !!!
			
			// second number system digit + 5 symbol manufacter code
			string lines = "";
			for(int i=0;i<6;i++){
				bool oddParity = leftSideParity[i];
				if(oddParity){
					lines += EAN_NumberToStripes("odd",symbols[i + 1]);
				}
				// Even parity
				else{
					lines += EAN_NumberToStripes("even",symbols[i + 1]);
				}
			}

			// Paint human readable left-side 6 symbol code
			g.DrawString(barCode.Substring(1,6),font,new SolidBrush(Color.Black),x,drawBounds.Y + drawBounds.Height - 12);

			// Paint left-side 6 symbol stripes
			x += PaintStripes(lines,g,lineWidth,x,drawBounds.Y,drawBounds.Height - 12);
			
			// Paint center 'guard bars', always same '01010'
			x += PaintStripes("01010",g,lineWidth,x,drawBounds.Y,drawBounds.Height);

			// 5 symbol product code + 1 symbol parity
			lines = "";
			for(int i=7;i<13;i++){
				lines += EAN_NumberToStripes("character",symbols[i]);
			}

			// Paint human readable right-side 6 symbol code
			g.DrawString(barCode.Substring(7,6),font,new SolidBrush(Color.Black),x,drawBounds.Y + drawBounds.Height - 12);

			// Paint right-side 6 symbol stripes
			x += PaintStripes(lines,g,lineWidth,x,drawBounds.Y,drawBounds.Height - 12);
			
			// Paint right 'guard bars', always same '101'
			x += PaintStripes("101",g,lineWidth,x,drawBounds.Y,drawBounds.Height);
		}

		#endregion


		#region static method EAN_NumberToStripes

		private static string EAN_NumberToStripes(string parity,char number)
		{
			string lines = "";

			if(parity.ToLower() == "odd"){
				switch(number)
				{
					case '0':
						lines += "0001101";
						break;
					case '1':
						lines += "0011001";
						break;
					case '2':
						lines += "0010011";
						break;
					case '3':
						lines += "0111101";
						break;
					case '4':
						lines += "0100011";
						break;
					case '5':
						lines += "0110001";
						break;
					case '6':
						lines += "0101111";
						break;
					case '7':
						lines += "0111011";
						break;
					case '8':
						lines += "0110111";
						break;
					case '9':
						lines += "0001011";
						break;
				}
			}
			// Even parity
			else if(parity.ToLower() == "even"){
				switch(number)
				{
					case '0':
						lines += "0100111";
						break;
					case '1':
						lines += "0110011";
						break;
					case '2':
						lines += "0011011";
						break;
					case '3':
						lines += "0100001";
						break;
					case '4':
						lines += "0011101";
						break;
					case '5':
						lines += "0111001";
						break;
					case '6':
						lines += "0000101";
						break;
					case '7':
						lines += "0010001";
						break;
					case '8':
						lines += "0001001";
						break;
					case '9':
						lines += "0010111";
						break;
				}
			}
			else if(parity.ToLower() == "character"){
				switch(number)
				{
					case '0':
						lines += "1110010";
						break;
					case '1':
						lines += "1100110";
						break;
					case '2':
						lines += "1101100";
						break;
					case '3':
						lines += "1000010";
						break;
					case '4':
						lines += "1011100";
						break;
					case '5':
						lines += "1001110";
						break;
					case '6':
						lines += "1010000";
						break;
					case '7':
						lines += "1000100";
						break;
					case '8':
						lines += "1001000";
						break;
					case '9':
						lines += "1110100";
						break;
				}
			}

			return lines;
		}

		#endregion

		#region static method Check_EAN_CheckSum

		/// <summary>
		/// Checks EAN checksum.
		/// </summary>
		/// <param name="barCode">Barcode value.</param>
		/// <returns>Returns true if barcode is ok.</returns>
		private static bool Check_EAN_CheckSum(string barCode)
		{
			char[] symbols = barCode.ToCharArray();

			//--- Check barcode checksum --------------------------------------------------------//
			int checkSum = Convert.ToInt32(symbols[symbols.Length - 1].ToString());
			int calcSum  = 0;
			bool one_three = false;
			for(int i=(barCode.Length-2);i>-1;i--){
				if(one_three){
					calcSum += (Convert.ToInt32(symbols[i].ToString()) * 1);
					one_three = false;
				}
				else{
					calcSum += (Convert.ToInt32(symbols[i].ToString()) * 3);
					one_three = true;
				}				
			}
					
			char[] calcSumChar = calcSum.ToString().ToCharArray();
			int neededForDiv10 = 10 - Convert.ToInt32(calcSumChar[calcSumChar.Length - 1].ToString());

			// If the sum calculated is evenly disivisible by 10, the check digit is "0" (not 10). 
			if(neededForDiv10 != 10 && checkSum != neededForDiv10){
				return false;
			}
			//------------------------------------------------------------------------------------//

			return true;
		}

		#endregion


		#region static method PaintStripes

		/// <summary>
		/// Paints stripes from string. String must contain only 0 or 1 values.
		/// 1 is black stripe, 0 is white stripe.
		/// </summary>
		/// <param name="stripes">Srtipes string.</param>
		/// <param name="g">Graphics to paint.</param>
		/// <param name="stripeWidth">One stripe width.</param>
		/// <param name="x">X start position.</param>
		/// <param name="y">Y position.</param>
		/// <param name="height">Stripe height.</param>
		/// <returns></returns>
		private static int PaintStripes(string stripes,Graphics g,int stripeWidth,int x,int y,int height)
		{
			int paintedWidth = 0;
			char[] stripeArray = stripes.ToCharArray();
			for(int i=0;i<stripeArray.Length;i++){
				if(stripeArray[i] == '0'){
					g.DrawLine(new Pen(Color.White,stripeWidth),x,y,x,y + height);
				}
				else{
					g.DrawLine(new Pen(Color.Black,stripeWidth),x,y,x,y + height);
				}
				x += stripeWidth;
				paintedWidth += stripeWidth;
			}

			return paintedWidth;
		}

		#endregion

		
		#region Properties Implementation

		/// <summary>
		/// Gets or sets barcode value.
		/// </summary>
		public string BarCode
		{
			get{ return m_BarCode; }

			set{ 
				m_BarCode = value; 

				this.Invalidate();
			}
		}

		#endregion
	}
}
