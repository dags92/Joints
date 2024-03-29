﻿using System.Collections.Generic;
using System.Numerics;

namespace Experior.Plugin.Joints.Recorder
{
    public class JointModel //TODO: ITERATOR PATTERN ?
    {
        #region Fields

        private readonly List<Vector3> _linearVelocity = new List<Vector3>();

        private readonly List<Vector3> _angularVelocity = new List<Vector3>();

        private readonly List<Vector3> _linearForce = new List<Vector3>();

        private readonly List<Vector3> _angularForce = new List<Vector3>();

        #endregion

        #region Constructor

        public JointModel(string name)
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

        public List<Vector3> GetLinearVelocity()
        {
            return _linearVelocity;
        }

        public List<Vector3> GetAngularVelocity()
        {
            return _angularVelocity;
        }

        public void AddLinearForce(Vector3 velocity)
        {
            _linearForce.Add(velocity);
        }

        public void AddAngularForce(Vector3 velocity)
        {
            _angularForce.Add(velocity);
        }

        public List<Vector3> GetLinearForce()
        {
            return _linearForce;
        }

        public List<Vector3> GetAngularForce()
        {
            return _angularForce;
        }

        #endregion
    }
}
