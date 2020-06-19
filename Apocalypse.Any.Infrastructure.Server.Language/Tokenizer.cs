using System;
using System.Collections.Generic;
using Apocalypse.Any.Domain.Common.Model.RPG;
using Apocalypse.Any.Core.Utilities;
using System.Linq;
using System.Reflection;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.ObjectModel;

namespace Apocalypse.Any.Infrastructure.Server.Language
{
    public class Tokenizer : IEnumerator<LexiconSymbol>
    {
        private Lexicon LanguageTokens { get; set; } = new Lexicon();
        private List<char> CurrentTokenBuffer { get; set; }
        public ReadOnlyCollection<char> CurrenBufferRaw => CurrentTokenBuffer.AsReadOnly();
        public string CurrentBuffer => string.Join("", CurrentTokenBuffer);

        private LexiconSymbol CurrentSymbol {get; set; }
        public LexiconSymbol Current => CurrentSymbol;

        object IEnumerator.Current => CurrentSymbol;

        private StreamReader TokenStreamReader { get; set; }
        public Tokenizer(StreamReader tokenStreamReader)
        {
            TokenStreamReader = tokenStreamReader ?? throw new ArgumentNullException(nameof(tokenStreamReader));
            CurrentSymbol = LexiconSymbol.NotFound;
        }
        private void InitlializeTokenBufferIfEmpty(){
            if(CurrentTokenBuffer == null)
                CurrentTokenBuffer = new List<char>();
        }
        public bool MoveNext()
        {
            InitlializeTokenBufferIfEmpty();
            if(CurrentSymbol == LexiconSymbol.SkipMaterial ||
               CurrentSymbol == LexiconSymbol.NA ){
                   CurrentTokenBuffer.Clear();
            }

            while(!TokenStreamReader.EndOfStream){
                var rawCharacter = TokenStreamReader.Read();
                var convertedCharacter = Convert.ToChar(rawCharacter);
                CurrentTokenBuffer.Add(convertedCharacter);

                var nextSymbol = LanguageTokens.FindLexiconSymbol(CurrentTokenBuffer);

                //Console.ForegroundColor = ConsoleColor.Magenta;
                //Console.WriteLine(Enum.GetName(typeof(LexiconSymbol), nextSymbol));
                //Console.ForegroundColor = ConsoleColor.Yellow;

                //Function stuff
                if (CurrentSymbol == LexiconSymbol.FunctionIdentifier &&
                    nextSymbol == LexiconSymbol.NotFound &&
                    char.IsLetter(convertedCharacter))
                {
                    nextSymbol = LexiconSymbol.FunctionLetter;
                }
                if (CurrentSymbol == LexiconSymbol.Execute &&
                    nextSymbol == LexiconSymbol.NotFound &&
                    char.IsLetter(convertedCharacter))
                {
                    nextSymbol = LexiconSymbol.FunctionLetter;
                }
                if (CurrentSymbol == LexiconSymbol.FunctionLetter &&
                    nextSymbol == LexiconSymbol.NotFound &&
                    char.IsLetter(convertedCharacter))
                {
                    nextSymbol = LexiconSymbol.FunctionLetter;
                }

                //Factions
                if (CurrentSymbol == LexiconSymbol.FactionIdentifier &&
                    nextSymbol == LexiconSymbol.NotFound &&
                    char.IsLetter(convertedCharacter))
                {
                    nextSymbol = LexiconSymbol.FactionLetter;
                }

                if (CurrentSymbol == LexiconSymbol.FactionLetter &&
                    nextSymbol == LexiconSymbol.NotFound &&
                    char.IsLetter(convertedCharacter))
                {
                    nextSymbol = LexiconSymbol.FactionLetter;
                }

                //Entities
                if (CurrentSymbol == LexiconSymbol.EntityIdentifier &&
                    nextSymbol == LexiconSymbol.NotFound &&
                    char.IsLetterOrDigit(convertedCharacter))
                {
                    nextSymbol = LexiconSymbol.EntityLetter;
                }
                if (CurrentSymbol == LexiconSymbol.EntityLetter &&
                    nextSymbol == LexiconSymbol.NotFound &&
                    char.IsLetterOrDigit(convertedCharacter))
                {
                    nextSymbol = LexiconSymbol.EntityLetter;
                }

                //Create
                if (CurrentSymbol == LexiconSymbol.Create &&
                    nextSymbol == LexiconSymbol.NotFound &&
                    char.IsLetterOrDigit(convertedCharacter))
                {
                    nextSymbol = LexiconSymbol.CreatorLetter;
                }

                if (CurrentSymbol == LexiconSymbol.CreatorLetter &&
                    nextSymbol == LexiconSymbol.NotFound &&
                    char.IsLetterOrDigit(convertedCharacter))
                {
                    nextSymbol = LexiconSymbol.CreatorLetter;
                }

                //Destroy
                //if (CurrentSymbol == LexiconSymbol.Destroy &&
                //    nextSymbol == LexiconSymbol.NotFound &&
                //    char.IsLetterOrDigit(convertedCharacter))
                //{
                //    nextSymbol = LexiconSymbol.DestroyerLetter;
                //}

                //if (CurrentSymbol == LexiconSymbol.DestroyerLetter &&
                //    nextSymbol == LexiconSymbol.NotFound &&
                //    char.IsLetterOrDigit(convertedCharacter))
                //{
                //    nextSymbol = LexiconSymbol.DestroyerLetter;
                //}

                //End Entity
                // if (CurrentSymbol == LexiconSymbol.EntityLetter &&
                //     nextSymbol == LexiconSymbol.NotFound &&
                //     !char.IsLetterOrDigit(convertedCharacter))
                // {
                //     nextSymbol = LexiconSymbol.Entity;
                // }
                //  //End Function
                // if (CurrentSymbol == LexiconSymbol.EntityLetter &&
                //     nextSymbol == LexiconSymbol.NotFound &&
                //     !char.IsLetter(convertedCharacter))
                // {
                //     nextSymbol = LexiconSymbol.Function;
                // }

                if (CurrentSymbol == LexiconSymbol.Number &&
                    nextSymbol == LexiconSymbol.NotFound &&
                    char.IsDigit(convertedCharacter))
                {
                    nextSymbol = LexiconSymbol.Number;
                }

                if ((CurrentSymbol == LexiconSymbol.PositiveSign ||
                    CurrentSymbol == LexiconSymbol.NegativeSign) &&
                    nextSymbol == LexiconSymbol.NotFound &&
                    char.IsDigit(convertedCharacter))
                {
                    nextSymbol = LexiconSymbol.Number;
                }

                CurrentSymbol = nextSymbol;

                var mayStopSymbol = LanguageTokens.FindLexiconSymbol(new List<char>() { convertedCharacter });
                if(mayStopSymbol == LexiconSymbol.SkipMaterial)
                {
                    CurrentSymbol = mayStopSymbol;
                }

                // if(CurrentSymbol != LexiconSymbol.NotFound)
                // {
                //     Console.ForegroundColor = ConsoleColor.Green;
                //     CurrentTokenBuffer.ToList().ForEach(c => Console.WriteLine(c));
                //     Console.ForegroundColor = ConsoleColor.White;
                // }

                if(CurrentSymbol != LexiconSymbol.NA)
                {
                    return true;
                }
            }
            return false;
        }

        public void Reset()
        {
            TokenStreamReader.BaseStream.Seek(0, SeekOrigin.Begin);
        }

        public void Dispose()
        {
            TokenStreamReader.Dispose();
            TokenStreamReader = null;
            LanguageTokens = null;
            CurrentTokenBuffer?.Clear();
        }
    }
}
