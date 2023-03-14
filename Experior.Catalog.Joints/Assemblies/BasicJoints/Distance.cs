using System;
using System.Windows.Media;
using System.Xml.Serialization;
using PhysX;

namespace Experior.Catalog.Joints.Assemblies.BasicJoints
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

        public override string Category => "Basic Joints";

        public override ImageSource Image => Common.Icon.Get("Distance");

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

            if (Joint is PhysX.DistanceJoint distance)
            {
                distance.Flags = DistanceJointFlag.MaximumDistanceEnabled;
                distance.MaximumDistance = 0.2f;
            }
        }

        #endregion
    }

    [Serializable, XmlInclude(typeof(DistanceInfo)), XmlType(TypeName = "Experior.Catalog.Joints.Assemblies.BasicJoints.DistanceInfo")]
    public class DistanceInfo : BaseInfo
    {
    }
}