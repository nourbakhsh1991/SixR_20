using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Antlr4.Runtime.Tree;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra.Double;
using SixR_20.Models;
using SixR_20.ViewModels;
using TwinCAT.Ads.TypeSystem;

namespace SixR_20.Interpreter
{
    class SixRGrammerVisitor : SixRGrammerBaseVisitor<object>
    {
        #region ' Private Variables '
        private List<Function> _func = new List<Function>();
        private List<Variable> _globalVariables = new List<Variable>();
        private Stack<Function> _callStack = new Stack<Function>();
        private bool shallIBreak = false;
        private IParseTree _whomToBreak = null;
        private bool shallIContinue = false;
        private IParseTree _whomToContinue = null;
        private bool shallIReturn = false;
        private IParseTree _whomToReturn = null;
        public bool isPuased = false;
        public bool isStoped = false;

        // Gcode Generator
        private bool _runCode = false;
        Traj7Seg _trj7;
        private MyController _ctrlr;
        private int pastingPointerOffset;
        private bool currectCode = true;
        TrajectoryPointList<int>[] points = new TrajectoryPointList<int>[SixRConstants.NumberOfAxis];
        private List<int> LineIndexer = new List<int>();
        private int NumberOfMs = 0;
        private List<int> ReadDigitalIndicator = new List<int>();
        private int GcodeLength = 0;
        public static decimal[] GcodeActualPosition = new decimal[SixRConstants.NumberOfAxis];
        public static int[][] ActualPosVal = new int[SixRConstants.NumberOfAxis][];
        public static int ActualPosValLength;
        public int Cnt;
        private decimal lastF = 100;
        private int LastIkSolutionBranchNumber = 0;
        private bool IsVailMovement;
        private decimal[] ValidJointSpace = { 165, 100, 95, 180, 100, 180 };
        private decimal[] toolParam = new decimal[8];
        private List<TrajectoryPointList<decimal>[]> Traj = new List<TrajectoryPointList<decimal>[]>();
        #endregion

        #region ' Public Variables '

        public static bool Finished = false;

        // Gcode Generator

        public List<Function> Functions
        {
            get { return _func; }

        }

        public List<Variable> GlobalVariables
        {
            get { return _globalVariables; }
        }

        public Stack<Function> CallStack
        {
            get { return _callStack; }
        }

        public bool RunCode
        {
            get { return _runCode; }
            set { _runCode = value; }
        }

        #endregion

        // implemented: Done
        // checked: Done
        // exceptions:
        // final:
        // bugs:
        private bool isLoopOperator(IParseTree node)
        {
            return (node is SixRGrammerParser.STATFORContext || node is SixRGrammerParser.STATWHILEContext);
        }

        // implemented:
        // checked:
        // exceptions:
        // final:
        // bugs:
        public SixRGrammerVisitor(MyController ctr, Traj7Seg trj)
        {
            RunCode = false;
            _ctrlr = ctr;
            _trj7 = trj;
            // AddDigitalInputs();
            // AddDigitalOutputs();
            AddSixRVariables();
        }

        // implemented: Done
        // checked: Done
        // exceptions:
        // final:
        // bugs:
        private void AddSixRVariables()
        {
            GlobalVariables.Add(new Variable()
            {
                Type = PrimitiveType.POINTJ,
                IsArray = false,
                ArrayLength = 0,
                Name = "_sixR_Home",
                IsReadOnly = true,
                Value = new Point6R(new double[] { 0, 0, 0, 0, 0, 0 }, new bool[] { true, true, true, true, true, true }, false, 100)
            });
        }

        // implemented:
        // checked:
        // exceptions:
        // final:
        // bugs:
        public override object VisitModuleRoutines(SixRGrammerParser.ModuleRoutinesContext context)
        {
            // Define Global Variables
            foreach (var child in context.variableDeclaration())
            {
                 VisitVariableDeclaration(child);
            }
            // Define Sub Routines
            foreach (var child in context.subRoutine())
            {
                var sub = VisitSubRoutine(child) as Function;
                Functions.Add(sub);
            }


            // Call Main Routine
            if (context.mainRoutine().Length > 1) throw new Exception("");
            var mainRoutine = context.mainRoutine(0);
            var main = new Function
            {
                Name = "Main",
                Output = new Variable()
                {
                    HasValue = false,
                    IsArray = false,
                    IsLeft = true,
                    Type = PrimitiveType.INT,
                    Value = null
                },
                Params = new Dictionary<string, Variable>(),
                BodyContext = mainRoutine.routineBody()
            };
            CallStack.Push(main);
            Functions.Add(main);
            GcodeStart();
            VisitRoutineBody(main.BodyContext);
            
            return null;
        }

        // implemented: Done
        // checked:
        // exceptions:
        // final:
        // bugs:
        public override object VisitSubRoutine(SixRGrammerParser.SubRoutineContext context)
        {
            var sub = new Function
            {
                Name = context.procedureName().GetText(),
                Output = new Variable()
                {
                    HasValue = false,
                    IsArray = false,
                    IsLeft = true,
                    Type = PrimitiveType.INT,
                    Value = null
                },
                Params = new Dictionary<string, Variable>()
            };
            var formals = context.formalParameters();
            if (formals != null)
            {
                foreach (var itm in formals.parameter())
                {
                    var param = VisitParameter(itm) as Variable;
                    if (param == null) throw new Exception("");
                    sub.Params.Add(param.Name, param);
                }
            }
            sub.BodyContext = context.routineBody();
            return sub;
        }

        // implemented: Done
        // checked: Done
        // exceptions:
        // final:
        // bugs:
        public override object VisitParameter(SixRGrammerParser.ParameterContext context)
        {
            var variable = new Variable();

            // GET TYPE
            var type = VisitType(context.type());
            if (type == null) throw new Exception("");
            variable.Type = (PrimitiveType)type;

            // GET NAME
            var name = context.variableName();
            if (name.ChildCount == 1)
            {
                // Single value
                variable.Name = name.GetText();
                variable.IsArray = false;
                variable.ArrayLength = 1;
            }
            else
            {
                // Array
                variable.Name = name.GetChild(0).GetText();
                variable.ArrayDim = VisitArrayVariableSuffix(name.arrayVariableSuffix()) as List<int>;
                if (variable.ArrayDim == null)
                    throw new Exception("");
                variable.ArrayLength = variable.ArrayDim.Last();
                variable.ArrayDim.RemoveAt(variable.ArrayDim.Count - 1);
                variable.IsArray = true;
            }

            return variable;
        }

        // implemented: Done
        // checked: Done
        // exceptions:
        // final:
        // bugs:
        public override object VisitRoutineBody(SixRGrammerParser.RoutineBodyContext context)
        {
            if (context == null)
                return null;
            VisitStatementList(context.statementList());
            return null;
        }

