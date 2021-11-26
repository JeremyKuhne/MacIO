using System.Text;

namespace TestProject;

public class StreamExtensionTests
{
    [Fact]
    public void TryGetLength_ThrowsOnNull()
    {
        Stream? stream = null;
        Assert.Throws<ArgumentNullException>("stream", () => stream!.TryGetLength(out _));
    }

    [Fact]
    public void TryGetLength_GetsLength()
    {
        using MemoryStream stream = new();
        stream.SetLength(1024);
        stream.TryGetLength(out long length).Should().BeTrue();
        length.Should().Be(1024);
    }

    [Fact]
    public void ReadLines_ThrowsOnNull()
    {
        Stream? stream = null;
        Assert.Throws<ArgumentNullException>("stream", () => stream!.ReadLines(null));
    }

    [Fact]
    public void ReadLines_ASCII()
    {
        using MemoryStream stream = new(Encoding.ASCII.GetBytes("This\nis a\ntest."));
        stream.ReadLines().Should().BeEquivalentTo("This", "is a", "test.");
    }

    [Fact]
    public void ReadLines_UTF8()
    {
        using MemoryStream stream = new(Encoding.UTF8.GetBytes("This\nis a\ntest."));
        stream.ReadLines(Encoding.UTF8).Should().BeEquivalentTo("This", "is a", "test.");
    }

    [Fact]
    public void Erase_ThrowsOnNull()
    {
        Stream? stream = null;
        Assert.Throws<ArgumentNullException>("stream", () => stream!.Erase(0, 0));
    }

    [Fact]
    public void Erase_NegativeOffset_ThrowsOutOfRange()
    {
        using MemoryStream stream = new();
        Assert.Throws<ArgumentOutOfRangeException>("offset", () => stream.Erase(-1, -1));
    }

    [Fact]
    public void Erase_NegativeLength_ThrowsOutOfRange()
    {
        using MemoryStream stream = new();
        Assert.Throws<ArgumentOutOfRangeException>("length", () => stream.Erase(0, -1));
    }

    [Fact]
    public void Erase_Basic()
    {
        byte[] data = new byte[] { 0xFE, 0xED, 0xDE, 0xAD, 0xBE, 0xEF };
        byte[] buffer = data.ToArray();
        using MemoryStream stream = new(buffer);
        stream.Erase(0, 0);
        buffer.Should().BeEquivalentTo(data);

        stream.Erase(0, 1);
        buffer.Should().BeEquivalentTo(new byte[] { 0x00, 0xED, 0xDE, 0xAD, 0xBE, 0xEF });
        stream.Erase(4, 2);
        buffer.Should().BeEquivalentTo(new byte[] { 0x00, 0xED, 0xDE, 0xAD, 0x00, 0x00 });
    }

    [Fact]
    public void CopyFrom_ThrowsOnNull()
    {
        Stream? nullStream = null;
        using MemoryStream stream = new();
        Assert.Throws<ArgumentNullException>("destination", () => nullStream!.CopyFrom(nullStream!, 0, 0));
        Assert.Throws<ArgumentNullException>("source", () => stream.CopyFrom(nullStream!, 0, 0));
    }

    [Fact]
    public void CopyFrom_NegativeOffset_ThrowsOutOfRange()
    {
        using MemoryStream stream = new();
        Assert.Throws<ArgumentOutOfRangeException>("offset", () => stream.CopyFrom(stream, -1, -1));
    }

    [Fact]
    public void CopyFrom_NegativeLength_ThrowsOutOfRange()
    {
        using MemoryStream stream = new();
        Assert.Throws<ArgumentOutOfRangeException>("length", () => stream.CopyFrom(stream, 0, -1));
    }

    [Fact]
    public void CopyFrom_InvalidBuffer_ThrowsOutOfRange()
    {
        using MemoryStream stream = new();
        Assert.Throws<ArgumentOutOfRangeException>("bufferSize", () => stream.CopyFrom(stream, 0, 0, 0));
    }

    [Fact]
    public void CopyFrom_Basic()
    {
        byte[] data = new byte[] { 0xFE, 0xED, 0xDE, 0xAD, 0xBE, 0xEF };
        byte[] buffer = data.ToArray();
        using MemoryStream source = new(buffer);
        using MemoryStream destination = new(new byte[6]);

        destination.CopyFrom(source, 0, 2);
        destination.ToArray().Should().BeEquivalentTo(new byte[] { 0xFE, 0xED, 0x00, 0x00, 0x00, 0x00 });

        destination.CopyFrom(source, 4, 2);
        destination.ToArray().Should().BeEquivalentTo(new byte[] { 0xFE, 0xED, 0xBE, 0xEF, 0x00, 0x00 });
    }
}
