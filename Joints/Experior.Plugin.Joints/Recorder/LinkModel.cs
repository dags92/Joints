using System.Collections.Generic;
using System.Numerics;

namespace Experior.Plugin.Joints.Recorder
{
    public class LinkModel
    {
        #region Fields

        private readonly List<Vector3> _linearVelocity = new List<Vector3>();

        private readonly List<Vector3> _angularVelocity = new List<Vector3>();

        private readonly List<Vector3> _localPosition = new List<Vector3>();

        private readonly List<Matrix4x4> _localOrientation = new List<Matrix4x4>();

        #endregion

        #region Constructor

        public LinkModel(string name)
        {
            Name = name;
        }

        #endregion

        #region Public Properties

        public string Name { get; }

        #endregion

        #region Public Methods

        public void AddLinearVelocity(Vector3 velocity)
        {
            _linearVelocity.Add(velocity);
        }

        public void AddAngularVelocity(Vector3 velocity)
        {
            _angularVelocity.Add(velocity);
        }

        public void AddLocalPosition(Vector3 velocity)
        {
            _localPosition.Add(velocity);
        }

        public void AddLocalOrientation(Matrix4x4 orientation)
        {
            _localOrientation.Add(orientation);
        }

        public List<Vector3> GetLinearVelocity()
        {
            return _linearVelocity;
        }

        public List<Vector3> GetAngularVelocity()
        {
            return _angularVelocity;
        }

        public List<Vector3> GetLocalPosition()
        {
            return _localPosition;
        }

        public List<Matrix4x4> GetLocalOrientation()
        {
            return _localOrientation;
        }

        #endregion
    }
}