namespace SixR_20.Models
{
    public enum CommandsEnum
    {

            SetControlWord = 0,
            SetMode,
            SetTargetPosition,
            SetTargetVelocity,
            StartTrajectory,
            StopTrajectory,
            SetProfileVelocity,
            SwitchPower,
            MoveAbs,
            Go2Home,
            ResetTrajectoryVariables,
            Jog,
            JogStop,
            JogTrajectory,
            JogTrajectortStop,
            ClearServoAlarms = 70,
            SwitchBreaks,
            WriteRegister = 80
    }
}
