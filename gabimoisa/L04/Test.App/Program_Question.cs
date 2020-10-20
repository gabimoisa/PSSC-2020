using Question.Domain.CreateQuestionWorkflow;
using System;
using System.Collections.Generic;
using System.Net;
using LanguageExt;
using static Question.Domain.CreateQuestionWorkflow.CreateQuestionResult;

namespace Test.App
{
    class Program_Question
    {
        static void Main(string[] args)
        {
            var cmd = new CreateQuestionCmd("Question", "Description", "C#");
            var result = CreateNewQuestion(cmd);

            var createQuestionEvent = result.Match(ProcessQuetionPosted,ProcessQuestionNotPosted,ProcessInvalidQuestion);

            Console.ReadLine();
        }

        private static ICreateQuestionResult ProcessInvalidQuestion(QuestionValidationFailed validationErrors)
        {
            Console.WriteLine("Question validation failed: ");
            foreach (var error in validationErrors.ValidationErrors)
            {
                Console.WriteLine(error);
            }
            return validationErrors;
        }

        private static ICreateQuestionResult ProcessQuestionNotPosted(QuestionNotPosted questionNotPosted)
        {
            Console.WriteLine($"Question not posted: {questionNotPosted.Reason}");
            return questionNotPosted;
        }

        private static ICreateQuestionResult ProcessQuetionPosted(QuestionPosted question)
        {
            Console.WriteLine($"Question: {question.QuestionId}");
            Console.WriteLine($"Description: {question.Description}");
            return question;
        }

        public static ICreateQuestionResult CreateNewQuestion(CreateQuestionCmd createQuestion)
        {
            if (string.IsNullOrWhiteSpace(createQuestion.Description))
            {
                var errors = new List<string>() { "Your question" };
                return new QuestionValidationFailed(errors);
            }

            if(string.IsNullOrEmpty(createQuestion.Title))
            {
                return new QuestionNotPosted("Give a title");
            }

            var questionId = Guid.NewGuid();
            var result = new QuestionPosted(questionId, createQuestion.Description); 

            return result;
        }
    }
}
