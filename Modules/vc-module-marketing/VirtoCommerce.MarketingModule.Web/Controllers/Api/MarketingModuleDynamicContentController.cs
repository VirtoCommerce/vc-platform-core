using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VirtoCommerce.MarketingModule.Core;
using VirtoCommerce.MarketingModule.Core.Services;
using VirtoCommerce.MarketingModule.Web.Converters;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Serialization;
using coreModel = VirtoCommerce.MarketingModule.Core.Model;
using webModel = VirtoCommerce.MarketingModule.Web.Model;

namespace VirtoCommerce.MarketingModule.Web.Controllers.Api
{
    [Route("api/marketing")]
    public class MarketingModuleDynamicContentController : Controller
    {
        private readonly IMarketingExtensionManager _marketingExtensionManager;
        private readonly IDynamicContentService _dynamicContentService;
        private readonly IMarketingDynamicContentEvaluator _dynamicContentEvaluator;
        private readonly IExpressionSerializer _expressionSerializer;
        private readonly IDynamicContentSearchService _dynamicConentSearchService;

        public MarketingModuleDynamicContentController(IDynamicContentService dynamicContentService, IMarketingExtensionManager marketingExtensionManager,
            IMarketingDynamicContentEvaluator dynamicContentEvaluator, IExpressionSerializer expressionSerializer, IDynamicContentSearchService dynamicConentSearchService)
        {
            _dynamicContentService = dynamicContentService;
            _marketingExtensionManager = marketingExtensionManager;
            _dynamicContentEvaluator = dynamicContentEvaluator;
            _expressionSerializer = expressionSerializer;
            _dynamicConentSearchService = dynamicConentSearchService;
        }

        /// <summary>
        /// Search content places list entries by given criteria
        /// </summary>
        /// <param name="criteria">criteria</param>
        [Authorize(ModuleConstants.Security.Permissions.Read)]
        [HttpPost]
        [Route("contentplaces/listentries/search")]
        public async Task<IActionResult> DynamicContentPlaceListEntriesSearch([FromBody]coreModel.DynamicContentPlaceSearchCriteria criteria)
        {
            var retVal = new GenericSearchResult<webModel.DynamicContentListEntry>
            {
                Results = new List<webModel.DynamicContentListEntry>()
            };

            var foldersSearchResult = await _dynamicConentSearchService.SearchFoldersAsync(new coreModel.DynamicContentFolderSearchCriteria { FolderId = criteria.FolderId, Keyword = criteria.Keyword, Take = criteria.Take, Skip = criteria.Skip, Sort = criteria.Sort });
            var folderSkip = Math.Min(foldersSearchResult.TotalCount, criteria.Skip);
            var folderTake = Math.Min(criteria.Take, Math.Max(0, foldersSearchResult.TotalCount - criteria.Skip));
            var folders = foldersSearchResult.Results.Skip(folderSkip).Take(folderTake).Select(x => x.ToWebModel());
            retVal.TotalCount += foldersSearchResult.TotalCount;
            retVal.Results.AddRange(folders);

            criteria.Skip = criteria.Skip - folderSkip;
            criteria.Take = criteria.Take - folderTake;

            var placesSearchResult = await _dynamicConentSearchService.SearchContentPlacesAsync(criteria);
            retVal.TotalCount += placesSearchResult.TotalCount;
            retVal.Results.AddRange(placesSearchResult.Results.Select(x => x.ToWebModel()));

            return Ok(retVal);
        }

        /// <summary>
        /// Search dynamic content places by given criteria
        /// </summary>
        /// <param name="criteria">criteria</param>
        [HttpPost]
        [Route("contentplaces/search")]
        public async Task<ActionResult<GenericSearchResult<webModel.DynamicContentPlace>>> DynamicContentPlacesSearch([FromBody]coreModel.DynamicContentPlaceSearchCriteria criteria)
        {
            var retVal = new GenericSearchResult<webModel.DynamicContentPlace>();
            var placesSearchResult = await _dynamicConentSearchService.SearchContentPlacesAsync(criteria);
            retVal.TotalCount = placesSearchResult.TotalCount;
            retVal.Results = placesSearchResult.Results.Select(x => x.ToWebModel()).ToList();
            return Ok(retVal);
        }

