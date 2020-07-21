﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.Text;
using TechTalk.SpecFlow.Parser;
using TechTalk.SpecFlow.Parser.Gherkin;

namespace TechTalk.SpecFlow.VsIntegration.Implementation.LanguageService
{
    internal class GherkinTextBufferPartialParserListener : GherkinTextBufferParserListenerBase
    {
        private readonly IGherkinFileScope previousScope;
        private readonly int changeLastLine;
        private readonly int changeLineDelta;

        protected override string FeatureTitle { get { return previousScope.HeaderBlock == null ? null : previousScope.HeaderBlock.Title; } }
        protected override IEnumerable<string> FeatureTags { get { return previousScope.HeaderBlock == null ? Enumerable.Empty<string>() : previousScope.HeaderBlock.Tags; } }

        public GherkinTextBufferPartialParserListener(GherkinDialectAdapter gherkinDialectAdapter, ITextSnapshot textSnapshot, IProjectScope projectScope, IGherkinFileScope previousScope, int changeLastLine, int changeLineDelta)
            : base(gherkinDialectAdapter, textSnapshot, projectScope)
        {
            this.previousScope = previousScope;
            this.changeLastLine = changeLastLine;
            this.changeLineDelta = changeLineDelta;
        }

        protected override void OnScenarioBlockCreating(int editorLine)
        {
            base.OnScenarioBlockCreating(editorLine);

            if (editorLine > changeLastLine)
            {
                var firstUnchangedScenario = previousScope.ScenarioBlocks.FirstOrDefault(
                    prevScenario => prevScenario.GetStartLine() + changeLineDelta == editorLine);

                if (firstUnchangedScenario != null)
                    throw new PartialListeningDoneException(firstUnchangedScenario);
            }
        }

        public override void Feature(string keyword, string name, string description, GherkinBufferSpan headerSpan, GherkinBufferSpan descriptionSpan)
        {
            Error("Duplicated feature title", headerSpan.StartPosition, null);
        }

        public override void Background(string keyword, string name, string description, GherkinBufferSpan headerSpan, GherkinBufferSpan descriptionSpan)
        {
            Error("Duplicated background", headerSpan.StartPosition, null);
        }
    }
}
