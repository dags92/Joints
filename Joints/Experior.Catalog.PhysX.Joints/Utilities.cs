using System.Numerics;
using PhysX;

namespace Experior.Catalog.Joints
{
    public static class Utilities
    {
        public static PhysX.RevoluteJointFlag GetRevoluteJointFlag(AuxiliaryData.RevoluteDriveTypes drive)
        {
            return drive == AuxiliaryData.RevoluteDriveTypes.FreeSpin
                ? RevoluteJointFlag.DriveFreeSpin
                : RevoluteJointFlag.DriveEnabled;
        }

        public static Matrix4x4 GetPose(Vector3 translation, float yaw, float pitch, float roll)
        {
            var pose = new Matrix4x4();

            pose.M41 = translation.X;
            pose.M42 = translation.Y;
            pose.M43 = translation.Z;

            var orientation = Matrix4x4.CreateFromYawPitchRoll(yaw, pitch, roll);

            return pose + orientation;
        }

        public static void SetLocalFrames(PhysX.Joint joint, Matrix4x4 localFrame0, Matrix4x4 localFrame1)
        {
            if (Matrix4x4.Decompose(localFrame0, out var scale0, out var quaternion0, out var translation0))
            {
                joint.LocalPose0 = new PhysX.Transform(quaternion0, translation0);
            }

            if (Matrix4x4.Decompose(localFrame1, out var scale1, out var quaternion1, out var translation1))
            {
                joint.LocalPose1 = new PhysX.Transform(quaternion1, translation1);
            }
        }
    }
}
