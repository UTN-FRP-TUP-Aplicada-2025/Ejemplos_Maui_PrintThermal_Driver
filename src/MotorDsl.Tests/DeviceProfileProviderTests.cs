using MotorDsl.Core.Contracts;
using MotorDsl.Core.Models;
using MotorDsl.Core.Providers;

namespace MotorDsl.Tests;

/// <summary>
/// Tests for IDeviceProfileProvider contract (v2.0) and InMemoryDeviceProfileProvider implementation.
/// Sprint 05 — TK-29
/// 
/// Interface contract (read-only):
///   DeviceProfile? GetProfile(string name)
///   IEnumerable&lt;DeviceProfile&gt; GetAll()
/// 
/// InMemory adds: void Add(DeviceProfile profile)
/// 
/// Note: replaces the v1.0 IDeviceProfileProvider which only had GetProfile(string)
/// returning non-nullable and throwing KeyNotFoundException.
/// </summary>
public class DeviceProfileProviderTests
{
    // ── GetProfile ───────────────────────────────────────────────

    [Fact]
    public void GetProfile_RegisteredName_ReturnsProfile()
    {
        var provider = new InMemoryDeviceProfileProvider();
        var profile = new DeviceProfile("58mm", 32, "escpos");
        provider.Add(profile);

        var result = provider.GetProfile("58mm");

        Assert.NotNull(result);
        Assert.Equal("58mm", result.Name);
        Assert.Equal(32, result.Width);
        Assert.Equal("escpos", result.RenderTarget);
    }

    [Fact]
    public void GetProfile_UnknownName_ReturnsNull()
    {
        var provider = new InMemoryDeviceProfileProvider();

        var result = provider.GetProfile("no-existe");

        Assert.Null(result);
    }

    [Fact]
    public void GetProfile_EmptyProvider_ReturnsNull()
    {
        var provider = new InMemoryDeviceProfileProvider();

        var result = provider.GetProfile("cualquier-nombre");

        Assert.Null(result);
    }

    // ── GetAll ───────────────────────────────────────────────────

    [Fact]
    public void GetAll_WithProfiles_ReturnsAll()
    {
        var provider = new InMemoryDeviceProfileProvider();
        provider.Add(new DeviceProfile("58mm", 32, "escpos"));
        provider.Add(new DeviceProfile("80mm", 48, "escpos"));

        var all = provider.GetAll().ToList();

        Assert.Equal(2, all.Count);
        Assert.Contains(all, p => p.Name == "58mm");
        Assert.Contains(all, p => p.Name == "80mm");
    }

    [Fact]
    public void GetAll_EmptyProvider_ReturnsEmpty()
    {
        var provider = new InMemoryDeviceProfileProvider();

        var all = provider.GetAll().ToList();

        Assert.Empty(all);
    }

    // ── Add (upsert behavior) ────────────────────────────────────

    [Fact]
    public void Add_SameNameTwice_OverwritesPreviousProfile()
    {
        var provider = new InMemoryDeviceProfileProvider();
        provider.Add(new DeviceProfile("58mm", 32, "escpos"));
        provider.Add(new DeviceProfile("58mm", 36, "text"));

        var result = provider.GetProfile("58mm");

        Assert.NotNull(result);
        Assert.Equal(36, result.Width);
        Assert.Equal("text", result.RenderTarget);
    }

    [Fact]
    public void Add_SameNameTwice_DoesNotDuplicateInGetAll()
    {
        var provider = new InMemoryDeviceProfileProvider();
        provider.Add(new DeviceProfile("58mm", 32, "escpos"));
        provider.Add(new DeviceProfile("58mm", 36, "text"));

        var all = provider.GetAll().ToList();

        Assert.Single(all);
    }

    // ── Interface compliance ─────────────────────────────────────

    [Fact]
    public void InMemoryDeviceProfileProvider_ImplementsIDeviceProfileProvider()
    {
        var provider = new InMemoryDeviceProfileProvider();

        Assert.IsAssignableFrom<IDeviceProfileProvider>(provider);
    }
}
