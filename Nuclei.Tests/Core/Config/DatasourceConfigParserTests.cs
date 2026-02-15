using System;
using FluentAssertions;
using Nuclei.Core.Config.Datasources;

namespace Nuclei.Tests.Core.Config;

/// <summary>
///     Tests for <see cref="DatasourceConfigParser" />.
/// </summary>
public sealed class DatasourceConfigParserTests
{
    [Test]
    public void Parse_WithValidJson_ReturnsConfigs()
    {
        const string json =
            "{\n" +
            "  \"*\": {\n" +
            "    \"host\": \"sqlite://nuclei.db\",\n" +
            "    \"pooling\": true,\n" +
            "    \"timeoutMillis\": 30000,\n" +
            "    \"connectTimeoutMillis\": 5000,\n" +
            "    \"readOnly\": false,\n" +
            "    \"options\": {\n" +
            "      \"journalMode\": \"WAL\",\n" +
            "      \"foreignKeys\": true\n" +
            "    }\n" +
            "  },\n" +
            "  \"bans\": {\n" +
            "    \"host\": \"postgres://user:pass@localhost:5432/nuclei\"\n" +
            "  }\n" +
            "}";

        var result = DatasourceConfigParser.Parse(json);

        result.Datasources.Should().HaveCount(2);
        result.GetDefault().Host.Should().Be("sqlite://nuclei.db");
        result.Get("bans").Host.Should().Be("postgres://user:pass@localhost:5432/nuclei");
        result.GetDefault().Options!["journalMode"].Should().Be("WAL");
        result.GetDefault().Options!["foreignKeys"].Should().Be("true");
    }

    [Test]
    public void Parse_WithInvalidTimeout_Throws()
    {
        const string json =
            "{\n" +
            "  \"*\": {\n" +
            "    \"host\": \"sqlite://nuclei.db\",\n" +
            "    \"timeoutMillis\": 0\n" +
            "  }\n" +
            "}";

        var act = () => DatasourceConfigParser.Parse(json);

        act.Should().Throw<FormatException>();
    }

    [Test]
    public void Parse_WithNonPrimitiveOption_Throws()
    {
        const string json =
            "{\n" +
            "  \"*\": {\n" +
            "    \"host\": \"sqlite://nuclei.db\",\n" +
            "    \"options\": {\n" +
            "      \"nested\": { \"value\": 1 }\n" +
            "    }\n" +
            "  }\n" +
            "}";

        var act = () => DatasourceConfigParser.Parse(json);

        act.Should().Throw<FormatException>();
    }
}

