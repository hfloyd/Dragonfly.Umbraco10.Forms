namespace Dragonfly.UmbracoForms.Services
{
    using System;
    using System.Collections.Generic;
    using Dragonfly.UmbracoForms.Models;
    using Newtonsoft.Json;
    using Umbraco.Forms.Core.Data.Storage;
    using Umbraco.Forms.Core.Services;

    public class NetPromoterHelperService
    {
        private readonly IFormService _FormService;
        private readonly IRecordReaderService _FormRecordReaderService;
        private IRecordStorage _RecordStorage;

        public NetPromoterHelperService(
            IFormService FormService,
            IRecordReaderService FormRecordReaderService,
            IRecordStorage RecordStorage
        )
        {
            _FormService = FormService;
            _FormRecordReaderService = FormRecordReaderService;
            _RecordStorage = RecordStorage;
        }

        public static NetPromoterRating RatingFromJson(string RatingJson)
        {
            NetPromoterRating npRating = JsonConvert.DeserializeObject<NetPromoterRating>(RatingJson);

            return npRating;
        }

        public IEnumerable<NetPromoterRating> RatingsfromFormRecords(string FormGuid, string NpsFieldAlias)
        {
            Guid formGuid;
            var validGuid = Guid.TryParse(FormGuid, out formGuid);
            var formData = new FormWithRecords(formGuid, _FormService, _FormRecordReaderService, _RecordStorage);
            return RatingsfromFormRecords(formData, NpsFieldAlias);
        }

        public IEnumerable<NetPromoterRating> RatingsfromFormRecords(FormWithRecords FormData, string NpsFieldAlias)
        {
            var npsDataRaw = FormData.AllFieldData(NpsFieldAlias);
            var npsDataSet = new List<NetPromoterRating>();

            foreach (var datapoint in npsDataRaw)
            {
                npsDataSet.Add(RatingFromJson(datapoint.Value.ValuesAsString()));
            }

            return npsDataSet;
        }

        public NetPromoterScore GetNetPromoterScore(IEnumerable<NetPromoterRating> Ratings)
        {
            return new NetPromoterScore(Ratings);
        }

        public NetPromoterScore GetNetPromoterScore(FormWithRecords FormData, string NpsFieldAlias)
        {
            var ratings = RatingsfromFormRecords(FormData, NpsFieldAlias);
            return new NetPromoterScore(ratings);
        }

        public NetPromoterScore GetNetPromoterScore(string FormGuid, string NpsFieldAlias)
        {
            var ratings = RatingsfromFormRecords(FormGuid, NpsFieldAlias);
            return new NetPromoterScore(ratings);
        }
    }
}
