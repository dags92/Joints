using Experior.Core.Mathematics;
using Experior.Core.Parts;
using PhysX;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Experior.Core.Loads;
using static System.Windows.Forms.LinkLabel;

namespace Experior.Catalog.Joints.Assemblies.Pendulum
{
    public class DoublePendulum : Pendulum
    {
        #region Fields

        private readonly DoublePendulumInfo _info;

        private Experior.Core.Loads.Load _link3;
        private PhysX.Joint _joint2;

        #endregion

        #region Constructor

        public DoublePendulum(DoublePendulumInfo info) : base(info)
        {
            _info = info;
        }

        #endregion

        #region Public Properties

        [Browsable(true)]
        [Category("Parameters")]
        [DisplayName("Length - Link 2")]
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

        #region Private Methods

        protected override void CreateJoint()
        {
            base.CreateJoint();

            if (_link3 == null)
            {
                _link3 = Experior.Core.Loads.Load.CreateBox(0.2f, 0.2f, 0.2f, Colors.BlueViolet);
                _link3.Position = Position + new Vector3(0, -Length2, 0);
                ConfigureLoad(_link3);
            }

            if (_joint2 != null)
            {
                RemoveJoint();
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
                link3Frame.M42 = Length2;

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

        protected override void RemoveJoint()
        {
            base.RemoveJoint();

            Experior.Core.Environment.InvokePhysicsAction(_joint2.Dispose);

            if (_link3 != null)
            {
                Experior.Core.Loads.Load.Delete(_link3);
                _link3.Dispose();
                _link3 = null;
            }
        }

        #endregion
    }

    public class DoublePendulumInfo : PendulumInfo
    {
        public float Length2 { get; set; } = 1f;
    }
}
