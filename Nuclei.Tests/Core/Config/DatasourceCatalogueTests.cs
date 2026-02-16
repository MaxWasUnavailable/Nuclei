using System.Collections.Generic;
using FluentAssertions;
using Nuclei.Core.Config.Datasource;

namespace Nuclei.Tests.Core.Config;

public sealed class DatasourceCatalogueTests
{
    [Test]
    public void GetBinding_WithUnknownName_FallsBackToDefault()
    {
        var bindings = new Dictionary<string, DatasourceBindingConfig>
        {
            ["*"] = new("*", ["default"], ["default"])
        };

        var sources = new Dictionary<string, DatasourceConfig>
        {
            ["default"] = new("default", "sqlite://nuclei.db")
        };

        var catalogue = new DatasourceCatalogue(bindings, sources, "*");

        var result = catalogue.GetBinding("unknown");

        result.Should().Be(bindings["*"]);
    }

    [Test]
    public void GetSources_WithUnknownName_FallsBackToDefault()
    {
        var bindings = new Dictionary<string, DatasourceBindingConfig>
        {
            ["*"] = new("*", ["default"], ["default"])
        };

        var sources = new Dictionary<string, DatasourceConfig>
        {
            ["default"] = new("default", "sqlite://nuclei.db")
        };

        var catalogue = new DatasourceCatalogue(bindings, sources, "*");

        var result = catalogue.GetAllReadSourcesForBinding("unknown");

        result.Should().HaveCount(1);
        result.Should().Contain(sources["default"]);
    }
}

