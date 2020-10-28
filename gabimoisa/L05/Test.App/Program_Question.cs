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
            int[] vote = new int[5] { -1, 1, -1, 1, 1 };
            var cmd = new CreateQuestionCmd("Question", "Description", "C#",vote);
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
            Console.Write($"Tag: ");
            foreach (var item in question.Description)
            {
                Console.Write(item.ToString() + " ");
            }
            Console.WriteLine();
            var yes = 0;
            var no = 0;
            foreach (var item in question.Vote)
            {
                if (item == 1)
                    yes++;
                else no++;
            }
            Console.Write($"Vote: yes({yes}) no({no})");
            Console.WriteLine();
            return question;
        }

        public static ICreateQuestionResult CreateNewQuestion(CreateQuestionCmd createQuestion)
        {
            if (string.IsNullOrWhiteSpace(createQuestion.Description))
            {
                var errors = new List<string>() { "Describe the question" };
                return new QuestionValidationFailed(errors);
            }
            if (createQuestion.Description.Length > 1000)
            {
                var errors = new List<string>() { "Question must have less than 1000 characters" };
                return new QuestionValidationFailed(errors);
            }
            if (createQuestion.Tags.Length < 1 || createQuestion.Tags.Length > 3)
            {
                var errors = new List<string>() { "You must have min. 1 tag or max. 3 tags" };
                return new QuestionValidationFailed(errors);
            }
            if (string.IsNullOrEmpty(createQuestion.Title))
            {
                return new QuestionNotPosted("Choose a title");
            }

            var questionId = Guid.NewGuid();
            var result = new QuestionPosted(questionId, createQuestion.Description, createQuestion.Vote);

            return result;
        }
    }
}

