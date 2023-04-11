using Experior.Core.Assemblies;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;
using System.Windows.Media;
using System.Xml.Serialization;
using Experior.Core.Mathematics;
using PhysX;
using Experior.Interfaces;
using System.Linq;
using System.Windows.Forms;

namespace Experior.Catalog.Joints.Assemblies
{
    /// <summary>
    /// Class <c>Base</c> provides the fundamental class members to build a Mechanism composed by PhysX joints.
    /// </summary>
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

        #region Delegates

        public delegate void MechanismBuilt(List<Link> l, List<Joint> j);

        #endregion

        #region Events

        public event MechanismBuilt BuiltDone;

        #endregion

        #region Public Properties

        [Browsable(false)]
        public List<string> JointId { get; } = new List<string>();

        [Browsable(false)]
        public List<string> LinkId { get; } = new List<string>();

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

        public void Rebuild()
        {
            Experior.Core.Environment.Invoke(RemoveAll);
            Build();
        }

        public Vector3 GetJointLinearVelocity(int joint)
        {
            return joint > _joints.Count ? Vector3.Zero : _joints[joint].GetRelativeLinearVelocity();
        }

        public Vector3 GetJointAngularVelocity(int joint)
        {
            return joint > _joints.Count ? Vector3.Zero : _joints[joint].GetRelativeAngularVelocity();
        }

        public Vector3 GetJointLinearForce(int joint)
        {
            return joint > _joints.Count ? Vector3.Zero : _joints[joint].Constraint.LinearForce;
        }

        public Vector3 GetJointAngularForce(int joint)
        {
            return joint > _joints.Count ? Vector3.Zero : _joints[joint].Constraint.AngularForce;
        }

        public Vector3 GetLinkLinearVelocity(int link)
        {
            return link > _links.Count ? Vector3.Zero : _links[link].LinkDynamic.LinearVelocity;
        }

        public Vector3 GetLinkAngularVelocity(int link)
        {
            return link > _links.Count ? Vector3.Zero : _links[link].LinkDynamic.AngularVelocity;
        }

        public Vector3 GetLinkLocalPosition(int link)
        {
            if (_links.Count < link)
            {
                return Vector3.Zero;
            }

            Trigonometry.GlobalToLocal(Position, Orientation, _links[link].LinkDynamic.Position, _links[link].LinkDynamic.Orientation, out var pos, out var ori);

            return pos;
        }

        public Vector3 GetLinkGlobalPosition(int link)
        {
            return link > _links.Count ? Vector3.Zero : _links[link].LinkDynamic.Position;
        }

        public Matrix4x4 GetLocalLinkOrientation(int link)
        {
            if (_links.Count < link)
            {
                return Matrix4x4.Identity;
            }

            Trigonometry.GlobalToLocal(Position, Orientation, _links[link].LinkDynamic.Position, _links[link].LinkDynamic.Orientation, out var pos, out var ori);

            return ori;
        }

        #endregion

        #region Protected Methods

        protected abstract void CreateLinks();

        protected virtual void ConfigureLinks()
        {
            foreach (var link in Links)
            {
                link.LinkDynamic.Deletable = false;
                link.LinkDynamic.UserDeletable = false;

                link.LinkDynamic.Part.MinPositionIterations = 20;
                link.LinkDynamic.Part.MinVelocityIterations = 5;

                link.LinkDynamic.SleepThreshold = 0f;
            }
        }

        protected abstract void CreateJoints();

        protected virtual void ConfigureJoints()
        {
            foreach (var joint in Joints)
            {
                joint.Constraint.Flags |= ConstraintFlag.Visualization;
            }
        }

        #endregion

        #region Private Methods

        private void Build()
        {
            Experior.Core.Environment.Invoke(() =>
            {
                CreateLinks();
                ConfigureLinks();

                Experior.Core.Environment.InvokePhysicsAction(() =>
                {
                    if (!VerifyActors())
                    {
                        return; 
                    }

                    CreateJoints();
                    ConfigureJoints();
                });

                BuiltDone?.Invoke(Links, Joints);
            });
        }

        private void RemoveAll()
        {
            foreach (var link in Links)
            {
                Experior.Core.Loads.Load.Items.Remove(link.LinkDynamic);
                link.LinkDynamic?.Dispose();
            }
            Links.Clear();

            Experior.Core.Environment.InvokePhysicsAction(() =>
            {
                foreach (var joint in Joints)
                {
                    joint.Dispose();
                }
                Joints.Clear();
            });
        }

        private bool VerifyActors()
        {
            if (Links.Any(a => a.LinkActor == null))
            {
                Log.Write("Physics Engine Action has not been invoked properly", Colors.Red, LogFilter.Error);
                return false;
            }

            return true;
        }

        #endregion
    }

    [Serializable, XmlInclude(typeof(BaseInfo)), XmlType(TypeName = "Experior.Catalog.Joints.Assemblies.BaseInfo")]
    public class BaseInfo : Experior.Core.Assemblies.AssemblyInfo
    {
    }
}