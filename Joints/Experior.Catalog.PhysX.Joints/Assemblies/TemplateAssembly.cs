using Experior.Core.Assemblies;
using System;
using System.Windows.Media;
using System.Xml.Serialization;

namespace Experior.Catalog.Assemblies
{
    public class TemplateAssembly : Assembly
    {
        public TemplateAssembly(TemplateAssemblyInfo info)
            : base(info)
        {
        }

        public override string Category { get; } = "Assembly";

        public override ImageSource Image { get; } = EmbeddedResource.GetImage("TemplateAssembly");
    }

    [Serializable, XmlInclude(typeof(TemplateAssemblyInfo)), XmlType(TypeName = "Experior.Catalog.Assemblies.TemplateAssemblyInfo")]
    public class TemplateAssemblyInfo : Experior.Core.Assemblies.AssemblyInfo
    {
    }
}