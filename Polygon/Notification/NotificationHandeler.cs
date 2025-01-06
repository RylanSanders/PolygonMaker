using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolygonMaker.Notification
{
    public static class NotificationHandler
    {
        public static List<IListener> RegisteredListeners = new List<IListener>();

        public static void RegisterListener(IListener listener)
        {
            RegisteredListeners.Add(listener);
        }

        public static void EmitEvent(EventBase eventBase)
        {
            RegisteredListeners.ForEach(listener => listener.Notify(eventBase));
        }
    }
}
