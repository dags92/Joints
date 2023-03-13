using Experior.Core.Assemblies;
using System;
using System.ComponentModel;
using System.Numerics;
using System.Xml.Serialization;
using Experior.Core.Mathematics;
using Experior.Core.Properties;
using Experior.Core.Properties.TypeConverter;
using Experior.Interfaces;
using PhysX;
using Experior.Core.Parts;

namespace Experior.Catalog.Joints.Assemblies.BasicJoints
{
    public abstract class Base : Assembly
    {
        #region Fields

        private readonly BaseInfo _info;

        #endregion

        #region Constructor

        protected Base(BaseInfo info)
            : base(info)
        {
            _info = info;
        }

        #endregion

        #region Public Properties

        [Category("Dynamic Link")]
        [DisplayName("Length")]
        [PropertyOrder(0)]
        [TypeConverter(typeof(FloatMeterToMillimeter))]
        public float Length
        {
            get => _info.length;
            set
            {
                if (value <= 0 || _info.length.IsEffectivelyEqual(value))
                {
                    return;
                }

                _info.length = value;
                Build();
            }
        }

        [Category("Dynamic Link")]
        [DisplayName("Height")]
        [PropertyOrder(1)]
        [TypeConverter(typeof(FloatMeterToMillimeter))]
        public float Height
        {
            get => _info.height;
            set
            {
                if (value <= 0 || _info.height.IsEffectivelyEqual(value))
                {
                    return;
                }

                _info.height = value;
                Build();
            }
        }

        [Category("Dynamic Link")]
        [DisplayName("Width")]
        [PropertyOrder(2)]
        [TypeConverter(typeof(FloatMeterToMillimeter))]
        public float Width
        {
            get => _info.width;
            set
            {
                if (value <= 0 || _info.width.IsEffectivelyEqual(value))
                {
                    return;
                }

                _info.width = value;
                Build();
            }
        }

        [Category("Dynamic Link")]
        [DisplayName("Center Of Mass")]
        [PropertyOrder(3)]
        [TypeConverter(typeof(Vector3MeterToMillimeter))]
        public Vector3 CenterOfMass
        {
            get => _info.CenterOfMass;
            set
            {
                if (_info.CenterOfMass == value)
                {
                    return;
                }

                _info.CenterOfMass = value;
                Experior.Core.Environment.Invoke(UpdateDynamicLinkProperties);
            }
        }

        [Category("Dynamic Link")]
        [DisplayName("Min. Position Iterations")]
        [PropertyOrder(4)]
        public int MinPositionIterations
        {
            get => _info.MinPositionIterations;
            set
            {
                if (value <= 0)
                {
                    return;
                }

                _info.MinPositionIterations = value;
                Experior.Core.Environment.Invoke(UpdateDynamicLinkProperties);
            }
        }

        [Category("Dynamic Link")]
        [DisplayName("Min. Velocity Iterations")]
        [PropertyOrder(5)]
        public int MinVelocityIterations
        {
            get => _info.MinVelocityIterations;
            set
            {
                if (value <= 0)
                {
                    return;
                }

                _info.MinVelocityIterations = value;
                Experior.Core.Environment.Invoke(UpdateDynamicLinkProperties);
            }
        }

        [Category("Dynamic Link")]
        [DisplayName("Weight")]
        [PropertyOrder(6)]
        [TypeConverter(typeof(Weight))]
        public float Weight
        {
            get => _info.Weight;
            set
            {
                if (value <= 0 || _info.Weight.IsEffectivelyEqual(value))
                {
                    return;
                }

                _info.Weight = value;
                Experior.Core.Environment.Invoke(UpdateDynamicLinkProperties);
            }
        }

        [Category("Frames")]
        [DisplayName("Static Link Translation")]
        [PropertyOrder(0)]
        [TypeConverter(typeof(Vector3MeterToMillimeter))]
        public Vector3 StaticLinkTranslation
        {
            get => _info.StaticLinkTranslation;
            set
            {
                if (_info.StaticLinkTranslation == value)
                {
                    return;
                }

                _info.StaticLinkTranslation = value;
                Build();
            }
        }

        [Category("Frames")]
        [DisplayName("Dynamic Link Translation")]
        [PropertyOrder(1)]
        [TypeConverter(typeof(Vector3MeterToMillimeter))]
        public Vector3 DynamicLinkTranslation
        {
            get => _info.DynamicLinkTranslation;
            set
            {
                if (_info.DynamicLinkTranslation == value)
                {
                    return;
                }

                _info.DynamicLinkTranslation = value;
                Build();
            }
        }

