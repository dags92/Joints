using System;
using System.ComponentModel;
using System.Windows.Media;
using System.Xml.Serialization;
using Experior.Core.Mathematics;
using Experior.Core.Properties;
using PhysX;

namespace Experior.Catalog.Joints.Assemblies.BasicJoints
{
    public class Revolute : Base
    {
        #region Fields

        private readonly RevoluteInfo _info;

        #endregion

        #region Constructor

        public Revolute(RevoluteInfo info) : base(info)
        {
            _info = info;
        }

        #endregion

        #region Public Properties

        [Category("Motion")]
        [DisplayName("Enable Drive")]
        [PropertyOrder(0)]
        public bool EnableDrive
        {
            get => _info.EnableDrive;
            set
            {
                if (value == _info.EnableDrive)
                {
                    return;
                }

                _info.EnableDrive = value;
                Experior.Core.Environment.InvokePhysicsAction(UpdateJointProperties);
            }
        }

        [Category("Motion")]
        [DisplayName("Enable Free Spin")]
        [PropertyOrder(1)]
        public bool EnableFreeSpin
        {
            get => _info.EnableFreeSpin;
            set
            {
                if (value == _info.EnableFreeSpin)
                {
                    return;
                }

                _info.EnableFreeSpin = value;
                Experior.Core.Environment.InvokePhysicsAction(UpdateJointProperties);
            }
        }

        [Category("Motion")]
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
                Experior.Core.Environment.InvokePhysicsAction(UpdateJointProperties);
            }
        }

        [Category("Joint")]
        [DisplayName("Motion Gear Ratio")]
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
                Experior.Core.Environment.InvokePhysicsAction(UpdateJointProperties);
            }
        }

        public override ImageSource Image => Common.Icon.Get("RevoluteJoint");

        #endregion

        #region Protected Methods

        protected override void CreateLinks()
        {
            if (StaticLink == null)
            {
                StaticLink = new Experior.Core.Parts.Box(Colors.DimGray, 0.1f, 0.1f, 0.1f)
                {
                    Rigid = true
                };
                Add(StaticLink);
            }

            if (DynamicLink == null)
            {
                DynamicLink = Experior.Core.Loads.Load.CreateBox(Length, Height, Width, Colors.DarkSlateGray);
            }
        }

        protected override void UpdateJointProperties()
        {
            base.UpdateJointProperties();

            if (Joint is PhysX.RevoluteJoint revolute)
            {
                var flags = EnableDrive ? RevoluteJointFlag.DriveEnabled : 0;
                if (EnableFreeSpin)
                {
                    flags |= RevoluteJointFlag.DriveFreeSpin;
                } 

                revolute.Flags = flags;
                revolute.DriveVelocity = DriveVelocity;
                revolute.DriveGearRatio = DriveGearRatio;
            }
        }

        #endregion
    }

    [Serializable, XmlInclude(typeof(RevoluteInfo)), XmlType(TypeName = "Experior.Catalog.Joints.Assemblies.BasicJoints.RevoluteInfo")]
    public class RevoluteInfo : BaseInfo
    {
        public float DriveVelocity { get; set; } = 0.4f;

        public float DriveGearRatio { get; set; } = 1f;

        public bool EnableDrive { get; set; } = true;

        public bool EnableFreeSpin { get; set; }
    }
}
