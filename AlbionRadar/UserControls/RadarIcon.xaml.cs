using AlbionRadar.Entities;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace AlbionRadar.UserControls
{
    /// <summary>  
    /// Represents a single icon on the radar, updating its appearance  
    /// based on the associated RadarEntity data.  
    /// </summary>  
    public partial class RadarIcon : UserControl
    {
        private RadarEntity? _radarEntity;

        /// <summary>  
        /// Gets or sets the RadarEntity associated with this icon.  
        /// Updates visuals when the entity changes.  
        /// </summary>  
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

        // Maps enchantment levels to corresponding drop shadow colors.  
        private static readonly Dictionary<int, Color> s_dropShadowColorMap = new()
           {
               { 0, Colors.Transparent },
               { 1, Colors.Green },
               { 2, Colors.Blue },
               { 3, Colors.Purple },
               { 4, Colors.Yellow }
           };

        // Maps mist enchantment levels to corresponding colors.
        private static readonly Dictionary<int, Color> s_mistColorMap = new()
           {
               { 0, Colors.LightSkyBlue },
               { 1, Colors.Green },
               { 2, Colors.Blue },
               { 3, Colors.Purple },
               { 4, Colors.Orange },
           };

        public RadarIcon()
        {
            InitializeComponent();
            this.DataContext = this;
            UpdateVisuals();
        }

        /// <summary>  
        /// Handles property changes in the associated RadarEntity.  
        /// Updates visuals on the UI thread.  
        /// </summary>  
        private void OnRadarEntityPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            Dispatcher.Invoke(UpdateVisuals);
        }

        /// <summary>  
        /// Updates the UI elements based on the current RadarEntity state.  
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


            if (_radarEntity.Name != null && _radarEntity.Name.Contains("MIST"))
            {
                Color mistColor = s_mistColorMap.TryGetValue(_radarEntity.EnchantmentLevel, out var mist_color )
                    ? mist_color
                    : Colors.SkyBlue;
                MistEllipse.Visibility = Visibility.Visible;
                MistEllipse.Fill = new SolidColorBrush(mistColor);
                MistDropShadow.Color = mistColor;
                Title.Text = "MIST";
                return;
            }

            Color shadowColor = s_dropShadowColorMap.TryGetValue(_radarEntity.EnchantmentLevel, out var color)
                ? color
                : Colors.Transparent;

            if (string.IsNullOrEmpty(_radarEntity.ImageUrl))
            {
                MainEllipse.Visibility = Visibility.Visible;
                Title.Text = $"{_radarEntity.TypeId}";
            }
            else
            {
                MainImage.Visibility = Visibility.Visible;
                try
                {
                    MainImage.Source = new BitmapImage(new Uri(
                        $"pack://application:,,,/AlbionRadar;component/Assets/{_radarEntity.ImageUrl}",
                        UriKind.Absolute));
                }catch(Exception ex)
                {
                    MainImage.Source = new BitmapImage(new Uri(
                        $"pack://application:,,,/AlbionRadar;component/Assets/Resources/GRIFFIN.png",
                        UriKind.Absolute));
                }
                ImageDropShadow.Color = shadowColor;
                Title.Text = $"{_radarEntity.TypeId}";
            }
        }
    }
}