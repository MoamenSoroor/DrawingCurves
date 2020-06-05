using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DrawingCurves
{
    interface IPlotter
    {
        void Init();
        void Calculate();
        void Plot();

        void OnMouseWheel(object sender, MouseWheelEventArgs e);
        void OnMouseEnter(object sender, MouseEventArgs e);
        void OnMouseLeave(object sender, MouseEventArgs e);
        void OnMouseMove(object sender, MouseEventArgs e);
    }
}
