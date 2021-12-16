
namespace UsermodeApp
{
    partial class MainForm
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
            this.CallBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // CallBtn
            // 
            this.CallBtn.Location = new System.Drawing.Point(140, 25);
            this.CallBtn.Name = "CallBtn";
            this.CallBtn.Size = new System.Drawing.Size(75, 23);
            this.CallBtn.TabIndex = 0;
            this.CallBtn.Text = "Call Hook";
            this.CallBtn.UseVisualStyleBackColor = true;
            this.CallBtn.Click += new System.EventHandler(this.CallBtn_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(352, 440);
            this.Controls.Add(this.CallBtn);
            this.Name = "MainForm";
            this.Text = "Control";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button CallBtn;
    }
}

