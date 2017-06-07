using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Forms;

namespace PrinterAgent
{
    public partial class Form1 : Form
    {

        private const string AppName = "Printer agent";

        private string status;

        public string Status
        {
            get { return status; }
            set
            {
                this.status = value;
                this.notifyIcon1.Text = AppName + (value != null ? Environment.NewLine + value : string.Empty);
            }
        }

        public Form1()
        {
            InitializeComponent();
            this.Load += Form1_Load;
        }

        public void Form1_Load(object sender, EventArgs e)
        {
            //this.notifyIcon1.Visible = true;
            this.notifyIcon1.Text = AppName;
            this.notifyIcon1.ContextMenu = new ContextMenu(new MenuItem[] {new MenuItem("Exit", Program.CloseApp)});
        }

        private void Form1_Load_1(object sender, EventArgs e)
        {

        }
    }
}
