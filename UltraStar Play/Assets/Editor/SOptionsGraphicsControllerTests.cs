using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System;
using Moq;

public class SOptionsGraphicsControllerTests
{
    Mock<IOptionsService> optionsServiceMock = null;
    Mock<IUnityService> unityServiceMock = null;
    OptionButton currentButton = null;

    [OneTimeSetUp]
    void Initialize()
    {
        optionsServiceMock = new Mock<IOptionsService>();
        unityServiceMock = new Mock<IUnityService>();
        currentButton = new OptionButton();
    }

    [Test]
    public IEnumerable ShouldHandleFullScreenOnToggleLeft()
    {
        //Arrange
        SOptionsGraphicsController controller = new SOptionsGraphicsController();

        currentButton.Setting = ESetting.FullScreen;

        optionsServiceMock.Setup(x => x.GetCurrent()).Returns(currentButton);
        controller.optionsService = optionsServiceMock.Object;

        float horizontalDPadAxis = -0.5f;//left
        unityServiceMock.Setup(x => x.GetAxis("Horizontal")).Returns(horizontalDPadAxis);
        controller.unityService = unityServiceMock.Object;

        //TODO:set settingservice(manager) set initial setting value and then check if toggle changes it


        //Act
        yield return null;

        //Assert
    }
    
}
