using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Microsoft.VisualBasic.Devices;
using static System.Collections.Specialized.BitVector32;

namespace OMF_Editor
{
    public partial class OMF_Editor : Form
    {
        OMFEditor editor = new OMFEditor();

        AnimationsContainer Main_OMF;

        BindingSource bs = new BindingSource();

        string number_mask = "";

        //int StopAtEnd = 1 << 1;
        //int NoMix = 1 << 2;
        //int SyncPart = 1 << 3;
        //int UseFootSteps = 1 << 4;
        //int MoveXForm = 1 << 5;
        //int Idle = 1 << 6;
        //int UseWeaponBone = 1 << 7;

        bool bKeyIsDown = false;
        bool bTextBoxEnabled = false;
        bool bMotMarkPanel = false;

        List<CheckBox> Boxes = new List<CheckBox>();
        List<TextBox> textBoxes = new List<System.Windows.Forms.TextBox>();

        ResourceManager rm = new ResourceManager(typeof(OMF_Editor));

        public OMF_Editor()
        {
            InitializeComponent();

            number_mask = @"^[0-9.]*$";
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");

            InitButtons();

            this.Opacity = 0;

            // Very dirty hack
            if (Environment.GetCommandLineArgs().Length > 1) OpenFile(Environment.GetCommandLineArgs()[1]);

            // drag drop capability
            this.DragDrop += new DragEventHandler(this.OMF_Editor_DragDrop);
            this.DragEnter += new DragEventHandler(this.OMF_Editor_DragEnter);

            this.lbxMotions.DragDrop += new DragEventHandler(this.OMF_Editor_DragDrop);
            this.lbxMotions.DragEnter += new DragEventHandler(this.OMF_Editor_DragEnter);

        }

        private void OMF_Editor_DragDrop(object sender, DragEventArgs e)
        {
            // thanks windows

            // Handle FileDrop data.
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                // Assign the file names to a string array, in 
                // case the user has selected multiple files.
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                string main_file = files[0];
                if (!(main_file.EndsWith(".omf"))) {
                    string message = String.Format("File \n\"{0}\"\n is not an OMF file.", Path.GetFileName(main_file));
                    MessageBox.Show(message, "Error");
                    return;
                }
                try
                {
                    OpenFile(files[0]);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return;
                }
            }
        }

        private void OMF_Editor_DragEnter(object sender, DragEventArgs e)
        {
            // thanks windows

            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

        [DllImport("converter.dll")]
        private static extern int CSharpStartAgent(string path, string out_path, int mode, int convert_to_mode, string motion_list);

        private int RunConverter(string path, string out_path, int mode, int convert_to_mode, string motion_list)
        {
            string dll_path = Application.ExecutablePath.Substring(0, Application.ExecutablePath.LastIndexOf('\\')) + "\\converter.dll";
            if (File.Exists(dll_path))
                return CSharpStartAgent(path, out_path, mode, convert_to_mode, motion_list);
            else
            {
                MessageBox.Show("Can't find converter.dll", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -1;
            }
        }

        private void InitButtons()
        {
            saveAsToolStripMenuItem.Enabled = false;
            saveToolStripMenuItem.Enabled = false;
            toolsToolStripMenuItem.Enabled = false;
            showBonePartsToolStripMenuItem.Enabled = false;

            openFileDialog1.Filter = "OMF file|*.omf";
            saveFileDialog1.Filter = saveFileDialog2.Filter = "OMF file|*.omf|Skls file|*.skls|Skl file|*.skl";

            this.Text = "OMF editor [Verdatim's modified version]";

            cloneToolStripMenuItem.Enabled = false;

            // from verdatim
            Boxes.Add(chbxESMFX);

            Boxes.Add(chbxStopAtEnd);
            Boxes.Add(chbxNoMix);
            Boxes.Add(chbxSyncPart);
            Boxes.Add(chbxUseFootSteps);
            Boxes.Add(chbxMoveXForm);
            Boxes.Add(chbxIdle);
            Boxes.Add(chbxUseWeaponBone);
            // from verdatim
            Boxes.Add(chbxSkipFirstFrame);

            Boxes.Add(chbxHasMotionMarks);

            textBoxes.Add(tbxMotName);
            textBoxes.Add(tbxMotSpeed);
            textBoxes.Add(tbxMotPower);
            textBoxes.Add(tbxMotAcc);
            textBoxes.Add(tbxMotFall);
            textBoxes.Add(tbxMotLength);

            DisableInput();
        }

        private void OpenFile(string filename)
        {
            Main_OMF = editor.OpenOMF(filename);

            if (Main_OMF != null)
            {
                bs.DataSource = Main_OMF.AnimsParams;
                lbxMotions.DataSource = bs;
                lbxMotions.DisplayMember = "Name";

                // init the visual list box
                VisBox.DisplayMember = "Name";
                VisBox_UpdateIndices();


				Main_OMF.FileName = filename;

                LabelStatusFile.Text = filename.Substring(filename.LastIndexOf('\\') + 1);
                saveAsToolStripMenuItem.Enabled = true;
                saveToolStripMenuItem.Enabled = true;
                toolsToolStripMenuItem.Enabled = true;
                showBonePartsToolStripMenuItem.Enabled = true;
            }
        }

        AnimationsContainer OpenSecondOMF(string filename)
        {
            if (Main_OMF == null) return null;

            AnimationsContainer new_omf = editor.OpenOMF(filename);

            if (new_omf == null) return new_omf;

            if (!editor.CompareOMF(Main_OMF, new_omf))
                return null;
            else
                return new_omf;
        }

        private void UpdateList(bool save_pos = false)
        {
            int pos = lbxMotions.SelectedIndex;
            bs.ResetBindings(false);
            if (save_pos) lbxMotions.SelectedIndex = pos;
            MotionParamsUpdate();
        }

        private void AppendFile(string filename, List<string> list)
        {
            AnimationsContainer new_omf = OpenSecondOMF(filename);
            if (new_omf == null) return;
            editor.CopyAnims(Main_OMF, new_omf, list, chbxAskForOverwrite.Checked);
            UpdateList();
        }

        private void AppendFile(string filename)
        {
            AnimationsContainer new_omf = OpenSecondOMF(filename);
            if (new_omf == null) return;
            editor.CopyAnims(Main_OMF, new_omf, chbxAskForOverwrite.Checked);
            UpdateList();
        }

        private void SwapMarks(string filename, object sender, EventArgs e)
        {
            AnimationsContainer old_omf = Main_OMF;

            try
            {
                OpenFile(openFileDialog1.FileName);
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.ToString());
            }
            bool Checked = false;
            bool OverWrite = false;

            for (int i = 0; i < old_omf.Anims.Count; i++) // Проходимся по старому омф
            {
                if (old_omf.AnimsParams[i].MarksCount > 0)     // Если есть моушн марка тогда
                {
                    for (int a = 0; a < Main_OMF.Anims.Count; a++) // Проходимся по текущему
                    {
                        if (old_omf.Anims[i].Name == Main_OMF.Anims[a].Name)
                        {
                            if (!Checked)
                            {
                                OverWrite = MessageBox.Show("Overwrite existing motion marks?", "", MessageBoxButtons.YesNo) == DialogResult.Yes;
                                Checked = true;
                            }

                            if (!PodgotovkaIfNotExist(a, sender, e, OverWrite))       // Ставим флаг на марки
                                continue;

                            GetCurrentMotion().m_marks = old_omf.AnimsParams[i].m_marks;
                            GetCurrentMotion().MarksCount = old_omf.AnimsParams[i].MarksCount;

                            MotionMarksGroupUpdate(true);
                        }
                    }
                }
            }
        }

        private void SaveOMF(AnimationsContainer omf_file, string file_name)
        {
            using (BinaryWriter writer = new BinaryWriter(File.Create(file_name)))
            {
                editor.WriteOMF(writer, omf_file);
            }
        }

        private void SetT0(float t0)
        {
            GetCurrentMotion().m_marks[listMotionMarksGroup.SelectedIndices[0]].m_params[listMotionMarksParams.SelectedIndices[0]].t0 = t0;
        }

        private void SetT1(float t1)
        {
            GetCurrentMotion().m_marks[listMotionMarksGroup.SelectedIndices[0]].m_params[listMotionMarksParams.SelectedIndices[0]].t1 = t1;
        }

        private AnimationParams GetCurrentMotion()
        {
            if (lbxMotions.SelectedIndex != -1)
                return lbxMotions.SelectedItem as AnimationParams;
            else
                return null;
        }

        private AnimVector GetCurrentAnimVector()
        {
            if (lbxMotions.SelectedIndex != -1)
                return Main_OMF.Anims[lbxMotions.SelectedIndex];
            else
                return null;
        }

        private void MotionParamsUpdate(bool dont_reset_pos = false)
        {
            if (GetCurrentMotion() == null) return;

            if (!bTextBoxEnabled) EnableInput();

            tbxMotName.Text = GetCurrentMotion().Name;
            tbxMotSpeed.Text = GetCurrentMotion().Speed.ToString();
            tbxMotPower.Text = GetCurrentMotion().Power.ToString();
            tbxMotAcc.Text = GetCurrentMotion().Accrue.ToString();
            tbxMotFall.Text = GetCurrentMotion().Falloff.ToString();

            float speed = chbxRealTimeLength.Checked ? GetCurrentMotion().Speed : 1.0f;
            float keys_lenght = GetCurrentAnimVector().GetNumKeys() / (radioButtonSeconds.Checked ? 30.0f : 1.0f);
            keys_lenght /= speed;

            if (!radioButtonSeconds.Checked)
                keys_lenght = ((float)Math.Round(keys_lenght, MidpointRounding.AwayFromZero));

            tbxMotLength.Text = keys_lenght.ToString();
            FillFlagsStates();

            CheckMotionMarksAvaiableAndUpdate(dont_reset_pos);
        }

        private void listMotionMarksGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (bMotionMarksGroupSelected())
                MotionMarksParamUpdate(listMotionMarksGroup.SelectedIndices[0]);
            else
                MotionMarksParamUpdate(-1);
        }

