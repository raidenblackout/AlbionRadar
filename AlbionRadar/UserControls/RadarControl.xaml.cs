using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using AlbionRadar.Entities;
using AlbionRadar.Mappers;
using BaseUtils.Logger.Impl;

namespace AlbionRadar.UserControls
{
    /// <summary>  
    /// Interaction logic for RadarControl.xaml  
    /// </summary>  
    public partial class RadarControl : UserControl
    {
        private readonly Dictionary<int, RadarIcon> _entityVisuals = new();
        private Ellipse _mainPlayerEllipse;

        public static readonly DependencyProperty MainPlayerProperty =
            DependencyProperty.Register("MainPlayer", typeof(PlayerEntity), typeof(RadarControl), new PropertyMetadata(null));

        public static readonly DependencyProperty RadarEntitiesProperty =
            DependencyProperty.Register("RadarEntities", typeof(ObservableCollection<RadarEntity>), typeof(RadarControl), new PropertyMetadata(null, OnRadarEntitiesChanged));

        public PlayerEntity MainPlayer
        {
            get => (PlayerEntity)GetValue(MainPlayerProperty);
            set => SetValue(MainPlayerProperty, value);
        }

        public ObservableCollection<RadarEntity> RadarEntities
        {
            get => (ObservableCollection<RadarEntity>)GetValue(RadarEntitiesProperty);
            set => SetValue(RadarEntitiesProperty, value);
        }

        public RadarControl()
        {
            InitializeComponent();
            // Attach to the rendering event for smooth updates.  
            CompositionTarget.Rendering += UpdateRadarVisuals;
        }

        private static void OnRadarEntitiesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not RadarControl control) return;

            // Clear visuals when the collection changes.  
            control.ClearAllVisuals();
        }

        private void ClearAllVisuals()
        {
            foreach (var visual in _entityVisuals.Values)
            {
                RadarCanvas.Children.Remove(visual);
            }
            _entityVisuals.Clear();
        }

        private static DateTime _lastUpdateTime = DateTime.MinValue;
        private void LogFPS()
        {
            var now = DateTime.Now;
            if (_lastUpdateTime != DateTime.MinValue)
            {
                var fps = 1 / (now - _lastUpdateTime).TotalSeconds;
                DLog.I($"Radar updated. FPS: {fps:F2}");
            }
            _lastUpdateTime = now;
        }

        /// <summary>  
        /// Updates the radar visuals on each frame.  
        /// </summary>  
        private void UpdateRadarVisuals(object? sender, EventArgs e)
        {
            if (RadarEntities == null || MainPlayer == null) return;

            var usedIds = new HashSet<int>();

            foreach (var entity in RadarEntities)
            {
                usedIds.Add(entity.Id);

                if (!_entityVisuals.TryGetValue(entity.Id, out var radarIcon))
                {
                    radarIcon = new RadarIcon();
                    _entityVisuals[entity.Id] = radarIcon;
                    RadarCanvas.Children.Add(radarIcon);
                }

                radarIcon.RadarEntity = entity;

                var relativePosition = GetPositionRelativeToPlayer(entity.PositionX, entity.PositionY);

                var mappedPoints = AlbionMapMapper.RotateWithCenter(
                    relativePosition.Item1,
                    relativePosition.Item2,
                    (float)RadarCanvas.ActualWidth / 2f,
                    (float)RadarCanvas.ActualHeight / 2f,
                    -45
                );

                Canvas.SetLeft(radarIcon, mappedPoints.Item1 - radarIcon.ActualWidth / 2.0);
                Canvas.SetTop(radarIcon, mappedPoints.Item2 - radarIcon.ActualHeight / 2.0);
            }

            var idsToRemove = _entityVisuals.Keys.Except(usedIds).ToList();
            foreach (var id in idsToRemove)
            {
                if (_entityVisuals.TryGetValue(id, out var visualToRemove))
                {
                    RadarCanvas.Children.Remove(visualToRemove);
                    _entityVisuals.Remove(id);
                }
            }

            UpdateMainPlayerVisual();
            LogFPS();
        }

        /// <summary>  
        /// Updates or creates the visual for the main player.  
        /// </summary>  
        private void UpdateMainPlayerVisual()
        {
            if (_mainPlayerEllipse == null)
            {
                _mainPlayerEllipse = new Ellipse
                {
                    Width = 10,
                    Height = 10,
                    Fill = Brushes.Brown,
                    ToolTip = "Main Player"
                };
                RadarCanvas.Children.Add(_mainPlayerEllipse);
            }

            double centerX = RadarCanvas.ActualWidth / 2.0;
            double centerY = RadarCanvas.ActualHeight / 2.0;
            Canvas.SetLeft(_mainPlayerEllipse, centerX - _mainPlayerEllipse.Width / 2.0);
            Canvas.SetTop(_mainPlayerEllipse, centerY - _mainPlayerEllipse.Height / 2.0);
        }

        /// <summary>  
        /// Calculates the position of an entity relative to the main player.  
        /// </summary>  
        private Tuple<float, float> GetPositionRelativeToPlayer(float x, float y)
        {
            float deltaX = x - MainPlayer.PositionX;
            float deltaY = y - MainPlayer.PositionY;

            const float zoomFactor = 5.0f;
            float scaledX = deltaY * zoomFactor;
            float scaledY = deltaX * zoomFactor;

            float relativeX = scaledX + (float)RadarCanvas.ActualWidth / 2.0f;
            float relativeY = scaledY + (float)RadarCanvas.ActualHeight / 2.0f;

            return new Tuple<float, float>(relativeX, relativeY);
        }
    }
}