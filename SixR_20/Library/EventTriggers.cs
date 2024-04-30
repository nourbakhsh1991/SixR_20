using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace SixR_20.Library
{
    public class QKeyDownEventTrigger : EventTrigger
    {

        public QKeyDownEventTrigger() : base("KeyDown")
        {
        }

        protected override void OnEvent(EventArgs eventArgs)
        {
            var e = eventArgs as KeyEventArgs;
            if (e != null && e.Key == Key.Q)
                this.InvokeActions(eventArgs);
        }
    }
}
