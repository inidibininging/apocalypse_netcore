using Apocalypse.Any.Core.Behaviour;
using Apocalypse.Any.Domain.Common.Model;
using Apocalypse.Any.Domain.Common.Model.Network;
using Apocalypse.Any.Domain.Server.Model;
using Apocalypse.Any.Domain.Server.Model.Interfaces;
using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Apocalypse.Any.Infrastructure.Server.Services.Data
{
    public class ExampleDialogService : GenericInMemoryDataLayer<DialogNode>, IDialogService
    {
        public const string GenericPeopleStartDialog = nameof(GenericPeopleStartDialog);
        public const string QuestionWhoAreYou = nameof(QuestionWhoAreYou);
        public const string QuestionWhatIsThisPlace = nameof(QuestionWhatIsThisPlace);
        public const string QuestionTrade = nameof(QuestionTrade);
        public const string QuestionDropPeople = nameof(QuestionDropPeople);
        public const string QuestionTransportPeople = nameof(QuestionTransportPeople);

        public IGenericTypeFactory<ImageData> PortraitGenerator { get; }

        private void CreateDialogs()
        {
            var randomPortrait = PortraitGenerator.Create<MovementBehaviour>(new MovementBehaviour() { X = 0, Y = 0 });
            new List<DialogNode>()
            {
                new DialogNode()
                {
                    Id = GenericPeopleStartDialog,
                    Content = "Welcome stranger, can I help you?",
                    FontSize = 20,
                    DialogIdContent = new List<Tuple<string,string>>()
                    {
                        new Tuple<string, string>(QuestionWhoAreYou, "Q: Who are you?"),
                        new Tuple<string, string>(QuestionWhatIsThisPlace, "Q: What is this place?"),
                        new Tuple<string, string>(QuestionTrade, "Q: Can we trade?"),
                        new Tuple<string, string>(QuestionDropPeople, "Q: Can I drop off some people?"),
                        new Tuple<string, string>("Exit", "[Exit Dialog] No, I'm fine ")
                    },
                    Portrait = randomPortrait
                },
                new DialogNode()
                {
                    Id = QuestionWhoAreYou,
                    Content = "I am a random npc. This is some generic answer",
                    FontSize = 20,
                    DialogIdContent = new List<Tuple<string,string>>()
                    {
                        new Tuple<string, string>(GenericPeopleStartDialog,"I want to know something else"),
                    },
                    Portrait = randomPortrait
                },
                new DialogNode()
                {
                    Id =QuestionWhatIsThisPlace,
                    Content = "This is a random place in the universe",
                    FontSize = 20,
                    DialogIdContent = new List<Tuple<string,string>>()
                    {
                        new Tuple<string, string>($"{QuestionWhatIsThisPlace}_Option_1","Really? Tell me more"),
                        new Tuple<string, string>($"{QuestionWhatIsThisPlace}_Option_2","Interesting, I would like to know more"),
                        new Tuple<string, string>($"{QuestionWhatIsThisPlace}_Option_3","Sure, I don't care"),
                    },
                    Portrait = randomPortrait
                },
                new DialogNode()
                {
                    Id = QuestionTrade,
                    Content = "Unfortunately, we cannot trade. Trading is not implemented yet.",
                    FontSize = 20,
                    DialogIdContent = new List<Tuple<string,string>>()
                    {
                        new Tuple<string, string>(GenericPeopleStartDialog,"Ok, I want to know something else"),
                    },
                    Portrait = randomPortrait
                },
                new DialogNode()
                {
                    Id = QuestionDropPeople,
                    Content = "Not right now. This place is overcrowded and we don't have enough resources",
                    FontSize = 20,
                    DialogIdContent = new List<Tuple<string,string>>()
                    {
                        new Tuple<string, string>(GenericPeopleStartDialog, "Ok"),
                    },
                    Portrait = randomPortrait
                },
                new DialogNode()
                {
                    Id = $"{QuestionWhatIsThisPlace}_Option_1",
                    Content = @"Lorem ipsum dolor sit amet, consectetur adipiscing elit. 
    Sed sagittis convallis purus ac pulvinar. 
    Cras vel euismod neque. Phasellus in nunc in nunc fermentum laoreet. 
    Duis libero nisi, pretium quis lobortis eu, tristique nec leo.
    Ut nunc eros, blandit ut arcu ac, vulputate scelerisque nibh. 
    Sed luctus sapien porttitor eros ornare eleifend. Suspendisse potenti.
    Nulla facilisi. In scelerisque consectetur lectus, non tempus lorem. 
    Phasellus a tellus et erat egestas gravida. Nulla vel leo mattis, malesuada nibh sed, fermentum nisl.",
                    FontSize = 20,
                    Portrait = randomPortrait
                },
                new DialogNode()
                {
                    Id = $"{QuestionWhatIsThisPlace}_Option_2",
                    Content = "I would like to, but you are a bit skeptic",
                    FontSize = 20,
                    DialogIdContent = new List<Tuple<string,string>>()
                    {
                        new Tuple<string, string>(GenericPeopleStartDialog, "Ok"),
                    },
                    Portrait = randomPortrait
                },
                new DialogNode()
                {
                    Id = $"{QuestionWhatIsThisPlace}_Option_3",
                    Content = "Yeah, fuck off",
                    FontSize = 20,
                    Portrait = randomPortrait
                },
                new DialogNode()
                {
                    Id = "Example_SubOption_4",
                    Content = "This is example suboption 4",
                    FontSize = 20,
                    Portrait = randomPortrait
                },
                new DialogNode()
                {
                    Id = "Example_SubOption_5",
                    Content = "This is example suboption 5",
                    FontSize = 20,
                    Portrait = randomPortrait
                },
                new DialogNode()
                {
                    Id = "Example_SubOption_6",
                    Content = "This is example suboption 6",
                    FontSize = 20,
                    Portrait = randomPortrait
                },
                new DialogNode()
                {
                    Id = "Example_SubOption_7",
                    Content = "This is example suboption 7",
                    FontSize = 20,
                    Portrait = randomPortrait
                },
                new DialogNode()
                {
                    Id = "Example_SubOption_8",
                    Content = "This is example suboption 8",
                    FontSize = 20,
                    Portrait = randomPortrait
                },
                new DialogNode()
                {
                    Id = "Example_SubOption_9",
                    Content = "This is example suboption 9",
                    FontSize = 20,
                    Portrait = randomPortrait
                },
                new DialogNode()
                {
                    Id = "Example_SubOption_10",
                    Content = "This is example suboption 10",
                    FontSize = 20,
                    Portrait = randomPortrait
                },
            }
            .ForEach(dialog => Add(dialog));
        }


        public DialogNode GetDialogNode(string id) => this.AsEnumerableSafe<DialogNode>().FirstOrDefault(dialog => dialog.Id == id);
        public ExampleDialogService(IGenericTypeFactory<ImageData> portraitGenerator)
        {
            PortraitGenerator = portraitGenerator ?? throw new ArgumentNullException(nameof(portraitGenerator));
            CreateDialogs();
        }
    }
}
