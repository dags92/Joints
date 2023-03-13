using System;
using System.Windows.Media;
using System.Xml.Serialization;

namespace Experior.Catalog.Joints.Assemblies.BasicJoints
{
    public class Prismatic : Base
    {
        #region Fields

        private readonly PrismaticInfo _info;

        #endregion

        #region Constructor

        public Prismatic(PrismaticInfo info) : base(info)
        {
            _info = info;
        }

        #endregion

        #region Public Properties

        public override ImageSource Image => Common.Icon.Get("PrismaticJoint");

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

    [Serializable, XmlInclude(typeof(PrismaticInfo)), XmlType(TypeName = "Experior.Catalog.Joints.Assemblies.BasicJoints.PrismaticInfo")]
    public class PrismaticInfo : BaseInfo
    {

    }
}
