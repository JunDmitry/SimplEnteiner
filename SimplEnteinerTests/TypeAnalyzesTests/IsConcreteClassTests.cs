using SimplEnteinerTests.TestInfrastructure;
using SimplEnteinerTests.TestTypes;
using SimplEnteinerTests.TestTypes.Classes;
using SimplEnteinerTests.TestTypes.Interfaces;
using SimplEnteinerTests.TestTypes.Structs;
using SimplEnteiner;

namespace SimplEnteinerTests.TypeAnalyzesTests
{
    public class IsConcreteClassTests : TypeExtensionsTestBase
    {
        [Fact]
        public void Returns_True_For_Simple_Concrete_Class()
        {
            Assert.True(typeof(SimpleClass).IsConcreteClass());
        }

        [Fact]
        public void Returns_True_For_Sealed_Class()
        {
            Assert.True(typeof(SealedTestClass).IsConcreteClass());
            Assert.True(typeof(SealedInheritFromSimpleClass).IsConcreteClass());
            Assert.True(typeof(TestAssignableSealedImpl).IsConcreteClass());
        }

        [Fact]
        public void Returns_True_For_Class_With_Public_Constructor()
        {
            Assert.True(typeof(SimpleService).IsConcreteClass());
            Assert.True(typeof(PublicTestClass).IsConcreteClass());
            Assert.True(typeof(ClassWithMixedCtors).IsConcreteClass());
        }

        [Fact]
        public void Returns_False_For_Abstract_Class()
        {
            Assert.False(typeof(AbstractInheritFromSimpleClass).IsConcreteClass());
            Assert.False(typeof(AbstractImplementsSimpleService).IsConcreteClass());
            Assert.False(typeof(AbstractGenericBase<>).IsConcreteClass());
            Assert.False(typeof(PlainAbstractClass).IsConcreteClass());
        }

        [Fact]
        public void Returns_False_For_Interface()
        {
            Assert.False(typeof(ISimpleService).IsConcreteClass());
            Assert.False(typeof(ITestAssignable).IsConcreteClass());
            Assert.False(typeof(IMarker).IsConcreteClass());
        }

        [Fact]
        public void Returns_False_For_Open_Generic_Type()
        {
            Assert.False(typeof(OpenGenericBase<>).IsConcreteClass());
            Assert.False(typeof(OpenGenericService<>).IsConcreteClass());
            Assert.False(typeof(OpenFromOpenGeneric<>).IsConcreteClass());
        }

        [Fact]
        public void Returns_True_For_Closed_Generic_Class()
        {
            // Closed generics are concrete classes (not open generic definitions)
            Assert.True(typeof(ClosedFromOpenGeneric).IsConcreteClass());
            Assert.True(typeof(ClosedFromOpenGenericString).IsConcreteClass());
            Assert.True(typeof(TestAssignableClosedGenericInt).IsConcreteClass());
            Assert.True(typeof(ClosedGenericIntService).IsConcreteClass());
            Assert.True(typeof(ClosedFromClosedGeneric).IsConcreteClass());
        }

        [Fact]
        public void Returns_False_For_Static_Class()
        {
            Assert.False(typeof(StaticHelperClass).IsConcreteClass());
            Assert.False(typeof(AnotherStaticClass).IsConcreteClass());
        }

        [Fact]
        public void Returns_False_For_Class_With_Only_Private_Constructor()
        {
            Assert.False(typeof(ClassWithPrivateCtorOnly).IsConcreteClass());
        }

        [Fact]
        public void Returns_False_For_Class_With_Only_Internal_Constructor()
        {
            Assert.False(typeof(ClassWithInternalCtorOnly).IsConcreteClass());
        }

        [Fact]
        public void Returns_False_For_Class_With_Only_Protected_Constructor()
        {
            Assert.False(typeof(ClassWithProtectedCtorOnly).IsConcreteClass());
        }

        [Fact]
        public void Returns_True_For_Class_With_Public_And_Private_Constructors()
        {
            Assert.True(typeof(ClassWithMixedCtors).IsConcreteClass());
        }

