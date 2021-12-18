
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
            this.ModuleBaseBtn = new System.Windows.Forms.Button();
            this.procName = new System.Windows.Forms.TextBox();
            this.selectProcBtn = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.readAddrTb = new System.Windows.Forms.TextBox();
            this.readBtn = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.writeAddrTb = new System.Windows.Forms.TextBox();
            this.writeBtn = new System.Windows.Forms.Button();
            this.writeValTb = new System.Windows.Forms.TextBox();
            this.lbl = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // ModuleBaseBtn
            // 
            this.ModuleBaseBtn.Location = new System.Drawing.Point(106, 51);
            this.ModuleBaseBtn.Name = "ModuleBaseBtn";
            this.ModuleBaseBtn.Size = new System.Drawing.Size(117, 23);
            this.ModuleBaseBtn.TabIndex = 0;
            this.ModuleBaseBtn.Text = "Get Base Address";
            this.ModuleBaseBtn.UseVisualStyleBackColor = true;
            this.ModuleBaseBtn.Click += new System.EventHandler(this.ModuleBaseBtn_Click);
            // 
            // procName
            // 
            this.procName.Location = new System.Drawing.Point(89, 13);
            this.procName.Name = "procName";
            this.procName.Size = new System.Drawing.Size(174, 20);
            this.procName.TabIndex = 1;
            // 
            // selectProcBtn
            // 
            this.selectProcBtn.Location = new System.Drawing.Point(269, 12);
            this.selectProcBtn.Name = "selectProcBtn";
            this.selectProcBtn.Size = new System.Drawing.Size(71, 23);
            this.selectProcBtn.TabIndex = 0;
            this.selectProcBtn.Text = "Select";
            this.selectProcBtn.UseVisualStyleBackColor = true;
            this.selectProcBtn.Click += new System.EventHandler(this.selectProcBtn_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(79, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Process Name:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 142);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(48, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Address:";
            // 
            // readAddrTb
            // 
            this.readAddrTb.Location = new System.Drawing.Point(66, 139);
            this.readAddrTb.Name = "readAddrTb";
            this.readAddrTb.Size = new System.Drawing.Size(174, 20);
            this.readAddrTb.TabIndex = 1;
            // 
            // readBtn
            // 
            this.readBtn.Location = new System.Drawing.Point(106, 165);
            this.readBtn.Name = "readBtn";
            this.readBtn.Size = new System.Drawing.Size(93, 23);
            this.readBtn.TabIndex = 0;
            this.readBtn.Text = "Read INT64";
            this.readBtn.UseVisualStyleBackColor = true;
            this.readBtn.Click += new System.EventHandler(this.readBtn_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 255);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(48, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Address:";
            // 
            // writeAddrTb
            // 
            this.writeAddrTb.Location = new System.Drawing.Point(66, 252);
            this.writeAddrTb.Name = "writeAddrTb";
            this.writeAddrTb.Size = new System.Drawing.Size(174, 20);
            this.writeAddrTb.TabIndex = 5;
            // 
            // writeBtn
            // 
            this.writeBtn.Location = new System.Drawing.Point(106, 306);
            this.writeBtn.Name = "writeBtn";
            this.writeBtn.Size = new System.Drawing.Size(93, 23);
            this.writeBtn.TabIndex = 4;
            this.writeBtn.Text = "Write INT64";
            this.writeBtn.UseVisualStyleBackColor = true;
            this.writeBtn.Click += new System.EventHandler(this.writeBtn_Click);
            // 
            // writeValTb
            // 
            this.writeValTb.Location = new System.Drawing.Point(66, 278);
            this.writeValTb.Name = "writeValTb";
            this.writeValTb.Size = new System.Drawing.Size(174, 20);
            this.writeValTb.TabIndex = 5;
            // 
            // lbl
            // 
            this.lbl.AutoSize = true;
            this.lbl.Location = new System.Drawing.Point(12, 281);
            this.lbl.Name = "lbl";
            this.lbl.Size = new System.Drawing.Size(37, 13);
            this.lbl.TabIndex = 6;
            this.lbl.Text = "Value:";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(352, 440);
            this.Controls.Add(this.lbl);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.writeValTb);
            this.Controls.Add(this.writeAddrTb);
            this.Controls.Add(this.writeBtn);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.readAddrTb);
            this.Controls.Add(this.procName);
            this.Controls.Add(this.selectProcBtn);
            this.Controls.Add(this.readBtn);
            this.Controls.Add(this.ModuleBaseBtn);
            this.Name = "MainForm";
            this.Text = "Control";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button ModuleBaseBtn;
        private System.Windows.Forms.TextBox procName;
        private System.Windows.Forms.Button selectProcBtn;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox readAddrTb;
        private System.Windows.Forms.Button readBtn;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox writeAddrTb;
        private System.Windows.Forms.Button writeBtn;
        private System.Windows.Forms.TextBox writeValTb;
        private System.Windows.Forms.Label lbl;
    }
}

