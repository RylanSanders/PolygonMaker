using PolygonMaker.Render;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolygonMaker.Notification
{
    class PolygonSelectedEvent: EventBase
    {
        public PolygonRender SelectedPolygon;
    }
}