        // implemented:
        // checked:
        // exceptions:
        // final:
        // bugs:
        public override object VisitStatementList(SixRGrammerParser.StatementListContext context)
        {
            for (var i = 0; i < context.ChildCount; i++)
            {
                if (shallIBreak)
                    break;
                if (shallIContinue)
                    break;
                if(shallIReturn)
                    break;
                if (isStoped)
                    return null;
                if (isPuased)
                    TrajectoryViewModel.SleepEvent.WaitOne();
                var stat = context.statement(i);
                Application.Current.Dispatcher.Invoke(new System.Action(() =>
                {
                    if (TrajectoryViewModel.ColoringLine != null && TrajectoryViewModel.TextEditor != null)
                    {
                        TrajectoryViewModel.ColoringLine.curtLine = stat.Start.Line;
                        TrajectoryViewModel.TextEditor.TextArea.TextView.Redraw();
                    }
                }));
                if (stat is SixRGrammerParser.STATIFContext)
                    VisitSTATIF(stat as SixRGrammerParser.STATIFContext);
                else if (stat is SixRGrammerParser.STATFORContext)
                    VisitSTATFOR(stat as SixRGrammerParser.STATFORContext);
                else if (stat is SixRGrammerParser.STATPTPContext)
                    VisitSTATPTP(stat as SixRGrammerParser.STATPTPContext);
                else if (stat is SixRGrammerParser.STATLINContext)
                    VisitSTATLIN(stat as SixRGrammerParser.STATLINContext);
                else if (stat is SixRGrammerParser.STATCIRContext)
                    VisitSTATCIR(stat as SixRGrammerParser.STATCIRContext);
                else if (stat is SixRGrammerParser.STATBRAKEContext)
                    VisitSTATBRAKE(stat as SixRGrammerParser.STATBRAKEContext);
                else if (stat is SixRGrammerParser.STATCONTINUEContext)
                    VisitSTATCONTINUE(stat as SixRGrammerParser.STATCONTINUEContext);
                else if (stat is SixRGrammerParser.STATRETURNContext)
                    VisitSTATRETURN(stat as SixRGrammerParser.STATRETURNContext);
                else if (stat is SixRGrammerParser.STATWHILEContext)
                    VisitSTATWHILE(stat as SixRGrammerParser.STATWHILEContext);
                else if (stat is SixRGrammerParser.STATWAITSECContext)
                    VisitSTATWAITSEC(stat as SixRGrammerParser.STATWAITSECContext);
                else if (stat is SixRGrammerParser.STATVARDECContext)
                    VisitSTATVARDEC(stat as SixRGrammerParser.STATVARDECContext);
                else if (stat is SixRGrammerParser.STATEXPContext)
                    VisitSTATEXP(stat as SixRGrammerParser.STATEXPContext);
                else
                    VisitChildren(stat);
            }
            return null;
        }

        // implemented: Done
        // checked: Done
        // exceptions:
        // final:
        // bugs:
        public override object VisitSTATEXP(SixRGrammerParser.STATEXPContext context)
        {
            if (context == null)
                return null;
            return VisitExpression(context.expression());
        }

        // implemented: Done
        // checked: Done
        // exceptions:
        // final:
        // bugs:
        public override object VisitSTATIF(SixRGrammerParser.STATIFContext context)
        {
            var condition = VisitExpression(context.expression());
            if (!(condition is bool))
                throw new Exception("");
            if ((bool)condition)
                VisitStatementList(context.statementList(0));
            if (context.ELSE() != null && !((bool)condition))
                VisitStatementList(context.statementList(1));
            return null;
        }

        // implemented: Done
        // checked: Done
        // exceptions:
        // final:
        // bugs:
        public override object VisitSTATFOR(SixRGrammerParser.STATFORContext context)
        {
            var _var = new Variable() { IsArray = false, Type = PrimitiveType.INT, Name = context.IDENTIFIER().ToString() };
            var from = VisitExpression(context.expression(0));
            if (!(from is int))
                throw new Exception("");
            var to = VisitExpression(context.expression(1));
            if (!(to is int))
                throw new Exception("");
            var varlist = CallStack.Peek();
            if (varlist.Variables.Any(a => a.Name == _var.Name))
                throw new Exception("");
            varlist.Variables.Add(_var);
            for (var i = (int)from; i <= (int)to; i++)
            {
                _var.Value = i;
                if (shallIBreak && _whomToBreak == context)
                {
                    shallIBreak = false;
                    _whomToBreak = null;
                    break;
                }
                if (isStoped)
                    return null;
                //if (shallIContinue && howToContinue == context)
                //{
                //    shallIContinue = false;
                //    howToContinue = null;
                //}
                VisitStatementList(context.statementList());

            }
            varlist.Variables.Remove(_var);
            return null;
        }

        // implemented: Done
        // checked: Done
        // exceptions:
        // final:
        // bugs:
        public override object VisitSTATPTP(SixRGrammerParser.STATPTPContext context)
        {
            Finished = false;
            Variable pnt = null;
            var experssionCounter = 0;
            float F = -1;
            int Con = 1;
            var walker = context.GetChild(1);
            if (walker is SixRGrammerParser.VariableNameContext)
            {
                var point = CallStack.Peek().Variables.FirstOrDefault(a => (a.Type == PrimitiveType.POINTJ || a.Type == PrimitiveType.POINTP) && a.Name == walker.GetText());
                if (point == null)
                {
                    point = GlobalVariables.FirstOrDefault(a => (a.Type == PrimitiveType.POINTJ || a.Type == PrimitiveType.POINTP) && a.Name == walker.GetText());
                    if (point == null)
                        throw new Exception("");
                    
                }
                pnt = point;
            }
            else if (walker is SixRGrammerParser.SixRJXPointContext)
            {
                var point = new Variable
                {
                    Name = null,
                    IsArray = false,
                    Type = PrimitiveType.POINTP
                };
                if ((walker as SixRGrammerParser.SixRJXPointContext).GetChild(0) is SixRGrammerParser.SixRJPointContext)
                    point.Type = PrimitiveType.POINTJ;
                point.Value = VisitSixRJXPoint(walker as SixRGrammerParser.SixRJXPointContext);
                pnt = point;
            }

            if (context.children.Any(a => a.GetText().ToLower() == "f"))
            {
                var f = VisitExpression(context.expression(experssionCounter++));
                if (f is float)
                    F = (float)f;
                else if (f is int)
                    F = (int)f;
                else
                    throw new Exception("");
            }
            if (context.children.Any(a => a.GetText().ToLower() == "con"))
            {
                var con = VisitExpression(context.expression(experssionCounter));
                if (con is int)
                {
                    switch ((int)con)
                    {
                        case 1:
                            con = 1;
                            break;
                        case 0:
                            con = 0;
                            break;
                        default:
                            throw new Exception("");
                    }

                }
                else if (con is bool)
                {
                    switch ((bool)con)
                    {
                        case true:
                            con = 1;
                            break;
                        case false:
                            con = 0;
                            break;
                        default:
                            throw new Exception("");
                    }
                }
                else
                {
                    throw new Exception("");
                }
            }
            var p = pnt.Value as Point6R;
            if (p.F < .00000001)
                p.F = 100;
            p.F = F;
            p.Con = Con;
            Traj = new List<TrajectoryPointList<decimal>[]>();
            var result = GeneratePTP(p, context.start.Line.ToString());
            if (!RunCode) return null;
            if (result)
            {
                GcodeExecute();
                TrajectoryViewModel.SleepEvent.WaitOne();
            }
            return null;
        }

