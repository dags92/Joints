using System;
using System.Windows.Media;
using System.Xml.Serialization;

namespace Experior.Catalog.Joints.Assemblies.BasicJoints
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

        public override ImageSource Image => Common.Icon.Get("FixedJoint");

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

        #endregion
    }

    [Serializable, XmlInclude(typeof(FixedInfo)), XmlType(TypeName = "Experior.Catalog.Joints.Assemblies.BasicJoints.FixedInfo")]
    public class FixedInfo : BaseInfo
    {
    }
}