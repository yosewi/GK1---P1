using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GK.enums
{
    // w którą stronę idzie algorytm sprawdzający ograniczenia
    public enum CheckingDirection
    {
        Forward, // po kolejnych secondEdge
        Backward, // po kolejnych firstEdge
    }
}
