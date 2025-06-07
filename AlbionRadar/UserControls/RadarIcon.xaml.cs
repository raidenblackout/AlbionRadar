using AlbionRadar.Entities;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace AlbionRadar.UserControls
{
    /// <summary>  
    /// Represents a single icon on the radar, which updates its appearance
    /// based on the data in its associated RadarEntity.
    /// </summary>  
    public partial class RadarIcon : UserControl
    {
        private RadarEntity? _radarEntity;

        public RadarEntity? RadarEntity
        {
            get => _radarEntity;
            set
            {
                if (!Equals(_radarEntity, value))
                {
                    if (_radarEntity != null)
                        _radarEntity.PropertyChanged -= OnRadarEntityPropertyChanged;

                    _radarEntity = value;

                    if (_radarEntity != null)
                        _radarEntity.PropertyChanged += OnRadarEntityPropertyChanged;

                    UpdateVisuals();
                }
            }
        }

        // Mapping of enchantment levels to drop shadow colors  
        private static readonly Dictionary<int, Color> s_dropShadowColorMap = new()
        {
            { 0, Colors.Transparent },
            { 1, Colors.Green },
            { 2, Colors.Blue },
            { 3, Colors.Purple },
            { 4, Colors.Yellow }
        };

        public RadarIcon()
        {
            InitializeComponent();
            this.DataContext = this; // Simplifies bindings in XAML if needed
            UpdateVisuals(); // Set initial state
        }

        private void OnRadarEntityPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            // When a property of the entity changes (e.g., EnchantmentLevel), update the visuals.
            // Dispatching to the UI thread is good practice if the event could come from a background thread.
            Dispatcher.Invoke(UpdateVisuals);
        }

        /// <summary>  
        /// Updates all UI elements based on the current state of the RadarEntity.  
        /// </summary>  
        private void UpdateVisuals()
        {
            if (_radarEntity == null)
            {
                this.Visibility = Visibility.Collapsed;
                return;
            }

            this.Visibility = Visibility.Visible;
            MistEllipse.Visibility = Visibility.Collapsed;
            MainImage.Visibility = Visibility.Collapsed;
            MainEllipse.Visibility = Visibility.Collapsed;

            Color shadowColor = s_dropShadowColorMap.TryGetValue(_radarEntity.EnchantmentLevel, out var color)
                ? color
                : Colors.Transparent;

            if (_radarEntity.Name != null && _radarEntity.Name.Contains("MIST"))
            {
                MistEllipse.Visibility = Visibility.Visible;
                MistEllipse.Fill = new SolidColorBrush(shadowColor);
                MistDropShadow.Color = shadowColor;
                Title.Text = "MIST";
            }
            else if (string.IsNullOrEmpty(_radarEntity.ImageUrl))
            {
                MainEllipse.Visibility = Visibility.Visible;
                // You might want a default color for the ellipse
                // MainEllipse.Fill = ... 
                Title.Text = $"{_radarEntity.TypeId}";
            }
            else
            {
                MainImage.Visibility = Visibility.Visible;
                MainImage.Source = new BitmapImage(new Uri(
                    $"pack://application:,,,/AlbionRadar;component/Assets/{_radarEntity.ImageUrl}",
                    UriKind.Absolute));
                ImageDropShadow.Color = shadowColor;
                Title.Text = $"{_radarEntity.TypeId}";
            }
        }
    }
}