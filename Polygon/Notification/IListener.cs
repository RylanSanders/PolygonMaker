using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolygonMaker.Notification
{
    public interface IListener
    {
        public void Notify(EventBase eventBase);
    }
}
