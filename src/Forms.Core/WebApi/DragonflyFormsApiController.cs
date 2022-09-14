namespace Dragonfly.UmbracoForms.WebApi
{
    using System;
    using System.Text;
    using System.Net.Http;
    using Dragonfly.NetHelpers;
    using Dragonfly.UmbracoForms.Models;
    using Lucene.Net.Util;
    using Umbraco.Cms.Web.Common.Attributes;
    using Umbraco.Cms.Web.Common.Controllers;
    using Microsoft.AspNetCore.Mvc;
    using Umbraco.Forms.Core.Enums;
    using Umbraco.Forms.Core.Interfaces;
    using Umbraco.Forms.Core.Services;
    using Umbraco.Forms.Core.Data.Storage;


    // /Umbraco/Api/DragonflyFormsApi <-- UmbracoApiController
    // /Umbraco/backoffice/Api/DragonflyFormsApi <-- UmbracoAuthorizedApiController

    [IsBackOffice]
    public class DragonflyFormsApiController : UmbracoApiController
    {
        private readonly IFormService _FormService;
        private readonly IRecordReaderService _FormRecordReaderService;
        private readonly IFieldService _FieldService;
        private readonly IFieldTypeStorage _FieldTypeStorage;
        private readonly IRecordStorage _RecordStorage;

        public DragonflyFormsApiController(
            IFormService FormService,
            IRecordReaderService FormRecordReaderService,
            IFieldTypeStorage FieldTypeStorage, 
            IRecordStorage RecordStorage
            )
        {
            _FormService = FormService;
            _FormRecordReaderService = FormRecordReaderService;
            _FieldTypeStorage = FieldTypeStorage;
            _RecordStorage = RecordStorage;
        }



        /// /umbraco/backoffice/api/DragonflyFormsApi/Test
        [HttpGet]
        public bool Test()
        {
            //LogHelper.Info<DragonflyFormsApiController>("Test STARTED/ENDED");
            return true;
        }

        /// /umbraco/backoffice/api/DragonflyFormsApi/GenerateFormClass?FormGuid=xxx
        [HttpGet]
        public HttpResponseMessage GenerateFormClass(string FormGuid)
        {
            var returnSB = new StringBuilder();

            Guid formGuid;
            var validGuid = Guid.TryParse(FormGuid, out formGuid);
            var formData = new FormWithRecords(formGuid, _FormService, _FormRecordReaderService, _RecordStorage);
            var formClass = new StringBuilder();
            var recordClass = new StringBuilder();

            string formClassName = formData.Form.Name.MakeCodeSafe("", true);

            //TODO: HLF - Update with new Record and FormsHelper syntax

            formClass.AppendLine($@"
                public partial class Form{formClassName}
            {{
                public FormWithRecords FormWithRecords {{ get; internal set; }}
                public IEnumerable<Form{formClassName}Record> Records {{ get; internal set; }}

                public Form{formClassName}(string FormGuid)
                {{
                    this.FormWithRecords = new FormWithRecords(FormGuid);

                    var formRecords = new List<Form{formClassName}Record>();
                    foreach (var record in FormWithRecords.RecordsAll)
                    {{
                        var intValue = 0;
                        var intTest = false;

                        var formGuid = new Guid(FormGuid);
                        var typedRecord = new Form{formClassName}Record();

                        //Standard Record Info
                        typedRecord.RecordId = record.Id;
                        typedRecord.State = record.State;
                        typedRecord.RecordUniqueId = FormHelper.GetRecordGuid(FormGuid, record.Id);
                        typedRecord.Created = record.Created;
                        typedRecord.IP = record.IP;
                        typedRecord.MemberKey = record.MemberKey;
                        typedRecord.UmbracoPageId = record.UmbracoPageId;
                        typedRecord.Updated = record.Updated;

                        //Custom Field Values
                    ");

            recordClass.AppendLine($@"
                    public class Form{formClassName}Record
            {{
                public string IP {{get; internal set; }}
                public string RecordUniqueId {{ get; internal set; }}
                public string RecordId {{ get; internal set; }}
                public FormState? State {{ get; internal set; }}
                public object MemberKey {{ get; internal set; }}
                public int UmbracoPageId {{ get; internal set; }}
                public DateTime Created {{ get; internal set; }}
                public DateTime Updated {{ get; internal set; }}
                                           ");

            foreach (var field in formData.Form.AllFields)
            {
                string fieldAlias = field.Alias;
                var fieldType = _FieldTypeStorage.GetFieldTypeByField(field);
                if (fieldType.DataType == FieldDataType.String ||
                    fieldType.DataType == FieldDataType.LongString)
                {

                    //Add field to contructor for form class
                    formClass.AppendLine($@"
                        typedRecord.{fieldAlias} = record.GetField(""{fieldAlias}"").ValuesAsString();
                                           ");

                    //Add field as property to record class
                    recordClass.AppendLine($@"
                        public string {fieldAlias} {{ get; internal set; }}
                                           ");
                }
                else if (fieldType.DataType == FieldDataType.Integer)
                {
                    //Add field to contructor for form class
                    formClass.AppendLine($@"
                          intValue = 0;
                        intTest = Int32.TryParse(record.GetField(""{fieldAlias}"").ValuesAsString(), out intValue);
                        typedRecord.{fieldAlias} = intValue;
                            ");

                    //Add field as property to record class
                    recordClass.AppendLine($@"
                        public int {fieldAlias} {{ get; internal set; }}
                                           ");
                }
                else if (fieldType.DataType == FieldDataType.DateTime)
                {

                    //Add field to contructor for form class
                    formClass.AppendLine($@"
                        typedRecord.{fieldAlias} = DateTime.Parse(record.GetField(""{fieldAlias}"").ValuesAsString());
                                           ");

                    //Add field as property to record class
                    recordClass.AppendLine($@"
                        public DateTime {fieldAlias} {{ get; internal set; }}
                                           ");
                }
            }

            //Finalize the classes and combine
            recordClass.AppendLine($@"
                            public Form{formClassName}Record() {{ }}
            }}
                                           ");

            formClass.AppendLine($@"
                    }}
                }}
          
            }}
                                   ");

            returnSB.Append(formClass);
            returnSB.AppendLine("");
            returnSB.Append(recordClass);


            return new HttpResponseMessage()
            {
                Content = new StringContent(
                    returnSB.ToString(),
                    Encoding.UTF8,
                    "text/html"
                )
            };
        }
    }
}
