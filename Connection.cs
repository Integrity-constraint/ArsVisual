﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace ArsVisual
{
    public class Connection : Control, ISelectable, INotifyPropertyChanged
    {
        public static readonly RoutedUICommand SetSourceArrowCommand = new RoutedUICommand("Set Source Arrow", "SetSourceArrow", typeof(Connection));
        public static readonly RoutedUICommand SetSinkArrowCommand = new RoutedUICommand("Set Sink Arrow", "SetSinkArrow", typeof(Connection));
        public static readonly RoutedUICommand SetLineStyleCommand = new RoutedUICommand("Set Line Style","SetLineStyle", typeof(Connection));

        public void SaveStateInitialize()
        {
            var canvas = VisualTreeHelper.GetParent(this) as DesignerCanvas;
            canvas?.SaveUndoState();
        }
        public enum ConnectionLineType
        {
            Straight,   
            Orthogonal,
            Curved 
        }
        private ConnectionLineType _connectionLineType;
        public ConnectionLineType _ConnectionLineType
        {
            get { return _connectionLineType; }
            set
            {
               
                if (_connectionLineType != value)
                {
                    _connectionLineType = value;
                    IsStraightLine = (value == ConnectionLineType.Straight); 
                    OnPropertyChanged(nameof(ConnectionLineType));
                }
            }
        }

        private bool _isStraightLine;
        public bool IsStraightLine
        {
            get => _isStraightLine;
            set
            {
                if (_isStraightLine != value)
                {
                    _isStraightLine = value;
                    UpdatePathGeometry(); 
                    OnPropertyChanged(nameof(IsStraightLine));
                }
            }
        }
        public static readonly DependencyProperty TextProperty =
      DependencyProperty.Register("Text", typeof(string), typeof(Connection), new PropertyMetadata(string.Empty));

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        private void OnSetSourceArrowCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
           SaveStateInitialize();
            if (e.Parameter is string arrowSymbol)
            {
                if (Enum.TryParse(arrowSymbol, out ArrowSymbol symbol))
                {
                    this.SourceArrowSymbol = symbol;
                }
            }
        }
        private void OnSetLineStyleCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
          
            if (e.Parameter is string style)
            {
                switch (style)
                {
                    case "Solid":
                        StrokeDashArray = null;
                        break;
                    case "Dotted":
                        StrokeDashArray = new DoubleCollection(new double[] { 2, 2 });
                        break;
                    case "Dashed":
                        StrokeDashArray = new DoubleCollection(new double[] { 5, 3 });
                        break;
                }
            }

        }

        private void OnSetSinkArrowCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
          
            if (e.Parameter is string arrowSymbol)
            {
                if (Enum.TryParse(arrowSymbol, out ArrowSymbol symbol))
                {
                    this.SinkArrowSymbol = symbol;
                }
            }
        }
        private Adorner connectionAdorner;

        #region Properties

        public Guid ID { get; set; }

        // source connector
        private Connector source;
        public Connector Source
        {
            get
            {
                return source;
            }
            set
            {
                if (source != value)
                {
                    if (source != null)
                    {
                        source.PropertyChanged -= new PropertyChangedEventHandler(OnConnectorPositionChanged);
                        source.Connections.Remove(this);
                    }

                    source = value;

                    if (source != null)
                    {
                        source.Connections.Add(this);
                        source.PropertyChanged += new PropertyChangedEventHandler(OnConnectorPositionChanged);
                    }

                    UpdatePathGeometry();
                }
            }
        }

        // sink connector
        private Connector sink;
        public Connector Sink
        {
            get { return sink; }
            set
            {
                if (sink != value)
                {
                    if (sink != null)
                    {
                        sink.PropertyChanged -= new PropertyChangedEventHandler(OnConnectorPositionChanged);
                        sink.Connections.Remove(this);
                    }

                    sink = value;

                    if (sink != null)
                    {
                        sink.Connections.Add(this);
                        sink.PropertyChanged += new PropertyChangedEventHandler(OnConnectorPositionChanged);
                    }
                    UpdatePathGeometry();
                }
            }
        }

        // connection path geometry
        private PathGeometry pathGeometry;
        public PathGeometry PathGeometry
        {
            get { return pathGeometry; }
            set
            {
                if (pathGeometry != value)
                {
                    pathGeometry = value;
                    UpdateAnchorPosition();
                    OnPropertyChanged("PathGeometry");
                }
            }
        }

      
        private Point anchorPositionSource;
        public Point AnchorPositionSource
        {
            get { return anchorPositionSource; }
            set
            {
                if (anchorPositionSource != value)
                {
                    anchorPositionSource = value;
                    OnPropertyChanged("AnchorPositionSource");
                }
            }
        }

     
        private double anchorAngleSource = 0;
        public double AnchorAngleSource
        {
            get { return anchorAngleSource; }
            set
            {
                if (anchorAngleSource != value)
                {
                    anchorAngleSource = value;
                    OnPropertyChanged("AnchorAngleSource");
                }
            }
        }

      
        private Point anchorPositionSink;
        public Point AnchorPositionSink
        {
            get { return anchorPositionSink; }
            set
            {
                if (anchorPositionSink != value)
                {
                    anchorPositionSink = value;
                    OnPropertyChanged("AnchorPositionSink");
                }
            }
        }
        
        private double anchorAngleSink = 0;
        public double AnchorAngleSink
        {
            get { return anchorAngleSink; }
            set
            {
                if (anchorAngleSink != value)
                {
                    anchorAngleSink = value;
                    OnPropertyChanged("AnchorAngleSink");
                }
            }
        }

        private ArrowSymbol sourceArrowSymbol = ArrowSymbol.None;
        public ArrowSymbol SourceArrowSymbol
        {
            get { return sourceArrowSymbol; }
            set
            {
                if (sourceArrowSymbol != value)
                {
                    sourceArrowSymbol = value;
                    OnPropertyChanged("SourceArrowSymbol");
                }
            }
        }

        public ArrowSymbol sinkArrowSymbol = ArrowSymbol.Arrow;
        public ArrowSymbol SinkArrowSymbol
        {
            get { return sinkArrowSymbol; }
            set
            {
                if (sinkArrowSymbol != value)
                {
                    sinkArrowSymbol = value;
                    OnPropertyChanged("SinkArrowSymbol");
                }
            }
        }

      
        private Point labelPosition;
        public Point LabelPosition
        {
            get { return labelPosition; }
            set
            {
                if (labelPosition != value)
                {
                    labelPosition = value;
                    OnPropertyChanged("LabelPosition");
                }
            }
        }

        
        private DoubleCollection strokeDashArray;
        public DoubleCollection StrokeDashArray
        {
            get => strokeDashArray;
            set
            {
                if (strokeDashArray != value)
                {
                    strokeDashArray = value;
                    OnPropertyChanged(nameof(StrokeDashArray));
                }
            }
        }
     
        private bool isSelected;
        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                if (isSelected != value)
                {
                    isSelected = value;
                    OnPropertyChanged("IsSelected");
                    if (isSelected)
                        ShowAdorner();
                    else
                        HideAdorner();
                }
            }
        }

        #endregion

        public Connection(Connector source, Connector sink)
        {
            this.ID = Guid.NewGuid();
            this.Source = source;
            this.Sink = sink;

            DataContext = this;
            CommandBindings.Add(new CommandBinding(SetSourceArrowCommand, OnSetSourceArrowCommandExecuted));
            CommandBindings.Add(new CommandBinding(SetSinkArrowCommand, OnSetSinkArrowCommandExecuted));
            CommandBindings.Add(new CommandBinding(SetLineTypeCommand, OnSetLineTypeCommandExecuted));
            CommandBindings.Add(new CommandBinding(SetLineStyleCommand, OnSetLineStyleCommandExecuted));

            base.Unloaded += new RoutedEventHandler(Connection_Unloaded);
        }

        public static readonly RoutedUICommand SetLineTypeCommand = new RoutedUICommand("Set Line Type", "SetLineType", typeof(Connection));

        private void OnSetLineTypeCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            
            if (e.Parameter is string lineType)
            {
                if (Enum.TryParse(lineType, out ConnectionLineType type))
                {
                    this._ConnectionLineType = type;
                    UpdatePathGeometry();
                }
            }
        }

        protected override void OnMouseDown(System.Windows.Input.MouseButtonEventArgs e)
        {
          SaveStateInitialize();
            base.OnMouseDown(e);

            
            DesignerCanvas designer = VisualTreeHelper.GetParent(this) as DesignerCanvas;
            if (designer != null)
            {
                if ((Keyboard.Modifiers & (ModifierKeys.Shift | ModifierKeys.Control)) != ModifierKeys.None)
                    if (this.IsSelected)
                    {
                        designer.SelectionService.RemoveFromSelection(this);
                    }
                    else
                    {
                        designer.SelectionService.AddToSelection(this);
                    }
                else if (!this.IsSelected)
                {
                    designer.SelectionService.SelectItem(this);
                }

                Focus();
            }
            e.Handled = false;
        }

        void OnConnectorPositionChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("Position"))
            {
                UpdatePathGeometry(); 
            }
        }
        private List<Point> GetCurvedConnectionLine(ConnectorInfo sourceInfo, ConnectorInfo sinkInfo)
        {
            var points = new List<Point>();

            Point start = GetOffsetPoint(sourceInfo);
            Point end = GetOffsetPoint(sinkInfo);

       
            Point controlPoint1;
            Point controlPoint2;

            switch (sourceInfo.Orientation)
            {
                case ConnectorOrientation.Left:
                    controlPoint1 = new Point(start.X - 100, start.Y);
                    break;
                case ConnectorOrientation.Top:
                    controlPoint1 = new Point(start.X, start.Y - 100);
                    break;
                case ConnectorOrientation.Right:
                    controlPoint1 = new Point(start.X + 100, start.Y);
                    break;
                case ConnectorOrientation.Bottom:
                    controlPoint1 = new Point(start.X, start.Y + 100);
                    break;
                default:
                    controlPoint1 = new Point(start.X + 100, start.Y);
                    break;
            }

            switch (sinkInfo.Orientation)
            {
                case ConnectorOrientation.Left:
                    controlPoint2 = new Point(end.X - 100, end.Y);
                    break;
                case ConnectorOrientation.Top:
                    controlPoint2 = new Point(end.X, end.Y - 100);
                    break;
                case ConnectorOrientation.Right:
                    controlPoint2 = new Point(end.X + 100, end.Y);
                    break;
                case ConnectorOrientation.Bottom:
                    controlPoint2 = new Point(end.X, end.Y + 100);
                    break;
                default:
                    controlPoint2 = new Point(end.X - 100, end.Y);
                    break;
            }

          
            for (double t = 0; t <= 1; t += 0.01)
            {
                var x = (1 - t) * (1 - t) * (1 - t) * start.X +
                        3 * (1 - t) * (1 - t) * t * controlPoint1.X +
                        3 * (1 - t) * t * t * controlPoint2.X +
                        t * t * t * end.X;

                var y = (1 - t) * (1 - t) * (1 - t) * start.Y +
                        3 * (1 - t) * (1 - t) * t * controlPoint1.Y +
                        3 * (1 - t) * t * t * controlPoint2.Y +
                        t * t * t * end.Y;

                points.Add(new Point(x, y));
            }

            return points;
        }
        private Point GetOffsetPoint(ConnectorInfo connector)
        {
            Point point = connector.Position;
            double margin = 10; 

            switch (connector.Orientation)
            {
                case ConnectorOrientation.Left:
                    point.X -= margin;
                    break;
                case ConnectorOrientation.Top:
                    point.Y -= margin;
                    break;
                case ConnectorOrientation.Right:
                    point.X += margin;
                    break;
                case ConnectorOrientation.Bottom:
                    point.Y += margin;
                    break;
            }

            return point;
        }
        public void UpdatePathGeometry()
        {
        
            if (Source != null && Sink != null)
            {
                PathGeometry geometry = new PathGeometry();
                List<Point> linePoints;

                switch (_ConnectionLineType)
                {
                   
                    case ConnectionLineType.Straight:
                 
                        linePoints = PathFinder.GetStraightConnectionLine(Source.GetInfo(), Sink.GetInfo(), true);
                        break;

                    case ConnectionLineType.Orthogonal:
           
                        linePoints = PathFinder.GetConnectionLine(Source.GetInfo(), Sink.GetInfo(), true);
                        break;

                    case ConnectionLineType.Curved:
                   
                        linePoints = GetCurvedConnectionLine(Source.GetInfo(), Sink.GetInfo());
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }

                if (linePoints.Count > 0)
                {
                    PathFigure figure = new PathFigure();
                    figure.StartPoint = linePoints[0];
                    linePoints.Remove(linePoints[0]);
                    figure.Segments.Add(new PolyLineSegment(linePoints, true));
                    geometry.Figures.Add(figure);

                    this.PathGeometry = geometry;
                }
            }
        }

        private void UpdateAnchorPosition()
        {
            Point pathStartPoint, pathTangentAtStartPoint;
            Point pathEndPoint, pathTangentAtEndPoint;
            Point pathMidPoint, pathTangentAtMidPoint;

         
            this.PathGeometry.GetPointAtFractionLength(0, out pathStartPoint, out pathTangentAtStartPoint);
            this.PathGeometry.GetPointAtFractionLength(1, out pathEndPoint, out pathTangentAtEndPoint);
            this.PathGeometry.GetPointAtFractionLength(0.5, out pathMidPoint, out pathTangentAtMidPoint);

      
            this.AnchorAngleSource = Math.Atan2(-pathTangentAtStartPoint.Y, -pathTangentAtStartPoint.X) * (180 / Math.PI);
            this.AnchorAngleSink = Math.Atan2(pathTangentAtEndPoint.Y, pathTangentAtEndPoint.X) * (180 / Math.PI);

         
            pathStartPoint.Offset(-pathTangentAtStartPoint.X * 5, -pathTangentAtStartPoint.Y * 5);
            pathEndPoint.Offset(pathTangentAtEndPoint.X * 5, pathTangentAtEndPoint.Y * 5);

            this.AnchorPositionSource = pathStartPoint;
            this.AnchorPositionSink = pathEndPoint;
            this.LabelPosition = pathMidPoint;
        }

        private void ShowAdorner()
        {
        
            if (this.connectionAdorner == null)
            {
                DesignerCanvas designer = VisualTreeHelper.GetParent(this) as DesignerCanvas;

                AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(this);
                if (adornerLayer != null)
                {
                    this.connectionAdorner = new ConnectionAdorner(designer, this);
                    adornerLayer.Add(this.connectionAdorner);
                }
            }
            this.connectionAdorner.Visibility = Visibility.Visible;
        }

        internal void HideAdorner()
        {
            if (this.connectionAdorner != null)
                this.connectionAdorner.Visibility = Visibility.Collapsed;
        }

        void Connection_Unloaded(object sender, RoutedEventArgs e)
        {
           
            this.Source = null;
            this.Sink = null;

          
            if (this.connectionAdorner != null)
            {
                DesignerCanvas designer = VisualTreeHelper.GetParent(this) as DesignerCanvas;

                AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(this);
                if (adornerLayer != null)
                {
                    adornerLayer.Remove(this.connectionAdorner);
                    this.connectionAdorner = null;
                }
            }
        }

        #region INotifyPropertyChanged Members

        // we could use DependencyProperties as well to inform others of property changes
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        #endregion
    }

    public enum ArrowSymbol
    {
        None,
        Arrow,
        Diamond,
        HollowDiamond,
        HollowArrow
    }
}
