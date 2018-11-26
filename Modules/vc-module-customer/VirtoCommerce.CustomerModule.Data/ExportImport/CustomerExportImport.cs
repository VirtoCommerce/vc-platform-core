using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using VirtoCommerce.CustomerModule.Core.Model;
using VirtoCommerce.CustomerModule.Core.Model.Search;
using VirtoCommerce.CustomerModule.Core.Services;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.ExportImport;
using VirtoCommerce.Platform.Core.Settings;

namespace VirtoCommerce.CustomerModule.Data.ExportImport
{
    public sealed class CustomerExportImport : IExportSupport, IImportSupport
    {
        private readonly IMemberService _memberService;
        private readonly IMemberSearchService _memberSearchService;
        private readonly ISettingsManager _settingsManager;
        private readonly JsonSerializer _serializer;
        private const int _batchSize = 50;

        public CustomerExportImport(IMemberService memberService, IMemberSearchService memberSearchService, ISettingsManager settingsManager, IOptions<MvcJsonOptions> jsonOptions)
        {
            _memberService = memberService;
            _memberSearchService = memberSearchService;
            _settingsManager = settingsManager;
            _serializer = JsonSerializer.Create(jsonOptions.Value.SerializerSettings);
        }

        public async Task ExportAsync(Stream outStream, ExportImportOptions options, Action<ExportImportProgressInfo> progressCallback, ICancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var progressInfo = new ExportImportProgressInfo { Description = "loading data..." };
            progressCallback(progressInfo);

            using (var sw = new StreamWriter(outStream, Encoding.UTF8))
            using (var writer = new JsonTextWriter(sw))
            {
                writer.WriteStartObject();

                progressInfo.Description = "Members exporting...";
                progressCallback(progressInfo);

                var members = await _memberSearchService.SearchMembersAsync(new MembersSearchCriteria { Take = 0, DeepSearch = true });
                var memberCount = members.TotalCount;
                writer.WritePropertyName("MembersTotalCount");
                writer.WriteValue(memberCount);

                cancellationToken.ThrowIfCancellationRequested();
              
                writer.WritePropertyName("Members");
                writer.WriteStartArray();

                for (var i = 0; i < memberCount; i += _batchSize)
                {
                    var searchResponse = await _memberSearchService.SearchMembersAsync(new MembersSearchCriteria { Skip = i, Take = _batchSize, DeepSearch = true });
                    foreach (var member in searchResponse.Results)
                    {
                        _serializer.Serialize(writer, member);
                    }
                    writer.Flush();
                    progressInfo.Description = $"{ Math.Min(memberCount, i + _batchSize) } of { memberCount } members exported";
                    progressCallback(progressInfo);
                }
                writer.WriteEndArray();

                writer.WriteEndObject();
                writer.Flush();
            }
        }

        public async Task ImportAsync(Stream inputStream, ExportImportOptions options, Action<ExportImportProgressInfo> progressCallback,
            ICancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var progressInfo = new ExportImportProgressInfo();
            var membersTotalCount = 0;

            using (var streamReader = new StreamReader(inputStream))
            using (var reader = new JsonTextReader(streamReader))
            {
                while (reader.Read())
                {
                    if (reader.TokenType == JsonToken.PropertyName)
                    {
                        if (reader.Value.ToString().EqualsInvariant("MembersTotalCount"))
                        {
                            membersTotalCount = reader.ReadAsInt32() ?? 0;
                        }
                        else if (reader.Value.ToString().EqualsInvariant("Members"))
                        {
                            cancellationToken.ThrowIfCancellationRequested();
                            reader.Read();
                            if (reader.TokenType == JsonToken.StartArray)
                            {
                                reader.Read();

                                var members = new List<Member>();
                                var membersCount = 0;
                                //TODO: implement to iterative import without whole members loading
                                while (reader.TokenType != JsonToken.EndArray)
                                {
                                    var member = _serializer.Deserialize<Member>(reader);
                                    members.Add(member);
                                    membersCount++;

                                    reader.Read();
                                }

                                cancellationToken.ThrowIfCancellationRequested();
                                //Need to import by topological sort order, because Organizations have a graph structure and here references integrity must be preserved 
                                var organizations = members.OfType<Organization>();
                                var nodes = new HashSet<string>(organizations.Select(x => x.Id));
                                var edges = new HashSet<Tuple<string, string>>(organizations.Where(x => !string.IsNullOrEmpty(x.ParentId) && x.Id != x.ParentId).Select(x => new Tuple<string, string>(x.Id, x.ParentId)));
                                var orgsTopologicalSortedList = TopologicalSort.Sort(nodes, edges);
                                members = members.OrderByDescending(x => orgsTopologicalSortedList.IndexOf(x.Id)).ToList();

                                for (var i = 0; i < membersCount; i += _batchSize)
                                {
                                    await _memberService.SaveChangesAsync(members.Skip(i).Take(_batchSize).ToArray());

                                    if (membersTotalCount > 0)
                                    {
                                        progressInfo.Description = $"{ i } of { membersTotalCount } members imported";
                                    }
                                    else
                                    {
                                        progressInfo.Description = $"{ i } members imported";
                                    }
                                    progressCallback(progressInfo);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
