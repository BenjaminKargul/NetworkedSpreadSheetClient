namespace SS
{
    partial class GraphForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxRowStart = new System.Windows.Forms.TextBox();
            this.textBoxRowEnd = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.buttonGraph = new System.Windows.Forms.Button();
            this.textBoxXCol = new System.Windows.Forms.TextBox();
            this.textBoxYCol = new System.Windows.Forms.TextBox();
            this.textBoxXLabel = new System.Windows.Forms.TextBox();
            this.textBoxYLabel = new System.Windows.Forms.TextBox();
            this.textBoxFileName = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(34, 57);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(222, 25);
            this.label1.TabIndex = 2;
            this.label1.Text = "X values from Column";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(342, 57);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(223, 25);
            this.label2.TabIndex = 3;
            this.label2.Text = "Y values from Column";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(651, 57);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(109, 25);
            this.label3.TabIndex = 4;
            this.label3.Text = "Use Rows";
            // 
            // textBoxRowStart
            // 
            this.textBoxRowStart.Location = new System.Drawing.Point(768, 54);
            this.textBoxRowStart.Name = "textBoxRowStart";
            this.textBoxRowStart.Size = new System.Drawing.Size(100, 31);
            this.textBoxRowStart.TabIndex = 5;
            // 
            // textBoxRowEnd
            // 
            this.textBoxRowEnd.Location = new System.Drawing.Point(969, 54);
            this.textBoxRowEnd.Name = "textBoxRowEnd";
            this.textBoxRowEnd.Size = new System.Drawing.Size(100, 31);
            this.textBoxRowEnd.TabIndex = 6;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(876, 57);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(85, 25);
            this.label4.TabIndex = 7;
            this.label4.Text = "through";
            // 
            // buttonGraph
            // 
            this.buttonGraph.Location = new System.Drawing.Point(455, 214);
            this.buttonGraph.Name = "buttonGraph";
            this.buttonGraph.Size = new System.Drawing.Size(223, 46);
            this.buttonGraph.TabIndex = 9;
            this.buttonGraph.Text = "Graph Data";
            this.buttonGraph.UseVisualStyleBackColor = true;
            this.buttonGraph.Click += new System.EventHandler(this.buttonGraph_Click);
            // 
            // textBoxXCol
            // 
            this.textBoxXCol.Location = new System.Drawing.Point(264, 54);
            this.textBoxXCol.Name = "textBoxXCol";
            this.textBoxXCol.Size = new System.Drawing.Size(70, 31);
            this.textBoxXCol.TabIndex = 10;
            // 
            // textBoxYCol
            // 
            this.textBoxYCol.Location = new System.Drawing.Point(573, 54);
            this.textBoxYCol.Name = "textBoxYCol";
            this.textBoxYCol.Size = new System.Drawing.Size(70, 31);
            this.textBoxYCol.TabIndex = 11;
            // 
            // textBoxXLabel
            // 
            this.textBoxXLabel.Location = new System.Drawing.Point(346, 99);
            this.textBoxXLabel.Name = "textBoxXLabel";
            this.textBoxXLabel.Size = new System.Drawing.Size(216, 31);
            this.textBoxXLabel.TabIndex = 12;
            // 
            // textBoxYLabel
            // 
            this.textBoxYLabel.Location = new System.Drawing.Point(704, 99);
            this.textBoxYLabel.Name = "textBoxYLabel";
            this.textBoxYLabel.Size = new System.Drawing.Size(224, 31);
            this.textBoxYLabel.TabIndex = 13;
            // 
            // textBoxFileName
            // 
            this.textBoxFileName.Location = new System.Drawing.Point(573, 151);
            this.textBoxFileName.Name = "textBoxFileName";
            this.textBoxFileName.Size = new System.Drawing.Size(272, 31);
            this.textBoxFileName.TabIndex = 14;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(211, 102);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(129, 25);
            this.label5.TabIndex = 15;
            this.label5.Text = "X axis label:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(568, 102);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(130, 25);
            this.label6.TabIndex = 16;
            this.label6.Text = "Y axis label:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(392, 151);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(170, 25);
            this.label7.TabIndex = 17;
            this.label7.Text = "Graph file name:";
            // 
            // GraphForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1124, 299);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.textBoxFileName);
            this.Controls.Add(this.textBoxYLabel);
            this.Controls.Add(this.textBoxXLabel);
            this.Controls.Add(this.textBoxYCol);
            this.Controls.Add(this.textBoxXCol);
            this.Controls.Add(this.buttonGraph);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.textBoxRowEnd);
            this.Controls.Add(this.textBoxRowStart);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(1150, 1000);
            this.Name = "GraphForm";
            this.Text = "Graph your Spreadsheet";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxRowStart;
        private System.Windows.Forms.TextBox textBoxRowEnd;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button buttonGraph;
        private System.Windows.Forms.TextBox textBoxXCol;
        private System.Windows.Forms.TextBox textBoxYCol;
        private System.Windows.Forms.TextBox textBoxXLabel;
        private System.Windows.Forms.TextBox textBoxYLabel;
        private System.Windows.Forms.TextBox textBoxFileName;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
    }
}