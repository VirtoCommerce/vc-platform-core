using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using VirtoCommerce.CoreModule.Core.Conditions;
using VirtoCommerce.MarketingModule.Core;
using VirtoCommerce.MarketingModule.Core.Services;
using VirtoCommerce.Platform.Core.Common;
using coreModel = VirtoCommerce.MarketingModule.Core.Model;

namespace VirtoCommerce.MarketingModule.Web.Controllers.Api
{
    [Route("api/marketing")]
    public class MarketingModuleDynamicContentController : Controller
    {
        private readonly IDynamicContentService _dynamicContentService;
        private readonly IMarketingDynamicContentEvaluator _dynamicContentEvaluator;
        private readonly IDynamicContentSearchService _dynamicConentSearchService;
        private readonly IMarketingExtensionManager _marketingExtensionManager;

        public MarketingModuleDynamicContentController(IDynamicContentService dynamicContentService,
            IMarketingDynamicContentEvaluator dynamicContentEvaluator, IDynamicContentSearchService dynamicConentSearchService
            , IMarketingExtensionManager marketingExtensionManager)
        {
            _dynamicContentService = dynamicContentService;
            _dynamicContentEvaluator = dynamicContentEvaluator;
            _dynamicConentSearchService = dynamicConentSearchService;
            _marketingExtensionManager = marketingExtensionManager;
        }

        /// <summary>
        /// Search content places list entries by given criteria
        /// </summary>
        /// <param name="criteria">criteria</param>
        [Authorize(ModuleConstants.Security.Permissions.Read)]
        [HttpPost]
        [Route("contentplaces/listentries/search")]
        public async Task<ActionResult<GenericSearchResult<coreModel.DynamicContentFolder>>> DynamicContentPlaceListEntriesSearch([FromBody]coreModel.DynamicContentPlaceSearchCriteria criteria)
        {
            var retVal = new GenericSearchResult<coreModel.DynamicContentListEntry>
            {
                Results = new List<coreModel.DynamicContentListEntry>()
            };

            var foldersSearchResult = await _dynamicConentSearchService.SearchFoldersAsync(new coreModel.DynamicContentFolderSearchCriteria { FolderId = criteria.FolderId, Keyword = criteria.Keyword, Take = criteria.Take, Skip = criteria.Skip, Sort = criteria.Sort });
            var folderSkip = Math.Min(foldersSearchResult.TotalCount, criteria.Skip);
            var folderTake = Math.Min(criteria.Take, Math.Max(0, foldersSearchResult.TotalCount - criteria.Skip));
            retVal.TotalCount += foldersSearchResult.TotalCount;
            retVal.Results.AddRange(foldersSearchResult.Results.Skip(folderSkip).Take(folderTake));

            criteria.Skip = criteria.Skip - folderSkip;
            criteria.Take = criteria.Take - folderTake;

            var placesSearchResult = await _dynamicConentSearchService.SearchContentPlacesAsync(criteria);
            retVal.TotalCount += placesSearchResult.TotalCount;
            retVal.Results.AddRange(placesSearchResult.Results);

            return Ok(retVal);
        }

        /// <summary>
        /// Search dynamic content places by given criteria
        /// </summary>
        /// <param name="criteria">criteria</param>
        [HttpPost]
        [Route("contentplaces/search")]
        public async Task<ActionResult<GenericSearchResult<coreModel.DynamicContentPlace>>> DynamicContentPlacesSearch([FromBody]coreModel.DynamicContentPlaceSearchCriteria criteria)
        {
            var placesSearchResult = await _dynamicConentSearchService.SearchContentPlacesAsync(criteria);
            return Ok(placesSearchResult);
        }

