using Experior.Core.Assemblies;

namespace Experior.Catalog.PhysX.Joints
{
    public class Create
    {
        public static Assembly TemplateAssembly(string group, string title, object properties)
        {
            var info = new Assemblies.TemplateAssemblyInfo
            {
                name = Assembly.GetValidName("Template Assembly")
            };
            return new Assemblies.TemplateAssembly(info);
        }
    }
}