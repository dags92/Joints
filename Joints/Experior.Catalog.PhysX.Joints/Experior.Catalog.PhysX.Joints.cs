using Experior.Core.Resources;
using System.Windows.Media;

namespace Experior.Catalog.PhysX.Joints
{
    public class Experior.Catalog.PhysX.Joints : Experior.Core.Catalog
    {
        public Experior.Catalog.PhysX.Joints()
            : base("Experior.Catalog.PhysX.Joints")
        {
            Simulation = Experior.Core.Environment.Simulation.Events | Experior.Core.Environment.Simulation.Physics;

            Add(EmbeddedResource.GetImage("TemplateAssembly"), "Template Assembly", "", Experior.Core.Environment.Simulation.Events | Experior.Core.Environment.Simulation.Physics, Create.TemplateAssembly);
        }

public override ImageSource Logo => EmbeddedResource.GetImage("Logo");
    }

    internal static class EmbeddedResource
{
    private static EmbeddedImageLoader Images { get; } = new Experior.Core.Resources.EmbeddedImageLoader();
    private static EmbeddedResourceLoader Resource { get; } = new Experior.Core.Resources.EmbeddedResourceLoader();

    public static ImageSource GetImage(string resourceFileName) => Images.Get(resourceFileName);
    public static Experior.Core.Resources.EmbeddedResource GetResource(string resourceFileName) => Resource.Get(resourceFileName);
}
}