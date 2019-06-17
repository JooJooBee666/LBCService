namespace LBCServiceSettings
{
    partial class SettingsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsForm));
            this.browseButton = new System.Windows.Forms.Button();
            this.keyboardCorePathText = new System.Windows.Forms.TextBox();
            this.timeoutUpDown = new System.Windows.Forms.NumericUpDown();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioHigh = new System.Windows.Forms.RadioButton();
            this.radioLow = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.statusTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.saveButton = new System.Windows.Forms.Button();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.enableDebugLoggingCheck = new System.Windows.Forms.CheckBox();
            this.wakeStateCheck = new System.Windows.Forms.CheckBox();
            this.monitorDisplayState = new System.Windows.Forms.CheckBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.BacklightOff = new System.Windows.Forms.RadioButton();
            this.BacklightLow = new System.Windows.Forms.RadioButton();
            this.BacklightHigh = new System.Windows.Forms.RadioButton();
            ((System.ComponentModel.ISupportInitialize)(this.timeoutUpDown)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // browseButton
            // 
            this.browseButton.Location = new System.Drawing.Point(469, 8);
            this.browseButton.Margin = new System.Windows.Forms.Padding(2);
            this.browseButton.Name = "browseButton";
            this.browseButton.Size = new System.Drawing.Size(56, 21);
            this.browseButton.TabIndex = 0;
            this.browseButton.Text = "Browse...";
            this.browseButton.UseVisualStyleBackColor = true;
            this.browseButton.Click += new System.EventHandler(this.browseButton_Click);
            // 
            // keyboardCorePathText
            // 
            this.keyboardCorePathText.Location = new System.Drawing.Point(9, 8);
            this.keyboardCorePathText.Margin = new System.Windows.Forms.Padding(2);
            this.keyboardCorePathText.Name = "keyboardCorePathText";
            this.keyboardCorePathText.Size = new System.Drawing.Size(451, 20);
            this.keyboardCorePathText.TabIndex = 2;
            // 
            // timeoutUpDown
            // 
            this.timeoutUpDown.Location = new System.Drawing.Point(8, 60);
            this.timeoutUpDown.Margin = new System.Windows.Forms.Padding(2);
            this.timeoutUpDown.Maximum = new decimal(new int[] {
            99999999,
            0,
            0,
            0});
            this.timeoutUpDown.Name = "timeoutUpDown";
            this.timeoutUpDown.Size = new System.Drawing.Size(80, 20);
            this.timeoutUpDown.TabIndex = 3;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioHigh);
            this.groupBox1.Controls.Add(this.radioLow);
            this.groupBox1.Location = new System.Drawing.Point(197, 95);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox1.Size = new System.Drawing.Size(161, 39);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Default Brightness Level";
            // 
            // radioHigh
            // 
            this.radioHigh.AutoSize = true;
            this.radioHigh.Location = new System.Drawing.Point(95, 16);
            this.radioHigh.Margin = new System.Windows.Forms.Padding(2);
            this.radioHigh.Name = "radioHigh";
            this.radioHigh.Size = new System.Drawing.Size(47, 17);
            this.radioHigh.TabIndex = 1;
            this.radioHigh.TabStop = true;
            this.radioHigh.Text = "High";
            this.radioHigh.UseVisualStyleBackColor = true;
            // 
            // radioLow
            // 
            this.radioLow.AutoSize = true;
            this.radioLow.Cursor = System.Windows.Forms.Cursors.Default;
            this.radioLow.Location = new System.Drawing.Point(13, 17);
            this.radioLow.Margin = new System.Windows.Forms.Padding(2);
            this.radioLow.Name = "radioLow";
            this.radioLow.Size = new System.Drawing.Size(45, 17);
            this.radioLow.TabIndex = 0;
            this.radioLow.TabStop = true;
            this.radioLow.Text = "Low";
            this.radioLow.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(92, 57);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(181, 26);
            this.label1.TabIndex = 5;
            this.label1.Text = "Default backlight timeout in seconds,\r\n0 = disable timer.";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.statusTextBox);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Location = new System.Drawing.Point(298, 32);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox2.Size = new System.Drawing.Size(227, 59);
            this.groupBox2.TabIndex = 6;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Service";
            // 
            // statusTextBox
            // 
            this.statusTextBox.BackColor = System.Drawing.Color.Black;
            this.statusTextBox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.statusTextBox.Location = new System.Drawing.Point(57, 24);
            this.statusTextBox.Margin = new System.Windows.Forms.Padding(2);
            this.statusTextBox.Name = "statusTextBox";
            this.statusTextBox.ReadOnly = true;
            this.statusTextBox.Size = new System.Drawing.Size(156, 20);
            this.statusTextBox.TabIndex = 3;
            this.statusTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 26);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(40, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Status:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 31);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(130, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Path to Keyboard_Core.dll";
            // 
            // saveButton
            // 
            this.saveButton.Location = new System.Drawing.Point(438, 141);
            this.saveButton.Margin = new System.Windows.Forms.Padding(2);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(87, 32);
            this.saveButton.TabIndex = 8;
            this.saveButton.Text = "Save Settings";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // openFileDialog
            // 
            this.openFileDialog.DefaultExt = "dll";
            this.openFileDialog.FileName = "Keyboard_Core.dll";
            this.openFileDialog.Filter = "Keyboard_Core.dll|Keyboard_Core.dll";
            this.openFileDialog.InitialDirectory = "C:\\ProgramData\\Lenovo\\ImController\\Plugins\\ThinkKeyboardPlugin\\x86\\";
            this.openFileDialog.Title = "Browse for Keyboard_Core.dll";
            // 
            // enableDebugLoggingCheck
            // 
            this.enableDebugLoggingCheck.AutoSize = true;
            this.enableDebugLoggingCheck.Location = new System.Drawing.Point(9, 97);
            this.enableDebugLoggingCheck.Margin = new System.Windows.Forms.Padding(2);
            this.enableDebugLoggingCheck.Name = "enableDebugLoggingCheck";
            this.enableDebugLoggingCheck.Size = new System.Drawing.Size(135, 17);
            this.enableDebugLoggingCheck.TabIndex = 9;
            this.enableDebugLoggingCheck.Text = "Enable Debug Logging";
            this.enableDebugLoggingCheck.UseVisualStyleBackColor = true;
            // 
            // wakeStateCheck
            // 
            this.wakeStateCheck.AutoSize = true;
            this.wakeStateCheck.Location = new System.Drawing.Point(8, 118);
            this.wakeStateCheck.Margin = new System.Windows.Forms.Padding(2);
            this.wakeStateCheck.Name = "wakeStateCheck";
            this.wakeStateCheck.Size = new System.Drawing.Size(185, 17);
            this.wakeStateCheck.TabIndex = 10;
            this.wakeStateCheck.Text = "Remeber backlight state on wake";
            this.wakeStateCheck.UseVisualStyleBackColor = true;
            // 
            // monitorDisplayState
            // 
            this.monitorDisplayState.AutoSize = true;
            this.monitorDisplayState.Location = new System.Drawing.Point(8, 139);
            this.monitorDisplayState.Margin = new System.Windows.Forms.Padding(2);
            this.monitorDisplayState.Name = "monitorDisplayState";
            this.monitorDisplayState.Size = new System.Drawing.Size(122, 17);
            this.monitorDisplayState.TabIndex = 11;
            this.monitorDisplayState.Text = "Monitor display state";
            this.monitorDisplayState.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.BacklightHigh);
            this.groupBox3.Controls.Add(this.BacklightLow);
            this.groupBox3.Controls.Add(this.BacklightOff);
            this.groupBox3.Location = new System.Drawing.Point(364, 95);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(161, 41);
            this.groupBox3.TabIndex = 12;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Current Brightness Level";
            // 
            // BacklightOff
            // 
            this.BacklightOff.AutoSize = true;
            this.BacklightOff.Location = new System.Drawing.Point(7, 20);
            this.BacklightOff.Name = "BacklightOff";
            this.BacklightOff.Size = new System.Drawing.Size(39, 17);
            this.BacklightOff.TabIndex = 0;
            this.BacklightOff.TabStop = true;
            this.BacklightOff.Text = "Off";
            this.BacklightOff.UseVisualStyleBackColor = true;
            this.BacklightHigh.Enabled = false;
            // 
            // BacklightLow
            // 
            this.BacklightLow.AutoSize = true;
            this.BacklightLow.Location = new System.Drawing.Point(52, 20);
            this.BacklightLow.Enabled = false;
            this.BacklightLow.Name = "BacklightLow";
            this.BacklightLow.Size = new System.Drawing.Size(45, 17);
            this.BacklightLow.TabIndex = 1;
            this.BacklightLow.TabStop = true;
            this.BacklightLow.Text = "Low";
            this.BacklightLow.UseVisualStyleBackColor = true;
            // 
            // BacklightHigh
            // 
            this.BacklightHigh.AutoSize = true;
            this.BacklightHigh.Location = new System.Drawing.Point(103, 20);
            this.BacklightHigh.Name = "BacklightHigh";
            this.BacklightHigh.Size = new System.Drawing.Size(47, 17);
            this.BacklightHigh.TabIndex = 2;
            this.BacklightHigh.TabStop = true;
            this.BacklightHigh.Text = "High";
            this.BacklightHigh.UseVisualStyleBackColor = true;
            this.BacklightOff.Enabled = false;
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(530, 177);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.monitorDisplayState);
            this.Controls.Add(this.wakeStateCheck);
            this.Controls.Add(this.enableDebugLoggingCheck);
            this.Controls.Add(this.saveButton);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.timeoutUpDown);
            this.Controls.Add(this.keyboardCorePathText);
            this.Controls.Add(this.browseButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "SettingsForm";
            this.Text = "LBCService Settings";
            this.TransparencyKey = System.Drawing.Color.Magenta;
            this.Load += new System.EventHandler(this.SettingsForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.timeoutUpDown)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button browseButton;
        private System.Windows.Forms.TextBox keyboardCorePathText;
        private System.Windows.Forms.NumericUpDown timeoutUpDown;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radioHigh;
        private System.Windows.Forms.RadioButton radioLow;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox statusTextBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.CheckBox enableDebugLoggingCheck;
        private System.Windows.Forms.CheckBox wakeStateCheck;
        private System.Windows.Forms.CheckBox monitorDisplayState;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.RadioButton BacklightOff;
        private System.Windows.Forms.RadioButton BacklightHigh;
        private System.Windows.Forms.RadioButton BacklightLow;
    }
}

