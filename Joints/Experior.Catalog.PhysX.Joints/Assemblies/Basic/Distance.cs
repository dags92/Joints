using Experior.Core.Assemblies;
using Experior.Core.Loads;
using Experior.Core.Properties;
using PhysX;
using System;
using System.ComponentModel;
using System.Numerics;
using System.Windows.Media;
using System.Xml.Serialization;

namespace Experior.Catalog.Joints.Assemblies.Basic
{
    public class Distance : Base
    {
        #region Fields

        private readonly DistanceInfo _info;

        #endregion

        #region Constructor

        public Distance(DistanceInfo info)
            : base(info)
        {
            _info = info;
        }

        #endregion

        #region Public Properties

        [Category("Motion")]
        [DisplayName("Actor 0 Kinematic")]
        [PropertyOrder(0)]
        public bool Actor0Kinematic
        {
            get => _info.Actor0Kinematic;
            set
            {
                _info.Actor0Kinematic = value;
                Rebuild();
            }
        }

        [Category("Motion")]
        [DisplayName("Max. Distance")]
        [PropertyOrder(0)]
        public float MaxDistance
        {
            get => _info.MaxDistance;
            set
            {
                _info.MaxDistance = value;
                UpdateDrive();
            }
        }

        public override string Category => "Basic Joints";

        public override ImageSource Image => Common.Icon.Get("Distance");

        #endregion

        #region Protected Methods

        protected override void CreateLinks()
        {
            Links.Add(new Link(Load.CreateBox(0.25f, 0.25f, 0.25f, Colors.DarkRed), true));
            Links.Add(new Link(Load.CreateBox(0.25f, 0.25f, 0.25f, Colors.Gray), false));

            Links[0].LinkDynamic.Position = Position + new Vector3(0, -0.5f, 0);
            Links[1].LinkDynamic.Position = Links[0].LinkDynamic.Position + new Vector3(0, -Links[0].LinkDynamic.Height / 2 - Links[1].LinkDynamic.Height / 2, 0);

            LinkId.Add("Box-1");
            LinkId.Add("Box-2");
        }

        protected override void CreateJoints()
        {
            Links[1].RelativeLocalFrame = Matrix4x4.CreateTranslation(new Vector3(0, Links[0].LinkDynamic.Height / 2 + Links[1].LinkDynamic.Height / 2, 0));

            Joints.Add(Core.Environment.Scene.PhysXScene.CreateJoint(JointType.Distance, Links[0].LinkActor, Links[0].JointLocalFrame, Links[1].LinkActor, Links[1].RelativeLocalFrame));

            Joints[0].Name = "Distance";
            JointId.Add(Joints[0].Name);
        }

        protected override void ConfigureJoints()
        {
            base.ConfigureJoints();
            UpdateDrive();
        }

        #endregion

        #region Private Methods

        private void UpdateDrive()
        {
            if (!(Joints[0] is DistanceJoint distance))
            {
                return;
            }

            distance.Flags = DistanceJointFlag.MaximumDistanceEnabled;

            distance.Tolerance = 0.01f;
            distance.MaximumDistance = 1f;
        }

        #endregion
    }

    [Serializable, XmlInclude(typeof(DistanceInfo)), XmlType(TypeName = "Experior.Catalog.Joints.Assemblies.Basic.DistanceInfo")]
    public class DistanceInfo : BaseInfo
    {
        public bool Actor0Kinematic { get; set; } = true;

        public float MaxDistance { get; set; } = 1f;
    }
}