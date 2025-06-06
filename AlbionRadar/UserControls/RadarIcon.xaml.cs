using AlbionRadar.Entities;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace AlbionRadar.UserControls
{
    /// <summary>  
    /// Interaction logic for RadarIcon.xaml  
    /// </summary>  
    public partial class RadarIcon : UserControl
    {
        // Backing field for the RadarEntity property  
        private RadarEntity _radarEntity;

        /// <summary>  
        /// Gets or sets the RadarEntity associated with this control.  
        /// Updates the UI whenever the RadarEntity changes.  
        /// </summary>  
        public RadarEntity RadarEntity
        {
            get => _radarEntity;
            set
            {
                if (_radarEntity != value)
                {
                    _radarEntity = value;
                    UpdateUI();
                }
            }
        }

        // Mapping of enchantment levels to drop shadow colors  
        private readonly Dictionary<int, Color> _dropShadowColorMap = new Dictionary<int, Color>
           {
               { 0, Colors.Transparent },
               { 1, Colors.Green },
               { 2, Colors.Blue },
               { 3, Colors.Purple },
               { 4, Colors.Yellow }
           };

        /// <summary>  
        /// Updates the UI elements based on the current RadarEntity.  
        /// </summary>  
        private void UpdateUI()
        {
            if (RadarEntity == null)
            {
                // If RadarEntity is null, hide all elements  
                MainImage.Visibility = Visibility.Collapsed;
                MainEllipse.Visibility = Visibility.Collapsed;
                Title.Text = string.Empty;
                return;
            }

            if(RadarEntity.Name != null && RadarEntity.Name.Contains("MIST"))
            {
                // If the entity is a MIST, set the title to "MIST" and hide the image  
                MainImage.Visibility = Visibility.Collapsed;
                MainEllipse.Visibility = Visibility.Collapsed;
                MistEllipse.Visibility  = Visibility.Visible;
                MistDropShadow.Color = _dropShadowColorMap.ContainsKey(RadarEntity.EnchantmentLevel)
                    ? _dropShadowColorMap[RadarEntity.EnchantmentLevel]
                    : Colors.Transparent;
                Title.Text = "MIST";
                return;
            }

            if (string.IsNullOrEmpty(RadarEntity.ImageUrl))
            {
                // If no image URL is provided, show the ellipse and hide the image  
                MainImage.Visibility = Visibility.Collapsed;
                MainEllipse.Visibility = Visibility.Visible;
                Title.Text = $"{RadarEntity.TypeId}";
            }
            else
            {
                // If an image URL is provided, show the image and hide the ellipse  
                MainImage.Visibility = Visibility.Visible;
                MainEllipse.Visibility = Visibility.Collapsed;

                // Set the image source using the provided URL  
                MainImage.Source = new BitmapImage(new Uri(
                    $"pack://application:,,,/AlbionRadar;component/Assets/{RadarEntity.ImageUrl}",
                    UriKind.Absolute));

                // Update the title text  
                Title.Text = $"{RadarEntity.TypeId}";

                // Set the drop shadow color based on the enchantment level  
                ImageDropShadow.Color = _dropShadowColorMap.ContainsKey(RadarEntity.EnchantmentLevel)
                    ? _dropShadowColorMap[RadarEntity.EnchantmentLevel]
                    : Colors.Transparent;
            }
        }

        /// <summary>  
        /// Initializes a new instance of the RadarIcon control.  
        /// </summary>  
        public RadarIcon()
        {
            InitializeComponent();
        }
    }
}
