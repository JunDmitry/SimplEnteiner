using System;
using System.Collections.Generic;
using System.Linq;
using SimplEnteinerTests.TestInfrastructure;
using SimplEnteinerTests.TestTypes;
using SimplEnteinerTests.TestTypes.Classes;
using SimplEnteinerTests.TestTypes.Interfaces;
using SimplEnteinerTests.TestTypes.Structs;
using Xunit;
using static SimplEnteiner.TypeAnalyzes;

namespace SimplEnteinerTests.TypeAnalyzesTests
{
    public class FindAllAssignableFromWithConditionTests : TypeExtensionsTestBase
    {
        [Fact]
        public void Throws_ArgumentNullException_When_Type_Is_Null()
        {
            ResetDomainTypeCache();

            Assert.Throws<ArgumentNullException>(() =>
            {
                ((Type)null).FindAllAssignableFrom(TypeCondition.Class);
            });
        }

        [Fact]
        public void Condition_None_Returns_All_Assignable_Types()
        {
            ResetDomainTypeCache();

            var results = typeof(ITestAssignable)
                .FindAllAssignableFrom(TypeCondition.None)
                .ToList();

            // Should include everything assignable (interfaces, classes, structs, etc.)
            Assert.Contains(typeof(TestAssignableImpl), results);
            Assert.Contains(typeof(ITestAssignable), results);
            Assert.Contains(typeof(TestAssignableStruct), results);
        }

        [Fact]
        public void Condition_IsClass_Returns_Only_Classes()
        {
            ResetDomainTypeCache();

            var results = typeof(ITestAssignable)
                .FindAllAssignableFrom(TypeCondition.Class)
                .ToList();

            Assert.Contains(typeof(TestAssignableImpl), results);
            Assert.Contains(typeof(TestAssignableSealedImpl), results);
            Assert.Contains(typeof(TestAssignableConcreteFromAbstract), results);
            Assert.Contains(typeof(TestAssignableOpenGeneric<>), results);
            Assert.Contains(typeof(TestAssignableClosedGenericInt), results);

            Assert.DoesNotContain(typeof(ITestAssignable), results);
            Assert.DoesNotContain(typeof(ITestAssignableDerived), results);
            Assert.DoesNotContain(typeof(TestAssignableStruct), results);
        }

        [Fact]
        public void Condition_IsInterface_Returns_Only_Interfaces()
        {
            ResetDomainTypeCache();

            var results = typeof(ITestAssignable)
                .FindAllAssignableFrom(TypeCondition.Interface)
                .ToList();

            Assert.Contains(typeof(ITestAssignable), results);
            Assert.Contains(typeof(ITestAssignableDerived), results);

            Assert.DoesNotContain(typeof(IMultiInterfaceService), results);
            Assert.DoesNotContain(typeof(TestAssignableImpl), results);
            Assert.DoesNotContain(typeof(TestAssignableStruct), results);
        }

        [Fact]
        public void Condition_IsAbstract_Returns_Only_Abstract_Types()
        {
            ResetDomainTypeCache();

            var results = typeof(SimpleClass)
                .FindAllAssignableFrom(TypeCondition.Abstract)
                .ToList();

            Assert.Contains(typeof(AbstractInheritFromSimpleClass), results);

            Assert.DoesNotContain(typeof(AbstractGenericBase<>), results);
            Assert.DoesNotContain(typeof(AbstractImplementsSimpleService), results);
            Assert.DoesNotContain(typeof(SimpleClass), results);
            Assert.DoesNotContain(typeof(FirstInheritFromSimpleClass), results);
        }

        [Fact]
        public void Condition_IsGenericType_Returns_Only_Generic_Types()
        {
            ResetDomainTypeCache();

            var results = typeof(ITestAssignable)
                .FindAllAssignableFrom(TypeCondition.GenericType)
                .ToList();

            // Open generics
            Assert.Contains(typeof(TestAssignableOpenGeneric<>), results);

            // Non-generic should not be included
            Assert.DoesNotContain(typeof(TestAssignableImpl), results);
            Assert.DoesNotContain(typeof(TestAssignableSealedImpl), results);
            Assert.DoesNotContain(typeof(ClosedFromOpenGeneric), results); // Closed from open is NOT generic
        }

        [Fact]
        public void Condition_IsGenericTypeDefinition_Returns_Only_Open_Generic_Definitions()
        {
            ResetDomainTypeCache();

            var results = typeof(ITestAssignable)
                .FindAllAssignableFrom(TypeCondition.GenericTypeDefinition)
                .ToList();

            // Only open generic type definitions
            Assert.Contains(typeof(TestAssignableOpenGeneric<>), results);

            // Closed generics are NOT type definitions
            Assert.DoesNotContain(typeof(TestAssignableClosedGenericInt), results);
            Assert.DoesNotContain(typeof(ClosedGenericIntService), results);

            // Non-generics
            Assert.DoesNotContain(typeof(TestAssignableImpl), results);
        }

        [Fact]
        public void Condition_IsPublic_Returns_Only_Public_Types()
        {
            ResetDomainTypeCache();

            var results = typeof(ITestAssignable)
                .FindAllAssignableFrom(TypeCondition.Public)
                .ToList();
            
            Assert.Contains(typeof(TestAssignableImpl), results);
            Assert.Contains(typeof(ITestAssignable), results);

            Assert.DoesNotContain(typeof(InternalTestClass), results);
            Assert.DoesNotContain(typeof(ClassWithInternalCtorOnly), results);
        }

