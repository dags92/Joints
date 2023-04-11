using System;
using System.Xml.Serialization;

namespace Experior.Catalog.Joints.Assemblies
{
    [Serializable, XmlInclude(typeof(SpringLimit)), XmlType(TypeName = "Experior.Catalog.Joints.Assemblies.SpringLimit")]
    public class SpringLimit
    {
        public float Stiffness { get; set; } = 5f;

        public float Damping { get; set; } = 0.05f;
    }
}
