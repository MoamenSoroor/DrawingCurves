using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

using static System.Environment;

using static System.Math;

namespace DrawingCurves
{
    public class CanvasPlotter : INotifyPropertyChanged
    {
        private Canvas canvas;
        private Ellipse mousePoint;
        private Polyline curve;
        private PointCollection points;
        private Line xAxis;
        private Line yAxis;

       
        private ZoomMode zoomMode;

        private double originX;
        private double originY;

        private double oX;
        private double oY;

        public double X1 { get; set; }
        public double Y1{ get; set;}
        public double X2{ get; set;}
        public double Y2{ get; set;}

        public double CX1 { get => XToCanvas(X1); }
        public double CY1{ get=> YToCanvas(Y1);}
        public double CX2{ get=> XToCanvas(X2);}
        public double CY2{ get=> YToCanvas(Y1);}

        private double density = 100;
        //public double CDX { get => Width / density; }
        public double DX { get => DeltaX / density; }

        public double ScaleX { get => DeltaX / Width; }
        public double ScaleY { get => DeltaY / Height; }
        public double DeltaX { get => CalculateDelta(X1, X2); }
        public double DeltaY { get => CalculateDelta(Y1, Y2); }
        public double Width { get => this.canvas.ActualWidth; }
        public double Height { get => this.canvas.ActualHeight; }
        public ZoomMode ZoomMode { get => zoomMode; set => zoomMode = value; }
        public double OriginX
        {
            get => originX; set
            {
                originX = value;
                
            }
        }
        public double OriginY
        {
            get => originY; set
            {
                originY = value;
                
            }
        }

        public PointCollection Points
        {
            get => points; set
            {
                if (points != value)
                {
                    points = value;
                    NotifyPropertyChanged(nameof(Points));
                }

            }
        }

        public double OX { get => oX; set => oX = value; }
        public double OY { get => oY; set => oY = value; }

        private ObservableCollection<string> infoList;

