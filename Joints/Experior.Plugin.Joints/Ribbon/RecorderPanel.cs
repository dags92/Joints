using System;
using Experior.Interfaces;
using static Experior.Plugin.Joints.Joints;
using Environment = Experior.Core.Environment;

namespace Experior.Plugin.Joints.Ribbon
{
    public abstract class RecorderPanel
    {
        #region Fields

        private RecorderStatus _status;
        private Environment.UI.Toolbar.Button _recordButton, _stopButton, _exportButton;

        #endregion

        #region Constructor

        protected RecorderPanel()
        {
            Init();

            Experior.Core.Environment.Scene.OnCleared += SceneOnCleared;
        }

        #endregion

        #region Enum

        public enum RecorderStatus
        {
            Stopped,
            Recording,
            Exporting,
            Idle
        }

        #endregion

        #region Public Properties

        public RecorderStatus Status
        {
            get => _status;
            protected set
            {
                switch (value)
                {
                    case RecorderStatus.Stopped:

                        _recordButton.Enabled = true;
                        _stopButton.Enabled = false;
                        _exportButton.Enabled = true;

                        Log.Write($"Recording has stopped...", System.Windows.Media.Colors.Green, LogFilter.Communication);

                        break;

                    case RecorderStatus.Recording:

                        _recordButton.Enabled = false;
                        _stopButton.Enabled = true;
                        _exportButton.Enabled = false;

                        Log.Write($"Recording has started...", System.Windows.Media.Colors.Green, LogFilter.Communication);

                        break;

                    case RecorderStatus.Exporting:

                        _recordButton.Enabled = false;
                        _stopButton.Enabled = false;
                        _exportButton.Enabled = false;

                        Log.Write($"Exporting data...", System.Windows.Media.Colors.Green, LogFilter.Communication);

                        break;

                    default:

                        _recordButton.Enabled = true;
                        _stopButton.Enabled = false;
                        _exportButton.Enabled = false;

                        break;
                }

                _status = value;
            }
        }

        #endregion

        #region Public Methods

        public abstract void Record();

        public abstract void Stop();

        public abstract void Export();

        public virtual void Clean()
        {
            _status = RecorderStatus.Stopped;

            _recordButton.Enabled = true;
            _stopButton.Enabled = false;
            _exportButton.Enabled = false;
        }

        public virtual void Dispose()
        {
            Status = RecorderStatus.Stopped;
        }

        #endregion

        #region Private Methods

        private void Init()
        {
            _recordButton = new Environment.UI.Toolbar.Button("Record", Common.Icon.Get("Play"))
            {
                OnClick = RecordOnClick,
                Enabled = true
            };

            _stopButton = new Environment.UI.Toolbar.Button("Stop", Common.Icon.Get("Stop"))
            {
                OnClick = StopOnClick,
                Enabled = false
            };
            _exportButton = new Environment.UI.Toolbar.Button("Export", Common.Icon.Get("Excel"))
            {
                OnClick = ExportOnClick,
                Enabled = false
            };

            Environment.UI.Toolbar.Add(_recordButton, "Joints", "Data");
            Environment.UI.Toolbar.Add(_stopButton, "Joints", "Data");
            Environment.UI.Toolbar.Add(_exportButton, "Joints", "Data");
        }

        private void RecordOnClick(object sender, EventArgs e)
        {
            if (Experior.Core.Environment.Scene.Paused)
            {
                Log.Write($"Start the simulation to enable Record feature", System.Windows.Media.Colors.Orange, LogFilter.Communication);
                return;
            }

            Status = RecorderStatus.Recording;

            Record();
        }

        private void StopOnClick(object sender, EventArgs e)
        {
            if (!Experior.Core.Environment.Scene.Paused)
            {
                Experior.Core.Environment.Scene.Pause();
            }

            Status = RecorderStatus.Stopped;
            Stop();
        }

        private void ExportOnClick(object sender, EventArgs e)
        {
            Status = RecorderStatus.Exporting;

            Export();
        }

        private void SceneOnCleared()
        {
            Clean();
        }

        #endregion
    }
}
