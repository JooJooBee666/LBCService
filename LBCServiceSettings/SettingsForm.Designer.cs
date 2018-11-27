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
            ((System.ComponentModel.ISupportInitialize)(this.timeoutUpDown)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // browseButton
            // 
            this.browseButton.Location = new System.Drawing.Point(704, 10);
            this.browseButton.Name = "browseButton";
            this.browseButton.Size = new System.Drawing.Size(84, 32);
            this.browseButton.TabIndex = 0;
            this.browseButton.Text = "Browse...";
            this.browseButton.UseVisualStyleBackColor = true;
            this.browseButton.Click += new System.EventHandler(this.browseButton_Click);
            // 
            // keyboardCorePathText
            // 
            this.keyboardCorePathText.Location = new System.Drawing.Point(13, 12);
            this.keyboardCorePathText.Name = "keyboardCorePathText";
            this.keyboardCorePathText.Size = new System.Drawing.Size(674, 26);
            this.keyboardCorePathText.TabIndex = 2;
            // 
            // timeoutUpDown
            // 
            this.timeoutUpDown.Location = new System.Drawing.Point(14, 107);
            this.timeoutUpDown.Maximum = new decimal(new int[] {
            99999999,
            0,
            0,
            0});
            this.timeoutUpDown.Name = "timeoutUpDown";
            this.timeoutUpDown.Size = new System.Drawing.Size(120, 26);
            this.timeoutUpDown.TabIndex = 3;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioHigh);
            this.groupBox1.Controls.Add(this.radioLow);
            this.groupBox1.Location = new System.Drawing.Point(13, 152);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(392, 64);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Default Brightness Level";
            // 
            // radioHigh
            // 
            this.radioHigh.AutoSize = true;
            this.radioHigh.Location = new System.Drawing.Point(260, 25);
            this.radioHigh.Name = "radioHigh";
            this.radioHigh.Size = new System.Drawing.Size(67, 24);
            this.radioHigh.TabIndex = 1;
            this.radioHigh.TabStop = true;
            this.radioHigh.Text = "High";
            this.radioHigh.UseVisualStyleBackColor = true;
            // 
            // radioLow
            // 
            this.radioLow.AutoSize = true;
            this.radioLow.Cursor = System.Windows.Forms.Cursors.Default;
            this.radioLow.Location = new System.Drawing.Point(20, 26);
            this.radioLow.Name = "radioLow";
            this.radioLow.Size = new System.Drawing.Size(63, 24);
            this.radioLow.TabIndex = 0;
            this.radioLow.TabStop = true;
            this.radioLow.Text = "Low";
            this.radioLow.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(140, 109);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(265, 20);
            this.label1.TabIndex = 5;
            this.label1.Text = "Default backlight timeout in seconds";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.statusTextBox);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Location = new System.Drawing.Point(447, 57);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(341, 91);
            this.groupBox2.TabIndex = 6;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Service";
            // 
            // statusTextBox
            // 
            this.statusTextBox.BackColor = System.Drawing.Color.Black;
            this.statusTextBox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.statusTextBox.Location = new System.Drawing.Point(85, 37);
            this.statusTextBox.Name = "statusTextBox";
            this.statusTextBox.ReadOnly = true;
            this.statusTextBox.Size = new System.Drawing.Size(232, 26);
            this.statusTextBox.TabIndex = 3;
            this.statusTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(19, 40);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(60, 20);
            this.label2.TabIndex = 2;
            this.label2.Text = "Status:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 47);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(193, 20);
            this.label3.TabIndex = 7;
            this.label3.Text = "Path to Keyboard_Core.dll";
            // 
            // saveButton
            // 
            this.saveButton.Location = new System.Drawing.Point(657, 167);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(131, 50);
            this.saveButton.TabIndex = 8;
            this.saveButton.Text = "Save Settings";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // openFileDialog
            // 
            this.openFileDialog.InitialDirectory = "C:\\";
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 229);
            this.Controls.Add(this.saveButton);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.timeoutUpDown);
            this.Controls.Add(this.keyboardCorePathText);
            this.Controls.Add(this.browseButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SettingsForm";
            this.Text = "LBCService Settings";
            this.TransparencyKey = System.Drawing.Color.Magenta;
            this.Load += new System.EventHandler(this.SettingsForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.timeoutUpDown)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
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
    }
}

