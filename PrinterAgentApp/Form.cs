using System;
using System.Windows.Forms;

namespace PrinterAgentApp
{
    public partial class Form : System.Windows.Forms.Form
    {

        private const string AppName = "Printer agent";

        private string status;

        public string Status
        {
            get => status;
            set
            {
                this.status = value;
                this.notifyIcon.ShowBalloonTip(4000, AppName, value, ToolTipIcon.Info);
                var text = AppName + (value != null ? Environment.NewLine + value : string.Empty);
                text = text.Length>60 ? text.Substring(0, 60) : text;
                this.notifyIcon.Text = text;
            }
        }

        public Form()
        {
            InitializeComponent();            
            this.notifyIcon.Text = AppName;
            this.Exit.Click += Program.CloseApp;            
        }
        
    }
}
