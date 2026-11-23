using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OMF_Editor
{
	public partial class RenameMotionMarks : Form
	{
		AnimationsContainer Main_OMF;
		public RenameMotionMarks(AnimationsContainer main_OMF)
		{
			InitializeComponent();
			Main_OMF = main_OMF;
		}

		private void RenameMotionMarksButton_Click(object sender, EventArgs e)
		{
			DialogResult result = DialogResult.Yes;
			string start_name = MotionMarkTextBoxFirst.Text;
			string end_name = MotionMarkTextBoxSecond.Text;
			string message;
			if (end_name == ""){
				MessageBox.Show("End name cannot be missing!", "Error");
				return;
			}
			if (start_name == ""){
				message = String.Format("Are you sure you want to rename all motion marks to {0}?", end_name);
			}
			else
				message = String.Format("Are you sure you want to rename all motion marks with name {0} to {1}?", start_name, end_name);
			result = MessageBox.Show(message, "Warning!", MessageBoxButtons.YesNo);
			if (result == DialogResult.Yes)
			{
				bool renamed = false;
				foreach (AnimationParams Params in Main_OMF.AnimsParams)
				{
					if (Params.m_marks != null){
						foreach (MotionMark motion_mark in Params.m_marks)
						{
							string name = motion_mark.Name;
							// long-winded solution but this should work (hopefully)
							char[] chracters_to_remove = new char[name.Length];
							int _i = 0;
							foreach (char v in name)
							{
								if ((int)v < (33))
								{
									chracters_to_remove[_i] = v;
									_i++;
								}
							}
							name = name.Trim(chracters_to_remove);
							if ((name == start_name) || (name == ""))
							{
								motion_mark.Name = end_name;
								renamed = true;
							}
						}
					}
				}
				if (renamed)
					Close();
			}
		}

		private void RenameMotionMarks_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyData == Keys.Escape)
			{
				Close();
			}
		}

		private void MotionMarkTextBoxFirst_KeyDown(object sender, KeyEventArgs e)
		{
			RenameMotionMarks_KeyDown(sender, e);
		}

		private void MotionMarkTextBoxSecond_KeyDown(object sender, KeyEventArgs e)
		{
			RenameMotionMarks_KeyDown(sender, e);
		}

		private void RenameMotionMarksButton_KeyDown(object sender, KeyEventArgs e)
		{
			RenameMotionMarks_KeyDown(sender, e);
		}
	}
}
