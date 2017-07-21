using System;
using System.IO;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Drawing.Printing;

namespace BarCode
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class wfrm_Main : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button m_pSet;
		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.Button m_pPrintPreview;
		private BarCode.wctrl_BarCode wctrl_BarCode1;
		private System.Windows.Forms.Button m_pSaveToFile;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public wfrm_Main()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			// 下载于www.mycodes.net
			//
			// TODO: Add any constructor code after InitializeComponent call
			//

			wctrl_BarCode1.BarCode = textBox1.Text;
		}

		#region method Dispose

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

		#endregion

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(wfrm_Main));
            this.m_pSet = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.m_pPrintPreview = new System.Windows.Forms.Button();
            this.wctrl_BarCode1 = new BarCode.wctrl_BarCode();
            this.m_pSaveToFile = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // m_pSet
            // 
            this.m_pSet.Location = new System.Drawing.Point(277, 27);
            this.m_pSet.Name = "m_pSet";
            this.m_pSet.Size = new System.Drawing.Size(128, 33);
            this.m_pSet.TabIndex = 0;
            this.m_pSet.Text = "Set";
            this.m_pSet.Click += new System.EventHandler(this.m_pSet_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(33, 27);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(218, 25);
            this.textBox1.TabIndex = 2;
            this.textBox1.Text = "7501031311309";
            // 
            // m_pPrintPreview
            // 
            this.m_pPrintPreview.Location = new System.Drawing.Point(277, 82);
            this.m_pPrintPreview.Name = "m_pPrintPreview";
            this.m_pPrintPreview.Size = new System.Drawing.Size(128, 33);
            this.m_pPrintPreview.TabIndex = 3;
            this.m_pPrintPreview.Text = "PrintPreview";
            this.m_pPrintPreview.Click += new System.EventHandler(this.m_pPrintPreview_Click);
            // 
            // wctrl_BarCode1
            // 
            this.wctrl_BarCode1.BarCode = "";
            this.wctrl_BarCode1.Location = new System.Drawing.Point(33, 82);
            this.wctrl_BarCode1.Name = "wctrl_BarCode1";
            this.wctrl_BarCode1.Size = new System.Drawing.Size(218, 66);
            this.wctrl_BarCode1.TabIndex = 4;
            // 
            // m_pSaveToFile
            // 
            this.m_pSaveToFile.Location = new System.Drawing.Point(277, 126);
            this.m_pSaveToFile.Name = "m_pSaveToFile";
            this.m_pSaveToFile.Size = new System.Drawing.Size(128, 33);
            this.m_pSaveToFile.TabIndex = 5;
            this.m_pSaveToFile.Text = "Save to File";
            this.m_pSaveToFile.Click += new System.EventHandler(this.m_pSaveToFile_Click);
            // 
            // wfrm_Main
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(8, 18);
            this.ClientSize = new System.Drawing.Size(434, 191);
            this.Controls.Add(this.m_pSaveToFile);
            this.Controls.Add(this.wctrl_BarCode1);
            this.Controls.Add(this.m_pPrintPreview);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.m_pSet);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "wfrm_Main";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "条形码生成器";
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		
		private void m_pSet_Click(object sender, System.EventArgs e)
		{
			wctrl_BarCode1.BarCode = textBox1.Text;
		}

		private void m_pSaveToFile_Click(object sender, System.EventArgs e)
		{	
			int width  = 217; // wctrl_BarCode1.Width
			int height = 38;  // wctrl_BarCode1.Height
	
			using(Bitmap bmp = new Bitmap(width,height)){
				using(Graphics g = Graphics.FromImage(bmp)){
					wctrl_BarCode.PaintBarCode(textBox1.Text,g,new Rectangle(0,0,width,height));
				}

				SaveFileDialog dlg = new SaveFileDialog();
				dlg.Filter = "Jpeg (*.jpg)|*.jpg";
				if(dlg.ShowDialog() == DialogResult.OK){
					using(FileStream fs = File.Create(dlg.FileName)){
						bmp.Save(fs,System.Drawing.Imaging.ImageFormat.Jpeg);
					}
				}
			}
			
		}
		
		private void m_pPrintPreview_Click(object sender, System.EventArgs e)
		{
			PrintDocument tmpprndoc = new PrintDocument();
			tmpprndoc.PrintPage += new PrintPageEventHandler(toPrinter);

			PrintPreviewDialog tmpprdiag = new PrintPreviewDialog();
			tmpprdiag.Document = tmpprndoc;
			tmpprdiag.Show();
		}

		private void toPrinter(Object sender,PrintPageEventArgs e)
		{
			wctrl_BarCode.PaintBarCode(textBox1.Text,e.Graphics,new Rectangle(10,0,100,56));
		}		
	}
}
