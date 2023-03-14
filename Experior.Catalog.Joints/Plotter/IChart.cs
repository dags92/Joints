using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Experior.Catalog.Joints.Plotter
{
    public interface IChart
    {
        System.Windows.Forms.DataVisualization.Charting.Chart Chart { get; }
    }
}
