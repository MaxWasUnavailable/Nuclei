using System;
using FluentAssertions;
using Nuclei.Core.Config.Datasource;

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
            "  \"bindings\": {\n" +
            "    \"*\": {\n" +
            "      \"write\": \"default\",\n" +
            "      \"read\": [\"default\", \"shared\"]\n" +
            "    },\n" +
            "    \"bans\": {\n" +
            "      \"write\": [\"bans_local\", \"bans_secondary\"],\n" +
            "      \"read\": [\"bans_local\"]\n" +
            "    }\n" +
            "  },\n" +
            "  \"sources\": {\n" +
            "    \"default\": {\n" +
            "      \"host\": \"sqlite://nuclei.db\",\n" +
            "      \"pooling\": true,\n" +
            "      \"timeoutMillis\": 30000,\n" +
            "      \"connectTimeoutMillis\": 5000,\n" +
            "      \"readOnly\": false,\n" +
            "      \"options\": {\n" +
            "        \"journalMode\": \"WAL\",\n" +
            "        \"foreignKeys\": true\n" +
            "      }\n" +
            "    },\n" +
            "    \"shared\": {\n" +
            "      \"host\": \"postgres://user:pass@localhost:5432/nuclei\"\n" +
            "    },\n" +
            "    \"bans_local\": {\n" +
            "      \"host\": \"sqlite://bans.db\"\n" +
            "    },\n" +
            "    \"bans_secondary\": {\n" +
            "      \"host\": \"postgres://user:pass@localhost:5432/bans\"\n" +
            "    }\n" +
            "  }\n" +
            "}";

        var result = DatasourceConfigParser.Parse(json);

        result.Sources.Should().HaveCount(4);
        result.Bindings.Should().HaveCount(2);
        result.GetDefaultBinding().WriteSources.Should().ContainSingle("default");
        result.GetBinding("bans").WriteSources.Should().Contain(new[] { "bans_local", "bans_secondary" });
        result.GetSource("default").Host.Should().Be("sqlite://nuclei.db");
        result.GetSource("shared").Host.Should().Be("postgres://user:pass@localhost:5432/nuclei");
        result.GetSource("default").Options!["journalMode"].Should().Be("WAL");
        result.GetSource("default").Options!["foreignKeys"].Should().Be("true");
        result.GetDefaultBinding().ReadSources.Should().Contain(new[] { "default", "shared" });
    }

    [Test]
    public void Parse_WithInvalidTimeout_Throws()
    {
        const string json =
            "{\n" +
            "  \"bindings\": {\n" +
            "    \"*\": {\n" +
            "      \"write\": \"default\"\n" +
            "    }\n" +
            "  },\n" +
            "  \"sources\": {\n" +
            "    \"default\": {\n" +
            "      \"host\": \"sqlite://nuclei.db\",\n" +
            "      \"timeoutMillis\": 0\n" +
            "    }\n" +
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
            "  \"bindings\": {\n" +
            "    \"*\": {\n" +
            "      \"write\": \"default\"\n" +
            "    }\n" +
            "  },\n" +
            "  \"sources\": {\n" +
            "    \"default\": {\n" +
            "      \"host\": \"sqlite://nuclei.db\",\n" +
            "      \"options\": {\n" +
            "        \"nested\": { \"value\": 1 }\n" +
            "      }\n" +
            "    }\n" +
            "  }\n" +
            "}";

        var act = () => DatasourceConfigParser.Parse(json);

        act.Should().Throw<FormatException>();
    }
}
