using FluentValidation;
using GraphApiBlazorServerApp.GraphBrokers.UserBrokers;
using GraphApiBlazorServerApp.Models;

namespace GraphApiBlazorServerApp.Validations
{
    public class AddCalendarValidations : AbstractValidator<MyCalendarModel>
    {
        public AddCalendarValidations()
        {
            ClassLevelCascadeMode = CascadeMode.Continue;
            RuleLevelCascadeMode = CascadeMode.Stop;


            RuleFor(t => t.CalendarName)
               .NotEmpty().WithMessage("This field is required")
               .Matches(@"^([a-zA-Z]\w*)$")
               .WithMessage("only alphabets and numbers are allowed");

        }

    }
}
