using PhysX;
using System;
using System.Runtime.InteropServices;
using System.Windows.Media;
using System.Xml.Serialization;

namespace Experior.Catalog.Joints.Assemblies.BasicJoints
{
    public class Spherical : Base
    {
        #region Fields

        private readonly SphericalInfo _info;

        #endregion

        #region Constructor

        public Spherical(SphericalInfo info) : base(info)
        {
            _info = info;
        }

        #endregion

        #region Public Properties

        public override ImageSource Image => Common.Icon.Get("SphericalJoint");

        #endregion

        #region Protected Methods

        protected override void CreateLinks()
        {
            if (StaticLink == null)
            {
                StaticLink = new Experior.Core.Parts.Box(Colors.DimGray, 0.1f, 0.1f, 0.1f)
                {
                    Rigid = true
                };
                Add(StaticLink);
            }

            if (DynamicLink == null)
            {
                DynamicLink = Experior.Core.Loads.Load.CreateBox(Length, Height, Width, Colors.DarkSlateGray);
            }
        }

        protected override void UpdateJointProperties()
        {
            base.UpdateJointProperties();

            if (Joint is PhysX.SphericalJoint spherical)
            {
                spherical.Flags = SphericalJointFlag.LimitEnabled;

                var temp = spherical.GetLimitCone();
                if (temp != null)
                {
                    // Modify parameters here !
                }
            }
        }

        #endregion
    }

    [Serializable, XmlInclude(typeof(SphericalInfo)), XmlType(TypeName = "Experior.Catalog.Joints.Assemblies.BasicJoints.SphericalInfo")]
    public class SphericalInfo : BaseInfo
    {

    }
}
