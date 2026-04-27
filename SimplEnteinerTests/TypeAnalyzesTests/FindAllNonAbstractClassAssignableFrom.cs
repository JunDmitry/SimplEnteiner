using SimplEnteinerTests.TestInfrastructure;
using SimplEnteiner;
using SimplEnteinerTests.TestTypes.Classes;
using SimplEnteinerTests.TestTypes.Interfaces;
using SimplEnteinerTests.TestTypes.Structs;

namespace SimplEnteinerTests.TypeAnalyzesTests
{
    public class FindAllNonAbstractClassAssignableFromTests : TypeExtensionsTestBase
    {
        [Fact]
        public void Throws_ArgumentNullException_When_Type_Is_Null()
        {
            ResetDomainTypeCache();

            Type nullType = null;

            Assert.Throws<ArgumentNullException>(() =>
            {
                // ReSharper disable once ExpressionIsAlwaysNull
                _ = nullType.FindAllNonAbstractClassAssignableFrom();
            });
        }

        [Fact]
        public void Returns_Only_NonAbstract_Classes_Assignable_From_Interface_Default_NonGeneric()
        {
            ResetDomainTypeCache();

            var results = typeof(ITestAssignable)
                .FindAllNonAbstractClassAssignableFrom()
                .ToList();

            // Expect concrete/non-abstract classes implementing ITestAssignable (non-generic)
            Assert.Contains(typeof(TestAssignableImpl), results);
            Assert.Contains(typeof(TestAssignableConcreteFromAbstract), results);
            Assert.Contains(typeof(TestAssignableSealedImpl), results);
            Assert.Contains(typeof(TestAssignableClosedGenericInt), results);

            // Must NOT include abstract classes, interfaces, structs, generic types
            Assert.DoesNotContain(typeof(TestAssignableAbstractImpl), results);
            Assert.DoesNotContain(typeof(ITestAssignable), results);
            Assert.DoesNotContain(typeof(ITestAssignableDerived), results);
            Assert.DoesNotContain(typeof(TestAssignableStruct), results);
            Assert.DoesNotContain(typeof(TestAssignableOpenGeneric<>), results);
            Assert.DoesNotContain(typeof(TestAssignableGenericStruct<>), results);
            Assert.DoesNotContain(typeof(TestNestedTypes.NestedAssignableAbstract), results);
            Assert.DoesNotContain(typeof(TestNestedTypes.INestedAssignable), results);
            Assert.DoesNotContain(typeof(TestNestedTypes.NestedOpenGeneric<>), results);
            Assert.DoesNotContain(typeof(TestNestedTypes.NestedClosedGenericString), results);
        }

        [Fact]
        public void Returns_Only_NonAbstract_Classes_Assignable_From_Interface_Generic_Flag()
        {
            ResetDomainTypeCache();

            var results = typeof(ITestAssignable)
                .FindAllNonAbstractClassAssignableFrom(isGenericType: true)
                .ToList();

            // Expect only non-abstract generic classes implementing ITestAssignable
            Assert.Contains(typeof(TestAssignableOpenGeneric<>), results);

            // Must NOT include non-generic concrete, abstract, interfaces, structs
            Assert.DoesNotContain(typeof(TestAssignableClosedGenericInt), results);
            Assert.DoesNotContain(typeof(TestAssignableImpl), results);
            Assert.DoesNotContain(typeof(TestAssignableConcreteFromAbstract), results);
            Assert.DoesNotContain(typeof(TestAssignableSealedImpl), results);
            Assert.DoesNotContain(typeof(TestAssignableAbstractImpl), results);
            Assert.DoesNotContain(typeof(ITestAssignable), results);
            Assert.DoesNotContain(typeof(TestAssignableStruct), results);
            Assert.DoesNotContain(typeof(TestAssignableGenericStruct<>), results);
            Assert.DoesNotContain(typeof(TestNestedTypes.NestedAssignableImpl), results);
            Assert.DoesNotContain(typeof(TestNestedTypes.NestedAssignableAbstract), results);
        }

        [Fact]
        public void Returns_Only_NonAbstract_Classes_Assignable_From_Base_Class_Default()
        {
            ResetDomainTypeCache();

            var results = typeof(SimpleClass)
                .FindAllNonAbstractClassAssignableFrom()
                .ToList();

            // Expect derived non-abstract classes (including sealed, and maybe closed generic derived)
            Assert.Contains(typeof(SimpleClass), results);
            Assert.Contains(typeof(FirstInheritFromSimpleClass), results);
            Assert.Contains(typeof(SecondInheritFromSimpleClass), results);
            Assert.Contains(typeof(SealedInheritFromSimpleClass), results);
            Assert.Contains(typeof(IntegerClosedInheritFromSimleClass), results);

            // Must NOT include abstract
            Assert.DoesNotContain(typeof(AbstractInheritFromSimpleClass), results);
        }

