using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace Apocalypse.Any.Core.Services
{
    /// <summary>
    /// Ugly quick-n-dirty code for playing WAV sounds.
    /// </summary>
    public class WaveSoundService : GameObject
    {
        public Dictionary<string, SoundEffect> Effects { get; set; }

        //IWavePlayer WaveOutDevice { get; set; }
        private Uri Location { get; set; }

        private string Directory { get; set; }

        public WaveSoundService() : base()
        {
            Effects = new Dictionary<string, SoundEffect>();
            Location = new Uri(Assembly.GetEntryAssembly().GetName().CodeBase);
            Directory = new FileInfo(Location.AbsolutePath).Directory.FullName;
        }

        public override void LoadContent(ContentManager manager)
        {
            //var basePath = "Sound/FX";
            var sounds = new List<string>() {
                "Armor00",
                "Boom00",
                "Boom01",
                "Charge00",
                "Hit00",
                "Hit01",
                "Hit02",
                "Hit03",
                "Hit04",
                "Hit05",
                "Hit06",
                "Hit07",
                "Swoosh",
                "GameBoyDie",
                "GameBoyDeepBoom",
                "SynthLaser01",
                "SynthLaser02",
                "SynthLaser03",
                "SynthLaserLong00",
                "SynthMachinegun00",
                "SynthMachinegun01",
                "SynthMachinegun02",
                "SynthMachinegun03",
                "SynthMachinegun04",
                "SynthMachinegun05",
                "SynthShoot00",
                "SynthShoot01",
                "SynthShoot02",
                "SynthShoot03",
                "SynthHit00",
                "SynthHit01",
                "SynthShootDeep00",
                "SynthDive00",
                "SynthDive01",
                "SynthDive02",
                "SynthDive03",
                "SynthBoom00",
                "SynthBoom01",
                "SynthBoomElectro00",
                "SynthBoomElectro01",
                "SynthBoomElectro02",
                "SynthHitElectro00",
                "SynthHitElectro01",
                "SynthHitElectro02",
                "ItemGather"
            };

            sounds.ForEach(audioFileName =>
            {
                var filePath = Path.Combine(Directory, "Content", "Sound", "FX", audioFileName + ".wav");
                Console.WriteLine($"loading {filePath}");

                // if (!File.Exists(filePath)){
                //     Console.WriteLine($"{filePath} doesn't  exists...");
                // }
                // else
                //     Console.WriteLine($"{filePath}  exists...");

                //var str = SoundEffect.FromStream(File.OpenRead(filePath));
                //str.Name = Path.GetFileNameWithoutExtension(filePath);
                
                //!!!!-----
                Effects.Add(audioFileName, SoundEffect.FromStream(File.OpenRead(filePath)));
                
                // Effects.Add(audioFileName, manager.Load<SoundEffect>(filePath));
            });
            base.LoadContent(manager);
        }

        public override void UnloadContent()
        {
            foreach (var fx in Effects.Values)
                fx.Dispose();
            Effects.Clear();
            //WaveOutDevice.Dispose();
            base.UnloadContent();
        }

        public override void Update(GameTime time)
        {
        }

        /// <summary>
        /// The max. plays of audio. Somehow this likes to crash the game
        /// </summary>
        private int maxAudioPlays = 8;

        private int currentAudioPlays = 0;

        public void Play(string audioFileName)
        {
            
            if (currentAudioPlays > maxAudioPlays)
                return;

            var filePath = Path.Combine(Directory, "Content", "Sound", "FX", audioFileName + ".xnb");

            Task t = new Task(() =>
            {
                if (!Effects.ContainsKey(audioFileName))
                    return;
                if (currentAudioPlays > maxAudioPlays){
                    return;
                }

                currentAudioPlays += 1;
                Effects[audioFileName].Play();
                // using (var fx = Effects[audioFileName].CreateInstance())
                // {
                //     fx.Volume = 1f;
                //     fx.Play();
                // }

                currentAudioPlays--;
            });
            t.Start();
            
        }
    }
}