namespace SmallDemoManager.GUI
{
    partial class CustomInput
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CustomInput));
            TB_Input = new ReaLTaiizor.Controls.MaterialTextBoxEdit();
            LBL_TBInfo = new ReaLTaiizor.Controls.MaterialLabel();
            BTN_OK = new ReaLTaiizor.Controls.MaterialButton();
            BTN_Cancle = new ReaLTaiizor.Controls.MaterialButton();
            SuspendLayout();
            // 
            // TB_Input
            // 
            TB_Input.AnimateReadOnly = false;
            TB_Input.AutoCompleteMode = AutoCompleteMode.None;
            TB_Input.AutoCompleteSource = AutoCompleteSource.None;
            TB_Input.BackgroundImageLayout = ImageLayout.None;
            TB_Input.CharacterCasing = CharacterCasing.Normal;
            TB_Input.Depth = 0;
            TB_Input.Font = new Font("Microsoft Sans Serif", 15.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            TB_Input.HideSelection = true;
            TB_Input.Hint = "Please enter the new filename (without extension)";
            TB_Input.LeadingIcon = null;
            TB_Input.Location = new Point(20, 44);
            TB_Input.Margin = new Padding(20, 20, 20, 0);
            TB_Input.MaxLength = 32767;
            TB_Input.MouseState = ReaLTaiizor.Helper.MaterialDrawHelper.MaterialMouseState.OUT;
            TB_Input.Name = "TB_Input";
            TB_Input.PasswordChar = '\0';
            TB_Input.PrefixSuffixText = null;
            TB_Input.ReadOnly = false;
            TB_Input.RightToLeft = RightToLeft.No;
            TB_Input.SelectedText = "";
            TB_Input.SelectionLength = 0;
            TB_Input.SelectionStart = 0;
            TB_Input.ShortcutsEnabled = true;
            TB_Input.Size = new Size(457, 48);
            TB_Input.TabIndex = 0;
            TB_Input.TabStop = false;
            TB_Input.TextAlign = HorizontalAlignment.Left;
            TB_Input.TrailingIcon = null;
            TB_Input.UseSystemPasswordChar = false;
            // 
            // LBL_TBInfo
            // 
            LBL_TBInfo.AutoSize = true;
            LBL_TBInfo.Depth = 0;
            LBL_TBInfo.Enabled = false;
            LBL_TBInfo.Font = new Font("Roboto", 12F, FontStyle.Regular, GraphicsUnit.Pixel);
            LBL_TBInfo.FontType = ReaLTaiizor.Manager.MaterialSkinManager.FontType.Caption;
            LBL_TBInfo.Location = new Point(25, 97);
            LBL_TBInfo.Margin = new Padding(25, 5, 0, 0);
            LBL_TBInfo.MouseState = ReaLTaiizor.Helper.MaterialDrawHelper.MaterialMouseState.HOVER;
            LBL_TBInfo.Name = "LBL_TBInfo";
            LBL_TBInfo.Size = new Size(244, 14);
            LBL_TBInfo.TabIndex = 1;
            LBL_TBInfo.Text = "Your input will be added before the file name.";
            // 
            // BTN_OK
            // 
            BTN_OK.AutoSize = false;
            BTN_OK.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            BTN_OK.Density = ReaLTaiizor.Controls.MaterialButton.MaterialButtonDensity.Default;
            BTN_OK.Depth = 0;
            BTN_OK.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            BTN_OK.HighEmphasis = true;
            BTN_OK.Icon = null;
            BTN_OK.IconType = ReaLTaiizor.Controls.MaterialButton.MaterialIconType.Rebase;
            BTN_OK.Location = new Point(217, 121);
            BTN_OK.Margin = new Padding(0, 10, 20, 0);
            BTN_OK.MouseState = ReaLTaiizor.Helper.MaterialDrawHelper.MaterialMouseState.HOVER;
            BTN_OK.Name = "BTN_OK";
            BTN_OK.NoAccentTextColor = Color.Empty;
            BTN_OK.Size = new Size(100, 36);
            BTN_OK.TabIndex = 2;
            BTN_OK.Text = "OK";
            BTN_OK.Type = ReaLTaiizor.Controls.MaterialButton.MaterialButtonType.Outlined;
            BTN_OK.UseAccentColor = true;
            BTN_OK.UseVisualStyleBackColor = true;
            // 
            // BTN_Cancle
            // 
            BTN_Cancle.AutoSize = false;
            BTN_Cancle.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            BTN_Cancle.CharacterCasing = ReaLTaiizor.Controls.MaterialButton.CharacterCasingEnum.Normal;
            BTN_Cancle.Density = ReaLTaiizor.Controls.MaterialButton.MaterialButtonDensity.Default;
            BTN_Cancle.Depth = 0;
            BTN_Cancle.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            BTN_Cancle.HighEmphasis = true;
            BTN_Cancle.Icon = null;
            BTN_Cancle.IconType = ReaLTaiizor.Controls.MaterialButton.MaterialIconType.Rebase;
            BTN_Cancle.Location = new Point(337, 121);
            BTN_Cancle.Margin = new Padding(0, 0, 60, 0);
            BTN_Cancle.MouseState = ReaLTaiizor.Helper.MaterialDrawHelper.MaterialMouseState.HOVER;
            BTN_Cancle.Name = "BTN_Cancle";
            BTN_Cancle.NoAccentTextColor = Color.Empty;
            BTN_Cancle.Size = new Size(100, 36);
            BTN_Cancle.TabIndex = 3;
            BTN_Cancle.Text = "Cancle";
            BTN_Cancle.Type = ReaLTaiizor.Controls.MaterialButton.MaterialButtonType.Outlined;
            BTN_Cancle.UseAccentColor = true;
            BTN_Cancle.UseVisualStyleBackColor = true;
            // 
            // CustomInput
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            ClientSize = new Size(500, 175);
            Controls.Add(BTN_Cancle);
            Controls.Add(BTN_OK);
            Controls.Add(LBL_TBInfo);
            Controls.Add(TB_Input);
            DrawerShowIconsWhenHidden = true;
            Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            FormStyle = ReaLTaiizor.Enum.Material.FormStyles.ActionBar_None;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MaximumSize = new Size(500, 250);
            MdiChildrenMinimizedAnchorBottom = false;
            MinimizeBox = false;
            MinimumSize = new Size(500, 175);
            Name = "CustomInput";
            Padding = new Padding(0, 24, 3, 3);
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "New Filename";
            TopMost = true;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ReaLTaiizor.Controls.MaterialTextBoxEdit TB_Input;
        private ReaLTaiizor.Controls.MaterialLabel LBL_TBInfo;
        private ReaLTaiizor.Controls.MaterialButton BTN_OK;
        private ReaLTaiizor.Controls.MaterialButton BTN_Cancle;
    }
}