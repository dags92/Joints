using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Xml.Serialization;
using Experior.Core.Assemblies;
using Experior.Core.Communication.PLC;
using Experior.Core.Parts;
using Experior.Core.Routes;
using PhysX;

namespace Experior.Catalog.Joints.Assemblies
{
    public class OpcReader : Assembly, IMechanism
    {
        #region Private fields

        private OpcReaderInfo _info;

        private Experior.Core.Parts.Box _visualizationBox;

        #endregion

        #region Public Properties

        public override string Category { get; } = "External Com";
        public override ImageSource Image { get; } = Common.Icon.Get("OpcReader");
        public string Name { get; } = "OPC Reader";
        public List<string> JointId { get; } = new List<string>();
        public List<string> LinkId { get; } = new List<string>();

        [Category("Input"),
        DisplayName("PLC Joint Values")]
        public List<Input> PlcJointValues
        {
            get => _info.PlcJointValues;
            set => _info.PlcJointValues = value;
        }

        #endregion

        #region Constructor

        public OpcReader(OpcReaderInfo info) : base(info)
        {
            _info = info;

            _visualizationBox = new Box(Colors.DarkRed, 0.5f, 0.5f, 0.5f);
        }

        #endregion
        #region Public Methods

        public Vector3 GetJointLinearVelocity(int joint)
        {
            return Vector3.Zero;
        }

        public Vector3 GetJointAngularVelocity(int joint)
        {
            return Vector3.Zero;
        }

        public Vector3 GetJointLinearForce(int joint)
        {
            return Vector3.Zero;
        }

        public Vector3 GetJointAngularForce(int joint)
        {
            return Vector3.Zero;
        }

        public Vector3 GetLinkLinearVelocity(int link)
        {
            return Vector3.Zero;
        }

        public Vector3 GetLinkAngularVelocity(int link)
        {
            return Vector3.Zero;
        }

        public Vector3 GetLinkLocalPosition(int link)
        {
            return link > PlcJointValues.Count ? Vector3.Zero : Vector3.UnitZ * (float)PlcJointValues[link].Value;
        }

        public Vector3 GetLinkGlobalPosition(int link)
        {
            return Vector3.Zero;
        }

        public Matrix4x4 GetLocalLinkOrientation(int link)
        {
            return Matrix4x4.Identity;
        }

        #endregion
    }

    [Serializable, XmlInclude(typeof(OpcReaderInfo)),
     XmlType(TypeName = "Experior.Catalog.Joints.Assemblies.OpcReaderInfo")]
    public class OpcReaderInfo : AssemblyInfo
    {
        public List<Input> PlcJointValues = new List<Input>();
    }
}
