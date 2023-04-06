using System;
using System.Xml.Serialization;

namespace Experior.Catalog.Joints
{
    [Serializable, XmlInclude(typeof(AuxiliaryData)), XmlType(TypeName = "Experior.Catalog.Joints.AuxiliaryData")]
    public static class AuxiliaryData
    {
        [XmlType(TypeName = "Experior.Catalog.Joints.AuxiliaryData.RevoluteDriveTypes")]
        public enum RevoluteDriveTypes
        {
            FreeSpin,
            Motorized
        }
    }
}
