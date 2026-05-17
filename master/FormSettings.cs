using System;
using System.Windows.Forms;
using System.IO;
using Microsoft.WindowsAPICodePack.Dialogs;
using TTG_Tools;

namespace TTG_Tools
{
    public partial class FormSettings : Form
    {
        public FormSettings()
        {
            InitializeComponent();
        }

        public string SetFolder(string inputPath)
        {
            CommonOpenFileDialog folderDialog = new CommonOpenFileDialog();
            folderDialog.IsFolderPicker = true;
            folderDialog.EnsurePathExists = true;

            if (Directory.Exists(inputPath))
            {
                folderDialog.InitialDirectory = inputPath;
            }
            else
            {
                folderDialog.InitialDirectory = Application.StartupPath;
            }

            if (folderDialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                return folderDialog.FileName;
            }
            else { return inputPath; }
        }

        private void buttonSaveSettings_Click(object sender, EventArgs e)
        {
            AppData.settings.ASCII_N = (int)numericUpDownASCII.Value;
            AppData.settings.pathForInputFolder = textBoxInputFolder.Text;
            AppData.settings.pathForOutputFolder = textBoxOutputFolder.Text;
            
            if (rbNormalUnicode.Checked == true) AppData.settings.unicodeSettings = 0;
            else if (rbNonNormalUnicode2.Checked == true) AppData.settings.unicodeSettings = 1;
            else AppData.settings.unicodeSettings = 2;

            AppData.settings.supportTwdNintendoSwitch = rbTwdNintendoSwitch.Checked;

            // Save scan text file paths
            AppData.settings.scanTextFilePaths.Clear();
            foreach (var item in listBoxScanTextPaths.Items)
            {
                AppData.settings.scanTextFilePaths.Add(item.ToString());
            }

            AppData.settings.languageIndex = -1;
            if (checkLanguage.Checked)
            {
                AppData.settings.languageIndex = languageComboBox.SelectedIndex;

                string selectedLanguage = languageComboBox.Text;
                
                // Check if text contains parentheses to extract ASCII code
                if (selectedLanguage.Contains("(") && selectedLanguage.Contains(")"))
                {
                    int start = selectedLanguage.IndexOf("(") + 1;
                    int end = selectedLanguage.IndexOf(")");
                    
                    if (start < end && start > 0)
                    {
                        string str_num = selectedLanguage.Substring(start, end - start).Trim();
                        
                        if (int.TryParse(str_num, out int asciiValue) && asciiValue > 0)
                        {
                            AppData.settings.ASCII_N = asciiValue;
                        }
                        else
                        {
                            // Fallback to language name mapping
                            ApplyLanguageASCIIDefault(selectedLanguage);
                        }
                    }
                    else
                    {
                        ApplyLanguageASCIIDefault(selectedLanguage);
                    }
                }
                else
                {
                    // If no parentheses, use name mapping
                    ApplyLanguageASCIIDefault(selectedLanguage);
                }

                numericUpDownASCII.Value = AppData.settings.ASCII_N;
            }

            if (((AppData.settings.pathForInputFolder != "") && (Directory.Exists(AppData.settings.pathForInputFolder)))
                && ((AppData.settings.pathForOutputFolder != "") && (Directory.Exists(AppData.settings.pathForOutputFolder))))
            {
                Settings.SaveConfig(AppData.settings);

                if (Program.FirstTime)
                {
                    // First-time setup: close settings and launch FontEditor
                    this.Close();
                    Program.FirstTime = false;
                    FontEditor fe = new FontEditor();
                    fe.Show();
                }
            }
            else
            {
                MessageBox.Show("Please set correct paths for input and output folders!");
            }
        }

        private void ApplyLanguageASCIIDefault(string languageName)
        {
            // Remove any content in parentheses to get only the language name
            if (languageName.Contains("("))
            {
                languageName = languageName.Substring(0, languageName.IndexOf("(")).Trim();
            }

            switch (languageName)
            {
                case "Thai":
                    AppData.settings.ASCII_N = 874;
                    break;

                case "Czech":
                case "Polish":
                case "Slovak":
                case "Hungarian":
                case "Serbo-Croatian":
                case "Montenegrin":
                case "Gagauz":
                    AppData.settings.ASCII_N = 1250;
                    break;

                case "Belarusian":
                case "Bulgarian":
                case "Macedonian":
                case "Russian":
                case "Rusyn":
                case "Ukrainian":
                    AppData.settings.ASCII_N = 1251;
                    break;

                case "Basque":
                case "Catalan":
                case "Faroese":
                case "Occitan":
                case "Romansh":
                case "Swahili":
                    AppData.settings.ASCII_N = 1252;
                    break;

                case "Dutch":
                case "Greek":
                    AppData.settings.ASCII_N = 1253;
                    break;

                case "Turkish":
                    AppData.settings.ASCII_N = 1254;
                    break;

                case "Hebrew":
                    AppData.settings.ASCII_N = 1255;
                    break;

                case "Arabic":
                case "Persian":
                case "Urdu":
                    AppData.settings.ASCII_N = 1256;
                    break;

                case "Latvian":
                case "Lithuanian":
                case "Latgalian":
                case "Icelandic":
                    AppData.settings.ASCII_N = 1257;
                    break;

                case "Vietnamese":
                    AppData.settings.ASCII_N = 1258;
                    break;

                default:
                    AppData.settings.ASCII_N = 1252; // Safe default value
                    break;
            }
        }

