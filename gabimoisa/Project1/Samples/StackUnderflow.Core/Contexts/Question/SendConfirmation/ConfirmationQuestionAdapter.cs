﻿using Access.Primitives.Extensions.ObjectExtensions;
using Access.Primitives.IO;
using Access.Primitives.IO.Mocking;
using StackUnderflow.EF.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using static StackUnderflow.Domain.Core.Contexts.Question.SendConfirmation.ConfirmationQuestionResult;

namespace StackUnderflow.Domain.Core.Contexts.Question.SendConfirmation
{
    public partial class ConfirmationQuestionAdapter: Adapter<ConfirmationQuestionCmd, IConfirmationQuestionResult, QuestionWriteContext, QuestionDependencies>
    {
        private readonly IExecutionContext _ex;

        public ConfirmationQuestionAdapter(IExecutionContext ex)
        {
            _ex = ex;
        }

        public override async Task<IConfirmationQuestionResult> Work(ConfirmationQuestionCmd command, QuestionWriteContext state, QuestionDependencies dependencies)
        {
            var wf = from isValid in command.TryValidate()
                     from user in command.QuestionUser.ToTryAsync()
                     let letter = GenerateConfirmationLetter(user)
                     from confirmationAck in dependencies.SendConfirmationEmail(letter)
                     select (user, confirmationAck);

            return await wf.Match(
                Succ: r => new QuestionConfirmed(r.user, r.confirmationAck.Receipt),
                Fail: ex => (IConfirmationQuestionResult)new InvalidRequest(ex.ToString()));
        }
        private ConfirmationLetter GenerateConfirmationLetter(User user)
        {
            var link = $"https://stackunderflow/Question986";
            var letter = $@"Dear {user.DisplayName} The question is posted. Please click on {link}";
            return new ConfirmationLetter(user.Email, letter, new Uri(link));
        }
        public override Task PostConditions(ConfirmationQuestionCmd cmd, IConfirmationQuestionResult result, QuestionWriteContext state)
        {
            return Task.CompletedTask;
        }
    }
}
