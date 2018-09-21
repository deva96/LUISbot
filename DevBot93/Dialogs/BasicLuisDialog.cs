using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using LuisBot.Services;

namespace Microsoft.Bot.Sample.LuisBot
{
    // For more information about this template visit http://aka.ms/azurebots-csharp-luis
    [Serializable]
    public class BasicLuisDialog : LuisDialog<object>
    {
        public BasicLuisDialog() : base(new LuisService(new LuisModelAttribute(
            ConfigurationManager.AppSettings["LuisAppId"], 
            ConfigurationManager.AppSettings["LuisAPIKey"], 
            domain: ConfigurationManager.AppSettings["LuisAPIHostName"])))
        {
        }

        [LuisIntent("")]
        [LuisIntent("None")]
        public async Task None(IDialogContext context, LuisResult result)

        {
            string message = $"Sorry, I did not understand '{result.Query}'. Type 'help' if you need assistance.";

            await context.PostAsync(message);

            context.Wait(this.MessageReceived);
        }

        // Go to https://luis.ai and create a new intent, then train/publish your luis app.
        // Finally replace "Greeting" with the name of your newly created intent in the following handler
        [LuisIntent("Greetings")]
        public async Task GreetingIntent(IDialogContext context, LuisResult result)
        {
            var name = context.Activity.From.Name;
            await context.PostAsync($"Hey {name} !!How can i be your assistance?");
            context.Wait(MessageReceived);
        }

        [LuisIntent("Order-cancel")]
        public async Task CancelIntent(IDialogContext context, LuisResult result)
        {
            await this.ShowLuisResult(context, result);
        }

        [LuisIntent("Order-Shipping")]
        public async Task ShippingIntent(IDialogContext context, LuisResult result)
        {
            var message = "";
            try
            {
                if (result.Entities != null)
                {
                    if (result.Entities[0].Type == "builtin.number")
                    {
                        Dictionary<string, string> Status = new Dictionary<string, string>();
                        var OrderId = result.Entities[0].Entity;
                        Status = OrderDetails.Order(OrderId, "shipStatus");
                        message = $"Your Shipment Status is {Status["Status"]}";
                    }
                    else if (result.Entities[0].Type == "Address")
                    {
                        Dictionary<string, string> Address = new Dictionary<string, string>();
                        var OrderId = "";
                        if (result.Entities.Count > 1)
                            OrderId = result.Entities[1].Entity;
                        Address = OrderDetails.Order(OrderId, "shipStatus");
                        if (OrderId != "")
                        {
                            if (Address["Status"] == "NotYetShipped")
                            {
                                var OrderAddress = OrderDetails.Order(OrderId, "shipAddress");
                                await context.PostAsync($"Can I Change the address of {OrderId}?\nYour address\n{OrderAddress["Address"]}?");
                                context.Wait(MessageReceived);
                                await this.ShowLuisConv(context, result);
                            }
                            else if (Address["Status"] == "Order not found")
                                message = $"Sorry you didnt place an Order {OrderId}";
                            else
                                message = $"Sorry,Your order {OrderId} is already shipped";
                        }
                    }
                }
            }
            catch(Exception e)
            {
                message = "Please enter your Order no?";
            }
            await context.PostAsync(message); 
            context.Wait(MessageReceived);
        }

        private async Task ShowLuisConv(IDialogContext context, LuisResult result)
        {
            var message = result.Query;
            if (message == "yes" || message == "ok" || message == "y")
            {
                await context.PostAsync($"Enter your desired address");
                context.Wait(MessageReceived);
            }
            await context.PostAsync($"Ok What else can i do for you?");
            context.Wait(MessageReceived);
        }

        [LuisIntent("Order-Status")]
        public async Task StatusIntent(IDialogContext context, LuisResult result)
        {
            var OrderId = " ";
            Dictionary<string, string> Status = new Dictionary<string, string>();
            if (result.Entities[0].Type == "builtin.number")
            {
                OrderId = result.Entities[0].Entity;
                Status = OrderDetails.Order(OrderId, "OrderStatus");
            }
            await context.PostAsync($"{Status["Status"]}");
            context.Wait(MessageReceived);
        }

        [LuisIntent("Help")]
        public async Task HelpIntent(IDialogContext context, LuisResult result)
        {
            await this.ShowLuisResult(context, result);
        }

        [LuisIntent("End")]
        public async Task EndIntent(IDialogContext context, LuisResult result)
        {
            var name = context.Activity.From.Name;
            await context.PostAsync($"Thanks for using the bot :{name}");
            context.Wait(MessageReceived);
        }


        private async Task ShowLuisResult(IDialogContext context, LuisResult result) 
        {
            await context.PostAsync($"You have reached {result.Intents[0].Intent}. You said: {result.Query}");
            context.Wait(MessageReceived);
        }
    }
}