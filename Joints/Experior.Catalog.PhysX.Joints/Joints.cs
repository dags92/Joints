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

            Add(Common.Icon.Get("Slider-Crank"), "Basic Joints", "Revolute", Simulation, Create.Revolute);
            Add(Common.Icon.Get("Slider-Crank"), "Basic Joints", "Prismatic", Simulation, Create.Prismatic);
            Add(Common.Icon.Get("Slider-Crank"), "Basic Joints", "Fixed", Simulation, Create.Fixed);
            Add(Common.Icon.Get("Slider-Crank"), "Basic Joints", "Spherical", Simulation, Create.Spherical);
            Add(Common.Icon.Get("Slider-Crank"), "Basic Joints", "Distance", Simulation, Create.Distance);

            #endregion

            #region D6 Joints

            Add(Common.Icon.Get("Slider-Crank"), "D6 Joints", "Revolute", Simulation, Create.D6Revolute);
            Add(Common.Icon.Get("Slider-Crank"), "D6 Joints", "Prismatic", Simulation, Create.D6Prismatic);

            #endregion

            #region Mechanisms

            Add(Common.Icon.Get("Slider-Crank"), "Mechanisms", "Slider-Crank", Simulation, Create.SliderCrank);
            Add(Common.Icon.Get("Slider-Crank"), "Mechanisms", "Slider-Crank Inverted", Simulation, Create.SliderCrankInverted);
            Add(Common.Icon.Get("Slider-Crank"), "Mechanisms", "Movable Pendulum", Simulation, Create.MovablePendulum);

            #endregion

            #region Robotics

            Add(Common.Icon.Get("Slider-Crank"), "Robotics", "Basic Gripper", Simulation, Create.BasicGripper);

            #endregion
        }

        #endregion

        #region Properties

        public override ImageSource Logo => Common.Icon.Get("Logo");

        #endregion
    }
}