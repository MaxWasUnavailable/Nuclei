using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Nuclei.Abstractions.Nuclei;

namespace Nuclei.Tests.Core.Services;

public sealed class DependsOnAttributeTests
{
    [Test]
    public void OrderServices_WithMultiLevelDependencies_OrdersInDependencySequence()
    {
        var services = new INucleiService[]
        {
            new ServiceA(),
            new ServiceB(),
            new ServiceC()
        };

        var ordered = DependsOnAttribute.OrderServices(services);

        IndexOf(ordered, typeof(ServiceC)).Should().BeLessThan(IndexOf(ordered, typeof(ServiceB)));
        IndexOf(ordered, typeof(ServiceB)).Should().BeLessThan(IndexOf(ordered, typeof(ServiceA)));
    }

    [Test]
    public void OrderServices_WithDiamondDependencies_OrdersInPartialDependencySequence()
    {
        var services = new INucleiService[]
        {
            new ServiceD(),
            new ServiceB2(),
            new ServiceC2(),
            new ServiceA2()
        };

        var ordered = DependsOnAttribute.OrderServices(services);

        var indexA = IndexOf(ordered, typeof(ServiceA2));
        var indexB = IndexOf(ordered, typeof(ServiceB2));
        var indexC = IndexOf(ordered, typeof(ServiceC2));
        var indexD = IndexOf(ordered, typeof(ServiceD));

        indexA.Should().BeLessThan(indexB);
        indexA.Should().BeLessThan(indexC);
        indexB.Should().BeLessThan(indexD);
        indexC.Should().BeLessThan(indexD);
    }

    [Test]
    public void OrderServices_WithMultipleDependencies_OrdersAfterAllDependencies()
    {
        var services = new INucleiService[]
        {
            new ServiceX(),
            new ServiceY(),
            new ServiceZ()
        };

        var ordered = DependsOnAttribute.OrderServices(services);

        IndexOf(ordered, typeof(ServiceX)).Should().BeLessThan(IndexOf(ordered, typeof(ServiceZ)));
        IndexOf(ordered, typeof(ServiceY)).Should().BeLessThan(IndexOf(ordered, typeof(ServiceZ)));
    }

    [Test]
    public void OrderServices_WithMissingDependency_Throws()
    {
        var services = new INucleiService[]
        {
            new ServiceMissingDependency()
        };

        var act = () => DependsOnAttribute.OrderServices(services);

        act.Should().Throw<InvalidOperationException>();
    }

    [Test]
    public void OrderServices_WithDependencyCycle_Throws()
    {
        var services = new INucleiService[]
        {
            new ServiceCycleA(),
            new ServiceCycleB()
        };

        var act = () => DependsOnAttribute.OrderServices(services);

        act.Should().Throw<InvalidOperationException>();
    }

    [Test]
    public void OrderServices_WithNoDependencies_KeepsAllServices()
    {
        var services = new INucleiService[]
        {
            new ServiceIndependentA(),
            new ServiceIndependentB()
        };

        var ordered = DependsOnAttribute.OrderServices(services);

        ordered.Should().HaveCount(2);
        ordered.Select(service => service.GetType())
            .Should().BeEquivalentTo([typeof(ServiceIndependentA), typeof(ServiceIndependentB)]);
    }

    private static int IndexOf(IReadOnlyList<INucleiService> ordered, Type type)
    {
        for (var i = 0; i < ordered.Count; i++)
            if (ordered[i].GetType() == type)
                return i;

        throw new InvalidOperationException($"Service type '{type.FullName}' not found in ordered list.");
    }

    [DependsOn(typeof(ServiceB))]
    private sealed class ServiceA : INucleiService
    {
        public void Initialize(INucleiContext context) { }
        public void Dispose() { }
    }

    [DependsOn(typeof(ServiceC))]
    private sealed class ServiceB : INucleiService
    {
        public void Initialize(INucleiContext context) { }
        public void Dispose() { }
    }

    private sealed class ServiceC : INucleiService
    {
        public void Initialize(INucleiContext context) { }
        public void Dispose() { }
    }

    private sealed class ServiceA2 : INucleiService
    {
        public void Initialize(INucleiContext context) { }
        public void Dispose() { }
    }

    [DependsOn(typeof(ServiceA2))]
    private sealed class ServiceB2 : INucleiService
    {
        public void Initialize(INucleiContext context) { }
        public void Dispose() { }
    }

    [DependsOn(typeof(ServiceA2))]
    private sealed class ServiceC2 : INucleiService
    {
        public void Initialize(INucleiContext context) { }
        public void Dispose() { }
    }

    [DependsOn(typeof(ServiceB2), typeof(ServiceC2))]
    private sealed class ServiceD : INucleiService
    {
        public void Initialize(INucleiContext context) { }
        public void Dispose() { }
    }

    private sealed class ServiceX : INucleiService
    {
        public void Initialize(INucleiContext context) { }
        public void Dispose() { }
    }

    private sealed class ServiceY : INucleiService
    {
        public void Initialize(INucleiContext context) { }
        public void Dispose() { }
    }

    [DependsOn(typeof(ServiceX), typeof(ServiceY))]
    private sealed class ServiceZ : INucleiService
    {
        public void Initialize(INucleiContext context) { }
        public void Dispose() { }
    }

    [DependsOn(typeof(ServiceNotRegistered))]
    private sealed class ServiceMissingDependency : INucleiService
    {
        public void Initialize(INucleiContext context) { }
        public void Dispose() { }
    }

    private sealed class ServiceNotRegistered : INucleiService
    {
        public void Initialize(INucleiContext context) { }
        public void Dispose() { }
    }

    [DependsOn(typeof(ServiceCycleB))]
    private sealed class ServiceCycleA : INucleiService
    {
        public void Initialize(INucleiContext context) { }
        public void Dispose() { }
    }

    [DependsOn(typeof(ServiceCycleA))]
    private sealed class ServiceCycleB : INucleiService
    {
        public void Initialize(INucleiContext context) { }
        public void Dispose() { }
    }

    private sealed class ServiceIndependentA : INucleiService
    {
        public void Initialize(INucleiContext context) { }
        public void Dispose() { }
    }

    private sealed class ServiceIndependentB : INucleiService
    {
        public void Initialize(INucleiContext context) { }
        public void Dispose() { }
    }
}

