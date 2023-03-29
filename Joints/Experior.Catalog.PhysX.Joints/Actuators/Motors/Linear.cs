using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Xml.Serialization;
using Experior.Core.Properties.TypeConverter;
using Experior.Core.Properties;

namespace Experior.Catalog.Joints.Actuators.Motors
{
    public class Linear : Base
    {
        #region Fields

        private readonly LinearInfo _info;

        #endregion

        #region Constructor

        public Linear(LinearInfo info)
            : base(info)
        {
            _info = info;
        }

        #endregion

        #region Public Properties

        [Display(Order = 1, GroupName = "Speed")]
        [DisplayName(@"Speed (rad/s)")]
        [TypeConverter(typeof(MeterPerSeconds))]
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

        public static Linear Create()
        {
            var info = new LinearInfo()
            {
                name = Experior.Core.Motors.Motor.GetValidName("Joint - Linear Motor"),
                BaseSpeed = 0.2f // m/s
            };

            return new Linear(info);
        }

        #endregion
    }

    [Serializable, XmlInclude(typeof(LinearInfo)), XmlType(TypeName = "Experior.Catalog.Joints.Actuators.Motor.LinearInfo")]
    public class LinearInfo : BaseInfo
    {
    }
}
