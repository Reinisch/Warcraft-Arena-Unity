using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using JetBrains.Annotations;

using Object = UnityEngine.Object;
using UnityAssert = UnityEngine.Assertions.Assert;

namespace Common
{
    [DebuggerStepThrough]
    public static class Assert
    {
        private const string AssertionDefine = "USE_UNITY_ASSERTIONS";

        [EditorBrowsable(EditorBrowsableState.Never), UsedImplicitly, Obsolete("Assert.Equals should not be used for Assertions", true)]
        public new static bool Equals(object obj1, object obj2)
        {
            throw new InvalidOperationException("Assert.Equals should not be used for Assertions");
        }

        [EditorBrowsable(EditorBrowsableState.Never), UsedImplicitly, Obsolete("Assert.ReferenceEquals should not be used for Assertions", true)]
        public new static bool ReferenceEquals(object obj1, object obj2)
        {
            throw new InvalidOperationException("Assert.ReferenceEquals should not be used for Assertions");
        }

        /// <summary>
        ///   <para>Perfroms failed assertion.</para>
        /// </summary>
        /// <param name="message"></param>
        [Conditional(AssertionDefine)]
        public static void Fail(string message = null) => UnityAssert.IsTrue(false, message);

        /// <summary>
        ///   <para>Asserts that the condition is true.</para>
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="message"></param>
        [Conditional(AssertionDefine)]
        public static void IsTrue(bool condition, string message = null) => UnityAssert.IsTrue(condition, message);

        /// <summary>
        ///   <para>Asserts that the condition is false.</para>
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="message"></param>
        [Conditional(AssertionDefine)]
        public static void IsFalse(bool condition, string message = null) => UnityAssert.IsFalse(condition, message);

        /// <summary>
        ///         <para>Asserts that the values are approximately equal. An absolute error check is used for approximate equality check (|a-b| &lt; tolerance). Default tolerance is 0.00001f.
        /// 
        /// Note: Every time you call the method with tolerance specified, a new instance of Assertions.Comparers.FloatComparer is created. For performance reasons you might want to instance your own comparer and pass it to the AreEqual method. If the tolerance is not specifies, a default comparer is used and the issue does not occur.</para>
        ///       </summary>
        /// <param name="expected"></param>
        /// <param name="actual"></param>
        /// <param name="message"></param>
        [Conditional(AssertionDefine)]
        public static void AreApproximatelyEqual(float expected, float actual, string message = null) => UnityAssert.AreApproximatelyEqual(expected, actual, message);

        /// <summary>
        ///         <para>Asserts that the values are approximately equal. An absolute error check is used for approximate equality check (|a-b| &lt; tolerance). Default tolerance is 0.00001f.
        /// 
        /// Note: Every time you call the method with tolerance specified, a new instance of Assertions.Comparers.FloatComparer is created. For performance reasons you might want to instance your own comparer and pass it to the AreEqual method. If the tolerance is not specifies, a default comparer is used and the issue does not occur.</para>
        ///       </summary>
        /// <param name="tolerance">Tolerance of approximation.</param>
        /// <param name="expected"></param>
        /// <param name="actual"></param>
        /// <param name="message"></param>
        [Conditional(AssertionDefine)]
        public static void AreApproximatelyEqual(float expected, float actual, float tolerance, string message = null) => UnityAssert.AreApproximatelyEqual(expected, actual, tolerance, message);

        /// <summary>
        ///   <para>Asserts that the values are approximately not equal. An absolute error check is used for approximate equality check (|a-b| &lt; tolerance). Default tolerance is 0.00001f.</para>
        /// </summary>
        /// <param name="expected"></param>
        /// <param name="actual"></param>
        /// <param name="message"></param>
        [Conditional(AssertionDefine)]
        public static void AreNotApproximatelyEqual(float expected, float actual, string message = null) => UnityAssert.AreNotApproximatelyEqual(expected, actual, message);

