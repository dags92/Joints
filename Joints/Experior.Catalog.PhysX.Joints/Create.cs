using Experior.Catalog.Joints.Assemblies.Basic;
using Experior.Catalog.Joints.Assemblies.Mechanisms;
using Experior.Core.Assemblies;
using PhysX;

namespace Experior.Catalog.Joints
{
    internal class Common
    {
        public static Experior.Core.Resources.EmbeddedImageLoader Icon;
        public static Experior.Core.Resources.EmbeddedResourceLoader Mesh;
    }

    public class Create
    {
        #region Basic Joints

        public static Assembly Revolute(string title, string subtitle, object properties)
        {
            var info = new RevoluteInfo()
            {
                name = Experior.Core.Assemblies.Assembly.GetValidName("Revolute"),
                height = 3f
            };

            var assembly = new Revolute(info);
            return assembly;
        }

        #endregion

        #region D6 Joints

        public static Assembly D6Revolute(string title, string subtitle, object properties)
        {
            var info = new Assemblies.D6.RevoluteInfo()
            {
                name = Experior.Core.Assemblies.Assembly.GetValidName("D6 - Revolute"),
                height = 3f
            };

            var assembly = new Assemblies.D6.Revolute(info);
            return assembly;
        }

        #endregion

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