        /// <summary>
        /// Search content places list entries by given criteria
        /// </summary>
        /// <param name="criteria">criteria</param>
        [HttpPost]
        [Route("contentitems/listentries/search")]
        public async Task<ActionResult<GenericSearchResult<webModel.DynamicContentListEntry>>> DynamicContentItemsEntriesSearch([FromBody]coreModel.DynamicContentItemSearchCriteria criteria)
        {
            var retVal = new GenericSearchResult<webModel.DynamicContentListEntry>
            {
                Results = new List<webModel.DynamicContentListEntry>()
            };

            var foldersSearchResult = await _dynamicConentSearchService.SearchFoldersAsync(new coreModel.DynamicContentFolderSearchCriteria { FolderId = criteria.FolderId, Keyword = criteria.Keyword, Take = criteria.Take, Skip = criteria.Skip, Sort = criteria.Sort });
            var folderSkip = Math.Min(foldersSearchResult.TotalCount, criteria.Skip);
            var folderTake = Math.Min(criteria.Take, Math.Max(0, foldersSearchResult.TotalCount - criteria.Skip));
            var folders = foldersSearchResult.Results.Skip(folderSkip).Take(folderTake).Select(x => x.ToWebModel());
            retVal.TotalCount += foldersSearchResult.TotalCount;
            retVal.Results.AddRange(folders);

            criteria.Skip = criteria.Skip - folderSkip;
            criteria.Take = criteria.Take - folderTake;

            var itemsSearchResult = await _dynamicConentSearchService.SearchContentItemsAsync(criteria);
            retVal.TotalCount += itemsSearchResult.TotalCount;
            retVal.Results.AddRange(itemsSearchResult.Results.Select(x => x.ToWebModel()));

            return Ok(retVal);
        }

        /// <summary>
        /// Search dynamic content items by given criteria
        /// </summary>
        /// <param name="criteria">criteria</param>
        [HttpPost]
        [Route("contentitems/search")]
        public async Task<ActionResult<GenericSearchResult<webModel.DynamicContentItem>>> DynamicContentItemsSearch([FromBody]coreModel.DynamicContentItemSearchCriteria criteria)
        {
            var retVal = new GenericSearchResult<webModel.DynamicContentItem>();
            var itemsSearchResult = await _dynamicConentSearchService.SearchContentItemsAsync(criteria);
            retVal.TotalCount = itemsSearchResult.TotalCount;
            retVal.Results = itemsSearchResult.Results.Select(x => x.ToWebModel()).ToList();
            return Ok(retVal);
        }

        /// <summary>
        /// Search dynamic content items by given criteria
        /// </summary>
        /// <param name="criteria">criteria</param>
        [HttpPost]
        [Route("contentpublications/search")]
        public async Task<ActionResult<GenericSearchResult<webModel.DynamicContentPublication>>> DynamicContentPublicationsSearch([FromBody]coreModel.DynamicContentPublicationSearchCriteria criteria)
        {
            var retVal = new GenericSearchResult<webModel.DynamicContentPublication>();
            var publicationSearchResult = await _dynamicConentSearchService.SearchContentPublicationsAsync(criteria);
            retVal.TotalCount = publicationSearchResult.TotalCount;
            retVal.Results = publicationSearchResult.Results.Select(x => x.ToWebModel()).ToList();
            return Ok(retVal);
        }


        /// <summary>
        /// Get dynamic content for given placeholders
        /// </summary>
        [HttpPost]
        [Route("contentitems/evaluate")]
        public async Task<ActionResult<webModel.DynamicContentItem[]>> EvaluateDynamicContent([FromBody]coreModel.DynamicContentEvaluationContext evalContext)
        {
            var items = await _dynamicContentEvaluator.EvaluateItemsAsync(evalContext);
            var retVal = items.Select(x => x.ToWebModel()).ToArray();
            return Ok(retVal);
        }

        /// <summary>
        /// Find dynamic content item object by id
        /// </summary>
        /// <remarks>Return a single dynamic content item object </remarks>
        /// <param name="id"> content item id</param>
        [HttpGet]
        [Route("contentitems/{id}")]
        [Authorize(ModuleConstants.Security.Permissions.Read)]
        public async Task<ActionResult<webModel.DynamicContentItem>> GetDynamicContentById(string id)
        {
            var items = await _dynamicContentService.GetContentItemsByIdsAsync(new[] { id });
            var retVal = items.FirstOrDefault();
            if (retVal != null)
            {
                return Ok(retVal.ToWebModel());
            }
            return NotFound();
        }


        /// <summary>
        /// Add new dynamic content item object to marketing system
        /// </summary>
        /// <param name="contentItem">dynamic content object that needs to be added to the dynamic content system</param>
        [HttpPost]
        [Route("contentitems")]
        [Authorize(ModuleConstants.Security.Permissions.Create)]
        public async Task<ActionResult<webModel.DynamicContentItem>> CreateDynamicContent([FromBody]webModel.DynamicContentItem contentItem)
        {
            var coreItem = contentItem.ToCoreModel();
            await _dynamicContentService.SaveContentItemsAsync(new[] { coreItem });
            return await GetDynamicContentById(coreItem.Id);
        }


