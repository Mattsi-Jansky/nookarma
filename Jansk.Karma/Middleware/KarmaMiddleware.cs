using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Jansk.Karma.Models;
using Jansk.Karma.Plugins;
using Noobot.Core.MessagingPipeline.Middleware;
using Noobot.Core.MessagingPipeline.Middleware.ValidHandles;
using Noobot.Core.MessagingPipeline.Request;
using Noobot.Core.MessagingPipeline.Response;

namespace Jansk.Karma.Middleware
{
        public class KarmaMiddleware : MiddlewareBase
    {

        private KarmaRepositoryPlugin _karmaRepositoryPlugin;
        private KarmaPlugin _karmaPlugin;
        
        public KarmaMiddleware(IMiddleware next, KarmaRepositoryPlugin karmaRepositoryPlugin, KarmaPlugin karmaPlugin) : base(next)
        {
            _karmaRepositoryPlugin = karmaRepositoryPlugin;
            _karmaPlugin = karmaPlugin;
            HandlerMappings = new[]
            {
                new HandlerMapping
                {
                    ValidHandles = RegexHandle.For(KarmaPlugin.OperatorRegex),
                    Description = "Allows upvoting and downvoting on things and people with `--` and `++`.",
                    EvaluatorFunc = KarmaHandler,
                    MessageShouldTargetBot = false,
                    VisibleInHelp = false
                },
                new HandlerMapping
                {
                    ValidHandles = RegexHandle.For(KarmaPlugin.ListKarmaRegex),
                    Description = "List recorded karma",
                    EvaluatorFunc = ListHandler,
                    MessageShouldTargetBot = true,
                    VisibleInHelp = true
                },
                new HandlerMapping
                {
                    ValidHandles = RegexHandle.For(KarmaPlugin.ListReasonRegex),
                    Description = "List reasons for a karma entry",
                    EvaluatorFunc = ReasonHandler,
                    MessageShouldTargetBot = true,
                    VisibleInHelp = true
                }
            };
        }
        
        private async IAsyncEnumerable<ResponseMessage> KarmaHandler(IncomingMessage message, IValidHandle matchedHandle)
        {
            var operatorMatches = _karmaPlugin.GetOperatorMatchesInMessage(message.FullText);
            var reasonMatches = _karmaPlugin.GetReasonMatchesInMessage(message.FullText);

            operatorMatches = operatorMatches.Where(x => reasonMatches.All(y => y.Index != x.Index)).ToList();
            
            foreach (Match match in operatorMatches)
            {
                var changeRequest = _karmaPlugin.ParseKarmaChange(match.Value);
                yield return HandleKarmaChange(message, changeRequest);
            }
            foreach(Match match in reasonMatches)
            {
                var changeRequest = _karmaPlugin.ParseKarmaChangeWithReason(match.Value);
                yield return HandleKarmaChange(message, changeRequest);
            }
        }

        private async IAsyncEnumerable<ResponseMessage> ListHandler(IncomingMessage message, IValidHandle matchedHandle)
        {
            foreach (var entry in _karmaRepositoryPlugin.GetTop(_karmaPlugin.ParseNumberFromEndOfRequest(message.FullText)))
            {
                yield return message.ReplyToChannel(_karmaPlugin.GenerateCurrentKarmaMessage(entry));
            }
        }

        private async IAsyncEnumerable<ResponseMessage> ReasonHandler(IncomingMessage message, IValidHandle matchedHandle)
        {
            var karmaEntryName = _karmaPlugin.ParseNameFromReasonRequest(message.FullText);
            var numberRequested = _karmaPlugin.ParseNumberFromEndOfRequest(message.FullText);

            int reasonsCount = 0;
            foreach (var entry in _karmaRepositoryPlugin.GetReasons(karmaEntryName, numberRequested))
            {
                yield return message.ReplyToChannel(_karmaPlugin.GenerateReasonMessage(entry));
                reasonsCount++;
            }

            if (reasonsCount == 0) yield return message.ReplyToChannel($"No reasons found for {karmaEntryName}");
        }
        
        private ResponseMessage HandleKarmaChange(IncomingMessage message, ChangeRequest changeRequest)
        {
            try
            {
                _karmaRepositoryPlugin.Update(changeRequest);
                var currentKarma = _karmaRepositoryPlugin.GetKarma(changeRequest.Name);
                return message.ReplyToChannel(_karmaPlugin.GenerateCurrentKarmaMessage(changeRequest, currentKarma));
            }
            catch (Exception e)
            {
                return ResponseMessage.ChannelMessage("bots",e.Message + "\n" + e.StackTrace,new List<Attachment>());
            }
        }
    }
}