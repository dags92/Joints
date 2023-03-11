using Experior.Core.Assemblies;
using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;
using System.Numerics;
using Experior.Core.Parts;
using PhysX;
using System.ComponentModel;

namespace Experior.Catalog.Joints.Assemblies.Pendulum
{
    public class Pendulum : Assembly
    {
        #region Fields

        private readonly PendulumInfo _info;

        private readonly Experior.Core.Parts.Box _pivot;
        private Experior.Core.Loads.Load _node;
        private PhysX.Joint _joint;

        #endregion

        #region Constructor

        public Pendulum(PendulumInfo info)
            : base(info)
        {
            _info = info;

            _pivot = new Experior.Core.Parts.Box(Colors.Blue, 0.15f, 0.15f, 0.15f);
            Add(_pivot);
            
            CreateNode();
        }

        #endregion

        #region Public Properties

        [Browsable(true)]
        [Category("Motion")]
        [DisplayName("Drive Velocity")]
        public float Length1
        {
            get => _info.Length1;
            set
            {
                if(value <= 0) 
                {
                    return;
                }

                _info.Length1 = value;

            }
        }

        public override string Category => "Pendulum";

        public override ImageSource Image => Common.EmbeddedImageLoader.Get("Pendulum");

        #endregion

        #region Protected Properties

        protected List<PhysX.Joint> Joints { get; } = new List<PhysX.Joint>();

        protected List<Experior.Core.Parts.Box> Nodes { get; } = new List<Box>();

        #endregion

        #region Public Methods

        public override void Inserted()
        {
            base.Inserted();

            CreateJoint();
        }

        #endregion

        protected virtual void CreateJoint()
        {
            Experior.Core.Environment.InvokeIfRequired(() =>
            {
                RigidDynamic pivotActor = _pivot.Actor;
                RigidDynamic nodeActor = ((Dynamic)_node.Part).Actor;

                if (pivotActor == null || nodeActor == null)
                {
                    return;
                }

                var nodeFrame = Matrix4x4.Identity;
                nodeFrame.M42 = Length1;

                _joint = Core.Environment.Scene.PhysXScene.CreateJoint(JointType.Spherical, pivotActor, pivotActor.GlobalPose, nodeActor, );
            });
        }

        #region Private Methods



        #endregion

        #region Nested Types

        public class PendulumLink
        {
        }

        #endregion
    }

    [Serializable, XmlInclude(typeof(PendulumInfo)), XmlType(TypeName = "Experior.Catalog.Joints.Assemblies.Pendulum.PendulumInfo")]
    public class PendulumInfo : Experior.Core.Assemblies.AssemblyInfo
    {
        public float Length1 { get; set; } = 0.5f;
    }
}