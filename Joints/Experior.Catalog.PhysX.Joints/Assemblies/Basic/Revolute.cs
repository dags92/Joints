using Experior.Catalog.Joints.Actuators.Motors;
using Experior.Core.Loads;
using Experior.Interfaces;
using PhysX;
using System;
using System.Linq;
using System.Numerics;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Serialization;

namespace Experior.Catalog.Joints.Assemblies.Basic
{
    public class Revolute : Base
    {
        #region Fields

        private readonly RevoluteInfo _info;

        private readonly Actuators.Motors.Rotative _motor;

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

        public override string Category => "Basic Joints";

        public override ImageSource Image => Common.Icon.Get("Revolute");

        #endregion

        #region Public Methods

        public override void Step(float deltatime)
        {
            _motor.Step(deltatime);

            if (Joints[0] is PhysX.RevoluteJoint revolute)
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

            const float linkL = 0.025f;
            Links.Add(new Link(Load.CreateBox(linkL, linkL, 0.4f, Colors.Gray), false));

            Links[0].LinkDynamic.Position = Position;
            Links[1].LinkDynamic.Position = Links[0].LinkDynamic.Position + new Vector3(-Links[0].LinkDynamic.Length / 2 - Links[1].LinkDynamic.Length / 2, 0, -Links[1].LinkDynamic.Width / 2 + 0.02f);

            foreach (var temp in Links)
            {
                temp.LinkDynamic.Deletable = false;
                temp.LinkDynamic.UserDeletable = false;
            }

            LinkId.Add("Motor");
            LinkId.Add("Bar");
        }

        protected override void CreateJoints()
        {
            if (Links.Any(a => a.LinkActor == null))
            {
                Log.Write("Physics Engine Action has not been invoked properly", Colors.Red, LogFilter.Error);
                return;
            }

            Links[1].RelativeLocalFrame = Matrix4x4.CreateTranslation(Links[0].LinkDynamic.Length / 2 + Links[1].LinkDynamic.Length / 2, 0, -Links[1].LinkDynamic.Width / 2 + Links[0].LinkDynamic.Width / 2);
            Joints.Add(Core.Environment.Scene.PhysXScene.CreateJoint(JointType.Revolute, Links[0].LinkActor, Links[0].JointLocalFrame, Links[1].LinkActor, Links[1].RelativeLocalFrame));
            
            Joints[0].Name = "Revolute";
            JointId.Add(Joints[0].Name);
        }

        protected override void ConfigureJoints()
        {
            if (Joints[0] is PhysX.RevoluteJoint revolute)
            {
                Joints[0].ConstraintFlags |= ConstraintFlag.Visualization;
                revolute.Flags = RevoluteJointFlag.DriveEnabled;
            }
        }

        #endregion
    }

    [Serializable, XmlInclude(typeof(RevoluteInfo)), XmlType(TypeName = "Experior.Catalog.Joints.Assemblies.Basic.RevoluteInfo")]
    public class RevoluteInfo : BaseInfo
    {
    }
}