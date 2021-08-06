
namespace CsGetTgs
{
    partial class Form2
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
            this.button1 = new System.Windows.Forms.Button();
            this.tb_dieX = new System.Windows.Forms.TextBox();
            this.tb_dieY = new System.Windows.Forms.TextBox();
            this.tb_scribe = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(85, 160);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(108, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "ScribeLine Width";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(85, 66);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(66, 15);
            this.label2.TabIndex = 1;
            this.label2.Text = "Die Width";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(85, 98);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(69, 15);
            this.label3.TabIndex = 2;
            this.label3.Text = "Die Height";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(117, 322);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "OK";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // tb_dieX
            // 
            this.tb_dieX.Location = new System.Drawing.Point(234, 55);
            this.tb_dieX.Name = "tb_dieX";
            this.tb_dieX.Size = new System.Drawing.Size(100, 25);
            this.tb_dieX.TabIndex = 4;
            this.tb_dieX.Text = "2220";
            // 
            // tb_dieY
            // 
            this.tb_dieY.Location = new System.Drawing.Point(234, 98);
            this.tb_dieY.Name = "tb_dieY";
            this.tb_dieY.Size = new System.Drawing.Size(100, 25);
            this.tb_dieY.TabIndex = 5;
            this.tb_dieY.Text = "2050";
            // 
            // tb_scribe
            // 
            this.tb_scribe.Location = new System.Drawing.Point(234, 149);
            this.tb_scribe.Name = "tb_scribe";
            this.tb_scribe.Size = new System.Drawing.Size(100, 25);
            this.tb_scribe.TabIndex = 6;
            this.tb_scribe.Text = "130";
            // 
            // Form2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(441, 406);
            this.Controls.Add(this.tb_scribe);
            this.Controls.Add(this.tb_dieY);
            this.Controls.Add(this.tb_dieX);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "Form2";
            this.Text = "Form2";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox tb_dieX;
        private System.Windows.Forms.TextBox tb_dieY;
        private System.Windows.Forms.TextBox tb_scribe;
    }
}