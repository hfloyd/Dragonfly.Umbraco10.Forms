namespace Dragonfly.UmbracoForms.FieldTypes
{
    using System;
    using System.Collections.Generic;
    using Umbraco.Forms.Core;
    using Umbraco.Forms.Core.Attributes;
    using Umbraco.Forms.Core.Enums;

    public class LikertItem : FieldType
    {
        [Setting("Maximum Scale Number",
            Description = "Scale will be 1 to Max Number (Default is 5)",
            Alias = "MaxScaleNum",
            View = "Textfield")]
        public string MaxScaleNumString { get; set; }

        public const int MaxScaleNumDefault = 5;

        public int MaxScaleNum
        {
            get
            {
                int maxScaleNum = MaxScaleNumDefault;
                var isNum = int.TryParse(MaxScaleNumString, out maxScaleNum);

                return maxScaleNum;
            }
        }

        [Setting("Include 'N/A' Option?",
            Description = "(It will return a value of 0)",
            Alias = "IncludeNaOption",
            View = "CheckBox")]
        public string IncludeNaOptionString { get; set; }

        public bool IncludeNaOption
        {
            get
            {
                if (IncludeNaOptionString == "True")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        [Setting("'N/A' Option Text",
            Description = "Default if unspecified is 'N/A'",
            Alias = "NaOptionText",
            View = "Textfield")]
        public string NaOptionText { get; set; }

        public const string NaOptionDefaultText = "N/A";

        public LikertItem()
        {
            // Mandatory
            Id = new Guid("5CF57666-565D-46B8-AB88-55CB9E2A5E01");
            Name = "Likert Rating Item";
            Description = "Renders a single Likert Rating input";
            Icon = "icon-bars";
            DataType = FieldDataType.Integer;
            FieldTypeViewName = "FieldType.LikertItem.cshtml";

            // Optional      
            Category = "Custom Types";

#if NET6_0
	        HideField = false;
#endif
            
            HideLabel = false;
            SortOrder = 10;
           // SupportsPrevalues = false;
            SupportsRegex = false;
        }

        public override string GetDesignView() =>
            "~/App_Plugins/Dragonfly.UmbracoForms/Backoffice/Common/FieldTypes/LikertItem.html";

        public Dictionary<int, string> RatingOptions()
        {
            var ratingOptions = new Dictionary<int, string>();

            for (int i = 1; i < MaxScaleNum + 1; i++)
            {
                ratingOptions.Add(i, i.ToString());
            }

            if (IncludeNaOption)
            {
                var naText = NaOptionText != "" ? NaOptionText : NaOptionDefaultText;
                ratingOptions.Add(0, naText);
            }

            return ratingOptions;
        }
    }
}