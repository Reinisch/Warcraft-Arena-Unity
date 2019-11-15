using System;

#pragma warning disable 1591
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable IntroduceOptionalParameters.Global
// ReSharper disable MemberCanBeProtected.Global
// ReSharper disable InconsistentNaming
// ReSharper disable CheckNamespace

namespace JetBrains.Annotations
{
	/// <summary>
	/// Indicates that the value of the marked element could be <c>null</c> sometimes,
	/// so the check for <c>null</c> is necessary before its usage.
	/// </summary>
	/// <example><code>
	/// [CanBeNull] public object Test() { return null; }
	/// public void UseTest() {
	///   var p = Test();
	///   var s = p.ToString(); // Warning: Possible 'System.NullReferenceException'
	/// }
	/// </code></example>
	[AttributeUsage(
		AttributeTargets.Method | AttributeTargets.Parameter | AttributeTargets.Property |
		AttributeTargets.Delegate | AttributeTargets.Field | AttributeTargets.Event )]
	public sealed class CanBeNullAttribute : Attribute
	{
	}

	/// <summary>
	/// Indicates that the value of the marked element could never be <c>null</c>.
	/// </summary>
	/// <example><code>
	/// [NotNull] public object Foo() {
	///   return null; // Warning: Possible 'null' assignment
	/// }
	/// </code></example>
	[AttributeUsage(
		AttributeTargets.Method | AttributeTargets.Parameter | AttributeTargets.Property |
		AttributeTargets.Delegate | AttributeTargets.Field | AttributeTargets.Event )]
	public sealed class NotNullAttribute : Attribute
	{
	}

	/// <summary>
	/// Describes dependency between method input and output.
	/// </summary>
	/// <syntax>
	/// <p>Function Definition Table syntax:</p>
	/// <list>
	/// <item>FDT      ::= FDTRow [;FDTRow]*</item>
	/// <item>FDTRow   ::= Input =&gt; Output | Output &lt;= Input</item>
	/// <item>Input    ::= ParameterName: Value [, Input]*</item>
	/// <item>Output   ::= [ParameterName: Value]* {halt|stop|void|nothing|Value}</item>
	/// <item>Value    ::= true | false | null | notnull | canbenull</item>
	/// </list>
	/// If method has single input parameter, it's name could be omitted.<br/>
	/// Using <c>halt</c> (or <c>void</c>/<c>nothing</c>, which is the same)
	/// for method output means that the methos doesn't return normally.<br/>
	/// <c>canbenull</c> annotation is only applicable for output parameters.<br/>
	/// You can use multiple <c>[ContractAnnotation]</c> for each FDT row,
	/// or use single attribute with rows separated by semicolon.<br/>
	/// </syntax>
	/// <examples><list>
	/// <item><code>
	/// [ContractAnnotation("=> halt")]
	/// public void TerminationMethod()
	/// </code></item>
	/// <item><code>
	/// [ContractAnnotation("halt &lt;= condition: false")]
	/// public void Assert(bool condition, string text) // regular assertion method
	/// </code></item>
	/// <item><code>
	/// [ContractAnnotation("s:null => true")]
	/// public bool IsNullOrEmpty(string s) // string.IsNullOrEmpty()
	/// </code></item>
	/// <item><code>
	/// // A method that returns null if the parameter is null,
	/// // and not null if the parameter is not null
	/// [ContractAnnotation("null => null; notnull => notnull")]
	/// public object Transform(object data) 
	/// </code></item>
	/// <item><code>
	/// [ContractAnnotation("s:null=>false; =>true,result:notnull; =>false, result:null")]
	/// public bool TryParse(string s, out Person result)
	/// </code></item>
	/// </list></examples>
	[AttributeUsage( AttributeTargets.Method, AllowMultiple = true )]
	public sealed class ContractAnnotationAttribute : Attribute
	{
		public ContractAnnotationAttribute( [NotNull] string contract )
			: this( contract, false )
		{
		}

		public ContractAnnotationAttribute( [NotNull] string contract, bool forceFullStates )
		{
			Contract = contract;
			ForceFullStates = forceFullStates;
		}

		public string Contract { get; private set; }
		public bool ForceFullStates { get; private set; }
	}
}