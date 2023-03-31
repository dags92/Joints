using System;
using System.Numerics;
using System.Windows.Media;
using System.Xml.Serialization;
using Experior.Core.Loads;
using Experior.Interfaces;
using System.Linq;
using Experior.Catalog.Joints.Actuators.Motors;
using Experior.Core.Mathematics;
using Experior.Core.Parts;
using PhysX;

namespace Experior.Catalog.Joints.Assemblies.Robotics
{
    public class BasicGripper : Base
    {
        #region Fields

        private readonly BasicGripperInfo _info;

        private readonly Linear _motor;
        private readonly Rotative _rightMotor;
        private readonly Rotative _leftMotor;

        private bool flag;

        #endregion

        #region Constructor

        public BasicGripper(BasicGripperInfo info) : base(info)
        {
            _info = info;

            _motor = Linear.Create();
            Add(_motor);

            _rightMotor = Rotative.Create();
            Add(_rightMotor);

            _leftMotor = Rotative.Create();
            Add(_leftMotor);
        }

        #endregion

        #region Public Properties

        public override string Category => "Robotics";

        public override ImageSource Image => Common.Icon.Get("BasicGripper");

        #endregion

        #region Public Methods

        public override void Step(float deltatime)
        {
            if (!flag)
            {
                Links[2].LinkDynamic.Pitch = 45f.ToRadians();
                Links[3].LinkDynamic.Pitch = -45f.ToRadians();

                flag = true;
                return;
            }

            _motor.Step(deltatime);
            _rightMotor.Step(deltatime);
            _leftMotor.Step(deltatime);

            if (Joints[0] is D6Joint jointD6)
            {
                jointD6.DriveLinearVelocity = new Vector3(0, -_motor.CurrentSpeed, 0);
            }

            if (Joints[1] is D6Joint right)
            {
                right.DriveAngularVelocity = new Vector3(-_rightMotor.CurrentSpeed, 0, 0);
            }

            if (Joints[2] is D6Joint left)
            {
                left.DriveAngularVelocity = new Vector3(_leftMotor.CurrentSpeed, 0, 0);
            }

            foreach (Experior.Interfaces.ILoad item in Experior.Core.Loads.Load.Items)
            {
                if (item is Experior.Core.Loads.Load)
                {
                    Experior.Core.Loads.Load temp = item as Experior.Core.Loads.Load;
                    temp.Friction = new Experior.Core.Parts.Friction() { Coefficient = Coefficients.Custom, Dynamic = 0.5f, Static = 5 };
                }
            }
        }

        #endregion

        #region Protected Methods

        protected override void CreateLinks()
        {
            Links.Add(new Link(Load.CreateBox(0.05f, 0.05f, 0.05f, Colors.DarkRed), true));
            Links.Add(new Link(Load.CreateBox(0.1f, 0.02f, 0.2f, Colors.Gray), false));

            Links.Add(new Link(Load.CreateBox(0.1f, 0.02f, 0.15f, Colors.Blue), false));
            Links.Add(new Link(Load.CreateBox(0.1f, 0.02f, 0.15f, Colors.Green), false));

            // Positions:
            var link0 = Links[0].LinkDynamic;
            var link1 = Links[1].LinkDynamic;
            var link2 = Links[2].LinkDynamic;
            var link3 = Links[3].LinkDynamic;

            link0.Position = Position;
            link1.Position = link0.Position;
            link2.Position = link1.Position + new Vector3(0, 0, -link1.Width / 2 - link2.Width / 2);
            link3.Position = link1.Position + new Vector3(0, 0, link1.Width / 2 + link3.Width / 2);

            foreach (var temp in Links)
            {
                temp.LinkDynamic.Deletable = false;
                temp.LinkDynamic.UserDeletable = false;
            }

            LinkId.Add("Pneumatic");
            LinkId.Add("Base");
            LinkId.Add("Right");
            LinkId.Add("Left");
        }

        protected override void ConfigureLinks()
        {
            base.ConfigureLinks();

            foreach (var link in Links)
            {
                link.LinkDynamic.Friction = new Friction(){Coefficient = Coefficients.Custom, Dynamic = 0.5f, Static = 5};
            }
        }

        protected override void CreateJoints()
        {
            if (Links.Any(a => a.LinkActor == null))
            {
                Log.Write("Physics Engine Action has not been invoked properly", Colors.Red, LogFilter.Error);
                return;
            }

            // Definition of Joint Local Frames and Relative Local Frames:

            var link0 = Links[0].LinkDynamic;
            var link1 = Links[1].LinkDynamic;
            var link2 = Links[2].LinkDynamic;
            var link3 = Links[3].LinkDynamic;

            Links[2].RelativeLocalFrame = Matrix4x4.CreateTranslation(new Vector3(0, 0, -link2.Width / 2));
            Links[3].RelativeLocalFrame = Matrix4x4.CreateTranslation(new Vector3(0, 0, link3.Width / 2));

            Joints.Add(Core.Environment.Scene.PhysXScene.CreateJoint(JointType.D6, Links[0].LinkActor, Links[0].JointLocalFrame, Links[1].LinkActor, Links[1].RelativeLocalFrame));

            var rightJointFrame = Matrix4x4.CreateTranslation(new Vector3(0, 0, -link1.Width / 2));
            var leftJointFrame = Matrix4x4.CreateTranslation(new Vector3(0, 0, link1.Width / 2));

            Joints.Add(Core.Environment.Scene.PhysXScene.CreateJoint(JointType.D6, Links[1].LinkActor, rightJointFrame, Links[2].LinkActor, Links[2].RelativeLocalFrame));

            Joints.Add(Core.Environment.Scene.PhysXScene.CreateJoint(JointType.D6, Links[1].LinkActor, leftJointFrame, Links[3].LinkActor, Links[3].RelativeLocalFrame));

            Joints[0].Name = "Prismatic";
            Joints[1].Name = "Revolute";
            Joints[2].Name = "Revolute";

            foreach (var temp in Joints)
            {
                JointId.Add(temp.Name);
            }

            //Links[1].LinkDynamic.Pitch = 45f.ToRadians();
        }

        protected override void ConfigureJoints()
        {
            var count = 0;
            foreach (var item in Joints)
            {
                item.ConstraintFlags |= ConstraintFlag.Visualization;

                if (count == 0)
                {
                    if (item is PhysX.D6Joint d6)
                    {
                        d6.SetMotion(D6Axis.Y, D6Motion.Free);
                        d6.SetDrive(D6Drive.Y, new D6JointDrive(1f, 10000f, float.MaxValue, false));
                    }
                }
                else if (count == 1 || count == 2)
                {
                    if (item is PhysX.D6Joint d6)
                    {
                        d6.SetMotion(D6Axis.Twist, D6Motion.Free);
                        d6.SetDrive(D6Drive.Twist, new D6JointDrive(1f, 10000f, float.MaxValue, false));
                    }
                }

                count++;
            }
        }

        #endregion
    }

    [Serializable, XmlInclude(typeof(BasicGripperInfo)), XmlType(TypeName = "Experior.Catalog.Joints.Assemblies.Robotics.BasicGripperInfo")]
    public class BasicGripperInfo : BaseInfo
    {
    }
}