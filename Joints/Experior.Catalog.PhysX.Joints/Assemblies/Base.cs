using Experior.Core.Assemblies;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;
using System.Windows.Media;
using System.Xml.Serialization;
using PhysX;

namespace Experior.Catalog.Joints.Assemblies
{
    public abstract class Base : Assembly, IMechanism
    {
        #region Fields

        private BaseInfo _info;

        private readonly List<Link> _links = new List<Link>();
        private readonly List<Joint> _joints = new List<Joint>();

        #endregion

        #region Constructor

        protected Base(BaseInfo info) : base(info)
        {
            _info = info;

            var sphere = new Experior.Core.Parts.Sphere(Colors.Black, 0.015f, 8);
            Add(sphere, new Vector3(0f, 0.2f, 0f));
        }

        #endregion

        #region Public Properties

        [Browsable(false)]
        public List<string> JointId { get; } = new List<string>();

        #endregion

        #region Protected Properties

        protected List<Link> Links => _links;

        protected List<Joint> Joints => _joints;

        #endregion

        #region Public Methods

        public override void Inserted()
        {
            base.Inserted();

            Build();
        }

        public Vector3 GetLinearVelocity(int joint)
        {
            return joint > _joints.Count ? Vector3.Zero : _joints[joint].GetRelativeLinearVelocity();
        }

        public Vector3 GetAngularVelocity(int joint)
        {
            return joint > _joints.Count ? Vector3.Zero : _joints[joint].GetRelativeAngularVelocity();
        }

        public Vector3 GetLinearForce(int joint)
        {
            return joint > _joints.Count ? Vector3.Zero : _joints[joint].Constraint.LinearForce;
        }

        public Vector3 GetAngularForce(int joint)
        {
            return joint > _joints.Count ? Vector3.Zero : _joints[joint].Constraint.AngularForce;
        }

        #endregion

        #region Protected Methods

        protected abstract void CreateLinks();

        protected abstract void CreateJoints();

        protected abstract void ConfigureJoints();

        #endregion

        #region Private Methods

        private void Build()
        {
            Experior.Core.Environment.InvokeIfRequired(() =>
            {
                CreateLinks();

                Experior.Core.Environment.InvokePhysicsAction(() =>
                {
                    CreateJoints();
                    ConfigureJoints();
                });
            });
        }

        #endregion
    }

    [Serializable, XmlInclude(typeof(BaseInfo)), XmlType(TypeName = "Experior.Catalog.Joints.Assemblies.BaseInfo")]
    public class BaseInfo : Experior.Core.Assemblies.AssemblyInfo
    {
    }
}