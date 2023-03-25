using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Xml.Serialization;
using Experior.Core.Properties;

namespace Experior.Catalog.Joints.Actuators.Motors
{
    public class Rotative : Base
    {
        #region Fields

        private readonly RotativeInfo _info;

        #endregion

        #region Constructor

        public Rotative(RotativeInfo info) : base(info)
        {
            _info = info;
        }

        #endregion

        #region Public Properties

        [Display(Order = 1, GroupName = "Speed")]
        [DisplayName(@"Speed (rad/s)")]
        [PropertyOrder(1)]
        public override float BaseSpeed
        {
            get => _info.BaseSpeed;
            set
            {
                if (value <= 0)
                    return;

                _info.BaseSpeed = value;
                if (Running)
                {
                    Controller();
                }
            }
        }

        #endregion

        #region Public Methods

        public static Rotative Create()
        {
            var info = new RotativeInfo()
            {
                name = Experior.Core.Motors.Motor.GetValidName("Joint - Rotative Motor"),
                BaseSpeed = 0.5f, // rad/s
                embedded = true
            };

            return new Rotative(info);
        }

        #endregion
    }

    [Serializable, XmlInclude(typeof(RotativeInfo)), XmlType(TypeName = "Experior.Catalog.Joints.Actuators.Motor.RotativeInfo")]
    public class RotativeInfo : BaseInfo
    {

    }
}