        public override string Category => "Basic Joints";

        #endregion

        #region Protected Properties

        protected Experior.Core.Parts.Static StaticLink { get; set; }

        protected Experior.Core.Loads.Load DynamicLink { get; set; }

        protected PhysX.Joint Joint { get; set; }

        protected AuxiliaryData.JointTypes JointType => _info.JointType;

        #endregion

        #region Public Methods

        public override void Inserted()
        {
            base.Inserted();
            Build();
            Position += new Vector3(0, 1f, 0);
        }

        public override void Dispose()
        {
            Experior.Core.Environment.Invoke(Remove);

            base.Dispose();
        }

        #endregion

        #region Protected Methods

        protected abstract void CreateLinks();

        protected PhysX.JointType JointTypeParser()
        {
            switch (JointType)
            {
                case AuxiliaryData.JointTypes.Revolute:
                    return PhysX.JointType.Revolute;

                case AuxiliaryData.JointTypes.Prismatic:
                    return PhysX.JointType.Prismatic;

                case AuxiliaryData.JointTypes.Spherical:
                    return PhysX.JointType.Spherical;

                default:
                    return PhysX.JointType.Fixed;
            }
        }

        protected virtual void UpdateJointProperties()
        {
            if (Joint == null)
            {
                return;
            }

            Joint.ConstraintFlags |= ConstraintFlag.Visualization;
        }

        #endregion

        #region Private Methods

        private void Build()
        {
            Experior.Core.Environment.Invoke(() =>
            {
                Remove();
                CreateLinks();
                UpdateDynamicLinkProperties();
                Experior.Core.Environment.InvokePhysicsAction(() =>
                {
                    CreateJoint();
                    UpdateJointProperties();
                });
            });
        }

        private void CreateJoint()
        {
            if (StaticLink == null || DynamicLink == null || Joint != null)
            {
                return;
            }

            RigidDynamic staticActor = StaticLink.Actor;
            RigidDynamic dynamicActor = ((Dynamic)DynamicLink.Part).Actor;

            if (staticActor == null || dynamicActor == null)
            {
                Log.Write("Either Static Link or Dynamic Link actor is not present", System.Windows.Media.Colors.Red, LogFilter.Error);
                return;
            }

            var staticLinkFrame = Matrix4x4.CreateTranslation(StaticLinkTranslation);
            var dynamicLinkFrame = Matrix4x4.CreateTranslation(DynamicLinkTranslation);

            Joint = Core.Environment.Scene.PhysXScene.CreateJoint(JointTypeParser(), staticActor, staticLinkFrame, dynamicActor, dynamicLinkFrame);
        }

        private void Remove()
        {
            if (StaticLink != null)
            {
                Remove(StaticLink);
                StaticLink.Dispose();
                StaticLink = null;
            }

            if (DynamicLink != null)
            {
                Experior.Core.Loads.Load.Delete(DynamicLink);
                DynamicLink.Dispose();
                DynamicLink = null;
            }

            if (Joint != null)
            {
                Experior.Core.Environment.InvokePhysicsAction(Joint.Dispose);
                Joint = null;
            }
        }

        private void UpdateDynamicLinkProperties()
        {
            if (DynamicLink == null)
            {
                return;
            }

            DynamicLink.Part.MinPositionIterations = MinPositionIterations;
            DynamicLink.Part.MinVelocityIterations = MinVelocityIterations;
            DynamicLink.CenterOfMassOffsetLocalPosition = CenterOfMass;
            DynamicLink.Weight = 1f;
        }

        #endregion
    }

    [Serializable, XmlInclude(typeof(BaseInfo)), XmlType(TypeName = "Experior.Catalog.Joints.Assemblies.BasicJoints.BaseInfo")]
    public class BaseInfo : Experior.Core.Assemblies.AssemblyInfo
    {
        public AuxiliaryData.JointTypes JointType { get; set; }

        public Vector3 StaticLinkTranslation { get; set; } = Vector3.Zero;

        public Vector3 DynamicLinkTranslation { get; set; } = new Vector3(0f, 0.5f, 0f);

        public Vector3 CenterOfMass { get; set; } = Vector3.Zero;

        public int MinPositionIterations { get; set; } = 20;

        public int MinVelocityIterations { get; set; } = 5;

        public float Weight { get; set; } = 1f;
    }
}