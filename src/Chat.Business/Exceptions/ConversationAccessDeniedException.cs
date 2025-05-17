using System;

namespace Chat.Business.Exceptions;

public class ConversationAccessDeniedException : Exception
{
    public ConversationAccessDeniedException()
        : base("You don't have access to this conversation") { }
}
