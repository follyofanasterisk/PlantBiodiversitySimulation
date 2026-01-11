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
            AffectsRender<_2DViewer>(IsRunningProperty);
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

        public override void Render(DrawingContext context)
        {
            base.Render(context);
            
            if (IsRunning)
            {
                //Make sure to catch in case screen is very tiny
                double ScalingFactor = (Math.Min(Bounds.Center.X, Bounds.Center.Y) - 5) / 50;
                for (int i = 0; i < WorldGrid.Length; i++)
                {
                    double FirstTwoDigits = WorldGrid[i] % 100;
                    double LastTwoDigits = WorldGrid[i] - (WorldGrid[i] % 1000);
                    switch (WorldGrid[i] - LastTwoDigits - FirstTwoDigits)
                    {
                        case 0:
                            context.DrawRectangle(new Pen(Brushes.Green), new Rect(Bounds.Center.X + FirstTwoDigits * ScalingFactor,
                                Bounds.Center.Y + (LastTwoDigits / 1000) * ScalingFactor, 1, 1));
                            break;
                        case 900:
                            context.DrawRectangle(new Pen(Brushes.Green), new Rect(Bounds.Center.X + -(100 - FirstTwoDigits) * ScalingFactor,
                                Bounds.Center.Y + ((LastTwoDigits / 1000) + 1) * ScalingFactor, 1, 1));
                            break;
                        case -900:
                            context.DrawRectangle(new Pen(Brushes.Green), new Rect(Bounds.Center.X + (100 + FirstTwoDigits) * ScalingFactor,
                                Bounds.Center.Y + ((LastTwoDigits / 1000) - 1) * ScalingFactor, 1, 1));
                            break;
                    }
                }
                Dispatcher.UIThread.Post(InvalidateVisual, DispatcherPriority.Render);
            }
        }
    }
}
