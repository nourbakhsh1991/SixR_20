using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SixR_20.Models
{
    public enum StatuswordEnum
    {
        ReadyToSwitchOn=1,
        SwitchedOn=2,
        OperationEnabled=4,
        Fault=8,
        VoltageEnabled=16,
        QuickStop=32,
        SwitchOnDisplayed=64,
        Warning=128,
    }
}
