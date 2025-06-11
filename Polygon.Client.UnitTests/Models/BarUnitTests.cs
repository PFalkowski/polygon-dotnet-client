﻿using FluentAssertions;
using Polygon.Client.Models;

namespace Polygon.Client.UnitTests.Models;

public class BarUnitTests
{
    [Fact]
    public void Clone_Does_Not_Modify_Original()
    {
        var originalBar = new Bar
        {
            Close = 100
        };

        var clonedBar = originalBar.Clone();

        clonedBar.Close = 200;

        originalBar.Close.Should().Be(100);
    }
}
