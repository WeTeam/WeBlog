﻿using System.Text.RegularExpressions;
using Sitecore.Pipelines.ExpandInitialFieldValue;

namespace Sitecore.Modules.WeBlog.Pipelines
{
    public class TokenReplacer : ExpandInitialFieldValueProcessor
    {
        public override void Process(ExpandInitialFieldValueArgs args)
        {
            if (args.SourceField.Value.Contains("$weblogsetting"))
            {
                string originalValue = args.SourceField.Value;
                Match match = Regex.Match(args.SourceField.Value, @"\(([^)]*)\)");

                if (!string.IsNullOrEmpty(match.Value))
                {
                    string settingName = match.Value.Trim(new char[] { '(', ')' });
                    string settingsValue = Sitecore.Configuration.Settings.GetSetting(settingName);

                    if (!string.IsNullOrEmpty(settingsValue))
                    {
                        args.Result = originalValue.Replace(string.Format("$weblogsetting{0}", match.Value), settingsValue);
                    }
                }
            }
        }
    }
}