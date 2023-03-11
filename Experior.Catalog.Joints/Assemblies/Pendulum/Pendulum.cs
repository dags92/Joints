using Experior.Core.Assemblies;
using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Xml.Serialization;
using System.Numerics;
using Experior.Core.Parts;
using PhysX;
using System.ComponentModel;
using Experior.Core.Mathematics;
using Experior.Core.Loads;

namespace Experior.Catalog.Joints.Assemblies.Pendulum
{
    public class Pendulum : Assembly
    {
        #region Fields

        private readonly PendulumInfo _info;

        private readonly Experior.Core.Parts.Box _link1;
        private Experior.Core.Loads.Load _link2;
        private PhysX.Joint _joint1;

        #endregion

        #region Constructor

        public Pendulum(PendulumInfo info)
            : base(info)
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

        [Browsable(true)]
        [Category("Parameters")]
        [DisplayName("Length - Link 1")]
        public float Length1
        {
            get => _info.Length1;
            set
            {
                if(value <= 0 || _info.Length1.IsEffectivelyEqual(value)) 
                {
                    return;
                }

                _info.Length1 = value;
                Experior.Core.Environment.Invoke(CreateJoint);
            }
        }

        public override string Category => "Pendulum";

        public override ImageSource Image => Common.Icon.Get("Pendulum");

        #endregion

        #region Public Methods

        public override void Inserted()
        {
            base.Inserted();
            Experior.Core.Environment.Invoke(CreateJoint);
        }

        public override void Dispose()
        {
            RemoveJoint();
            base.Dispose();
        }

        #endregion

        #region Protected Properties

        protected Experior.Core.Parts.Box Link1 => _link1;

        protected Experior.Core.Loads.Load Link2 => _link2;

        #endregion

        #region Protected Methods

        protected virtual void CreateJoint()
        {
            if (_joint1 != null)
            {
                RemoveJoint();
            }

            if (_link2 == null)
            {
                _link2 = Experior.Core.Loads.Load.CreateBox(0.2f, 0.2f, 0.2f, Colors.BlueViolet);
                _link2.Position = Position + new Vector3(0, -Length1, 0);
                ConfigureLoad(_link2);
            }

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
                link2Frame.M42 = Length1;

                _joint1 = Core.Environment.Scene.PhysXScene.CreateJoint(JointType.Spherical, link1Actor, link1Frame, link2Actor, link2Frame);

                ConfigureJoint();
            });
        }

        protected virtual void ConfigureJoint()
        {
            if (_link2 != null)
            {
                _link2.SleepThreshold = 0.001f;
                _link2.Sleep();
            }

            if (_joint1 != null)
            {
                _joint1.ConstraintFlags |= ConstraintFlag.Visualization;
            }
        }

        protected void ConfigureLoad(Load load)
        {
            load.Part.MinPositionIterations = 20;
            load.Part.MinVelocityIterations = 5;

            load.Part.SleepThreshold = 0.01f;
            load.Part.Sleep();
        }

        protected virtual void RemoveJoint()
        {
            Experior.Core.Environment.InvokePhysicsAction(_joint1.Dispose);

            if (_link2 != null)
            {
                Experior.Core.Loads.Load.Delete(_link2);
                _link2.Dispose();
                _link2 = null;
            }
        }

        #endregion
    }

    [Serializable, XmlInclude(typeof(PendulumInfo)), XmlType(TypeName = "Experior.Catalog.Joints.Assemblies.Pendulum.PendulumInfo")]
    public class PendulumInfo : Experior.Core.Assemblies.AssemblyInfo
    {
        public float Length1 { get; set; } = 0.5f;
    }
}