using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Windows.Media;
using System.Xml.Serialization;
using Experior.Catalog.Joints.Assemblies.Mechanisms;
using Experior.Core.Communication.PLC;
using Experior.Core.Mathematics;
using Experior.Core.Motors;
using Experior.Core.Properties;
using Experior.Core.Properties.TypeConverter;
using Experior.Interfaces;
using Environment = Experior.Core.Environment;

namespace Experior.Catalog.Joints.Actuators.Motors
{
    /// <summary>
    /// Abstract class <c>SplittableStraight</c> contains common class members and behaviors to recreate a Forward/Backward motor functionality.
    /// </summary>
    public abstract class Base : Electric
    {
        #region Fields

        private readonly BaseInfo _info;

        private bool _move;
        private AuxiliaryData.Commands _command;

        #endregion

        #region Constructor

        protected Base(BaseInfo info) : base(info)
        {
            _info = info;

            SetPlcSignals();
            Motion = new Motion { EnableAcceleration = info.UseRamp };

            _command = AuxiliaryData.Commands.Forward;
        }

        #endregion

        #region Events

        public EventHandler<float> TargetSpeedReached;

        #endregion

        #region Public Properties

        [Browsable(false)]
        public override float Speed { get; set; }

        /// <summary>
        /// Gets or sets the speed. 
        /// </summary>
        [Display(Order = 1, GroupName = "Speed")]
        [DisplayName(@"Speed")]
        [PropertyOrder(1)]
        public virtual float BaseSpeed
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

        /// <summary>
        /// Gets or sets the use of ramp for acceleration and deceleration states.
        /// </summary>
        [Display(Order = 1, GroupName = "Acceleration/Deceleration")]
        [DisplayName(@"Enabled")]
        [PropertyOrder(3)]
        public bool UseRamp
        {
            get => _info.UseRamp;
            set
            {
                _info.UseRamp = value;

                if (Motion != null)
                {
                    Motion.EnableAcceleration = value;
                }

                Environment.Properties.Refresh();
            }
        }

        /// <summary>
        /// Gets or sets the ramp value implemented in acceleration.
        /// </summary>
        [Display(Order = 1, GroupName = "Acceleration/Deceleration")]
        [DisplayName(@"Ramp Up")]
        [PropertyAttributesProvider("DynamicPropertyAcceleration")]
        [TypeConverter(typeof(MilliSeconds))]
        [PropertyOrder(4)]
        public float RampUp
        {
            get => _info.RampUp;
            set
            {
                if (RampDown.IsEffectivelyEqual(0f) && value.IsEffectivelyEqual(0f))
                {
                    UseRamp = false;
                    return;
                }

                if (value < 0)
                {
                    return;
                }

                _info.RampUp = value;
            }
        }

        /// <summary>
        /// Gets or sets the ramp value implemented in deceleration.
        /// </summary>
        [Display(Order = 1, GroupName = "Acceleration/Deceleration")]
        [DisplayName(@"Ramp Down")]
        [PropertyAttributesProvider("DynamicPropertyAcceleration")]
        [TypeConverter(typeof(MilliSeconds))]
        [PropertyOrder(5)]
        public float RampDown
        {
            get => _info.RampDown;
            set
            {
                if (RampUp.IsEffectivelyEqual(0f) && value.IsEffectivelyEqual(0f))
                {
                    UseRamp = false;
                    return;
                }

                if (value < 0)
                {
                    return;
                }

                _info.RampDown = value;
            }
        }

        /// <summary>
        /// Gets or sets the Forward PLC Input signal.
        /// This instance move the motor in forward direction when its value is true.
        /// </summary>
        [Category("PLC Output")]
        [DisplayName(@"Activate")]
        [PropertyOrder(1)]
        public Input InputActivate
        {
            get => _info.InputActivate;
            set => _info.InputActivate = value;
        }

        /// <summary>
        /// Gets or sets Current Speed.
        /// </summary>
        [Browsable(false)]
        public override float CurrentSpeed
        {
            get => base.CurrentSpeed;
            protected set
            {
                base.CurrentSpeed = value;

                if (!value.IsEffectivelyZero())
                {
                    LastSpeed = value;
                }
            }
        }

        /// <summary>
        /// Indicates if the motor is running.
        /// </summary>
        public override bool Running => !CurrentSpeed.IsEffectivelyZero();

        /// <summary>
        /// Gets the direction in which the motor is running.
        /// </summary>
        [Browsable(false)]
        public override MotorDirection Direction =>
            Command == AuxiliaryData.Commands.Forward || Command == AuxiliaryData.Commands.Stop
                ? MotorDirection.Forward
                : MotorDirection.Backward;

        #endregion

        #region Protected Properties

        /// <summary>
        /// Gets or sets the Target Speed the motor must reach.
        /// </summary>
        protected internal float TargetSpeed { get; internal set; }

