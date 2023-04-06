using Experior.Catalog.Joints.Assemblies.Basic;
using Experior.Catalog.Joints.Assemblies.Mechanisms;
using Experior.Catalog.Joints.Assemblies.Robotics;
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
                name = Experior.Core.Assemblies.Assembly.GetValidName("Revolute"),
                height = 3f
            };

            var assembly = new Revolute(info);
            return assembly;
        }

        public static Assembly Prismatic(string title, string subtitle, object properties)
        {
            var info = new PrismaticInfo()
            {
                name = Experior.Core.Assemblies.Assembly.GetValidName("Prismatic"),
                height = 3f
            };

            var assembly = new Prismatic(info);
            return assembly;
        }

        public static Assembly Fixed(string title, string subtitle, object properties)
        {
            var info = new FixedInfo()
            {
                name = Experior.Core.Assemblies.Assembly.GetValidName("Fixed"),
                height = 3f
            };

            var assembly = new Fixed(info);
            return assembly;
        }

        public static Assembly Spherical(string title, string subtitle, object properties)
        {
            var info = new SphericalInfo()
            {
                name = Experior.Core.Assemblies.Assembly.GetValidName("Spherical"),
                height = 3f
            };

            var assembly = new Spherical(info);
            return assembly;
        }

        public static Assembly Distance(string title, string subtitle, object properties)
        {
            var info = new DistanceInfo()
            {
                name = Experior.Core.Assemblies.Assembly.GetValidName("Distance"),
                height = 3f
            };

            var assembly = new Distance(info);
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

        public static Assembly D6Prismatic(string title, string subtitle, object properties)
        {
            var info = new Assemblies.D6.PrismaticInfo()
            {
                name = Experior.Core.Assemblies.Assembly.GetValidName("D6 - Prismatic"),
                height = 3f
            };

            var assembly = new Assemblies.D6.Prismatic(info);
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

        public static Assembly SliderCrankInverted(string title, string subtitle, object properties)
        {
            var info = new SliderCrankInvertedInfo()
            {
                name = Experior.Core.Assemblies.Assembly.GetValidName("Slider Crank"),
                height = 3f
            };

            var assembly = new SliderCrankInverted(info);
            return assembly;
        }

        public static Assembly MovablePendulum(string title, string subtitle, object properties)
        {
            var info = new MovablePendulumInfo()
            {
                name = Experior.Core.Assemblies.Assembly.GetValidName("Movable Pendulum"),
                height = 3f
            };

            var assembly = new MovablePendulum(info);
            return assembly;
        }

        #endregion

        #region Robotics

        public static Assembly BasicGripper(string title, string subtitle, object properties)
        {
            var info = new BasicGripperInfo()
            {
                name = Experior.Core.Assemblies.Assembly.GetValidName("Basic Gripper"),
                height = 3f
            };

            var assembly = new BasicGripper(info);
            return assembly;
        }

        #endregion
    }
}