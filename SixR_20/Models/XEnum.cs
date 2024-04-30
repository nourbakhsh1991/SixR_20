using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SixR_20.Models
{
    public enum X
    {
        [Type(TypeAttribute.Types.Int, TypeAttribute.RW.FullControll, length: SixRConstants.NumberOfAxis)]
        GUI_CtrWord,
        [Type(TypeAttribute.Types.DInt, TypeAttribute.RW.FullControll, length: SixRConstants.NumberOfAxis)]
        GUI_TargetPosition,
        [Type(TypeAttribute.Types.SInt, TypeAttribute.RW.FullControll, length: SixRConstants.NumberOfAxis)]
        GUI_ModesOfOperation,
        [Type(TypeAttribute.Types.DInt, TypeAttribute.RW.ReadOnly, length: SixRConstants.NumberOfAxis, notify: true)]
        GUI_ActualPositions,
        [Type(TypeAttribute.Types.DInt, TypeAttribute.RW.FullControll, length: SixRConstants.NumberOfAxis)]
        GUI_ProfileVelocity,
        [Type(TypeAttribute.Types.DInt, TypeAttribute.RW.FullControll, length: SixRConstants.NumberOfAxis)]
        GUI_TargetVelocity,
        [Type(TypeAttribute.Types.Int, TypeAttribute.RW.FullControll)]
        GUI_Command,
        [Type(TypeAttribute.Types.Int, TypeAttribute.RW.FullControll)]
        GUI_MotorNum,
        [Type(TypeAttribute.Types.Bool, TypeAttribute.RW.FullControll, length: SixRConstants.NumberOfAxis)]
        GUI_MSelect,
        [Type(TypeAttribute.Types.DInt, TypeAttribute.RW.WriteOnly, length: SixRConstants.NumberOfAxis * SixRConstants.BufferLen)]
        BufferPos1,
        [Type(TypeAttribute.Types.DInt, TypeAttribute.RW.WriteOnly, length: SixRConstants.NumberOfAxis * SixRConstants.BufferLen)]
        BufferPos2,
        [Type(TypeAttribute.Types.DInt, TypeAttribute.RW.FullControll, length: SixRConstants.NumberOfAxis * SixRConstants.BufferLen)]
        BufferPosAct1,
        [Type(TypeAttribute.Types.DInt, TypeAttribute.RW.FullControll, length: SixRConstants.NumberOfAxis * SixRConstants.BufferLen)]
        BufferPosAct2,
        [Type(TypeAttribute.Types.Int, TypeAttribute.RW.ReadOnly, notify: true)]
        BufferNumber,
        [Type(TypeAttribute.Types.UDInt, TypeAttribute.RW.FullControll)]
        TrajLen,
        [Type(TypeAttribute.Types.Bool, TypeAttribute.RW.FullControll, notify: true)]
        Pulse,
        [Type(TypeAttribute.Types.SInt, TypeAttribute.RW.FullControll, notify: true)]
        GUI_Flags,
        [Type(TypeAttribute.Types.Word, TypeAttribute.RW.FullControll)]
        GUI_RegisterAdress,
        [Type(TypeAttribute.Types.UDInt, TypeAttribute.RW.FullControll)]
        GUI_RegisterValue,
        [Type(TypeAttribute.Types.Bool, TypeAttribute.RW.FullControll, length: SixRConstants.NumberOfAxis, notify: true)]
        GUI_MotorBreaks,
        [Type(TypeAttribute.Types.Int, TypeAttribute.RW.ReadOnly, length: SixRConstants.NumberOfAxis)]
        GUI_Statusword,
        [Type(TypeAttribute.Types.Int, TypeAttribute.RW.ReadOnly, length: SixRConstants.NumberOfAxis, notify: true)]
        GUI_ErrorCode,
        [Type(TypeAttribute.Types.UDInt, TypeAttribute.RW.FullControll)]
        GUI_Counter,
        [Type(TypeAttribute.Types.Int, TypeAttribute.RW.FullControll)]
        GUI_StopingJog,
        [Type(TypeAttribute.Types.Int, TypeAttribute.RW.FullControll, length: SixRConstants.NumberOfAxis)]
        GUI_JogDirection,
        [Type(TypeAttribute.Types.UDInt, TypeAttribute.RW.FullControll, length: SixRConstants.NumberOfAxis)]
        GUI_JogMaxSpeed,
        [Type(TypeAttribute.Types.DInt, TypeAttribute.RW.FullControll)]
        GUI_JogAcceleration,
        [Type(TypeAttribute.Types.DInt, TypeAttribute.RW.WriteOnly, length: 10000)]
        ExecuteLineNumber,
        [Type(TypeAttribute.Types.DInt, TypeAttribute.RW.FullControll, notify: true)]
        LineNumberIndexer,
        [Type(TypeAttribute.Types.Bool, TypeAttribute.RW.ReadOnly, length: SixRConstants.NumberOfInputs, notify: true)]
        GUI_DigitalInput,
        [Type(TypeAttribute.Types.Bool, TypeAttribute.RW.FullControll, length: SixRConstants.NumberOfOutputs)]
        GUI_DigitalOutput,
        [Type(TypeAttribute.Types.Bool, TypeAttribute.RW.FullControll, notify: true)]
        GUI_TrajectoryPuls,
        [Type(TypeAttribute.Types.DInt, TypeAttribute.RW.FullControll, length: SixRConstants.NumberOfAxis * SixRConstants.BufferLen)]
        GUI_ErrorPosition1,
        [Type(TypeAttribute.Types.DInt, TypeAttribute.RW.FullControll, length: SixRConstants.NumberOfAxis * SixRConstants.BufferLen)]
        GUI_ErrorPosition2

    }
}
