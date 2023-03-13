using Experior.Core.Mathematics;
using Experior.Core.Parts;
using PhysX;
using System;
using System.ComponentModel;
using System.Numerics;
using System.Windows.Media;
using System.Xml.Serialization;
using Experior.Core.Properties;
using Experior.Core.Properties.TypeConverter;

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
        [DisplayName("Length 3")]
        [TypeConverter(typeof(FloatMeterToMillimeter))]
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

            RemoveLocalJoint();

            if (_link4 == null)
            {
                _link4 = Experior.Core.Loads.Load.CreateBox(0.2f, 0.2f, 0.2f, Colors.BlueViolet);
                _link4.Position = Position + new Vector3(0, -Length3, 0);
                ConfigureLoad(_link4);
            }

            Experior.Core.Environment.InvokePhysicsAction(() =>
            {
                RigidDynamic link3Actor = ((Dynamic)Link3.Part).Actor;
                RigidDynamic link4Actor = ((Dynamic)_link4.Part).Actor;

                if (link3Actor == null || link4Actor == null)
                {
                    return;
                }
                var link3Frame = Matrix4x4.Identity;
                var link4Frame = Matrix4x4.Identity;
                link4Frame.M42 = Length3 - Length2;

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

            if (_joint3 is PhysX.SphericalJoint spherical)
            {
                spherical.Flags = SphericalJointFlag.LimitEnabled;
                var temp = spherical.GetLimitCone();
                if (temp != null)
                {
                    temp.YLimitAngle = (float)Math.PI / 6;
                    temp.ZLimitAngle = (float)Math.PI / 6;
                    temp.Damping = float.MaxValue;
                    temp.BounceThreshold = 0.001f;
                    temp.Stiffness = 20f;
                }
            }
        }

        #endregion

        #region Private Methods

        private void RemoveLocalJoint()
        {
            if (_joint3 != null)
            {
                Experior.Core.Environment.InvokePhysicsAction(_joint3.Dispose);
            }

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
