namespace Byndyusoft.Byndyusoft.DotNet.Testing.Infrastructure.Tests.Extensions
{
    using System;
    using System.Collections.Generic;
    using FluentAssertions;
    using global::Byndyusoft.DotNet.Testing.Infrastructure.Extensions;
    using Moq;
    using Xunit;
    using Xunit.Sdk;

    public class ShouldTests
    {
        private readonly Mock<ICollection<TestEntity>> _mock;
        private readonly TestEntity _firstEntity;
        private readonly TestEntity _secondEntity;

        public ShouldTests()
        {
            _mock = new Mock<ICollection<TestEntity>>();

            _firstEntity = new TestEntity
            {
                Id = 4,
                Name = "Самый лучший день",
                DateTime = DateTime.UtcNow
            };

            _secondEntity = new TestEntity
            {
                Id = 34,
                Name = "Заходил вчера",
                DateTime = DateTime.Today
            };
        }

        public class TestEntity
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public DateTime DateTime { get; set; }
        }

        [Fact]
        public void BeEquivalentTo_SingleMockInvocationsAndDifferentInvocationVerified_ThrowExceptionWithDiff()
        {
            //Act
            _mock.Object.Add(_firstEntity);

            //Assert
            Action action = () => _mock.Verify(x => x.Add(Should.BeEquivalentTo(_secondEntity)));

            action.Should().Throw<XunitException>().WithMessage("Expected member *, but *");
        }

        [Fact]
        public void BeEquivalentTo_SingleMockInvocationAndItVerified_VerificationSucceed()
        {
            //Act
            _mock.Object.Add(_firstEntity);

            //Assert
            _mock.Verify(x => x.Add(Should.BeEquivalentTo(_firstEntity)));
        }

        [Fact]
        public void AnyBeEquivalentTo_TwoMockInvocationsAndDifferentInvocationVerified_ThrowExceptionWithoutDiff()
        {
            //Act
            _mock.Object.Add(_firstEntity);
            _mock.Object.Add(_firstEntity);

            //Assert
            Action action = () => _mock.Verify(x => x.Add(Should.AnyBeEquivalentTo(_secondEntity)));

            action.Should().Throw<MockException>();
        }

        [Fact]
        public void AnyBeEquivalentTo_TwoMockInvocationsAndOneOfThemVerified_VerificationSucceed()
        {
            //Act
            _mock.Object.Add(_firstEntity);
            _mock.Object.Add(_secondEntity);

            //Assert
            _mock.Verify(x => x.Add(Should.AnyBeEquivalentTo(_secondEntity)));
        }
    }
}
