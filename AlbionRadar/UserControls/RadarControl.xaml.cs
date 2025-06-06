using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using AlbionRadar.Mappers;

namespace AlbionRadar.UserControls
{
    /// <summary>
    /// Interaction logic for RadarControl.xaml
    /// </summary>
    public partial class RadarControl : UserControl
    {
        public static readonly DependencyProperty MainPLayerProperty =
            DependencyProperty.Register("MainPlayer", typeof(Entities.PlayerEntity), typeof(RadarControl), new PropertyMetadata(null, OnMainPlayerChanged));
        private static void OnMainPlayerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as RadarControl;
            if (control != null)
            {
                control.UpdateUI();
            }
        }

        public Entities.PlayerEntity MainPlayer
        {
            get { return (Entities.PlayerEntity)GetValue(MainPLayerProperty); }
            set { SetValue(MainPLayerProperty, value); }
        }

        public static readonly DependencyProperty RadarEntitiesProperty =
            DependencyProperty.Register("RadarEntities", typeof(ObservableCollection<Entities.RadarEntity>), typeof(RadarControl), new PropertyMetadata(null, OnRadarEntitiesChanged));

        private static void OnRadarEntitiesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as RadarControl;
            if (control != null)
            {
                control.UpdateUI();
            }
        }

        private void UpdateUI()
        {
            RadarCanvas.Children.Clear();
            if (RadarEntities != null)
            {
                foreach (var entity in RadarEntities)
                {

                    FrameworkElement element = new Ellipse
                    {
                        Width = 20,
                        Height = 20,
                        Fill = Brushes.Red,
                        ToolTip = entity.Name
                    };

                    if (entity.ImageUrl != null && entity.ImageUrl.Length > 0)
                    {
                        element = new Image
                        {
                            Source = new BitmapImage(new Uri($"pack://application:,,,/AlbionRadar;component/Assets/{entity.ImageUrl}", UriKind.Absolute)),
                            Width = 40,
                            Height = 40,
                            Stretch = Stretch.UniformToFill
                        };
                    }

                    var relativePosition = GetPositionRelativeToPlayer(entity.PositionX, entity.PositionY);
                    var title = new TextBlock();
                    title.Text = $"{entity.TypeId}";
                    title.Foreground = new SolidColorBrush(Colors.Green);

                    var mappedPoints = AlbionMapMapper.RotateWithCenter(relativePosition.Item1, relativePosition.Item2, (float)RadarCanvas.ActualWidth / 2.0f, (float)RadarCanvas.ActualHeight / 2.0f, -45);

                    Canvas.SetLeft(element, mappedPoints.Item1);
                    Canvas.SetTop(element, mappedPoints.Item2);
                    if (element is Image)
                    {
                        title.InvalidateMeasure();
                        title.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                        title.Arrange(new Rect(title.DesiredSize));
                        double width = title.DesiredSize.Width;
                        Canvas.SetLeft(title, mappedPoints.Item1 + width/2);
                        Canvas.SetTop(title, mappedPoints.Item2 + 40);
                    }
                    else
                    {
                        Canvas.SetLeft(title, mappedPoints.Item1 - title.Text.Length);
                        Canvas.SetTop(title, mappedPoints.Item2 + 20);
                    }
                    

                    RadarCanvas.Children.Add(element);
                    RadarCanvas.Children.Add(title);
                }
            }

            var mainPlayer = new Ellipse
            {
                Width = 10,
                Height = 10,
                Fill = Brushes.Blue,
            };

            Canvas.SetLeft(mainPlayer, RadarCanvas.ActualWidth / 2.0);
            Canvas.SetTop(mainPlayer, RadarCanvas.ActualHeight / 2.0);

            RadarCanvas.Children.Add(mainPlayer);
        }

        private Tuple<float, float> GetPositionRelativeToPlayer(float x, float y)
        {
            if (MainPlayer == null)
                return new Tuple<float, float>(x, y);

            // Rotate the position by 90 degrees clockwise around the main player  
            float deltaX = x - MainPlayer.PositionX;
            float deltaY = y - MainPlayer.PositionY;
            float rotatedX = deltaY*5f;
            float rotatedY = deltaX*5f;

            float relativeX = rotatedX + (float)RadarCanvas.ActualWidth / 2.0f;
            float relativeY = rotatedY + (float)RadarCanvas.ActualHeight / 2.0f;

            return new Tuple<float, float>(relativeX, relativeY);
        }

        public ObservableCollection<Entities.RadarEntity> RadarEntities
        {
            get { return (ObservableCollection<Entities.RadarEntity>)GetValue(RadarEntitiesProperty); }
            set { 
                SetValue(RadarEntitiesProperty, value);
                UpdateUI();
            }
        }

        public RadarControl()
        {
            InitializeComponent();
        }
    }
}
