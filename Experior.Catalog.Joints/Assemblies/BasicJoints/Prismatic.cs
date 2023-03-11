using System;
using System.Numerics;
using System.Windows.Media;
using System.Xml.Serialization;
using Experior.Core.Loads;
using Experior.Core.Parts;
using PhysX;

namespace Experior.Catalog.Joints.Assemblies.BasicJoints
{
    public class Prismatic : Experior.Core.Assemblies.Assembly
    {
        #region Fields

        private readonly PrismaticInfo _info;

        private readonly Experior.Core.Parts.Box _link1;
        private Experior.Core.Loads.Load _link2;
        private PhysX.Joint _joint1;

        #endregion

        #region Constructor

        public Prismatic(PrismaticInfo info) : base(info)
        {
            _info = info;

            _link1 = new Experior.Core.Parts.Box(Colors.Blue, 0.15f, 0.15f, 0.15f)
            {
                Rigid = true
            };
            Add(_link1);
        }

        #endregion

        #region Public Properties

        public override string Category => "Basic Joints";

        public override ImageSource Image => Common.Icon.Get("PrismaticJoint");

        #endregion

        #region Public Methods

        public override void Inserted()
        {
            base.Inserted();
            Experior.Core.Environment.Invoke(CreateJoint);
        }

        #endregion

        #region Private Methods

        private void CreateJoint()
        {
            if (_joint1 != null)
            {
                Experior.Core.Environment.InvokePhysicsAction(_joint1.Dispose);
            }

            if (_link2 != null)
            {
                Experior.Core.Loads.Load.Delete(_link2);
                _link2.Dispose();
                _link2 = null;
            }

            _link2 = Experior.Core.Loads.Load.CreateBox(0.2f, 0.2f, 0.2f, Colors.BlueViolet);
            _link2.Position = Position + new Vector3(0.5f, 0, 0);
            ConfigureLoad(_link2);

            Experior.Core.Environment.InvokePhysicsAction(() =>
            {
                RigidDynamic link1Actor = _link1.Actor;
                RigidDynamic link2Actor = ((Dynamic)_link2.Part).Actor;

                if (link1Actor == null || link2Actor == null)
                {
                    return;
                }

                var link1Frame = Matrix4x4.Identity;
                var link2Frame = Matrix4x4.Identity;

                _joint1 = Core.Environment.Scene.PhysXScene.CreateJoint(JointType.Prismatic, link1Actor, link1Frame, link2Actor, link2Frame);

                ConfigureJoint();
            });
        }

        private void ConfigureJoint()
        {
            if (_joint1 is PhysX.PrismaticJoint prismatic)
            {
                prismatic.ConstraintFlags |= ConstraintFlag.Visualization;
            }
        }

        private void ConfigureLoad(Load load)
        {
            load.Part.MinPositionIterations = 20;
            load.Part.MinVelocityIterations = 5;
        }

        #endregion
    }

    [Serializable, XmlInclude(typeof(PrismaticInfo)), XmlType(TypeName = "Experior.Catalog.Joints.Assemblies.BasicJoints.PrismaticInfo")]
    public class PrismaticInfo : Experior.Core.Assemblies.AssemblyInfo
    {

    }
}
