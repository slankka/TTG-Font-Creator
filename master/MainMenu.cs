using System;
using System.Linq;
using System.Windows.Forms;

namespace TTG_Tools
{
    /// <summary>
    /// Legacy main menu — kept for reference but no longer used as entry point.
    /// FontEditor now starts directly. Use AppData for shared static data.
    /// </summary>
    [System.Obsolete("This form is no longer the entry point. Use FontEditor instead.")]
    public partial class MainMenu : Form
    {
        public MainMenu()
        {
            InitializeComponent();
        }

        private void MainMenu_Load(object sender, EventArgs e)
        {
            // Data is now initialized in AppData's static constructor.
            // Config is loaded by Program.cs via AppData.LoadConfig().
        }

        private void MainMenu_Resize(object sender, EventArgs e)
        {
        }

        private void OpenAutopacker_Form_Click(object sender, EventArgs e)
        {
            if (Application.OpenForms.OfType<AutoPacker>().Count() == 0)
            {
                Form autopacker = new AutoPacker();
                autopacker.Show();
            }
        }

        private void RunFontEditor_Click(object sender, EventArgs e)
        {
            if (Application.OpenForms.OfType<FontEditor>().Count() == 0)
            {
                Form fonteditor = new FontEditor();
                fonteditor.Show();
            }
        }

        private void About_Click(object sender, EventArgs e)
        {
            if (Application.OpenForms.OfType<About>().Count() == 0)
            {
                Form about = new About();
                about.Show();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (Application.OpenForms.OfType<TextEditor>().Count() == 0)
            {
                Form txteditor = new TextEditor();
                txteditor.Show();
            }
        }

        private void buttonSettings_Click(object sender, EventArgs e)
        {
            if (Application.OpenForms.OfType<FormSettings>().Count() == 0)
            {
                Form settings = new FormSettings();
                settings.Show(this);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (Application.OpenForms.OfType<ArchivePacker>().Count() == 0)
            {
                Form archiveForm = new ArchivePacker();
                archiveForm.Show();
            }
        }

        private void arcUnpackerBtn_Click(object sender, EventArgs e)
        {
            if (Application.OpenForms.OfType<ArchiveUnpacker>().Count() == 0)
            {
                Form arcUnpackerForm = new ArchiveUnpacker();
                arcUnpackerForm.Show();
            }
        }

        private void modCreatorBtn_Click(object sender, EventArgs e)
        {
            if (Application.OpenForms.OfType<ModCreator>().Count() == 0)
            {
                Form modCreatorForm = new ModCreator();
                modCreatorForm.Show();
            }
        }

        private void ttarch2ScannerBtn_Click(object sender, EventArgs e)
        {
            if (Application.OpenForms.OfType<Ttarch2Scanner>().Count() == 0)
            {
                Form scannerForm = new Ttarch2Scanner();
                scannerForm.Show();
            }
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Show();
            WindowState = FormWindowState.Normal;
            notifyIcon1.Visible = false;
        }
    }
}
