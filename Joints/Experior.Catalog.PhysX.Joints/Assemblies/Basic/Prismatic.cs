using Experior.Core.Loads;
using PhysX;
using System;
using System.Numerics;
using System.Windows.Media;
using System.Xml.Serialization;
using Experior.Core.Properties;
using System.ComponentModel;
using System.Windows.Input;

namespace Experior.Catalog.Joints.Assemblies.Basic
{
    public class Prismatic : Base
    {
        #region Fields

        private readonly PrismaticInfo _info;

        #endregion

        #region Constructor

        public Prismatic(PrismaticInfo info)
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
        [DisplayName("Lower Limit")]
        [PropertyOrder(1)]
        [PropertyAttributesProvider("DynamicLimits")]
        public float LowerLimit
        {
            get => _info.LowerLimit;
            set
            {
                if (value >= _info.UpperLimit)
                {
                    return;
                }

                _info.LowerLimit = value;
                UpdateDrive();
            }
        }

        [Category("Motion")]
        [DisplayName("Upper Limit")]
        [PropertyOrder(2)]
        [PropertyAttributesProvider("DynamicLimits")]
        public float UpperLimit
        {
            get => _info.UpperLimit;
            set
            {
                if (value <= _info.LowerLimit)
                {
                    return;
                }

                _info.UpperLimit = value;
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

        public override ImageSource Image => Common.Icon.Get("Prismatic");

        #endregion

        #region Public Methods

        public override void KeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.A)
            {
                Experior.Core.Environment.InvokeIfRequired(() =>
                {
                    Links[1].LinkDynamic.AddForce(new Vector3(100f, 0f, 0f));
                });
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void DynamicLimits(PropertyAttributes attributes)
        {
            attributes.IsBrowsable = _info.EnableLimits;
        }

        #endregion

        #region Protected Methods

        protected override void CreateLinks()
        {
            Links.Add(new Link(Load.CreateBox(0.1f, 0.1f, 0.1f, Colors.DarkRed), true));
            Links.Add(new Link(Load.CreateBox(0.3f, 0.01f, 0.3f, Colors.Gray), false));

            Links[0].LinkDynamic.Position = Position + new Vector3(0, -0.5f, 0);
            Links[1].LinkDynamic.Position = Links[0].LinkDynamic.Position;

            LinkId.Add("Motor");
            LinkId.Add("Platform");
        }

        protected override void CreateJoints()
        {
            Joints.Add(Core.Environment.Scene.PhysXScene.CreateJoint(JointType.Prismatic, Links[0].LinkActor, Links[0].JointLocalFrame, Links[1].LinkActor, Links[1].RelativeLocalFrame));

            Joints[0].Name = "Prismatic";
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
            if (!(Joints[0] is PrismaticJoint prismatic))
            {
                return;
            }

            prismatic.Flag = EnableLimits ? PrismaticJointFlag.LimitEnabled : 0;
            prismatic.Limit = new JointLinearLimitPair(LowerLimit, UpperLimit, new Spring(Stiffness, Damping));
        }

        #endregion
    }

    [Serializable, XmlInclude(typeof(PrismaticInfo)), XmlType(TypeName = "Experior.Catalog.Joints.Assemblies.Basic.PrismaticInfo")]
    public class PrismaticInfo : BaseInfo
    {
        public bool EnableLimits { get; set; } = false;

        public float LowerLimit { get; set; } = -1f;

        public float UpperLimit { get; set; } = 1f;

        public SpringLimit SpringLimit { get; set; } = new SpringLimit();
    }
}