using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OMF_Editor
{
	public partial class KeyframeWindow : Form
	{
		#region Quat2EulerExport
		// export from https://stackoverflow.com/questions/1031005/is-there-an-algorithm-for-converting-quaternion-rotations-to-euler-angle-rotatio by user Jonas Byström, tysm!
		// credits to him and all those much more wonderful and smarter than me that understand quarternions better than I do
		public struct Quaternion
		{
			public double w, x, y, z;
			public Quaternion(double _x, double _y, double _z, double _w)
			{
				x = _x;
				y = _y;
				z = _z;
				w = _w;
			}
		};

		public double[] GetEulerAngles(Quaternion q, double yaw, double pitch, double roll)
		{
			double w2 = q.w * q.w;
			double x2 = q.x * q.x;
			double y2 = q.y * q.y;
			double z2 = q.z * q.z;
			double unitLength = w2 + x2 + y2 + z2;    // Normalised == 1, otherwise correction divisor.
			double abcd = q.w * q.x + q.y * q.z;
			const double eps = 1e-7;    // TODO: pick from your math lib instead of hardcoding.
			const double pi = Math.PI;   // TODO: pick from your math lib instead of hardcoding.

			if (abcd > (0.5 - eps) * unitLength)
			{
				yaw = 2 * Math.Atan2(q.y, q.w);
				pitch = pi;
				roll = 0;
			}
			else if (abcd < (-0.5 + eps) * unitLength)
			{
				yaw = -2 * Math.Atan2(q.y, q.w);
				pitch = -pi;
				roll = 0;
			}
			else
			{
				double adbc = q.w * q.z - q.x * q.y;
				double acbd = q.w * q.y - q.x * q.z;
				yaw = Math.Atan2(2 * adbc, 1 - 2 * (z2 + x2));
				pitch = Math.Asin(2 * abcd / unitLength);
				roll = Math.Atan2(2 * acbd, 1 - 2 * (y2 + x2));
			}
			double[] ret = { yaw, pitch, roll };
			return ret;
		}
		#endregion
		int selected_index;
		int keyframe_number;
		bool rad2degrees = true;
		bool LeftHandedCoords = false;
		uint keyframes;

		List<List<List<float>>> rotations;
		List<List<List<float>>> translations;
		List <BoneParts> saved_parts;
		public KeyframeWindow(List<BoneParts> parts, List<List<List<float>>> rotation_result, List<List<List<float>>> translation_result, AnimationParams Anim_Params, uint TotalKeyFrames)
		{
			InitializeComponent();
			saved_parts = parts;
			keyframes = TotalKeyFrames;
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

			rotations = rotation_result;
			translations = translation_result;

			BoneComboBox.SelectedIndex = 0;
			selected_index = BoneComboBox.SelectedIndex;


			AnimationName.Text = Anim_Params.Name;

			KeyframeScrollBar.Maximum = ((int)TotalKeyFrames);

			LeftHandedCoordcheck.Enabled = true;

			update_text();


		}

		private void KeyframeScrollBar_Scroll(object sender, ScrollEventArgs e)
		{
			KeyframeNumber.Text = KeyframeScrollBar.Value.ToString();
			keyframe_number = KeyframeScrollBar.Value;
			update_text();
		}

		private void BoneComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			selected_index = BoneComboBox.SelectedIndex;
			update_text();
		}

		private void update_text(){
			if (rad2degrees)
			{
				RotationW.Enabled = false;
				float x = rotations[selected_index][keyframe_number][0];
				float y = rotations[selected_index][keyframe_number][1];
				float z = rotations[selected_index][keyframe_number][2];
				float w = rotations[selected_index][keyframe_number][3];
				var _q = new Quaternion(x, y, z, w);

				double[] euler_rotations = GetEulerAngles(_q, 0, 0, 0);
				double x_final = (euler_rotations[1] * (180 / Math.PI));
				double y_final = (euler_rotations[2] * (180 / Math.PI));
				double z_final;

				if (LeftHandedCoords)
					z_final = (euler_rotations[0] * (180 / Math.PI));
				else
					z_final = ((-1) * (euler_rotations[0] * (180 / Math.PI)));

				if (showValuesIn2dp.Checked){
					x_final = Math.Floor(x_final * 100) / 100;
					y_final = Math.Floor(y_final * 100) / 100;
					z_final = Math.Floor(z_final * 100) / 100;
				}
				RotationX.Text = x_final.ToString();
				RotationY.Text = y_final.ToString();
				RotationZ.Text = z_final.ToString();
			}
			else{
				RotationW.Enabled = true;
				RotationX.Text = rotations[selected_index][keyframe_number][0].ToString();
				RotationY.Text = rotations[selected_index][keyframe_number][1].ToString();
				RotationZ.Text = rotations[selected_index][keyframe_number][2].ToString();
				RotationW.Text = rotations[selected_index][keyframe_number][3].ToString();
			}

			float[] translation_values = CalculateTranslations(translations, keyframes);
			float x_final_t = translation_values[0];
			float y_final_t = translation_values[1];
			float z_final_t = translation_values[2];
			if (showValuesIn2dp.Checked){
				x_final_t = (float)(Math.Floor(x_final_t * 100) / 100);
				y_final_t = (float)(Math.Floor(y_final_t * 100) / 100);
				z_final_t = (float)(Math.Floor(z_final_t * 100) / 100);
			}
			TranslationX.Text = x_final_t.ToString();
			TranslationY.Text = y_final_t.ToString();
			TranslationZ.Text = z_final_t.ToString();
		}

		private float[] CalculateTranslations(List<List<List<float>>> translation_result, uint TotalKeyFrames)
		{
			float[] ret = new float[3];
			int list_size = translation_result[selected_index].Count();

			List<float> vector_init = translation_result[selected_index][(int)(TotalKeyFrames)];
			List<float> vector_size = new List<float>();
			if (list_size == (TotalKeyFrames+2)) {
				vector_size = translation_result[selected_index][(int)(TotalKeyFrames + 1)];
				ret[0] = (translation_result[selected_index][keyframe_number][0] * vector_size[0]) + vector_init[0];
				ret[1] = (translation_result[selected_index][keyframe_number][1] * vector_size[1]) + vector_init[1];
				ret[2] = (translation_result[selected_index][keyframe_number][2] * vector_size[2]) + vector_init[2];
			}
			else {
				ret[0] = vector_init[0];
				ret[1] = vector_init[1];
				ret[2] = vector_init[2];
			}
			// calculating the final values
			return ret;
		}

		private void rad2degreescheck_CheckedChanged(object sender, EventArgs e)
		{
			rad2degrees = rad2degreescheck.Checked;
			if (rad2degrees){
				LeftHandedCoordcheck.Enabled = true;
			}
			else 
				LeftHandedCoordcheck.Enabled = false;
			update_text();
		}

		private void LeftHandedCoordcheck_CheckedChanged(object sender, EventArgs e)
		{
			LeftHandedCoords = LeftHandedCoordcheck.Checked;
			update_text();
		}

		private void showValuesIn2dp_CheckedChanged(object sender, EventArgs e)
		{
			update_text();
		}

		private void RotationInfoButton_Click(object sender, EventArgs e)
		{
			MessageBox.Show("Rotation in degrees can be innacurate near rotation values of 0, 180, 360... and -180, -360... due to a phenomenon called \"gimbal lock\" where angles near these values cause the final solution to no longer be unqiue. Quaternion Rotation does not have this issue.", "Rotation Information");
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
