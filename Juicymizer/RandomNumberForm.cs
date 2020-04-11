using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Juicymizer
{
    public partial class RandomNumberForm : UBTStandardLibrary.Forms.N1Form
    {
        public RandomNumberForm()
        {
            InitializeComponent();
        }
        private readonly Random _rand = new Random();
        public new void Show(int fontSize, int min, int max)
        {
            lblNumber.Font = new Font(FontFamily.GenericSansSerif, fontSize);
            lblNumber.Text = _rand.Next(min, max)+"";
            Size s = TextRenderer.MeasureText(lblNumber.Text, lblNumber.Font);
            this.Size = new Size(s.Width+10, s.Height+40);
            base.Show();
        }
    }
}
