using System;
using System.Linq;
using System.Numerics;
using System.Windows.Input;
using System.Windows.Media;

using PhysX;
using System.Xml.Serialization;
using Experior.Catalog.Joints.Actuators.Motors;
using Experior.Core.Loads;
using Experior.Interfaces;
using Colors = System.Windows.Media.Colors;
using Environment = Experior.Core.Environment;

namespace Experior.Catalog.Joints.Assemblies.Mechanisms
{
    /// <summary>
    /// Class <c>SliderCrank</c> depicts the dynamics of a slider-crank mechanism using D6 and Basic joints from PhysX 3.4.1
    /// </summary>
    public class SliderCrank : Base
    {
        #region Fields

        private readonly SliderCrankInfo _info;

        private readonly Actuators.Motors.Rotative _motor;

        #endregion

        #region Constructor

        public SliderCrank(SliderCrankInfo info) : base(info)
        {
            _info = info;

            var sphere = new Experior.Core.Parts.Sphere(Colors.Black, 0.015f, 8);
            Add(sphere, new Vector3(0f, 0.2f, 0f));

            _motor = Rotative.Create();
            Add(_motor);
        }

        #endregion

        #region Public Properties

        public override string Category => "Mechanisms";

        public override ImageSource Image => Common.Icon.Get("Slider-Crank");

        #endregion

        #region Public Methods

        public override void Step(float deltatime)
        {
            _motor.Step(deltatime);

            if (Joints[0] is D6Joint jointD6)
            {
                jointD6.DriveAngularVelocity = new Vector3(_motor.CurrentSpeed, 0, 0);
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

        #region Private Methods

        protected override void CreateLinks()
        {
            // Motor:
            Links.Add(new Link(Load.CreateBox(0.1f, 0.1f, 0.1f, Colors.DarkRed), true));

            // Links:
            const float linkL = 0.025f;

            Links.Add(new Link(Load.CreateBox(linkL, linkL, 0.4f, Colors.Gray), false));
            Links.Add(new Link(Load.CreateBox(linkL, linkL, 0.8f, Colors.DarkGray), false));
            Links.Add(new Link(Load.CreateBox(0.05f, 0.05f, 0.05f, Colors.LightGray), false));

            // Piston chamber:
            Links.Add(new Link(Load.CreateBox(0.05f, 0.05f, 0.05f, Colors.Blue), true));
            //Links[4].LinkDynamic.Position = Position;

            // Positions:
            Links[0].LinkDynamic.Position = Position;
            Links[1].LinkDynamic.Position = Links[0].LinkDynamic.Position + new Vector3(-Links[0].LinkDynamic.Length / 2 - Links[1].LinkDynamic.Length / 2, 0, -Links[1].LinkDynamic.Width / 2 + 0.02f);

            Links[2].LinkDynamic.Position = Links[1].LinkDynamic.Position + new Vector3(-Links[1].LinkDynamic.Length / 2 - Links[2].LinkDynamic.Length / 2, 0, -Links[1].LinkDynamic.Width / 2 - Links[2].LinkDynamic.Width / 2 + 0.05f);

            Links[3].LinkDynamic.Position = Links[2].LinkDynamic.Position + new Vector3(0, 0, -Links[2].LinkDynamic.Width / 2 - Links[3].LinkDynamic.Width / 2);

            Links[4].LinkDynamic.Position = Links[3].LinkDynamic.Position + new Vector3(0, 0, -Links[3].LinkDynamic.Width / 2 - Links[4].LinkDynamic.Width / 2 - 0.2f);

            foreach (var temp in Links)
            {
                temp.LinkDynamic.Deletable = false;
                temp.LinkDynamic.UserDeletable = false;
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

            Links[1].RelativeLocalFrame = Matrix4x4.CreateTranslation(Links[0].LinkDynamic.Length / 2 + Links[1].LinkDynamic.Length / 2, 0, -Links[1].LinkDynamic.Width / 2 + Links[0].LinkDynamic.Width / 2);

            Links[1].JointLocalFrame = Matrix4x4.CreateTranslation(new Vector3(0, 0, Links[1].LinkDynamic.Width / 2 - 0.015f));

            Links[2].RelativeLocalFrame = Matrix4x4.CreateTranslation(new Vector3(Links[1].LinkDynamic.Length / 2 + Links[2].LinkDynamic.Length / 2, 0, -Links[2].LinkDynamic.Width / 2 + 0.015f));

            Links[2].JointLocalFrame = Matrix4x4.CreateTranslation(new Vector3(0, 0, Links[2].LinkDynamic.Width / 2));

            Links[4].JointLocalFrame = Matrix4x4.CreateRotationY((float)Math.PI / 2);

            // Creation of Joints:

            Joints.Add(Environment.Scene.PhysXScene.CreateJoint(JointType.D6, Links[0].LinkActor, Links[0].JointLocalFrame, Links[1].LinkActor, Links[1].RelativeLocalFrame));

            Joints.Add(Environment.Scene.PhysXScene.CreateJoint(JointType.Revolute, Links[1].LinkActor, Links[1].JointLocalFrame, Links[2].LinkActor, Links[2].RelativeLocalFrame));

            Joints.Add(Environment.Scene.PhysXScene.CreateJoint(JointType.Spherical, Links[2].LinkActor, Links[2].JointLocalFrame, Links[3].LinkActor, Links[3].RelativeLocalFrame));

            Joints.Add(Environment.Scene.PhysXScene.CreateJoint(JointType.Prismatic, Links[4].LinkActor, Links[4].JointLocalFrame, Links[3].LinkActor, Links[3].RelativeLocalFrame));

            Joints[0].Name = "D6";
            Joints[1].Name = "Revolute";
            Joints[2].Name = "Spherical";
            Joints[3].Name = "Prismatic";

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
                    if (item is D6Joint d6)
                    {
                        var stiffness = 1f;
                        var damping = 10000f;

                        d6.SetMotion(D6Axis.Twist, D6Motion.Free);
                        d6.SetDrive(D6Drive.Twist, new D6JointDrive(stiffness, damping, float.MaxValue, true));
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

    [Serializable, XmlInclude(typeof(SliderCrankInfo)), XmlType(TypeName = "Experior.Catalog.PhysX.Joints.Assemblies.Mechanisms.SliderCrankInfo")]
    public class SliderCrankInfo : BaseInfo
    {

    }
}
