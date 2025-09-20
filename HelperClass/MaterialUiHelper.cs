using ReaLTaiizor.Controls;
using ReaLTaiizor.Forms;
using System.Drawing;
using System.Windows.Forms;
using SmallDemoManager.UtilClass;

namespace SmallDemoManager.HelperClass
{
    public static class MaterialUiHelper
    {
        // =================== SNACKBAR ===================

        /// <summary>
        /// Displays a Material Design snackbar message on the specified form.
        /// The snackbar can be styled as an error (red background) or as an info message.
        /// If the owner is of type <see cref="SmallDemoManager.GUI.NewGUI"/>, the
        /// form's <c>SnackBarBackColor</c> property is used for the info style.
        /// Otherwise, a system default color is applied.
        /// </summary>
        public static void ShowSnack(Form owner, string text, bool error)
        {
            var snack = new MaterialSnackBar(text, 5000, true, "OK", !error);

            if (error)
            {
                snack.BackColor = Color.FromArgb(205, 38, 38);
            }
            else if (owner is SmallDemoManager.GUI.NewGUI guiOwner)
            {
                snack.BackColor = guiOwner.SnackBarBackColor;
            }
            else
            {
                snack.BackColor = SystemColors.ControlDark;
            }

            snack.Show(owner);
        }

        // =================== MESSAGE BOX ===================

        /// <summary>
        /// Displays a Material Design styled message box with customizable title, message text, 
        /// and button labels. Optionally shows a cancel button.
        /// </summary>
        public static DialogResult ShowMaterialMsgBox(Form owner, string title, string text, string okText, bool showCancel, string cancelText)
        {
            MaterialDialog materialDialog = new(owner, title, text, okText, showCancel, cancelText, true);
            DialogResult result = materialDialog.ShowDialog(owner);

            return result;
        }

        /// <summary>
        /// Displays a Material Design message box optimized for long multi-line messages,
        /// such as error reports or batch operation logs. The message box supports 
        /// scrolling when the content is larger than the visible area.
        /// </summary>
        public static DialogResult ShowLongMessageBox(string title, string text,
            MessageBoxButtons buttons = MessageBoxButtons.OK,
            MaterialFlexibleForm.ButtonsPosition buttonPosition = MaterialFlexibleForm.ButtonsPosition.Center)
        {
            string msg = text.HardWrap(80);
            return MaterialMessageBox.Show(msg, title, buttons, buttonPosition);
        }
    }
}
