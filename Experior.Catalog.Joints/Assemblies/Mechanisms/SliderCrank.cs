using Experior.Core.Loads;
using Experior.Core.Parts;
using PhysX;
using System;
using System.Numerics;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Serialization;
using static System.Windows.Forms.LinkLabel;

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
            _link2.Position = Position + new Vector3(_shaft.Length / 2 + _link2.Length / 2, 0, -_link2.Width / 2);
            ConfigureLoad(_link2);

            _link3 = Experior.Core.Loads.Load.CreateBox(0.05f, 0.05f, 1.6f, Colors.LightSlateGray);
            _link3.Position = _link2.Position + new Vector3(_link2.Length / 2 + _link3.Length / 2, 0, -_link2.Width / 2 - _link3.Width / 2 + 0.05f);
            ConfigureLoad(_link3);

            Experior.Core.Environment.InvokePhysicsAction(() =>
            {
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
        }

        #endregion
    }

    [Serializable, XmlInclude(typeof(SliderCrankInfo)), XmlType(TypeName = "Experior.Catalog.Joints.Assemblies.Mechanisms.SliderCrankInfo")]
    public class SliderCrankInfo : Experior.Core.Assemblies.AssemblyInfo
    {
    }
}
