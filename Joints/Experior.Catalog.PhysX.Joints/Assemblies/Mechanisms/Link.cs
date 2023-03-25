using System.ComponentModel;
using System.Numerics;
using Experior.Core;
using Experior.Core.Loads;
using Experior.Core.Parts;

namespace Experior.Catalog.Joints.Assemblies.Mechanisms
{
    public class Link
    {
        #region Fields

        private Matrix4x4 _relativeLocalFrame = Matrix4x4.Identity;

        private Matrix4x4 _jointLocalFrame = Matrix4x4.Identity;

        private int _minPositionIterations = 20;

        private int _minVelocityIterations = 5;

        private float _weight = 0.2f;

        #endregion

        #region Constructor

        public Link(Load link, bool kinematic)
        {
            LinkDynamic = link;
            LinkDynamic.Kinematic = kinematic;

            Environment.InvokePhysicsAction(() =>
            {
                LinkActor = ((Dynamic)LinkDynamic.Part).Actor;
            });

            UpdateProperties();
        }

        #endregion

        #region Public Properties

        [Browsable(false)]
        public PhysX.RigidActor LinkActor { get; private set; }

        [Browsable(false)]
        public Load LinkDynamic { get; }

        [Browsable(false)]
        public Matrix4x4 RelativeLocalFrame
        {
            get => _relativeLocalFrame;
            set => _relativeLocalFrame = value;
        }

        [Browsable(false)]
        public Matrix4x4 JointLocalFrame
        {
            get => _jointLocalFrame;
            set => _jointLocalFrame = value;
        }

        public int MinPositionIterations
        {
            get => _minPositionIterations;
            set
            {
                if (value < 4)
                {
                    return;
                }

                _minPositionIterations = value;
                UpdateProperties();
            }
        }

        public int MinVelocityIterations
        {
            get => _minVelocityIterations;
            set
            {
                if (value < 1)
                {
                    return;
                }

                _minVelocityIterations = value;
                UpdateProperties();
            }
        }

        public float Weight
        {
            get => _weight;
            set
            {
                if (value <= 0)
                {
                    return;
                }

                _weight = value;
                UpdateProperties();
            }
        }

        #endregion

        #region Private Methods

        private void UpdateProperties()
        {
            Environment.InvokeIfRequired(() =>
            {
                LinkDynamic.Part.MinPositionIterations = MinPositionIterations;
                LinkDynamic.Part.MinVelocityIterations = MinVelocityIterations;
                LinkDynamic.Part.Weight = Weight;
            });
        }

        #endregion
    }
}
