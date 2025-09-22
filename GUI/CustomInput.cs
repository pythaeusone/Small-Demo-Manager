using ReaLTaiizor.Forms;
using ReaLTaiizor.Manager;
using System.Windows.Forms;

namespace SmallDemoManager.GUI
{
    public partial class CustomInput : MaterialForm
    {
        /// <summary>
        /// Returns the user input from the TextBox.
        /// </summary>
        public string InputText => TB_Input.Text;

        /// <summary>
        /// Returns the currently selected index of the ComboBox as string.
        /// Default is "0".
        /// </summary>
        public string ComboBoxNumber { get; private set; } = "0";

        public CustomInput()
        {
            InitializeComponent();

            // Configure dialog behavior
            BTN_OK.DialogResult = DialogResult.OK;
            BTN_Cancle.DialogResult = DialogResult.Cancel;
            AcceptButton = BTN_OK;
            CancelButton = BTN_Cancle;
            Sizable = false;

            // Focus + SelectAll when the dialog is shown
            Shown += (_, __) => { TB_Input.Focus(); TB_Input.SelectAll(); };

            // Register this form with the MaterialSkinManager so theme/styles are applied
            var mgr = MaterialSkinManager.Instance;
            mgr.AddFormToManage(this);
        }

        /// <summary>
        /// Static helper to show the CustomInput dialog.
        /// Returns an array:
        /// [0] = user input text
        /// [1] = ComboBox selected index (as string)
        /// </summary>
        public static string[] ShowInput(IWin32Window owner)
        {
            using var dlg = new CustomInput();

            // Explicitly register with the manager if owner is also a MaterialForm
            if (owner is MaterialForm mf)
            {
                var mgr = MaterialSkinManager.Instance;
                mgr.AddFormToManage(dlg);
            }

            // Return both input text and ComboBox index
            string[] outputForInput =
            {
                dlg.ShowDialog(owner) == DialogResult.OK ? dlg.InputText : null,
                dlg.ComboBoxNumber
            };

            return outputForInput;
        }

        /// <summary>
        /// Event handler for ComboBox selection change.
        /// Updates ComboBoxNumber with the selected index.
        /// </summary>
        private void ComboBox_DemoFileNameOption_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (sender is ComboBox combo)
            {
                ComboBoxNumber = combo.SelectedIndex.ToString();

                if (combo.SelectedIndex == 2)
                {
                    TB_Input.Enabled = false;
                    TB_Input.Text = "Original";
                }
                else
                {
                    TB_Input.Enabled = true;
                    TB_Input.Text = "";
                }
            }
        }
    }
}
