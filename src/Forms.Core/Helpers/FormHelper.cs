namespace Dragonfly.UmbracoForms.Helpers
{
    using System;
    using System.Linq;
    using Umbraco.Forms.Core.Interfaces;
    using Umbraco.Forms.Core.Persistence.Dtos;

    public static class FormHelper
    {
        public static string GetStringFieldValue(IRecord Record, string FieldAlias)
        {
            string val = "";
            var isValid = TryGetStringFieldValue(Record, FieldAlias, out val);

            return val;
        }

        public static bool TryGetStringFieldValue(IRecord Record, string FieldAlias, out string StringValue)
        {
            try
            {
                var fieldValues = Record.RecordFields.Values.Where(n => n.Alias == FieldAlias).ToList();
                if (fieldValues.Any())
                {
                    StringValue = fieldValues.FirstOrDefault().ValuesAsString();
                    return true;
                }
                else
                {
                    var msg = $"ERROR on record # {Record.Id} - No field with alias '{FieldAlias}' found.";
                    StringValue = "";
                    return false;
                    //LogHelper.Warn<string>(msg);
                }
            }
            catch (Exception e)
            {
                var msg = $"ERROR on record # {Record.Id} - Field conversion for '{FieldAlias}'";
                StringValue = "";
                return false;
                //LogHelper.Error<string>(msg, e);
            }
        }

        public static int GetIntFieldValue(IRecord Record, string FieldAlias)
        {
            int val = 0;
            var isValid = TryGetIntFieldValue(Record, FieldAlias, out val);

            return val;
        }

        public static bool TryGetIntFieldValue(IRecord Record, string FieldAlias, out int IntegerValue)
        {
            try
            {
                var fieldValues = Record.RecordFields.Values.Where(n => n.Alias == FieldAlias).ToList();
                if (fieldValues.Any())
                {
                    var intString = fieldValues.FirstOrDefault().ValuesAsString();
                    var isNumeric = int.TryParse(intString, out IntegerValue);

                    if (!isNumeric)
                    {
                        var msg =
                            $"ERROR on record # {Record.Id} - Field '{FieldAlias}' with value '{intString}' could not be converted to an integer.";
                        IntegerValue = 0;
                        return false;
                        //LogHelper.Warn<int>(msg);
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    var msg = $"ERROR on record # {Record.Id} - No field with alias '{FieldAlias}' found.";
                    IntegerValue = 0;
                    return false;
                    //LogHelper.Warn<int>(msg);
                }
            }
            catch (Exception e)
            {
                var msg = $"ERROR on record # {Record.Id} - Field conversion for '{FieldAlias}'";
                IntegerValue = 0;
                return false;
                //LogHelper.Error<int>(msg, e);
            }

        }

    }
}