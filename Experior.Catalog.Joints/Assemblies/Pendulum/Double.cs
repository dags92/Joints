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
    public class Double : Single
    {
        #region Fields

        private readonly DoubleInfo _info;

        private Experior.Core.Loads.Load _link3;
        private PhysX.Joint _joint2;

        #endregion

        #region Constructor

        public Double(DoubleInfo info) : base(info)
        {
            _info = info;
        }

        #endregion

        #region Public Properties

        [Browsable(true)]
        [Category("Parameters")]
        [DisplayName("Length 2")]
        [TypeConverter(typeof(FloatMeterToMillimeter))]
        [PropertyOrder(1)]
        public float Length2
        {
            get => _info.Length2;
            set
            {
                if (value <= 0 || _info.Length2.IsEffectivelyEqual(value))
                {
                    return;
                }

                _info.Length2 = value;
                Experior.Core.Environment.Invoke(CreateJoint);
            }
        }

        #endregion

        #region Protected Properties

        protected Experior.Core.Loads.Load Link3 => _link3;

        #endregion

        #region Private Methods

        protected override void CreateJoint()
        {
            base.CreateJoint();

            RemoveLocalJoint();

            if (_link3 == null)
            {
                _link3 = Experior.Core.Loads.Load.CreateBox(0.2f, 0.2f, 0.2f, Colors.BlueViolet);
                _link3.Position = Position + new Vector3(0, -Length2, 0);
                ConfigureLoad(_link3);
            }

            Experior.Core.Environment.InvokePhysicsAction(() =>
            {
                RigidDynamic link2Actor = ((Dynamic)Link2.Part).Actor;
                RigidDynamic link3Actor = ((Dynamic)_link3.Part).Actor;

                if (link2Actor == null || link3Actor == null)
                {
                    return;
                }
                var link2Frame = Matrix4x4.Identity;
                var link3Frame = Matrix4x4.Identity;
                link3Frame.M42 = Length2 - Length1;

                _joint2 = Core.Environment.Scene.PhysXScene.CreateJoint(JointType.Spherical, link2Actor, link2Frame, link3Actor, link3Frame);

                ConfigureJoint();
            });
        }

        protected override void ConfigureJoint()
        {
            if (_joint2 == null)
            {
                return;
            }

            base.ConfigureJoint();

            _joint2.ConstraintFlags |= ConstraintFlag.Visualization;
        }

        #endregion

        #region Private Methods

        private void RemoveLocalJoint()
        {
            if (_joint2 != null)
            {
                Experior.Core.Environment.InvokePhysicsAction(_joint2.Dispose);
            }

            if (_link3 != null)
            {
                Experior.Core.Loads.Load.Delete(_link3);
                _link3.Dispose();
                _link3 = null;
            }
        }

        #endregion
    }

    [Serializable, XmlInclude(typeof(DoubleInfo)), XmlType(TypeName = "Experior.Catalog.Joints.Assemblies.Pendulum.DoubleInfo")]
    public class DoubleInfo : SingleInfo
    {
        public float Length2 { get; set; } = 1f;
    }
}