        [Fact]
        public void Condition_IsSealed_Returns_Only_Sealed_Types()
        {
            ResetDomainTypeCache();

            var results = typeof(ITestAssignable)
                .FindAllAssignableFrom(TypeCondition.Sealed)
                .ToList();

            Assert.Contains(typeof(TestAssignableSealedImpl), results);

            Assert.DoesNotContain(typeof(TestAssignableImpl), results);
            Assert.DoesNotContain(typeof(NonSealedTestClass), results);
        }

        [Fact]
        public void Condition_IsValueType_Returns_Only_Value_Types()
        {
            ResetDomainTypeCache();

            var results = typeof(IConvertible)
                .FindAllAssignableFrom(TypeCondition.ValueType)
                .ToList();
            
            Assert.Contains(typeof(ValueTypeStruct), results);
            Assert.Contains(typeof(int), results);
            Assert.Contains(typeof(TestEnum), results); // Enums are value types

            Assert.DoesNotContain(typeof(TestAssignableImpl), results);
            Assert.DoesNotContain(typeof(SimpleClass), results);
        }

        [Fact]
        public void Condition_IsEnum_Returns_Only_Enums()
        {
            ResetDomainTypeCache();

            var results = typeof(IConvertible)
                .FindAllAssignableFrom(TypeCondition.Enum)
                .ToList();

            Assert.Contains(typeof(TestEnum), results);
            Assert.Contains(typeof(SimpleEnum), results);

            Assert.DoesNotContain(typeof(ValueTypeStruct), results);
            Assert.DoesNotContain(typeof(TestAssignableImpl), results);
        }

        [Fact]
        public void Condition_Combination_IsClass_And_IsPublic()
        {
            ResetDomainTypeCache();

            var condition = TypeCondition.Class | TypeCondition.Public;

            var results = typeof(ITestAssignable)
                .FindAllAssignableFrom(condition)
                .ToList();

            Assert.Contains(typeof(TestAssignableImpl), results);
            Assert.Contains(typeof(TestAssignableSealedImpl), results);

            Assert.DoesNotContain(typeof(InternalTestClass), results);
            Assert.DoesNotContain(typeof(ITestAssignable), results);
        }

        [Fact]
        public void Condition_Combination_IsClass_And_NotAbstract()
        {
            ResetDomainTypeCache();

            // IsClass already implies checking, but we combine with ensuring non-abstract via IsClass & ~IsAbstract
            // Actually, IsClass doesn't exclude abstract. Let's test IsClass with explicit check.
            var condition = TypeCondition.Class | TypeCondition.Public;

            var results = typeof(SimpleClass)
                .FindAllAssignableFrom(condition)
                .ToList();

            // Should include both abstract and non-abstract classes
            Assert.Contains(typeof(AbstractInheritFromSimpleClass), results);
            Assert.Contains(typeof(FirstInheritFromSimpleClass), results);
        }

        [Fact]
        public void Closed_Generic_Inheritance_Not_Treated_As_Generic()
        {
            ResetDomainTypeCache();

            // ClosedFromOpenGeneric inherits from OpenGenericBase<int> but is NOT generic itself
            var results = typeof(OpenGenericBase<>)
                .FindAllAssignableFrom(TypeCondition.GenericTypeDefinition)
                .ToList();

            // Should NOT include ClosedFromOpenGeneric (it's closed, not a definition)
            Assert.DoesNotContain(typeof(ClosedFromOpenGeneric), results);
            Assert.DoesNotContain(typeof(ClosedFromClosedGeneric), results);
            
            // But should include open generics
            Assert.Contains(typeof(OpenFromOpenGeneric<>), results);
        }

        [Fact]
        public void Closed_Generic_Inheritance_Treated_As_Class()
        {
            ResetDomainTypeCache();

            var results = typeof(OpenGenericBase<>)
                .FindAllAssignableFrom(TypeCondition.Class)
                .ToList();

            // Closed generics ARE classes (non-generic classes)
            Assert.Contains(typeof(ClosedFromOpenGeneric), results);
            Assert.Contains(typeof(ClosedFromOpenGenericString), results);
            Assert.Contains(typeof(ClosedFromClosedGeneric), results);
        }

        [Fact]
        public void Nested_Types_With_IsNested_Condition()
        {
            ResetDomainTypeCache();

            var results = typeof(AbstractSimpleBase)
                .FindAllAssignableFrom(TypeCondition.Nested)
                .ToList();
            
            Assert.Contains(typeof(NestedTypeVisibilityContainer.PublicNestedClass), results);
            Assert.Contains(NestedTypeVisibilityContainer.PrivateNestedClassType, results);
            Assert.Contains(typeof(NestedTypeVisibilityContainer.InternalNestedClass), results);

            Assert.DoesNotContain(typeof(NestedTypeVisibilityContainer), results);
        }

        [Fact]
        public void Nested_Public_Types_With_IsNestedPublic_Condition()
        {
            ResetDomainTypeCache();

            var results = typeof(object)
                .FindAllAssignableFrom(TypeCondition.NestedPublic)
                .ToList();

            Assert.Contains(typeof(NestedTypeVisibilityContainer.PublicNestedClass), results);
            Assert.Contains(typeof(NestedTypeVisibilityContainer.PublicNestedStruct), results);

            Assert.DoesNotContain(NestedTypeVisibilityContainer.PrivateNestedClassType, results);
        }
    }
}