using ReaLTaiizor.Forms;
using ReaLTaiizor.Manager;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace SmallDemoManager.GUI
{
    public partial class CustomInput : MaterialForm
    {
        public string InputText => TB_Input.Text;

        public CustomInput()
        {
            InitializeComponent();

            // Dialog-Verhalten
            BTN_OK.DialogResult = DialogResult.OK;
            BTN_Cancle.DialogResult = DialogResult.Cancel;
            AcceptButton = BTN_OK;
            CancelButton = BTN_Cancle;
            Sizable = false;

            // Fokus + SelectAll beim Anzeigen
            Shown += (_, __) => { TB_Input.Focus(); TB_Input.SelectAll(); };

            // WICHTIG: Beim Skin-Manager registrieren, damit Styles greifen
            var mgr = MaterialSkinManager.Instance;
            mgr.AddFormToManage(this);
            // Wenn du in der MainForm bereits Theme/ColorScheme gesetzt hast,
            // brauchst du hier nichts weiter zu tun – der Manager ist singleton.
            // OPTIONAL: Falls du sicher vom Owner übernehmen willst, nutze unten ShowInput(owner).
        }

        // Bequemer statischer Aufruf mit Owner: übernimmt Theme/ColorScheme vom Owner-MaterialForm
        public static string ShowInput(IWin32Window owner)
        {
            using var dlg = new CustomInput();

            // Theme/ColorScheme explizit vom Owner übernehmen (falls Owner ein MaterialForm ist)
            if (owner is MaterialForm mf)
            {
                var mgr = MaterialSkinManager.Instance;
                // Schon registriert, aber sicher ist sicher:
                mgr.AddFormToManage(dlg);
                // Wenn du in deiner App mehrere Themes nutzt, kannst du hier gezielt übernehmen:
                // mgr.Theme = mgr.Theme; // meist schon identisch
                // mgr.ColorScheme = mgr.ColorScheme; // meist schon identisch
                // In der Praxis reicht oft AddFormToManage(dlg).
            }

            return dlg.ShowDialog(owner) == DialogResult.OK ? dlg.InputText : null;
        }
    }
}
