using Experior.Core.Loads;
using Experior.Interfaces;
using PhysX;
using System;
using System.Linq;
using System.Numerics;
using System.Windows.Media;
using System.Xml.Serialization;
using Experior.Catalog.Joints.Actuators.Motors;
using Experior.Core.Mathematics;

namespace Experior.Catalog.Joints.Assemblies.Mechanisms
{
    public class MovablePendulum : Base
    {
        #region Fields

        private readonly MovablePendulumInfo _info;

        private readonly Linear _motor;

        #endregion

        #region Constructor

        public MovablePendulum(MovablePendulumInfo info) : base(info)
        {
            _info = info;

            _motor = Linear.Create();
            Add(_motor);
        }

        #endregion

        #region Public Properties

        public override string Category => "Mechanisms";

        public override ImageSource Image => Common.Icon.Get("MovablePendulum");

        #endregion

        #region Public Methods

        public override void Step(float deltatime)
        {
            _motor.Step(deltatime);

            if (Joints[0] is D6Joint jointD6)
            {
                jointD6.DriveLinearVelocity = new Vector3(0,0, _motor.CurrentSpeed);
            }
        }

        #endregion

        #region Protected Methods

        protected override void CreateLinks()
        {
            Links.Add(new Link(Load.CreateBox(0.1f, 0.1f, 0.1f, Colors.DarkRed), true));
            Links.Add(new Link(Load.CreateBox(0.3f, 0.1f, 0.3f, Colors.DarkSlateGray), false));
            Links.Add(new Link(Load.CreateBox(0.05f, 0.8f, 0.05f, Colors.Yellow), false));

            Links[0].LinkDynamic.Position = Position;
            Links[1].LinkDynamic.Position = Links[0].LinkDynamic.Position;
            Links[2].LinkDynamic.Position = Links[1].LinkDynamic.Position + new Vector3(-Links[1].LinkDynamic.Length / 2 - Links[2].LinkDynamic.Length / 2, -Links[2].LinkDynamic.Height / 2 + 0.01f, 0);

            foreach (var temp in Links)
            {
                temp.LinkDynamic.Deletable = false;
                temp.LinkDynamic.UserDeletable = false;
            }

            LinkId.Add("Motor");
            LinkId.Add("Platform");
            LinkId.Add("Bar-1");
        }

        protected override void CreateJoints()
        {
            if (Links.Any(a => a.LinkActor == null))
            {
                Log.Write("Physics Engine Action has not been invoked properly", Colors.Red, LogFilter.Error);
                return;
            }

            Links[1].JointLocalFrame = Matrix4x4.CreateTranslation(new Vector3(0, 0, 0));

            Links[2].RelativeLocalFrame = Matrix4x4.CreateTranslation(new Vector3(Links[1].LinkDynamic.Length / 2 + Links[2].LinkDynamic.Length / 2, Links[2].LinkDynamic.Height / 2 - 0.01f, 0));

            Joints.Add(Core.Environment.Scene.PhysXScene.CreateJoint(JointType.D6, Links[0].LinkActor, Links[0].JointLocalFrame, Links[1].LinkActor, Links[1].RelativeLocalFrame));

            Joints.Add(Core.Environment.Scene.PhysXScene.CreateJoint(JointType.Revolute, Links[1].LinkActor, Links[1].JointLocalFrame, Links[2].LinkActor, Links[2].RelativeLocalFrame));

            Joints[0].Name = "D6-Prismatic";
            Joints[1].Name = "Revolute";

            foreach (var temp in Joints)
            {
                JointId.Add(temp.Name);
            }
        }

        protected override void ConfigureJoints()
        {
            var count = 0;
            foreach (var item in Joints)
            {
                item.ConstraintFlags |= ConstraintFlag.Visualization;

                if (count == 0)
                {
                    if (Joints[0] is PhysX.D6Joint d6)
                    {
                        d6.SetMotion(D6Axis.Z, D6Motion.Free);
                        d6.SetDrive(D6Drive.Z, new D6JointDrive(1f, 10000f, float.MaxValue, true));
                    }
                }
                else if (count == 1)
                {
                    if (item is RevoluteJoint revolute)
                    {
                        revolute.Flags = RevoluteJointFlag.DriveFreeSpin;
                    }
                }

                count++;
            }
        }

        #endregion
    }

    [Serializable, XmlInclude(typeof(MovablePendulumInfo)), XmlType(TypeName = "Experior.Catalog.Joints.Assemblies.Mechanisms.MovablePendulumInfo")]
    public class MovablePendulumInfo : BaseInfo
    {
    }
}