using Experior.Core.Loads;
using Experior.Core.Parts;
using PhysX;
using System;
using System.Numerics;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Serialization;

namespace Experior.Catalog.Joints.Assemblies.Mechanisms
{
    public class SliderCrank : Experior.Core.Assemblies.Assembly
    {
        #region Fields

        private readonly SliderCrankInfo _info;

        private readonly Experior.Core.Parts.Box _shaft;
        private Experior.Core.Loads.Load _link2;
        private PhysX.Joint _joint1;

        private Experior.Core.Loads.Load _link3;
        private PhysX.Joint _joint2;

        private Experior.Core.Loads.Load _link4;
        private PhysX.Joint _joint3;

        private Experior.Core.Parts.Box _chamber;
        private PhysX.Joint _joint4;

        #endregion

        #region Constructor

        public SliderCrank(SliderCrankInfo info) : base(info)
        {
            _info = info;

            _shaft = new Experior.Core.Parts.Box(Colors.Blue, 0.15f, 0.15f, 0.15f)
            {
                Rigid = true
            };
            Add(_shaft);

            _chamber = new Experior.Core.Parts.Box(Colors.Orange, 0.05f, 0.05f, 0.05f)
            {
                Rigid = true
            };
            Add(_chamber, new Vector3(_shaft.Length / 2 + 0.05f + _chamber.Length / 2, 0, -_shaft.Width / 2 -_chamber.Width / 2 - 2.35f));
        }

        #endregion

        #region Public Properties

        public override string Category => "Mechanisms";

        public override ImageSource Image => Common.Icon.Get("SliderCrank");

        #endregion

        #region Public Methods

        public override void KeyDown(KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.A:
                    Experior.Core.Environment.Invoke(Create);
                    break;
            }
        }

        #endregion

        #region Private Methods

        private void Create()
        {
            _link2 = Experior.Core.Loads.Load.CreateBox(0.05f, 0.05f, 0.8f, Colors.Gray);
            ConfigureLoad(_link2);

            _link3 = Experior.Core.Loads.Load.CreateBox(0.05f, 0.05f, 1.6f, Colors.LightSlateGray);
            ConfigureLoad(_link3);

            _link4 = Experior.Core.Loads.Load.CreateBox(0.1f, 0.1f, 0.1f, Colors.Silver);
            ConfigureLoad(_link4);

            Experior.Core.Environment.InvokePhysicsAction(() =>
            {
                // Part 1:

                RigidDynamic link1Actor = _shaft.Actor;
                RigidDynamic link2Actor = ((Dynamic)_link2.Part).Actor;

                if (link1Actor == null || link2Actor == null)
                {
                    return;
                }

                var link1Frame = Matrix4x4.Identity;
                var link2Frame = Matrix4x4.Identity;
                link2Frame.M41 = -(_shaft.Length / 2 + _link2.Length / 2);
                link2Frame.M43 = -_link2.Width / 2;

                _joint1 = Core.Environment.Scene.PhysXScene.CreateJoint(JointType.Revolute, link1Actor, link1Frame, link2Actor, link2Frame);

                // Part 2:
                RigidDynamic link3Actor = ((Dynamic)_link3.Part).Actor;

                if (link3Actor == null)
                {
                    return;
                }

                var interLink2Frame = Matrix4x4.Identity;
                interLink2Frame.M43 = _link2.Width / 2;

                var link3Frame = Matrix4x4.Identity;
                link3Frame.M41 = -(_link2.Length / 2 + _link3.Length / 2);
                link3Frame.M43 = -_link3.Width / 2 + 0.05f;

                _joint2 = Core.Environment.Scene.PhysXScene.CreateJoint(JointType.Revolute, link2Actor, interLink2Frame, link3Actor, link3Frame);

                // Part 3:
                RigidDynamic link4Actor = ((Dynamic)_link4.Part).Actor;

                if (link4Actor == null)
                {
                    return;
                }

                var interLink3Frame = Matrix4x4.Identity;
                interLink3Frame.M43 = _link3.Width / 2;

                var link4Frame = Matrix4x4.Identity;
                link4Frame.M41 = -_link4.Length / 2;

                _joint3 = Core.Environment.Scene.PhysXScene.CreateJoint(JointType.Spherical, link3Actor, interLink3Frame, link4Actor, link4Frame);

                // Part 4:
                RigidDynamic link5Actor = _chamber.Actor;

                if (link5Actor == null)
                {
                    return;
                }

                var interLink4Frame = Matrix4x4.CreateRotationY((float)Math.PI / 2);

                _joint4 = Core.Environment.Scene.PhysXScene.CreateJoint(JointType.Prismatic, link5Actor,
                    interLink4Frame, link4Actor, Matrix4x4.Identity);

                ConfigureJoints();
            });
        }

        private void ConfigureLoad(Load load)
        {
            load.Part.MinPositionIterations = 20;
            load.Part.MinVelocityIterations = 5;
        }

        private void ConfigureJoints()
        {
            if (_joint1 != null)
            {
                _joint1.ConstraintFlags |= ConstraintFlag.Visualization;

                if (_joint1 is PhysX.RevoluteJoint revolution)
                {
                    revolution.Flags = RevoluteJointFlag.DriveEnabled;
                    revolution.DriveVelocity = 1f;
                }
            }

            if (_joint2 != null)
            {
                _joint2.ConstraintFlags |= ConstraintFlag.Visualization;

                if (_joint2 is PhysX.RevoluteJoint revolution)
                {
                    revolution.Flags = RevoluteJointFlag.DriveFreeSpin;
                }
            }

            if (_joint3 != null)
            {
                _joint3.ConstraintFlags |= ConstraintFlag.Visualization;
            }

            if (_joint4 != null)
            {
                _joint4.ConstraintFlags |= ConstraintFlag.Visualization;
            }
        }

        #endregion
    }

    [Serializable, XmlInclude(typeof(SliderCrankInfo)), XmlType(TypeName = "Experior.Catalog.Joints.Assemblies.Mechanisms.SliderCrankInfo")]
    public class SliderCrankInfo : Experior.Core.Assemblies.AssemblyInfo
    {
    }
}