        private void buttonOkSettings_Click(object sender, EventArgs e)
        {
            buttonSaveSettings_Click(sender, e);
            if(!Program.FirstTime) this.Close();
        }

        private void buttonCloseSettingsForm_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void numericUpDownASCII_ValueChanged(object sender, EventArgs e)
        {
            int asciiValue = (int)numericUpDownASCII.Value;
            
            switch (asciiValue)
            {
                case 873:
                    numericUpDownASCII.Value = 874;
                    break;
                case 875:
                    numericUpDownASCII.Value = 1250;
                    break;
                case 1249:
                    numericUpDownASCII.Value = 874;
                    break;
                case 1259:
                    numericUpDownASCII.Value = 1258;
                    break;
            }

            // Terrible fix for users windows-1252 encoding
            if ((int)numericUpDownASCII.Value == 1252)
            {
                if(rbNonNormalUnicode2.Checked) rbNormalUnicode.Checked = true;
                rbNonNormalUnicode2.Enabled = false;
            }
            else
            {
                rbNonNormalUnicode2.Enabled = true;

                switch (AppData.settings.unicodeSettings)
                {
                    case 0:
                        rbNormalUnicode.Checked = true;
                        break;

                    case 1:
                        rbNonNormalUnicode2.Checked = true;
                        break;

                    case 2:
                        rbNewBttF.Checked = true;
                        break;
                }
            }
        }

        private void FormSettings_Load(object sender, EventArgs e)
        {
            numericUpDownASCII.Value = AppData.settings.ASCII_N;
            textBoxInputFolder.Text = AppData.settings.pathForInputFolder;
            textBoxOutputFolder.Text = AppData.settings.pathForOutputFolder;

            buttonSaveSettings.Enabled = !Program.FirstTime;

            foreach(string lang in AppData.languagesASCII)
            {
                languageComboBox.Items.Add(lang);
            }

            checkLanguage.Checked = AppData.settings.languageIndex != -1;
            languageComboBox.Enabled = AppData.settings.languageIndex != -1;
            languageComboBox.SelectedIndex = AppData.settings.languageIndex != -1 ? languageComboBox.SelectedIndex = AppData.settings.languageIndex : 0;

            switch (AppData.settings.unicodeSettings)
            {
                case 1:
                    rbNonNormalUnicode2.Checked = true;
                    break;
                case 2:
                    rbNewBttF.Checked = true;
                    break;
                default:
                    rbNormalUnicode.Checked = true;
                    break;
            }

            rbTwdNintendoSwitch.Checked = AppData.settings.supportTwdNintendoSwitch;

            // Load scan text file paths
            listBoxScanTextPaths.Items.Clear();
            if (AppData.settings.scanTextFilePaths != null)
            {
                foreach (string path in AppData.settings.scanTextFilePaths)
                {
                    listBoxScanTextPaths.Items.Add(path);
                }
            }
        }

        private void buttonInputFolder_Click(object sender, EventArgs e)
        {
            textBoxInputFolder.Text = SetFolder(textBoxInputFolder.Text);
        }

        private void buttonOutputFolder_Click(object sender, EventArgs e)
        {
            textBoxOutputFolder.Text = SetFolder(textBoxOutputFolder.Text);
        }

        private void checkLanguage_CheckedChanged(object sender, EventArgs e)
        {
            languageComboBox.SelectedIndex = 0;
            languageComboBox.Enabled = checkLanguage.Checked;
        }

        private void buttonAddScanPath_Click(object sender, EventArgs e)
        {
            CommonOpenFileDialog folderDialog = new CommonOpenFileDialog();
            folderDialog.IsFolderPicker = true;
            folderDialog.EnsurePathExists = true;
            folderDialog.Title = "Select a folder to scan for text files";
            folderDialog.AllowNonFileSystemItems = false;

            // Set initial directory to the last added path or application path
            if (listBoxScanTextPaths.Items.Count > 0)
            {
                string lastPath = listBoxScanTextPaths.Items[listBoxScanTextPaths.Items.Count - 1].ToString();
                if (Directory.Exists(lastPath))
                {
                    folderDialog.InitialDirectory = lastPath;
                }
            }
            else
            {
                folderDialog.InitialDirectory = Application.StartupPath;
            }

            if (folderDialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                string selectedPath = folderDialog.FileName;

                // Add path if it's not already in the list
                if (!listBoxScanTextPaths.Items.Contains(selectedPath))
                {
                    listBoxScanTextPaths.Items.Add(selectedPath);
                }
                else
                {
                    MessageBox.Show("This path is already in the list.", "Path Exists",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void buttonRemoveScanPath_Click(object sender, EventArgs e)
        {
            // Remove selected item from the list
            if (listBoxScanTextPaths.SelectedIndex != -1)
            {
                listBoxScanTextPaths.Items.RemoveAt(listBoxScanTextPaths.SelectedIndex);
            }
            else
            {
                MessageBox.Show("Please select a path to remove.", "No Selection",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}


