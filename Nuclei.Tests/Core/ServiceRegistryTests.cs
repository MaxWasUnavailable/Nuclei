using System;
using System.Collections.Generic;
using FluentAssertions;
using Nuclei.Abstractions.Nuclei;
using Nuclei.Core;

namespace Nuclei.Tests.Core;

/// <summary>
///     Tests for <see cref="ServiceRegistry" />.
/// </summary>
public sealed class ServiceRegistryTests
{
    [Test]
    public void Register_WithNullService_Throws()
    {
        var registry = new ServiceRegistry();

        var act = () => registry.Register<INucleiService>(null!);

        act.Should().Throw<ArgumentNullException>();
    }

    [Test]
    public void Register_WithDuplicateServiceType_Throws()
    {
        var registry = new ServiceRegistry();
        var service = new TestService();

        registry.Register(service);
        var act = () => registry.Register(service);

        act.Should().Throw<InvalidOperationException>();
    }

    [Test]
    public void Get_WithMissingService_Throws()
    {
        var registry = new ServiceRegistry();

        var act = () => registry.Get<TestService>();

        act.Should().Throw<KeyNotFoundException>();
    }

    [Test]
    public void TryGet_WithMissingService_ReturnsFalse()
    {
        var registry = new ServiceRegistry();

        var result = registry.TryGet<TestService>(out var service);

        result.Should().BeFalse();
        service.Should().BeNull();
    }

    [Test]
    public void Register_ThenGet_ReturnsSameInstance()
    {
        var registry = new ServiceRegistry();
        var service = new TestService();

        registry.Register(service);
        var result = registry.Get<TestService>();

        result.Should().BeSameAs(service);
    }

    [Test]
    public void Register_ThenTryGet_ReturnsSameInstance()
    {
        var registry = new ServiceRegistry();
        var service = new TestService();

        registry.Register(service);
        var found = registry.TryGet<TestService>(out var result);

        found.Should().BeTrue();
        result.Should().BeSameAs(service);
    }

    [Test]
    public void Register_WithMultipleDifferentServices_DoesNotConflict()
    {
        var registry = new ServiceRegistry();
        var service1 = new TestService();
        var service2 = new TestService2();

        registry.Register(service1);
        registry.Register(service2);

        registry.Get<TestService>().Should().BeSameAs(service1);
        registry.Get<TestService2>().Should().BeSameAs(service2);
    }

    [Test]
    public void Register_WithSharedInterfaceImplementations_DoesNotConflict()
    {
        var registry = new ServiceRegistry();
        var service1 = new SharedInterfaceService();
        var service2 = new SharedInterfaceService2();

        registry.Register(service1);
        registry.Register(service2);

        registry.Get<SharedInterfaceService>().Should().BeSameAs(service1);
        registry.Get<SharedInterfaceService2>().Should().BeSameAs(service2);
    }

    [Test]
    public void TryGet_WithSharedInterface_ReturnsFalse()
    {
        var registry = new ServiceRegistry();
        registry.Register(new SharedInterfaceService());
        registry.Register(new SharedInterfaceService2());

        var found = registry.TryGet<ISharedTestService>(out var service);

        found.Should().BeFalse();
        service.Should().BeNull();
    }

    private abstract class TestServiceBase : INucleiService
    {
        public void Dispose()
        {
        }

        public void Initialize(INucleiContext context)
        {
        }
    }

    private sealed class TestService : TestServiceBase
    {
    }

    private sealed class TestService2 : TestServiceBase
    {
    }

    private interface ISharedTestService : INucleiService
    {
    }

    private sealed class SharedInterfaceService : TestServiceBase, ISharedTestService
    {
    }

    private sealed class SharedInterfaceService2 : TestServiceBase, ISharedTestService
    {
    }
}