using System.Numerics;
using System.Windows.Forms.DataVisualization.Charting;
using Experior.Catalog.Joints.Assemblies;
using Experior.Core;

namespace Experior.Plugin.Joints.GUI.Chart
{
    public class DataHandler
    {
        #region Fields

        private readonly IMechanism _mechanism;

        private ChartWindow _window;
        private readonly Series _dataPoints;

        private Timer _timer;

        #endregion

        #region Constructor

        public DataHandler(IMechanism mechanism, ChartWindow window, ChartConfigurator configurator)
        {
            _mechanism = mechanism;
            _window = window;
            _dataPoints = configurator.CreateSeries();

            window.JointModified += JointModified;
            window.ChartDataTypeModified += ChartDataTypeModified;
            window.AxisModified += AxisModified;
            window.SamplingTimeModified += (sender, value) => SetSamplingTime(value);

            Init();
        }

        #endregion

        #region Public Methods

        public void SetSamplingTime(float value)
        {
            if (_timer != null)
            {
                if (_timer.Running)
                {
                    _timer.Stop();
                }

                _timer.Timeout = value;
                _timer.Start();
            }
        }

        public void DisposeData()
        {
            _timer.OnElapsed -= TimerOnElapsed;

            _timer.Stop();
            _timer.Dispose();
            _timer = null;
        }

        #endregion

        #region Private Methods

        private void Init()
        {
            _timer = new Timer(0.5f)
            {
                AutoReset = true
            };

            _timer.OnElapsed += TimerOnElapsed;
            _timer.Start();
        }

        private void TimerOnElapsed(Timer sender)
        {
            Experior.Core.Environment.InvokeUiAction(() =>
            {
                _dataPoints.Points?.AddXY(GetSimulatedTime(), GetValue());
            });
        }

        private static float GetSimulatedTime()
        {
            return (float)(Environment.Time.Elapsed);
        }

        private void JointModified(object sender, string e)
        {
            CleanChart();
        }

        private void ChartDataTypeModified(object sender, ChartWindow.ChartDataTypes e)
        {
            CleanChart();
        }

        private void AxisModified(object sender, ChartWindow.Axes e)
        {
            _dataPoints.Points.Clear();
        }

        private void CleanChart() // TODO: STOP SIMULATION ??
        {
            _dataPoints.Points.Clear();
        }

        private int GetJointIndex()
        {
            var result = _mechanism.JointId.IndexOf(_window.Joint);

            return result < 0 ? 0 : result;
        }

        private float GetValue()
        {
            var index = GetJointIndex();

            switch (_window.ChartDataType)
            {
                case ChartWindow.ChartDataTypes.Angular_Velocity:
                    return GetAxisValue(_mechanism.GetAngularVelocity(index));

                case ChartWindow.ChartDataTypes.Linear_Force:
                    return GetAxisValue(_mechanism.GetLinearForce(index));

                case ChartWindow.ChartDataTypes.Angular_Force:
                    return GetAxisValue(_mechanism.GetAngularForce(index));

                default:
                    return GetAxisValue(_mechanism.GetLinearVelocity(index));
            }
        }

        private float GetAxisValue(Vector3 value)
        {
            switch (_window.Axis)
            {
                case ChartWindow.Axes.Y:
                    return value.Y;

                case ChartWindow.Axes.Z:
                    return value.Z;

                default:
                    return value.X;
            }
        }

        #endregion
    }
}
