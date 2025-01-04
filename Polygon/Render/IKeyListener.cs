using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PolygonMaker.Render
{
    public interface IKeyListener
    {
        public void HandleKeyEvent(KeyEventArgs args);
    }
}
