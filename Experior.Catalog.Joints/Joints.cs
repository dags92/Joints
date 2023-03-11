using System.Windows.Media;

namespace Experior.Catalog.Joints
{
    public class Joints : Experior.Core.Catalog
    {
        public Joints()
            : base("Joints")
        {
            Simulation = Experior.Core.Environment.Simulation.Physics;

            Common.Mesh = new Experior.Core.Resources.EmbeddedResourceLoader(System.Reflection.Assembly.GetExecutingAssembly());
            Common.Icon = new Experior.Core.Resources.EmbeddedImageLoader(System.Reflection.Assembly.GetExecutingAssembly());

            #region Pendulums

            Add(Common.Icon.Get("SinglePendulum"), "Pendulums", "Single", Simulation, Create.Pendulum);
            Add(Common.Icon.Get("DoublePendulum"), "Pendulums", "Double", Simulation, Create.DoublePendulum);

            #endregion
        }

        public override ImageSource Logo => Common.Icon.Get("Logo");
    }
}