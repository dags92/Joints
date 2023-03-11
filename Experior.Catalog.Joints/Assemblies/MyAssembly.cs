using Experior.Core.Assemblies;
using System;
using System.Windows.Media;
using System.Xml.Serialization;

namespace Experior.Catalog.Joints.Assemblies
{
    public class MyAssembly : Assembly
    {
        public MyAssembly(MyAssemblyInfo info)
            : base(info)
        {
        }

        public override string Category { get; } = "Assembly";

        public override ImageSource Image { get; } = Common.EmbeddedImageLoader?.Get("MyAssembly");
    }

    [Serializable, XmlInclude(typeof(MyAssemblyInfo)), XmlType(TypeName = "Experior.Catalog.Joints.Assemblies.MyAssemblyInfo")]
    public class MyAssemblyInfo : Experior.Core.Assemblies.AssemblyInfo
    {
    }
}