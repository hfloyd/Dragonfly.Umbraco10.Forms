namespace Dragonfly.UmbracoForms.FieldTypes
{
    using System;
    using Umbraco.Forms.Core.Providers.FieldTypes;

    public class LongAnswerNoLabel : Textarea
    {
        public LongAnswerNoLabel()
        {
            // Mandatory
            this.Id = new Guid("FD7A8788-B06C-42E4-9738-DE22F638C982");
            this.Name = "Long Answer (No Label)";
            this.Description = "Long answer text area with hidden label";
            //this.Icon = "icon-medal";
            //this.DataType = FieldDataType.String;
            this.FieldTypeViewName = "FieldType.Textarea.cshtml";

            // Optional         
            this.Category = "Custom Types";
#if NET6_0
            HideField = false;
#endif
            this.HideLabel = true;
            this.SortOrder = 10;
            //  this.SupportsPrevalues = false;
            this.SupportsRegex = true;
        }
        public override string GetDesignView() =>
            "~/App_Plugins/Dragonfly.UmbracoForms/Backoffice/Common/FieldTypes/LongAnswerNoLabel.html";
    }
}