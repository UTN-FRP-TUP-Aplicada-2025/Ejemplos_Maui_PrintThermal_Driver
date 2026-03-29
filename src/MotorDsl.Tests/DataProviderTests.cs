using MotorDsl.Core.Contracts;
using MotorDsl.Core.Providers;

namespace MotorDsl.Tests;

/// <summary>
/// Tests for IDataProvider contract and InMemoryDataProvider implementation.
/// Sprint 05 — TK-28
/// 
/// Interface contract (read-only):
///   IDictionary&lt;string, object&gt;? GetData(string dataKey)
/// 
/// InMemory adds: void Add(string dataKey, IDictionary&lt;string, object&gt; data)
/// </summary>
public class DataProviderTests
{
    // ── GetData ──────────────────────────────────────────────────

    [Fact]
    public void GetData_RegisteredKey_ReturnsDictionary()
    {
        var provider = new InMemoryDataProvider();
        var data = new Dictionary<string, object>
        {
            ["empresa"] = "Mi Tienda",
            ["total"] = 1500.00
        };
        provider.Add("venta-001", data);

        var result = provider.GetData("venta-001");

        Assert.NotNull(result);
        Assert.Equal("Mi Tienda", result["empresa"]);
        Assert.Equal(1500.00, result["total"]);
    }

    [Fact]
    public void GetData_UnknownKey_ReturnsNull()
    {
        var provider = new InMemoryDataProvider();

        var result = provider.GetData("no-existe");

        Assert.Null(result);
    }

    [Fact]
    public void GetData_EmptyProvider_ReturnsNull()
    {
        var provider = new InMemoryDataProvider();

        var result = provider.GetData("cualquier-key");

        Assert.Null(result);
    }

    // ── Add (upsert behavior) ────────────────────────────────────

    [Fact]
    public void Add_SameKeyTwice_OverwritesPreviousData()
    {
        var provider = new InMemoryDataProvider();
        var data1 = new Dictionary<string, object> { ["total"] = 100.00 };
        var data2 = new Dictionary<string, object> { ["total"] = 200.00 };
        provider.Add("venta", data1);
        provider.Add("venta", data2);

        var result = provider.GetData("venta");

        Assert.NotNull(result);
        Assert.Equal(200.00, result["total"]);
    }

    [Fact]
    public void GetData_ReturnedDictionary_ContainsAllKeys()
    {
        var provider = new InMemoryDataProvider();
        var data = new Dictionary<string, object>
        {
            ["empresa"] = "Tienda",
            ["items"] = new List<string> { "Café", "Medialunas" },
            ["total"] = 240.00
        };
        provider.Add("venta-002", data);

        var result = provider.GetData("venta-002");

        Assert.NotNull(result);
        Assert.Equal(3, result.Count);
        Assert.True(result.ContainsKey("empresa"));
        Assert.True(result.ContainsKey("items"));
        Assert.True(result.ContainsKey("total"));
    }

    // ── Interface compliance ─────────────────────────────────────

    [Fact]
    public void InMemoryDataProvider_ImplementsIDataProvider()
    {
        var provider = new InMemoryDataProvider();

        Assert.IsAssignableFrom<IDataProvider>(provider);
    }
}
