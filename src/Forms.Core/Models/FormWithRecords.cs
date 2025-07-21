namespace Dragonfly.UmbracoForms.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Umbraco.Forms.Core.Data.Storage;
    using Umbraco.Forms.Core.Enums;
    using Umbraco.Forms.Core.Interfaces;
    using Umbraco.Forms.Core.Models;
    using Umbraco.Forms.Core.Persistence.Dtos;
    using Umbraco.Forms.Core.Services;
    using Umbraco.Cms.Core.Models;

//#if NET6_0
//    using System.Linq;
//    using Umbraco.Forms.Core.Data.Storage;
//    using Umbraco.Forms.Core.Enums;
//    using Umbraco.Forms.Core.Interfaces;
//    using Umbraco.Forms.Core.Models;
//    using Umbraco.Forms.Core.Persistence.Dtos;
//    using Umbraco.Forms.Core.Services;
//#elif NET8_0
//    using System.Linq;
//    using Umbraco.Forms.Core.Data.Storage;
//    using Umbraco.Forms.Core.Enums;
//    using Umbraco.Forms.Core.Interfaces;
//    using Umbraco.Forms.Core.Models;
//    using Umbraco.Forms.Core.Persistence.Dtos;
//    using Umbraco.Forms.Core.Services;
//#else
//	// Code common to all target frameworks
//#endif
    public class FormWithRecords
    {

        private IFormService _FormService;
        private IRecordReaderService _FormRecordReaderService;
        private IRecordStorage _RecordStorage;

        private Form? _form;
        private List<string> _formInfo = new List<string>();
        private List<Record> _recordsAll = new List<Record>();
        private List<string> _errors = new List<string>();
        private bool _isValid = false;
        private long _qtyAllRecords = 0;
        private long _qtyApprovedRecords = 0;


        #region Public Properties/Methods

        public bool IsValid => _isValid;
        public Form Form => _form;
        public IEnumerable<string> FormInfo => _formInfo;
        public IEnumerable<string> Errors => _errors;

        public long QtyAllRecords => _qtyAllRecords;

        public long QtyApprovedRecords => _qtyApprovedRecords;

        public IEnumerable<Record> RecordsAll()
        {
            return _recordsAll;
        }

        public IEnumerable<Record> RecordsApproved()
        {
            if (_recordsAll.Any())
            {
                return _recordsAll.Where(x => x.State == FormState.Approved);
            }
            else
            {
                return new List<Record>();
            }
        }

        public IEnumerable<KeyValuePair<Guid, RecordField>> AllFieldData(string FieldAlias, bool ApprovedOnly = true)
        {
            var returnData = new List<KeyValuePair<Guid, RecordField>>();

            var records = ApprovedOnly ? RecordsApproved() : RecordsAll();

            foreach (var record in records)
            {
                var match = record.RecordFields.Where(n => n.Value.Alias == FieldAlias).First();

                returnData.Add(match);
            }

            return returnData;

        }

        #endregion

        public FormWithRecords(Guid FormGuid, IFormService FormService, IRecordReaderService FormRecordReaderService, IRecordStorage RecordStorage)
        {
            SetStandardProps(FormService, FormRecordReaderService, RecordStorage);

            var form = FormService.Get(FormGuid);
            _form = form;

            if (form != null)
            {
                _isValid = true;
                SetFormProps(form);
            }
            else
            {
                _isValid = false;
                var msg = $"Form with GUID '{FormGuid}' not found.";
                _errors.Add(msg);
            }
        }

        public FormWithRecords(string FormName, IFormService FormService, IRecordReaderService FormRecordReaderService, IRecordStorage RecordStorage)
        {
            SetStandardProps(FormService, FormRecordReaderService, RecordStorage);

            var form = FormService.Get(FormName);
            _form = form;

            if (form != null)
            {
                _isValid = true;
                SetFormProps(form);
            }
            else
            {
                _isValid = false;
                var msg = $"Form named '{FormName}' not found.";
                _errors.Add(msg);
            }

        }

        private void SetStandardProps(IFormService FormService, IRecordReaderService FormRecordReaderService, IRecordStorage RecordStorage)
        {
            _FormService = FormService;
            _FormRecordReaderService = FormRecordReaderService;
            _RecordStorage = RecordStorage;
        }

        private void SetFormProps(Form FormModel)
        {
            _form = FormModel;

            //Get Records
            //  int fetchQty = Int32.MaxValue;

            //var allRecs = _RecordStorage.GetAllRecords(FormModel);
            //_qtyAllRecords = allRecs.Count;

            //if (allRecs.Any())
            //{
            //    _recordsAll = allRecs.ToList();
            //    _qtyApprovedRecords = _recordsAll.Count(x => x.State == FormState.Approved);
            //}


#if NET6_0
            var alliRecs = _FormRecordReaderService.GetRecordsFromForm(FormModel.Id, 1, int.MaxValue);
            var allRecs = new List<Record>();
            if (alliRecs.Items != null)
            {
	            foreach (var iRec in alliRecs.Items)
	            {
                    // Convert each RecordDto to Record
                    var record = new Record
                    {
                        Id = iRec.Id,
                        //FormId = iRec.FormId,
                        State = iRec.State,
                        Created = iRec.Created,
                        Updated = iRec.Updated,
                        RecordFields = iRec.RecordFields.ToDictionary(k => k.Key, v => v.Value)
                    };
                    allRecs.Add(record);
	            }
			}

            if (allRecs.Any())
            {
	            _recordsAll = allRecs.ToList();
	            _qtyApprovedRecords = _recordsAll.Count(x => x.State == FormState.Approved);
            }
#else
	        var allRecs = _FormRecordReaderService.GetRecordsFromForm(FormModel.Id, 1, int.MaxValue);
            if (allRecs.Items != null)
            {
                _recordsAll = allRecs.Items.ToList();
                _qtyApprovedRecords = _recordsAll.Count(x => x.State == FormState.Approved);
            }
#endif
			

        }

    }
}
