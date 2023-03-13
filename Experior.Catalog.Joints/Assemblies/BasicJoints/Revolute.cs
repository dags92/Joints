using System;
using System.ComponentModel;
using System.Numerics;
using System.Windows.Media;
using System.Xml.Serialization;
using Experior.Core.Mathematics;
using Experior.Core.Parts;
using Experior.Core.Properties;
using PhysX;
using Experior.Core.Loads;

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

        [Browsable(true)]
        [Category("Joint")]
        [DisplayName("Drive Velocity")]
        [PropertyOrder(0)]
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

        [Browsable(true)]
        [Category("Joint")]
        [DisplayName("Drive Gear Ratio")]
        [PropertyOrder(1)]
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

        public override string Category => "Basic Joints";

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
                revolute.Flags = RevoluteJointFlag.DriveEnabled;
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
    }
}
