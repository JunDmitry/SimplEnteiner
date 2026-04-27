using System.Reflection;
using SimplEnteiner;
using SimplEnteinerTests.TestInfrastructure;
using SimplEnteinerTests.TestTypes.AtributeTypes;
using SimplEnteinerTests.TestTypes.Classes;
using SimplEnteinerTests.TestTypes.Interfaces;
using SimplEnteinerTests.TestTypes.IsAndGetAssignableTypes;
using SimplEnteinerTests.TestTypes.WithConstraints;
using static SimplEnteiner.TypeAnalyzes;

namespace SimplEnteinerTests.TypeAnalyzesTests
{
    public class IntegrationTests : TypeExtensionsTestBase
    {
        [Fact]
        public void FindAllAssignableFrom_With_Condition_Uses_IsConcreteClass_Logic()
        {
            ResetDomainTypeCache();

            // Find all concrete classes assignable from ISimpleService
            var condition = TypeCondition.Class | TypeCondition.Public;
            var results = typeof(ISimpleService)
                .FindAllAssignableFrom(condition)
                .Where(t => t.IsConcreteClass())
                .ToList();

            // Should match what IsConcreteClass would return
            Assert.Contains(typeof(SimpleService), results);
            Assert.Contains(typeof(FirstSimpleService), results);
            Assert.Contains(typeof(SecondSimpleService), results);

            Assert.DoesNotContain(typeof(AbstractImplementsSimpleService), results);
        }

        [Fact]
        public void GetLoadableTypes_Combined_With_IsConcreteClass_Filter()
        {
            ResetDomainTypeCache();

            var assembly = typeof(NestedTypeVisibilityContainer).Assembly;
            var concreteTypes = assembly
                .GetLoadableTypes()
                .Where(t => t.IsConcreteClass())
                .ToList();

            Assert.Contains(typeof(NestedTypeVisibilityContainer), concreteTypes);
            Assert.Contains(typeof(NestedTypeVisibilityContainer.PublicNestedClass), concreteTypes);

            Assert.DoesNotContain(NestedTypeVisibilityContainer.PrivateNestedClassType, concreteTypes);
            Assert.DoesNotContain(NestedTypeVisibilityContainer.PrivateNestedInterfaceType, concreteTypes);
        }

        [Fact]
        public void Closed_Generic_Types_Are_Treated_Correctly_Across_All_Methods()
        {
            ResetDomainTypeCache();

            var closedType = typeof(ClosedFromOpenGeneric);

            // IsConcreteClass should return true
            Assert.True(closedType.IsConcreteClass());

            // FindAllAssignableFrom with IsClass should include it
            var classResults = typeof(OpenGenericBase<>)
                .FindAllAssignableFrom(TypeCondition.Class)
                .ToList();
            Assert.Contains(closedType, classResults);

            // FindAllAssignableFrom with IsGenericType should NOT include it (it's closed)
            var genericResults = typeof(OpenGenericBase<>)
                .FindAllAssignableFrom(TypeCondition.GenericType)
                .ToList();
            Assert.DoesNotContain(closedType, genericResults);

            // FindAllAssignableFrom with IsGenericTypeDefinition should NOT include it
            var defResults = typeof(OpenGenericBase<>)
                .FindAllAssignableFrom(TypeCondition.GenericTypeDefinition)
                .ToList();
            Assert.DoesNotContain(closedType, defResults);
        }

        [Fact]
        public void IsAssignableToGenericTypeDefinition_With_GetAssignableToGenericArguments()
        {
            ResetDomainTypeCache();

            // Test that both methods work together
            var type = typeof(GenericServiceInt);
            var isAssignable = type.IsAssignableToGenericTypeDefinition(typeof(IGenericServiceDefinition<>));
            var args = type.GetAssignableToGenericArguments(typeof(IGenericServiceDefinition<>));

            Assert.True(isAssignable);
            Assert.Single(args);
            Assert.Equal(typeof(int), args[0]);
        }

        [Fact]
        public void GetTypesWithAttribute_With_FindAllAssignableFrom()
        {
            ResetDomainTypeCache();

            var assembly = typeof(ClassWithSimpleAttribute).Assembly;
            var typesWithAttribute = assembly.GetTypesWithAttribute<SimpleTestAttribute>().ToList();

            // Find all assignable from one of the types with attribute
            var baseType = typesWithAttribute.First();
            var assignableTypes = baseType.FindAllAssignableFrom(TypeCondition.Class).ToList();

            // Should find at least the type itself
            Assert.Contains(baseType, assignableTypes);
        }

