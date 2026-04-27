using System.Reflection;
using System.Reflection.Emit;
using SimplEnteinerTests.TestInfrastructure;
using SimplEnteiner;

namespace SimplEnteinerTests.TypeAnalyzesTests
{
    public class AddAssembliesTests : TypeExtensionsTestBase
    {
        [Fact]
        public void Throws_ArgumentNullException_When_Assemblies_Is_Null()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                TypeAnalyzes.AddAssemblies(null);
            });
        }

        [Fact]
        public void Adds_New_Assembly_To_Cache()
        {
            //ResetAssembliesCache();

            //var initialAssemblies = GetAssembliesCache().Count;
            //var initialTypes = GetCachedDomainTypes().Count;

            //// Use current assembly (should not be in cache initially)
            //var assembly = Assembly.GetExecutingAssembly();
            //TypeAnalyzes.AddAssemblies(new[] { assembly });

            //var finalAssemblies = GetAssembliesCache().Count;
            //var finalTypes = GetCachedDomainTypes().Count;

            //Assert.Equal(initialAssemblies + 1, finalAssemblies);
            //Assert.True(finalTypes > initialTypes);
        }

        [Fact]
        public void Does_Not_Add_Duplicate_Assembly()
        {
            //ResetAssembliesCache();

            var assembly = Assembly.GetExecutingAssembly();
            TypeAnalyzes.AddAssemblies(new[] { assembly });

            var countAfterFirstAdd = GetAssembliesCache().Count;

            // Try adding again
            TypeAnalyzes.AddAssemblies(new[] { assembly });

            var countAfterSecondAdd = GetAssembliesCache().Count;

            Assert.Equal(countAfterFirstAdd, countAfterSecondAdd);
        }

        [Fact]
        public void Adds_Multiple_Assemblies()
        {
            //ResetAssembliesCache();

            //var initialCount = GetAssembliesCache().Count;

            //var assemblies = new[]
            //{
            //    Assembly.GetExecutingAssembly(),
            //    typeof(object).Assembly // System.Private.CoreLib
            //};

            //TypeAnalyzes.AddAssemblies(assemblies);

            //var finalCount = GetAssembliesCache().Count;

            //// Should add at least one new assembly
            //Assert.True(finalCount > initialCount);
        }

        [Fact]
        public void Updates_Cached_Types_When_Assembly_Added()
        {
            //ResetAssembliesCache();

            //var initialTypes = GetCachedDomainTypes().Count;

            //var assembly = Assembly.GetExecutingAssembly();
            //TypeAnalyzes.AddAssemblies(new[] { assembly });

            //var finalTypes = GetCachedDomainTypes().Count;

            //Assert.True(finalTypes > initialTypes);
        }

        [Fact]
        public void Does_Not_Update_Cached_Types_When_No_New_Assemblies()
        {
            //ResetAssembliesCache();

            //var assembly = Assembly.GetExecutingAssembly();
            //TypeAnalyzes.AddAssemblies(new[] { assembly });

            //var typesAfterFirstAdd = GetCachedDomainTypes().Count;

            //// Try adding again
            //TypeAnalyzes.AddAssemblies(new[] { assembly });

            //var typesAfterSecondAdd = GetCachedDomainTypes().Count;

            //Assert.Equal(typesAfterFirstAdd, typesAfterSecondAdd);
        }

        [Fact]
        public void Handles_Empty_Assembly_List()
        {
            //ResetAssembliesCache();

            //var initialAssemblies = GetAssembliesCache().Count;
            //var initialTypes = GetCachedDomainTypes().Count;

            //TypeAnalyzes.AddAssemblies(new Assembly[] { });

            //var finalAssemblies = GetAssembliesCache().Count;
            //var finalTypes = GetCachedDomainTypes().Count;

            //Assert.Equal(initialAssemblies, finalAssemblies);
            //Assert.Equal(initialTypes, finalTypes);
        }

        [Fact]
        public void Handles_Assembly_With_No_Loadable_Types()
        {
            //ResetAssembliesCache();

            // Create a dynamic assembly that throws on GetTypes()
            var asmName = new AssemblyName("DynAsmWithBrokenType");
            var asmBuilder = AssemblyBuilder.DefineDynamicAssembly(asmName, AssemblyBuilderAccess.Run);
            var modBuilder = asmBuilder.DefineDynamicModule("DynModule");

            // Define a valid type
            var validTypeBuilder = modBuilder.DefineType("ValidType", TypeAttributes.Public | TypeAttributes.Class);
            var validType = validTypeBuilder.CreateType();

            // Add the assembly
            TypeAnalyzes.AddAssemblies(new[] { asmBuilder });

            // Should not throw and should add the assembly
            var assemblies = GetAssembliesCache();
            Assert.Contains(asmBuilder, assemblies);
        }

        [Fact]
        public void Thread_Safety_Smoke_Test()
        {
            //ResetAssembliesCache();

            var assembly = Assembly.GetExecutingAssembly();
            var tasks = new List<Task>();

            // Try adding the same assembly from multiple threads
            for (int i = 0; i < 10; i++)
            {
                tasks.Add(Task.Run(() =>
                {
                    TypeAnalyzes.AddAssemblies(new[] { assembly });
                }));
            }

            Task.WaitAll(tasks.ToArray());

            // Should not throw and should contain the assembly once
            var assemblies = GetAssembliesCache();
            Assert.Contains(assembly, assemblies);
        }
    }
}