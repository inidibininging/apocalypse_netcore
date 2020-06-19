using System;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Threading.Tasks;


namespace Apocalypse.Any.GameServer.Config.ViewModel
{
    public class StatesViewModel
    {
        private ObservableCollection<string> states;
        public ObservableCollection<string> States
        {
            get
            {
                return new ObservableCollection<string>();
            }
            set
            {
                states = value;
                // this.RaiseAndSetIfChanged(ref states,value);
            }
        }
        public CLIServerConnector Connector {get; set;} = new CLIServerConnector();
        // public string Port { get; set; }
        // public string ServerIP { get; set; }
#region Commands        
        // public ReactiveCommand<string, int> ConnectCommand { get; }
        public int Connect(string input)
        {
            Connector.Initialize();
            return 0;
        }
#endregion
        public StatesViewModel()
        {
            // ConnectCommand = ReactiveCommand.Create<string,int>(Connect);
        }
    }
}
