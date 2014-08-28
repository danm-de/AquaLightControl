AquaLightControl
================

A client-/server application for RaspberryPi(R) to control LED light stripes. It was designed to simulate daylight changes (fade-in/fade-out) in a fish tank.

It uses [Raspberry#](http://www.raspberry-sharp.org/) to access the GPIO pins
and the SPI bus for communication. It currently supports Adafruit's [12-Channel 16-bit PWM LED Driver](http://www.adafruit.com/products/1455) module only.

The server software implements an easy to use RESTful API for configuration and status information. You can configure it by using a simple Windows WPF GUI application.

AquaLightControl.Contracts
--------------------------
Contains all DTOs used for communication as DataContracts.

AquaLightControl.Math
---------------------
Mathematic helper methods and contracts.

AquaLightControl.Service
------------------------
Server application that runs on the RaspberryPi(R) using [Mono](http://www.mono-project.com/).

AquaLightControl.ClientApi
--------------------------
Simple HTTP/REST API to access the AquaLightControl.Service application.

AquaLightControl.Gui
--------------------
Windows GUI application written in C#/WPF.
