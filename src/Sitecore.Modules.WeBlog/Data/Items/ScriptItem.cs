using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using System;

namespace Sitecore.Modules.WeBlog.Data.Items
{
    public class ScriptItem : FileItem
    {
        public static readonly ID TemplateId = new ID("{E7FE27D8-1999-492A-99BF-53E66C2FC310}");

        public ScriptItem(Item innerItem) : base(innerItem) { }

        public static implicit operator ScriptItem(Item innerItem)
        {
            return innerItem != null ? new ScriptItem(innerItem) : null;
        }

        public static implicit operator Item(ScriptItem customItem)
        {
            return customItem != null ? customItem.InnerItem : null;
        }

        public string FallbackUrl
        {
            get
            {
                return InnerItem[ScriptItemFields.FallbackUrl];
            }
        }

        public string VerificationObject
        {
            get
            {
                return InnerItem[ScriptItemFields.VerificationObject];
            }
        }

        public static class ScriptItemFields
        {
            public const string FallbackUrl = "Fallback Url";
            public const string VerificationObject = "Verification Object";
        }

        [Obsolete("Use ScriptItemFields instead.")]
        public static class Fields
        {
            public const string FallbackUrl = "Fallback Url";
            public const string VerificationObject = "Verification Object";
        }
    }
}