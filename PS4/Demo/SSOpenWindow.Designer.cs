﻿namespace SS
{
    partial class SSOpenWindow
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
            this.listBoxSpreadsheets = new System.Windows.Forms.ListBox();
            this.selectionBox = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // listBoxSpreadsheets
            // 
            this.listBoxSpreadsheets.FormattingEnabled = true;
            this.listBoxSpreadsheets.Location = new System.Drawing.Point(13, 13);
            this.listBoxSpreadsheets.Name = "listBoxSpreadsheets";
            this.listBoxSpreadsheets.Size = new System.Drawing.Size(259, 186);
            this.listBoxSpreadsheets.TabIndex = 0;
            this.listBoxSpreadsheets.SelectedIndexChanged += new System.EventHandler(this.listBoxSpreadsheets_SelectedIndexChanged);
            // 
            // selectionBox
            // 
            this.selectionBox.Location = new System.Drawing.Point(13, 214);
            this.selectionBox.Name = "selectionBox";
            this.selectionBox.Size = new System.Drawing.Size(167, 20);
            this.selectionBox.TabIndex = 1;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(186, 214);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(86, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "Open File";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // SSOpenWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.selectionBox);
            this.Controls.Add(this.listBoxSpreadsheets);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(300, 300);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(300, 300);
            this.Name = "SSOpenWindow";
            this.Text = "Open a Spreadsheet";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SSOpenWindow_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox listBoxSpreadsheets;
        private System.Windows.Forms.TextBox selectionBox;
        private System.Windows.Forms.Button button1;
    }
}