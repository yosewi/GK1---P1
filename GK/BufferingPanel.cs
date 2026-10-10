using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GK
{
    internal class BufferingPanel : Panel
    {
        public BufferingPanel()
        {
            DoubleBuffered = true;
        }
    }
}
