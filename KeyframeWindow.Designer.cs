namespace OMF_Editor
{
	partial class KeyframeWindow
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
			this.bone_label = new System.Windows.Forms.Label();
			this.BoneComboBox = new System.Windows.Forms.ComboBox();
			this.AnimationName = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// bone_label
			// 
			this.bone_label.AutoSize = true;
			this.bone_label.Location = new System.Drawing.Point(55, 9);
			this.bone_label.Name = "bone_label";
			this.bone_label.Size = new System.Drawing.Size(32, 13);
			this.bone_label.TabIndex = 0;
			this.bone_label.Text = "Bone";
			// 
			// BoneComboBox
			// 
			this.BoneComboBox.FormattingEnabled = true;
			this.BoneComboBox.Location = new System.Drawing.Point(12, 25);
			this.BoneComboBox.Name = "BoneComboBox";
			this.BoneComboBox.Size = new System.Drawing.Size(121, 21);
			this.BoneComboBox.TabIndex = 1;
			// 
			// AnimationName
			// 
			this.AnimationName.AutoSize = true;
			this.AnimationName.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F);
			this.AnimationName.Location = new System.Drawing.Point(300, 9);
			this.AnimationName.Name = "AnimationName";
			this.AnimationName.Size = new System.Drawing.Size(99, 25);
			this.AnimationName.TabIndex = 2;
			this.AnimationName.Text = "Animation";
			// 
			// KeyframeWindow
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(643, 369);
			this.Controls.Add(this.AnimationName);
			this.Controls.Add(this.BoneComboBox);
			this.Controls.Add(this.bone_label);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.Name = "KeyframeWindow";
			this.ShowIcon = false;
			this.Text = "Keyframes";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label bone_label;
		private System.Windows.Forms.ComboBox BoneComboBox;
		private System.Windows.Forms.Label AnimationName;
	}
}