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
        private RadarEntity _radarEntity;
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

        private readonly Dictionary<int, Color> _dropShadowColorMap = new Dictionary<int, Color>
        {
            { 0, Colors.Transparent },
            { 1, Colors.Green },
            { 2, Colors.Blue },
            { 3, Colors.Purple },
            { 4, Colors.Yellow }
        };

        private void UpdateUI()
        {
            if (string.IsNullOrEmpty(RadarEntity.ImageUrl))
            {
                MainImage.Visibility = Visibility.Collapsed;
                MainEllipse.Visibility = Visibility.Visible;
                Title.Text = $"{RadarEntity.TypeId}";
            }
            else
            {
                MainImage.Visibility = Visibility.Visible;
                MainEllipse.Visibility = Visibility.Collapsed;
                MainImage.Source = new BitmapImage(new Uri($"pack://application:,,,/AlbionRadar;component/Assets/{RadarEntity.ImageUrl}", UriKind.Absolute));
                Title.Text = $"{RadarEntity.TypeId}";
                ImageDropShadow.Color = _dropShadowColorMap.ContainsKey(RadarEntity.EnchantmentLevel)
                    ? _dropShadowColorMap[RadarEntity.EnchantmentLevel]
                    : Colors.Transparent;
            }
        }

        public RadarIcon()
        {
            InitializeComponent();
        }
    }
}
