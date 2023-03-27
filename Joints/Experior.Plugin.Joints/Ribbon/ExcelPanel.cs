using System;
using Experior.Interfaces;
using Experior.Plugin.Joints.Recorder;

namespace Experior.Plugin.Joints.Ribbon
{
    public class ExcelPanel : RecorderPanel
    {
        #region Fields

        private readonly JointData _recorder;

        #endregion

        #region Constructor

        public ExcelPanel()
        {
            _recorder = new JointData();
            _recorder.Exported += OnExported;
            _recorder.AssemblyDeleted += OnAssemblyDeleted;
        }

        #endregion

        #region Public Methods

        public override void Record()
        {
            if (!_recorder.Record())
            {
                Status = RecorderStatus.Idle;
            }
        }

        public override void Stop()
        {
            _recorder.Stop();
        }

        public override void Export()
        {
            _recorder.Export();
        }

        public override void Dispose()
        {
            _recorder.Clean();

            base.Dispose();
        }

        public override void Clean()
        {
            _recorder.Clean();

            base.Clean();
        }

        #endregion

        #region Private Methods

        private void OnExported(object sender, EventArgs e)
        {
            Log.Write($"Exporting process has finished...", System.Windows.Media.Colors.Green, LogFilter.Communication);

            Status = RecorderStatus.Idle;
        }

        private void OnAssemblyDeleted(object sender, EventArgs e)
        {
            Status = RecorderStatus.Stopped;
            Clean();
        }

        #endregion
    }
}
