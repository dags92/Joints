using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using Experior.Catalog.Joints.Assemblies;
using Experior.Core.Assemblies;
using Experior.Interfaces;
using Experior.Plugin.Joints.DataHandler;
using Microsoft.Office.Interop.Excel;
using Environment = Experior.Core.Environment;

namespace Experior.Plugin.Joints.Recorder
{
    public class JointData
    {
        #region Fields

        private bool _record;

        private Experior.Core.Assemblies.Assembly _assembly;
        private IMechanism _mechanism;
        private readonly List<JointModel> _jointsData = new List<JointModel>();

        private readonly List<float> _time = new List<float>();

        #endregion

        #region Events

        public EventHandler Exported;

        public EventHandler AssemblyDeleted;

        #endregion

        #region Public Methods

        public bool Record()
        {
            try
            {
                _assembly = (Assembly)Assembly.Items.First(a => a is IMechanism); //TODO: WHAT IF MULTIPLE ASSEMBLIES ?
            }
            catch (Exception e)
            {
                Log.Write("No assembly containing PhysX Joints has been found", Colors.Orange, LogFilter.Communication);
                return false;
            }

            _mechanism = _assembly as IMechanism;

            if (_mechanism == null)
            {
                return false;
            }

            foreach (var name in _mechanism.JointId)
            {
                _jointsData.Add(new JointModel(name));
            }

            _assembly.OnDisposing += AssemblyOnDisposing;
            Experior.Core.Environment.Scene.OnStep += SceneOnStep;

            _record = true;

            return true;
        }

        public void Stop()
        {
            _record = false;
        }

        public void Export()
        {
            Experior.Core.Environment.Scene.OnStep -= SceneOnStep; //TODO: DOUBLE CHECK THIS...

            try
            {
                if (!Environment.Scene.Paused)
                {
                    Environment.Scene.Pause();
                }

                var excel = new ExcelWriter();
                var workBook = excel.CreateWorkBook();

                if (!(workBook.Worksheets.Item[1] is Worksheet workSheet))
                {
                    return;
                }

                var row = 1;
                var col = 1;
                var count = 0;
                foreach (var model in _jointsData)
                {
                    if (count == 0)
                    {
                        workSheet.Cells[row, col] = "Time (s)";

                        foreach (var time in _time)
                        {
                            workSheet.Cells[row + 1, col] = time;
                            row++;
                        }
                    }
                    row = 1;

                    // Headers:
                    workSheet.Cells[row, col + 1] = $"Joint: {model.Name}";

                    workSheet.Cells[row + 1, col + 1] = "Linear Velocity (X)";
                    workSheet.Cells[row + 1, col + 2] = "Linear Velocity (Y)";
                    workSheet.Cells[row + 1, col + 3] = "Linear Velocity (Z)";

                    workSheet.Cells[row + 1, col + 4] = "Angular Velocity (X)";
                    workSheet.Cells[row + 1, col + 5] = "Angular Velocity (Y)";
                    workSheet.Cells[row + 1, col + 6] = "Angular Velocity (Z)";

                    // Data:
                    foreach (var linearV in model.GetLinearVelocity())
                    { 
                        workSheet.Cells[row + 2, col + 1] = linearV.X;
                        workSheet.Cells[row + 2, col + 2] = linearV.Y;
                        workSheet.Cells[row + 2, col + 3] = linearV.Z;

                        row++;
                    }
                    row = 1;

                    foreach (var angularV in model.GetAngularVelocity())
                    {
                        workSheet.Cells[row + 2, col + 4] = angularV.X;
                        workSheet.Cells[row + 2, col + 5] = angularV.Y;
                        workSheet.Cells[row + 2, col + 6] = angularV.Z;

                        row++;
                    }

                    row = 1;
                    col += 7;
                    count++;
                }

                excel.Generate($"JointData");
            }
            catch (Exception exp)
            {
                Log.Write(exp);
                throw;
            }

            Clean();
            Exported?.Invoke(this, EventArgs.Empty);
        }

        public void Clean()
        {
            _jointsData.Clear();
            _time.Clear();
            _assembly = null;
            _mechanism = null;

            _record = false;
        }

        #endregion

        #region Private Methods

        private void SceneOnStep(float deltaTime)
        {
            if (!_record)
            {
                return;
            }

            for (var i = 0; i < _mechanism.JointId.Count; i++)
            {
                _jointsData[i].AddLinearVelocity(_mechanism.GetLinearVelocity(i));
                _jointsData[i].AddAngularVelocity(_mechanism.GetAngularVelocity(i));
            }

            _time.Add((float)Experior.Core.Environment.Time.Simulated);
        }

        private void AssemblyOnDisposing(Assembly sender)
        {
            _assembly.OnDisposing -= AssemblyOnDisposing;

            Clean();
            AssemblyDeleted?.Invoke(this, EventArgs.Empty);
        }

        #endregion
    }
}
