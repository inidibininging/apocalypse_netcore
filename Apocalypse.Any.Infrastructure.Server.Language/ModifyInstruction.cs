using System;
using System.Collections.Generic;
using System.Linq;
using Apocalypse.Any.Domain.Common.Model;
using Apocalypse.Any.Domain.Common.Model.Network;
using Apocalypse.Any.Domain.Common.Model.RPG;
using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;
using States.Core.Infrastructure.Services;

namespace Apocalypse.Any.Infrastructure.Server.Language
{
    public class ModifyInstruction : AbstractInterpreterInstruction
    {
        public ModifyInstruction(Interpreter interpreter, ModifyAttributeExpression modifyExpression) : base(interpreter)
        {
            Console.WriteLine("adding mod instruction");
            ModifyExpression = modifyExpression;
        }
        private ModifyAttributeExpression ModifyExpression { get; set; }

        public override void Handle(IStateMachine<string, IGameSectorLayerService> machine)
        {
            if (ModifyExpression.Identifier is EntityExpression)
                HandleEntity(machine);
            if (ModifyExpression.Identifier is FactionExpression)
                HandleFaction(machine);
        }
        
        private void HandleCharacter(CharacterEntity character)
        {
            if (ModifyExpression.Section.Name == "Position" ||
               ModifyExpression.Section.Name == "Alpha") // heap values
            {
                SetPropertyForImageData(character.CurrentImage);
                return;
            }
            if (ModifyExpression.Section.Name == "Color")
            {
                ColorStrategy(character);
                return;
            }
            if (ModifyExpression.Section.Name == "Scale")
            {
                ScaleStrategy(character);
                return;
            }

            if (ModifyExpression.Section.Name == "Stats")
            {
                SetStatsCharacterProperty(character.Stats);
                return;
            }
        }
        private void HandleEntity(IStateMachine<string, IGameSectorLayerService> machine)
        {
            var getByName = new Func<CharacterEntity, bool>((entity) => entity.DisplayName == ModifyExpression.Identifier.Name);
            var selectedCharacter = GetAllValidCharacters(machine).FirstOrDefault(getByName);
            HandleCharacter(selectedCharacter);
        }
        private void SetPropertyForImageData(ImageData imageData) 
        {
            var propertyInfo = imageData.GetType().GetProperty(ModifyExpression.Section.Name);
            var property = propertyInfo.GetValue(imageData);
            var attribute = property.GetType().GetProperty(ModifyExpression.Attribute.Name);
            attribute.SetValue(property, ModifyExpression.Number.NumberValue * ModifyExpression.SignConverter.Polarity);
        }
        private void SetStatsCharacterProperty(CharacterSheet sheet)
        {
            var propertyInfo = sheet.GetType().GetProperty(ModifyExpression.Attribute.Name);
            var property = propertyInfo.GetValue(sheet);
            propertyInfo.SetValue(sheet, ModifyExpression.Number.NumberValue * ModifyExpression.SignConverter.Polarity);
        }

        //private void PositionStrategy(CharacterEntity character)
        //{
        //    var subject = character.CurrentImage.Position;
        //    subject.GetType().GetProperty(ModifyExpression.Attribute.Name).SetValue(subject, ModifyExpression.Number.NumberValue * ModifyExpression.SignConverter.Polarity);
        //}


        private void ScaleStrategy(CharacterEntity character)
        {
            // I have to do this due to the value type.
            var valueToPass = ModifyExpression.Number.NumberValue * ModifyExpression.SignConverter.Polarity;
            if (!valueToPass.HasValue)
                return;
            if (ModifyExpression.Attribute.Name == "X")
                character.CurrentImage.Scale = new Microsoft.Xna.Framework.Vector2(valueToPass.Value, character.CurrentImage.Scale.Y);
            if (ModifyExpression.Attribute.Name == "Y")
                character.CurrentImage.Scale = new Microsoft.Xna.Framework.Vector2(character.CurrentImage.Scale.X, valueToPass.Value);
        }

        private void ColorStrategy(CharacterEntity character)        
        {
            var valueToPass = ModifyExpression.Number.NumberValue * ModifyExpression.SignConverter.Polarity;
            if (!valueToPass.HasValue)
                return;

            if (ModifyExpression.Attribute.Name == "R")
                character.CurrentImage.Color = new  Microsoft.Xna.Framework.Color(
                                                    (int)valueToPass,
                                                    character.CurrentImage.Color.G,
                                                    character.CurrentImage.Color.B,
                                                    character.CurrentImage.Color.A);
            if (ModifyExpression.Attribute.Name == "G")
                character.CurrentImage.Color = new Microsoft.Xna.Framework.Color(
                                                    character.CurrentImage.Color.R,
                                                     (int)valueToPass,
                                                    character.CurrentImage.Color.B,
                                                    character.CurrentImage.Color.A);
            if (ModifyExpression.Attribute.Name == "B")
                character.CurrentImage.Color = new Microsoft.Xna.Framework.Color(
                                                    character.CurrentImage.Color.R,
                                                    character.CurrentImage.Color.G,
                                                     (int)valueToPass,
                                                    character.CurrentImage.Color.A);
            if (ModifyExpression.Attribute.Name == "A")
                character.CurrentImage.Color = new Microsoft.Xna.Framework.Color(
                                                    character.CurrentImage.Color.R,
                                                    character.CurrentImage.Color.G,
                                                    character.CurrentImage.Color.B,
                                                     (int)valueToPass);
        }

        private void ApplyChangeToEntity<T>(T obj)
        {
            obj
            .GetType()
            .GetProperty(ModifyExpression.Attribute.Name)
            .SetValue(obj, ModifyExpression.Number.NumberValue.Value);
        }
        private void HandleFaction(IStateMachine<string, IGameSectorLayerService> machine)
        {
            var belongsToFaction = new Func<CharacterEntity, bool>(entity => entity.Tags != null && entity.Tags.Any(factionName => factionName == ModifyExpression.Identifier.Name));
            var characters = GetAllValidCharacters(machine).Where(belongsToFaction);
            if (characters.Count() != 0)
            {
                for (var index = 0; index < characters.Count(); index++)
                {
                    var someEntity = characters.ElementAt(index);
                    HandleCharacter(someEntity);
                    //positions
                    //.GetType()
                    //.GetProperty(ModifyExpression.Attribute.Name)
                    //.SetValue(someEntity, ModifyExpression.Number.NumberValue.Value);
                }
            }
        }
        private static IEnumerable<CharacterEntity> GetAllValidCharacters(IStateMachine<string, IGameSectorLayerService> machine) =>
            machine.SharedContext.DataLayer.Players.Cast<CharacterEntity>()
            .Concat(machine.SharedContext.DataLayer.Enemies.Cast<CharacterEntity>())
            .Concat(machine.SharedContext.DataLayer.Items.Cast<CharacterEntity>())
            .Concat(machine.SharedContext.DataLayer.GeneralCharacter.Cast<CharacterEntity>());


    }
}
