namespace SS
{
    partial class FindWindow
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
            this.searchBox = new System.Windows.Forms.TextBox();
            this.findButton = new System.Windows.Forms.Button();
            this.formulaSearch = new System.Windows.Forms.CheckBox();
            this.ignoreCase = new System.Windows.Forms.CheckBox();
            this.resultText = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // searchBox
            // 
            this.searchBox.Location = new System.Drawing.Point(24, 25);
            this.searchBox.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.searchBox.Name = "searchBox";
            this.searchBox.Size = new System.Drawing.Size(408, 31);
            this.searchBox.TabIndex = 0;
            // 
            // findButton
            // 
            this.findButton.Location = new System.Drawing.Point(448, 23);
            this.findButton.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.findButton.Name = "findButton";
            this.findButton.Size = new System.Drawing.Size(212, 40);
            this.findButton.TabIndex = 1;
            this.findButton.Text = "Find";
            this.findButton.UseVisualStyleBackColor = true;
            this.findButton.Click += new System.EventHandler(this.findButton_Click);
            // 
            // formulaSearch
            // 
            this.formulaSearch.AutoSize = true;
            this.formulaSearch.Location = new System.Drawing.Point(24, 75);
            this.formulaSearch.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.formulaSearch.Name = "formulaSearch";
            this.formulaSearch.Size = new System.Drawing.Size(207, 29);
            this.formulaSearch.TabIndex = 2;
            this.formulaSearch.Text = "Search Formulas";
            this.formulaSearch.UseVisualStyleBackColor = true;
            // 
            // ignoreCase
            // 
            this.ignoreCase.AutoSize = true;
            this.ignoreCase.Location = new System.Drawing.Point(246, 75);
            this.ignoreCase.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.ignoreCase.Name = "ignoreCase";
            this.ignoreCase.Size = new System.Drawing.Size(160, 29);
            this.ignoreCase.TabIndex = 4;
            this.ignoreCase.Text = "Ignore Case";
            this.ignoreCase.UseVisualStyleBackColor = true;
            // 
            // resultText
            // 
            this.resultText.Location = new System.Drawing.Point(448, 75);
            this.resultText.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.resultText.Name = "resultText";
            this.resultText.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.resultText.Size = new System.Drawing.Size(212, 33);
            this.resultText.TabIndex = 5;
            this.resultText.Text = "No results.";
            this.resultText.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // FindWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(684, 113);
            this.Controls.Add(this.resultText);
            this.Controls.Add(this.ignoreCase);
            this.Controls.Add(this.formulaSearch);
            this.Controls.Add(this.findButton);
            this.Controls.Add(this.searchBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.Name = "FindWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Find...";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.stopHighlighting);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox searchBox;
        private System.Windows.Forms.Button findButton;
        private System.Windows.Forms.CheckBox formulaSearch;
        private System.Windows.Forms.CheckBox ignoreCase;
        private System.Windows.Forms.Label resultText;
    }
}