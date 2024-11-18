namespace StockTrackerApp
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            
            
            this.ClientSize = new System.Drawing.Size(1000, 700);
            this.Name = "MainForm";
            this.ResumeLayout(false);
        }
    }
}
