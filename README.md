# Meet the new  major version of  Virto Commerce platform 3.0
Our development efforts were focused on moving to ASP.NET Core, performance, architecture improvements, further enhancements and fixing arhitectural bugs. 

## What were our objective when starting development project on VC platform v3? 

- Change primary technology stack to .NET Core for the platform application and all key modules. 
- Eliminate known technical and architecture design issues of 2.x version (Caching, Overloaded core module, Asynchronous code, Platform simplification, Extensibility, Performance, Authentication and Authorization) 
- Provide easy and clear migration from 2.x version by preserving complete backward compatibility for API and Database schema 
- The platform and 18 core modules were planned to be migrated. 

## Release status note 
    We inspire you to try and investigate the new version of the system and give us your feedback 

    This is a beta release, which hasn't been verified on a production project yet 

    We have delivered a simple migration from 2.x version by preserving complete backward compatibility for API and Database schema, while you need for additional efforts in case there are custom changes in your current 2.X system. Please follow our migration guide during the migration project. 

    We cannot guarantee the backward compatibility of current the beta version with the final 3.X release 

# These Virto Commerce Release Notes below are a subset of the larger list of changes in migration to ASP.NET Core. 
## What does Virto V3 provide to developers and architects?
- Improved extensibility and unification increase the development speed and decrease time to market. 
- Unified architecture and usage of good architecture practices leads to shorter learning curve for developers who are new to working with Virto Commerce. 

## Used technological stack 
- **ASP.NET Core 2.2.0** as base platform 
- **EF Core 2.2.0** as primary ORM
- **ASP.NET Core Identity 2.2.0** for authentification and authorization
- **OpenIddict 2.0.0** for OAuth authorization
- **WebPack** as primary design/runtime bundler and minifier
- **Swashbuckle.AspNetCore.SwaggerGen** for Swagger docs and UI
- **SignalR Core** for push notifcations
- **AngularJS 1.4** as primary framework for SPA
- **HangFire 1.6.21** for run background tasks

**Platform changes**:
  - Configuration
    - Use NET Core configuration paradigm (configuration providers and strongly types IOptions)
  - Solution structure
    - Split concrete implementations into projects (Modules, Assets etc)
  - DI
    - Replaced Unity DI to build-in .NET Core DI Microsoft.Extensions.DependencyInjection
  - Modularity
    - Completely reworked assembly and dependency loading into platform process
    - Changed IModule abstraction to have only two methods Initialize and PostInitialize.
    - Changed module.manifest file structure (removed settings and persmissions sections)
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
     
**Modules changes**:
- Changed module solution structure (Core project, Constants, Caching)
- Switched all DAL into asynchronous operations
- Export/Import now is streamed for all modules

**New modules**:
- `Notifications module` (written from scratch) key features:
    - Functionality which was spread across the system is shifted to dedicated module 
    - Manage notification availability for each store
    - Unlimited cannels types for sending notifications (Email, Sms, Social networks etc)
    - Possibility to activate/deactivate each notification individually for each store 
    - New flexible extendibility model 
    - Allows to preview a notification template with data
    - Support of LIQUID syntax for templates based on Scriban engine 
    - The new notification messaged feed allows to search and preview individual messages 
- `Tax module` key features:
    - The tax calculation functionality which was spread across the system is shifted to a dedicated module which is now responsible for tax settings and calculation 
    - The new module is a single integration point for third party software and custom extensions 
- `Shipping module` key features:
    -  The shipping costs calculation functionality which was spread across the system is shifted to a dedicated module which is now responsible for shipping methods, related settings and shipping costs calculation
    - The new module is a single integration point for third party software and custom extensions 
- `Payment module` key features:
    - The payment methods functionality and integrations which were spread across the system are shifted to a dedicated module which is now responsible for payment methods and related settings 
    - The new module is a single integration point for payment gateways integration
- `Search module` key features:
    - The new module is a single integration point for search engines integration and provides a generic UI and program components for indexed search
    
**Removed modules**: 
-  **VirtoCommerce.Domain** project and nuget package (now each module defines self domain model and abstractions in Core project)
-  **VirtoCommerce.Cache**
- **VirtoCommerce.DynamicExpressions**

**Whats next**:
- Implement cache synchronization logic between multiple platform instances use `Redis` cache for this purposes 
- Resource based authorization (scope bounded permissions)
- Remaining modules
    - ElasticSearch
    - AzureSearch
    
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
