# VirtoCommerce Platform Core - COMING SOON!
List of changes:

Tecnology stack
- ASP.NET Core 2.1.6
- EF Core 2.1.4
- ASP.NET Core Identity 2.1.6
- OpenIddict 2.0.0
- WebPack
- Swashbuckle.AspNetCore.SwaggerGen
- SignalR Core

**Platform**
  - Configuration
    - Use NET Core configuration paradigm (configuration providers and strongly types IOptions)
  - Solution structure
    - Split concrete implementations into projects (Modules, Assets etc)
  - DI
    - Replaced Unity DI to build-in .NET Core DI Microsoft.Extensions.DependencyInjection
  - Modularity
    - Completely reworked assembly and dependency loading into platform process
    - Changed IModule abstraction to have only two methods Initialize and PostInitialize.
    - Changed module.manifest file structure (removed settings and presmissions sections)
 - Security
    - Completely migrate authentification and authorization to the default ASP.NET Identity without any extension
    - OpenIddict server to support  all OAuth flows also used for token based authorization
    - Removed Hmac and simple key authorization for call platform API
    - Now permissions are defined only in design time in special fluent syntax
    - Added localization for permissions
    - The storefront switched to work with using barrier token authorization
 - Persistent infrastructure
    - New migrations
    - TPH inheritence model only (map hierarhy to single table)
    - DbContext now is defined separately from repository
    - Using  DbContext triggers for auditing and change logging
    - Switch to asynchronous calls of DbCOntext methods
 - Settings
    - Now settings are defined only in design time in special fluent syntax
    - Added localization for settings
    - Allow to change setting value through any  .NET Core configuration provider
 - Caching
    - Replaced CacheManager at  ASP.NET InMemory
    - Strongly typed cache regions and cache dependencies 
    - Allow to manage expiration time of cached objects and disable cache 
    - Removed special CacheModule, now caching is implemented in place where it is needed. 
 - Dynamic properties
    - Changed registration logic, now is using manual registration instead of using reflection as it was done in 2.x
 - Logging
    - Used build in .NET Core  ILog abstraction and logic instead of ICommonLogging and NLog
 - UI
    - Replaced Gulp + Bower to Webpack + npm 
     
**Modules**
- Changed module solution structure (Core project, Constants, Caching)
- Switched all DAL into asynchronous operations
- the new Notifications module (written from scratch)
- Removed **VirtoCommerce.Domain** project and nuget package (now each module defines self domain model and abstractions in Core project)
- Removed **CacheModule**
- Export/Import now is streamed for all modules

TODO check list:
- Implement cache synchronization logic between multiple platform instances use Redis cache for this purposes 
- Resource based authorization (scope bounded permissions)
- Remaining modules
    - Catalog (eliminated webmodel and improved extensibility model)
    - Marketing (rework expressions serialization design)
    - ElasticSearch
    - Azure
    - Personalization ???
    - Publishing ??? need to improve design and extensibility
    
- Migration script from 2.x -> 3.x

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
