using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace GraphApiBlazorServerApp.Components.ValidationComponents
{
    public class ServerInputValidator : ComponentBase
    {
        [CascadingParameter]
        EditContext CurrentEditContext { get; set; }
        private ValidationMessageStore _messageStore;
        protected override void OnInitialized()
        {
            base.OnInitialized();

            if (this.CurrentEditContext == null)
            {
                throw new InvalidOperationException($"{nameof(ServerInputValidator)} requires a cascading " +
                    $"parameter of type {nameof(EditContext)}. For example, you can use {nameof(ServerInputValidator)} " +
                    $"inside an EditForm.");
            }

            else
            {
                _messageStore = new ValidationMessageStore(this.CurrentEditContext);
                //CurrentEditContext.OnValidationRequested += (s, e) => _messageStore.Clear();
                //CurrentEditContext.OnFieldChanged += (s, e) => ValidateField(CurrentEditContext, _messageStore, e.FieldIdentifier);
                CurrentEditContext.OnFieldChanged += FieldChanged;
               
            }
        }

        private void FieldChanged(object sender, FieldChangedEventArgs args)
        {
            FieldIdentifier fieldIdentifier = args.FieldIdentifier;
            _messageStore.Clear(fieldIdentifier);
            CurrentEditContext.NotifyValidationStateChanged();
        }

        public void ValidateApiErrors(Dictionary<string, string> ErrorDictionary,object model)
        {

            _messageStore.Clear();

            foreach (var errorKeys in ErrorDictionary.Keys)
            {
                var fieldIdentifier = new FieldIdentifier(model, errorKeys);
                _messageStore.Add(fieldIdentifier, ErrorDictionary[errorKeys]);
            }

            CurrentEditContext.NotifyValidationStateChanged();
        }
    }
}