        // implemented: Done
        // checked: Done
        // exceptions:
        // final:
        // bugs:
        public override object VisitSTATLIN(SixRGrammerParser.STATLINContext context)
        {
            Finished = false;
            var experssionCounter = 0;
            Variable pnt = null;
            float F = -1;
            int Con = 1;
            var walker = context.GetChild(1);
            if (walker is SixRGrammerParser.VariableNameContext)
            {
                var point = CallStack.Peek().Variables.FirstOrDefault(a => (a.Type == PrimitiveType.POINTJ || a.Type == PrimitiveType.POINTP) && a.Name == walker.GetText());
                if (point == null)
                {
                    point = GlobalVariables.FirstOrDefault(a => (a.Type == PrimitiveType.POINTJ || a.Type == PrimitiveType.POINTP) && a.Name == walker.GetText());
                    if (point == null)
                        throw new Exception(SixRConstants.ResourceManager.GetString("AA0010002") + "\r\n in Line: " + context.Start.Line);
                    
                }
                if (point.Type == PrimitiveType.POINTJ)
                    throw new Exception(SixRConstants.ResourceManager.GetString("AA0010020") + "\r\n in Line: " + context.Start.Line);
                pnt = point;

            }
            else if (walker is SixRGrammerParser.SixRJXPointContext)
            {
                var point = new Variable
                {
                    Name = null,
                    IsArray = false,
                    Type = PrimitiveType.POINTP
                };
                if ((walker as SixRGrammerParser.SixRJXPointContext).GetChild(0) is SixRGrammerParser.SixRJPointContext)
                    throw new Exception(SixRConstants.ResourceManager.GetString("AA0010020") + "\r\n in Line: " + context.Start.Line);
                point.Value = VisitSixRJXPoint(walker as SixRGrammerParser.SixRJXPointContext);
                pnt = point;
            }
            if (context.children.Any(a => a.GetText().ToLower() == "f"))
            {
                var f = VisitExpression(context.expression(experssionCounter++));
                if (f is float)
                    F = (float)f;
                else if (f is int)
                    F = (int)f;
                else
                    throw new Exception(SixRConstants.ResourceManager.GetString("AA0010018") + "\r\n in Line: " + context.Start.Line);
            }
            if (context.children.Any(a => a.GetText().ToLower() == "con"))
            {
                var con = VisitExpression(context.expression(experssionCounter));
                if (con is int)
                {
                    switch ((int)con)
                    {
                        case 1:
                            con = 1;
                            break;
                        case 0:
                            con = 0;
                            break;
                        default:
                            throw new Exception(SixRConstants.ResourceManager.GetString("AA0010019") + "\r\n in Line: " + context.Start.Line);
                    }

                }
                else if (con is bool)
                {
                    switch ((bool)con)
                    {
                        case true:
                            con = 1;
                            break;
                        case false:
                            con = 0;
                            break;
                        default:
                            throw new Exception(SixRConstants.ResourceManager.GetString("AA0010019") + "\r\n in Line: " + context.Start.Line);
                    }
                }
                else
                {
                    throw new Exception(SixRConstants.ResourceManager.GetString("AA0010019") + "\r\n in Line: " + context.Start.Line);
                }
            }

            var p = pnt.Value as Point6R;
            if (p.F < 000001)
                p.F = 100;
            p.F = F;
            p.Con = Con;
            Traj = new List<TrajectoryPointList<decimal>[]>();
            var result = GenerateLIN(p);
            if (!RunCode) return null;
            if (result)
            {
                GcodeExecute();
                TrajectoryViewModel.SleepEvent.WaitOne();
            }
            return null;

        }

        // implemented: Done
        // checked: Done
        // exceptions:
        // final:
        // bugs:
        public override object VisitSixRJXPoint(SixRGrammerParser.SixRJXPointContext context)
        {
            if (context.GetChild(0) is SixRGrammerParser.SixRPPointContext)
                return VisitSixRPPoint(context.sixRPPoint());
            if (context.GetChild(0) is SixRGrammerParser.SixRJPointContext)
                return VisitSixRJPoint(context.sixRJPoint());
            throw new Exception(SixRConstants.ResourceManager.GetString("AA0010016") + "\r\n in Line: " + context.Start.Line);
        }

        // implemented: Done
        // checked: Done
        // exceptions:
        // final:
        // bugs:
        public override object VisitSixRJPoint(SixRGrammerParser.SixRJPointContext context)
        {
            var point = new Point6R(new double[6], new bool[6], false);
            for (var i = 1; i < context.ChildCount - 1; i += 2)
            {
                float tmp;
                var part = context.sixRJPart(i/2);
                if (!float.TryParse(VisitExpression(part.expression()).ToString(), out tmp))
                    throw new Exception(SixRConstants.ResourceManager.GetString("AA0010015") + "\r\n in Line: " + context.Start.Line);
                if (part.J1() != null)
                {
                    point.Js[0] = (float)tmp;
                    point.ValidVals[0] = true;
                }
                else if (part.J2() != null)
                {
                    point.Js[1] = (float)tmp;
                    point.ValidVals[1] = true;
                }
                else if (part.J3() != null)
                {
                    point.Js[2] = (float)tmp;
                    point.ValidVals[2] = true;
                }
                else if (part.J4() != null)
                {
                    point.Js[3] = (float)tmp;
                    point.ValidVals[3] = true;
                }
                else if (part.J5() != null)
                {
                    point.Js[4] = (float)tmp;
                    point.ValidVals[4] = true;
                }
                else if (part.J6() != null)
                {
                    point.Js[5] = (float)tmp;
                    point.ValidVals[5] = true;
                }
                else
                {
                    throw new Exception(SixRConstants.ResourceManager.GetString("AA0010014") + "\r\n in Line: " + context.Start.Line);
                }
            }
            return point;
        }

        // implemented: Done
        // checked: Done
        // exceptions:
        // final:
        // bugs:
        public override object VisitSixRPPoint(SixRGrammerParser.SixRPPointContext context)
        {
            var point = new Point6R(new double[6], new bool[6], true);
            if (context.variableName().Length>0)
            {
                var posName = context.variableName(0).GetText();
                var pos = CallStack.Peek().Variables.FirstOrDefault(a => (a.Type == PrimitiveType.POS) && a.Name == posName);
                if (pos == null)
                {
                    pos = GlobalVariables.FirstOrDefault(a => (a.Type == PrimitiveType.POS) && a.Name == posName);
                    if (pos == null)
                        throw new Exception("");
                }
                var oriName = context.variableName(1).GetText();
                var ori = CallStack.Peek().Variables.FirstOrDefault(a => (a.Type == PrimitiveType.ORIENT) && a.Name == oriName);
                if (ori == null)
                {
                    ori = GlobalVariables.FirstOrDefault(a => (a.Type == PrimitiveType.ORIENT) && a.Name == oriName);
                    if (ori == null)
                        throw new Exception("");
                }
            }

            return point;
        }

