using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Question.Domain.CreateQuestionWorkflow
{
    public struct CreateQuestionCmd
    {
        [Required]
        public string Title { get; private set; }
        [Required]
        public string Description { get; private set; }
        public string Tags { get; private set; } 

        public CreateQuestionCmd(string title, string description, string tags)
        {
            Title = title;
            Description = description;
            Tags = tags;
        }
    }
}
