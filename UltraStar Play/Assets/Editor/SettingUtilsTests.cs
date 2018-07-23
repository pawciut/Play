using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System;

public class SettingUtilsTests
{
    [Test]
    public void ShouldParseResolutionForProperFormat()
    {
        //Arrange
        int expectedWidth = 800;
        int expectedHeight = 600;
        string resolutionToBeParsed = expectedWidth + "x" + expectedHeight;
        int parsedWidth = 0;
        int parsedHeight = 0;

        //Act
        SettingUtils.ParseResolution(resolutionToBeParsed, out parsedWidth, out parsedHeight);

        //Assert
        Assert.AreEqual(expectedWidth, parsedWidth);
        Assert.AreEqual(expectedHeight, parsedHeight);
    }


    [Test]
    public void ShouldThrowExceptionForEmpty()
    {
        //Arrange
        string resolutionToBeParsed = null;
        int parsedWidth = 0;
        int parsedHeight = 0;

        //Act
        TestDelegate testDelegate = () =>
        {
            SettingUtils.ParseResolution(resolutionToBeParsed, out parsedWidth, out parsedHeight);
        };

        //Assert
        Assert.Throws<InvalidCastException>(testDelegate);
    }


    [Test]
    public void ShouldThrowExceptionForInvalidFormat()
    {
        //Arrange
        string resolutionToBeParsed = "800x600,423";
        int parsedWidth = 0;
        int parsedHeight = 0;

        //Act
        TestDelegate testDelegate = () =>
        {
            SettingUtils.ParseResolution(resolutionToBeParsed, out parsedWidth, out parsedHeight);
        };

        //Assert
        Assert.Throws<FormatException>(testDelegate);
    }
}