        // implemented: Done
        // checked: Done
        // exceptions:
        // final:
        // bugs:
        private void GcodeStart()
        {
            IsVailMovement = true;
            LineIndexer = new List<int>();
            Traj = new List<TrajectoryPointList<decimal>[]>();
            toolParam = SixRConstants.toolParam;
            ReadDigitalIndicator = new List<int>();
            decimal[] theta =
            {
                _ctrlr.MotorsEncoder[0]*UnitConverter.PulsToDegFactor[0],
                _ctrlr.MotorsEncoder[1]*UnitConverter.PulsToDegFactor[1],
                _ctrlr.MotorsEncoder[2]*UnitConverter.PulsToDegFactor[2],
                _ctrlr.MotorsEncoder[3]*UnitConverter.PulsToDegFactor[3],
                _ctrlr.MotorsEncoder[4]*UnitConverter.PulsToDegFactor[4],
                _ctrlr.MotorsEncoder[5]*UnitConverter.PulsToDegFactor[5],
            };
            for (int i = 0; i < 6; i++)
            {
                GcodeActualPosition[i] = theta[i];
            }
        }

        // implemented: Done
        // checked: Done
        // exceptions:
        // final:
        // bugs:
        private bool GeneratePTP(Point6R point, string Line)
        {
            if (!point.IsEular)
            {
                var tmppoints = new TrajectoryPointList<decimal>[6];
                var tmpKeys = new List<string>();
                tmpKeys.AddRange(new string[] { "J1", "J2", "J3", "J4", "J5", "J6", "F", "CON" });
                var tmpVals = new List<decimal>();
                tmpVals.AddRange(new decimal[]
                {
                    GcodeActualPosition[0], GcodeActualPosition[1], GcodeActualPosition[2], GcodeActualPosition[3],
                    GcodeActualPosition[4], GcodeActualPosition[5], lastF, 1
                });
                for (var i = 0; i < 6; i++)
                {
                    if (point.ValidVals[i])
                        tmpVals[i] =Convert.ToDecimal( point.Js[i]);
                }
                var tolerance = .00001;
                if (Math.Abs(point.F - (-1)) > tolerance)
                    tmpVals[6] = Convert.ToDecimal(point.F);
                if (Math.Abs(point.Con - (-1)) > tolerance)
                    tmpVals[7] = Convert.ToDecimal(point.Con);
                var tarjectory = _trj7.PTPList(GcodeActualPosition, tmpKeys, tmpVals);
                if (tarjectory[0].TrajLength > 0)
                {
                    Traj.Add(tarjectory);
                    for (int j = 0; j < 6; j++)
                    {
                        GcodeActualPosition[j] = Traj[Traj.Count - 1][j].q[Traj[Traj.Count - 1][j].TrajLength - 1];
                    }
                    return true;
                }
                return false;
            }
            else
            {
                var cartesianPosition = _trj7.GetCartPos(GcodeActualPosition.Select(a => a * Convert.ToDecimal(Math.PI / 180)).ToArray(), toolParam);
                var rpy = _trj7.toEulerianAngle(cartesianPosition);
                var tmpKeys = new List<string>();
                tmpKeys.AddRange(new string[] { "X", "Y", "Z", "A", "B", "C", "F", "CON" });
                var tmpVals = new List<decimal>();
                tmpVals.AddRange(new [] { 0, 0, 0, 0, 0, 0, lastF, 1 });
                // for X,Y,Z
                for (var i = 0; i < 3; i++)
                {
                    if (point.ValidVals[i])
                        cartesianPosition[i + 5] =Convert.ToDecimal( point.Eulars[i]);
                }
                //for A,B,C
                for (var i = 3; i < 6; i++)
                {
                    if (point.ValidVals[i])
                        rpy[i - 3] = Convert.ToDecimal( point.Eulars[i] * (Math.PI / 180.0));
                }
                var tolerance = .00001;
                if (Math.Abs(point.F - (-1)) > tolerance)
                    tmpVals[6] = Convert.ToDecimal( point.F);
                if (Math.Abs(point.Con - (-1)) > tolerance)
                    tmpVals[7] = Convert.ToDecimal(point.Con);

                var quaternionOfRpy = _trj7.toQuaternion(rpy[0], rpy[1], rpy[2]);
                for (int i = 0; i < 4; i++)
                    cartesianPosition[i] = quaternionOfRpy[i];
                var Ans = _trj7.Inversekinematic(cartesianPosition, toolParam);
                bool[] ValidIkSolutionBranchNumber = { true, true, true, true, true, true, true, true };

                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 6; j++)
                    {
                        if (Math.Abs((double)Ans[i, j] * (180 / Math.PI)) > (double)ValidJointSpace[j])
                            ValidIkSolutionBranchNumber[i] = false;
                    }
                }
                var jointDisplacement = new double[8];
                var min = 0;
                for (int i = 0; i < 8; i++)
                {
                    if (ValidIkSolutionBranchNumber[i])
                    {
                        for (int j = 0; j < 6; j++)
                            jointDisplacement[i] += Math.Abs((double)Ans[i, j] - (double)GcodeActualPosition[j] * Math.PI / 180.0);
                    }
                    else
                    {
                        jointDisplacement[i] = double.MaxValue;
                    }
                    if (jointDisplacement[i] < jointDisplacement[min])
                        min = i;
                }
                LastIkSolutionBranchNumber = point.IkSolutionBranchNumber = min;
                for (int i = 0; i < SixRConstants.NumberOfAxis; i++)
                {
                    tmpVals[i] = Ans[LastIkSolutionBranchNumber, i] *Convert.ToDecimal (180 / Math.PI);
                }

