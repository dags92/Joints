using System.Windows.Media;

namespace Experior.Catalog.Joints
{
    public class Joints : Experior.Core.Catalog
    {
        #region Fields



        #endregion

        #region Constructor

        public Joints(): base("Joints")
        {
            Simulation = Experior.Core.Environment.Simulation.Physics;

            Common.Mesh = new Experior.Core.Resources.EmbeddedResourceLoader(System.Reflection.Assembly.GetExecutingAssembly());
            Common.Icon = new Experior.Core.Resources.EmbeddedImageLoader(System.Reflection.Assembly.GetExecutingAssembly());

            #region Basic Joints

            Add(Common.Icon.Get("Slider-Crank"), "Basic Joints", "Twist", Simulation, Create.Revolute);
            Add(Common.Icon.Get("Slider-Crank"), "Basic Joints", "Prismatic", Simulation, Create.Prismatic);
            Add(Common.Icon.Get("Slider-Crank"), "Basic Joints", "Fixed", Simulation, Create.Fixed);
            Add(Common.Icon.Get("Slider-Crank"), "Basic Joints", "Spherical", Simulation, Create.Spherical);
            Add(Common.Icon.Get("Slider-Crank"), "Basic Joints", "Distance", Simulation, Create.Distance);
            Add(Common.Icon.Get("Slider-Crank"), "Basic Joints", "Pose", Simulation, Create.Pose);

            #endregion

            #region D6 Joints

            Add(Common.Icon.Get("Slider-Crank"), "D6 Joints", "Prismatic", Simulation, Create.D6Prismatic);
            Add(Common.Icon.Get("Slider-Crank"), "D6 Joints", "Twist", Simulation, Create.D6Twist);
            Add(Common.Icon.Get("Slider-Crank"), "D6 Joints", "Swing 1", Simulation, Create.D6Swing1);
            Add(Common.Icon.Get("Slider-Crank"), "D6 Joints", "Swing 2", Simulation, Create.D6Swing2);
            Add(Common.Icon.Get("Slider-Crank"), "D6 Joints", "Isotropic", Simulation, Create.D6Isotropic);
            Add(Common.Icon.Get("Slider-Crank"), "D6 Joints", "Universal", Simulation, Create.D6Universal);

            #endregion

            #region Mechanisms

            Add(Common.Icon.Get("Slider-Crank"), "Mechanisms", "Slider-Crank", Simulation, Create.SliderCrank);
            Add(Common.Icon.Get("Slider-Crank"), "Mechanisms", "Slider-Crank Inverted", Simulation, Create.SliderCrankInverted);
            Add(Common.Icon.Get("Slider-Crank"), "Mechanisms", "Movable Pendulum", Simulation, Create.MovablePendulum);

            #endregion

            #region Robotics

            Add(Common.Icon.Get("Slider-Crank"), "Robotics", "Basic Gripper", Simulation, Create.BasicGripper);

            #endregion

            #region Materials

            Add(Common.Icon.Get("Slider-Crank"), "Materials", "Deformable", Simulation, Create.Deformable);

            #endregion
        }

        #endregion

        #region Properties

        public override ImageSource Logo => Common.Icon.Get("Logo");

        #endregion
    }
}