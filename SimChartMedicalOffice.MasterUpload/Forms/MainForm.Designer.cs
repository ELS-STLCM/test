namespace SimChartMedicalOffice.MasterUpload
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
            this.btnCompetency = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnCompetency
            // 
            this.btnCompetency.Location = new System.Drawing.Point(12, 21);
            this.btnCompetency.Name = "btnCompetency";
            this.btnCompetency.Size = new System.Drawing.Size(95, 23);
            this.btnCompetency.TabIndex = 0;
            this.btnCompetency.Text = "Competency";
            this.btnCompetency.UseVisualStyleBackColor = true;
            this.btnCompetency.Click += new System.EventHandler(this.BtnCompetencyClick);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.btnCompetency);
            this.Name = "MainForm";
            this.Text = "MainForm";
            this.Load += new System.EventHandler(this.MainFormLoad);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnCompetency;
    }
}