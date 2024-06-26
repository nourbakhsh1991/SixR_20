//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     ANTLR Version: 4.6.5-SNAPSHOT
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from F:\D\WorkSpace\TwinCat\SixR_20\SixR_20\Interpreter\SixRGrammer.g4 by ANTLR 4.6.5-SNAPSHOT

// Unreachable code detected
#pragma warning disable 0162
// The variable '...' is assigned but its value is never used
#pragma warning disable 0219
// Missing XML comment for publicly visible type or member '...'
#pragma warning disable 1591
// Ambiguous reference in cref attribute
#pragma warning disable 419

namespace SixR_20.Interpreter {
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using IToken = Antlr4.Runtime.IToken;
using ParserRuleContext = Antlr4.Runtime.ParserRuleContext;

/// <summary>
/// This class provides an empty implementation of <see cref="ISixRGrammerVisitor{Result}"/>,
/// which can be extended to create a visitor which only needs to handle a subset
/// of the available methods.
/// </summary>
/// <typeparam name="Result">The return type of the visit operation.</typeparam>
[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.6.5-SNAPSHOT")]
[System.CLSCompliant(false)]
public partial class SixRGrammerBaseVisitor<Result> : AbstractParseTreeVisitor<Result>, ISixRGrammerVisitor<Result> {
	/// <summary>
	/// Visit a parse tree produced by the <c>STATASINEPR</c>
	/// labeled alternative in <see cref="SixRGrammerParser.statement"/>.
	/// <para>
	/// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
	/// on <paramref name="context"/>.
	/// </para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	public virtual Result VisitSTATASINEPR([NotNull] SixRGrammerParser.STATASINEPRContext context) { return VisitChildren(context); }

	/// <summary>
	/// Visit a parse tree produced by the <c>STATBRAKE</c>
	/// labeled alternative in <see cref="SixRGrammerParser.statement"/>.
	/// <para>
	/// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
	/// on <paramref name="context"/>.
	/// </para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	public virtual Result VisitSTATBRAKE([NotNull] SixRGrammerParser.STATBRAKEContext context) { return VisitChildren(context); }

	/// <summary>
	/// Visit a parse tree produced by the <c>STATRETURN</c>
	/// labeled alternative in <see cref="SixRGrammerParser.statement"/>.
	/// <para>
	/// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
	/// on <paramref name="context"/>.
	/// </para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	public virtual Result VisitSTATRETURN([NotNull] SixRGrammerParser.STATRETURNContext context) { return VisitChildren(context); }

	/// <summary>
	/// Visit a parse tree produced by the <c>STATLIN</c>
	/// labeled alternative in <see cref="SixRGrammerParser.statement"/>.
	/// <para>
	/// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
	/// on <paramref name="context"/>.
	/// </para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	public virtual Result VisitSTATLIN([NotNull] SixRGrammerParser.STATLINContext context) { return VisitChildren(context); }

	/// <summary>
	/// Visit a parse tree produced by the <c>STATWHILE</c>
	/// labeled alternative in <see cref="SixRGrammerParser.statement"/>.
	/// <para>
	/// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
	/// on <paramref name="context"/>.
	/// </para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	public virtual Result VisitSTATWHILE([NotNull] SixRGrammerParser.STATWHILEContext context) { return VisitChildren(context); }

	/// <summary>
	/// Visit a parse tree produced by the <c>STATEXP</c>
	/// labeled alternative in <see cref="SixRGrammerParser.statement"/>.
	/// <para>
	/// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
	/// on <paramref name="context"/>.
	/// </para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	public virtual Result VisitSTATEXP([NotNull] SixRGrammerParser.STATEXPContext context) { return VisitChildren(context); }

	/// <summary>
	/// Visit a parse tree produced by the <c>STATPTP</c>
	/// labeled alternative in <see cref="SixRGrammerParser.statement"/>.
	/// <para>
	/// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
	/// on <paramref name="context"/>.
	/// </para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	public virtual Result VisitSTATPTP([NotNull] SixRGrammerParser.STATPTPContext context) { return VisitChildren(context); }

	/// <summary>
	/// Visit a parse tree produced by the <c>STATFOR</c>
	/// labeled alternative in <see cref="SixRGrammerParser.statement"/>.
	/// <para>
	/// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
	/// on <paramref name="context"/>.
	/// </para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	public virtual Result VisitSTATFOR([NotNull] SixRGrammerParser.STATFORContext context) { return VisitChildren(context); }

	/// <summary>
	/// Visit a parse tree produced by the <c>STATWAITSEC</c>
	/// labeled alternative in <see cref="SixRGrammerParser.statement"/>.
	/// <para>
	/// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
	/// on <paramref name="context"/>.
	/// </para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	public virtual Result VisitSTATWAITSEC([NotNull] SixRGrammerParser.STATWAITSECContext context) { return VisitChildren(context); }

	/// <summary>
	/// Visit a parse tree produced by the <c>STATNEWLINE</c>
	/// labeled alternative in <see cref="SixRGrammerParser.statement"/>.
	/// <para>
	/// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
	/// on <paramref name="context"/>.
	/// </para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	public virtual Result VisitSTATNEWLINE([NotNull] SixRGrammerParser.STATNEWLINEContext context) { return VisitChildren(context); }

	/// <summary>
	/// Visit a parse tree produced by the <c>STATCIR</c>
	/// labeled alternative in <see cref="SixRGrammerParser.statement"/>.
	/// <para>
	/// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
	/// on <paramref name="context"/>.
	/// </para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	public virtual Result VisitSTATCIR([NotNull] SixRGrammerParser.STATCIRContext context) { return VisitChildren(context); }

	/// <summary>
	/// Visit a parse tree produced by the <c>STATVARDEC</c>
	/// labeled alternative in <see cref="SixRGrammerParser.statement"/>.
	/// <para>
	/// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
	/// on <paramref name="context"/>.
	/// </para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	public virtual Result VisitSTATVARDEC([NotNull] SixRGrammerParser.STATVARDECContext context) { return VisitChildren(context); }

	/// <summary>
	/// Visit a parse tree produced by the <c>STATIF</c>
	/// labeled alternative in <see cref="SixRGrammerParser.statement"/>.
	/// <para>
	/// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
	/// on <paramref name="context"/>.
	/// </para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	public virtual Result VisitSTATIF([NotNull] SixRGrammerParser.STATIFContext context) { return VisitChildren(context); }

	/// <summary>
	/// Visit a parse tree produced by the <c>STATWAITFOR</c>
	/// labeled alternative in <see cref="SixRGrammerParser.statement"/>.
	/// <para>
	/// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
	/// on <paramref name="context"/>.
	/// </para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	public virtual Result VisitSTATWAITFOR([NotNull] SixRGrammerParser.STATWAITFORContext context) { return VisitChildren(context); }

	/// <summary>
	/// Visit a parse tree produced by the <c>STATCONTINUE</c>
	/// labeled alternative in <see cref="SixRGrammerParser.statement"/>.
	/// <para>
	/// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
	/// on <paramref name="context"/>.
	/// </para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	public virtual Result VisitSTATCONTINUE([NotNull] SixRGrammerParser.STATCONTINUEContext context) { return VisitChildren(context); }

	/// <summary>
	/// Visit a parse tree produced by <see cref="SixRGrammerParser.start"/>.
	/// <para>
	/// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
	/// on <paramref name="context"/>.
	/// </para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	public virtual Result VisitStart([NotNull] SixRGrammerParser.StartContext context) { return VisitChildren(context); }

	/// <summary>
	/// Visit a parse tree produced by <see cref="SixRGrammerParser.moduleRoutines"/>.
	/// <para>
	/// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
	/// on <paramref name="context"/>.
	/// </para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	public virtual Result VisitModuleRoutines([NotNull] SixRGrammerParser.ModuleRoutinesContext context) { return VisitChildren(context); }

	/// <summary>
	/// Visit a parse tree produced by <see cref="SixRGrammerParser.mainRoutine"/>.
	/// <para>
	/// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
	/// on <paramref name="context"/>.
	/// </para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	public virtual Result VisitMainRoutine([NotNull] SixRGrammerParser.MainRoutineContext context) { return VisitChildren(context); }

	/// <summary>
	/// Visit a parse tree produced by <see cref="SixRGrammerParser.subRoutine"/>.
	/// <para>
	/// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
	/// on <paramref name="context"/>.
	/// </para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	public virtual Result VisitSubRoutine([NotNull] SixRGrammerParser.SubRoutineContext context) { return VisitChildren(context); }

	/// <summary>
	/// Visit a parse tree produced by <see cref="SixRGrammerParser.procedureName"/>.
	/// <para>
	/// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
	/// on <paramref name="context"/>.
	/// </para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	public virtual Result VisitProcedureName([NotNull] SixRGrammerParser.ProcedureNameContext context) { return VisitChildren(context); }

	/// <summary>
	/// Visit a parse tree produced by <see cref="SixRGrammerParser.formalParameters"/>.
	/// <para>
	/// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
	/// on <paramref name="context"/>.
	/// </para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	public virtual Result VisitFormalParameters([NotNull] SixRGrammerParser.FormalParametersContext context) { return VisitChildren(context); }

	/// <summary>
	/// Visit a parse tree produced by <see cref="SixRGrammerParser.parameter"/>.
	/// <para>
	/// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
	/// on <paramref name="context"/>.
	/// </para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	public virtual Result VisitParameter([NotNull] SixRGrammerParser.ParameterContext context) { return VisitChildren(context); }

	/// <summary>
	/// Visit a parse tree produced by <see cref="SixRGrammerParser.routineBody"/>.
	/// <para>
	/// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
	/// on <paramref name="context"/>.
	/// </para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	public virtual Result VisitRoutineBody([NotNull] SixRGrammerParser.RoutineBodyContext context) { return VisitChildren(context); }

	/// <summary>
	/// Visit a parse tree produced by <see cref="SixRGrammerParser.statementList"/>.
	/// <para>
	/// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
	/// on <paramref name="context"/>.
	/// </para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	public virtual Result VisitStatementList([NotNull] SixRGrammerParser.StatementListContext context) { return VisitChildren(context); }

	/// <summary>
	/// Visit a parse tree produced by <see cref="SixRGrammerParser.statement"/>.
	/// <para>
	/// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
	/// on <paramref name="context"/>.
	/// </para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	public virtual Result VisitStatement([NotNull] SixRGrammerParser.StatementContext context) { return VisitChildren(context); }

	/// <summary>
	/// Visit a parse tree produced by <see cref="SixRGrammerParser.variableDeclaration"/>.
	/// <para>
	/// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
	/// on <paramref name="context"/>.
	/// </para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	public virtual Result VisitVariableDeclaration([NotNull] SixRGrammerParser.VariableDeclarationContext context) { return VisitChildren(context); }

	/// <summary>
	/// Visit a parse tree produced by <see cref="SixRGrammerParser.variableInitialisation"/>.
	/// <para>
	/// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
	/// on <paramref name="context"/>.
	/// </para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	public virtual Result VisitVariableInitialisation([NotNull] SixRGrammerParser.VariableInitialisationContext context) { return VisitChildren(context); }

	/// <summary>
	/// Visit a parse tree produced by <see cref="SixRGrammerParser.variableListRest"/>.
	/// <para>
	/// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
	/// on <paramref name="context"/>.
	/// </para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	public virtual Result VisitVariableListRest([NotNull] SixRGrammerParser.VariableListRestContext context) { return VisitChildren(context); }

	/// <summary>
	/// Visit a parse tree produced by <see cref="SixRGrammerParser.assignmentExpression"/>.
	/// <para>
	/// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
	/// on <paramref name="context"/>.
	/// </para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	public virtual Result VisitAssignmentExpression([NotNull] SixRGrammerParser.AssignmentExpressionContext context) { return VisitChildren(context); }

	/// <summary>
	/// Visit a parse tree produced by <see cref="SixRGrammerParser.sixRJPR"/>.
	/// <para>
	/// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
	/// on <paramref name="context"/>.
	/// </para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	public virtual Result VisitSixRJPR([NotNull] SixRGrammerParser.SixRJPRContext context) { return VisitChildren(context); }

	/// <summary>
	/// Visit a parse tree produced by <see cref="SixRGrammerParser.sixRJXPoint"/>.
	/// <para>
	/// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
	/// on <paramref name="context"/>.
	/// </para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	public virtual Result VisitSixRJXPoint([NotNull] SixRGrammerParser.SixRJXPointContext context) { return VisitChildren(context); }

	/// <summary>
	/// Visit a parse tree produced by <see cref="SixRGrammerParser.sixRPPoint"/>.
	/// <para>
	/// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
	/// on <paramref name="context"/>.
	/// </para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	public virtual Result VisitSixRPPoint([NotNull] SixRGrammerParser.SixRPPointContext context) { return VisitChildren(context); }

	/// <summary>
	/// Visit a parse tree produced by <see cref="SixRGrammerParser.sixRPosition"/>.
	/// <para>
	/// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
	/// on <paramref name="context"/>.
	/// </para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	public virtual Result VisitSixRPosition([NotNull] SixRGrammerParser.SixRPositionContext context) { return VisitChildren(context); }

	/// <summary>
	/// Visit a parse tree produced by <see cref="SixRGrammerParser.sixROrientation"/>.
	/// <para>
	/// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
	/// on <paramref name="context"/>.
	/// </para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	public virtual Result VisitSixROrientation([NotNull] SixRGrammerParser.SixROrientationContext context) { return VisitChildren(context); }

	/// <summary>
	/// Visit a parse tree produced by <see cref="SixRGrammerParser.sixRJPoint"/>.
	/// <para>
	/// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
	/// on <paramref name="context"/>.
	/// </para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	public virtual Result VisitSixRJPoint([NotNull] SixRGrammerParser.SixRJPointContext context) { return VisitChildren(context); }

	/// <summary>
	/// Visit a parse tree produced by <see cref="SixRGrammerParser.sixRJPart"/>.
	/// <para>
	/// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
	/// on <paramref name="context"/>.
	/// </para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	public virtual Result VisitSixRJPart([NotNull] SixRGrammerParser.SixRJPartContext context) { return VisitChildren(context); }

	/// <summary>
	/// Visit a parse tree produced by <see cref="SixRGrammerParser.sixRRPPart"/>.
	/// <para>
	/// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
	/// on <paramref name="context"/>.
	/// </para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	public virtual Result VisitSixRRPPart([NotNull] SixRGrammerParser.SixRRPPartContext context) { return VisitChildren(context); }

	/// <summary>
	/// Visit a parse tree produced by <see cref="SixRGrammerParser.sixRPPart"/>.
	/// <para>
	/// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
	/// on <paramref name="context"/>.
	/// </para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	public virtual Result VisitSixRPPart([NotNull] SixRGrammerParser.SixRPPartContext context) { return VisitChildren(context); }

	/// <summary>
	/// Visit a parse tree produced by <see cref="SixRGrammerParser.sixRRPart"/>.
	/// <para>
	/// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
	/// on <paramref name="context"/>.
	/// </para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	public virtual Result VisitSixRRPart([NotNull] SixRGrammerParser.SixRRPartContext context) { return VisitChildren(context); }

	/// <summary>
	/// Visit a parse tree produced by <see cref="SixRGrammerParser.variableName"/>.
	/// <para>
	/// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
	/// on <paramref name="context"/>.
	/// </para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	public virtual Result VisitVariableName([NotNull] SixRGrammerParser.VariableNameContext context) { return VisitChildren(context); }

	/// <summary>
	/// Visit a parse tree produced by <see cref="SixRGrammerParser.arrayVariableSuffix"/>.
	/// <para>
	/// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
	/// on <paramref name="context"/>.
	/// </para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	public virtual Result VisitArrayVariableSuffix([NotNull] SixRGrammerParser.ArrayVariableSuffixContext context) { return VisitChildren(context); }

	/// <summary>
	/// Visit a parse tree produced by <see cref="SixRGrammerParser.expression"/>.
	/// <para>
	/// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
	/// on <paramref name="context"/>.
	/// </para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	public virtual Result VisitExpression([NotNull] SixRGrammerParser.ExpressionContext context) { return VisitChildren(context); }

	/// <summary>
	/// Visit a parse tree produced by <see cref="SixRGrammerParser.relationalOp"/>.
	/// <para>
	/// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
	/// on <paramref name="context"/>.
	/// </para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	public virtual Result VisitRelationalOp([NotNull] SixRGrammerParser.RelationalOpContext context) { return VisitChildren(context); }

	/// <summary>
	/// Visit a parse tree produced by <see cref="SixRGrammerParser.conditionalOrExpression"/>.
	/// <para>
	/// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
	/// on <paramref name="context"/>.
	/// </para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	public virtual Result VisitConditionalOrExpression([NotNull] SixRGrammerParser.ConditionalOrExpressionContext context) { return VisitChildren(context); }

	/// <summary>
	/// Visit a parse tree produced by <see cref="SixRGrammerParser.exclusiveOrExpression"/>.
	/// <para>
	/// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
	/// on <paramref name="context"/>.
	/// </para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	public virtual Result VisitExclusiveOrExpression([NotNull] SixRGrammerParser.ExclusiveOrExpressionContext context) { return VisitChildren(context); }

	/// <summary>
	/// Visit a parse tree produced by <see cref="SixRGrammerParser.conditionalAndExpression"/>.
	/// <para>
	/// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
	/// on <paramref name="context"/>.
	/// </para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	public virtual Result VisitConditionalAndExpression([NotNull] SixRGrammerParser.ConditionalAndExpressionContext context) { return VisitChildren(context); }

	/// <summary>
	/// Visit a parse tree produced by <see cref="SixRGrammerParser.additiveExpression"/>.
	/// <para>
	/// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
	/// on <paramref name="context"/>.
	/// </para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	public virtual Result VisitAdditiveExpression([NotNull] SixRGrammerParser.AdditiveExpressionContext context) { return VisitChildren(context); }

	/// <summary>
	/// Visit a parse tree produced by <see cref="SixRGrammerParser.multiplicativeExpression"/>.
	/// <para>
	/// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
	/// on <paramref name="context"/>.
	/// </para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	public virtual Result VisitMultiplicativeExpression([NotNull] SixRGrammerParser.MultiplicativeExpressionContext context) { return VisitChildren(context); }

	/// <summary>
	/// Visit a parse tree produced by <see cref="SixRGrammerParser.unaryNotExpression"/>.
	/// <para>
	/// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
	/// on <paramref name="context"/>.
	/// </para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	public virtual Result VisitUnaryNotExpression([NotNull] SixRGrammerParser.UnaryNotExpressionContext context) { return VisitChildren(context); }

	/// <summary>
	/// Visit a parse tree produced by <see cref="SixRGrammerParser.unaryPlusMinuxExpression"/>.
	/// <para>
	/// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
	/// on <paramref name="context"/>.
	/// </para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	public virtual Result VisitUnaryPlusMinuxExpression([NotNull] SixRGrammerParser.UnaryPlusMinuxExpressionContext context) { return VisitChildren(context); }

	/// <summary>
	/// Visit a parse tree produced by <see cref="SixRGrammerParser.primary"/>.
	/// <para>
	/// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
	/// on <paramref name="context"/>.
	/// </para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	public virtual Result VisitPrimary([NotNull] SixRGrammerParser.PrimaryContext context) { return VisitChildren(context); }

	/// <summary>
	/// Visit a parse tree produced by <see cref="SixRGrammerParser.literal"/>.
	/// <para>
	/// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
	/// on <paramref name="context"/>.
	/// </para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	public virtual Result VisitLiteral([NotNull] SixRGrammerParser.LiteralContext context) { return VisitChildren(context); }

	/// <summary>
	/// Visit a parse tree produced by <see cref="SixRGrammerParser.intLITERAL"/>.
	/// <para>
	/// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
	/// on <paramref name="context"/>.
	/// </para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	public virtual Result VisitIntLITERAL([NotNull] SixRGrammerParser.IntLITERALContext context) { return VisitChildren(context); }

	/// <summary>
	/// Visit a parse tree produced by <see cref="SixRGrammerParser.floatLITERAL"/>.
	/// <para>
	/// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
	/// on <paramref name="context"/>.
	/// </para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	public virtual Result VisitFloatLITERAL([NotNull] SixRGrammerParser.FloatLITERALContext context) { return VisitChildren(context); }

	/// <summary>
	/// Visit a parse tree produced by <see cref="SixRGrammerParser.charLITERAL"/>.
	/// <para>
	/// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
	/// on <paramref name="context"/>.
	/// </para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	public virtual Result VisitCharLITERAL([NotNull] SixRGrammerParser.CharLITERALContext context) { return VisitChildren(context); }

	/// <summary>
	/// Visit a parse tree produced by <see cref="SixRGrammerParser.stringLITERAL"/>.
	/// <para>
	/// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
	/// on <paramref name="context"/>.
	/// </para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	public virtual Result VisitStringLITERAL([NotNull] SixRGrammerParser.StringLITERALContext context) { return VisitChildren(context); }

	/// <summary>
	/// Visit a parse tree produced by <see cref="SixRGrammerParser.numberLITERAL"/>.
	/// <para>
	/// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
	/// on <paramref name="context"/>.
	/// </para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	public virtual Result VisitNumberLITERAL([NotNull] SixRGrammerParser.NumberLITERALContext context) { return VisitChildren(context); }

	/// <summary>
	/// Visit a parse tree produced by <see cref="SixRGrammerParser.booleanLiteral"/>.
	/// <para>
	/// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
	/// on <paramref name="context"/>.
	/// </para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	public virtual Result VisitBooleanLiteral([NotNull] SixRGrammerParser.BooleanLiteralContext context) { return VisitChildren(context); }

	/// <summary>
	/// Visit a parse tree produced by <see cref="SixRGrammerParser.parExpression"/>.
	/// <para>
	/// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
	/// on <paramref name="context"/>.
	/// </para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	public virtual Result VisitParExpression([NotNull] SixRGrammerParser.ParExpressionContext context) { return VisitChildren(context); }

	/// <summary>
	/// Visit a parse tree produced by <see cref="SixRGrammerParser.type"/>.
	/// <para>
	/// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
	/// on <paramref name="context"/>.
	/// </para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	public virtual Result VisitType([NotNull] SixRGrammerParser.TypeContext context) { return VisitChildren(context); }

	/// <summary>
	/// Visit a parse tree produced by <see cref="SixRGrammerParser.sixRPrimitiveType"/>.
	/// <para>
	/// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
	/// on <paramref name="context"/>.
	/// </para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	public virtual Result VisitSixRPrimitiveType([NotNull] SixRGrammerParser.SixRPrimitiveTypeContext context) { return VisitChildren(context); }

	/// <summary>
	/// Visit a parse tree produced by <see cref="SixRGrammerParser.primitiveType"/>.
	/// <para>
	/// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
	/// on <paramref name="context"/>.
	/// </para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	public virtual Result VisitPrimitiveType([NotNull] SixRGrammerParser.PrimitiveTypeContext context) { return VisitChildren(context); }
}
} // namespace SixR_20.Interpreter
