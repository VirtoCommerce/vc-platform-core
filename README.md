# VirtoCommerce Platform Core - COMING SOON!
List of changes:

Tecnology stack
- ASP.NET Core 2.1.6
- EF Core 2.1.4
- ASP.NET Core Identity 2.1.6
- OpenIddict 2.0.0
- WebPack

Platform
- The new platform solution structure
  - Security
    - Completely migrate authentification and authorization to the default ASP.NET Identity without any extension
    - OpenIddict server to support  all OAuth flows also used for token based authorization 
    - Removed Hmac and simple key authorization for call platform API
Modules
- the new Notifications module
- Removed VirtoCommerce.Domain project and nuget package (now each module defines self domain model in Core project)
TODO check list:

![8ea72ae0c3d511e7a1325bdfb85b1215 map](https://user-images.githubusercontent.com/7566324/32503635-68fa4a8c-c3e6-11e7-910a-88af3fec87e1.png)


# License
Copyright (c) Virto Solutions LTD.  All rights reserved.

Licensed under the Virto Commerce Open Software License (the "License"); you
may not use this file except in compliance with the License. You may
obtain a copy of the License at

http://virtocommerce.com/opensourcelicense

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or
implied.
