using Apocalypse.Any.Domain.Client.Model;
using Apocalypse.Any.Domain.Common.Model;
using Apocalypse.Any.Domain.Common.Model.Network;
using Apocalypse.Any.Domain.Server.Model;
using Apocalypse.Any.Infrastructure.Common.Services.Serializer.JsonAdapter;
using Apocalypse.Any.Infrastructure.Common.Services.Serializer.MsgPackAdapter;
using Apocalypse.Any.Infrastructure.Common.Services.Serializer.YamlAdapter;
using Apocalypse.Any.Infrastructure.Server.Worker;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using Terminal.Gui;

namespace Apocalypse.Any.WorkerBee
{
	public enum Page
    {
		Players,
		Enemies,
		Items,
		Projectiles,
		Props
    }
    class Program
    {
		private static Page CurrentPage;
        static void Main(string[] args)
        {
            var yamler = new YamlSerializerAdapter();
            var config = yamler.DeserializeObject<GameClientConfiguration>(File.ReadAllText(args[0]));

            var dataLayerWorker = new DataLayerWorker<
                                        PlayerSpaceship,
                                        EnemySpaceship,
                                        Item,
                                        Projectile,
                                        CharacterEntity,
                                        CharacterEntity,
                                        ImageData>(config);

            Application.Init();
			var top = Application.Top;
			// Creates the top-level window to show
			var win = new Window("MyApp")
			{
				X = 0,
				Y = 1, // Leave one row for the toplevel menu

				// By using Dim.Fill(), it will automatically resize without manual intervention
				Width = Dim.Fill(),
				Height = Dim.Fill()
			};
			top.Add(win);

			// Creates a menubar, the item "New" has a help menu.
			var menu = new MenuBar(new MenuBarItem[] {
			new MenuBarItem ("_DataLayer", new MenuItem [] {
				new MenuItem ("_Players", "Shows players information", () => { CurrentPage = Page.Players;  }),
				new MenuItem ("_Enemies", "Shows enemies information", () => { CurrentPage = Page.Enemies; }),
				new MenuItem ("_Items", "Shows items information", () => { CurrentPage = Page.Items; }),
				new MenuItem ("_Projectiles", "Shows projectiles information", () => { CurrentPage = Page.Projectiles; }),
				new MenuItem ("_Props", "Shows props information", () => { CurrentPage = Page.Props; }),
				new MenuItem ("_Quit", "", () => { top.Running = false;/*if (Quit ()) top.Running = false;*/ })
			})
		});
			top.Add(menu);

			var infoLabel = new Label("General Information. ") { X = 1, Y = 2, Height = 40 };
			var suggestionTextbox = new TextField("Hub search...") {  X = 1, Y = 20 };
			win.Add(infoLabel, suggestionTextbox);

			//var login = new Label("Login: ") { X = 3, Y = 2 };
			//var password = new Label("Password: ")
			//{
			//	X = Pos.Left(login),
			//	Y = Pos.Top(login) + 1
			//};
			//var loginText = new TextField("")
			//{
			//	X = Pos.Right(password),
			//	Y = Pos.Top(login),
			//	Width = 40
			//};
			//var passText = new TextField("")
			//{
			//	Secret = true,
			//	X = Pos.Left(loginText),
			//	Y = Pos.Top(password),
			//	Width = Dim.Width(loginText)
			//};

			// Add some controls, 
			//win.Add(
			//	// The ones with my favorite layout system
			//	login, password, loginText, passText,

			//		// The ones laid out like an australopithecus, with absolute positions:
			//		new CheckBox(3, 6, "Remember me"),
			//		new RadioGroup(3, 8, new[] { "_Personal", "_Company" }),
			//		new Button(3, 14, "Ok"),
			//		new Button(10, 14, "Cancel"),
			//		new Label(3, 18, "Press F9 or ESC plus 9 to activate the menubar"));

			Thread lol = new Thread(() => {

				while (true)
				{
					dataLayerWorker.ProcessIncomingMessages();
					if(!string.IsNullOrWhiteSpace(suggestionTextbox.Text.ToString()))
                    {
						dataLayerWorker.Suggestion = suggestionTextbox.Text.ToString();
						if(!string.IsNullOrWhiteSpace(dataLayerWorker.Result))
							infoLabel.Text = dataLayerWorker.Result;
						continue;
					}

					if (dataLayerWorker.DataLayer != null)
                    {
                        switch (CurrentPage)
                        {
                            case Page.Players:
								infoLabel.Text = string.Join(System.Environment.NewLine, dataLayerWorker.DataLayer?.Players?.ToList().OrderByDescending(e => e.Id).Select(e => e.Id));
								break;
                            case Page.Enemies:
								infoLabel.Text = string.Join(System.Environment.NewLine, dataLayerWorker.DataLayer?.Enemies?.ToList().OrderByDescending(e => e.Id).Select(e => e.Id));
								break;
                            case Page.Items:
								infoLabel.Text = string.Join(System.Environment.NewLine, dataLayerWorker.DataLayer?.Items?.ToList().OrderByDescending(e => e.Id).Select(e => e.Id));
								break;
                            case Page.Projectiles:
								infoLabel.Text = string.Join(System.Environment.NewLine, dataLayerWorker.DataLayer?.Projectiles?.ToList().OrderByDescending(e => e.DisplayName).Select(e => e.DisplayName + " owner: " + e.OwnerName));
								break;
                            case Page.Props:
								infoLabel.Text = string.Join(System.Environment.NewLine, dataLayerWorker.DataLayer?.GeneralCharacter?.ToList().OrderByDescending(e => e.Id).Select(e => e.Id));
								break;
                        }
					}

                }
            }) { IsBackground = true };
			lol.Start();
			Application.Run();


        }
    }
}