        /// <summary>
        /// Search content places list entries by given criteria
        /// </summary>
        /// <param name="criteria">criteria</param>
        [HttpPost]
        [Route("contentitems/listentries/search")]
        public async Task<ActionResult<GenericSearchResult<coreModel.DynamicContentListEntry>>> DynamicContentItemsEntriesSearch([FromBody]coreModel.DynamicContentItemSearchCriteria criteria)
        {
            var retVal = new GenericSearchResult<coreModel.DynamicContentListEntry>
            {
                Results = new List<coreModel.DynamicContentListEntry>()
            };

            var foldersSearchResult = await _dynamicConentSearchService.SearchFoldersAsync(new coreModel.DynamicContentFolderSearchCriteria { FolderId = criteria.FolderId, Keyword = criteria.Keyword, Take = criteria.Take, Skip = criteria.Skip, Sort = criteria.Sort });
            var folderSkip = Math.Min(foldersSearchResult.TotalCount, criteria.Skip);
            var folderTake = Math.Min(criteria.Take, Math.Max(0, foldersSearchResult.TotalCount - criteria.Skip));
            retVal.TotalCount += foldersSearchResult.TotalCount;
            retVal.Results.AddRange(foldersSearchResult.Results.Skip(folderSkip).Take(folderTake));

            criteria.Skip = criteria.Skip - folderSkip;
            criteria.Take = criteria.Take - folderTake;

            var itemsSearchResult = await _dynamicConentSearchService.SearchContentItemsAsync(criteria);
            retVal.TotalCount += itemsSearchResult.TotalCount;
            retVal.Results.AddRange(itemsSearchResult.Results);

            return Ok(retVal);
        }

        /// <summary>
        /// Search dynamic content items by given criteria
        /// </summary>
        /// <param name="criteria">criteria</param>
        [HttpPost]
        [Route("contentitems/search")]
        public async Task<ActionResult<GenericSearchResult<coreModel.DynamicContentItem>>> DynamicContentItemsSearch([FromBody]coreModel.DynamicContentItemSearchCriteria criteria)
        {
            var itemsSearchResult = await _dynamicConentSearchService.SearchContentItemsAsync(criteria);
            return Ok(itemsSearchResult);
        }

        /// <summary>
        /// Search dynamic content items by given criteria
        /// </summary>
        /// <param name="criteria">criteria</param>
        [HttpPost]
        [Route("contentpublications/search")]
        public async Task<ActionResult<GenericSearchResult<coreModel.DynamicContentPublication>>> DynamicContentPublicationsSearch([FromBody]coreModel.DynamicContentPublicationSearchCriteria criteria)
        {
            var publicationSearchResult = await _dynamicConentSearchService.SearchContentPublicationsAsync(criteria);
            return Ok(publicationSearchResult);
        }


        /// <summary>
        /// Get dynamic content for given placeholders
        /// </summary>
        [HttpPost]
        [Route("contentitems/evaluate")]
        public async Task<ActionResult<coreModel.DynamicContentItem[]>> EvaluateDynamicContent([FromBody]coreModel.DynamicContentEvaluationContext evalContext)
        {
            var items = await _dynamicContentEvaluator.EvaluateItemsAsync(evalContext);
            return Ok(items);
        }

