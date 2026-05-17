using System;
using System.IO;
using System.Windows.Forms;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace TTG_Tools
{
    public partial class AutoDePackerSettings : Form
    {
        public AutoDePackerSettings()
        {
            InitializeComponent();
        }

        private void cancelBtn_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void AutoDePackerSettings_Load(object sender, EventArgs e)
        {
            if (AppData.settings.tsvFormat)
            {
                tsvFilesRB.Checked = true;
            }
            else
            {
                if (!AppData.settings.tsvFormat && AppData.settings.newTxtFormat) newTxtFormatRB.Checked = true;
                else txtFilesRB.Checked = true;
            }

            checkBoxChangeLangFlags.Enabled = AppData.settings.newTxtFormat;
            checkBoxChangeLangFlags.Visible = AppData.settings.newTxtFormat;
            checkBoxChangeLangFlags.Checked = AppData.settings.changeLangFlags;

            switch(AppData.settings.unicodeSettings)
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

            checkBoxSortStrings.Checked = AppData.settings.sortSameString;
            clearMessagesCB.Checked = AppData.settings.clearMessages;
            checkBoxD3DTX_after_import.Checked = AppData.settings.deleteD3DTXafterImport;
            checkBoxDDS_after_import.Checked = AppData.settings.deleteDDSafterImport;
            checkBoxExportRealID.Checked = AppData.settings.exportRealID;
            checkBoxImportingOfNames.Checked = AppData.settings.importingOfName;
            cbIgnoreEmptyStrings.Checked = AppData.settings.ignoreEmptyStrings;

            textBoxInputFolder.Text = AppData.settings.pathForInputFolder;
            textBoxOutputFolder.Text = AppData.settings.pathForOutputFolder;
        }

        private void okBtn_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(textBoxInputFolder.Text)) AppData.settings.pathForInputFolder = textBoxInputFolder.Text;
            if (Directory.Exists(textBoxOutputFolder.Text)) AppData.settings.pathForOutputFolder = textBoxOutputFolder.Text;

            AppData.settings.clearMessages = clearMessagesCB.Checked;
            AppData.settings.sortSameString = checkBoxSortStrings.Checked;
            AppData.settings.deleteD3DTXafterImport = checkBoxD3DTX_after_import.Checked;
            AppData.settings.deleteDDSafterImport = checkBoxDDS_after_import.Checked;
            AppData.settings.exportRealID = checkBoxExportRealID.Checked;
            AppData.settings.importingOfName = checkBoxImportingOfNames.Checked;
            AppData.settings.changeLangFlags = checkBoxChangeLangFlags.Checked;
            AppData.settings.ignoreEmptyStrings = cbIgnoreEmptyStrings.Checked;

            if (rbNormalUnicode.Checked) AppData.settings.unicodeSettings = 0;
            else if (rbNonNormalUnicode2.Checked) AppData.settings.unicodeSettings = 1;
            else AppData.settings.unicodeSettings = 2;

            AppData.settings.supportTwdNintendoSwitch = rbTwdNintendoSwitch.Checked;

            if (tsvFilesRB.Checked)
            {
                AppData.settings.tsvFormat = true;
                AppData.settings.newTxtFormat = false;
            }
            else
            {
                AppData.settings.newTxtFormat = !txtFilesRB.Checked && newTxtFormatRB.Checked;
                AppData.settings.tsvFormat = false;
            }

            Settings.SaveConfig(AppData.settings);

            Close();
        }

        private void newTxtFormatRB_CheckedChanged(object sender, EventArgs e)
        {
            checkBoxChangeLangFlags.Enabled = newTxtFormatRB.Checked;
            checkBoxChangeLangFlags.Visible = newTxtFormatRB.Checked;
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

        private void buttonInputFolder_Click(object sender, EventArgs e)
        {
            textBoxInputFolder.Text = SetFolder(textBoxInputFolder.Text);
        }

        private void buttonOutputFolder_Click(object sender, EventArgs e)
        {
            textBoxOutputFolder.Text = SetFolder(textBoxOutputFolder.Text);
        }
    }
}

