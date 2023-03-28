using System.Collections.Generic;
using System.Numerics;

namespace Experior.Catalog.Joints.Assemblies
{
    public interface IMechanism
    {
        string Name { get; }

        List<string> JointId { get; }

        Vector3 GetLinearVelocity(int joint);

        Vector3 GetAngularVelocity(int joint);

        Vector3 GetLinearForce(int joint);

        Vector3 GetAngularForce(int joint);
    }
}