        /// <summary>
        /// Find dynamic content item object by id
        /// </summary>
        /// <remarks>Return a single dynamic content item object </remarks>
        /// <param name="id"> content item id</param>
        [HttpGet]
        [Route("contentitems/{id}")]
        [Authorize(ModuleConstants.Security.Permissions.Read)]
        public async Task<ActionResult<coreModel.DynamicContentItem>> GetDynamicContentById(string id)
        {
            var items = await _dynamicContentService.GetContentItemsByIdsAsync(new[] { id });
            var retVal = items.FirstOrDefault();
            if (retVal != null)
            {
                return Ok(retVal);
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
        public async Task<ActionResult<coreModel.DynamicContentItem>> CreateDynamicContent([FromBody]coreModel.DynamicContentItem contentItem)
        {
            await _dynamicContentService.SaveContentItemsAsync(new[] { contentItem });
            return await GetDynamicContentById(contentItem.Id);
        }


        /// <summary>
        ///  Update a existing dynamic content item object
        /// </summary>
        /// <param name="contentItem">dynamic content object that needs to be updated in the dynamic content system</param>
        [HttpPut]
        [Route("contentitems")]
        [Authorize(ModuleConstants.Security.Permissions.Update)]
        public IActionResult UpdateDynamicContent([FromBody]coreModel.DynamicContentItem contentItem)
        {
            _dynamicContentService.SaveContentItemsAsync(new[] { contentItem });
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
        public async Task<ActionResult<coreModel.DynamicContentPlace>> GetDynamicContentPlaceById(string id)
        {
            var places = await _dynamicContentService.GetPlacesByIdsAsync(new[] { id });
            var retVal = places.FirstOrDefault();
            if (retVal != null)
            {
                return Ok(retVal);
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
        public async Task<ActionResult<coreModel.DynamicContentPlace>> CreateDynamicContentPlace([FromBody]coreModel.DynamicContentPlace contentPlace)
        {
            await _dynamicContentService.SavePlacesAsync(new[] { contentPlace });
            return await GetDynamicContentPlaceById(contentPlace.Id);
        }


        /// <summary>
        ///  Update a existing dynamic content place object
        /// </summary>
        /// <param name="contentPlace">dynamic content place object that needs to be updated in the dynamic content system</param>
        [HttpPut]
        [Route("contentplaces")]
        [Authorize(ModuleConstants.Security.Permissions.Update)]
        public async Task<IActionResult> UpdateDynamicContentPlace([FromBody]coreModel.DynamicContentPlace contentPlace)
        {
            await _dynamicContentService.SavePlacesAsync(new[] { contentPlace });
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
        public ActionResult<coreModel.DynamicContentPublication> GetNewDynamicPublication()
        {
            var retVal = new coreModel.DynamicContentPublication
            {
                ContentItems = new coreModel.DynamicContentItem[] { },
                ContentPlaces = new coreModel.DynamicContentPlace[] { },
                DynamicExpression = _marketingExtensionManager.ContentCondition,
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
        public async Task<ActionResult<coreModel.DynamicContentPublication>> GetDynamicContentPublicationById(string id)
        {
            var publications = await _dynamicContentService.GetPublicationsByIdsAsync(new[] { id });
            var retVal = publications.FirstOrDefault();
            if (retVal != null)
            {
                FillConditions(retVal);
                return Ok(retVal);
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
        public async Task<ActionResult<coreModel.DynamicContentPublication>> CreateDynamicContentPublication([FromBody]coreModel.DynamicContentPublication publication)
        {
            await _dynamicContentService.SavePublicationsAsync(new[] { publication });
            return await GetDynamicContentPublicationById(publication.Id);
        }


        /// <summary>
        ///  Update a existing dynamic content publication object
        /// </summary>
        /// <param name="publication">dynamic content publication object that needs to be updated in the dynamic content system</param>
        [HttpPut]
        [Route("contentpublications")]
        [Authorize(ModuleConstants.Security.Permissions.Update)]
        public async Task<IActionResult> UpdateDynamicContentPublication([FromBody]coreModel.DynamicContentPublication publication)
        {
            await _dynamicContentService.SavePublicationsAsync(new[] { publication });
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
        public async Task<ActionResult<coreModel.DynamicContentFolder>> GetDynamicContentFolderById(string id)
        {
            var folders = await _dynamicContentService.GetFoldersByIdsAsync(new[] { id });
            var retVal = folders.FirstOrDefault();
            if (retVal != null)
            {
                return Ok(retVal);
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
        public async Task<ActionResult<coreModel.DynamicContentFolder>> CreateDynamicContentFolder([FromBody]coreModel.DynamicContentFolder folder)
        {
            await _dynamicContentService.SaveFoldersAsync(new[] { folder });
            return await GetDynamicContentFolderById(folder.Id);
        }

        /// <summary>
        ///  Update a existing dynamic content folder
        /// </summary>
        /// <param name="folder">dynamic content folder that needs to be updated</param>
        [HttpPut]
        [Route("contentfolders")]
        [Authorize(ModuleConstants.Security.Permissions.Update)]
        public async Task<IActionResult> UpdateDynamicContentFolder([FromBody]coreModel.DynamicContentFolder folder)
        {
            await _dynamicContentService.SaveFoldersAsync(new[] { folder });
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

        private void FillConditions(coreModel.DynamicContentPublication publication)
        {
            publication.DynamicExpression = _marketingExtensionManager.ContentCondition;
            if (!string.IsNullOrEmpty(publication.PredicateVisualTreeSerialized))
            {
                publication.DynamicExpression = JsonConvert.DeserializeObject<IConditionTree>(publication.PredicateVisualTreeSerialized, new ConditionJsonConverter());
                if (_marketingExtensionManager.ContentCondition != null)
                {
                    //Copy available elements from etalon because they not persisted
                    var sourceBlocks = _marketingExtensionManager.ContentCondition.Traverse(x => x.Children);
                    var targetBlocks = publication.DynamicExpression.Traverse(x => x.Children).ToList();
                    foreach (var sourceBlock in sourceBlocks)
                    {
                        foreach (var targetBlock in targetBlocks.Where(x => x.Id == sourceBlock.Id))
                        {
                            targetBlock.AvailableChildren = sourceBlock.AvailableChildren;
                        }
                    }
                    //copy available elements from etalon
                    publication.DynamicExpression.AvailableChildren = _marketingExtensionManager.ContentCondition.AvailableChildren;
                }
            }
        }
    }
}
