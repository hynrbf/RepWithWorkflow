using Aspose.Words.Replacing;
using Aspose.Words;

namespace Common
{
    // Reference : https://docs.aspose.com/words/net/find-and-replace/#customize-find-and-replace-operation
    public class ReplaceWithHtmlEvaluator : IReplacingCallback
    {
        //This simplistic method will only work well when the match starts at the beginning of a run. 
        ReplaceAction IReplacingCallback.Replacing(ReplacingArgs args)
        {
            var builder = new DocumentBuilder((Document)args.MatchNode.Document);
            builder.MoveTo(args.MatchNode);
            builder.InsertHtml(args.Replacement);
            args.Replacement = "";
            return ReplaceAction.Replace;
        }
    }
}
