using Avalonia;
using Avalonia.Media;
using Avalonia.Controls;
using Avalonia.Visuals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Threading;
using System.Diagnostics;

namespace SoilFertilitySimulation.Controls
{
    public class _2DViewer : Control
    {
        public _2DViewer()
        {
            //AffectsRender<>
        }

        private int[] _worldgrid = [];

        public int[] WorldGrid
        {
            get {  return _worldgrid; }
            set { SetAndRaise(WorldGridProperty, ref _worldgrid, value); }
        }

        public static readonly DirectProperty<_2DViewer, int[]> WorldGridProperty =
            AvaloniaProperty.RegisterDirect<_2DViewer, int[]>(
                nameof(WorldGrid),
                o => o.WorldGrid,
                (o, v) => o.WorldGrid = v);

        public bool IsRunning
        {
            get { return GetValue(IsRunningProperty); }
            set { SetValue(IsRunningProperty, value); }
        }

        public static readonly StyledProperty<bool> IsRunningProperty =
            AvaloniaProperty.Register<_2DViewer, bool>(
                nameof(IsRunning));

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);
            if (change.Property == IsRunningProperty)
            {
                Dispatcher.UIThread.Post(InvalidateVisual, DispatcherPriority.Render);
            }
        }

        public override void Render(DrawingContext context)
        {
            base.Render(context);
            
            if (IsRunning)
            {
                //transform coordinates and then get center, add the coordinates divided by distance from center to corner
                //Make sure to catch in case screen is very tiny
                //draw rectanges based 
                //What scaling do you want for the screen size and grid??? Currently is 0
                Point ScalingFactor = new Point((Bounds.Center.X - 5) / 50, (Bounds.Center.Y - 5) / 50);
                for (int i = 0; i < WorldGrid.Length; i++)
                {
                    double FirstTwoDigits = WorldGrid[i] % 100;
                    double LastTwoDigits = WorldGrid[i] - (WorldGrid[i] % 1000);
                    //border problems
                    switch (WorldGrid[i] - LastTwoDigits - FirstTwoDigits)
                    {
                        case 0:
                            context.DrawRectangle(new Pen(Brushes.White), new Rect(Bounds.Center.X + FirstTwoDigits * ScalingFactor.X,
                                Bounds.Center.Y + (LastTwoDigits / 1000) * ScalingFactor.Y, 1, 1));
                            break;
                        case 900:
                            context.DrawRectangle(new Pen(Brushes.White), new Rect(Bounds.Center.X + -(100 - FirstTwoDigits) * ScalingFactor.X,
                                Bounds.Center.Y + ((LastTwoDigits / 1000) + 1) * ScalingFactor.Y, 1, 1));
                            break;
                        case -900:
                            context.DrawRectangle(new Pen(Brushes.White), new Rect(Bounds.Center.X + (100 + FirstTwoDigits) * ScalingFactor.X,
                                Bounds.Center.Y + ((LastTwoDigits / 1000) - 1) * ScalingFactor.Y, 1, 1));
                            break;
                    }
                }
                //context.DrawRectangle(new Pen(Brushes.White), new Rect(0, 0, 10, 10));
                //context.DrawEllipse(Brushes.White, null, new Point(100, 100), 10, 10);
                Dispatcher.UIThread.Post(InvalidateVisual, DispatcherPriority.Render);
            }
        }
    }
}
