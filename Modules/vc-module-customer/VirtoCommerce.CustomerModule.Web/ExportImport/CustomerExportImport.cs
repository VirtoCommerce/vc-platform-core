using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using VirtoCommerce.CustomerModule.Data.Common;
using VirtoCommerce.Domain.Customer.Model;
using VirtoCommerce.Domain.Customer.Services;
using VirtoCommerce.Platform.Core.ExportImport;
using VirtoCommerce.Platform.Core.Settings;

namespace VirtoCommerce.CustomerModule.Web.ExportImport
{
    public sealed class CustomerExportImport
    {
        private readonly IMemberService _memberService;
        private readonly IMemberSearchService _memberSearchService;
        private readonly ISettingsManager _settingsManager;
        private readonly JsonSerializer _serializer;

        private int? _batchSize;

        public CustomerExportImport(
            IMemberService memberService,
            IMemberSearchService memberSearchService,
            ISettingsManager settingsManager
            )
        {
            _memberService = memberService;
            _memberSearchService = memberSearchService;
            _settingsManager = settingsManager;
            _serializer = new JsonSerializer { TypeNameHandling = TypeNameHandling.All };
        }

        private int BatchSize
        {
            get
            {
                if (_batchSize == null)
                {
                    _batchSize = _settingsManager.GetValue("Customer.ExportImport.PageSize", 50);
                }
                return (int)_batchSize;
            }
        }

        public void DoExport(Stream backupStream, Action<ExportImportProgressInfo> progressCallback)
        {
            var progressInfo = new ExportImportProgressInfo { Description = "loading data..." };
            progressCallback(progressInfo);

            using (var sw = new StreamWriter(backupStream, Encoding.UTF8))
            using (var writer = new JsonTextWriter(sw))
            {
                writer.WriteStartObject();

                progressInfo.Description = "Members exporting...";
                progressCallback(progressInfo);

                var memberCount = _memberSearchService.SearchMembers(new MembersSearchCriteria { Take = 0, DeepSearch = true }).TotalCount;
                writer.WritePropertyName("MembersTotalCount");
                writer.WriteValue(memberCount);

                writer.WritePropertyName("Members");
                writer.WriteStartArray();

                for (var i = 0; i < memberCount; i += BatchSize)
                {
                    var searchResponse = _memberSearchService.SearchMembers(new MembersSearchCriteria { Skip = i, Take = BatchSize, DeepSearch = true });
                    foreach (var member in searchResponse.Results)
                    {
                        _serializer.Serialize(writer, member);
                    }
                    writer.Flush();
                    progressInfo.Description = $"{ Math.Min(memberCount, i + BatchSize) } of { memberCount } members exported";
                    progressCallback(progressInfo);
                }
                writer.WriteEndArray();

                writer.WriteEndObject();
                writer.Flush();
            }
        }

        public void DoImport(Stream backupStream, Action<ExportImportProgressInfo> progressCallback)
        {
            var progressInfo = new ExportImportProgressInfo();
            var membersTotalCount = 0;

            using (var streamReader = new StreamReader(backupStream))
            using (var reader = new JsonTextReader(streamReader))
            {
                while (reader.Read())
                {
                    if (reader.TokenType == JsonToken.PropertyName)
                    {
                        if (reader.Value.ToString() == "MembersTotalCount")
                        {
                            membersTotalCount = reader.ReadAsInt32() ?? 0;
                        }
                        else if (reader.Value.ToString() == "Members")
                        {
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
                                //Need to import by topological sort order, because Organizations have a graph structure and here references integrity must be preserved 
                                var organizations = members.OfType<Organization>();
                                var nodes = new HashSet<string>(organizations.Select(x => x.Id));
                                var edges = new HashSet<Tuple<string, string>>(organizations.Where(x => !string.IsNullOrEmpty(x.ParentId) && x.Id != x.ParentId).Select(x => new Tuple<string, string>(x.Id, x.ParentId)));
                                var orgsTopologicalSortedList = TopologicalSort.Sort(nodes, edges);
                                members = members.OrderByDescending(x => orgsTopologicalSortedList.IndexOf(x.Id)).ToList();

                                for (int i = 0; i < membersCount; i += BatchSize)
                                {
                                    _memberService.SaveChanges(members.Skip(i).Take(BatchSize).ToArray());

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
