using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using GlobalLowLevelHooks;
using gma.System.Windows;
using Juicymizer.Properties;
using Newtonsoft.Json;
using KeyEventArgs = System.Windows.Forms.KeyEventArgs;
using MouseEventArgs = System.Windows.Forms.MouseEventArgs;

namespace Juicymizer
{
    public partial class MainWindow : Form
    {
        private Setting CurrentSetting;
        private TrayService service;
        public MainWindow()
        {
            InitializeComponent();
            service = new TrayService(this);
            service.Start();
            nudMinimum.DataBindings.Add("Maximum", nudMiximum, "Value");
            //MessageBox.Show(Settings.Default.JsonSetting);
            //Settings.Default.JsonSetting = "";
            if (!Settings.Default.JsonSetting.Trim().Equals(""))
            { 
                CurrentSetting = JsonConvert.DeserializeObject<Setting>(Settings.Default.JsonSetting);
                txtKey.Text = GetKeyName(CurrentSetting.KeyData);
                nudMinimum.Value = CurrentSetting.Minimum;
                nudMiximum.Value = CurrentSetting.Maximum;
                nudSize.Value = CurrentSetting.FontSize;
                chkEnableLeftClick.Checked = CurrentSetting.LeftClick;
                chkEnableRightClick.Checked = CurrentSetting.RightClick;
                chkEnableHotKey.Checked = CurrentSetting.HotKey;
                chkXB1.Checked = CurrentSetting.XButton1;
                chkXB2.Checked = CurrentSetting.XButton2;
            }
            else
            {
                CurrentSetting = new Setting();
            }
            this.Load += delegate(object sender, EventArgs args) { BringToFront(); };

        }

        private void txtKey_KeyDown(object sender, KeyEventArgs e)
        {
            CurrentKey.Alt = e.Alt;
            CurrentKey.Shift = e.Shift;
            CurrentKey.Control = e.Control;
            CurrentKey.Key = e.KeyCode;
            txtKey.Text = GetKeyName(CurrentKey);
        }

        private KeyData CurrentKey { get; set; } = new KeyData();
        private readonly Keys[] _redundantKeys = new[] { Keys.Control, Keys.Shift, Keys.ShiftKey, Keys.ControlKey, Keys.Alt };

        private string GetKeyName(KeyData a)
        {
            string name = "";

            if (a.Control)
                name += "CTRL + ";
            if (a.Shift)
                name += "SHIFT + ";
            if (a.Alt)
                name += "ALT + ";
            if (!_redundantKeys.Contains(a.Key))
                name += a.Key;

            return name;
        }

