using Experior.Catalog.Joints.Assemblies.BasicJoints;
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
        #region Basic Joints

        public static Assembly Revolute(string title, string subtitle, object properties)
        {
            var info = new RevoluteInfo()
            {
                name = Experior.Core.Assemblies.Assembly.GetValidName("Revolute Joint"),
                height = 5f
            };

            var assembly = new Revolute(info);
            return assembly;
        }

        public static Assembly Prismatic(string title, string subtitle, object properties)
        {
            var info = new PrismaticInfo()
            {
                name = Experior.Core.Assemblies.Assembly.GetValidName("Prismatic Joint"),
                height = 4f
            };

            var assembly = new Prismatic(info);
            return assembly;
        }

        #endregion

        #region Pendulums

        public static Assembly Pendulum(string title, string subtitle, object properties)
        {
            var info = new SingleInfo()
            {
                name = Experior.Core.Assemblies.Assembly.GetValidName("Single"),
                height = 5f
            };

            var assembly = new Single(info);
            return assembly;
        }

        public static Assembly DoublePendulum(string title, string subtitle, object properties)
        {
            var info = new DoubleInfo()
            {
                name = Experior.Core.Assemblies.Assembly.GetValidName("Double Single"),
                height = 5f
            };

            var assembly = new Double(info);
            return assembly;
        }

        public static Assembly TriplePendulum(string title, string subtitle, object properties)
        {
            var info = new TripleInfo()
            {
                name = Experior.Core.Assemblies.Assembly.GetValidName("Triple Single"),
                height = 7f
            };

            var assembly = new Triple(info);
            return assembly;
        }

        #endregion
    }
}