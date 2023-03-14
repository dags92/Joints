using System.Numerics;
using Experior.Catalog.Joints.Assemblies;
using Experior.Catalog.Joints.Assemblies.BasicJoints;
using Experior.Catalog.Joints.Assemblies.Mechanisms;
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
                length = 0.2f,
                width = 0.2f,
                height = 0.2f,
                JointType = AuxiliaryData.JointTypes.Revolute
            };

            var assembly = new Revolute(info);
            return assembly;
        }

        public static Assembly Prismatic(string title, string subtitle, object properties)
        {
            var info = new PrismaticInfo()
            {
                name = Experior.Core.Assemblies.Assembly.GetValidName("Prismatic Joint"),
                length = 0.2f,
                width = 0.2f,
                height = 0.2f,
                JointType = AuxiliaryData.JointTypes.Prismatic,
                DynamicLinkTranslation = Vector3.Zero
            };

            var assembly = new Prismatic(info);
            return assembly;
        }

        public static Assembly Fixed(string title, string subtitle, object properties)
        {
            var info = new FixedInfo()
            {
                name = Experior.Core.Assemblies.Assembly.GetValidName("Fixed Joint"),
                length = 0.2f,
                width = 0.2f,
                height = 0.2f,
                JointType = AuxiliaryData.JointTypes.Fixed,
                DynamicLinkTranslation = new Vector3(0.5f, 0f, 0f)
            };

            var assembly = new Fixed(info);
            return assembly;
        }

        public static Assembly Spherical(string title, string subtitle, object properties)
        {
            var info = new SphericalInfo()
            {
                name = Experior.Core.Assemblies.Assembly.GetValidName("Spherical Joint"),
                length = 0.2f,
                width = 0.2f,
                height = 0.2f,
                JointType = AuxiliaryData.JointTypes.Spherical
            };

            var assembly = new Spherical(info);
            return assembly;
        }

        public static Assembly Distance(string title, string subtitle, object properties)
        {
            var info = new SphericalInfo()
            {
                name = Experior.Core.Assemblies.Assembly.GetValidName("Distance Joint"),
                length = 0.2f,
                width = 0.2f,
                height = 0.2f,
                JointType = AuxiliaryData.JointTypes.Distance
            };

            var assembly = new Spherical(info);
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

        #region Mechanisms

        public static Assembly SliderCrank(string title, string subtitle, object properties)
        {
            var info = new SliderCrankInfo()
            {
                name = Experior.Core.Assemblies.Assembly.GetValidName("Slider Crank"),
                height = 5f
            };

            var assembly = new SliderCrank(info);
            return assembly;
        }

        #endregion
    }
}