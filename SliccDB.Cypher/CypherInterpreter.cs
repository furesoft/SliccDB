﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Newtonsoft.Json;
using SliccDB.Core;
using SliccDB.Cypher.Listeners;
using SliccDB.Serialization;

namespace SliccDB.Cypher
{
    public class CypherInterpreter
    {
        private DatabaseConnection _databaseConnection;
        public IParseTree debugTreeInfo { get; set; }
        public CypherInterpreter(DatabaseConnection databaseConnection)
        {
            _databaseConnection = databaseConnection;
        }

        public QueryResult Interpret(string fileName)
        {
            if (_databaseConnection.ConnectionStatus == ConnectionStatus.NotConnected) return null;
            using (StreamReader fileStream = new StreamReader(fileName))
            {
                AntlrInputStream inputStream = new AntlrInputStream(fileStream);

                CypherLexer lexer = new CypherLexer(inputStream);
                CommonTokenStream commonTokenStream = new CommonTokenStream(lexer);
                CypherParser parser = new CypherParser(commonTokenStream);
                CustomCypherListener listener = new CustomCypherListener();
                listener.CypherQueryResult = new QueryResult();
                listener.CurrentDatabaseConnection = _databaseConnection;

                ParseTreeWalker.Default.Walk(listener, parser.oC_Cypher());
                using (StreamReader fileStreamx = new StreamReader(fileName))
                {
                    AntlrInputStream inputStreamx = new AntlrInputStream(fileStreamx);

                    CypherLexer lexerx = new CypherLexer(inputStreamx);
                    CommonTokenStream commonTokenStreamx = new CommonTokenStream(lexerx);
                    CypherParser parserx = new CypherParser(commonTokenStreamx);
                    debugTreeInfo = parserx.oC_Cypher();
                }

                return listener.CypherQueryResult;

            }
        }
    }


}
