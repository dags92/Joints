using Experior.Core.Mathematics;
using Experior.Core.Parts;
using PhysX;
using System;
using System.ComponentModel;
using System.Numerics;
using System.Windows.Media;
using System.Xml.Serialization;
using Experior.Core.Properties;

namespace Experior.Catalog.Joints.Assemblies.Pendulum
{
    public class Triple : Double
    {
        #region Fields

        private readonly TripleInfo _info;

        private Experior.Core.Loads.Load _link4;
        private PhysX.Joint _joint3;

        #endregion

        #region Constructor

        public Triple(TripleInfo info) : base(info)
        {
            _info = info;
        }

        #endregion

        #region Public Properties

        [Browsable(true)]
        [Category("Parameters")]
        [DisplayName("Length - Link 2")]
        [PropertyOrder(2)]
        public float Length3
        {
            get => _info.Length3;
            set
            {
                if (value <= 0 || _info.Length3.IsEffectivelyEqual(value))
                {
                    return;
                }

                _info.Length3 = value;
                Experior.Core.Environment.Invoke(CreateJoint);
            }
        }

        #endregion

        #region Protected Methods

        protected override void CreateJoint()
        {
            base.CreateJoint();

            if (_link4 == null)
            {
                _link4 = Experior.Core.Loads.Load.CreateBox(0.2f, 0.2f, 0.2f, Colors.BlueViolet);
                _link4.Position = Position + new Vector3(0, -Length3, 0);
                ConfigureLoad(_link4);
            }

            if (_joint3 != null)
            {
                RemoveJoint();
            }

            Experior.Core.Environment.InvokePhysicsAction(() =>
            {
                RigidDynamic link3Actor = ((Dynamic)Link2.Part).Actor;
                RigidDynamic link4Actor = ((Dynamic)_link4.Part).Actor;

                if (link4Actor == null || link3Actor == null)
                {
                    return;
                }
                var link3Frame = Matrix4x4.Identity;
                var link4Frame = Matrix4x4.Identity;
                link3Frame.M42 = Length2;

                _joint3 = Core.Environment.Scene.PhysXScene.CreateJoint(JointType.Spherical, link3Actor, link3Frame, link4Actor, link4Frame);

                ConfigureJoint();
            });
        }

        protected override void ConfigureJoint()
        {
            if (_joint3 == null)
            {
                return;
            }

            base.ConfigureJoint();

            _joint3.ConstraintFlags |= ConstraintFlag.Visualization;
        }

        protected override void RemoveJoint()
        {
            base.RemoveJoint();

            Experior.Core.Environment.InvokePhysicsAction(_joint3.Dispose);

            if (_link4 != null)
            {
                Experior.Core.Loads.Load.Delete(_link4);
                _link4.Dispose();
                _link4 = null;
            }
        }

        #endregion
    }

    [Serializable, XmlInclude(typeof(TripleInfo)), XmlType(TypeName = "Experior.Catalog.Joints.Assemblies.Pendulum.TripleInfo")]
    public class TripleInfo : DoubleInfo
    {
        public float Length3 { get; set; } = 1.5f;
    }
}
