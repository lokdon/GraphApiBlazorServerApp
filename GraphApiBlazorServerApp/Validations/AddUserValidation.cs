using FluentValidation;
using GraphApiBlazorServerApp.GraphBrokers.UserBrokers;
using GraphApiBlazorServerApp.Models;
using Microsoft.Graph;
using System.Reflection;

namespace GraphApiBlazorServerApp.Validations;

public class AddUserValidation : AbstractValidator<AddUserModel>
{
    private readonly IUserGraphBroker _userGraphBroker;

    public AddUserValidation(IUserGraphBroker userGraphBroker)
    {
        _userGraphBroker = userGraphBroker;
        // CascadeMode = CascadeMode.Continue;

        ClassLevelCascadeMode = CascadeMode.Continue;
        RuleLevelCascadeMode = CascadeMode.Stop;


        RuleFor(t => t.FirstName)
           .NotEmpty().WithMessage("This field is required")
           .Matches(@"^([a-zA-Z]\w*)$")
           .WithMessage("only alphabets and numbers are allowed");

        RuleFor(t => t.LastName)
           .NotEmpty().WithMessage("This field is required")
           .Matches(@"^([a-zA-Z]\w*)$")
           .WithMessage("only alphabets and numbers are allowed");


        RuleFor(t => t.UserName)
          .NotEmpty().WithMessage("This field is required")
           .MustAsync(async (model, name, cancellation) =>
           {

               var result = await _userGraphBroker.CheckIfUserAlreadyExistsAsync(model.Email);
               if (result)
                   return false;

               return true;
           }).WithMessage("User Already Exists");
    }

}

