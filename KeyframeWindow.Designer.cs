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
			this.components = new System.ComponentModel.Container();
			this.bone_label = new System.Windows.Forms.Label();
			this.BoneComboBox = new System.Windows.Forms.ComboBox();
			this.AnimationName = new System.Windows.Forms.Label();
			this.KeyframeScrollBar = new System.Windows.Forms.HScrollBar();
			this.KeyframeNumber = new System.Windows.Forms.Label();
			this.RotationX = new System.Windows.Forms.Label();
			this.RotationY = new System.Windows.Forms.Label();
			this.RotationZ = new System.Windows.Forms.Label();
			this.RotationW = new System.Windows.Forms.Label();
			this.RotationLabel = new System.Windows.Forms.Label();
			this.TranslationLabel = new System.Windows.Forms.Label();
			this.TranslationZ = new System.Windows.Forms.Label();
			this.TranslationY = new System.Windows.Forms.Label();
			this.TranslationX = new System.Windows.Forms.Label();
			this.rad2degreescheck = new System.Windows.Forms.CheckBox();
			this.LeftHandedCoordcheck = new System.Windows.Forms.CheckBox();
			this.rad2degreesToolTip = new System.Windows.Forms.ToolTip(this.components);
			this.LeftHandedCoordSystemToolTip = new System.Windows.Forms.ToolTip(this.components);
			this.showValuesIn2dp = new System.Windows.Forms.CheckBox();
			this.RotationInfoButton = new System.Windows.Forms.Button();
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
			this.BoneComboBox.SelectedIndexChanged += new System.EventHandler(this.BoneComboBox_SelectedIndexChanged);
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
			// KeyframeScrollBar
			// 
			this.KeyframeScrollBar.Location = new System.Drawing.Point(9, 49);
			this.KeyframeScrollBar.Name = "KeyframeScrollBar";
			this.KeyframeScrollBar.Size = new System.Drawing.Size(163, 30);
			this.KeyframeScrollBar.TabIndex = 3;
			this.KeyframeScrollBar.Scroll += new System.Windows.Forms.ScrollEventHandler(this.KeyframeScrollBar_Scroll);
			// 
			// KeyframeNumber
			// 
			this.KeyframeNumber.AutoSize = true;
			this.KeyframeNumber.Location = new System.Drawing.Point(12, 79);
			this.KeyframeNumber.Name = "KeyframeNumber";
			this.KeyframeNumber.Size = new System.Drawing.Size(13, 13);
			this.KeyframeNumber.TabIndex = 4;
			this.KeyframeNumber.Text = "0";
			// 
			// RotationX
			// 
			this.RotationX.AutoSize = true;
			this.RotationX.Location = new System.Drawing.Point(9, 158);
			this.RotationX.Name = "RotationX";
			this.RotationX.Size = new System.Drawing.Size(13, 13);
			this.RotationX.TabIndex = 5;
			this.RotationX.Text = "0";
			// 
			// RotationY
			// 
			this.RotationY.AutoSize = true;
			this.RotationY.Location = new System.Drawing.Point(9, 171);
			this.RotationY.Name = "RotationY";
			this.RotationY.Size = new System.Drawing.Size(13, 13);
			this.RotationY.TabIndex = 6;
			this.RotationY.Text = "0";
			// 
			// RotationZ
			// 
			this.RotationZ.AutoSize = true;
			this.RotationZ.Location = new System.Drawing.Point(9, 184);
			this.RotationZ.Name = "RotationZ";
			this.RotationZ.Size = new System.Drawing.Size(13, 13);
			this.RotationZ.TabIndex = 7;
			this.RotationZ.Text = "0";
			// 
			// RotationW
			// 
			this.RotationW.AutoSize = true;
			this.RotationW.Location = new System.Drawing.Point(9, 197);
			this.RotationW.Name = "RotationW";
			this.RotationW.Size = new System.Drawing.Size(13, 13);
			this.RotationW.TabIndex = 8;
			this.RotationW.Text = "0";
			// 
			// RotationLabel
			// 
			this.RotationLabel.AutoSize = true;
			this.RotationLabel.Location = new System.Drawing.Point(9, 145);
			this.RotationLabel.Name = "RotationLabel";
			this.RotationLabel.Size = new System.Drawing.Size(93, 13);
			this.RotationLabel.TabIndex = 9;
			this.RotationLabel.Text = "Rotations (x,y,z,w)";
			// 
			// TranslationLabel
			// 
			this.TranslationLabel.AutoSize = true;
			this.TranslationLabel.Location = new System.Drawing.Point(144, 145);
			this.TranslationLabel.Name = "TranslationLabel";
			this.TranslationLabel.Size = new System.Drawing.Size(94, 13);
			this.TranslationLabel.TabIndex = 14;
			this.TranslationLabel.Text = "Translations (x,y,z)";
			// 
			// TranslationZ
			// 
			this.TranslationZ.AutoSize = true;
			this.TranslationZ.Location = new System.Drawing.Point(144, 184);
			this.TranslationZ.Name = "TranslationZ";
			this.TranslationZ.Size = new System.Drawing.Size(13, 13);
			this.TranslationZ.TabIndex = 12;
			this.TranslationZ.Text = "0";
			// 
			// TranslationY
			// 
			this.TranslationY.AutoSize = true;
			this.TranslationY.Location = new System.Drawing.Point(144, 171);
			this.TranslationY.Name = "TranslationY";
			this.TranslationY.Size = new System.Drawing.Size(13, 13);
			this.TranslationY.TabIndex = 11;
			this.TranslationY.Text = "0";
			// 
			// TranslationX
			// 
			this.TranslationX.AutoSize = true;
			this.TranslationX.Location = new System.Drawing.Point(144, 158);
			this.TranslationX.Name = "TranslationX";
			this.TranslationX.Size = new System.Drawing.Size(13, 13);
			this.TranslationX.TabIndex = 10;
			this.TranslationX.Text = "0";
			// 
			// rad2degreescheck
			// 
			this.rad2degreescheck.AutoSize = true;
			this.rad2degreescheck.Checked = true;
			this.rad2degreescheck.CheckState = System.Windows.Forms.CheckState.Checked;
			this.rad2degreescheck.Location = new System.Drawing.Point(7, 226);
			this.rad2degreescheck.Name = "rad2degreescheck";
			this.rad2degreescheck.Size = new System.Drawing.Size(108, 17);
			this.rad2degreescheck.TabIndex = 15;
			this.rad2degreescheck.Text = "Show In Degrees";
			this.rad2degreesToolTip.SetToolTip(this.rad2degreescheck, "Show Values in degrees as ZXY Euler instead of radians as quaternions.");
			this.rad2degreescheck.UseVisualStyleBackColor = true;
			this.rad2degreescheck.CheckedChanged += new System.EventHandler(this.rad2degreescheck_CheckedChanged);
			// 
			// LeftHandedCoordcheck
			// 
			this.LeftHandedCoordcheck.AutoSize = true;
			this.LeftHandedCoordcheck.Location = new System.Drawing.Point(7, 249);
			this.LeftHandedCoordcheck.Name = "LeftHandedCoordcheck";
			this.LeftHandedCoordcheck.Size = new System.Drawing.Size(176, 17);
			this.LeftHandedCoordcheck.TabIndex = 16;
			this.LeftHandedCoordcheck.Text = "Left Handed Coordinate System";
			this.rad2degreesToolTip.SetToolTip(this.LeftHandedCoordcheck, "Show ZXY Euler rotation values as if they were in a Left Handed Coordinate System" +
        ". Blender is right handed.");
			this.LeftHandedCoordcheck.UseVisualStyleBackColor = true;
			this.LeftHandedCoordcheck.CheckedChanged += new System.EventHandler(this.LeftHandedCoordcheck_CheckedChanged);
			// 
			// showValuesIn2dp
			// 
			this.showValuesIn2dp.AutoSize = true;
			this.showValuesIn2dp.Checked = true;
			this.showValuesIn2dp.CheckState = System.Windows.Forms.CheckState.Checked;
			this.showValuesIn2dp.Location = new System.Drawing.Point(15, 125);
			this.showValuesIn2dp.Name = "showValuesIn2dp";
			this.showValuesIn2dp.Size = new System.Drawing.Size(184, 17);
			this.showValuesIn2dp.TabIndex = 17;
			this.showValuesIn2dp.Text = "Show Values in 2 Decimal Places";
			this.showValuesIn2dp.UseVisualStyleBackColor = true;
			this.showValuesIn2dp.CheckedChanged += new System.EventHandler(this.showValuesIn2dp_CheckedChanged);
			// 
			// RotationInfoButton
			// 
			this.RotationInfoButton.Location = new System.Drawing.Point(7, 272);
			this.RotationInfoButton.Name = "RotationInfoButton";
			this.RotationInfoButton.Size = new System.Drawing.Size(95, 23);
			this.RotationInfoButton.TabIndex = 18;
			this.RotationInfoButton.Text = "Rotation Info";
			this.RotationInfoButton.UseVisualStyleBackColor = true;
			this.RotationInfoButton.Click += new System.EventHandler(this.RotationInfoButton_Click);
			// 
			// KeyframeWindow
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(643, 369);
			this.Controls.Add(this.RotationInfoButton);
			this.Controls.Add(this.showValuesIn2dp);
			this.Controls.Add(this.LeftHandedCoordcheck);
			this.Controls.Add(this.rad2degreescheck);
			this.Controls.Add(this.TranslationLabel);
			this.Controls.Add(this.TranslationZ);
			this.Controls.Add(this.TranslationY);
			this.Controls.Add(this.TranslationX);
			this.Controls.Add(this.RotationLabel);
			this.Controls.Add(this.RotationW);
			this.Controls.Add(this.RotationZ);
			this.Controls.Add(this.RotationY);
			this.Controls.Add(this.RotationX);
			this.Controls.Add(this.KeyframeNumber);
			this.Controls.Add(this.KeyframeScrollBar);
			this.Controls.Add(this.AnimationName);
			this.Controls.Add(this.BoneComboBox);
			this.Controls.Add(this.bone_label);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.Name = "KeyframeWindow";
			this.ShowIcon = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Keyframes";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label bone_label;
		private System.Windows.Forms.ComboBox BoneComboBox;
		private System.Windows.Forms.Label AnimationName;
		private System.Windows.Forms.HScrollBar KeyframeScrollBar;
		private System.Windows.Forms.Label KeyframeNumber;
		private System.Windows.Forms.Label RotationX;
		private System.Windows.Forms.Label RotationY;
		private System.Windows.Forms.Label RotationZ;
		private System.Windows.Forms.Label RotationW;
		private System.Windows.Forms.Label RotationLabel;
		private System.Windows.Forms.Label TranslationLabel;
		private System.Windows.Forms.Label TranslationZ;
		private System.Windows.Forms.Label TranslationY;
		private System.Windows.Forms.Label TranslationX;
		private System.Windows.Forms.CheckBox rad2degreescheck;
		private System.Windows.Forms.CheckBox LeftHandedCoordcheck;
		private System.Windows.Forms.ToolTip rad2degreesToolTip;
		private System.Windows.Forms.ToolTip LeftHandedCoordSystemToolTip;
		private System.Windows.Forms.CheckBox showValuesIn2dp;
		private System.Windows.Forms.Button RotationInfoButton;
	}
}