        public event Func<double, double> Equation = (X) => Sin(X);
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string info)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }

        // properties



        public CanvasPlotter(Canvas canvas)
        {
            this.canvas = canvas;

        }

        public void SetPlotInfo(ObservableCollection<string> infoList)
        {
            this.infoList = infoList;
        }

        public void Canvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {

            Zoom(e.Delta < 0);
            Plot();
        }

        public void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            Point mouse = e.GetPosition(canvas);
            Point p1 = this.Solve(XToMath(mouse.X));

            Point cpoint = ToCanvasPoint(p1);

            if (!IsCanvasOut(cpoint))
            {
                Canvas.SetLeft(mousePoint, cpoint.X - mousePoint.ActualWidth / 2);
                Canvas.SetTop(mousePoint, cpoint.Y - mousePoint.ActualHeight / 2);
            }


        }

        public void Canvas_MouseEnter(object sender, MouseEventArgs e)
        {

            canvas.Cursor = Cursors.None;
        }

        public void Canvas_MouseLeave(object sender, MouseEventArgs e)
        {
            canvas.Cursor = Cursors.Arrow;

        }

        public void InitCanvas()
        {

            OriginX = Width / 2;
            OriginY = Height / 2;


            ZoomMode = ZoomMode.All;

            //canvas.MouseMove += Canvas_MouseMove;
            //canvas.MouseWheel += Canvas_MouseWheel;
            //canvas.MouseEnter += Canvas_MouseEnter;
            //canvas.MouseLeave += Canvas_MouseLeave;

            InitMousePoint();
            InitAxis();
            InitCurve();

            

        }

        private void InitCurve()
        {
            curve = new Polyline();
            curve.StrokeThickness = 5;
            //curve.StrokeLineJoin = PenLineJoin.Round;
            curve.Stroke = Brushes.SkyBlue;
            //curve.Fill = Brushes.Black;
            Binding binding = new Binding("Points");
            //binding.Source = this;
            curve.SetBinding(Polyline.PointsProperty, binding);
            this.canvas.Children.Add(curve);

        }

        private void InitAxis()
        {
            xAxis = new Line();
            xAxis.X1 = 0;
            xAxis.X2 = Width;
            xAxis.Y1 = OriginY;
            xAxis.Y2 = OriginY;
            xAxis.StrokeThickness = 3;
            xAxis.Stroke = Brushes.Gray;
            this.canvas.Children.Add(xAxis);


            yAxis = new Line();
            yAxis.X1 = OriginX;
            yAxis.X2 = OriginX;
            yAxis.Y1 = 0;
            yAxis.Y2 = Height;
            yAxis.StrokeThickness = 3;
            yAxis.Stroke = Brushes.Gray;
            this.canvas.Children.Add(yAxis);
        }

        private void InitMousePoint()
        {
            mousePoint = new Ellipse();
            mousePoint.Width = 15;
            mousePoint.Height = 15;
            mousePoint.Fill = Brushes.Red;
            this.canvas.Children.Add(mousePoint);
        }

        public void Plot()
        {
            
            PlotAxis();
            PlotCurve();
            RefreshInfoList();
        }

        public void PlotCurve()
        {
            //for (double x = x1,index=0; x <= x2 ; x += DX)

            Points = new PointCollection();
            for (double x = X1; x <= X2; x += DX)
            {
                Point point = Solve(x);
                if (IsValidPoint(point))
                    Points.Add(ToCanvasPoint(point));
            }

        }

        public void PlotAxis()
        {
            xAxis.X1 = 0;
            xAxis.X2 = Width;
            xAxis.Y1 = OriginY;
            xAxis.Y2 = OriginY;

            yAxis.X1 = OriginX;
            yAxis.X2 = OriginX;
            yAxis.Y1 = 0;
            yAxis.Y2 = Height;

        }

        public Point Solve(double x)
        {
            double y = Equation(x);
            Point point = new Point(x, y);
            return point;
        }

        public Point ToCanvasPoint(Point point)
        {
            double cx = point.X / ScaleX + OriginX;
            double cy = -point.Y / ScaleY + OriginY;
            return new Point(cx, cy);
        }

        public Point ToMathPoint(Point point)
        {
            double cx = (point.X - OriginX) * ScaleX;
            double cy = (-point.Y + OriginY) * ScaleY;
            return new Point(cx, cy);
        }

        public double XToCanvas(double x)
        {
            double cx = x / ScaleX + OriginX;
            return cx;
        }

        public double XToMath(double cx)
        {
            double x = (cx - OriginX) * ScaleX;
            return x;

        }

        public double YToCanvas(double y)
        {

            double cy = y / ScaleY + OriginY;
            return cy;
        }

        public double YToMath(double cy)
        {
            double y = (-cy + OriginY) * ScaleY;
            return y;
        }

        public bool IsCanvasOut(Point point)
        {
            return point.X < 0 || point.X > Width
                || point.Y < 0 || point.Y > Height;
        }

        public bool IsValidPoint(Point point)
        {
            return !Double.IsNaN(point.Y) && !Double.IsInfinity(point.Y);
        }

        public void Zoom(bool zoomIn)
        {
            int sign = zoomIn ? 1 : -1;
            double factorX = sign * DeltaX / 4.0;
            double factorY = sign * DeltaY / 4.0;
            switch (ZoomMode)
            {
                case ZoomMode.All:
                    X1 -= factorX;
                    X2 += factorX;
                    Y1 -= factorY;
                    Y2 += factorY;
                    break;
                case ZoomMode.Horizontal:
                    X1 -= factorX;
                    X2 += factorX;
                    break;
                case ZoomMode.Vertical:
                    Y1 -= factorY;
                    Y2 += factorY;
                    break;
                default:
                    break;
            }
        }


        public override string ToString()
        {
            //return base.ToString();
            return
            "X1      :" + X1        + NewLine +
            "Y1      :" + Y1        + NewLine +
            "X2      :" + X2        + NewLine +
            "Y1      :" + Y1        + NewLine +
            "DeltaX  :" + DeltaX        + NewLine +
            "DeltaY  :" + DeltaY        + NewLine +
            "scaleX  :" + ScaleX        + NewLine +
            "scaleY  :" + ScaleY        + NewLine +
            "originX :" + originX       + NewLine +
            "originY :" + originY       + NewLine +
            "CX1     :" + CX1       + NewLine +
            "CY1     :" + CY1       + NewLine +
            "CX2     :" + CX2       + NewLine +
            "CY2     :" + CY2       + NewLine +
            "DX      :" + DX        + NewLine;
        }

        public void RefreshInfoList()
        {
            infoList.Clear();
            infoList.Add("Width   :" + Width);
            infoList.Add("Height  :" + Height);
            infoList.Add("X1      :" + X1);
            infoList.Add("Y1      :" + Y1);
            infoList.Add("X2      :" + X2);
            infoList.Add("Y1      :" + Y1);
            infoList.Add("DeltaX  :" + DeltaX);
            infoList.Add("DeltaY  :" + DeltaY);
            infoList.Add("scaleX  :" + ScaleX);
            infoList.Add("scaleY  :" + ScaleY);
            infoList.Add("originX :" + originX);
            infoList.Add("originY :" + originY);
            infoList.Add("CX1     :" + CX1);
            infoList.Add("CY1     :" + CY1);
            infoList.Add("CX2     :" + CX2);
            infoList.Add("CY2     :" + CY2);
            infoList.Add("DX      :" + DX);

        }

        public string [] PlotInfo()
        {
            return ToString().Split(new string[] { NewLine }, StringSplitOptions.RemoveEmptyEntries);
        }


        public double CalculateDelta(double n1, double n2)
        {
            if ((n1 > 0 && n2 < 0) || (n1 < 0 && n2 > 0))
                return Abs(n1) + Abs(n2);
            else if (n1 >= 0 && n2 >= 0)
                return Abs(n1 - n2);
            else
                return Abs(n1 - n2);
        }
    }
}
