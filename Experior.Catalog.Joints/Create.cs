using Experior.Core.Assemblies;

namespace Experior.Catalog.Joints
{
    internal class Common
    {
        public static Experior.Core.Resources.EmbeddedImageLoader EmbeddedImageLoader;
        public static Experior.Core.Resources.EmbeddedResourceLoader EmbeddedResourceLoader;
    }

    public class Create
    {
        public static Assembly MyAssembly(string title, string subtitle, object properties)
        {
            var info = new Experior.Catalog.Joints.Assemblies.MyAssemblyInfo { name = Experior.Core.Assemblies.Assembly.GetValidName("MyAssembly") };
            var assembly = new Experior.Catalog.Joints.Assemblies.MyAssembly(info);
            return assembly;
        }
    }
}