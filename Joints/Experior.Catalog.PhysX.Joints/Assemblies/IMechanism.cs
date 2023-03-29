using System.Collections.Generic;
using System.Numerics;

namespace Experior.Catalog.Joints.Assemblies
{
    public interface IMechanism
    {
        string Name { get; }

        List<string> JointId { get; }

        List<string> LinkId { get; }

        Vector3 GetJointLinearVelocity(int joint);

        Vector3 GetJointAngularVelocity(int joint);

        Vector3 GetJointLinearForce(int joint);

        Vector3 GetJointAngularForce(int joint);

        Vector3 GetLinkLinearVelocity(int link);

        Vector3 GetLinkAngularVelocity(int link);
    }
}