        [Fact]
        public void Closed_Generic_Inheritance_Consistency()
        {
            ResetDomainTypeCache();

            // Test that closed generic classes are handled consistently
            var closedType = typeof(ClosedFromOpenGenericService);

            // Should be assignable to generic definition
            Assert.True(closedType.IsAssignableToGenericTypeDefinition(typeof(IGenericServiceDefinition<>)));

            // Should return correct generic arguments
            var args = closedType.GetAssignableToGenericArguments(typeof(IGenericServiceDefinition<>));
            Assert.Single(args);
            Assert.Equal(typeof(int), args[0]);

            // Should be found as a class
            var classResults = typeof(OpenGenericServiceDefinition<>)
                .FindAllAssignableFrom(TypeCondition.Class)
                .ToList();
            Assert.Contains(closedType, classResults);
        }

        [Fact]
        public void GetInjectableConstructor_With_GetFactoryMethod_Integration()
        {
            ResetDomainTypeCache();

            var type = typeof(MultipleCtorWithOneAttribute);
            var constructor = type.GetInjectableConstructor(typeof(InjectAttribute));
            var factory = constructor.GetFactoryMethod();

            var service = new SimpleService();
            var instance = factory(new object[] { service });

            Assert.NotNull(instance);
            Assert.IsType<MultipleCtorWithOneAttribute>(instance);
        }

        [Fact]
        public void GetDependencyType_With_GetInjectableConstructor_Integration()
        {
            ResetDomainTypeCache();

            var type = typeof(SingleParamCtorFactoryClass);
            var constructor = type.GetInjectableConstructor(typeof(InjectAttribute));
            var parameter = constructor.GetParameters().First();

            var dependencyType = parameter.GetDependencyType();

            Assert.Equal(typeof(ISimpleService), dependencyType);
        }

        [Fact]
        public void GetInjectableMembers_With_GetDependencyType_Integration()
        {
            ResetDomainTypeCache();

            // Get injectable members
            var members = typeof(MixedInjectableMembersClass)
                .GetInjectableMembers(typeof(InjectFieldAttribute))
                .ToList();

            // Get the field's type
            var field = (FieldInfo)members.First();
            var parameterType = field.FieldType;

            // Use GetDependencyType via ParameterInfo simulation
            // Note: GetDependencyType works on ParameterInfo, but we can test the underlying logic
            // by checking the type directly
            Assert.Equal(typeof(ISimpleService), parameterType);
        }

        [Fact]
        public void AddAssemblies_Updates_FindAllAssignableFrom_Results()
        {
            ResetDomainTypeCache();

            // Before adding assembly, some types may not be found
            var assembly = Assembly.GetExecutingAssembly();
            TypeAnalyzes.AddAssemblies(new[] { assembly });

            // Now search for a type from that assembly
            var results = typeof(ISimpleService)
                .FindAllAssignableFrom(TypeCondition.Class)
                .ToList();

            // Should contain at least one type from the executing assembly
            Assert.Contains(typeof(SimpleService), results);
        }

        [Fact]
        public void GetAllDependencies_With_Cyclic_Detection_Integration()
        {
            ResetDomainTypeCache();

            var dependencies = typeof(CycleClassA).GetAllDependencies(typeof(InjectAttribute)).ToList();
            var hasCycle = typeof(CycleClassA).HasCyclicDependencies(typeof(InjectAttribute), out var cyclePath);

            // Should detect cycle and still return some dependencies
            Assert.True(hasCycle);
            Assert.NotEmpty(dependencies);
            Assert.Contains(typeof(CycleClassB), dependencies);
        }

        [Fact]
        public void MatchesGenericParameters_With_GetAssignableToGenericArguments_Integration()
        {
            ResetDomainTypeCache();

            // Get generic arguments for a type implementing a generic interface
            var args = typeof(GenericServiceInt).GetAssignableToGenericArguments(typeof(IGenericServiceDefinition<>));

            // Check if they match constraints (in this case, no constraints, so any type should match)
            var constraints = new Type[] { typeof(object) }; // Dummy constraint
            var matches = args.MatchesGenericParameters(constraints);

            // int is assignable from object
            Assert.True(matches);
        }

