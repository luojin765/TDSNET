using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TDSNET.UI
{
    internal class MsgRecev : IMessageFilter
    {
        Action act;
        public MsgRecev(Action act)
        {
            this.act = act;
        }

        public bool PreFilterMessage(ref Message m)
        {
            if (m.Msg == 0x010)
            {
                act.Invoke();
                return true;
            }
            return false;
        }
    }

}
