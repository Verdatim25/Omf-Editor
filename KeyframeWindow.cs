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
	public partial class KeyframeWindow : Form
	{
		List<BoneParts> saved_parts;
		public KeyframeWindow(List<BoneParts> parts, short[][][] rotation_result, short[][][] translation_result, AnimationParams Anim_Params)
		{
			InitializeComponent();
			saved_parts = parts;
			for (int i = 0; i < parts.Count; i++)
			{
				for (int ii = 0; ii < parts[i].bones.Count; ii++) {
					Bone_params temp = new Bone_params();
					temp.name = parts[i].bones[ii].Name;
					temp.id = ii;
					temp.part = i;
					BoneComboBox.Items.Add(temp);
				}
			}

			AnimationName.Text = Anim_Params.Name;


		}
	}
	public class Bone_params
	{
		public string name;
		public int id;
		public int part;

		public override string ToString()
		{
			return name;
		}
	}
}
