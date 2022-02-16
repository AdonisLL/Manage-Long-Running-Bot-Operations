using Microsoft.Bot.Builder.Dialogs;

namespace MultiTurnPromptBot.Dialogs
{
    /// <summary>
    /// Options sent to <see cref="LongOperationPrompt"/> demonstrating how a value
    /// can be passed along with the queued activity.
    /// </summary>
    public class LongOperationPromptOptions : PromptOptions
    {
        /// <summary>
        /// This is a property sent through the Queue, and is used
        /// in the queue consumer (the Azure Function) to differentiate 
        /// between long operations chosen by the user.
        /// </summary>
        public string LongOperationOption { get; set; }
    }
}
