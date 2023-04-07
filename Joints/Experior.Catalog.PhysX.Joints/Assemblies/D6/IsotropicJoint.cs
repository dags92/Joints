using Experior.Catalog.Joints.Actuators.Motors;
using Experior.Core.Assemblies;
using Experior.Core.Loads;
using Experior.Core.Properties;
using Experior.Interfaces;
using PhysX;
using System;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Serialization;

namespace Experior.Catalog.Joints.Assemblies.D6
{
    public class IsotropicJoint : Base
    {
        #region Fields

        private readonly IsotropicJointInfo _info;

        private readonly Actuators.Motors.Rotative _motor;

        private bool flagY, flagZ;

        #endregion

        #region Constructor

        public IsotropicJoint(IsotropicJointInfo info)
            : base(info)
        {
            _info = info;

            _motor = Rotative.Create();
            Add(_motor);
        }

        #endregion

        #region Public Properties

        [Category("Motion")]
        [DisplayName("Stiffness")]
        [PropertyOrder(0)]
        public float Stiffness
        {
            get => _info.Stiffness;
            set
            {
                _info.Stiffness = value;
                UpdateDrive();
            }
        }

        [Category("Motion")]
        [DisplayName("Damping")]
        [PropertyOrder(1)]
        public float Damping
        {
            get => _info.Damping;
            set
            {
                _info.Damping = value;
                UpdateDrive();
            }
        }

        [Category("Motion")]
        [DisplayName("Acceleration")]
        [PropertyOrder(2)]
        public bool Acceleration
        {
            get => _info.Acceleration;
            set
            {
                _info.Acceleration = value;
                UpdateDrive();
            }
        }

        public override string Category => "D6 Joints";

        public override ImageSource Image => Common.Icon.Get("IsotropicJoint");

        #endregion

        #region Public Methods

        public override void Step(float deltatime)
        {
            _motor.Step(deltatime);

            if (Joints[0] is D6Joint jointD6)
            {
                jointD6.DriveAngularVelocity = new Vector3(0, flagY ? _motor.CurrentSpeed * 1.5f : 0f, flagZ ? _motor.CurrentSpeed : 0f);
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

                case Key.Z:

                    flagZ = !flagZ;

                    return;

                case Key.Y:

                    flagY = !flagY;

                    return;
            }
        }

        #endregion

        #region Protected Methods

        protected override void CreateLinks()
        {
            Links.Add(new Link(Load.CreateBox(0.1f, 0.1f, 0.1f, Colors.DarkRed), true));

            const float linkL = 0.25f;
            Links.Add(new Link(Load.CreateBox(linkL, linkL, linkL, Colors.Gray), false));

            Links[0].LinkDynamic.Position = Position;
            Links[1].LinkDynamic.Position = Links[0].LinkDynamic.Position + new Vector3(0, -Links[0].LinkDynamic.Height / 2 - Links[1].LinkDynamic.Height / 2 - 0.1f, 0);

            foreach (var temp in Links)
            {
                temp.LinkDynamic.Deletable = false;
                temp.LinkDynamic.UserDeletable = false;
            }

            LinkId.Add("Motor");
            LinkId.Add("Box");
        }

        protected override void CreateJoints()
        {
            if (Links.Any(a => a.LinkActor == null))
            {
                Log.Write("Physics Engine Action has not been invoked properly", Colors.Red, LogFilter.Error);
                return;
            }

            Links[1].RelativeLocalFrame = Matrix4x4.CreateTranslation(0, Links[0].LinkDynamic.Height / 2 + Links[1].LinkDynamic.Height / 2 + 0.1f, 0);

            Joints.Add(Core.Environment.Scene.PhysXScene.CreateJoint(JointType.D6, Links[0].LinkActor, Links[0].JointLocalFrame, Links[1].LinkActor, Links[1].RelativeLocalFrame));

            Joints[0].Name = "Swing1";
            JointId.Add(Joints[0].Name);
        }

        protected override void ConfigureJoints()
        {
            if (Joints[0] is PhysX.D6Joint d6)
            {
                Joints[0].ConstraintFlags |= ConstraintFlag.Visualization;

                d6.SetMotion(D6Axis.Swing1, D6Motion.Free);
                d6.SetMotion(D6Axis.Swing2, D6Motion.Free);

                d6.SetDrive(D6Drive.Swing, new D6JointDrive(Stiffness, Damping, float.MaxValue, Acceleration));
            }
        }

        #endregion

        #region Private Methods

        private void UpdateDrive()
        {
            Experior.Core.Environment.InvokeIfRequired(() =>
            {
                if (Joints[0] is PhysX.D6Joint d6)
                {
                    d6.SetDrive(D6Drive.Swing, new D6JointDrive(Stiffness, Damping, float.MaxValue, Acceleration));
                }
            });
        }

        #endregion
    }

    [Serializable, XmlInclude(typeof(IsotropicJointInfo)), XmlType(TypeName = "Experior.Catalog.Joints.Assemblies.D6.IsotropicJointInfo")]
    public class IsotropicJointInfo : BaseInfo
    {
        public bool Acceleration { get; set; } = true;

        public float Stiffness { get; set; } = 1f;

        public float Damping { get; set; } = 10000f;
    }
}