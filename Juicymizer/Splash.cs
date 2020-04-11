using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Juicymizer
{
    public partial class Splash : Form
    {
        public Splash()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
            Task.Run(() =>
            {
                Thread.Sleep(4000);
                Thread thread = new Thread(() => { Application.Run(new MainWindow()); });
                thread.SetApartmentState(ApartmentState.STA);
                thread.Start();
                this.Close();
            });
        }
    }
}
