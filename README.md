# Dragonfly.Umbraco10.Forms #

A collection of custom Umbraco Forms FieldTypes and Helpers created by [Heather Floyd](https://www.HeatherFloyd.com).

See the article "[Displaying Results After an Umbraco Form Submission](https://heatherfloyd.com/blog/posts/displaying-results-after-an-umbraco-form-submission/)" for an explanation of usage (of the version 7 package).

## Installation ##
[![Nuget Downloads](https://buildstats.info/nuget/Dragonfly.Umbraco10.Forms)](https://www.nuget.org/packages/Dragonfly.Umbraco10.Forms/)

    PM > Install-Package Dragonfly.Umbraco10.Forms


**NOTE:** This project was ported from the [v7 version](https://github.com/hfloyd/Dragonfly.Umbraco7Forms). Please [report any issues](https://github.com/hfloyd/Dragonfly.Umbraco10.Forms/issues) you experience.


## Usage ##

There are several new Forms Field Types which will be available after installation.

There is also a model called `FormWithRecords` which helps you to work with a form and all its data. You will need to inject `IFormService` & `IRecordReaderService`

    using System;
    using System.Collections.Generic;
    using Dragonfly.UmbracoForms.Helpers;
    using Dragonfly.UmbracoForms.Models;
    using Dragonfly.UmbracoForms.Services;
    using Umbraco.Forms.Core.Services;
	using Umbraco.Forms.Core.Data.Storage;

	public class FormDemo
    {
        private IFormService _FormService;
        private IRecordReaderService _FormRecordReaderService;
		private IRecordStorage _RecordStorage;
        private NetPromoterHelperService _NetPromoterHelperService;

		public FormDemo(string FormGuidString, IFormService FormService, IRecordReaderService FormRecordReaderService, IRecordStorage RecordStorage)
        {
            _FormService = FormService;
            _FormRecordReaderService = FormRecordReaderService;
			_RecordStorage = RecordStorage;
            _NetPromoterHelperService = new NetPromoterHelperService(_FormService, _FormRecordReaderService, _RecordStorage);

            Guid formGuid;
            var validGuid = Guid.TryParse(FormGuidString, out formGuid);

            var formWithRecords = new FormWithRecords(formGuid, _FormService, _FormRecordReaderService, _RecordStorage);
            
            var netPromoterOverallScore = _NetPromoterHelperService.GetNetPromoterScore(formWithRecords, "RecommendThisThing");
            
            foreach (var record in formWithRecords.RecordsAll())
            {
				//Do stuff with records...
			}
		}
	}

## Resources ##

GitHub : https://github.com/hfloyd/Dragonfly.Umbraco10.Forms


