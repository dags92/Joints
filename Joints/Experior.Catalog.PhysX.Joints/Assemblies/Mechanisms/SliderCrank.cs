using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Windows.Media;

using PhysX;
using System.Xml.Serialization;
using Experior.Catalog.Joints.Actuators.Motors;
using Experior.Core.Assemblies;
using Experior.Core.Loads;
using Experior.Interfaces;
using Colors = System.Windows.Media.Colors;
using Environment = Experior.Core.Environment;

namespace Experior.Catalog.Joints.Assemblies.Mechanisms
{
    /// <summary>
    /// Class <c>SliderCrank</c> depicts the dynamics of a slider-crank mechanism using D6 and Basic joints from PhysX 3.4.1
    /// </summary>
    public class SliderCrank : Assembly
    {
        #region Fields

        private readonly SliderCrankInfo _info;

        private readonly List<Link> _links = new List<Link>();
        private readonly Dictionary<string, Joint> _joints = new Dictionary<string, Joint>();

        private readonly Actuators.Motors.Rotative _motor;

        #endregion

        #region Constructor

        public SliderCrank(SliderCrankInfo info) : base(info)
        {
            _info = info;

            _motor = Rotative.Create();
            Add(_motor);
        }

        #endregion

        #region Public Properties

        public override string Category => "Mechanisms";

        public override ImageSource Image => Common.Icon.Get("Slider-Crank");

        #endregion

        #region Public Methods

        public override void Inserted()
        {
            base.Inserted();

            Build();
        }

        public override void Step(float deltatime)
        {
            _motor.Step(deltatime);

            if (_joints["Joint 1"] is D6Joint joint)
            {
                joint.DriveAngularVelocity = new Vector3(_motor.CurrentSpeed, 0, 0);
            }
        }

        #endregion

        #region Private Methods

        private void Build()
        {
            Environment.InvokeIfRequired(() =>
            {
                CreateLinks();

                Environment.InvokePhysicsAction(() =>
                {
                    CreateJoints();
                    ConfigureJoints();
                });
            });
        }

        private void CreateLinks()
        {
            Environment.InvokeIfRequired(() =>
            {
                // Motor:
                _links.Add(new Link(Load.CreateBox(0.1f, 0.1f, 0.1f, Colors.DarkRed), true));

                // Links:
                const float linkL = 0.025f;

                _links.Add(new Link(Load.CreateBox(linkL, linkL, 0.4f, Colors.Gray), false));
                _links.Add(new Link(Load.CreateBox(linkL, linkL, 0.8f, Colors.DarkGray), false));
                _links.Add(new Link(Load.CreateBox(0.05f, 0.05f, 0.05f, Colors.LightGray), false));

                // Piston chamber:
                _links.Add(new Link(Load.CreateBox(0.05f, 0.05f, 0.05f, Colors.Blue), true));
                _links[4].LinkDynamic.Position = Position;

                // Positions:
                _links[0].LinkDynamic.Position = Position;
                _links[1].LinkDynamic.Position = _links[0].LinkDynamic.Position + new Vector3(-_links[0].LinkDynamic.Length / 2 - _links[1].LinkDynamic.Length / 2, 0, -_links[1].LinkDynamic.Width / 2 + 0.02f);

                _links[2].LinkDynamic.Position = _links[1].LinkDynamic.Position + new Vector3(-_links[1].LinkDynamic.Length / 2 - _links[2].LinkDynamic.Length / 2, 0, -_links[1].LinkDynamic.Width / 2 - _links[2].LinkDynamic.Width / 2 + 0.05f);

                _links[3].LinkDynamic.Position = _links[2].LinkDynamic.Position + new Vector3(0, 0, -_links[2].LinkDynamic.Width / 2 - _links[3].LinkDynamic.Width / 2);

                _links[4].LinkDynamic.Position = _links[3].LinkDynamic.Position + new Vector3(0, 0, -_links[3].LinkDynamic.Width / 2 - _links[4].LinkDynamic.Width / 2 - 0.2f);
            });
        }

        private void CreateJoints()
        {
            if (_links.Any(a => a.LinkActor == null))
            {
                Log.Write("Physics Engine Action has not been invoked properly", Colors.Red, LogFilter.Error);
                return;
            }

            // Definition of Joint Local Frames and Relative Local Frames:

            _links[1].RelativeLocalFrame = Matrix4x4.CreateTranslation(_links[0].LinkDynamic.Length / 2 + _links[1].LinkDynamic.Length / 2, 0, -_links[1].LinkDynamic.Width / 2 + _links[0].LinkDynamic.Width / 2);

            _links[1].JointLocalFrame = Matrix4x4.CreateTranslation(new Vector3(0, 0, _links[1].LinkDynamic.Width / 2 - 0.015f));

            _links[2].RelativeLocalFrame = Matrix4x4.CreateTranslation(new Vector3(_links[1].LinkDynamic.Length / 2 + _links[2].LinkDynamic.Length / 2, 0, -_links[2].LinkDynamic.Width / 2 + 0.015f));

            _links[2].JointLocalFrame = Matrix4x4.CreateTranslation(new Vector3(0, 0, _links[2].LinkDynamic.Width / 2));

            _links[4].JointLocalFrame = Matrix4x4.CreateRotationY((float)Math.PI / 2);

            // Creation of Joints:

            _joints.Add("Joint 1", Environment.Scene.PhysXScene.CreateJoint(JointType.D6, _links[0].LinkActor, _links[0].JointLocalFrame, _links[1].LinkActor, _links[1].RelativeLocalFrame));

            _joints.Add("Joint 2", Environment.Scene.PhysXScene.CreateJoint(JointType.Revolute, _links[1].LinkActor, _links[1].JointLocalFrame, _links[2].LinkActor, _links[2].RelativeLocalFrame));

            _joints.Add("Joint 3", Environment.Scene.PhysXScene.CreateJoint(JointType.Spherical, _links[2].LinkActor, _links[2].JointLocalFrame, _links[3].LinkActor, _links[3].RelativeLocalFrame));

            _joints.Add("Joint 4", Environment.Scene.PhysXScene.CreateJoint(JointType.Prismatic, _links[4].LinkActor, _links[4].JointLocalFrame, _links[3].LinkActor, _links[3].RelativeLocalFrame));
        }

        private void ConfigureJoints()
        {
            foreach (var item in _joints)
            {
                item.Value.ConstraintFlags |= ConstraintFlag.Visualization;

                if (item.Key == "Joint 1")
                {
                    if (item.Value is D6Joint d6)
                    {
                        var stiffness = 1f;
                        var damping = 10000f;

                        d6.SetMotion(D6Axis.Twist, D6Motion.Free);
                        d6.SetDrive(D6Drive.Twist, new D6JointDrive(stiffness, damping, float.MaxValue, true));
                    }
                }
                else if (item.Key == "Joint 2")
                {
                    if (item.Value is RevoluteJoint revolute)
                    {
                        revolute.Flags = RevoluteJointFlag.DriveFreeSpin;
                    }
                }
            }
        }

        #endregion
    }

    [Serializable, XmlInclude(typeof(SliderCrankInfo)), XmlType(TypeName = "Experior.Catalog.PhysX.Joints.Assemblies.Mechanisms.SliderCrankInfo")]
    public class SliderCrankInfo : AssemblyInfo
    {

    }
}
