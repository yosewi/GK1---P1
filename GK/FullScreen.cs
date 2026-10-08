using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace GK
{
    internal class FullScreen
    {
        Form TargetForm;

        FormWindowState PreviousWindowState;

        public FullScreen(Form targetForm)
        {
            TargetForm = targetForm;
            TargetForm.KeyPreview = true;
            TargetForm.KeyDown += TargetForm_KeyDown;
        }

        private void TargetForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.F11)
            {
                Toggle();
            }
        }

        public void Toggle()
        {
            if (TargetForm.WindowState == FormWindowState.Maximized)
            {
                Leave();
            }
            else
            {
                Enter();
            }
        }

        private void Leave()
        {
            TargetForm.FormBorderStyle = FormBorderStyle.Sizable;
            TargetForm.WindowState = PreviousWindowState;
        }

        private void Enter()
        {
            PreviousWindowState = TargetForm.WindowState;
            TargetForm.FormBorderStyle = FormBorderStyle.None;
            TargetForm.WindowState = FormWindowState.Maximized;

        }
    }
}