        /// <summary>
        ///   <para>Asserts that the values are approximately not equal. An absolute error check is used for approximate equality check (|a-b| &lt; tolerance). Default tolerance is 0.00001f.</para>
        /// </summary>
        /// <param name="tolerance">Tolerance of approximation.</param>
        /// <param name="expected"></param>
        /// <param name="actual"></param>
        /// <param name="message"></param>
        [Conditional(AssertionDefine)]
        public static void AreNotApproximatelyEqual(float expected, float actual, float tolerance, string message = null) => UnityAssert.AreNotApproximatelyEqual(expected, actual, tolerance, message);

        [Conditional(AssertionDefine)]
        public static void AreEqual<T>(T expected, T actual, string message = null) => UnityAssert.AreEqual(expected, actual, message);

        [Conditional(AssertionDefine)]
        public static void AreEqual<T>(T expected, T actual, string message, IEqualityComparer<T> comparer) => UnityAssert.AreEqual(expected, actual, message, comparer);

        [Conditional(AssertionDefine)]
        public static void AreEqual(Object expected, Object actual, string message) => UnityAssert.AreEqual(expected, actual, message);

        [Conditional(AssertionDefine)]
        public static void AreNotEqual<T>(T expected, T actual) => UnityAssert.AreNotEqual(expected, actual);

        [Conditional(AssertionDefine)]
        public static void AreNotEqual<T>(T expected, T actual, string message) => UnityAssert.AreNotEqual(expected, actual, message);

        [Conditional(AssertionDefine)]
        public static void AreNotEqual<T>(T expected, T actual, string message, IEqualityComparer<T> comparer) => UnityAssert.AreNotEqual(expected, actual, message, comparer);

        [Conditional(AssertionDefine)]
        public static void AreNotEqual(Object expected, Object actual, string message) => UnityAssert.AreNotEqual(expected, actual, message);

        [Conditional(AssertionDefine)]
        public static void IsNull<T>(T value) where T : class => UnityAssert.IsNull(value);

        [Conditional(AssertionDefine)]
        public static void IsNull<T>(T value, string message) where T : class => UnityAssert.IsNull(value, message);

        [Conditional(AssertionDefine)]
        public static void IsNull(Object value, string message) => UnityAssert.IsNull(value, message);

        [Conditional(AssertionDefine), AssertionMethod, ContractAnnotation("value:null=>halt")]
        public static void IsNotNull<T>(T value, string message = null) where T : class => UnityAssert.IsNotNull(value, message);

        [Conditional(AssertionDefine), AssertionMethod, ContractAnnotation("value:null=>halt")]
        public static void IsNotNull(Object value, string message) => UnityAssert.IsNotNull(value, message);

        [Conditional(AssertionDefine)]
        public static void AreEqual(sbyte expected, sbyte actual) => UnityAssert.AreEqual(expected, actual);

        [Conditional(AssertionDefine)]
        public static void AreEqual(sbyte expected, sbyte actual, string message) => UnityAssert.AreEqual(expected, actual);

        [Conditional(AssertionDefine)]
        public static void AreNotEqual(sbyte expected, sbyte actual) => UnityAssert.AreNotEqual(expected, actual);

        [Conditional(AssertionDefine)]
        public static void AreNotEqual(sbyte expected, sbyte actual, string message) => UnityAssert.AreNotEqual(expected, actual);

        [Conditional(AssertionDefine)]
        public static void AreEqual(byte expected, byte actual) => UnityAssert.AreEqual(expected, actual);

        [Conditional(AssertionDefine)]
        public static void AreEqual(byte expected, byte actual, string message) => UnityAssert.AreEqual(expected, actual);

        [Conditional(AssertionDefine)]
        public static void AreNotEqual(byte expected, byte actual) => UnityAssert.AreNotEqual(expected, actual);

        [Conditional(AssertionDefine)]
        public static void AreNotEqual(byte expected, byte actual, string message) => UnityAssert.AreNotEqual(expected, actual);

        [Conditional(AssertionDefine)]
        public static void AreEqual(char expected, char actual) => UnityAssert.AreEqual(expected, actual);

        [Conditional(AssertionDefine)]
        public static void AreEqual(char expected, char actual, string message) => UnityAssert.AreEqual(expected, actual);

