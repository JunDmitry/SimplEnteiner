using System.Reflection;
using SimplEnteinerTests.TestTypes.Classes;
using SimplEnteinerTests.TestTypes.Interfaces;
using SimplEnteiner;

namespace SimplEnteinerTests.TypeAnalyzesTests
{
    public class GetMemberDependencyTypeTests
    {
        [Fact]
        public void Throws_ArgumentNullException_When_Member_Is_Null()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                ((MemberInfo)null).GetMemberDependencyType();
            });
        }

        [Fact]
        public void Returns_Field_Type_For_FieldInfo()
        {
            var field = typeof(ClassWithInjectableField).GetField("Field");
            var result = field.GetMemberDependencyType();

            Assert.Single(result);
            Assert.Equal(typeof(ISimpleService), result[0]);
        }

        [Fact]
        public void Returns_Property_Type_For_PropertyInfo()
        {
            var property = typeof(ClassWithInjectableProperty).GetProperty("Property");
            var result = property.GetMemberDependencyType();

            Assert.Single(result);
            Assert.Equal(typeof(ISimpleService), result[0]);
        }

        [Fact]
        public void Returns_Parameter_Types_For_MethodInfo()
        {
            var method = typeof(ClassWithInjectableMethod).GetMethod("Method");
            var result = method.GetMemberDependencyType();

            Assert.Equal(2, result.Length);
            Assert.Contains(typeof(ISimpleService), result);
            Assert.Contains(typeof(int), result);
        }

        [Fact]
        public void Returns_Empty_For_Unknown_Member_Type()
        {
            // We can't easily create a custom MemberInfo, so we test with a known type
            // that doesn't match FieldInfo, PropertyInfo, or MethodInfo
            // For this test, we'll use a type that is not a member
            var type = typeof(ISimpleService);
            var result = type.GetMemberDependencyType(); // This will return empty since Type is not a member

            Assert.Empty(result);
        }
    }
}