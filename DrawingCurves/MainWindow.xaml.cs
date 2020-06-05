using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using  static System.Math;
using static System.Environment;
using System.Collections.ObjectModel;

namespace DrawingCurves
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
            Plotter = new CanvasPlotter(this.myCanvas);
            this.myCanvas.DataContext = Plotter;
            //PlotCurve();
            //PlotCurve2();
        }

        private CanvasPlotter plotter;

        public CanvasPlotter Plotter { get => plotter; set => plotter = value; }

        private ObservableCollection<string> infoList = new ObservableCollection<string>();

        public ObservableCollection<string> InfoList
        {
            get => infoList; set =>  infoList = value;
        }



        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.myCanvas.MouseMove += Canvas_MouseMove;
            this.myCanvas.MouseWheel += Canvas_MouseWheel;
            this.myCanvas.MouseEnter += Canvas_MouseEnter;
            this.myCanvas.MouseLeave += Canvas_MouseLeave;

            Binding binding = new Binding("InfoList");
            binding.Source = plotter;
            plotInfoList.SetBinding(ListBox.ItemsSourceProperty, binding);
            plotInfoList.ItemsSource = InfoList;
            plotter.SetPlotInfo(InfoList);

            Plotter.InitCanvas();

            Plotter.Equation += (x) => Sin(x);
            Plotter.X1 = -2 * PI;
            Plotter.X2 = 2 * PI;
            Plotter.Y1 = -2;
            Plotter.Y2 = 2;

            Plotter.Plot();





        }
        

        private void Canvas_MouseEnter(object sender, MouseEventArgs e)
        {
            Plotter.Canvas_MouseEnter(sender, e);
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            Plotter.Canvas_MouseMove(sender, e);

            double cx = e.GetPosition(this.myCanvas).X;

            Point point = Plotter.Solve(Plotter.XToMath(cx));
            SetXResult(point.X);
            SetYResult(point.Y);
        }

        private void Canvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            Plotter.Canvas_MouseWheel(sender, e);
        }

        private void Canvas_MouseLeave(object sender, MouseEventArgs e)
        {
            Plotter.Canvas_MouseLeave(sender, e);
        }

        private void ZoomMode_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as ToggleButton;
            if (btn == null) return;
            switch (btn)
            {
                case ToggleButton btn1 when btn1 == xAxisButton:
                    Plotter.ZoomMode = ZoomMode.Horizontal;
                    break;
                case ToggleButton btn1 when btn1 == yAxisButton:
                    Plotter.ZoomMode = ZoomMode.Vertical;
                    break;
                case ToggleButton btn1 when btn1 == xyAxisButton:
                    Plotter.ZoomMode = ZoomMode.All;
                    break;
                default:
                    break;
            }

        }

        private void UpdateZooming(object sender, RoutedEventArgs e)
        {
            ZoomMode mode;
            

        }
        private void plotButton_Click(object sender, RoutedEventArgs e)
        {
            //PlotCurve2();
        }


        public void SetXResult(double x)
        {
            this.resultXTextBox.Text = x.ToString();
        }
        public void SetYResult(double y)
        {
            this.resultYTextBox.Text = y.ToString();
        }

        private void resetButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void zoomInButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ZoomOutButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }

    public enum ZoomMode
    {
        All, Horizontal, Vertical
    }
}
