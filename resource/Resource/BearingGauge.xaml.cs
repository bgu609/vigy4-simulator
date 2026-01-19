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

namespace Pa5455CmsResource.Resource
{
    /// <summary>
    /// BearingGauge.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class BearingGauge : UserControl
    {
        public BearingGauge()
        {
            InitializeComponent();
        }
        #region BearingDgr (전체 회전)
        public double BearingAngle
        {
            get => (double)GetValue(BearingAngleProperty);
            set => SetValue(BearingAngleProperty, value);
        }

        public static readonly DependencyProperty BearingAngleProperty =
            DependencyProperty.Register(
                nameof(BearingAngle),
                typeof(double),
                typeof(BearingGaugeElevation),
                new PropertyMetadata(0.0, OnBearingDrgChanged));

        /// <summary>
        /// BearingDrg 값이 변경되면 바늘 회전 및 각도 표시를 업데이트
        /// </summary>
        private static void OnBearingDrgChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not BearingGaugeElevation ctrl)
                return;

            double newValue = (double)e.NewValue;


            // DialGroup 회전
            if (ctrl.DialGroup.RenderTransform is TransformGroup tg)
            {
                foreach (var tr in tg.Children)
                {
                    if (tr is RotateTransform rt)
                    {
                        rt.Angle = newValue;
                        break;
                    }
                }
            }
        }
        #endregion
        #region NeedleAngle (바늘 개별 회전)
        public double NeedleAngle
        {
            get => (double)GetValue(NeedleAngleProperty);
            set => SetValue(NeedleAngleProperty, value);
        }

        public static readonly DependencyProperty NeedleAngleProperty =
            DependencyProperty.Register(nameof(NeedleAngle), typeof(double), typeof(BearingGaugeElevation),
                new PropertyMetadata(0.0, OnNeedleChanged));

        private static void OnNeedleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = (BearingGaugeElevation)d;
            double angle = (double)e.NewValue;


            if (ctrl.Needle.RenderTransform is RotateTransform rt)
                rt.Angle = angle;
        }
        #endregion
    }
}
