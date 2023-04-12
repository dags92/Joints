using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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
using Experior.Interfaces;
using PhysX;

namespace Experior.Catalog.Joints.Assemblies.Mechanisms.Test
{
    public class OpcReader : Assembly, IMechanism
    {
        #region Private fields

        private OpcReaderInfo _info;

        private Box _visualizationBox;

        #endregion

        #region Public Properties

        public override string Category { get; } = "External Com";
        public override ImageSource Image { get; } = Common.Icon.Get("OpcReader");
        public string Name { get; } = "OPC Reader";
        public List<string> JointId { get; } = new List<string>();
        public List<string> LinkId { get; } = new List<string>();
        [Category("Input"),
        DisplayName("New Input Data Size")]
        public DataSize InputDataSize { get; set; } = DataSize.LREAL;

        [Category("Input"),
        DisplayName("PLC Joint Values")]
        public ObservableCollection<Input> PlcLinkValues
        {
            get => _info.PlcLinkValues;
            set => _info.PlcLinkValues = value;
        }

        #endregion

        #region Constructor

        public OpcReader(OpcReaderInfo info) : base(info)
        {
            _info = info;

            _visualizationBox = new Box(Colors.DarkRed, 0.5f, 0.5f, 0.5f);
            Add(_visualizationBox);

            if (_info != null)
            {
                foreach (var plcJointValue in PlcLinkValues)
                {
                    Add(plcJointValue);
                }
            }

            PlcLinkValues.CollectionChanged += PlcValues_CollectionChanged;

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
            return link > PlcLinkValues.Count ? Vector3.Zero : Vector3.UnitZ * float.Parse(PlcLinkValues[link].Value.ToString());
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

        #region Private Methods
        private void PlcValues_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (!(sender is ObservableCollection<Input> inputs)) return;
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (var newItem in e.NewItems.OfType<Input>())
                    {
                        newItem.DataSize = InputDataSize;
                        newItem.SymbolName = $"Input Value {inputs.IndexOf(newItem)}";
                        JointId.Add(newItem.SymbolName + " Joint");
                        LinkId.Add(newItem.SymbolName + " Link");
                        Add(newItem);
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (var oldItem in e.OldItems.OfType<Input>())
                    {
                        Remove(oldItem);
                        JointId.Remove(oldItem.SymbolName + " Joint");
                        LinkId.Remove(oldItem.SymbolName + " Link");
                        oldItem.Dispose();
                    }
                    break;
                default:
                    Log.Write(e.Action.ToString() + " Collection event not handled");
                    return;
            }
        }


        #endregion
    }

    [Serializable, XmlInclude(typeof(OpcReaderInfo)),
     XmlType(TypeName = "Experior.Catalog.Joints.Assemblies.Mechanisms.Test.OpcReaderInfo")]
    public class OpcReaderInfo : AssemblyInfo
    {
        public ObservableCollection<Input> PlcLinkValues = new ObservableCollection<Input>();
    }
}
