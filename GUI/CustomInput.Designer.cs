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
            BTN_OK = new ReaLTaiizor.Controls.MaterialButton();
            BTN_Cancle = new ReaLTaiizor.Controls.MaterialButton();
            ComboBox_DemoFileNameOption = new ReaLTaiizor.Controls.MaterialComboBox();
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
            BTN_OK.Location = new Point(257, 114);
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
            BTN_Cancle.Location = new Point(377, 114);
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
            // ComboBox_DemoFileNameOption
            // 
            ComboBox_DemoFileNameOption.AutoResize = false;
            ComboBox_DemoFileNameOption.BackColor = Color.FromArgb(255, 255, 255);
            ComboBox_DemoFileNameOption.Depth = 0;
            ComboBox_DemoFileNameOption.DrawMode = DrawMode.OwnerDrawVariable;
            ComboBox_DemoFileNameOption.DropDownHeight = 174;
            ComboBox_DemoFileNameOption.DropDownStyle = ComboBoxStyle.DropDownList;
            ComboBox_DemoFileNameOption.DropDownWidth = 121;
            ComboBox_DemoFileNameOption.Font = new Font("Microsoft Sans Serif", 14F, FontStyle.Bold, GraphicsUnit.Pixel);
            ComboBox_DemoFileNameOption.ForeColor = Color.FromArgb(222, 0, 0, 0);
            ComboBox_DemoFileNameOption.FormattingEnabled = true;
            ComboBox_DemoFileNameOption.IntegralHeight = false;
            ComboBox_DemoFileNameOption.ItemHeight = 43;
            ComboBox_DemoFileNameOption.Items.AddRange(new object[] { "Expand original name", "Completely new name", "Keep original name" });
            ComboBox_DemoFileNameOption.Location = new Point(20, 107);
            ComboBox_DemoFileNameOption.Margin = new Padding(3, 10, 20, 3);
            ComboBox_DemoFileNameOption.MaxDropDownItems = 4;
            ComboBox_DemoFileNameOption.MouseState = ReaLTaiizor.Helper.MaterialDrawHelper.MaterialMouseState.OUT;
            ComboBox_DemoFileNameOption.Name = "ComboBox_DemoFileNameOption";
            ComboBox_DemoFileNameOption.Size = new Size(212, 49);
            ComboBox_DemoFileNameOption.StartIndex = 0;
            ComboBox_DemoFileNameOption.TabIndex = 4;
            ComboBox_DemoFileNameOption.SelectedIndexChanged += ComboBox_DemoFileNameOption_SelectedIndexChanged;
            // 
            // CustomInput
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            ClientSize = new Size(500, 175);
            Controls.Add(ComboBox_DemoFileNameOption);
            Controls.Add(BTN_Cancle);
            Controls.Add(BTN_OK);
            Controls.Add(TB_Input);
            DrawerShowIconsWhenHidden = true;
            Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            FormStyle = ReaLTaiizor.Enum.Material.FormStyles.ActionBar_None;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MaximumSize = new Size(500, 175);
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
        }

        #endregion

        private ReaLTaiizor.Controls.MaterialTextBoxEdit TB_Input;
        private ReaLTaiizor.Controls.MaterialButton BTN_OK;
        private ReaLTaiizor.Controls.MaterialButton BTN_Cancle;
        private ReaLTaiizor.Controls.MaterialComboBox ComboBox_DemoFileNameOption;
    }
}