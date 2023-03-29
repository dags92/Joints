using Experior.Core.Assemblies;
using System;
using System.Windows.Media;
using System.Xml.Serialization;

namespace Experior.Catalog.Joints.Assemblies.Robotics
{
    public class BasicGripper : Assembly
    {
        #region Fields



        #endregion

        #region Constructor

        public BasicGripper(BasicGripperInfo info) : base(info)
        {
        }

        #endregion

        #region Public Properties

        public override string Category => "Robotics";

        public override ImageSource Image => Common.Icon.Get("BasicGripper");

        #endregion

        #region Public Methods



        #endregion

        #region Private Methods



        #endregion
    }

    [Serializable, XmlInclude(typeof(BasicGripperInfo)), XmlType(TypeName = "Experior.Catalog.Joints.Assemblies.Robotics.BasicGripperInfo")]
    public class BasicGripperInfo : Experior.Core.Assemblies.AssemblyInfo
    {
    }
}