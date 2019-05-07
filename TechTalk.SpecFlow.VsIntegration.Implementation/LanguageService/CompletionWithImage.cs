﻿using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.VisualStudio.Language.Intellisense;
using TechTalk.SpecFlow.VsIntegration.Implementation.Utils;

namespace TechTalk.SpecFlow.VsIntegration.Implementation.LanguageService
{
    internal class CompletionWithImage : Completion
    {
        public CompletionWithImage(string displayText, string insertionText, string description, ImageSource iconSource, string iconAutomationText) : base(displayText, insertionText, description, iconSource, iconAutomationText)
        {
        }

        public string IconDescriptor { get; set; }

        public override ImageSource IconSource
        {
            get
            {
                if (base.IconSource == null && IconDescriptor != null)
                {
                    base.IconSource = new BitmapImage(
                        new Uri(string.Format("pack://application:,,,/{1};component/resources/autocomplete-{0}.png", 
                            IconDescriptor.ToLowerInvariant(), "TechTalk.SpecFlow.VsIntegration.Implementation")));
                }

                return base.IconSource;
            }
            set
            {
                base.IconSource = value;
            }
        }

        public string InsertionTextColumnNames
        {
            get
            {
                var firstPipeIndex = InsertionText.IndexOf("|", StringComparison.Ordinal) + 1;
                if (firstPipeIndex <= 0)
                    return string.Empty;

                var lastColumnNameCharIndex = StringLiteralHelper.LastIndexOfValidChar(InsertionText, new[] { ' ', '|', '\r', '\n' }) + 1;
                var formattedColumnNames = InsertionText.Substring(firstPipeIndex, lastColumnNameCharIndex - firstPipeIndex);

                return "(" + formattedColumnNames.Replace('|', ',').Trim() + ")";
            }
        }
    }
}