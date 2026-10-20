using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GK
{
    internal class LengthDialog : Form
    {
        private readonly NumericUpDown input;

        public double Value => (double)input.Value;

        public LengthDialog(double initialLength)
        {
            Text = "Długość krawędzi";
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            MinimizeBox = false;
            MaximizeBox = false;
            ClientSize = new Size(220, 90);

            input = new NumericUpDown
            {
                Minimum = 1,
                Maximum = 10000,
                DecimalPlaces = 2,
                Location = new Point(12, 12),
                Width = 196
            };

            input.Value = (decimal)Math.Clamp(Math.Round(initialLength, 2), 1, 10000);

            Button ok = new Button { Text = "OK", DialogResult = DialogResult.OK, Location = new Point(52, 50) };
            Button cancel = new Button { Text = "Anuluj", DialogResult = DialogResult.Cancel, Location = new Point(133, 50) };

            Controls.Add(input);
            Controls.Add(ok);
            Controls.Add(cancel);

            AcceptButton = ok;
            CancelButton = cancel;
        }
    }
}