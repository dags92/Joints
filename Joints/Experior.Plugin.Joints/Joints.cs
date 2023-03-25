using Experior.Core.Resources;
using System.Windows.Media;

namespace Experior.Plugin.Joints
{
    public class Joints : Experior.Core.Plugin
    {
        public Joints()
            : base("MyPlugin")
        {
        }

        public override ImageSource Logo { get; } = EmbeddedResource.GetImage("Experior.png");
    }

    internal static class EmbeddedResource
    {
        private static EmbeddedImageLoader Images { get; } = new Experior.Core.Resources.EmbeddedImageLoader();
        private static EmbeddedResourceLoader Resource { get; } = new Experior.Core.Resources.EmbeddedResourceLoader();

        public static ImageSource GetImage(string resourceFileName) => Images.Get(resourceFileName);
        public static Experior.Core.Resources.EmbeddedResource GetResource(string resourceFileName) => Resource.Get(resourceFileName);
    }
}