        private void listMotionMarksParams_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (bMotionMarksGroupSelected() && bMotionMarkSelected())
                MotionMarksParamValuesUpdate(listMotionMarksGroup.SelectedIndices[0], listMotionMarksParams.SelectedIndices[0]);
            else
                MotionMarksParamValuesUpdate(-1, -1);
        }

        private void MotionMarksParamValuesUpdate(int index, int index2)
        {
            if (index != -1 && index2 != -1)
            {
                var currentAnimParams = GetCurrentMotion().m_marks[index].m_params[index2];
                float speed = chbxRealTimeLength.Checked ? GetCurrentMotion().Speed : 1.0f;
                ChangeMarksParamsEditState(true);
                boxStartMotionMark.Text = ((currentAnimParams.t0 * (radioButtonSeconds.Checked ? 1 : 30.0f)) / speed).ToString();
                boxEndMotionMark.Text = ((currentAnimParams.t1 * (radioButtonSeconds.Checked ? 1 : 30.0f)) / speed).ToString();
            }
            else
            {
                ChangeMarksParamsEditState(false);
            }
        }

        private void CheckMotionMarksAvaiableAndUpdate(bool dont_reset_pos = false)
        {
            if (!bMotMarkPanel && chbxHasMotionMarks.Checked)
                ChangeMotionMarksPanelState(true);
            else if (bMotMarkPanel && !chbxHasMotionMarks.Checked)
                ChangeMotionMarksPanelState(false);

            if (!dont_reset_pos) MotionMarkPanelReset();

            MotionMarksGroupUpdate(dont_reset_pos);
        }

        private void MotionMarksGroupUpdate(bool save_pos = false)
        {
            int pos = bMotionMarksGroupSelected() && save_pos ? listMotionMarksGroup.SelectedIndices[0] : 0;

            listMotionMarksGroup.Items.Clear();

            if (GetCurrentMotion().m_marks != null)
            {
                foreach (var mot in GetCurrentMotion().m_marks)
                    listMotionMarksGroup.Items.Add(mot.Name);

                if (listMotionMarksGroup.Items.Count <= pos)
                    pos = 0;

                if (listMotionMarksGroup.Items.Count > 0)
                    listMotionMarksGroup.Items[pos].Selected = true;
            }
        }

        private void MotionMarksParamUpdate(int index, bool save_pos = false)
        {
            int pos = bMotionMarkSelected() && save_pos ? listMotionMarksParams.SelectedIndices[0] : 0;

            listMotionMarksParams.Items.Clear();

            if (index != -1)
            {
                ChangeMarksParamsPanelState(true);

                var currentAnimParams = GetCurrentMotion().m_marks[index].m_params;

                for (int i = 0; i < currentAnimParams.Count; i++)
                {
                    listMotionMarksParams.Items.Add(index + "_mark" + i);
                }

                if (listMotionMarksParams.Items.Count > 0)
                    listMotionMarksParams.Items[pos].Selected = true;
            }
            else
            {
                ChangeMarksParamsPanelState(false);
                listMotionMarksParams_SelectedIndexChanged(null, null);
            }
        }

        private void FillFlagsStates()
        {
            if (Main_OMF == null) return;

            int Flags = GetCurrentMotion().Flags;

            for (int i = 1; i < 10; i++)
            {
                Boxes[i - 1].Checked = (Flags & (1 << i - 1)) == (1 << i - 1);
            }

            chbxHasMotionMarks.Checked = (Main_OMF.GetMotionVersion() == 4 && GetCurrentMotion().m_marks != null);
        }

        private void WriteAllFlags()
        {
            if (Main_OMF == null || !bTextBoxEnabled) return;

            for (int i = 1; i < 10; i++)
            {
                GetCurrentMotion().Flags = BitSet(GetCurrentMotion().Flags, (1 << i - 1), Boxes[i - 1].Checked);
            }
        }

        //Events
        private void TextBoxFilter(object sender, EventArgs e)
        {
            if (Main_OMF == null || !bTextBoxEnabled) return;

            TextBox current = sender as TextBox;

            if (bKeyIsDown)
            {
                if (current.Text.Length == 0)
                    return;

                switch (current.Tag.ToString())
                {
                    case "Speed":
                    case "Power":
                    case "Accrue":
                    case "Falloff":
                        {
                            while (current.Text.Length >= 8)
                            {
                                if (current.SelectionStart < 1)
                                    current.SelectionStart = current.Text.Length;

                                int temp = current.SelectionStart;
                                current.Text = current.Text.Remove(current.Text.Length - 1, 1);
                                current.SelectionStart = temp;
                            }
                        }
                        break;
                    default: break;
                }

                string mask = current.Tag.ToString() == "MotionName" ? @"^[A-Za-z0-9_$]*$" : number_mask;
                Match match = Regex.Match(current.Text, mask);
                if (!match.Success)
                {
                    if (current.SelectionStart < 1)
                        current.SelectionStart = current.Text.Length;

                    int temp = current.SelectionStart;
                    current.Text = current.Text.Remove(current.SelectionStart - 1, 1);
                    current.SelectionStart = temp - 1;
                }
            }

            bKeyIsDown = false;

            int OldSelect = current.SelectionStart;

            try
            {
                Convert.ToSingle(current.Text);
            }
            catch (Exception)
            {
                switch (current.Tag.ToString())
                {
                    case "Speed": current.Text = GetCurrentMotion().Speed.ToString(); current.SelectionStart = current.Text.Length; break;
                    case "Power": current.Text = GetCurrentMotion().Power.ToString(); current.SelectionStart = current.Text.Length; break;
                    case "Accrue": current.Text = GetCurrentMotion().Accrue.ToString(); current.SelectionStart = current.Text.Length; break;
                    case "Falloff": current.Text = GetCurrentMotion().Falloff.ToString(); current.SelectionStart = current.Text.Length; break;
                    default: break;
                }
            }

            switch (current.Tag.ToString())
            {
                case "Speed": GetCurrentMotion().Speed = Convert.ToSingle(current.Text); checkBox1_CheckedChanged_2(sender, e); break;
                case "Power": GetCurrentMotion().Power = Convert.ToSingle(current.Text); break;
                case "Accrue": GetCurrentMotion().Accrue = Convert.ToSingle(current.Text); break;
                case "Falloff": GetCurrentMotion().Falloff = Convert.ToSingle(current.Text); break;
                case "MotionName":
                    {
                        if (GetCurrentMotion().Name == current.Text) return;
                        GetCurrentMotion().Name = current.Text;
                        int index = GetCurrentMotion().MotionID;
                        Main_OMF.Anims[index].Name = current.Text;
                        UpdateList(true);
                    }
                    break;
                default: break;
            }

            if (current.Text.Length > 2)
            {
                if (current.Text[current.Text.Length - 2] != '.' && current.Text[current.Text.Length - 1] != '0')
                    MotionParamsUpdate(true);
            }
            else
            {
                if (current.Text.Length > 0 && current.Text[current.Text.Length - 1] != '.')
                    MotionParamsUpdate(true);
            }

            current.SelectionStart = OldSelect;
        }

        private void saveFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            string format = Path.GetExtension((sender as SaveFileDialog).FileName);

            if (format == ".omf")
                SaveOMF(Main_OMF, (sender as SaveFileDialog).FileName);
            else if (format == ".skls" || format == ".skl")
            {
                string temp_omf_name = Main_OMF.FileName.Substring(0, Main_OMF.FileName.LastIndexOf('.')) + "_temptemp176.omf";

                AnimationsContainer TempOMF = editor.OpenOMF(Main_OMF.FileName);

                if (File.Exists(temp_omf_name))
                {
                    FileInfo backup_file = new FileInfo(temp_omf_name);
                    backup_file.Delete();
                }

                TempOMF.GunslingerRepair();
                SaveOMF(TempOMF, temp_omf_name);

                RunConverter(temp_omf_name, (sender as SaveFileDialog).FileName, 1, (format == ".skl" ? 0 : 1), "");

                if (File.Exists(temp_omf_name))
                {
                    FileInfo backup_file = new FileInfo(temp_omf_name);
                    backup_file.Delete();
                }
            }
        }

        private void saveFileDialog2_FileOk(object sender, CancelEventArgs e)
        {
            string format = Path.GetExtension((sender as SaveFileDialog).FileName);

            AnimationsContainer SelectedAnims = editor.OpenOMF(Main_OMF.FileName);
            AnimationsContainer TempOMF = editor.OpenOMF(Main_OMF.FileName);
            TempOMF.GunslingerRepair();
            SelectedAnims.GunslingerRepair();
            SelectedAnims.Anims.Clear();
            SelectedAnims.AnimsParams.Clear();

            ListBox.SelectedIndexCollection _list = lbxMotions.SelectedIndices;
            int count = _list.Count;

            for (int i = 0; i < count; i++)
            {
                int idx = _list[i];
                SelectedAnims.Anims.Add(TempOMF.Anims[idx]);
                SelectedAnims.AnimsParams.Add(TempOMF.AnimsParams[idx]);
            }

            SelectedAnims.RecalcAllAnimIndex();
            SelectedAnims.RecalcAnimNum();

            if (format == ".omf")
                SaveOMF(SelectedAnims, (sender as SaveFileDialog).FileName);
            else if (format == ".skls" || format == ".skl")
            {
                string temp_omf_name = Main_OMF.FileName.Substring(0, Main_OMF.FileName.LastIndexOf('.')) + "_temptemp176.omf";
                string motion_list = "";

                for (int i = 0; i < count; i++)
                {
                    int idx = _list[i];
                    motion_list += TempOMF.Anims[idx].Name + ",";
                }

                if (File.Exists(temp_omf_name))
                {
                    FileInfo backup_file = new FileInfo(temp_omf_name);
                    backup_file.Delete();
                }

                SaveOMF(SelectedAnims, temp_omf_name);

                RunConverter(temp_omf_name, (sender as SaveFileDialog).FileName, 1, (format == ".skl" ? 0 : 1), motion_list);

                if (File.Exists(temp_omf_name))
                {
                    FileInfo backup_file = new FileInfo(temp_omf_name);
                    backup_file.Delete();
                }
            }
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (lbxMotions.SelectedIndex == -1) return;

            ListBox.SelectedIndexCollection _list = lbxMotions.SelectedIndices;
            int count = _list.Count;

            while (count > 0)
            {
                int i = _list[count - 1];
                Main_OMF.Anims.RemoveAt(i);
                Main_OMF.AnimsParams.RemoveAt(i);
                --count;
            }

            Main_OMF.RecalcAllAnimIndex();
            Main_OMF.RecalcAnimNum();
            UpdateList();
        }

        private void saveAsSelectedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (lbxMotions.SelectedIndex == -1) return;

            saveFileDialog2.FileName = "";
            saveFileDialog2.ShowDialog();
        }

        private int GetAnimNamesCount(string sname)
        {
            int count = 0;
            for (int i = 0; i < Main_OMF.Anims.Count; i++)
            {
                if (Main_OMF.Anims[i].Name == sname)
                    count++;
            }
            return count;
        }

        private void cloneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (lbxMotions.SelectedIndex == -1) return;

            ListBox.SelectedIndexCollection _list = lbxMotions.SelectedIndices;
            int count = _list.Count;

            while (count > 0)
            {
                int i = _list[count - 1];
                AnimationParams anim_params = new AnimationParams(Main_OMF.AnimsParams[i]);

                AnimVector anim = new AnimVector
                {
                    SectionId = Main_OMF.Anims[i].SectionId,
                    SectionSize = Main_OMF.Anims[i].SectionSize,
                    Name = Main_OMF.Anims[i].Name
                };

                anim.data = Main_OMF.Anims[i].data;
                anim.Name += "_copy";
                anim_params.Name += "_copy";
                string tmp_name = anim.Name;
                int counter = 1;

                if (GetAnimNamesCount(anim.Name) > 0)
                {
                    while (true)
                    {
                        if (GetAnimNamesCount(anim.Name) > 0)
                            tmp_name += counter.ToString();

                        if (GetAnimNamesCount(tmp_name) == 0)
                            break;

                        tmp_name = anim.Name;
                        counter++;
                    }
                    anim.Name += counter.ToString();
                    anim_params.Name += counter.ToString();
                }

                Main_OMF.AddAnim(anim);
                Main_OMF.AddAnimParams(anim_params);
                --count;
            }

            Main_OMF.RecalcAllAnimIndex();
            Main_OMF.RecalcAnimNum();
            UpdateList();
        }

        private void listBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;

            var index = lbxMotions.IndexFromPoint(e.Location);
            if (index != ListBox.NoMatches && lbxMotions.SelectedIndices.Contains(index))
            {
                contextMenuStrip1.Show(Cursor.Position);
                deleteToolStripMenuItem.Enabled = lbxMotions.Items.Count > 1;
                cloneToolStripMenuItem.Enabled = lbxMotions.Items.Count > 0;
                contextMenuStrip1.Visible = true;
            }
            else
            {
                contextMenuStrip1.Visible = false;
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (Main_OMF == null) return;

            WriteAllFlags();

            CheckBox current = sender as CheckBox;
            if (current.Tag.ToString() == "60FPS")
            {
                checkBox1_CheckedChanged_2(sender, e);
            }
        }

        private void listBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Delete)
            {
                if (Main_OMF.Anims.Count > 1)
                    deleteToolStripMenuItem_Click(sender, e);
            }
        }

        private void buttonAddAnims_Click(object sender, EventArgs e)
        {
            AddAnims form = new AddAnims();
            form.Owner = this;
            DialogResult result = form.ShowDialog();
            if (result == DialogResult.Cancel) form.Dispose();
            if (result == DialogResult.OK)
            {
                if (form.richTextBox1.Text == "") return;

                List<string> list = form.richTextBox1.Text.Split('\n').ToList();
                form.Dispose();

                openFileDialog1.FileName = "";
                DialogResult res = openFileDialog1.ShowDialog();

                if (res == DialogResult.OK)
                {
                    try
                    {
                        AppendFile(openFileDialog1.FileName, list);
                    }
                    catch (Exception exp)
                    {
                        MessageBox.Show(exp.ToString());
                    }
                }
            }
        }

        private void buttonRepair_Click(object sender, EventArgs e)
        {
            if (Main_OMF == null) return;

            Main_OMF.GunslingerRepair();
            SaveOMF(Main_OMF, Main_OMF.FileName);
            AutoClosingMessageBox.Show("Repaired and saved!", "", 500, MessageBoxIcon.Information);
        }

        private void textBox1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            bKeyIsDown = true;
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.S)
                buttonSave_Click(null, null);

            switch (e.KeyData)
            {
                case Keys.F4: loasToolStripMenuItem_Click(null, null); break;
                case Keys.F5: buttonSave_Click(null, null); break;
                case Keys.F6: buttonSaveAs_Click(null, null); break;
            }

            if (e.Control && e.KeyCode == Keys.C && lbxMotions.Focused)
            {
                var selected_indices = lbxMotions.SelectedIndices;

                if (selected_indices.Count == 0) return;

                string clipb = "";

                foreach (int i in selected_indices)
                {
                    clipb += Main_OMF.AnimsParams[i].Name + "\n";
                }

                AutoClosingMessageBox.Show("The animations are copied to the clipboard", "", 1000, MessageBoxIcon.Information);
                Clipboard.SetText(clipb);
            }
        }

        private void buttonMerge_Click(object sender, EventArgs e)
        {
            openFileDialog1.FileName = "";
            DialogResult res = openFileDialog1.ShowDialog();

            if (res == DialogResult.OK)
            {
                try
                {
                    AppendFile(openFileDialog1.FileName);
                }
                catch (Exception exp)
                {
                    MessageBox.Show(exp.ToString());
                }

            }
        }

        private void buttonSaveAs_Click(object sender, EventArgs e)
        {
            if (Main_OMF != null)
            {
                saveFileDialog1.FileName = "";
                saveFileDialog1.ShowDialog();
            }
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            if (Main_OMF != null)
            {
                SaveOMF(Main_OMF, Main_OMF.FileName);
                AutoClosingMessageBox.Show("Saved!", "", 500, MessageBoxIcon.Information);
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (bTextBoxEnabled && (lbxMotions.SelectedItems.Count > 1 || Main_OMF == null))
                DisableInput();
            else if (lbxMotions.SelectedItems.Count == 1)
                MotionParamsUpdate();
        }

        private void btnAddMarkGroup_Click(object sender, EventArgs e)
        {
            MotionMark motionMark = new MotionMark();
            motionMark.Name = "NewGroup" + GetCurrentMotion().m_marks.Count;
            motionMark.m_params = new List<MotionMarkParams>();

            MarkRename markRename = new MarkRename(motionMark.Name);
            markRename.ShowDialog();
            if (markRename.res)
                motionMark.Name = markRename.content;

            GetCurrentMotion().m_marks.Add(motionMark);
            GetCurrentMotion().MarksCount += 1;

            MotionMarksGroupUpdate(true);
        }

        private void btnAddMark_Click(object sender, EventArgs e)
        {
            MotionMarkParams motionMarkParams = new MotionMarkParams();
            motionMarkParams.t1 = (GetCurrentAnimVector().GetNumKeys() / (30.0f));
            GetCurrentMotion().m_marks[listMotionMarksGroup.SelectedIndices[0]].m_params.Add(motionMarkParams);
            GetCurrentMotion().m_marks[listMotionMarksGroup.SelectedIndices[0]].Count += 1;
            MotionMarksParamUpdate(listMotionMarksGroup.SelectedIndices[0], true);
        }

        private void btnDelMarkGroup_Click(object sender, EventArgs e)
        {
            if (!bMotionMarksGroupSelected()) return;

            GetCurrentMotion().m_marks.RemoveAt(listMotionMarksGroup.SelectedIndices[0]);
            GetCurrentMotion().MarksCount -= 1;
            MotionMarksGroupUpdate();

            listMotionMarksGroup_SelectedIndexChanged(null, null);
        }

        private void btnDelMark_Click(object sender, EventArgs e)
        {
            if (!bMotionMarkSelected()) return;

            GetCurrentMotion().m_marks[listMotionMarksGroup.SelectedIndices[0]].m_params.RemoveAt(listMotionMarksParams.SelectedIndices[0]);
            GetCurrentMotion().m_marks[listMotionMarksGroup.SelectedIndices[0]].Count -= 1;
            MotionMarksParamUpdate(listMotionMarksGroup.SelectedIndices[0]);

            listMotionMarksParams_SelectedIndexChanged(null, null);
        }

        private void chbxHasMotionMarks_Click(object sender, EventArgs e)
        {
            if (!chbxHasMotionMarks.Checked)
            {
                if (Main_OMF.GetMotionVersion() != 4)
                {
                    DialogResult result = MessageBox.Show("This operation will update OMF version from v3 to v4", "Upgrade.", MessageBoxButtons.YesNo);

                    if (result == DialogResult.Yes)
                        Main_OMF.bone_cont.OGF_V = 4;
                    else
                        return;
                }

                chbxHasMotionMarks.Checked = true;
                GetCurrentMotion().m_marks = new List<MotionMark>();

                CheckMotionMarksAvaiableAndUpdate();
            }
            else
            {
                // change by verdatim, only show confirmation box if motion mark count is not 0
                DialogResult result = DialogResult.Yes;
                if (GetCurrentMotion().MarksCount != 0)
                    result = MessageBox.Show("This operation will delete all existing motion marks. Are you sure?", "Warning!", MessageBoxButtons.YesNo);

                if (result == DialogResult.Yes)
                {
                    chbxHasMotionMarks.Checked = false;
                    GetCurrentMotion().MarksCount = 0;
                    GetCurrentMotion().m_marks = null;

                    CheckMotionMarksAvaiableAndUpdate();
                }
            }
        }

        private void boxMotionMark_TextChanged(object sender, EventArgs e)
        {
            if (Main_OMF == null || !(sender as Control).Enabled) return;

            if (!bMotionMarksGroupSelected() || !bMotionMarkSelected()) return;

            TextBox current = sender as TextBox;

            if (bKeyIsDown)
            {
                Match match = Regex.Match(current.Text, number_mask);
                if (!match.Success)
                {
                    int temp = current.SelectionStart;
                    current.Text = current.Text.Remove(current.SelectionStart - 1, 1);
                    current.SelectionStart = temp - 1;
                }

                float speed = chbxRealTimeLength.Checked ? GetCurrentMotion().Speed : 1.0f;
                float t = radioButtonSeconds.Checked ? 1.0f : 30.0f;

                float ck = 0.0f;

                if (!float.TryParse(boxStartMotionMark.Text, out ck) || !float.TryParse(boxEndMotionMark.Text, out ck))
                {
                    return;
                }

                if (current == boxStartMotionMark)
                    SetT0((Convert.ToSingle(boxStartMotionMark.Text) / t) * speed);
                else
                    SetT1((Convert.ToSingle(boxEndMotionMark.Text) / t) * speed);
            }

            bKeyIsDown = false;
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            MotionParamsUpdate(true);
        }

        private void Anim_timer1_Tick(object sender, EventArgs e)
        {
            this.Opacity += 0.1;

            if (this.Opacity >= 1) Anim_timer1.Enabled = false;
        }

        private bool PodgotovkaIfNotExist(int i, object sender, EventArgs e, bool Overwrite)
        {
            lbxMotions.ClearSelected();
            lbxMotions.SelectedIndex = i;

            if (GetCurrentMotion().MarksCount > 0 && !Overwrite)
                return false;

            listMotionMarksParams.Items.Clear();
            if (!chbxHasMotionMarks.Checked) chbxHasMotionMarks_Click(sender, e);

            GetCurrentMotion().m_marks.Clear();
            GetCurrentMotion().MarksCount -= 1;
            MotionMarksGroupUpdate();

            listMotionMarksGroup_SelectedIndexChanged(null, null);

            return true;
        }

        private void listMotionMarksGroup_DoubleClick(object sender, EventArgs e)
        {
            if (listMotionMarksGroup.SelectedItems.Count != 0)
            {
                MarkRename markRename = new MarkRename(listMotionMarksGroup.SelectedItems[0].Text);
                markRename.ShowDialog();
                if (markRename.res == false) return;

                listMotionMarksGroup.SelectedItems[0].Text = markRename.content;

                GetCurrentMotion().m_marks[listMotionMarksGroup.SelectedItems[0].Index].Name = markRename.content;
            }
        }

        private void loasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.FileName = "";
            DialogResult res = openFileDialog1.ShowDialog();

            if (res == DialogResult.OK)
            {
                try
                {
                    OpenFile(openFileDialog1.FileName);
                }
                catch (Exception exp)
                {
                    MessageBox.Show(exp.ToString());
                }

            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to exit?", "OMF Editor", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                Close();
        }

        private void linkLabel1_LinkClicked(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/VaIeroK/Omf-Editor");
        }

        private void linkLabel2_LinkClicked(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/Verdatim25/Omf-Editor");
        }

        private void linkLabel3_LinkClicked(object sender, EventArgs e)
        {
            MessageBox.Show("Original OMF editor by Mortany. \nModified version created by Valerok. \nFurther modified by Verdatim to add hidden flag \"esmFX\", and Anomaly Modded Exes [version_name_here] \"SkipFirstFrame\" flag for additive HUD animations.", "Acknowledgements");
        }

        private void linkLabel4_LinkClicked(object sender, EventArgs e)
        {
            MessageBox.Show("This explanation is in the perspective of Stalker Anomaly modding. Information may or may not be correct for other Stalker games/mods.\nESMFX:\n    ESMFX is a special flag used for " +
            "very few animations in Anomaly. Notable mentions include mutant and stalker hit animations such as the ones in mutant_boar_animation.omf. They are extra animations that have " +
            "their own chain of functions. it is meant to be played on top of the current animation. Though due to the nature of their implementation, it does very little.\nSkip First Frame:\n    " +
            "Skip First Frame is an Anomaly modded exes exclusive flag added for " +
            "additive HUD animations (also created by me). This flag simply copies the second keyframes translations and rotations onto the first keyframe. Only works for additive animations.\nStop At End:\n    Stop At End describes whether the motion will stop playing once it reaches the last keyframe, without this flag, the animation will reset " +
            "once it reaches the last keyframe and start playing from the first keyframe again.\nNo Mix:\n    No Mix has no current use in the stalker anomaly modded exes engine.\nSync Part:\n    " +
            "Sync Part is used for stalker NPC animations. It syncs the NPC's torso and legs blends' (animations) \"time\" together, essentially keeping the two animations in sync.\nUse Foot Steps:\n" +
            "    Use Foot Steps has no current use in the anomaly modded exes engine.\nMove XForm:\n    Move XForm is for stalker NPC animations. Upon the animation being called it: 1. Stops the " +
            "NPC's movement entirely and 2. Plays the animation.\nIdle:\n    Idle has no current use in the anomaly modded exes engine.\nUse Weapon Bone:\n    Use Weapon Bone has no current use in the anomaly modded exes engine.", "Flag Explanations");
        }

        private void swapAnimsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.FileName = "";
            DialogResult res = openFileDialog1.ShowDialog();

            if (res == DialogResult.OK)
            {
                try
                {
                    SwapMarks(openFileDialog1.FileName, sender, e);
                }
                catch (Exception exp)
                {
                    MessageBox.Show(exp.ToString());
                }
            }
        }

        private void checkBox1_CheckedChanged_2(object sender, EventArgs e)
        {
            if (bTextBoxEnabled)
            {
                MotionParamsUpdate(true);
                listMotionMarksParams_SelectedIndexChanged(null, null);
            }
        }

        private void showBonePartsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Main_OMF != null)
            {
                BonePartsWindow window = new BonePartsWindow(Main_OMF.bone_cont.parts);
                window.ShowDialog();
            }
        }

        private void renameBonesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Main_OMF != null)
            {
                RenameBones window = new RenameBones(Main_OMF.bone_cont);
                window.ShowDialog();
            }
        }

        private void ViewKeyFrames_Click(object sender, EventArgs e)
        {
            // code from verdatim 15/11/26 dd/mm/yy

            // alright time for a different approach, this just doesnt seem to work

            // eventually check out https://osf.io/prcmq/files/sedhn for quarternion to euler conversion

            List<List<List<float>>> rotation_result = new List<List<List<float>>>();
            List<List<List<float>>> translation_result = new List<List<List<float>>>();

            if (lbxMotions.SelectedIndex == -1) return;

            using (BinaryReader reader = new BinaryReader(File.Open(Main_OMF.FileName, FileMode.Open)))
            {
                //AnimationsContainer omf_file = new AnimationsContainer(reader, this);
                int SectionId = reader.ReadInt32();
                uint SectionSize = reader.ReadUInt32();

                int SectionId2 = reader.ReadInt32();
                uint SectionSize2 = reader.ReadUInt32();

                int AnimsCount = reader.ReadInt32();

                int count = AnimsCount;

                for (int i = 0; i <= lbxMotions.SelectedIndex; i++)
                {
                    AnimVector vector = new AnimVector
                    {
                        SectionId = reader.ReadInt32(),
                        SectionSize = reader.ReadUInt32(),
                        Name = editor.ReadSuperString(reader)
                    };

                    int size = (int)vector.SectionSize - (vector.Name.Length + 1);

                    if ((i) != lbxMotions.SelectedIndex)
                        vector.data = reader.ReadBytes(size);
                    else {
                        uint TotalKeyFrames = reader.ReadUInt32();
                        int Bone_count = 0;
                        for (int _i = 0; _i < Main_OMF.bone_cont.parts.Count; _i++)
                        {
                            Bone_count += Main_OMF.bone_cont.parts[_i].bones.Count;
                        }
                        for (int _i = 0; _i < Bone_count; _i++) {
                            List<List<float>> temp_bone_rot = new List<List<float>>();
                            List<List<float>> temp_bone_trans = new List<List<float>>();
                            ushort flags = reader.ReadByte();
                            float[] trans_vector_size = new float[3];
                            float[] trans_vector_init = new float[3];
                            if ((flags & flRKeyAbsent) != 0)
                            {
                                byte[] temp = reader.ReadBytes(8);
                                for (int k = 0; k < TotalKeyFrames; k++)
                                {
                                    temp_bone_rot.Add(new List<float> { 0, 0, 0, 0 });
                                }
                                rotation_result.Add(temp_bone_rot);
                            }

                            else
                            {
                                int crc_q = reader.ReadInt32();
                                for (int k = 0; k < TotalKeyFrames; k++)
                                {
                                    float x = ((float)reader.ReadInt16()) / 32767F;
                                    float y = ((float)reader.ReadInt16()) / 32767F;
                                    float z = ((float)reader.ReadInt16()) / 32767F;
                                    float w = ((float)reader.ReadInt16()) / 32767F;
                                    temp_bone_rot.Add(new List<float> { x, y, z, w });
                                }
                                rotation_result.Add(temp_bone_rot);

                            }
                            if ((flags & flTKeyPresent) != 0) {
                                int crc_t = reader.ReadInt32();
                                if ((flags & flTKey16IsBit) != 0) {
                                    for (int k = 0; k < TotalKeyFrames; k++) {
                                        float x = (float)reader.ReadInt16();
                                        float y = (float)reader.ReadInt16();
                                        float z = (float)reader.ReadInt16();
                                        temp_bone_trans.Add(new List<float> { x, y, z });
                                    }
                                }
                                else {
                                    for (int k = 0; k < TotalKeyFrames; k++)
                                    {
                                        float x = (float)reader.ReadByte();
                                        float y = (float)reader.ReadByte();
                                        float z = (float)reader.ReadByte();
                                        temp_bone_trans.Add(new List<float> { x, y, z });
                                    }
                                }

                                //byte[] vector_size = reader.ReadBytes(12);
                                //byte[] vector_initial = reader.ReadBytes(12);
                                float x_s = (float)reader.ReadSingle();
                                float y_s = (float)reader.ReadSingle();
                                float z_s = (float)reader.ReadSingle();

                                float x_i = (float)reader.ReadSingle();
                                float y_i = (float)reader.ReadSingle();
                                float z_i = (float)reader.ReadSingle();

                                temp_bone_trans.Add(new List<float> { x_i, y_i, z_i });
								temp_bone_trans.Add(new List<float> { x_s, y_s, z_s });
								translation_result.Add(temp_bone_trans);
                            }
                            else {
                                for (int k = 0; k < TotalKeyFrames; k++)
                                {
                                    temp_bone_trans.Add(new List<float> { 0, 0, 0 });
                                }
                                float x_i = (float)reader.ReadSingle();
                                float y_i = (float)reader.ReadSingle();
                                float z_i = (float)reader.ReadSingle();
                                temp_bone_trans.Add(new List<float> { x_i, y_i, z_i });
                                translation_result.Add(temp_bone_trans);
                                //byte[] vector_initial = reader.ReadBytes(12);
                            }

                        }
                        // call the keyframe window
                        KeyframeWindow window = new KeyframeWindow(Main_OMF.bone_cont.parts, rotation_result, translation_result, GetCurrentMotion(), TotalKeyFrames);
                        window.ShowDialog();
                    }
                }
            }





            //         AnimVector selected = GetCurrentAnimVector();
            //         if (selected == null) return;
            //         byte[] data = selected.data;

            //         // the binary data going forward is presented as such: a u32 4 byte string for the number of keyframes, and then a u8 flag for the animation data,
            //         // followed by the animation data for each bone (which consists of a rotation and translation key set)
            //         // for every bone id, there are rotation and translation key sets for each frame
            //         // the order is as such: bone_id -> frame -> rotation + translation
            //         // each bone_id is not represented in the binary data, though it dictates how many sets of frames tied to rotation and translation keys there are
            //         // the frame is similar
            //         // the rotation set is 4 quarternions of s16 size, i.e. 2 bytes, total 8 bytes
            //         // each translation set is 3 x,y,z coordinates of either s8, i.e. 1 byte, or s16, i.e. 2 bytes, size dicated from the flag above, total 3 or 6 bytes
            //         // flag definitions are below (straight from the engine at SkeletonMotions.h at line 20)

            //         // rotation values are quantized, i.e. they are the values multiplied by 32767 (16bit signed int limit)
            //         // to return to the old values, one must multiply by (1/32767) or just divide by 32767


            //         // get the bone count
            //         int Bone_count = 0;
            //for (int i = 0; i < Main_OMF.bone_cont.parts.Count; i++)
            //{
            //             Bone_count += Main_OMF.bone_cont.parts[i].bones.Count;
            //}


            //uint TotalKeyFrames = BitConverter.ToUInt32(data, 0);
            //         // fourth byte in the data, indices start at 0

            //         int stream_index = 4;


            //// iterate across bones
            //for (int i = 0; i < Bone_count; i++) {
            //	List<List<float>> temp_bone_rot = new List<List<float>>();
            //	List<List<short>> temp_bone_trans = new List<List<short>>();
            //             ushort flags = data[stream_index];
            //             stream_index++;
            //             bool rot_absent = false;
            //             for (int k = 0; k < TotalKeyFrames; k++)
            //             {
            //                 // test for rotation key presence flag
            //                 if ((flags & flRKeyAbsent) != 0)
            //                 {
            //                     // move stream index by 8
            //                     float x = BitConverter.ToInt16(data, stream_index) / 32767;
            //			float y = BitConverter.ToInt16(data, stream_index + 2) / 32767;
            //			float z = BitConverter.ToInt16(data, stream_index + 4) / 32767;
            //			float w = BitConverter.ToInt16(data, stream_index + 6) / 32767;
            //                     temp_bone_rot.Add(new List<float> { x, y, z, w });
            //                     if (!rot_absent)
            //                     {
            //                         stream_index += 8;
            //                         rot_absent = true;
            //                     }
            //                 }
            //                 else
            //                 {
            //                     // crc_q value
            //                     stream_index += 4;
            //                     // length of the rotation values is the total keyframes * 8
            //                     int length = 8;
            //			//rotation_result.Capacity += 4;
            //			float x = BitConverter.ToInt16(data, stream_index) / 32767;
            //			float y = BitConverter.ToInt16(data, stream_index + 2) / 32767;
            //			float z = BitConverter.ToInt16(data, stream_index + 4) / 32767;
            //			float w = BitConverter.ToInt16(data, stream_index + 6) / 32767;

            //                     temp_bone_rot.Add(new List<float> { x, y, z, w });

            //                     //rotation_result[i][k][0] = x;
            //                     //rotation_result[i][k][1] = y;
            //                     //rotation_result[i][k][2] = z;
            //                     //rotation_result[i][k][3] = w;
            //                     stream_index += length;
            //                 }
            //             }
            //	bool translation_absent = false;
            //	for (int k = 0; k < TotalKeyFrames; k++){
            //		if ((flags & flTKeyPresent) != 0)
            //                 {
            //                     // crc_t value
            //                     stream_index += 4;
            //                     if ((flags & flTKey16IsBit) != 0)
            //                     {
            //                         int length = 6;
            //                         short x = BitConverter.ToInt16(data, stream_index);
            //                         short y = BitConverter.ToInt16(data, stream_index + 2);
            //                         short z = BitConverter.ToInt16(data, stream_index + 4);

            //                         temp_bone_trans.Add(new List<short> { x, y, z });

            //                         //translation_result.Capacity += 3;
            //                         //translation_result[i][k][0] = x;
            //                         //translation_result[i][k][1] = y;
            //                         //translation_result[i][k][2] = z;
            //                         stream_index += length;
            //                     }
            //                     else
            //                     {
            //                         int length = 3;
            //                         short x = data[stream_index];
            //                         short y = data[stream_index + 1];
            //                         short z = data[stream_index + 2];
            //                         temp_bone_trans.Add(new List<short> { x, y, z });
            //                         //translation_result[i][k][0] = x;
            //                         //translation_result[i][k][1] = y;
            //                         //translation_result[i][k][2] = z;
            //                         stream_index += length;
            //                     }
            //			// reading the "_sizeT"
            //			if ((k + 1) == TotalKeyFrames)
            //			{
            //                         stream_index += 12;
            //		    }


            //		}
            //                 else
            //                 {
            //                     temp_bone_trans.Add(new List<short> { 0, 0, 0 });
            //                 }
            //		// reading "_initT"
            //		if ((k+1) == TotalKeyFrames){
            //                     stream_index += 12;
            //		}

            //	}
            //             rotation_result.Add(temp_bone_rot);
            //             translation_result.Add(temp_bone_trans);

            //}
            //         // call the keyframe window
            //KeyframeWindow window = new KeyframeWindow(Main_OMF.bone_cont.parts, rotation_result, translation_result, GetCurrentMotion(), TotalKeyFrames);
            //window.ShowDialog();

        }
        int flTKeyPresent = (1 << 0);
        int flRKeyAbsent = (1 << 1);
        int flTKey16IsBit = (1 << 2);

        private void toolTip1_Popup(object sender, PopupEventArgs e)
        {
            return;
		}

		private void showFlagToolTipsToolStripMenuItem_Click(object sender, EventArgs e)
		{
            showFlagToolTipsToolStripMenuItem.Checked = !(showFlagToolTipsToolStripMenuItem.Checked);
            // dirty hack to essentially "disable" the tool tips, youd have to hover for 16.6 min to get the tooltip
            FlagsToolTip.InitialDelay = (showFlagToolTipsToolStripMenuItem.Checked) ? 1000 : 999999;
		}

		private void renameMotionMarksToolStripMenuItem_Click(object sender, EventArgs e)
		{
			RenameMotionMarks window = new RenameMotionMarks(Main_OMF);
			window.ShowDialog();
		}

		private void deleteEmptyMotionMarksToolStripMenuItem_Click(object sender, EventArgs e)
		{
            DialogResult result = DialogResult.No;
            result = MessageBox.Show("Delete Empty Motion Mark Groups?", "Warning!", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes){
                foreach (AnimationParams Params in Main_OMF.AnimsParams) {
                    if (Params.m_marks != null){
                        List<MotionMark> Remove_list = new List<MotionMark>();
						foreach (MotionMark Mark in Params.m_marks){
                            if (Mark.Count == 0){
                                Remove_list.Add(Mark);
                                Params.MarksCount--;
                            }
                        }
                        foreach (MotionMark Mark in Remove_list){
                            Params.m_marks.Remove(Mark);
                        }
                    }
                }
            }
		}

        public class motion_binder {
            public string Name { get; set; }
            public int index { get; set; }
        }

		private void VisBox_UpdateIndices()
		{
            VisBox.Items.Clear();
            int _i = 0;
            foreach (AnimationParams Params in lbxMotions.Items){
                motion_binder motion = new motion_binder();
                if (Params.Name.Contains(SearchBox.Text))
                {
                    motion.Name = Params.Name;
                    motion.index = _i;
                    VisBox.Items.Add(motion);
                }
				_i++;
			}
		}
		private void VisBox_SelectedIndexChanged_1(object sender, EventArgs e)
		{
			motion_binder temp = (motion_binder)VisBox.SelectedItem;
            //lbxMotions.SelectedIndex = temp.index;
            lbxMotions.ClearSelected();
            foreach (motion_binder binder in VisBox.SelectedItems){
				lbxMotions.SetSelected(binder.index, true);
			}
            listBox1_SelectedIndexChanged(sender, e);
		}

		private void SearchBox_TextChanged(object sender, EventArgs e)
		{
            VisBox_UpdateIndices();
		}
	}
}