        private void txtKey_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            Setting s = new Setting()
            {
                FontSize = (int)nudSize.Value,
                Maximum= (int)nudMiximum.Value,
                Minimum= (int)nudMinimum.Value,
                KeyData = CurrentKey,
                HotKey = chkEnableHotKey.Checked,
                LeftClick = chkEnableLeftClick.Checked,
                RightClick = chkEnableRightClick.Checked,
                XButton1 = chkXB1.Checked,
                XButton2 = chkXB2.Checked,
            };
            CurrentSetting = s;
            Settings.Default.JsonSetting = JsonConvert.SerializeObject(s);
            Settings.Default.Save();
            MessageBox.Show("Setting Saved!", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void AlterEnable(bool enable)
        {
            btnStart.Enabled = enable;
            btnStop.Enabled = !enable;
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            Start();
        }

        public void Start()
        {
            StartService();
            AlterEnable(false);
        }

        public void Stop()
        {
            _uAH.KeyDown -= UAHOnKeyDown;
            _uAH.KeyUp-= UAHOnKeyUp;
            _uAH.OnMouseActivity -= UAHOnOnMouseActivity;
            AlterEnable(true);
        }
        private readonly UserActivityHook _uAH = new UserActivityHook(true, true);
        private void StartService()
        {
            _uAH.KeyDown +=  UAHOnKeyDown;
            _uAH.KeyUp += UAHOnKeyUp;
            _uAH.OnMouseActivity += UAHOnOnMouseActivity;
        }

        private void UAHOnKeyUp(object sender, KeyEventArgs e)
        {
            if (_controlKeys.Contains(e.KeyCode))
                _isControl = false;
            if (_altKeys.Contains(e.KeyCode))
                _isAlt = false;
            if (_shiftKeys.Contains(e.KeyCode))
                _isShift = false;

            System.Diagnostics.Debug.Write($"Control: {_isControl}\nShift: {_isShift}\nAlt: {_isAlt}\n\n");
        }

        private void UAHOnOnMouseActivity(object sender, MouseEventArgs e)
        {
            if ((e.Button == MouseButtons.Right && CurrentSetting.RightClick) ||
                (e.Button == MouseButtons.Left && CurrentSetting.LeftClick)||(e.Button == MouseButtons.XButton1 && CurrentSetting.XButton1) || (e.Button == MouseButtons.XButton2 && CurrentSetting.XButton2))
            {
                GenerateNew();
            }
        }

        private void UAHOnKeyDown(object sender, KeyEventArgs e)
        {
            if (_controlKeys.Contains(e.KeyCode))
                _isControl = true;
            else if (_altKeys.Contains(e.KeyCode))
                _isAlt = true;
            else if (_shiftKeys.Contains(e.KeyCode))
                _isShift = true;
            else
            {
                if ((CurrentSetting.KeyData.Shift = _isShift) && (CurrentSetting.KeyData.Alt = _isAlt) &&
                    (CurrentSetting.KeyData.Control = _isControl) && (e.KeyValue == (int) CurrentSetting.KeyData.Key))
                {
                    //MessageBox.Show("");
                    //System.Diagnostics.Debug.Write($"Control: {CurrentSetting.KeyData.Shift}--{_isControl}\nShift: {CurrentSetting.KeyData.Shift}---{_isShift}\nAlt: {CurrentSetting.KeyData.Shift}--{_isAlt}\n\n");
                    GenerateNew();
                }
            }
        }

        private bool _isControl = false;
        private bool _isAlt = false;
        private bool _isShift = false;

        private readonly Keys[] _controlKeys = new[] {Keys.ControlKey, Keys.Control, Keys.LControlKey, Keys.RControlKey};
        private readonly Keys[] _shiftKeys = new[] {Keys.ShiftKey, Keys.Shift, Keys.LShiftKey, Keys.RShiftKey};
        private readonly Keys[] _altKeys = new[] {Keys.Alt, Keys.Menu, Keys.LMenu, Keys.RMenu};

        private void GenerateNew()
        {
            if (f == null)
            {
                f = new RandomNumberForm { StartPosition = FormStartPosition.Manual };
                f.Closed += FOnClosed;
                f.Show(CurrentSetting.FontSize, CurrentSetting.Minimum, CurrentSetting.Maximum);
                f.Location = GetCursorPosition();
            }
            else
            {
                f.Closed -= FOnClosed;
                f.Dispose();
                f = null;
            }
            
        }

        private void FOnClosed(object sender, EventArgs e)
        {
            f = null;
        }

        private RandomNumberForm f;

        private void btnStop_Click(object sender, EventArgs e)
        {
            Stop();
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;

            public static implicit operator Point(POINT point)
            {
                return new Point(point.X, point.Y);
            }
        }

        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out POINT lpPoint);

        public static Point GetCursorPosition()
        {
            POINT lpPoint;
            GetCursorPos(out lpPoint);

            return lpPoint;
        }
    }

    class Setting
    {
        public KeyData KeyData { get; set; }
        public int Minimum { get; set; }
        public int Maximum { get; set; }
        public int FontSize { get; set; }
        public bool LeftClick { get; set; }
        public bool RightClick { get; set; }
        public bool HotKey { get; set; }
        public bool XButton1 { get; set; }
        public bool XButton2 { get; set; }

    }

    class KeyData
    {
        public bool Control { get; set; }
        public bool Alt { get; set; }
        public bool Shift { get; set; }
        public Keys Key { get; set; }
    }
}
