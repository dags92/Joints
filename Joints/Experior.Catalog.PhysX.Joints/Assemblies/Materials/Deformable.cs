using Experior.Core.Loads;
using Experior.Core.Properties;
using System;
using System.ComponentModel;
using System.Numerics;
using System.Windows.Media;
using System.Xml.Serialization;
using Experior.Core.Properties.TypeConverter;
using Experior.Interfaces;
using PhysX;
using System.Linq;

namespace Experior.Catalog.Joints.Assemblies.Materials
{
    public class Deformable : Base
    {
        #region Fields

        private readonly DeformableInfo _info;

        #endregion

        #region Constructor

        public Deformable(DeformableInfo info)
            : base(info)
        {
            _info = info;
        }

        #endregion

        #region Public Properties

        [Category("Parameters")]
        [DisplayName("Number of Loads")]
        [PropertyOrder(0)]
        public int NumberOfLoads
        {
            get => _info.NumberOfLoads;
            set
            {
                if (value < 2)
                {
                    return;
                }

                _info.NumberOfLoads = value;
            }
        }

        [Category("Parameters")]
        [DisplayName("Length")]
        [TypeConverter(typeof(FloatMeterToMillimeter))]
        [PropertyOrder(1)]
        public float Length
        {
            get => _info.length;
            set
            {
                if (value <= 0)
                {
                    return;
                }

                _info.length = value;
                Rebuild();
            }
        }

        [Category("Parameters")]
        [DisplayName("Height")]
        [TypeConverter(typeof(FloatMeterToMillimeter))]
        [PropertyOrder(2)]
        public float Height
        {
            get => _info.BoxHeight;
            set
            {
                if (value <= 0)
                {
                    return;
                }

                _info.BoxHeight = value;
                Rebuild();
            }
        }

        [Category("Parameters")]
        [DisplayName("Width")]
        [TypeConverter(typeof(FloatMeterToMillimeter))]
        [PropertyOrder(3)]
        public float Width
        {
            get => _info.width;
            set
            {
                if (value <= 0)
                {
                    return;
                }

                _info.width = value;
                Rebuild();
            }
        }

        public override string Category => "Loads";

        public override ImageSource Image => Common.Icon.Get("Deformable");

        #endregion

        #region Protected Methods

        protected override void CreateLinks()
        {
            for (var i = 0; i < NumberOfLoads; i++)
            {
                var box = Load.CreateBox(Length, Height, Width, Colors.DarkRed);

                var kinematic = false;
                Links.Add(new Link(box, kinematic));

                if (i == 0)
                {
                    box.Position = Position;
                }
                else
                {
                    box.Position = Links[i - 1].LinkDynamic.Position + new Vector3(-box.Length - 0.01f, 0, 0);
                }

                LinkId.Add($"Load{i}");
            }

            foreach (var temp in Links)
            {
                temp.LinkDynamic.Deletable = false;
                temp.LinkDynamic.UserDeletable = false;
            }
        }

        protected override void CreateJoints()
        {
            if (Links.Any(a => a.LinkActor == null))
            {
                Log.Write("Physics Engine Action has not been invoked properly", Colors.Red, LogFilter.Error);
                return;
            }

            for (var i = 0; i < Links.Count - 1; i++)
            {
                Links[i + 1].RelativeLocalFrame = Matrix4x4.CreateTranslation(new Vector3(Links[i].LinkDynamic.Length + 0.01f, 0, 0));

                Joints.Add(Core.Environment.Scene.PhysXScene.CreateJoint(JointType.Spherical, Links[i].LinkActor, Links[i].JointLocalFrame, Links[i + 1].LinkActor, Links[i + 1].RelativeLocalFrame));
                
                Joints[i].Name = $"Spherical{i}";
                JointId.Add(Joints[i].Name);
            }
        }

        #endregion
    }

    [Serializable, XmlInclude(typeof(DeformableInfo)), XmlType(TypeName = "Experior.Catalog.Joints.Assemblies.Materials.DeformableInfo")]
    public class DeformableInfo : BaseInfo
    {
        public int NumberOfLoads { get; set; } = 5;

        public float BoxHeight { get; set; } = 0.25f;
    }
}