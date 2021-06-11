using ContinuousLines.Resolvers;
using GalaSoft.MvvmLight.Command;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace ContinuousLines
{
    /// <summary>
    /// The view model class for MainWindow.
    /// </summary>
    class MainWindowViewModel : INotifyPropertyChanged
    {
        private readonly PathResolver pathResolver;
        private Point previousPoint;
        private bool isPreviousPointSet = false;

        public MainWindowViewModel()
        {
            pathResolver = new(Height, Width);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public int Height { get; set; } = 600;
        public int Width { get; set; } = 600;

        public ObservableCollection<PointCollection> PolylinesPointsCollection { get; } = new();
        public Thickness PreviousPointMargin => new(PreviousPoint.X, PreviousPoint.Y, 0, 0);

        public bool IsPreviousPointSet
        {
            get => isPreviousPointSet;
            set
            {
                isPreviousPointSet = value;
                NotifyPropertyChanged();
            }
        }

        /// Points where line will start. It is drawn after first user click.
        public Point PreviousPoint
        {
            get => previousPoint;
            set
            {
                previousPoint = value;
                NotifyPropertyChanged(nameof(PreviousPointMargin));
            }
        }

        public ICommand Window_MouseLeftButtonDown => new RelayCommand<object>(PerformWindow_MouseLeftButtonDown);

        private void PerformWindow_MouseLeftButtonDown(object param)
        {
            if (param is not IInputElement inputElement)
            {
                return;
            }

            var point = Mouse.GetPosition(inputElement);

            if (IsPreviousPointSet)
            {
                // Draws line between previous and current point
                AddLine(PreviousPoint, point);
                IsPreviousPointSet = false;
            }
            else
            {
                // Draws point, where line will start.
                PreviousPoint = point;
                IsPreviousPointSet = true;
            }
        }

        private void AddLine(Point startPoint, Point endPoint)
        {
            PolylinesPointsCollection.Add(pathResolver.ResolveNextPath(startPoint, endPoint));
        }

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
