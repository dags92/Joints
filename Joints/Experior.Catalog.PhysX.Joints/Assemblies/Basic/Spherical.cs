using Experior.Core.Assemblies;
using Experior.Core.Loads;
using PhysX;
using System;
using System.Numerics;
using System.Windows.Media;
using System.Xml.Serialization;
using Experior.Core.Mathematics;
using Experior.Core.Properties;
using System.ComponentModel;
using Experior.Core.Properties.TypeConverter;

namespace Experior.Catalog.Joints.Assemblies.Basic
{
    public class Spherical : Base
    {
        #region Fields

        private readonly SphericalInfo _info;

        #endregion

        #region Constructor

        public Spherical(SphericalInfo info)
            : base(info)
        {
            _info = info;
        }

        #endregion

        #region Public Properties

        [Category("Motion")]
        [DisplayName("Enable Limits")]
        [PropertyOrder(0)]
        public bool EnableLimits
        {
            get => _info.EnableLimits;
            set
            {
                _info.EnableLimits = value;
                UpdateDrive();

                Experior.Core.Environment.Properties.Refresh();
            }
        }

        [Category("Motion")]
        [DisplayName("Y Limit")]
        [PropertyOrder(1)]
        [PropertyAttributesProvider("DynamicLimits")]
        [TypeConverter(typeof(RadiansToDegrees))]
        public float YLimit
        {
            get => _info.YLimit;
            set
            {
                _info.YLimit = value;
                UpdateDrive();
            }
        }

        [Category("Motion")]
        [DisplayName("Z Limit")]
        [PropertyOrder(2)]
        [PropertyAttributesProvider("DynamicLimits")]
        [TypeConverter(typeof(RadiansToDegrees))]
        public float ZLimit
        {
            get => _info.ZLimit;
            set
            {
                _info.ZLimit = value;
                UpdateDrive();
            }
        }

        [Category("Motion")]
        [DisplayName("Stiffness")]
        [PropertyOrder(3)]
        [PropertyAttributesProvider("DynamicLimits")]
        public float Stiffness
        {
            get => _info.SpringLimit.Stiffness;
            set
            {
                _info.SpringLimit.Stiffness = value;
                UpdateDrive();
            }
        }

        [Category("Motion")]
        [DisplayName("Damping")]
        [PropertyOrder(4)]
        [PropertyAttributesProvider("DynamicLimits")]
        public float Damping
        {
            get => _info.SpringLimit.Damping;
            set
            {
                _info.SpringLimit.Damping = value;
                UpdateDrive();
            }
        }

        public override string Category => "Basic Joints";

        public override ImageSource Image => Common.Icon.Get("Spherical");

        #endregion

        #region Public Methods

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void DynamicLimits(PropertyAttributes attributes)
        {
            attributes.IsBrowsable = _info.EnableLimits;
        }

        #endregion

        #region Protected Methods

        protected override void CreateLinks()
        {
            Links.Add(new Link(Load.CreateBox(0.25f, 0.25f, 0.25f, Colors.DarkRed), true));
            Links.Add(new Link(Load.CreateBox(0.25f, 0.25f, 0.25f, Colors.Gray), false));

            Links[0].LinkDynamic.Position = Position + new Vector3(0, -0.5f, 0);
            Links[1].LinkDynamic.Position = Links[0].LinkDynamic.Position + new Vector3(0, 0, -Links[0].LinkDynamic.Width / 2 - Links[1].LinkDynamic.Width / 2);

            LinkId.Add("Box-1");
            LinkId.Add("Box-2");
        }

        protected override void CreateJoints()
        {
            Links[1].RelativeLocalFrame = Matrix4x4.CreateTranslation(new Vector3(0, 0, Links[0].LinkDynamic.Width / 2 + Links[1].LinkDynamic.Width / 2 + 0.1f));

            Joints.Add(Core.Environment.Scene.PhysXScene.CreateJoint(JointType.Spherical, Links[0].LinkActor, Links[0].JointLocalFrame, Links[1].LinkActor, Links[1].RelativeLocalFrame));

            Joints[0].Name = "Spherical";
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
            if (!(Joints[0] is SphericalJoint spherical))
            {
                return;
            }

            spherical.Flags = SphericalJointFlag.LimitEnabled;
            spherical.SetLimitCone(new JointLimitCone(YLimit, ZLimit, new Spring(Stiffness, Damping)));
        }

        #endregion
    }

    [Serializable, XmlInclude(typeof(SphericalInfo)), XmlType(TypeName = "Experior.Catalog.Joints.Assemblies.Basic.SphericalInfo")]
    public class SphericalInfo : BaseInfo
    {
        public bool EnableLimits { get; set; } = false;

        public float YLimit { get; set; } = 45f.ToRadians();

        public float ZLimit { get; set; } = 45f.ToRadians();

        public SpringLimit SpringLimit { get; set; } = new SpringLimit();
    }
}