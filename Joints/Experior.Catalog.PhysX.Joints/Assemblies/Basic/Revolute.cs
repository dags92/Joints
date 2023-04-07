using System;
using System.ComponentModel;
using System.Numerics;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Serialization;
using Experior.Catalog.Joints.Actuators.Motors;
using Experior.Core.Loads;
using Experior.Core.Properties;
using PhysX;
using Colors = System.Windows.Media.Colors;
using Environment = Experior.Core.Environment;

namespace Experior.Catalog.Joints.Assemblies.Basic
{
    public class Revolute : Base
    {
        #region Fields

        private readonly RevoluteInfo _info;

        private readonly Rotative _motor;

        #endregion

        #region Constructor

        public Revolute(RevoluteInfo info) : base(info)
        {
            _info = info;

            _motor = Rotative.Create();
            Add(_motor);
        }

        #endregion

        #region Public Properties

        [Category("Motion")]
        [DisplayName("Type")]
        [PropertyOrder(0)]
        public AuxiliaryData.RevoluteDriveTypes DriveType
        {
            get => _info.Drive;
            set
            {
                _info.Drive = value;
                Rebuild();
            }
        }

        [Category("Motion")]
        [DisplayName("Enable Limits")]
        [PropertyOrder(1)]
        public bool EnableLimits
        {
            get => _info.EnableLimits;
            set
            {
                _info.EnableLimits = value;
                UpdateDrive();

                Experior.Core.Environment.Properties.Refresh();
            }
        }

        [Category("Motion")]
        [DisplayName("Lower Limit")]
        [PropertyOrder(2)]
        [PropertyAttributesProvider("DynamicLimits")]
        public float LowerLimit
        {
            get => _info.LowerLimit;
            set
            {
                if (value >= _info.UpperLimit)
                {
                    return;
                }

                _info.LowerLimit = value;
                UpdateDrive();
            }
        }

        [Category("Motion")]
        [DisplayName("Upper Limit")]
        [PropertyOrder(3)]
        [PropertyAttributesProvider("DynamicLimits")]
        public float UpperLimit
        {
            get => _info.UpperLimit;
            set
            {
                if (value <= _info.LowerLimit)
                {
                    return;
                }

                _info.UpperLimit = value;
                UpdateDrive();
            }
        }

        [Category("Motion")]
        [DisplayName("Stiffness")]
        [PropertyOrder(4)]
        [PropertyAttributesProvider("DynamicLimits")]
        public float Stiffness
        {
            get => _info.SpringLimit.Stiffness;
            set
            {
                _info.SpringLimit.Stiffness = value;
                UpdateDrive();
            }
        }

        [Category("Motion")]
        [DisplayName("Damping")]
        [PropertyOrder(5)]
        [PropertyAttributesProvider("DynamicLimits")]
        public float Damping
        {
            get => _info.SpringLimit.Damping;
            set
            {
                _info.SpringLimit.Damping = value;
                UpdateDrive();
            }
        }

        public override string Category => "Basic Joints";

        public override ImageSource Image => Common.Icon.Get("Twist");

        #endregion

        #region Public Methods

        public override void Step(float deltatime)
        {
            _motor.Step(deltatime);

            if (Joints[0] is RevoluteJoint revolute && DriveType == AuxiliaryData.RevoluteDriveTypes.Motorized)
            {
                revolute.DriveVelocity = _motor.CurrentSpeed;
            }
        }

        public override void KeyDown(KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.A:

                    if (_motor.Running)
                    {
                        _motor.Stop();
                    }
                    else
                    {
                        _motor.Start();
                    }

                    return;
            }
        }

        #endregion

        #region Protected Methods

        protected override void CreateLinks()
        {
            Links.Add(new Link(Load.CreateBox(0.1f, 0.1f, 0.1f, Colors.DarkRed), true));
            Links.Add(new Link(Load.CreateBox(0.025f, 0.025f, 0.4f, Colors.Gray), false));

            Links[0].LinkDynamic.Position = Position + new Vector3(0, -0.5f, 0);
            Links[1].LinkDynamic.Position = Links[0].LinkDynamic.Position + new Vector3(-Links[0].LinkDynamic.Length / 2 - Links[1].LinkDynamic.Length / 2, 0, -Links[1].LinkDynamic.Width / 2 + 0.02f);

            LinkId.Add("Motor");
            LinkId.Add("Rod");
        }

        protected override void CreateJoints()
        {
            Links[1].RelativeLocalFrame = Matrix4x4.CreateTranslation(Links[0].LinkDynamic.Length / 2 + Links[1].LinkDynamic.Length / 2, 0, -Links[1].LinkDynamic.Width / 2 + Links[0].LinkDynamic.Width / 2);

            Joints.Add(Environment.Scene.PhysXScene.CreateJoint(JointType.Revolute, Links[0].LinkActor, Links[0].JointLocalFrame, Links[1].LinkActor, Links[1].RelativeLocalFrame));
            
            Joints[0].Name = "Twist";
            JointId.Add(Joints[0].Name);
        }

        protected override void ConfigureJoints()
        {
            base.ConfigureJoints();
            UpdateDrive();
        }

        #endregion

        #region Private Methods

        private void UpdateDrive()
        {
            if (!(Joints[0] is RevoluteJoint revolute))
            {
                return;
            }
            
            revolute.Flags = Utilities.GetRevoluteJointFlag(DriveType);
            revolute.Limit = new JointAngularLimitPair(LowerLimit, UpperLimit, new Spring(Stiffness, Damping));
            revolute.DriveGearRatio = 1;
        }

        #endregion
    }

    [Serializable, XmlInclude(typeof(RevoluteInfo)), XmlType(TypeName = "Experior.Catalog.Joints.Assemblies.Basic.TwistInfo")]
    public class RevoluteInfo : BaseInfo
    {
        public AuxiliaryData.RevoluteDriveTypes Drive { get; set; }

        public bool EnableLimits { get; set; } = false;

        public float LowerLimit { get; set; } = -1f;

        public float UpperLimit { get; set; } = 1f;

        public SpringLimit SpringLimit { get; set; } = new SpringLimit();
    }
}