        /// <summary>
        /// Gets or sets the Last Target Speed.
        /// </summary>
        protected internal float LastSpeed { get; internal set; }

        /// <summary>
        /// Instance <c>Move</c> allows the motion of the motor and notifies the Belt when it is about to start or stop.
        /// </summary>
        protected internal bool Move
        {
            get => _move;
            internal set
            {
                _move = value;

                if (CurrentSpeed != 0f)
                    return;

                if (value)
                {
                    InvokeBeginStart();
                    InvokeStarted();
                }
                else
                    InvokeStopped();
            }
        }

        /// <summary>
        /// Instance <c>Command</c> indicates the command to execute.
        /// </summary>
        protected internal AuxiliaryData.Commands Command
        {
            get => _command;
            private set
            {
                _command = value;

                if (Running)
                    Controller();
            }
        }

        /// <summary>
        /// Instance <c>Motion</c> takes care of the calculations related to the motion of the motor taking into account acceleration and deceleration.
        /// </summary>
        protected readonly Motion Motion;

        #endregion

        #region Public Methods

        public override void Step(float deltatime)
        {
            if (!Move)
            {
                return;
            }

            CurrentSpeed = Motion.Step(deltatime);

            if (CurrentSpeed.IsEffectivelyEqual(TargetSpeed))
            {
                Move = false;
                TargetSpeedReached?.Invoke(this, CurrentSpeed);

                UpdateColor(TargetSpeed.IsEffectivelyZero() ? Colors.Red : Colors.Green);
            }
            else
            {
                UpdateColor(Colors.Orange);
            }

            if (CurrentSpeed >= 0f)
            {
                SetForward();
            }
            else
            {
                SetBackward();
            }
        }

        public override void Start()
        {
            // Started from context menu
            if (Command == AuxiliaryData.Commands.Stop)
            {
                Command = LastSpeed >= 0f ? AuxiliaryData.Commands.Forward : AuxiliaryData.Commands.Backward;
            }

            Controller();
        }

        public override void Forward()
        {
            Command = AuxiliaryData.Commands.Forward;
        }

        public override void Backward()
        {
            Command = AuxiliaryData.Commands.Backward;
        }

        public override void Stop()
        {
            Command = AuxiliaryData.Commands.Stop;
        }

        public override void StopBreak()
        {
            Move = false;

            TargetSpeed = 0f;
            CurrentSpeed = 0f;
            Motion.Reset();

            _command = AuxiliaryData.Commands.Forward;
            UpdateColor(Colors.Red);
            SetForward();

            Environment.Invoke(InvokeStopped);
            Script.Logic.Motor.Stopped(this);
        }

        public override void SwitchDirection()
        {
            switch (Command)
            {
                case AuxiliaryData.Commands.Forward:
                    Backward();
                    break;
                case AuxiliaryData.Commands.Backward:
                    Forward();
                    break;
            }
        }

        public override void Reset()
        {
            StopBreak();
        }

        public override void Dispose()
        {
            _info.InputActivate.On -= InputActivateOn;
            _info.InputActivate.Off -= InputActivateOff;

            base.Dispose();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void DynamicPropertyAcceleration(PropertyAttributes attributes)
        {
            attributes.IsBrowsable = _info.UseRamp;
        }

        #endregion

        #region Protected Methods

        /// < summary>
        /// This method defines the Target Speed based on the I/Os and activates the motion of the motor.
        /// </summary>
        protected virtual void Controller()
        {
            TargetSpeed = BaseSpeed;
            TargetSpeed *= (int)Command;

            Motion.SetTargetSpeed(TargetSpeed, RampUp / 1000f, RampDown / 1000f);
            Move = true;
        }

        #endregion

        #region Private Methods

        private void SetPlcSignals()
        {
            if (_info.InputActivate == null)
            {
                _info.InputActivate = new Input { DataSize = DataSize.BOOL, Description = "Activate Motor", Symbol = "Activate" };
            }

            Add(_info.InputActivate);

            _info.InputActivate.On += InputActivateOn;
            _info.InputActivate.Off += InputActivateOff;
        }

        private void InputActivateOn(Input sender)
        {
            if (Command == AuxiliaryData.Commands.Forward && Move)
            {
                return;
            }

            Forward();

            if (!Running)
            {
                Start();
            }
        }

        private void InputActivateOff(Input sender)
        {
            if (Command != AuxiliaryData.Commands.Stop)
            {
                Stop();
            }
        }

        #endregion
    }

    [Serializable, XmlInclude(typeof(SliderCrankInfo)), XmlType(TypeName = "Experior.Catalog.Joints.Actuators.Motor.BaseInfo")]
    public class BaseInfo : ElectricInfo
    {
        public float BaseSpeed { get; set; } = 0.3f;

        public float RampUp { get; set; } = 300f;

        public float RampDown { get; set; } = 300f;

        public bool UseRamp { get; set; } = true;

        public Input InputActivate { get; set; }
    }
}
