using Experior.Core.Loads;
using Experior.Core.Properties;
using PhysX;
using System;
using System.ComponentModel;
using System.Numerics;
using System.Windows.Media;
using System.Xml.Serialization;
using Experior.Core.Properties.TypeConverter;
using Experior.Catalog.Joints.Actuators.Motors;
using System.Windows.Input;

namespace Experior.Catalog.Joints.Assemblies.Basic
{
    public class Pose : Base
    {
        #region Fields

        private readonly PoseInfo _info;

        private readonly Rotative _motor;

        #endregion

        #region Constructor

        public Pose(PoseInfo info)
            : base(info)
        {
            _info = info;

            _motor = Rotative.Create();
            Add(_motor);
        }

        #endregion

        #region Public Properties

        [Category("Position/Orientation")]
        [DisplayName("Actor 0 Translation")]
        [PropertyOrder(0)]
        [TypeConverter(typeof(Vector3MeterToMillimeter))]
        public Vector3 Actor0Translation
        {
            get => _info.Actor0Translation;
            set
            {
                _info.Actor0Translation = value;
                SetLocalFrames();
            }
        }

        [Category("Position/Orientation")]
        [DisplayName("Actor 0 Yaw")]
        [PropertyOrder(1)]
        [TypeConverter(typeof(RadiansToDegrees))]
        public float Actor0Yaw
        {
            get => _info.Actor0Yaw;
            set
            {
                _info.Actor0Yaw = value;
                SetLocalFrames();
            }
        }

        [Category("Position/Orientation")]
        [DisplayName("Actor 0 Pitch")]
        [PropertyOrder(2)]
        [TypeConverter(typeof(RadiansToDegrees))]
        public float Actor0Pitch
        {
            get => _info.Actor0Pitch;
            set
            {
                _info.Actor0Pitch = value;
                SetLocalFrames();
            }
        }

        [Category("Position/Orientation")]
        [DisplayName("Actor 0 Roll")]
        [PropertyOrder(3)]
        [TypeConverter(typeof(RadiansToDegrees))]
        public float Actor0Roll
        {
            get => _info.Actor0Roll;
            set
            {
                _info.Actor0Roll = value;
                SetLocalFrames();
            }
        }

        [Category("Position/Orientation")]
        [DisplayName("Actor 1 Translation")]
        [PropertyOrder(4)]
        [TypeConverter(typeof(Vector3MeterToMillimeter))]
        public Vector3 Actor1Translation
        {
            get => _info.Actor1Translation;
            set
            {
                _info.Actor1Translation = value;
                SetLocalFrames();
            }
        }

        [Category("Position/Orientation")]
        [DisplayName("Actor 1 Yaw")]
        [PropertyOrder(5)]
        [TypeConverter(typeof(RadiansToDegrees))]
        public float Actor1Yaw
        {
            get => _info.Actor1Yaw;
            set
            {
                _info.Actor1Yaw = value;
                SetLocalFrames();
            }
        }

        [Category("Position/Orientation")]
        [DisplayName("Actor 1 Pitch")]
        [PropertyOrder(6)]
        [TypeConverter(typeof(RadiansToDegrees))]
        public float Actor1Pitch
        {
            get => _info.Actor1Pitch;
            set
            {
                _info.Actor1Pitch = value;
                SetLocalFrames();
            }
        }

        [Category("Position/Orientation")]
        [DisplayName("Actor 1 Roll")]
        [PropertyOrder(7)]
        [TypeConverter(typeof(RadiansToDegrees))]
        public float Actor1Roll
        {
            get => _info.Actor1Roll;
            set
            {
                _info.Actor1Roll = value;
                SetLocalFrames();
            }
        }

        public override string Category => "Basic Joints";

        public override ImageSource Image => Common.Icon.Get("Pose");

        #endregion

        #region Public Methods

        public override void Step(float deltatime)
        {
            _motor.Step(deltatime);

            if (Joints[0] is RevoluteJoint revolute)
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
            Links.Add(new Link(Load.CreateBox(0.05f, 0.8f, 0.05f, Colors.Gray), false));

            Links[0].LinkDynamic.Position = Position + new Vector3(0, -0.5f, 0);
            Links[1].LinkDynamic.Position = Links[0].LinkDynamic.Position;

            LinkId.Add("Box-1");
            LinkId.Add("Box-2");
        }

        protected override void CreateJoints()
        {
            Joints.Add(Core.Environment.Scene.PhysXScene.CreateJoint(JointType.Revolute, Links[0].LinkActor, Links[0].JointLocalFrame, Links[1].LinkActor, Links[1].RelativeLocalFrame));

            Joints[0].Name = "Revolute";
            JointId.Add(Joints[0].Name);

            SetLocalFrames();
        }

        protected override void ConfigureJoints()
        {
            base.ConfigureJoints();

            if (Joints[0] is RevoluteJoint revolute)
            {
                revolute.Flags = RevoluteJointFlag.DriveEnabled;
                revolute.DriveGearRatio = 1;
            }
        }

        private void SetLocalFrames()
        {
            Links[0].JointLocalFrame = Utilities.GetPose(Actor0Translation, Actor0Yaw, Actor0Pitch, Actor0Roll);
            Links[1].RelativeLocalFrame = Utilities.GetPose(Actor1Translation, Actor1Yaw, Actor1Pitch, Actor1Roll); ;

            Utilities.SetLocalFrames(Joints[0], Links[0].JointLocalFrame, Links[1].RelativeLocalFrame);
        }

        #endregion
    }

    [Serializable, XmlInclude(typeof(PoseInfo)), XmlType(TypeName = "Experior.Catalog.Joints.Assemblies.Basic.PoseInfo")]
    public class PoseInfo : BaseInfo
    {
        public Vector3 Actor0Translation { get; set; } = Vector3.Zero;

        public float Actor0Yaw { get; set; } = 0f;

        public float Actor0Pitch { get; set; } = 0f;

        public float Actor0Roll { get; set; } = 0f;

        public Vector3 Actor1Translation { get; set; } = Vector3.Zero;

        public float Actor1Yaw { get; set; } = 0f;

        public float Actor1Pitch { get; set; } = 0f;

        public float Actor1Roll { get; set; } = 0f;
    }
}