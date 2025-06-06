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
        // DependencyProperty for the main player
        public static readonly DependencyProperty MainPlayerProperty =
            DependencyProperty.Register(
                "MainPlayer",
                typeof(Entities.PlayerEntity),
                typeof(RadarControl),
                new PropertyMetadata(null, OnMainPlayerChanged)
            );

        // DependencyProperty for radar entities
        public static readonly DependencyProperty RadarEntitiesProperty =
            DependencyProperty.Register(
                "RadarEntities",
                typeof(ObservableCollection<Entities.RadarEntity>),
                typeof(RadarControl),
                new PropertyMetadata(null, OnRadarEntitiesChanged)
            );

        /// <summary>
        /// Gets or sets the main player entity.
        /// </summary>
        public Entities.PlayerEntity MainPlayer
        {
            get => (Entities.PlayerEntity)GetValue(MainPlayerProperty);
            set => SetValue(MainPlayerProperty, value);
        }

        /// <summary>
        /// Gets or sets the collection of radar entities.
        /// </summary>
        public ObservableCollection<Entities.RadarEntity> RadarEntities
        {
            get => (ObservableCollection<Entities.RadarEntity>)GetValue(RadarEntitiesProperty);
            set
            {
                SetValue(RadarEntitiesProperty, value);
                UpdateUI();
            }
        }

        /// <summary>
        /// Constructor for RadarControl.
        /// </summary>
        public RadarControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Callback when the MainPlayer property changes.
        /// </summary>
        private static void OnMainPlayerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is RadarControl control)
            {
                control.UpdateUI();
            }
        }

        /// <summary>
        /// Callback when the RadarEntities property changes.
        /// </summary>
        private static void OnRadarEntitiesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is RadarControl control)
            {
                control.UpdateUI();
            }
        }

        /// <summary>
        /// Updates the radar UI by clearing and redrawing all entities and the main player.
        /// </summary>
        private void UpdateUI()
        {
            // Clear the canvas
            RadarCanvas.Children.Clear();

            // Draw radar entities
            if (RadarEntities != null)
            {
                foreach (var entity in RadarEntities)
                {
                    var radarIcon = new RadarIcon
                    {
                        RadarEntity = entity
                    };

                    // Calculate the position relative to the main player
                    var relativePosition = GetPositionRelativeToPlayer(entity.PositionX, entity.PositionY);

                    // Rotate the position and map it to the canvas
                    var mappedPoints = AlbionMapMapper.RotateWithCenter(
                        relativePosition.Item1,
                        relativePosition.Item2,
                        (float)RadarCanvas.ActualWidth / 2.0f,
                        (float)RadarCanvas.ActualHeight / 2.0f,
                        -45
                    );

                    // Set the position of the radar icon
                    Canvas.SetLeft(radarIcon, mappedPoints.Item1);
                    Canvas.SetTop(radarIcon, mappedPoints.Item2);

                    // Add the radar icon to the canvas
                    RadarCanvas.Children.Add(radarIcon);
                }
            }

            // Draw the main player as a brown ellipse
            var mainPlayerEllipse = new Ellipse
            {
                Width = 10,
                Height = 10,
                Fill = Brushes.Brown
            };

            // Center the main player on the canvas
            Canvas.SetLeft(mainPlayerEllipse, RadarCanvas.ActualWidth / 2.0);
            Canvas.SetTop(mainPlayerEllipse, RadarCanvas.ActualHeight / 2.0);

            // Add the main player to the canvas
            RadarCanvas.Children.Add(mainPlayerEllipse);
        }

        /// <summary>
        /// Calculates the position of an entity relative to the main player.
        /// </summary>
        /// <param name="x">The X coordinate of the entity.</param>
        /// <param name="y">The Y coordinate of the entity.</param>
        /// <returns>A tuple containing the relative X and Y coordinates.</returns>
        private Tuple<float, float> GetPositionRelativeToPlayer(float x, float y)
        {
            if (MainPlayer == null)
            {
                return new Tuple<float, float>(x, y);
            }

            // Calculate the deltas
            float deltaX = x - MainPlayer.PositionX;
            float deltaY = y - MainPlayer.PositionY;

            // Scale
            float scaledX = deltaY * 5f;
            float scaledY = deltaX * 5f;

            // Map the position to the canvas center
            float relativeX = scaledX + (float)RadarCanvas.ActualWidth / 2.0f;
            float relativeY = scaledY + (float)RadarCanvas.ActualHeight / 2.0f;

            return new Tuple<float, float>(relativeX, relativeY);
        }
    }
}
