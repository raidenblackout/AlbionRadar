using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AlbionRadar.UserControls
{
    /// <summary>
    /// Interaction logic for RadarControl.xaml
    /// </summary>
    public partial class RadarControl : UserControl
    {

        public static readonly DependencyProperty RadarEntitiesProperty =
            DependencyProperty.Register("RadarEntities", typeof(IEnumerable<Entities.RadarEntity>), typeof(RadarControl), new PropertyMetadata(null, OnRadarEntitiesChanged));

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
                    var ellipse = new Ellipse
                    {
                        Width = 10,
                        Height = 10,
                        Fill = Brushes.Red,
                        ToolTip = entity.Name
                    };
                    Canvas.SetLeft(ellipse, entity.PositionX);
                    Canvas.SetTop(ellipse, entity.PositionY);
                    RadarCanvas.Children.Add(ellipse);
                }
            }
        }

        public IEnumerable<Entities.RadarEntity> RadarEntities
        {
            get { return (IEnumerable<Entities.RadarEntity>)GetValue(RadarEntitiesProperty); }
            set { SetValue(RadarEntitiesProperty, value); }
        }

        public RadarControl()
        {
            InitializeComponent();
        }
    }
}
