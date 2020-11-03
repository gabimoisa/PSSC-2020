using LanguageExt;
using LanguageExt.Common;
using Profile.Domain.CreateProfileWorkflow;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net;
using static Profile.Domain.CreateProfileWorkflow.CreateProfileResult;
using static Profile.Domain.CreateProfileWorkflow.EmailAddress;
using static Profile.Domain.RespndQuestionWorkflow.BodyReplay;

namespace Test.App
{
    class Program
    {
        static void Main(string[] args)
        {
            var emailResult = UnverifiedEmail.Create("gabriel.moisa@gmail.com");


            emailResult.Match(
                    Succ: email =>
                    {
                        SendResetPasswordLink(email);

                        Console.WriteLine("Email address is valid.");
                        return Unit.Default;
                    },
                    Fail: ex =>
                    {
                        Console.WriteLine($"Email adress is invalid. Reason: {ex.Message}");
                        return Unit.Default;
                    }
                );


            var replayResult = Replay.CreateBody("MyRespondForQuestion");


            replayResult.Match(
                    Succ: replay =>
                    {
                        Console.WriteLine("Replay address is valid.");
                        return Unit.Default;
                    },
                    Fail: ex =>
                    {
                        Console.WriteLine($"Invalid replay address. Reason: {ex.Message}");
                        return Unit.Default;
                    }
                );


            Console.ReadLine();
        }

        private static void SendResetPasswordLink(UnverifiedEmail email)
        {
            var verifiedEmailResult = new VerifyEmailService().VerifyEmail(email);
            verifiedEmailResult.Match(
                    verifiedEmail =>
                    {
                        new RestPasswordService().SendRestPasswordLink(verifiedEmail).Wait();
                        return Unit.Default;
                    },
                    ex =>
                    {
                        Console.WriteLine("Email address could not be verified");
                        return Unit.Default;
                    }
                );
        }



    }
}
