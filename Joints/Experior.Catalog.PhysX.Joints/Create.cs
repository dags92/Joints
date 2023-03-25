using Experior.Catalog.Joints.Assemblies.Mechanisms;
using Experior.Core.Assemblies;

namespace Experior.Catalog.Joints
{
    internal class Common
    {
        public static Experior.Core.Resources.EmbeddedImageLoader Icon;
        public static Experior.Core.Resources.EmbeddedResourceLoader Mesh;
    }

    public class Create
    {
        #region Mechanisms

        public static Assembly SliderCrank(string title, string subtitle, object properties)
        {
            var info = new SliderCrankInfo()
            {
                name = Experior.Core.Assemblies.Assembly.GetValidName("Slider Crank"),
                height = 3f
            };

            var assembly = new SliderCrank(info);
            return assembly;
        }

        #endregion
    }
}