        [Conditional(AssertionDefine)]
        public static void AreNotEqual(char expected, char actual) => UnityAssert.AreNotEqual(expected, actual);

        [Conditional(AssertionDefine)]
        public static void AreNotEqual(char expected, char actual, string message) => UnityAssert.AreNotEqual(expected, actual);

        [Conditional(AssertionDefine)]
        public static void AreEqual(short expected, short actual) => UnityAssert.AreEqual(expected, actual);

        [Conditional(AssertionDefine)]
        public static void AreEqual(short expected, short actual, string message) => UnityAssert.AreEqual(expected, actual);

        [Conditional(AssertionDefine)]
        public static void AreNotEqual(short expected, short actual) => UnityAssert.AreNotEqual(expected, actual);

        [Conditional(AssertionDefine)]
        public static void AreNotEqual(short expected, short actual, string message) => UnityAssert.AreNotEqual(expected, actual);

        [Conditional(AssertionDefine)]
        public static void AreEqual(ushort expected, ushort actual) => UnityAssert.AreEqual(expected, actual);

        [Conditional(AssertionDefine)]
        public static void AreEqual(ushort expected, ushort actual, string message) => UnityAssert.AreEqual(expected, actual);

        [Conditional(AssertionDefine)]
        public static void AreNotEqual(ushort expected, ushort actual) => UnityAssert.AreNotEqual(expected, actual);

        [Conditional(AssertionDefine)]
        public static void AreNotEqual(ushort expected, ushort actual, string message) => UnityAssert.AreNotEqual(expected, actual);

        [Conditional(AssertionDefine)]
        public static void AreEqual(int expected, int actual) => UnityAssert.AreEqual(expected, actual);

        [Conditional(AssertionDefine)]
        public static void AreEqual(int expected, int actual, string message) => UnityAssert.AreEqual(expected, actual);

        [Conditional(AssertionDefine)]
        public static void AreNotEqual(int expected, int actual) => UnityAssert.AreNotEqual(expected, actual);

        [Conditional(AssertionDefine)]
        public static void AreNotEqual(int expected, int actual, string message) => UnityAssert.AreNotEqual(expected, actual);

        [Conditional(AssertionDefine)]
        public static void AreEqual(uint expected, uint actual) => UnityAssert.AreEqual(expected, actual);

        [Conditional(AssertionDefine)]
        public static void AreEqual(uint expected, uint actual, string message) => UnityAssert.AreEqual(expected, actual);

        [Conditional(AssertionDefine)]
        public static void AreNotEqual(uint expected, uint actual) => UnityAssert.AreNotEqual(expected, actual);

        [Conditional(AssertionDefine)]
        public static void AreNotEqual(uint expected, uint actual, string message) => UnityAssert.AreNotEqual(expected, actual);

        [Conditional(AssertionDefine)]
        public static void AreEqual(long expected, long actual) => UnityAssert.AreEqual(expected, actual);

        [Conditional(AssertionDefine)]
        public static void AreEqual(long expected, long actual, string message) => UnityAssert.AreEqual(expected, actual);

        [Conditional(AssertionDefine)]
        public static void AreNotEqual(long expected, long actual) => UnityAssert.AreNotEqual(expected, actual);

        [Conditional(AssertionDefine)]
        public static void AreNotEqual(long expected, long actual, string message) => UnityAssert.AreNotEqual(expected, actual);

        [Conditional(AssertionDefine)]
        public static void AreEqual(ulong expected, ulong actual) => UnityAssert.AreEqual(expected, actual);

        [Conditional(AssertionDefine)]
        public static void AreEqual(ulong expected, ulong actual, string message) => UnityAssert.AreEqual(expected, actual);

        [Conditional(AssertionDefine)]
        public static void AreNotEqual(ulong expected, ulong actual) => UnityAssert.AreNotEqual(expected, actual);

        [Conditional(AssertionDefine)]
        public static void AreNotEqual(ulong expected, ulong actual, string message) => UnityAssert.AreNotEqual(expected, actual);
    }
}