        /// <summary>
        ///  Update a existing dynamic content item object
        /// </summary>
        /// <param name="contentItem">dynamic content object that needs to be updated in the dynamic content system</param>
        [HttpPut]
        [Route("contentitems")]
        [Authorize(ModuleConstants.Security.Permissions.Update)]
        public IActionResult UpdateDynamicContent([FromBody]webModel.DynamicContentItem contentItem)
        {
            var coreItem = contentItem.ToCoreModel();
            _dynamicContentService.SaveContentItemsAsync(new[] { coreItem });
            return Ok();
        }

        /// <summary>
        ///  Delete a dynamic content item objects
        /// </summary>
        /// <param name="ids">content item object ids for delete in the dynamic content system</param>
        [HttpDelete]
        [Route("contentitems")]
        [Authorize(ModuleConstants.Security.Permissions.Delete)]
        public async Task<IActionResult> DeleteDynamicContents([FromQuery] string[] ids)
        {
            await _dynamicContentService.DeleteContentItemsAsync(ids);
            return Ok();
        }


        /// <summary>
        /// Find dynamic content place object by id
        /// </summary>
        /// <remarks>Return a single dynamic content place object </remarks>
        /// <param name="id">place id</param>
        [HttpGet]
        [Route("contentplaces/{id}")]
        public async Task<ActionResult<webModel.DynamicContentPlace>> GetDynamicContentPlaceById(string id)
        {
            var places = await _dynamicContentService.GetPlacesByIdsAsync(new[] { id });
            var retVal = places.FirstOrDefault();
            if (retVal != null)
            {
                return Ok(retVal.ToWebModel());
            }
            return NotFound();
        }


        /// <summary>
        /// Add new dynamic content place object to marketing system
        /// </summary>
        /// <param name="contentPlace">dynamic content place object that needs to be added to the dynamic content system</param>
        [HttpPost]
        [Route("contentplaces")]
        [Authorize(ModuleConstants.Security.Permissions.Create)]
        public async Task<ActionResult<webModel.DynamicContentPlace>> CreateDynamicContentPlace([FromBody]webModel.DynamicContentPlace contentPlace)
        {
            var corePlace = contentPlace.ToCoreModel();
            await _dynamicContentService.SavePlacesAsync(new[] { corePlace });
            return await GetDynamicContentPlaceById(corePlace.Id);
        }


        /// <summary>
        ///  Update a existing dynamic content place object
        /// </summary>
        /// <param name="contentPlace">dynamic content place object that needs to be updated in the dynamic content system</param>
        [HttpPut]
        [Route("contentplaces")]
        [Authorize(ModuleConstants.Security.Permissions.Update)]
        public async Task<IActionResult> UpdateDynamicContentPlace([FromBody]webModel.DynamicContentPlace contentPlace)
        {
            var corePlace = contentPlace.ToCoreModel();
            await _dynamicContentService.SavePlacesAsync(new[] { corePlace });
            return Ok();
        }

        /// <summary>
        ///  Delete a dynamic content place objects
        /// </summary>
        /// <param name="ids">content place object ids for delete from dynamic content system</param>
        [HttpDelete]
        [Route("contentplaces")]
        [Authorize(ModuleConstants.Security.Permissions.Delete)]
        public async Task<IActionResult> DeleteDynamicContentPlaces([FromQuery] string[] ids)
        {
            await _dynamicContentService.DeletePlacesAsync(ids);
            return Ok();
        }

        /// <summary>
        /// Get new dynamic content publication object 
        /// </summary>
        [HttpGet]
        [Route("contentpublications/new")]
        [Authorize(ModuleConstants.Security.Permissions.Create)]
        public ActionResult<webModel.DynamicContentPublication> GetNewDynamicPublication()
        {
            var retVal = new webModel.DynamicContentPublication
            {
                ContentItems = new webModel.DynamicContentItem[] { },
                ContentPlaces = new webModel.DynamicContentPlace[] { },
                //TODO
                //DynamicExpression = _marketingExtensionManager.DynamicContentExpressionTree,
                IsActive = true
            };
            return Ok(retVal);
        }

