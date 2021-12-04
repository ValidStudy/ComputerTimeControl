﻿
namespace TimeControl
{
    partial class ControlPanel
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
            this.startButton = new System.Windows.Forms.Button();
            this.timeBox = new System.Windows.Forms.NumericUpDown();
            this.controlTab = new System.Windows.Forms.TabControl();
            this.startNow = new System.Windows.Forms.TabPage();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.unlockPasswordBox = new System.Windows.Forms.TextBox();
            this.processKiller = new System.Windows.Forms.TabPage();
            ((System.ComponentModel.ISupportInitialize)(this.timeBox)).BeginInit();
            this.controlTab.SuspendLayout();
            this.startNow.SuspendLayout();
            this.SuspendLayout();
            // 
            // startButton
            // 
            this.startButton.Location = new System.Drawing.Point(307, 257);
            this.startButton.Name = "startButton";
            this.startButton.Size = new System.Drawing.Size(187, 96);
            this.startButton.TabIndex = 0;
            this.startButton.Text = "启动屏保（分）";
            this.startButton.UseVisualStyleBackColor = true;
            this.startButton.Click += new System.EventHandler(this.startButton_Click);
            // 
            // timeBox
            // 
            this.timeBox.Location = new System.Drawing.Point(324, 163);
            this.timeBox.Maximum = new decimal(new int[] {
            180,
            0,
            0,
            0});
            this.timeBox.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.timeBox.Name = "timeBox";
            this.timeBox.Size = new System.Drawing.Size(150, 27);
            this.timeBox.TabIndex = 1;
            this.timeBox.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // controlTab
            // 
            this.controlTab.Controls.Add(this.startNow);
            this.controlTab.Controls.Add(this.processKiller);
            this.controlTab.Location = new System.Drawing.Point(2, 1);
            this.controlTab.Name = "controlTab";
            this.controlTab.SelectedIndex = 0;
            this.controlTab.Size = new System.Drawing.Size(802, 449);
            this.controlTab.TabIndex = 2;
            // 
            // startNow
            // 
            this.startNow.Controls.Add(this.label2);
            this.startNow.Controls.Add(this.label1);
            this.startNow.Controls.Add(this.unlockPasswordBox);
            this.startNow.Controls.Add(this.startButton);
            this.startNow.Controls.Add(this.timeBox);
            this.startNow.Location = new System.Drawing.Point(4, 29);
            this.startNow.Name = "startNow";
            this.startNow.Padding = new System.Windows.Forms.Padding(3);
            this.startNow.Size = new System.Drawing.Size(794, 416);
            this.startNow.TabIndex = 0;
            this.startNow.Text = "立即启动";
            this.startNow.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.Color.Red;
            this.label2.Location = new System.Drawing.Point(198, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(309, 20);
            this.label2.TabIndex = 4;
            this.label2.Text = "设置管理码后，在屏保时可以凭管理码解锁。";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(54, 20);
            this.label1.TabIndex = 3;
            this.label1.Text = "管理码";
            // 
            // unlockPasswordBox
            // 
            this.unlockPasswordBox.Location = new System.Drawing.Point(66, 6);
            this.unlockPasswordBox.Name = "unlockPasswordBox";
            this.unlockPasswordBox.Size = new System.Drawing.Size(125, 27);
            this.unlockPasswordBox.TabIndex = 2;
            this.unlockPasswordBox.UseSystemPasswordChar = true;
            // 
            // processKiller
            // 
            this.processKiller.Location = new System.Drawing.Point(4, 29);
            this.processKiller.Name = "processKiller";
            this.processKiller.Padding = new System.Windows.Forms.Padding(3);
            this.processKiller.Size = new System.Drawing.Size(794, 416);
            this.processKiller.TabIndex = 1;
            this.processKiller.Text = "进程监测";
            this.processKiller.UseVisualStyleBackColor = true;
            // 
            // ControlPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.controlTab);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ControlPanel";
            this.Text = "ControlPanel";
            ((System.ComponentModel.ISupportInitialize)(this.timeBox)).EndInit();
            this.controlTab.ResumeLayout(false);
            this.startNow.ResumeLayout(false);
            this.startNow.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button startButton;
        private System.Windows.Forms.NumericUpDown timeBox;
        private System.Windows.Forms.TabControl controlTab;
        private System.Windows.Forms.TabPage startNow;
        private System.Windows.Forms.TabPage processKiller;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox unlockPasswordBox;
    }
}