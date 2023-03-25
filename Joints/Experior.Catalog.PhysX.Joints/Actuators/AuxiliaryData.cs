using System;
using System.Xml.Serialization;

namespace Experior.Catalog.Joints.Actuators
{
    [Serializable, XmlInclude(typeof(AuxiliaryData)), XmlType(TypeName = "Experior.Catalog.Joints.Actuators.AuxiliaryData")]
    public static class AuxiliaryData
    {
        [XmlType(TypeName = "Experior.Catalog.Joints.Actuators.Commands")]
        public enum Commands
        {
            Forward = 1,
            Backward = -1,
            Stop = 0
        }
    }
}