        /// <summary>
        /// Find dynamic content publication object by id
        /// </summary>
        /// <remarks>Return a single dynamic content publication object </remarks>
        /// <param name="id">publication id</param>
        [HttpGet]
        [Route("contentpublications/{id}")]
        public async Task<ActionResult<webModel.DynamicContentPublication>> GetDynamicContentPublicationById(string id)
        {
            var publications = await _dynamicContentService.GetPublicationsByIdsAsync(new[] { id });
            var retVal = publications.FirstOrDefault();
            if (retVal != null)
            {
                return Ok(retVal.ToWebModel());
            }
            return NotFound();
        }


        /// <summary>
        /// Add new dynamic content publication object to marketing system
        /// </summary>
        /// <param name="publication">dynamic content publication object that needs to be added to the dynamic content system</param>
        [HttpPost]
        [Route("contentpublications")]
        [Authorize(ModuleConstants.Security.Permissions.Create)]
        public async Task<ActionResult<webModel.DynamicContentPublication>> CreateDynamicContentPublication([FromBody]webModel.DynamicContentPublication publication)
        {
            var corePublication = publication.ToCoreModel(_expressionSerializer);
            await _dynamicContentService.SavePublicationsAsync(new[] { corePublication });
            return await GetDynamicContentPublicationById(corePublication.Id);
        }


        /// <summary>
        ///  Update a existing dynamic content publication object
        /// </summary>
        /// <param name="publication">dynamic content publication object that needs to be updated in the dynamic content system</param>
        [HttpPut]
        [Route("contentpublications")]
        [Authorize(ModuleConstants.Security.Permissions.Update)]
        public async Task<IActionResult> UpdateDynamicContentPublication([FromBody]webModel.DynamicContentPublication publication)
        {
            var corePublication = publication.ToCoreModel(_expressionSerializer);
            await _dynamicContentService.SavePublicationsAsync(new[] { corePublication });
            return Ok();
        }

        /// <summary>
        ///  Delete a dynamic content publication objects
        /// </summary>
        /// <param name="ids">content publication object ids for delete from dynamic content system</param>
        [HttpDelete]
        [Route("contentpublications")]
        [Authorize(ModuleConstants.Security.Permissions.Delete)]
        public async Task<IActionResult> DeleteDynamicContentPublications([FromQuery] string[] ids)
        {
            await _dynamicContentService.DeletePublicationsAsync(ids);
            return Ok();
        }

        /// <summary>
        /// Find dynamic content folder by id
        /// </summary>
        /// <remarks>Return a single dynamic content folder</remarks>
        /// <param name="id">folder id</param>
        [HttpGet]
        [Route("contentfolders/{id}")]
        public async Task<ActionResult<webModel.DynamicContentFolder>> GetDynamicContentFolderById(string id)
        {
            var folders = await _dynamicContentService.GetFoldersByIdsAsync(new[] { id });
            var retVal = folders.FirstOrDefault();
            if (retVal != null)
            {
                return Ok(retVal.ToWebModel());
            }
            return NotFound();
        }


        /// <summary>
        /// Add new dynamic content folder
        /// </summary>
        /// <param name="folder">dynamic content folder that needs to be added</param>
        [HttpPost]
        [Route("contentfolders")]
        [Authorize(ModuleConstants.Security.Permissions.Create)]
        public async Task<ActionResult<webModel.DynamicContentFolder>> CreateDynamicContentFolder([FromBody]webModel.DynamicContentFolder folder)
        {
            var coreFolder = folder.ToCoreModel();
            await _dynamicContentService.SaveFoldersAsync(new[] { coreFolder });
            return await GetDynamicContentFolderById(coreFolder.Id);
        }

        /// <summary>
        ///  Update a existing dynamic content folder
        /// </summary>
        /// <param name="folder">dynamic content folder that needs to be updated</param>
        [HttpPut]
        [Route("contentfolders")]
        [Authorize(ModuleConstants.Security.Permissions.Update)]
        public async Task<IActionResult> UpdateDynamicContentFolder([FromBody]webModel.DynamicContentFolder folder)
        {
            var coreFolder = folder.ToCoreModel();
            await _dynamicContentService.SaveFoldersAsync(new[] { coreFolder });
            return Ok();
        }

        /// <summary>
        ///  Delete a dynamic content folders
        /// </summary>
        /// <param name="ids">folders ids for delete</param>
        [HttpDelete]
        [Route("contentfolders")]
        [Authorize(ModuleConstants.Security.Permissions.Delete)]
        public async Task<IActionResult> DeleteDynamicContentFolders([FromQuery] string[] ids)
        {
            await _dynamicContentService.DeleteFoldersAsync(ids);
            return Ok();
        }


    }
}
