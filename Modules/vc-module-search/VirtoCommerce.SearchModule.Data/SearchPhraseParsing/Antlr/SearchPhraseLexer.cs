//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     ANTLR Version: 4.7
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from SearchPhrase.g4 by ANTLR 4.7

// Unreachable code detected
#pragma warning disable 0162
// The variable '...' is assigned but its value is never used
#pragma warning disable 0219
// Missing XML comment for publicly visible type or member '...'
#pragma warning disable 1591
// Ambiguous reference in cref attribute
#pragma warning disable 419

namespace VirtoCommerce.SearchModule.Data.Search.SearchPhraseParsing.Antlr
{
    using System;
    using System.IO;
    using Antlr4.Runtime;
    using Antlr4.Runtime.Atn;
    using Antlr4.Runtime.Misc;
    using DFA = Antlr4.Runtime.Dfa.DFA;

    [System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.7")]
    public partial class SearchPhraseLexer : Lexer
    {
        protected static DFA[] decisionToDFA;
        protected static PredictionContextCache sharedContextCache = new PredictionContextCache();
        public const int
            FD = 1, VD = 2, RD = 3, RangeStart = 4, RangeEnd = 5, String = 6, WS = 7;
        public static string[] channelNames = {
        "DEFAULT_TOKEN_CHANNEL", "HIDDEN"
    };

        public static string[] modeNames = {
        "DEFAULT_MODE"
    };

        public static readonly string[] ruleNames = {
        "FD", "VD", "RD", "RangeStart", "RangeEnd", "String", "SimpleString",
        "QuotedString", "Esc", "WS"
    };


        public SearchPhraseLexer(ICharStream input)
        : this(input, Console.Out, Console.Error) { }

        public SearchPhraseLexer(ICharStream input, TextWriter output, TextWriter errorOutput)
        : base(input, output, errorOutput)
        {
            Interpreter = new LexerATNSimulator(this, _ATN, decisionToDFA, sharedContextCache);
        }

        private static readonly string[] _LiteralNames = {
        null, "':'", "','"
    };
        private static readonly string[] _SymbolicNames = {
        null, "FD", "VD", "RD", "RangeStart", "RangeEnd", "String", "WS"
    };
        public static readonly IVocabulary DefaultVocabulary = new Vocabulary(_LiteralNames, _SymbolicNames);

        [NotNull]
        public override IVocabulary Vocabulary
        {
            get
            {
                return DefaultVocabulary;
            }
        }

        public override string GrammarFileName { get { return "SearchPhrase.g4"; } }

        public override string[] RuleNames { get { return ruleNames; } }

        public override string[] ChannelNames { get { return channelNames; } }

        public override string[] ModeNames { get { return modeNames; } }

        public override string SerializedAtn { get { return new string(_serializedATN); } }

        static SearchPhraseLexer()
        {
            decisionToDFA = new DFA[_ATN.NumberOfDecisions];
            for (int i = 0; i < _ATN.NumberOfDecisions; i++)
            {
                decisionToDFA[i] = new DFA(_ATN.GetDecisionState(i), i);
            }
        }
        private static char[] _serializedATN = {
        '\x3', '\x608B', '\xA72A', '\x8133', '\xB9ED', '\x417C', '\x3BE7', '\x7786',
        '\x5964', '\x2', '\t', '@', '\b', '\x1', '\x4', '\x2', '\t', '\x2', '\x4',
        '\x3', '\t', '\x3', '\x4', '\x4', '\t', '\x4', '\x4', '\x5', '\t', '\x5',
        '\x4', '\x6', '\t', '\x6', '\x4', '\a', '\t', '\a', '\x4', '\b', '\t',
        '\b', '\x4', '\t', '\t', '\t', '\x4', '\n', '\t', '\n', '\x4', '\v', '\t',
        '\v', '\x3', '\x2', '\x3', '\x2', '\x3', '\x3', '\x3', '\x3', '\x3', '\x4',
        '\x3', '\x4', '\x3', '\x4', '\x3', '\x4', '\x5', '\x4', ' ', '\n', '\x4',
        '\x3', '\x5', '\x3', '\x5', '\x3', '\x6', '\x3', '\x6', '\x3', '\a', '\x3',
        '\a', '\x5', '\a', '(', '\n', '\a', '\x3', '\b', '\x6', '\b', '+', '\n',
        '\b', '\r', '\b', '\xE', '\b', ',', '\x3', '\t', '\x3', '\t', '\x3', '\t',
        '\a', '\t', '\x32', '\n', '\t', '\f', '\t', '\xE', '\t', '\x35', '\v',
        '\t', '\x3', '\t', '\x3', '\t', '\x3', '\n', '\x3', '\n', '\x3', '\n',
        '\x3', '\v', '\x6', '\v', '=', '\n', '\v', '\r', '\v', '\xE', '\v', '>',
        '\x2', '\x2', '\f', '\x3', '\x3', '\x5', '\x4', '\a', '\x5', '\t', '\x6',
        '\v', '\a', '\r', '\b', '\xF', '\x2', '\x11', '\x2', '\x13', '\x2', '\x15',
        '\t', '\x3', '\x2', '\b', '\x4', '\x2', '*', '*', ']', ']', '\x4', '\x2',
        '+', '+', '_', '_', '\n', '\x2', '\v', '\v', '\"', '\"', '$', '$', '*',
        '+', '.', '.', '<', '<', ']', ']', '_', '_', '\x4', '\x2', '$', '$', '^',
        '^', '\a', '\x2', '$', '$', '^', '^', 'p', 'p', 't', 't', 'v', 'v', '\x4',
        '\x2', '\v', '\v', '\"', '\"', '\x2', '\x42', '\x2', '\x3', '\x3', '\x2',
        '\x2', '\x2', '\x2', '\x5', '\x3', '\x2', '\x2', '\x2', '\x2', '\a', '\x3',
        '\x2', '\x2', '\x2', '\x2', '\t', '\x3', '\x2', '\x2', '\x2', '\x2', '\v',
        '\x3', '\x2', '\x2', '\x2', '\x2', '\r', '\x3', '\x2', '\x2', '\x2', '\x2',
        '\x15', '\x3', '\x2', '\x2', '\x2', '\x3', '\x17', '\x3', '\x2', '\x2',
        '\x2', '\x5', '\x19', '\x3', '\x2', '\x2', '\x2', '\a', '\x1F', '\x3',
        '\x2', '\x2', '\x2', '\t', '!', '\x3', '\x2', '\x2', '\x2', '\v', '#',
        '\x3', '\x2', '\x2', '\x2', '\r', '\'', '\x3', '\x2', '\x2', '\x2', '\xF',
        '*', '\x3', '\x2', '\x2', '\x2', '\x11', '.', '\x3', '\x2', '\x2', '\x2',
        '\x13', '\x38', '\x3', '\x2', '\x2', '\x2', '\x15', '<', '\x3', '\x2',
        '\x2', '\x2', '\x17', '\x18', '\a', '<', '\x2', '\x2', '\x18', '\x4',
        '\x3', '\x2', '\x2', '\x2', '\x19', '\x1A', '\a', '.', '\x2', '\x2', '\x1A',
        '\x6', '\x3', '\x2', '\x2', '\x2', '\x1B', '\x1C', '\a', 'V', '\x2', '\x2',
        '\x1C', ' ', '\a', 'Q', '\x2', '\x2', '\x1D', '\x1E', '\a', 'v', '\x2',
        '\x2', '\x1E', ' ', '\a', 'q', '\x2', '\x2', '\x1F', '\x1B', '\x3', '\x2',
        '\x2', '\x2', '\x1F', '\x1D', '\x3', '\x2', '\x2', '\x2', ' ', '\b', '\x3',
        '\x2', '\x2', '\x2', '!', '\"', '\t', '\x2', '\x2', '\x2', '\"', '\n',
        '\x3', '\x2', '\x2', '\x2', '#', '$', '\t', '\x3', '\x2', '\x2', '$',
        '\f', '\x3', '\x2', '\x2', '\x2', '%', '(', '\x5', '\xF', '\b', '\x2',
        '&', '(', '\x5', '\x11', '\t', '\x2', '\'', '%', '\x3', '\x2', '\x2',
        '\x2', '\'', '&', '\x3', '\x2', '\x2', '\x2', '(', '\xE', '\x3', '\x2',
        '\x2', '\x2', ')', '+', '\n', '\x4', '\x2', '\x2', '*', ')', '\x3', '\x2',
        '\x2', '\x2', '+', ',', '\x3', '\x2', '\x2', '\x2', ',', '*', '\x3', '\x2',
        '\x2', '\x2', ',', '-', '\x3', '\x2', '\x2', '\x2', '-', '\x10', '\x3',
        '\x2', '\x2', '\x2', '.', '\x33', '\a', '$', '\x2', '\x2', '/', '\x32',
        '\x5', '\x13', '\n', '\x2', '\x30', '\x32', '\n', '\x5', '\x2', '\x2',
        '\x31', '/', '\x3', '\x2', '\x2', '\x2', '\x31', '\x30', '\x3', '\x2',
        '\x2', '\x2', '\x32', '\x35', '\x3', '\x2', '\x2', '\x2', '\x33', '\x31',
        '\x3', '\x2', '\x2', '\x2', '\x33', '\x34', '\x3', '\x2', '\x2', '\x2',
        '\x34', '\x36', '\x3', '\x2', '\x2', '\x2', '\x35', '\x33', '\x3', '\x2',
        '\x2', '\x2', '\x36', '\x37', '\a', '$', '\x2', '\x2', '\x37', '\x12',
        '\x3', '\x2', '\x2', '\x2', '\x38', '\x39', '\a', '^', '\x2', '\x2', '\x39',
        ':', '\t', '\x6', '\x2', '\x2', ':', '\x14', '\x3', '\x2', '\x2', '\x2',
        ';', '=', '\t', '\a', '\x2', '\x2', '<', ';', '\x3', '\x2', '\x2', '\x2',
        '=', '>', '\x3', '\x2', '\x2', '\x2', '>', '<', '\x3', '\x2', '\x2', '\x2',
        '>', '?', '\x3', '\x2', '\x2', '\x2', '?', '\x16', '\x3', '\x2', '\x2',
        '\x2', '\t', '\x2', '\x1F', '\'', ',', '\x31', '\x33', '>', '\x2',
    };

        public static readonly ATN _ATN =
            new ATNDeserializer().Deserialize(_serializedATN);


    }
} // namespace VirtoCommerce.CoreModule.Data.Search.SearchPhraseParsing.Antlr
