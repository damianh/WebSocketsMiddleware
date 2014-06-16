WebSockets Middleware
=====================

[![Build status](https://ci.appveyor.com/api/projects/status/ox3wa91nq1wiw57t)](https://ci.appveyor.com/project/damianh/websocketsmiddleware) [![NuGet Status](http://img.shields.io/nuget/v/WebSocketsMiddleware.svg?style=flat)](https://www.nuget.org/packages/WebSocketsMiddleware/) [![NuGet Status](http://img.shields.io/nuget/v/WebSocketsMiddleware.OwinAppBuilder.svg?style=flat)](https://www.nuget.org/packages/WebSocketsMiddleware.OwinAppBuilder/)

Middleware to help you work with websockets in an owin pipeline.

#### Installation

There are two nuget packages. The main one is pure owin and this has no dependencies.

`install-package WebSocketsMiddleware`

The second package provides integration with IAppBuilder, which is deprecated but provided here for legacy and compatability reasons.

`install-package WebSocketsMiddleware.OwinAppBuilder`

An asp.net vNext builder integration package will be forthcoming.

#### Using

See [the tests](https://github.com/damianh/WebSocketsMiddleware/blob/master/src/WebSocketsMiddleware.Tests/WebSocketsMiddlewareTests.cs) as examples of usage.

##### Developed with:

[![Resharper](http://neventstore.org/images/logo_resharper_small.gif)](http://www.jetbrains.com/resharper/)
[![TeamCity](http://neventstore.org/images/logo_teamcity_small.gif)](http://www.jetbrains.com/teamcity/)
[![dotCover](http://neventstore.org/images/logo_dotcover_small.gif)](http://www.jetbrains.com/dotcover/)
[![dotTrace](http://neventstore.org/images/logo_dottrace_small.gif)](http://www.jetbrains.com/dottrace/)
