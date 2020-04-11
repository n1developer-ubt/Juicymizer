using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Juicymizer
{
    class TrayService
    {
        private readonly MainWindow _currentForm;
        private readonly NotifyIcon _icon;
        public TrayService(MainWindow f)
        {
            _currentForm = f;
            _icon = new NotifyIcon();
            _icon.DoubleClick += IconOnDoubleClick;
            ContextMenu m = new ContextMenu();
            MenuItem open = new MenuItem()
            {
                Text = "Open"
            };
            open.Click += OpenOnClick;

            MenuItem start  = new MenuItem()
            {
                Text = "Start"
            };
            start.Click += StartOnClick;

            MenuItem stop = new MenuItem()
            {
                Text = "Stop"
            };
            stop.Click += StopOnClick;
            MenuItem[] items = new[] {open, start, stop};
            
            foreach (MenuItem item in items)
            {
                m.MenuItems.Add(item);
            }

            _icon.ContextMenu = m;
            _icon.Icon = _currentForm.Icon;
        }

        private void StopOnClick(object sender, EventArgs e)
        {
            _currentForm.Stop();
        }

        private void StartOnClick(object sender, EventArgs e)
        {
            _currentForm.Start();
        }

        private void OpenOnClick(object sender, EventArgs e)
        {
            ShowIt();
        }

        private void ShowIt()
        {
            _currentForm.Show();
            _currentForm.WindowState = FormWindowState.Normal;
            _icon.Visible = false;
        }
        private void IconOnDoubleClick(object sender, EventArgs e)
        {
            ShowIt();
        }

        public void Start()
        {
            _currentForm.SizeChanged += CurrentFormOnSizeChanged;
        }

        private void CurrentFormOnSizeChanged(object sender, EventArgs e)
        {
            if(sender is MainWindow x)
            {
                if (x.WindowState == FormWindowState.Minimized)
                {
                    x.Hide();
                    _icon.Visible = true;
                }
            }
        }
    }
}
