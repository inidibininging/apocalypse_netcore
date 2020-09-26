using System;
using System.Collections.Generic;
using System.Linq;
using Apocalypse.Any.Domain.Common.Model.RPG;

namespace Apocalypse.Any.Infrastructure.Server.Language
{
    public class Lexicon
    {
        public readonly List<char> Empty = new List<char>() { char.MinValue };
        public readonly List<char> Space = new List<char>(){ ' ' };
        public readonly List<char> Separator = System.Environment.NewLine.ToList();
        public readonly List<char> Carriage = new List<char>() { '\r' };
        public readonly List<char> LineFeed = new List<char>() { '\n' };
        public readonly List<char> PositiveSign = new List<char>() { '+' };
        public readonly List<char> NegativeSign = new List<char>() { '-' };
        public readonly List<char> Assignment = new List<char>() { '=' };
        public readonly List<char> Modify = "Mod".ToList();
        public readonly List<char> Set = "Set".ToList();
        public readonly List<char> Attribute = "Attribute".ToList();
        public readonly List<char> Stats = "Stats".ToList();
        public readonly List<char> Position = "Position".ToList();
        public readonly List<char> Rotation = "Rotation".ToList();
        public readonly List<char> Scale = "Scale".ToList();
        public readonly List<char> Alpha = "Alpha".ToList();
        public readonly List<char> Color = "Color".ToList();
        public readonly List<char> ExecuteAttribute = "!".ToList();
        public readonly List<char> CreateAttribute = "@>".ToList();
        public readonly List<char> DestroyAttribute = "<@".ToList();
        public readonly List<char> ClassNameForAttributes = typeof(CharacterSheet).FullName.ToList();
        public readonly List<char> EntityIdentifier = "#".ToList();
        public readonly List<char> TagIdentifier = ".".ToList();
        public readonly List<char> FunctionIdentifier = ":".ToList();
        public readonly List<char> MillisecondsAttribute = "Milliseconds".ToList();
        public readonly List<char> SecondsAttribute = "Seconds".ToList();
        public readonly List<char> MinutesAttribute = "Minutes".ToList();
        public readonly List<char> HoursAttribute = "Hours".ToList();
        public readonly List<char> WaitAttribute = "Wait".ToList();
        public readonly List<char> EveryAttribute = "Every".ToList();
        public readonly List<char> XAttribute = "X".ToList();
        public readonly List<char> YAttribute = "Y".ToList();
        public readonly List<char> GroupBegin = "(".ToList();
        public readonly List<char> GroupEnd = ")".ToList();
        public readonly List<char> ArgumentSeparator = ",".ToList();
        
        private readonly Dictionary<List<char>,LexiconSymbol> SymbolTable = new Dictionary<List<char>, LexiconSymbol>();

        private void InitializeSymbolTable()
        {
            SymbolTable.Clear();
            SymbolTable.Add(Space,LexiconSymbol.SkipMaterial);
            SymbolTable.Add(Separator,LexiconSymbol.SkipMaterial);
            SymbolTable.Add(Empty, LexiconSymbol.SkipMaterial);
            SymbolTable.Add(Carriage, LexiconSymbol.SkipMaterial);
            SymbolTable.Add(LineFeed, LexiconSymbol.SkipMaterial);
            SymbolTable.Add(PositiveSign,LexiconSymbol.PositiveSign);
            SymbolTable.Add(NegativeSign,LexiconSymbol.NegativeSign);
            
            SymbolTable.Add(GroupBegin,LexiconSymbol.GroupBegin);
            SymbolTable.Add(GroupEnd,LexiconSymbol.GroupEnd);
            SymbolTable.Add(ArgumentSeparator,LexiconSymbol.ArgumentSeparator);
            SymbolTable.Add(Modify,LexiconSymbol.Modify);
            SymbolTable.Add(Set, LexiconSymbol.Set);

            SymbolTable.Add(Scale,LexiconSymbol.Scale);
            SymbolTable.Add(Position,LexiconSymbol.Position);
            SymbolTable.Add(Rotation,LexiconSymbol.Rotation);
            SymbolTable.Add(Color,LexiconSymbol.Color);
            SymbolTable.Add(Stats, LexiconSymbol.Stats);

            SymbolTable.Add(CreateAttribute, LexiconSymbol.Create);
            SymbolTable.Add(DestroyAttribute,LexiconSymbol.Destroy);

            SymbolTable.Add(ClassNameForAttributes,LexiconSymbol.Entity);
            SymbolTable.Add(EntityIdentifier,LexiconSymbol.EntityIdentifier);
            SymbolTable.Add(TagIdentifier,LexiconSymbol.TagIdentifier);
            SymbolTable.Add(FunctionIdentifier,LexiconSymbol.FunctionIdentifier);
            SymbolTable.Add(ExecuteAttribute,LexiconSymbol.Execute);
            SymbolTable.Add(MillisecondsAttribute,LexiconSymbol.Milliseconds);
            SymbolTable.Add(SecondsAttribute,LexiconSymbol.Seconds);
            SymbolTable.Add(MinutesAttribute,LexiconSymbol.Minutes);
            SymbolTable.Add(HoursAttribute,LexiconSymbol.Hours);
            SymbolTable.Add(WaitAttribute,LexiconSymbol.Wait);
            SymbolTable.Add(EveryAttribute,LexiconSymbol.Every);
            SymbolTable.Add(XAttribute, LexiconSymbol.Attribute);
            SymbolTable.Add(YAttribute, LexiconSymbol.Attribute);
            typeof(CharacterSheet).GetProperties().ToList().ForEach(p => SymbolTable.Add(p.Name.ToList(),LexiconSymbol.Attribute));
        }
        public Lexicon()
        {
            InitializeSymbolTable();
        }

        public LexiconSymbol FindLexiconSymbol(List<char> token)
        {
            var possibleResult =  SymbolTable
                    .Where(symbolKeyPair => symbolKeyPair.Key.Count == token.Count &&
                                            symbolKeyPair.Key.SequenceEqual(token));
            if (!possibleResult.Any())
                return LexiconSymbol.NotFound;
            else
                return possibleResult.First().Value;
        }
    }
}
