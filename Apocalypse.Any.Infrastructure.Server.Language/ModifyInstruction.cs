using System;
using System.Collections.Generic;
using System.Linq;
using Apocalypse.Any.Domain.Common.Model;
using Apocalypse.Any.Domain.Common.Model.Language;
using Apocalypse.Any.Domain.Common.Model.Network;
using Apocalypse.Any.Domain.Common.Model.RPG;
using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;
using States.Core.Infrastructure.Services;

namespace Apocalypse.Any.Infrastructure.Server.Language
{
    public class ModifyInstruction : AbstractInterpreterInstruction<ModifyAttributeExpression>
    {
        public ModifyInstruction(Interpreter interpreter, ModifyAttributeExpression modifyExpression, int functionIndex) 
            : base(interpreter, modifyExpression, functionIndex)
        {
            Console.WriteLine("adding mod instruction");
        }

        
        public override void Handle(IStateMachine<string, IGameSectorLayerService> machine)
        {
            if (Expression.Identifier is IdentifierExpression)
                HandleIdentifier(machine);
            if (Expression.Identifier is EntityExpression)
                HandleEntity(machine);
            if (Expression.Identifier is TagExpression)
                HandleFaction(machine, Expression.Identifier.Name);
        }

        private void HandleIdentifier(IStateMachine<string, IGameSectorLayerService> machine)
        {
            //get the variable out of the function scope 
            var variable = machine
                .SharedContext
                .DataLayer
                .GetLayersByType<TagVariable>()
                .FirstOrDefault()
                ?.DataAsEnumerable<TagVariable>()
                .FirstOrDefault(t => t.Name == Expression.Identifier.Name &&
                                     t.Scope == Scope?.Expression.Name);

            var lastFn = Scope;
            var identifierName = Expression.Identifier.Name;
            while (variable == null)
            {
                var argumentIndex = lastFn.GetFunctionArgumentIndex(identifierName);
                variable = lastFn
                            .LastCaller
                            .GetVariableOfFunction(machine, argumentIndex);
                if (variable != null) 
                    continue;
                identifierName = lastFn
                    .LastCaller
                    .Expression
                    .Arguments
                    .Arguments
                    .ElementAt(argumentIndex)
                    .Name;
                lastFn = lastFn.LastCaller.Scope;
                
            }
            if (variable.DataTypeSymbol != LexiconSymbol.TagDataType)
                throw new InvalidOperationException($"Syntax error. Cannot execute a modify instruction. Data type of variable is not a tag.");

            HandleFaction(machine, variable.Value);
        }
        
        private void HandleCharacter(CharacterEntity character)
        {
            if (Expression.Section.Name == "Position" ||
                Expression.Section.Name == "Alpha") // heap values
            {
                SetPropertyForImageData(character.CurrentImage);
                return;
            }
            if (Expression.Section.Name == "Color")
            {
                ColorStrategy(character);
                return;
            }
            if (Expression.Section.Name == "Scale")
            {
                ScaleStrategy(character);
                return;
            }

            if (Expression.Section.Name == "Stats")
            {
                SetStatsCharacterProperty(character.Stats);
                return;
            }
        }
        private void HandleEntity(IStateMachine<string, IGameSectorLayerService> machine)
        {
            var getByName = new Func<CharacterEntity, bool>((entity) => entity.DisplayName == Expression.Identifier.Name);
            var selectedCharacter = GetAllValidCharacters(machine).FirstOrDefault(getByName);
            HandleCharacter(selectedCharacter);
        }
        private void SetPropertyForImageData(ImageData imageData) 
        {
            var propertyInfo = imageData.GetType().GetProperty(Expression.Section.Name);
            if (propertyInfo is null) return;
            var property = propertyInfo.GetValue(imageData);
            var attribute = property.GetType().GetProperty(Expression.Attribute.Name);
            attribute?.SetValue(property, Expression.Number.NumberValue * Expression.SignConverter.Polarity);
        }
        private void SetStatsCharacterProperty(CharacterSheet sheet)
        {
            var propertyInfo = sheet.GetType().GetProperty(Expression.Attribute.Name);
            if (!(propertyInfo is null))
            {
                var property = propertyInfo.GetValue(sheet);
                propertyInfo.SetValue(sheet, Expression.Number.NumberValue * Expression.SignConverter.Polarity);
            }
        }

        //private void PositionStrategy(CharacterEntity character)
        //{
        //    var subject = character.CurrentImage.Position;
        //    subject.GetType().GetProperty(ModifyExpression.Attribute.Name).SetValue(subject, ModifyExpression.Number.NumberValue * ModifyExpression.SignConverter.Polarity);
        //}


        private void ScaleStrategy(CharacterEntity character)
        {
            // I have to do this due to the value type.
            var valueToPass = Expression.Number.NumberValue * Expression.SignConverter.Polarity;
            if (!valueToPass.HasValue)
                return;
            if (Expression.Attribute.Name == "X")
                character.CurrentImage.Scale = new Microsoft.Xna.Framework.Vector2(valueToPass.Value, character.CurrentImage.Scale.Y);
            if (Expression.Attribute.Name == "Y")
                character.CurrentImage.Scale = new Microsoft.Xna.Framework.Vector2(character.CurrentImage.Scale.X, valueToPass.Value);
        }

        private void ColorStrategy(CharacterEntity character)        
        {
            var valueToPass = Expression.Number.NumberValue * Expression.SignConverter.Polarity;
            if (!valueToPass.HasValue)
                return;

            if (Expression.Attribute.Name == "R")
                character.CurrentImage.Color = new  Microsoft.Xna.Framework.Color(
                                                    (int)valueToPass,
                                                    character.CurrentImage.Color.G,
                                                    character.CurrentImage.Color.B,
                                                    character.CurrentImage.Color.A);
            if (Expression.Attribute.Name == "G")
                character.CurrentImage.Color = new Microsoft.Xna.Framework.Color(
                                                    character.CurrentImage.Color.R,
                                                     (int)valueToPass,
                                                    character.CurrentImage.Color.B,
                                                    character.CurrentImage.Color.A);
            if (Expression.Attribute.Name == "B")
                character.CurrentImage.Color = new Microsoft.Xna.Framework.Color(
                                                    character.CurrentImage.Color.R,
                                                    character.CurrentImage.Color.G,
                                                     (int)valueToPass,
                                                    character.CurrentImage.Color.A);
            if (Expression.Attribute.Name == "A")
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
            .GetProperty(Expression.Attribute.Name)
            ?.SetValue(obj, Expression.Number.NumberValue.Value);
        }

        private Func<CharacterEntity, string, bool> CharacterWithCertainTag => new Func<CharacterEntity, string, bool>(
            (entity, tag) =>
            entity.Tags != null && entity.Tags.Any(factionName => factionName == tag));
        
        private void HandleFaction(IStateMachine<string, IGameSectorLayerService> machine, string tag)
        {
            var characters = GetAllValidCharacters(machine).Where(c => CharacterWithCertainTag(c, tag));
            var characterEntities = characters as CharacterEntity[] ?? characters.ToArray();
            if (characterEntities.Count() != 0)
            {
                for (var index = 0; index < characterEntities.Count(); index++)
                {
                    var someEntity = characterEntities.ElementAt(index);
                    HandleCharacter(someEntity);
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
