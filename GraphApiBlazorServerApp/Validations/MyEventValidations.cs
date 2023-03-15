using FluentValidation;
using GraphApiBlazorServerApp.GraphBrokers.UserBrokers;
using GraphApiBlazorServerApp.Models;
using Microsoft.Graph;
using System.Reflection;

namespace GraphApiBlazorServerApp.Validations;

public class MyEventValidations : AbstractValidator<MyEventModel>
{
    private readonly IUserGraphBroker _userGraphBroker;

    public MyEventValidations(IUserGraphBroker userGraphBroker)
    {
        _userGraphBroker = userGraphBroker;
        // CascadeMode = CascadeMode.Continue;

        ClassLevelCascadeMode = CascadeMode.Continue;
        RuleLevelCascadeMode = CascadeMode.Stop;


        RuleFor(t => t.CalendarId)
           .NotEmpty().WithMessage("This field is required");


        RuleFor(t => t.Subject)
           .NotEmpty().WithMessage("This field is required");



        RuleFor(t => t.Content)
          .NotEmpty().WithMessage("This field is required");

        RuleFor(t => t.StartDateAndTime)
          .NotEmpty().WithMessage("This field is required");

        RuleFor(t => t.EndDateAndTime)
         .NotEmpty().WithMessage("This field is required")
         .Must((model,current) =>
         {
             if(model.StartDateAndTime > current)
             {
                 return false;
             }

             return true;
         }).WithMessage("End date cannot be less than start date");


        When(i => i.IsOnline == false, () =>
        {
            RuleFor(e => e.Location)
            .NotEmpty()
            .WithMessage("This field is required");
        });
    }

}

