using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Windows.Media;
using Experior.Catalog.Joints.Assemblies;
using Experior.Core.Assemblies;
using Experior.Core.Mathematics;
using Experior.Interfaces;
using Experior.Plugin.Joints.DataHandler;
using Environment = Experior.Core.Environment;

namespace Experior.Plugin.Joints.Recorder
{
    public class MechanismData
    {
        #region Fields

        private bool _record;

        private Experior.Core.Assemblies.Assembly _assembly;

        private IMechanism _mechanism;
        private readonly List<JointModel> _jointsData = new List<JointModel>();
        private readonly List<LinkModel> _linksData = new List<LinkModel>();

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

            foreach (var name in _mechanism.LinkId)
            {
                _linksData.Add(new LinkModel(name));
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

                var excel = new ExcelWriter("MechanismData");
                CreateJointsData(excel);
                CreateLinksData(excel);
                excel.Generate();
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
            _linksData.Clear();
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
                _jointsData[i].AddLinearVelocity(_mechanism.GetJointLinearVelocity(i));
                _jointsData[i].AddAngularVelocity(_mechanism.GetJointAngularVelocity(i));

                _jointsData[i].AddLinearForce(_mechanism.GetJointLinearForce(i));
                _jointsData[i].AddAngularForce(_mechanism.GetJointAngularForce(i));
            }

            for (var i = 0; i < _mechanism.LinkId.Count; i++)
            {
                _linksData[i].AddLinearVelocity(_mechanism.GetLinkLinearVelocity(i));
                _linksData[i].AddAngularVelocity(_mechanism.GetLinkAngularVelocity(i));
                _linksData[i].AddLocalPosition(_mechanism.GetLinkLocalPosition(i));
                _linksData[i].AddLocalOrientation(_mechanism.GetLocalLinkOrientation(i));
            }
        }

        private void AssemblyOnDisposing(Assembly sender)
        {
            if (_assembly != null)
            {
                _assembly.OnDisposing -= AssemblyOnDisposing;
            }

            Clean();
            AssemblyDeleted?.Invoke(this, EventArgs.Empty);
        }

        private void CreateJointsData(ExcelWriter excel)
        {
            foreach (var data in _jointsData)
            {
                var info = new List<List<string>>
                {
                    new List<string>() { $"Joint: {data.Name}" },
                    GetJointsDataHeaders()
                };

                // Data:
                var linearVelocity = data.GetLinearVelocity();
                var angularVelocity = data.GetAngularVelocity();
                var linearForce = data.GetLinearForce();
                var angularForce = data.GetAngularForce();

                for (var i = 0; i < linearVelocity.Count; i++)
                {
                    var row = new List<string>();

                    AddVectorComponents(row, linearVelocity[i]);
                    AddVectorComponents(row, angularVelocity[i]);
                    AddVectorComponents(row, linearForce[i]);
                    AddVectorComponents(row, angularForce[i]);

                    info.Add(row);
                }

                excel.CreateSheet(data.Name, info);
            }
        }

        private void CreateLinksData(ExcelWriter excel)
        {
            foreach (var data in _linksData)
            {
                var info = new List<List<string>>
                {
                    new List<string>() { $"Link: {data.Name}" },
                    GetLinkDataHeaders()
                };

                // Data:
                var linearVelocity = data.GetLinearVelocity();
                var angularVelocity = data.GetAngularVelocity();
                var localPosition = data.GetLocalPosition();
                var localOrientation = new List<Vector3>();

                foreach (var orientation in data.GetLocalOrientation())
                {
                    var yaw = Trigonometry.Yaw(orientation);
                    var pitch = Trigonometry.Pitch(orientation);
                    var roll = Trigonometry.Roll(orientation);

                    localOrientation.Add(new Vector3(yaw, pitch, roll));
                }

                for (var i = 0; i < linearVelocity.Count; i++)
                {
                    var row = new List<string>();

                    AddVectorComponents(row, linearVelocity[i]);
                    AddVectorComponents(row, angularVelocity[i]);

                    AddVectorComponents(row, localPosition[i]);
                    AddVectorComponents(row, localOrientation[i]);

                    info.Add(row);
                }

                excel.CreateSheet(data.Name, info);
            }
        }

        private List<string> GetJointsDataHeaders()
        {
            return new List<string>()
            {
                "Linear Velocity (X)",
                "Linear Velocity (Y)",
                "Linear Velocity (Z)",

                "Angular Velocity (X)",
                "Angular Velocity (Y)",
                "Angular Velocity (Z)",

                "Linear Force (X)",
                "Linear Force (Y)",
                "Linear Force (Z)",

                "Angular Force (X)",
                "Angular Force (Y)",
                "Angular Force (Z)"
            };
        }

        private List<string> GetLinkDataHeaders()
        {
            return new List<string>()
            {
                "Linear Velocity (X)",
                "Linear Velocity (Y)",
                "Linear Velocity (Z)",

                "Angular Velocity (X)",
                "Angular Velocity (Y)",
                "Angular Velocity (Z)",

                "Local Position (X)",
                "Local Position (Y)",
                "Local Position (Z)",

                "Local Yaw",
                "Local Pitch",
                "Local Roll"
            };
        }

        private void AddVectorComponents(List<string> values, Vector3 vector)
        {
            values.Add(vector.X.ToString(CultureInfo.InvariantCulture));
            values.Add(vector.Y.ToString(CultureInfo.InvariantCulture));
            values.Add(vector.Z.ToString(CultureInfo.InvariantCulture));
        }

        #endregion
    }
}
