namespace OMF_Editor
{
	partial class RenameMotionMarks
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
			this.MotionMarkTextBoxFirst = new System.Windows.Forms.TextBox();
			this.MotionMarkTextBoxSecond = new System.Windows.Forms.TextBox();
			this.RenameMotionMarksButton = new System.Windows.Forms.Button();
			this.RenameMotionMarksLabel = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// MotionMarkTextBoxFirst
			// 
			this.MotionMarkTextBoxFirst.Location = new System.Drawing.Point(12, 12);
			this.MotionMarkTextBoxFirst.Name = "MotionMarkTextBoxFirst";
			this.MotionMarkTextBoxFirst.Size = new System.Drawing.Size(140, 20);
			this.MotionMarkTextBoxFirst.TabIndex = 0;
			// 
			// MotionMarkTextBoxSecond
			// 
			this.MotionMarkTextBoxSecond.Location = new System.Drawing.Point(221, 12);
			this.MotionMarkTextBoxSecond.Name = "MotionMarkTextBoxSecond";
			this.MotionMarkTextBoxSecond.Size = new System.Drawing.Size(142, 20);
			this.MotionMarkTextBoxSecond.TabIndex = 1;
			// 
			// RenameMotionMarksButton
			// 
			this.RenameMotionMarksButton.Location = new System.Drawing.Point(121, 48);
			this.RenameMotionMarksButton.Name = "RenameMotionMarksButton";
			this.RenameMotionMarksButton.Size = new System.Drawing.Size(132, 23);
			this.RenameMotionMarksButton.TabIndex = 2;
			this.RenameMotionMarksButton.Text = "Rename Marks";
			this.RenameMotionMarksButton.UseVisualStyleBackColor = true;
			this.RenameMotionMarksButton.Click += new System.EventHandler(this.RenameMotionMarksButton_Click);
			// 
			// RenameMotionMarksLabel
			// 
			this.RenameMotionMarksLabel.AutoSize = true;
			this.RenameMotionMarksLabel.Location = new System.Drawing.Point(171, 15);
			this.RenameMotionMarksLabel.Name = "RenameMotionMarksLabel";
			this.RenameMotionMarksLabel.Size = new System.Drawing.Size(31, 13);
			this.RenameMotionMarksLabel.TabIndex = 3;
			this.RenameMotionMarksLabel.Text = "------>";
			// 
			// RenameMotionMarks
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(375, 106);
			this.Controls.Add(this.RenameMotionMarksLabel);
			this.Controls.Add(this.RenameMotionMarksButton);
			this.Controls.Add(this.MotionMarkTextBoxSecond);
			this.Controls.Add(this.MotionMarkTextBoxFirst);
			this.MaximizeBox = false;
			this.Name = "RenameMotionMarks";
			this.ShowIcon = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Rename Motion Marks";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox MotionMarkTextBoxFirst;
		private System.Windows.Forms.TextBox MotionMarkTextBoxSecond;
		private System.Windows.Forms.Button RenameMotionMarksButton;
		private System.Windows.Forms.Label RenameMotionMarksLabel;
	}
}