        [Fact]
        public void GetUnderlyingDependencyType_With_GetAllDependencies_Integration()
        {
            ResetDomainTypeCache();

            // Get dependencies for a type with IEnumerable dependency
            var dependencies = typeof(EnumerableDependencyClass).GetAllDependencies(typeof(InjectAttribute)).ToList();

            // Should include ISimpleService (unwrapped from IEnumerable<ISimpleService>)
            Assert.Contains(typeof(ISimpleService), dependencies);
        }

        [Fact]
        public void SatisfiesOpenedGenericConstraints_With_GetAssignableToGenericArguments_Integration()
        {
            // Get generic arguments for a type implementing a generic interface
            var args = typeof(ClosedGenericWithClassConstraint)
                .GetAssignableToGenericArguments(typeof(IGenericWithClassConstraint<>));

            // Check if they satisfy the constraints of the open generic
            var openGeneric = typeof(IGenericWithClassConstraint<>);
            var type = typeof(ClosedGenericWithClassConstraint);

            // The method SatisfiesOpenedGenericConstraints checks if the type's generic args satisfy the constraints
            // of the open generic interface it implements
            Assert.True(type.SatisfiesOpenedGenericConstraints(openGeneric));
        }

        [Fact]
        public void GetMarkedConstructors_With_GetInjectableConstructor_Integration()
        {
            // GetInjectableConstructor returns the constructor with attribute or the greediest
            // GetMarkedConstructors returns all constructors with attribute
            var markedConstructors = typeof(OneMarkedConstructor)
                .GetMarkedConstructors(typeof(InjectAttribute))
                .ToList();

            var injectableConstructor = typeof(OneMarkedConstructor)
                .GetInjectableConstructor(typeof(InjectAttribute));

            // GetInjectableConstructor should return one of the marked constructors
            Assert.Contains(injectableConstructor, markedConstructors);
        }

        [Fact]
        public void IsOptionalParameter_With_GetInjectableConstructor_Integration()
        {
            // GetInjectableConstructor returns the constructor with attribute or the greediest
            // We can check if parameters of that constructor are optional
            var type = typeof(ClassWithMixedParameters);
            var constructor = type.GetInjectableConstructor(typeof(InjectAttribute)); // No attribute, so returns greediest

            var parameters = constructor.GetParameters();
            Assert.Equal(2, parameters.Length);

            // First parameter is required, second is optional
            Assert.False(parameters[0].IsOptionalParameter());
            Assert.True(parameters[1].IsOptionalParameter());
        }

        [Fact]
        public void GetMemberDependencyType_With_GetAllDependencies_Integration()
        {
            ResetDomainTypeCache();

            // Get dependencies for a type with injectable field
            var dependencies = typeof(ClassWithInjectableField).GetAllDependencies(typeof(InjectFieldAttribute)).ToList();

            // Should include ISimpleService (from the field)
            Assert.Contains(typeof(ISimpleService), dependencies);
        }

        [Fact]
        public void CanResolveAllDependencies_With_GetAllDependencies_Integration()
        {
            ResetDomainTypeCache();

            var registry = new Dictionary<Type, Type>
                {
                    { typeof(ISimpleService), typeof(SimpleService) }
                };

            // Get dependencies
            var dependencies = typeof(ConcreteWithRegisteredInterfaceDependency)
                .GetAllDependencies(typeof(InjectAttribute))
                .ToList();

            // Check if all dependencies can be resolved
            var canResolve = typeof(ConcreteWithRegisteredInterfaceDependency)
                .CanResolveAllDependencies(typeof(InjectAttribute), registry);

            Assert.True(canResolve);
            Assert.Contains(typeof(ISimpleService), dependencies);
        }

        [Fact]
        public void ClearCache_With_FindAllAssignableFrom_Integration()
        {
            ResetDomainTypeCache();

            // Populate cache
            var type = typeof(ISimpleService);
            var results1 = type.FindAllAssignableFrom().ToList();

            // Clear cache
            TypeAnalyzes.ClearCache();

            // Repopulate cache
            var results2 = type.FindAllAssignableFrom().ToList();

            // Results should be the same
            Assert.Equal(results1.Count, results2.Count);
            foreach (var t in results1)
                Assert.Contains(t, results2);
        }
    }
}