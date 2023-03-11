using System;
using System.ComponentModel;
using System.Numerics;
using System.Windows.Media;
using System.Xml.Serialization;
using Experior.Core.Assemblies;
using Experior.Core.Mathematics;
using Experior.Core.Parts;
using Experior.Core.Properties.TypeConverter;
using Experior.Core.Properties;
using PhysX;
using Experior.Core.Loads;

namespace Experior.Catalog.Joints.Assemblies.BasicJoints
{
    public class Revolute : Assembly
    {
        #region Fields

        private readonly RevoluteInfo _info;

        private readonly Experior.Core.Parts.Box _link1;
        private Experior.Core.Loads.Load _link2;
        private PhysX.Joint _joint1;

        #endregion

        #region Constructor

        public Revolute(RevoluteInfo info) : base(info)
        {
            _info = info;

            _link1 = new Experior.Core.Parts.Box(Colors.Blue, 0.15f, 0.15f, 0.15f)
            {
                Rigid = true
            };
            Add(_link1);
        }

        #endregion

        #region Public Properties

        [Browsable(true)]
        [Category("Parameters")]
        [DisplayName("Length")]
        [TypeConverter(typeof(FloatMeterToMillimeter))]
        [PropertyOrder(0)]
        public float Length1
        {
            get => _info.Length1;
            set
            {
                if (value <= 0 || _info.Length1.IsEffectivelyEqual(value))
                {
                    return;
                }

                _info.Length1 = value;
                Experior.Core.Environment.Invoke(CreateJoint);
            }
        }

        [Browsable(true)]
        [Category("Parameters")]
        [DisplayName("Weight")]
        [TypeConverter(typeof(Weight))]
        [PropertyOrder(1)]
        public float LinkWeight
        {
            get => _info.LinkWeight;
            set
            {
                if (value <= 0 || _info.LinkWeight.IsEffectivelyEqual(value))
                {
                    return;
                }

                _info.LinkWeight = value;
                Experior.Core.Environment.Invoke(CreateJoint);
            }
        }

        [Browsable(true)]
        [Category("Parameters")]
        [DisplayName("Drive Velocity")]
        [PropertyOrder(2)]
        public float DriveVelocity
        {
            get => _info.DriveVelocity;
            set
            {
                if (value <= 0 || _info.DriveVelocity.IsEffectivelyEqual(value))
                {
                    return;
                }

                _info.DriveVelocity = value;
                Experior.Core.Environment.Invoke(ConfigureJoint);
            }
        }

        [Browsable(true)]
        [Category("Parameters")]
        [DisplayName("Drive Gear Ratio")]
        [PropertyOrder(3)]
        public float DriveGearRatio
        {
            get => _info.DriveGearRatio;
            set
            {
                if (value <= 0 || _info.DriveGearRatio.IsEffectivelyEqual(value))
                {
                    return;
                }

                _info.DriveGearRatio = value;
                Experior.Core.Environment.Invoke(ConfigureJoint);
            }
        }

        public override string Category => "Basic Joints";

        public override ImageSource Image => Common.Icon.Get("RevoluteJoint");

        #endregion

        #region Public Methods

        public override void Inserted()
        {
            base.Inserted();
            Experior.Core.Environment.Invoke(CreateJoint);
        }

        #endregion

        #region Private Methods

        private void CreateJoint()
        {
            if (_joint1 != null)
            {
                Experior.Core.Environment.InvokePhysicsAction(_joint1.Dispose);
            }

            if (_link2 != null)
            {
                Experior.Core.Loads.Load.Delete(_link2);
                _link2.Dispose();
                _link2 = null;
            }

            _link2 = Experior.Core.Loads.Load.CreateBox(0.2f, 0.2f, 0.2f, Colors.BlueViolet);
            _link2.Position = Position + new Vector3(0, -Length1, 0);
            ConfigureLoad(_link2);

            Experior.Core.Environment.InvokePhysicsAction(() =>
            {
                RigidDynamic link1Actor = _link1.Actor;
                RigidDynamic link2Actor = ((Dynamic)_link2.Part).Actor;

                if (link1Actor == null || link2Actor == null)
                {
                    return;
                }

                var link1Frame = Matrix4x4.Identity;
                var link2Frame = Matrix4x4.Identity;
                link2Frame.M42 = Length1;

                _joint1 = Core.Environment.Scene.PhysXScene.CreateJoint(JointType.Revolute, link1Actor, link1Frame, link2Actor, link2Frame);

                ConfigureJoint();
            });
        }

        private void ConfigureJoint()
        {
            if (_joint1 is PhysX.RevoluteJoint revolute)
            {
                revolute.Flags = RevoluteJointFlag.DriveEnabled;

                if (revolute.Flags == RevoluteJointFlag.DriveEnabled)
                {
                    revolute.DriveVelocity = DriveVelocity;
                    revolute.DriveGearRatio = DriveGearRatio;
                }
            }
        }

        private void ConfigureLoad(Load load)
        {
            load.Weight = LinkWeight;
            load.Part.MinPositionIterations = 20;
            load.Part.MinVelocityIterations = 5;
        }

        #endregion
    }

    [Serializable, XmlInclude(typeof(RevoluteInfo)), XmlType(TypeName = "Experior.Catalog.Joints.Assemblies.BasicJoints.RevoluteInfo")]
    public class RevoluteInfo : AssemblyInfo
    {
        public float Length1 { get; set; } = 0.5f;

        public float LinkWeight { get; set; } = 2f;

        public float DriveVelocity { get; set; } = 0.4f;

        public float DriveGearRatio { get; set; } = 1f;
    }
}