        [Fact]
        public void Returns_False_For_Compiler_Generated_Class_When_Flag_Set()
        {
            Assert.False(typeof(ExplicitlyGeneratedClass).IsConcreteClass(isIgnoreGeneratedType: true));
        }

        [Fact]
        public void Returns_True_For_Compiler_Generated_Class_When_Flag_Not_Set()
        {
            // Even with CompilerGenerated attribute, if flag is false, it's considered concrete
            Assert.True(typeof(ExplicitlyGeneratedClass).IsConcreteClass(isIgnoreGeneratedType: false));
        }

        [Fact]
        public void Returns_True_For_Class_With_Auto_Property()
        {
            Assert.True(typeof(ClassWithAutoProperty).IsConcreteClass());
        }

        [Fact]
        public void Returns_True_For_Class_With_Compiler_Generated_Iterator()
        {
            // The outer class is concrete even if it contains compiler-generated nested types
            Assert.True(typeof(ClassWithCompilerGeneratedNested).IsConcreteClass());
        }

        [Fact]
        public void Returns_False_For_Struct()
        {
            Assert.False(typeof(ValueTypeStruct).IsConcreteClass());
            Assert.False(typeof(SimpleStruct).IsConcreteClass());
        }

        [Fact]
        public void Returns_False_For_Enum()
        {
            Assert.False(typeof(TestEnum).IsConcreteClass());
            Assert.False(typeof(SimpleEnum).IsConcreteClass());
        }

        [Fact]
        public void Returns_True_For_Delegate()
        {
            Assert.True(typeof(SimpleDelegate).IsConcreteClass());
            Assert.True(typeof(TestDelegate).IsConcreteClass());
        }

        [Fact]
        public void Closed_Generic_Inheritance_Chain_IsConcrete()
        {
            // ClosedFromOpenGeneric -> ClosedFromClosedGeneric
            Assert.True(typeof(ClosedFromOpenGeneric).IsConcreteClass());
            Assert.True(typeof(ClosedFromClosedGeneric).IsConcreteClass());
        }

        [Fact]
        public void Abstract_Closed_Generic_Is_Not_Concrete()
        {
            Assert.False(typeof(AbstractClosedFromOpenGeneric).IsConcreteClass());
        }

        [Fact]
        public void Concrete_From_Abstract_Closed_Generic_Is_Concrete()
        {
            Assert.True(typeof(ConcreteFromAbstractClosed).IsConcreteClass());
        }

        [Fact]
        public void Class_With_Static_Members_Is_Concrete()
        {
            Assert.True(typeof(ClassWithStaticMembers).IsConcreteClass());
        }

        [Fact]
        public void Nested_Public_Class_Is_Concrete()
        {
            Assert.True(typeof(NestedTypeVisibilityContainer.PublicNestedClass).IsConcreteClass());
        }

        [Fact]
        public void Nested_Private_Class_Is_Not_Concrete()
        {
            // Private nested class has no public constructors accessible
            Assert.False(NestedTypeVisibilityContainer.PrivateNestedClassType.IsConcreteClass());
        }

        [Fact]
        public void Open_Generic_Inheriting_From_Open_Generic_Is_Not_Concrete()
        {
            Assert.False(typeof(OpenFromOpenGeneric<>).IsConcreteClass());
        }

        [Fact]
        public void Multiple_Inheritance_Scenario()
        {
            // Class inheriting from multiple interfaces and base class
            Assert.True(typeof(MultiServiceImplementation).IsConcreteClass());
        }

        [Fact]
        public void Generic_Class_With_Constraints_Is_Concrete_When_Closed()
        {
            Assert.True(typeof(ReferenceTypeConstrainedGenericService<string>).IsConcreteClass());
            Assert.True(typeof(ValueTypeConstrainedGenericService<int>).IsConcreteClass());
        }

        [Fact]
        public void Open_Generic_With_Constraints_Is_Not_Concrete()
        {
            Assert.False(typeof(ReferenceTypeConstrainedGenericService<>).IsConcreteClass());
        }
    }
}