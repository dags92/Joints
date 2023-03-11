using Experior.Catalog.Joints.Assemblies.Pendulum;
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
        #region Pendulums

        public static Assembly Pendulum(string title, string subtitle, object properties)
        {
            var info = new PendulumInfo()
            {
                name = Experior.Core.Assemblies.Assembly.GetValidName("Pendulum"),
                height = 5f
            };

            var assembly = new Pendulum(info);
            return assembly;
        }

        public static Assembly DoublePendulum(string title, string subtitle, object properties)
        {
            var info = new DoublePendulumInfo()
            {
                name = Experior.Core.Assemblies.Assembly.GetValidName("Double Pendulum"),
                height = 5f
            };

            var assembly = new DoublePendulum(info);
            return assembly;
        }

        #endregion
    }
}