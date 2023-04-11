using Experior.Core.Loads;
using Experior.Core.Properties;
using PhysX;
using System;
using System.ComponentModel;
using System.Numerics;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Serialization;

namespace Experior.Catalog.Joints.Assemblies.Basic
{
    public class Fixed : Base
    {
        #region Fields

        private readonly FixedInfo _info;

        #endregion

        #region Constructor

        public Fixed(FixedInfo info)
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

        public override string Category => "Basic Joints";

        public override ImageSource Image => Common.Icon.Get("Fixed");

        #endregion

        #region Public Methods

        public override void KeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.A)
            {
                Experior.Core.Environment.InvokeIfRequired(() =>
                {
                    Links[0].LinkDynamic.AddForce(new Vector3(100f, 0f, 0f));
                });
            }
            else if (e.Key == Key.D)
            {
                Experior.Core.Environment.InvokeIfRequired(() =>
                {
                    Links[1].LinkDynamic.AddForce(new Vector3(100f, 0f, 0f));
                });
            }
        }

        #endregion

        #region Protected Methods

        protected override void CreateLinks()
        {
            Links.Add(new Link(Load.CreateBox(0.25f, 0.25f, 0.25f, Colors.DarkRed), Actor0Kinematic));
            Links.Add(new Link(Load.CreateBox(0.25f, 0.25f, 0.25f, Colors.Gray), false));

            Links[0].LinkDynamic.Position = Position + new Vector3(0, -0.5f, 0);
            Links[1].LinkDynamic.Position = Links[0].LinkDynamic.Position + new Vector3(0, 0, -Links[0].LinkDynamic.Width / 2 - Links[1].LinkDynamic.Width / 2);

            LinkId.Add("Box-1");
            LinkId.Add("Box-2");
        }

        protected override void CreateJoints()
        {
            Links[1].RelativeLocalFrame = Matrix4x4.CreateTranslation(new Vector3(0, 0, Links[0].LinkDynamic.Width / 2 + Links[1].LinkDynamic.Width / 2));

            Joints.Add(Core.Environment.Scene.PhysXScene.CreateJoint(JointType.Prismatic, Links[0].LinkActor, Links[0].JointLocalFrame, Links[1].LinkActor, Links[1].RelativeLocalFrame));

            Joints[0].Name = "Fixed";
            JointId.Add(Joints[0].Name);
        }

        #endregion
    }

    [Serializable, XmlInclude(typeof(FixedInfo)), XmlType(TypeName = "Experior.Catalog.Joints.Assemblies.Basic.FixedInfo")]
    public class FixedInfo : BaseInfo
    {
        public bool Actor0Kinematic { get; set; } = true;
    }
}