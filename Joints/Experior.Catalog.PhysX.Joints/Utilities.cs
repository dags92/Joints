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
    }
}