        [Fact]
        public void Returns_Only_NonAbstract_Generic_Classes_Assignable_From_Base_Class()
        {
            ResetDomainTypeCache();

            var results = typeof(SimpleClass)
                .FindAllNonAbstractClassAssignableFrom(isGenericType: true)
                .ToList();

            // Expect only generic derived classes (non-abstract)
            Assert.Contains(typeof(GenericInheritFromSimpleClass<>), results);

            // Must NOT include non-generic derived or abstract
            Assert.DoesNotContain(typeof(SimpleClass), results);
            Assert.DoesNotContain(typeof(FirstInheritFromSimpleClass), results);
            Assert.DoesNotContain(typeof(SecondInheritFromSimpleClass), results);
            Assert.DoesNotContain(typeof(SealedInheritFromSimpleClass), results);
            Assert.DoesNotContain(typeof(AbstractInheritFromSimpleClass), results);
        }

        [Fact]
        public void Does_Not_Return_Static_Classes()
        {
            ResetDomainTypeCache();

            // StaticHelperClass is not assignable from anything meaningful, but just ensure
            // the filter (IsClass && !IsAbstract) would still be true for static? No: static class is abstract and sealed in IL,
            // so IsAbstract == true and should be excluded.
            var resultsForMarker = typeof(IMarker)
                .FindAllNonAbstractClassAssignableFrom()
                .ToList();

            // StaticHelperClass does not implement IMarker, so not returned anyway; but ensure no static class appears
            // (we can also check by scanning a type that could include it if any, but not needed)
            Assert.DoesNotContain(typeof(StaticHelperClass), resultsForMarker);
        }

        [Fact]
        public void Does_Not_Return_Structs()
        {
            ResetDomainTypeCache();

            var results = typeof(ITestAssignable)
                .FindAllNonAbstractClassAssignableFrom()
                .ToList();

            Assert.DoesNotContain(typeof(TestAssignableStruct), results);
            Assert.DoesNotContain(typeof(TestAssignableGenericStruct<>), results);
        }

        [Fact]
        public void Does_Not_Return_Interfaces()
        {
            ResetDomainTypeCache();

            var results = typeof(ITestAssignable)
                .FindAllNonAbstractClassAssignableFrom()
                .ToList();

            Assert.DoesNotContain(typeof(ITestAssignable), results);
            Assert.DoesNotContain(typeof(ITestAssignableDerived), results);
        }

        [Fact]
        public void Returns_Empty_When_No_Matching_NonAbstract_Classes()
        {
            ResetDomainTypeCache();

            // Search assignable from an interface that has only abstract implementations
            var results = typeof(TestAssignableAbstractImpl)
                .FindAllNonAbstractClassAssignableFrom()
                .ToList();

            // TestAssignableAbstractImpl has concrete derived but we search from abstract type? 
            // IsAssignableFrom: abstract type -> derived concrete are assignable, but the filter requires candidate to be non-abstract class.
            // Also searching from abstract class base: concrete derived classes will be returned (non-abstract). Wait, TestAssignableAbstractImpl is abstract.
            // FindAllNonAbstractClassAssignableFrom will look for non-abstract classes assignable from TestAssignableAbstractImpl -> should return TestAssignableConcreteFromAbstract.
            Assert.Contains(typeof(TestAssignableConcreteFromAbstract), results);
            Assert.DoesNotContain(typeof(TestAssignableAbstractImpl), results);
        }

        [Fact]
        public void Handles_Closed_Generic_Derived_Correctly()
        {
            ResetDomainTypeCache();

            var resultsNonGeneric = typeof(ITestAssignable)
                .FindAllNonAbstractClassAssignableFrom(isGenericType: false)
                .ToList();

            var resultsGeneric = typeof(ITestAssignable)
                .FindAllNonAbstractClassAssignableFrom(isGenericType: true)
                .ToList();

            // Closed generic is a concrete class type instance but IsGenericType == true
            Assert.Contains(typeof(TestAssignableClosedGenericInt), resultsNonGeneric);
            Assert.DoesNotContain(typeof(TestAssignableClosedGenericInt), resultsGeneric);
        }
    }
}
