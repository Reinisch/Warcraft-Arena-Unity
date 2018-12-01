using System;

/// <summary>
/// Sets the Unity script execution order
/// </summary>
/// <example>
/// *Example:* Setting the execution order of a manager class using an attribute.
/// 
/// ```csharp
/// [BoltExecutionOrder(-5000)]
/// public class SoundManager : MonoBehaviour {
///   void Awake() {
///     ConfigureSoundSettings();
///   }
/// }
/// ```
/// </example>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class BoltExecutionOrderAttribute : Attribute {
  readonly int _executionOrder;

  public BoltExecutionOrderAttribute(int order) {
    _executionOrder = order;
  }

  /// <summary>
  /// The order of this script in execution (lower is earlier)
  /// </summary>
  public int executionOrder {
    get { return _executionOrder; }
  }
}