                var tarjectory = _trj7.PTPList(GcodeActualPosition, tmpKeys, tmpVals);
                if (tarjectory[0].TrajLength > 0)
                {
                    Traj.Add(tarjectory);
                    for (int j = 0; j < 6; j++)
                    {
                        GcodeActualPosition[j] = Traj[Traj.Count - 1][j].q[Traj[Traj.Count - 1][j].TrajLength - 1];
                    }
                    return true;

                }
                return false;
            }
        }

        // implemented: Done
        // checked: Done
        // exceptions:
        // final:
        // bugs:
        private bool GenerateLIN(Point6R point)
        {
            if (!point.IsEular)
            {
                throw new Exception("Point Must be in Eular");
            }
            var tmpPos = _trj7.GetCartPos(GcodeActualPosition.Select(a => a * Convert.ToDecimal(Math.PI / 180)).ToArray(), toolParam);
            var currentPos = tmpPos.Select(a => a).ToArray();
            var rpy = _trj7.toEulerianAngle(tmpPos);
            
            var tmpKeys = new List<string>();
            tmpKeys.AddRange(new string[] { "X", "Y", "Z", "A", "B", "C", "F", "CON" });
            var tmpVals = new List<decimal>();
            tmpVals.AddRange(new [] { 0, 0, 0, 0, 0, 0, lastF, 1 });
            // for X,Y,Z
            for (var i = 0; i < 3; i++)
            {
                if (point.ValidVals[i])
                    tmpPos[i + 5] =Convert.ToDecimal( point.Eulars[i]);
            }
            //for A,B,C
            for (var i = 3; i < 6; i++)
            {
                if (point.ValidVals[i])
                    rpy[i - 3] = Convert.ToDecimal(point.Eulars[i] * (Math.PI / 180.0));
            }
            var tolerance = .00001;
            if (Math.Abs(point.F - (-1)) > tolerance)
                tmpVals[6] = Convert.ToDecimal(point.F);
            if (Math.Abs(point.Con - (-1)) > tolerance)
                tmpVals[7] = Convert.ToDecimal(point.Con);

            var tmpRpy = _trj7.toQuaternion(rpy[0], rpy[1], rpy[2]);
            for (int i = 0; i < 4; i++)
                tmpPos[i] = tmpRpy[i];

            var Ans = _trj7.Inversekinematic(currentPos, toolParam);
            var Error = new double[8];
            var min = 0;
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 6; j++)
                    Error[i] += Math.Abs((double)Ans[i, j] - (double)GcodeActualPosition[j] * Math.PI / 180.0);
                if (Error[i] < Error[min])
                    min = i;
            }
            LastIkSolutionBranchNumber = point.IkSolutionBranchNumber = min;

            var distance =
                  Convert.ToDecimal(Math.Sqrt(Math.Pow((double)(tmpPos[5] - currentPos[5]), 2) + Math.Pow((double)(tmpPos[6] - currentPos[6]), 2) +
                              Math.Pow((double)(tmpPos[7] - currentPos[7]), 2)));
            var pointList = _trj7.SingleAxisTraj(new TrajectoryPoint(0, 0), new TrajectoryPoint((double)distance, 0),
                (double)tmpVals[6], 5000, 10000, .001, .999);
            var resultList = new TrajectoryPointList<decimal>[SixRConstants.NumberOfAxis];
            for (int i = 0; i < SixRConstants.NumberOfAxis; i++)
                resultList[i] = new TrajectoryPointList<decimal>();
            var tmpTeta = new double[6];
            var tmpCart = currentPos.Select(a => a).ToArray();
            var Qend = tmpRpy;
            for (int i = 0; i < pointList.TrajLength; i++)
            {
                var x = tmpCart[5] + (pointList.q[i] / distance) * (tmpPos[5] - tmpCart[5]);
                var y = tmpCart[6] + (pointList.q[i] / distance) * (tmpPos[6] - tmpCart[6]);
                var z = tmpCart[7] + (pointList.q[i] / distance) * (tmpPos[7] - tmpCart[7]);
                var QCurrent =new [] { currentPos[0], currentPos[1], currentPos[2], currentPos[3] };
                var Qnext = _trj7.Slerp(QCurrent.Select(a=>(double)a).ToArray(), Qend.Select(a => (double)a).ToArray(), (double)(pointList.q[i] / distance)).Select(a=>Convert.ToDecimal(a)).ToArray();
                var res = _trj7.Inversekinematic(
                    new[] { Qnext[0], Qnext[1], Qnext[2], Qnext[3], 0, x, y, z }, toolParam);
                for (int j = 0; j < SixRConstants.NumberOfAxis; j++)
                {
                    if (Math.Abs((double)res[LastIkSolutionBranchNumber, j] * (180 / Math.PI)) >= (double)ValidJointSpace[j])
                        IsVailMovement = false;
                    resultList[j].AddPoint(res[LastIkSolutionBranchNumber, j] * Convert.ToDecimal(180 / Math.PI), 0, 0);
                }
            }
            Traj.Add(resultList);
            for (int j = 0; j < 6; j++)
            {
                GcodeActualPosition[j] = Traj[Traj.Count - 1][j].q[Traj[Traj.Count - 1][j].TrajLength - 1];
            }
            return true;
        }

        // implemented: Done
        // checked: Done
        // exceptions:
        // final:
        // bugs:
        private void GcodeExecute()
        {
            points = new TrajectoryPointList<int>[6];
            if (Traj.Count == 0)
                return;
            foreach (var val in Traj)
            {
                for (var j = 0; j < val[0].TrajLength; j++)
                {
                    for (var i = 0; i < SixRConstants.NumberOfAxis; i++)
                    {
                        if (points[i] == null)
                            points[i] = new TrajectoryPointList<int>();
                        points[i].AddPoint((int)(val[i].q[j] / UnitConverter.PulsToDegFactor[i]), (int)val[i].v[j], (int)val[i].a[j]);
                    }
                }
            }
            _ctrlr.SetRegisterCtrWord(new ushort[] { 15, 15, 15, 15, 15, 15 });
            _ctrlr.SetCommand((int)CommandsEnum.SetControlWord);
            _ctrlr.SetSelectedMotors(new bool[] { true, true, true, true, true, true });
            Cnt = 0;
            _ctrlr.InitilizeTrajectory(points);
            Finished = true;
            _ctrlr.SetCommand((int)CommandsEnum.StartTrajectory);
        }

        // implemented: Done
        // checked: Done
        // exceptions:
        // final:
        // bugs:
        public override object VisitSTATBRAKE(SixRGrammerParser.STATBRAKEContext context)
        {
            var walker = context.Parent;
            while (!(walker is SixRGrammerParser.RoutineBodyContext || isLoopOperator(walker)))
                walker = walker.Parent;
            if (walker is SixRGrammerParser.RoutineBodyContext)
                throw new Exception("");
            shallIBreak = true;
            _whomToBreak = walker;
            return null;
        }

        // implemented: Done
        // checked: Done
        // exceptions:
        // final:
        // bugs:
        public override object VisitSTATCONTINUE(SixRGrammerParser.STATCONTINUEContext context)
        {
            var walker = context.Parent;
            while (!(walker is SixRGrammerParser.RoutineBodyContext || isLoopOperator(walker)))
                walker = walker.Parent;
            if (walker is SixRGrammerParser.RoutineBodyContext)
                throw new Exception("");
            shallIContinue = true;
            _whomToContinue = walker;
            return null;
        }

        // implemented: Done
        // checked: Done
        // exceptions:
        // final:
        // bugs:
        public override object VisitSTATRETURN(SixRGrammerParser.STATRETURNContext context)
        {
            var walker = context.Parent;
            while (!(walker is SixRGrammerParser.RoutineBodyContext))
                walker = walker.Parent;
            if (!(walker is SixRGrammerParser.RoutineBodyContext))
                throw new Exception("");
            shallIReturn = true;
            _whomToReturn = walker;
            var res = VisitExpression(context.expression());
            CallStack.Peek().Output = res;
            return null;
        }

        // implemented: Done
        // checked: Done
        // exceptions:
        // final:
        // bugs:
        public override object VisitSTATWHILE(SixRGrammerParser.STATWHILEContext context)
        {
            var cond = VisitExpression(context.expression());
            if (!(cond is bool))
                throw new Exception("");
            while ((bool)cond)
            {
                if (shallIBreak && _whomToBreak == context)
                {
                    shallIBreak = false;
                    _whomToBreak = null;
                    break;
                }
                if (shallIContinue && _whomToContinue == context)
                {
                    shallIContinue = false;
                    _whomToContinue = null;
                }
                if (isStoped)
                    return null;
                VisitStatementList(context.statementList());
                cond = VisitExpression(context.expression());
            }
            return null;
        }

        // implemented: Done
        // checked: Done
        // exceptions:
        // final:
        // bugs:
        public override object VisitSTATWAITSEC(SixRGrammerParser.STATWAITSECContext context)
        {

            var sec = VisitExpression(context.expression());
            if (!(sec is int))
                throw new Exception("");
            Thread.Sleep((int)sec);
            return null;
        }

        // implemented: Done
        // checked: Done
        // exceptions:
        // final:
        // bugs:
        public override object VisitConditionalOrExpression(SixRGrammerParser.ConditionalOrExpressionContext context)
        {
            if (context.ChildCount == 1) return VisitExclusiveOrExpression(context.exclusiveOrExpression(0));
            for (var i = 0; i < (context.ChildCount + 1) / 2; i++)
            {
                var outout = VisitExclusiveOrExpression(context.exclusiveOrExpression(i));
                if (!(outout is bool))
                    throw new Exception("");
                if ((bool)outout)
                    return true;
            }
            return false;
        }

        // implemented: Done
        // checked: Done
        // exceptions:
        // final:
        // bugs:
        public override object VisitExclusiveOrExpression(SixRGrammerParser.ExclusiveOrExpressionContext context)
        {
            if (context.ChildCount == 1) return VisitConditionalAndExpression(context.conditionalAndExpression(0));
            var output = VisitConditionalAndExpression(context.conditionalAndExpression(0));
            if (!(output is bool))
                throw new Exception("");
            var xor = ((bool)output);
            for (var i = 1; i < (context.ChildCount + 1) / 2; i++)
            {
                output = VisitConditionalAndExpression(context.conditionalAndExpression(i));
                if (!(output is bool))
                    throw new Exception("");
                xor = (bool)output != xor;
            }
            return xor;
        }

        // implemented: Done
        // checked: Done
        // exceptions:
        // final:
        // bugs:
        public override object VisitConditionalAndExpression(SixRGrammerParser.ConditionalAndExpressionContext context)
        {
            if (context.ChildCount == 1) return VisitAdditiveExpression(context.additiveExpression(0));
            for (var i = 0; i < (context.ChildCount + 1) / 2; i++)
            {
                var outout = VisitAdditiveExpression(context.additiveExpression(i));
                if (!(outout is bool))
                    throw new Exception("");
                if (!(bool)outout)
                    return false;
            }
            return true;
        }

        // implemented: Done
        // checked: Done
        // exceptions:
        // final:
        // bugs:
        public override object VisitAdditiveExpression(SixRGrammerParser.AdditiveExpressionContext context)
        {
            object sum = 0;
            var isFloat = false;
            if (context.ChildCount == 1) return VisitMultiplicativeExpression(context.multiplicativeExpression(0));
            for (var i = 0; i < (context.ChildCount + 1) / 2; i++)
            {
                var outout = VisitMultiplicativeExpression(context.multiplicativeExpression(i));
                if (outout is int && !isFloat)
                {
                    if (i - 1 < 0 || context.GetChild(i * 2 - 1).GetText() == "+")
                    {
                        sum = (int)sum + (int)outout;

                    }
                    else
                    {
                        sum = (int)sum - (int)outout;
                    }

                }
                else if (outout is float)
                {
                    if (i - 1 < 0 || context.GetChild(i * 2 - 1).GetText() == "+")
                    {
                        if (sum is float)
                            sum = (float)sum + (float)outout;
                        else
                            sum = (int)sum + (float)outout;
                    }
                    else
                    {
                        if (sum is float)
                            sum = (float)sum - (float)outout;
                        else
                            sum = (int)sum - (float)outout;
                    }
                    isFloat = true;
                }
                else
                {
                    throw new Exception("");
                }
            }
            return sum;
        }

        // implemented: Done
        // checked: Done
        // exceptions:
        // final:
        // bugs:
        public override object VisitMultiplicativeExpression(SixRGrammerParser.MultiplicativeExpressionContext context)
        {
            object mul = 1;
            var isFloat = false;
            if (context.ChildCount == 1) return VisitUnaryNotExpression(context.unaryNotExpression(0));
            for (var i = 0; i < (context.ChildCount + 1) / 2; i++)
            {
                var outout = VisitUnaryNotExpression(context.unaryNotExpression(i));
                if (outout is int && !isFloat)
                {
                    if (i - 1 < 0 || context.GetChild(i * 2 - 1).GetText() == "*")
                    {
                        mul = (int)mul * (int)outout;

                    }
                    else
                    {
                        mul = (int)mul / (int)outout;
                    }

                }
                else if (outout is float)
                {
                    if (i - 1 < 0 || context.GetChild(i * 2 - 1).GetText() == "*")
                    {
                        if (mul is float)
                            mul = (float)mul * (float)outout;
                        else
                            mul = (int)mul * (float)outout;
                    }
                    else
                    {
                        if (mul is float)
                            mul = (float)mul / (float)outout;
                        else
                            mul = (int)mul / (float)outout;
                    }
                    isFloat = true;
                }
                else
                {
                    throw new Exception("");
                }
            }
            return mul;
        }

        // implemented: Done
        // checked: Done
        // exceptions:
        // final:
        // bugs:
        public override object VisitUnaryNotExpression(SixRGrammerParser.UnaryNotExpressionContext context)
        {
            if (context.ChildCount == 1) return VisitUnaryPlusMinuxExpression(context.unaryPlusMinuxExpression());
            var output = VisitUnaryNotExpression(context.unaryNotExpression());
            if (output is bool)
                return !((bool)output);
            throw new Exception("");
        }

        // implemented: Done
        // checked: Done
        // exceptions:
        // final:
        // bugs:
        public override object VisitUnaryPlusMinuxExpression(SixRGrammerParser.UnaryPlusMinuxExpressionContext context)
        {
            if (context.ChildCount == 1) return VisitPrimary(context.primary());
            var output = VisitUnaryPlusMinuxExpression(context.unaryPlusMinuxExpression());
            var sign = context.GetChild(0).GetText();
            if (sign != "-" && sign != "+")
                throw new Exception("");
            if (output is int)
                return -((int)output);
            if (output is float)
                return -((float)output);
            return output;
        }

        // implemented: Done
        // checked: 
        // exceptions:
        // final:
        // bugs:
        public override object VisitPrimary(SixRGrammerParser.PrimaryContext context)
        {
            IParseTree walker = context.GetChild(0);
            var _variables = CallStack.Peek().Variables;
            if (walker is SixRGrammerParser.LiteralContext)
            {
                return VisitLiteral(walker as SixRGrammerParser.LiteralContext);
            }
            if (walker is SixRGrammerParser.VariableNameContext)
            {
                var isArray = walker.ChildCount == 2;
                if (!isArray)
                {
                    var output = _variables.FirstOrDefault(a => a.Name == walker.GetChild(0).GetText());
                    if (output == null)
                    {
                        output = GlobalVariables.FirstOrDefault(a => a.Name == walker.GetChild(0).GetText());
                        if (output == null)
                            throw new Exception("");
                        
                    }
                    return output.Value;
                }
                var dim = (walker.GetChild(1).ChildCount - 1) / 2;
                var output1 = _variables.FirstOrDefault(a => a.Name == walker.GetChild(0).GetText());
                if (output1 == null)
                    throw new Exception("");
                if (output1.ArrayDim.Count != dim)
                    throw new Exception("");
                List<int> dims = new List<int>();
                for (int i = 1; i <= dim * 2; i += 2)
                {
                    dims.Add((int)VisitExpression(walker.GetChild(1).GetChild(i) as SixRGrammerParser.ExpressionContext));
                }
                for (int i = 0; i < dim; i++)
                {
                    if (dims[i] < 1 || dims[i] > output1.ArrayDim[i])
                        throw new Exception("");
                }
                var index = 0;
                for (int i = 0; i < dim; i++)
                {
                    var coeff = 1;
                    for (int j = i - 1; j >= 0; j--)
                        coeff *= output1.ArrayDim[j];
                    index += (dims[i] - 1) * coeff;
                }
                return (output1.Value as List<object>)[index];
            }
            if (walker is SixRGrammerParser.ParExpressionContext)
            {
                return
                    VisitExpression((walker as SixRGrammerParser.ParExpressionContext).expression());
            }
            return null;
        }

        // implemented:
        // checked:
        // exceptions:
        // final:
        // bugs:
        public override object VisitVariableDeclaration(SixRGrammerParser.VariableDeclarationContext context)
        {
            var variable = new Variable();

            // GET TYPE
            var type = VisitType(context.type());
            if (type == null) throw new Exception("");
            variable.Type = (PrimitiveType)type ;

            // GET NAME
            var name = context.variableName();
            if (name.ChildCount == 1)
            {
                // Single value
                variable.Name = name.GetText();
                variable.IsArray = false;
                variable.ArrayLength = 1;
            }
            else
            {
                // Array
                variable.Name = name.GetChild(0).GetText();
                variable.ArrayDim = VisitArrayVariableSuffix(name.arrayVariableSuffix()) as List<int>;
                if(variable.ArrayDim==null)
                    throw new Exception("");
                variable.ArrayLength = variable.ArrayDim.Last();
                variable.ArrayDim.RemoveAt(variable.ArrayDim.Count - 1);
                variable.IsArray = true;
            }
            // GET LAST PART
            var val = context.variableListRest();
            if (val != null)
            {
                variable.Value = null;
                variable.HasValue = false;
                if (CallStack.Count == 0)
                {
                    GlobalVariables.Add(variable);
                }
                else
                {
                    var func = CallStack.Peek();
                    func.Variables.Add(variable);
                }

                var restCount = val.ChildCount / 2;
                for (var j = 1; j < val.ChildCount; j += 2)
                {
                    variable = new Variable {Type = (PrimitiveType) type};

                    var subname = val.GetChild(j) as SixRGrammerParser.VariableNameContext;
                    if(subname == null) throw new Exception("");
                    if (subname.ChildCount == 1)
                    {
                        variable.Name = subname.GetText();
                        variable.IsArray = false;
                        variable.ArrayLength = 1;
                    }
                    else
                    {
                        // Array
                        variable.Name = subname.GetChild(0).GetText();
                        variable.ArrayDim = VisitArrayVariableSuffix((subname).arrayVariableSuffix()) as List<int>;
                        if (variable.ArrayDim == null)
                            throw new Exception("");
                        variable.ArrayLength = variable.ArrayDim.Last();
                        variable.ArrayDim.RemoveAt(variable.ArrayDim.Count - 1);
                        variable.IsArray = true;
                    }
                    variable.Value = null;
                    variable.HasValue = false;
                    if (CallStack.Count == 0)
                    {
                        GlobalVariables.Add(variable);
                    }
                    else
                    {
                        var func = CallStack.Peek();
                        func.Variables.Add(variable);
                    }
                }
            }
            var val2 = context.variableInitialisation();
            if (val2 != null)
            {
                var output = VisitVariableInitialisation(context.variableInitialisation());
                if (variable.IsArray)
                {
                    var objs = new List<object>();
                    for (var i = 0; i < variable.ArrayLength; i++)
                        objs.Add(output);
                    variable.Value = objs;
                }
                else
                {
                    variable.Value = output;
                }
                if (CallStack.Count == 0)
                {
                    GlobalVariables.Add(variable);
                }
                else
                {
                    var func = CallStack.Peek();
                    func.Variables.Add(variable);
                }
            }
            return null;
        }

        // implemented: Done
        // checked: Done
        // exceptions:
        // final:
        // bugs:
        public override object VisitVariableInitialisation(SixRGrammerParser.VariableInitialisationContext context)
        {
            return VisitExpression(context.expression());
        }

        // implemented:
        // checked:
        // exceptions:
        // final:
        // bugs:
        public override object VisitArrayVariableSuffix(SixRGrammerParser.ArrayVariableSuffixContext context)
        {
            var dim = new List<int>();
            var dimCount = (context.ChildCount - 1) / 2;
            if ((((context.ChildCount + 1) / 2.0) != ((context.ChildCount + 1) / 2)) || dimCount < 0)
                throw new Exception("");
            var ArrayLength = 1;
            for (var i = 1; i < context.ChildCount; i += 2)
            {
                var childWalker = context.GetChild(i);
                var tmp = VisitExpression(childWalker as SixRGrammerParser.ExpressionContext);
                ArrayLength = ArrayLength * (int)tmp;
                dim.Add(int.Parse(childWalker.GetText()));
            }
            dim.Add(ArrayLength);
            return dim;
        }

        // implemented: Done
        // checked: Done
        // exceptions:
        // final:
        // bugs:
        public override object VisitType(SixRGrammerParser.TypeContext context)
        {
            if (context == null)
                return null;
            var walker = context.children[0];
            while (walker.ChildCount != 0) walker = walker.GetChild(0);
            var terminalNodeImpl = walker as TerminalNodeImpl;
            if (terminalNodeImpl == null) return null;
            switch (SixRGrammerLexer.DefaultVocabulary.GetSymbolicName(terminalNodeImpl.Symbol.Type))
            {
                case "INT":
                    return PrimitiveType.INT;
                case "CHAR":
                    return PrimitiveType.CHAR;
                case "BOOL":
                    return PrimitiveType.BOOL;
                case "FLOAT":
                    return PrimitiveType.FLOAT;
                case "POS":
                    return PrimitiveType.POS;
                case "ORIENT":
                    return PrimitiveType.ORIENT;
                case "POINTJ":
                    return PrimitiveType.POINTJ;
                case "POINTP":
                    return PrimitiveType.POINTP;
                default:
                    throw new Exception("");
            }
        }

        // implemented:
        // checked:
        // exceptions:
        // final:
        // bugs:
        public override object VisitSTATVARDEC(SixRGrammerParser.STATVARDECContext context)
        {
            if (context == null)
                return null;
            return VisitVariableDeclaration(context.variableDeclaration());
        }

        // implemented: Done
        // checked: Done
        // exceptions:
        // final:
        // bugs:
        public override object VisitExpression(SixRGrammerParser.ExpressionContext context)
        {
            if (context.conditionalOrExpression().Length != 0)
            {
                var res = VisitConditionalOrExpression(context.conditionalOrExpression(0));
                for (var i = 1; i < (context.ChildCount + 1)/2; i++)
                {
                    var outout = VisitConditionalOrExpression(context.conditionalOrExpression(i));
                    if (!(((outout is bool) && (res is bool)) ||
                          ((outout is int) && (res is int)) ||
                          ((outout is int) && (res is float)) ||
                          ((outout is string) && (res is string)) ||
                          ((outout is float) && (res is float)) ||
                          ((outout is char) && (res is char)) ||
                          ((outout is float) && (res is int))))
                    {
                        throw new Exception("");
                    }
                    var op = context.GetChild(i*2 - 1).GetText();
                    switch (op)
                    {
                        case "==":
                            res = ((dynamic) res == (dynamic) outout);
                            break;
                        case "!=":
                            res = ((dynamic) res != (dynamic) outout);
                            break;
                        case "<=":
                            res = ((dynamic) res <= (dynamic) outout);
                            break;
                        case ">=":
                            res = ((dynamic) res >= (dynamic) outout);
                            break;
                        case "<":
                            res = ((dynamic) res < (dynamic) outout);
                            break;
                        case ">":
                            res = ((dynamic) res > (dynamic) outout);
                            break;
                        default:
                            throw new Exception("");
                    }
                }

                return res;
            }
            if (context.procedureName() != null)
            {
                var name = context.procedureName().GetText();
                var func = Functions.FirstOrDefault(a => a.Name == name);
                if (func == null)
                    throw new Exception("");
                var Callingfunc = new Function()
                {
                    Name = name,
                    BodyContext = func.BodyContext,
                };
                for (var i=0;i<func.Params.Count;i++)
                {
                    var res = VisitExpression(context.expression(i));
                    Callingfunc.Variables.Add(new Variable()
                    {
                        Name = func.Params.ElementAt(i).Value.Name,
                        IsArray = func.Params.ElementAt(i).Value.IsArray,
                        ArrayLength = func.Params.ElementAt(i).Value.ArrayLength,
                        ArrayDim = func.Params.ElementAt(i).Value.ArrayDim,
                        Value = res,
                        HasValue = true,
                        Type = func.Params.ElementAt(i).Value.Type,
                        IsReadOnly = true
                    });
                }
                CallStack.Push(Callingfunc);
                VisitRoutineBody(Callingfunc.BodyContext);
                shallIReturn = false;
                _whomToReturn = null;
                CallStack.Pop();
                return Callingfunc.Output;
            }
            return null;
        }

        // implemented: Done
        // checked: Done
        // exceptions: Done
        // final: Done
        // bugs: Done
        public override object VisitLiteral(SixRGrammerParser.LiteralContext context)
        {
            if (context == null)
                throw new Exception("");
            var walker = context.children[0];
            while (walker.ChildCount != 0) walker = walker.GetChild(0);
            var terminalNodeImpl = walker as TerminalNodeImpl;
            if (terminalNodeImpl == null) return null;
            switch (SixRGrammerLexer.DefaultVocabulary.GetSymbolicName(terminalNodeImpl.Symbol.Type))
            {
                case "FragINTLITERAL":
                    return int.Parse(walker.GetText());
                case "FragFLOATLITERAL":
                    return float.Parse(walker.GetText());
                case "FragCHARLITERAL":
                    return char.Parse(walker.GetText().Remove(walker.GetText().Length - 2, 1).Remove(0, 1));
                case "FragSTRINGLITERAL":
                    return walker.GetText().Remove(walker.GetText().Length - 2, 1).Remove(0, 1);
                case "TRUE":
                    return true;
                case "FALSE":
                    return false;
                default:
                    throw new Exception(SixRConstants.ResourceManager.GetString("AA0010001") + "\r\n in Line: " + context.Start.Line);
            }
        }
        // implemented: Done
        // checked: Done
        // exceptions: Done
        // final: Done
        // bugs: Done
        public override object VisitBooleanLiteral(SixRGrammerParser.BooleanLiteralContext context)
        {
            if (context == null)
                throw new Exception("");
            return bool.Parse(context.GetText());
        }

        // implemented: Done
        // checked: Done
        // exceptions: Done
        // final: Done
        // bugs: Done
        public override object VisitIntLITERAL(SixRGrammerParser.IntLITERALContext context)
        {
            if (context == null)
                throw new Exception("");
            return int.Parse(context.GetText());
        }

        // implemented: Done
        // checked: Done
        // exceptions: Done
        // final: Done
        // bugs: Done
        public override object VisitFloatLITERAL(SixRGrammerParser.FloatLITERALContext context)
        {
            if (context == null)
                throw new Exception("");
            return float.Parse(context.GetText());
        }

        // implemented: Done
        // checked: Done
        // exceptions: Done
        // final: Done
        // bugs: Done
        public override object VisitCharLITERAL(SixRGrammerParser.CharLITERALContext context)
        {
            if (context == null)
                throw new Exception("");
            return char.Parse(context.GetText().Remove(context.GetText().Length - 2, 1).Remove(0, 1));
        }

        // implemented: Done
        // checked: Done
        // exceptions: Done
        // final: Done
        // bugs: Done
        public override object VisitStringLITERAL(SixRGrammerParser.StringLITERALContext context)
        {
            if (context == null)
                throw new Exception("");
            return context.GetText().Remove(context.GetText().Length - 2, 1).Remove(0, 1);
        }
    }
}
